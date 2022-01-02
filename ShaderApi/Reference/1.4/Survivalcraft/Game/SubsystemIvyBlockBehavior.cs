using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B4 RID: 436
	public class SubsystemIvyBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x0004B45B File Offset: 0x0004965B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000B34 RID: 2868 RVA: 0x0004B45E File Offset: 0x0004965E
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0004B468 File Offset: 0x00049668
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 0.0))
			{
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toUpdate)
				{
					if (base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) == 0)
					{
						base.SubsystemTerrain.ChangeCell(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z, keyValuePair.Value, true);
					}
				}
				this.m_toUpdate.Clear();
			}
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0004B550 File Offset: 0x00049750
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int face = IvyBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			bool flag = false;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 197 && IvyBlock.GetFace(Terrain.ExtractData(cellValue)) == face)
			{
				flag = true;
			}
			if (!flag)
			{
				Point3 point = CellFace.FaceToPoint3(face);
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
				if (!BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsCollidable_(cellValue2))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, true, false);
				}
			}
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0004B60C File Offset: 0x0004980C
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_random.Float(0f, 1f) < 0.5f && !IvyBlock.IsGrowthStopCell(x, y, z) && Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z)) == 0)
			{
				this.m_toUpdate[new Point3(x, y - 1, z)] = value;
			}
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0004B674 File Offset: 0x00049874
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		// Token: 0x04000570 RID: 1392
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000571 RID: 1393
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000572 RID: 1394
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();
	}
}
