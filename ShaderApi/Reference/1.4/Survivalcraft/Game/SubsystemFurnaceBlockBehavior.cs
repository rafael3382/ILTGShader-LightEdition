using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A9 RID: 425
	public class SubsystemFurnaceBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x00048498 File Offset: 0x00046698
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					64,
					65
				};
			}
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x000484AC File Offset: 0x000466AC
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(oldValue) != 64 && Terrain.ExtractContents(oldValue) != 65)
			{
				DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("Furnace", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.PopulateFromDatabaseObject(databaseObject);
				valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
				Entity entity = base.SubsystemTerrain.Project.CreateEntity(valuesDictionary);
				base.SubsystemTerrain.Project.AddEntity(entity);
			}
			if (Terrain.ExtractContents(value) == 65)
			{
				this.AddFire(value, x, y, z);
			}
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x00048570 File Offset: 0x00046770
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (Terrain.ExtractContents(newValue) != 64 && Terrain.ExtractContents(newValue) != 65)
			{
				ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(x, y, z);
				if (blockEntity != null)
				{
					Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
					foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
					{
						inventory.DropAllItems(position);
					}
					base.SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
				}
			}
			if (Terrain.ExtractContents(value) == 65)
			{
				this.RemoveFire(x, y, z);
			}
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0004864C File Offset: 0x0004684C
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (Terrain.ExtractContents(value) == 65)
			{
				this.AddFire(value, x, y, z);
			}
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x00048664 File Offset: 0x00046864
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
				this.RemoveFire(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x00048768 File Offset: 0x00046968
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity != null && componentMiner.ComponentPlayer != null)
			{
				ComponentFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentFurnace>(true);
				componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FurnaceWidget(componentMiner.Inventory, componentFurnace);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				return true;
			}
			return false;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x000487FD File Offset: 0x000469FD
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0004880E File Offset: 0x00046A0E
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0004882C File Offset: 0x00046A2C
		public void AddFire(int value, int x, int y, int z)
		{
			Vector3 v = new Vector3(0.5f, 0.2f, 0.5f);
			float size = 0.15f;
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 16f);
			this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x00048894 File Offset: 0x00046A94
		public void RemoveFire(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem particleSystem = this.m_particleSystemsByCell[key];
			this.m_subsystemParticles.RemoveParticleSystem(particleSystem);
			this.m_particleSystemsByCell.Remove(key);
		}

		// Token: 0x0400054B RID: 1355
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400054C RID: 1356
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
	}
}
