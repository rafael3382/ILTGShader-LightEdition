using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DC RID: 476
	public class SubsystemTorchBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000CF8 RID: 3320 RVA: 0x0005D5E9 File Offset: 0x0005B7E9
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					31,
					17,
					132
				};
			}
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0005D5FC File Offset: 0x0005B7FC
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			if (num != 31)
			{
				if (num != 132)
				{
					return;
				}
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (!BlocksManager.Blocks[cellContents].IsCollidable_(cellValueFast))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
			else
			{
				Point3 point = CellFace.FaceToPoint3(Terrain.ExtractData(cellValueFast));
				int x2 = x - point.X;
				int y2 = y - point.Y;
				int z2 = z - point.Z;
				int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x2, y2, z2);
				if (!BlocksManager.Blocks[cellContents2].IsCollidable_(cellValueFast))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
					return;
				}
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x0005D6CE File Offset: 0x0005B8CE
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0005D6DC File Offset: 0x0005B8DC
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveTorch(x, y, z);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0005D6E9 File Offset: 0x0005B8E9
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveTorch(x, y, z);
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0005D702 File Offset: 0x0005B902
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0005D710 File Offset: 0x0005B910
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveTorch(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0005D814 File Offset: 0x0005BA14
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0005D830 File Offset: 0x0005BA30
		public void AddTorch(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractContents(value);
			Vector3 v;
			float size;
			if (num != 31)
			{
				if (num != 132)
				{
					v = new Vector3(0.5f, 0.2f, 0.5f);
					size = 0.2f;
				}
				else
				{
					v = new Vector3(0.5f, 0.1f, 0.5f);
					size = 0.1f;
				}
			}
			else
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					v = new Vector3(0.5f, 0.58f, 0.27f);
					break;
				case 1:
					v = new Vector3(0.27f, 0.58f, 0.5f);
					break;
				case 2:
					v = new Vector3(0.5f, 0.58f, 0.73f);
					break;
				case 3:
					v = new Vector3(0.73f, 0.58f, 0.5f);
					break;
				default:
					v = new Vector3(0.5f, 0.53f, 0.5f);
					break;
				}
				size = 0.15f;
			}
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 24f);
			this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0005D974 File Offset: 0x0005BB74
		public void RemoveTorch(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem particleSystem = this.m_particleSystemsByCell[key];
			this.m_subsystemParticles.RemoveParticleSystem(particleSystem);
			this.m_particleSystemsByCell.Remove(key);
		}

		// Token: 0x040006B0 RID: 1712
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040006B1 RID: 1713
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
	}
}
