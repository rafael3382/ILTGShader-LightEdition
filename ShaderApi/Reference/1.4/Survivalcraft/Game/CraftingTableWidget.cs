using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200037D RID: 893
	public class CraftingTableWidget : CanvasWidget
	{
		// Token: 0x06001A85 RID: 6789 RVA: 0x000CE334 File Offset: 0x000CC534
		public CraftingTableWidget(IInventory inventory, ComponentCraftingTable componentCraftingTable)
		{
			this.m_componentCraftingTable = componentCraftingTable;
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingTableWidget", null);
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_craftingGrid = this.Children.Find<GridPanelWidget>("CraftingGrid", true);
			this.m_craftingResultSlot = this.Children.Find<InventorySlotWidget>("CraftingResultSlot", true);
			this.m_craftingRemainsSlot = this.Children.Find<InventorySlotWidget>("CraftingRemainsSlot", true);
			int num = 10;
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			num = 0;
			for (int k = 0; k < this.m_craftingGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_craftingGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(this.m_componentCraftingTable, num++);
					this.m_craftingGrid.Children.Add(inventorySlotWidget2);
					this.m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_craftingResultSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.ResultSlotIndex);
			this.m_craftingRemainsSlot.AssignInventorySlot(this.m_componentCraftingTable, this.m_componentCraftingTable.RemainsSlotIndex);
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x000CE4D6 File Offset: 0x000CC6D6
		public override void Update()
		{
			if (!this.m_componentCraftingTable.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x04001203 RID: 4611
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001204 RID: 4612
		public GridPanelWidget m_craftingGrid;

		// Token: 0x04001205 RID: 4613
		public InventorySlotWidget m_craftingResultSlot;

		// Token: 0x04001206 RID: 4614
		public InventorySlotWidget m_craftingRemainsSlot;

		// Token: 0x04001207 RID: 4615
		public ComponentCraftingTable m_componentCraftingTable;
	}
}
