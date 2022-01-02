using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000195 RID: 405
	public class SubsystemChestBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x0003B472 File Offset: 0x00039672
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					45
				};
			}
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0003B47F File Offset: 0x0003967F
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemBlockEntities = base.Project.FindSubsystem<SubsystemBlockEntities>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0003B4AC File Offset: 0x000396AC
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.Project.GameDatabase.Database.FindDatabaseObject("Chest", base.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Entity entity = base.Project.CreateEntity(valuesDictionary);
			base.Project.AddEntity(entity);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0003B52C File Offset: 0x0003972C
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

		// Token: 0x0600096E RID: 2414 RVA: 0x0003B5C8 File Offset: 0x000397C8
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity != null && componentMiner.ComponentPlayer != null)
			{
				ComponentChest componentChest = blockEntity.Entity.FindComponent<ComponentChest>(true);
				componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new ChestWidget(componentMiner.Inventory, componentChest);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				return true;
			}
			return false;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0003B654 File Offset: 0x00039854
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			if (blockEntity != null)
			{
				IInventory inventory = blockEntity.Entity.FindComponent<ComponentChest>(true);
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

		// Token: 0x040004BC RID: 1212
		public SubsystemBlockEntities m_subsystemBlockEntities;

		// Token: 0x040004BD RID: 1213
		public SubsystemAudio m_subsystemAudio;
	}
}
