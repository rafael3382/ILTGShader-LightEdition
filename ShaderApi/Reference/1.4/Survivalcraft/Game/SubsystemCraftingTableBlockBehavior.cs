using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000197 RID: 407
	public class SubsystemCraftingTableBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x0003BBA1 File Offset: 0x00039DA1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					27
				};
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0003BBB0 File Offset: 0x00039DB0
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("CraftingTable", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, true);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(databaseObject);
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue<Point3>("Coordinates", new Point3(x, y, z));
			Entity entity = base.SubsystemTerrain.Project.CreateEntity(valuesDictionary);
			base.SubsystemTerrain.Project.AddEntity(entity);
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0003BC44 File Offset: 0x00039E44
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
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

		// Token: 0x0600097F RID: 2431 RVA: 0x0003BCF0 File Offset: 0x00039EF0
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (blockEntity != null && componentMiner.ComponentPlayer != null)
			{
				ComponentCraftingTable componentCraftingTable = blockEntity.Entity.FindComponent<ComponentCraftingTable>(true);
				componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new CraftingTableWidget(componentMiner.Inventory, componentCraftingTable);
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				return true;
			}
			return false;
		}
	}
}
