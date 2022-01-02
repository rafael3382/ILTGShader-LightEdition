using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019A RID: 410
	public class SubsystemDispenserBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000A12 RID: 2578 RVA: 0x00040D81 File Offset: 0x0003EF81
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					216
				};
			}
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00040D94 File Offset: 0x0003EF94
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockEntities = base.Project.FindSubsystem<SubsystemBlockEntities>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00040DF0 File Offset: 0x0003EFF0
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.Project.GameDatabase.Database.FindDatabaseObject("Dispenser", base.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Entity entity = base.Project.CreateEntity(valuesDictionary);
			base.Project.AddEntity(entity);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00040E70 File Offset: 0x0003F070
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(x, y, z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
				foreach (IInventory inventory in blockEntity.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position);
				}
				base.Project.RemoveEntity(blockEntity.Entity, true);
			}
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00040F0C File Offset: 0x0003F10C
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure)
			{
				ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
				if (blockEntity != null && componentMiner.ComponentPlayer != null)
				{
					ComponentDispenser componentDispenser = blockEntity.Entity.FindComponent<ComponentDispenser>(true);
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new DispenserWidget(componentMiner.Inventory, componentDispenser);
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00040FAC File Offset: 0x0003F1AC
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			if (blockEntity != null && DispenserBlock.GetAcceptsDrops(Terrain.ExtractData(this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z))))
			{
				IInventory inventory = blockEntity.Entity.FindComponent<ComponentDispenser>(true);
				Pickable pickable = worldItem as Pickable;
				int num = (pickable != null) ? pickable.Count : 1;
				int num2 = ComponentInventoryBase.AcquireItems(inventory, worldItem.Value, num);
				if (num2 < num)
				{
					this.m_subsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
				}
				if (num2 <= 0)
				{
					worldItem.ToRemove = true;
					return;
				}
				if (pickable != null)
				{
					pickable.Count = num2;
				}
			}
		}

		// Token: 0x040004DE RID: 1246
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004DF RID: 1247
		public SubsystemBlockEntities m_subsystemBlockEntities;

		// Token: 0x040004E0 RID: 1248
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004E1 RID: 1249
		public SubsystemAudio m_subsystemAudio;
	}
}
