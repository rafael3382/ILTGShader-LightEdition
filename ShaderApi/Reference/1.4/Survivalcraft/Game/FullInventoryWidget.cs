using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000386 RID: 902
	public class FullInventoryWidget : CanvasWidget
	{
		// Token: 0x06001ADB RID: 6875 RVA: 0x000D1028 File Offset: 0x000CF228
		public FullInventoryWidget(IInventory inventory, ComponentCraftingTable componentCraftingTable)
		{
			XElement node = ContentManager.Get<XElement>("Widgets/FullInventoryWidget", null);
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
					inventorySlotWidget2.AssignInventorySlot(componentCraftingTable, num++);
					this.m_craftingGrid.Children.Add(inventorySlotWidget2);
					this.m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
			this.m_craftingResultSlot.AssignInventorySlot(componentCraftingTable, componentCraftingTable.ResultSlotIndex);
			this.m_craftingRemainsSlot.AssignInventorySlot(componentCraftingTable, componentCraftingTable.RemainsSlotIndex);
		}

		// Token: 0x04001245 RID: 4677
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001246 RID: 4678
		public GridPanelWidget m_craftingGrid;

		// Token: 0x04001247 RID: 4679
		public InventorySlotWidget m_craftingResultSlot;

		// Token: 0x04001248 RID: 4680
		public InventorySlotWidget m_craftingRemainsSlot;
	}
}
