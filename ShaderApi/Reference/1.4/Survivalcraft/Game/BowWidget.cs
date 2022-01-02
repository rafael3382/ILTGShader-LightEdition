using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000370 RID: 880
	public class BowWidget : CanvasWidget
	{
		// Token: 0x06001A16 RID: 6678 RVA: 0x000CC6C8 File Offset: 0x000CA8C8
		public BowWidget(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			XElement node = ContentManager.Get<XElement>("Widgets/BowWidget", null);
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_inventorySlotWidget = this.Children.Find<InventorySlotWidget>("InventorySlot", true);
			this.m_instructionsLabel = this.Children.Find<LabelWidget>("InstructionsLabel", true);
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget();
					this.m_inventoryGrid.Children.Add(widget);
					this.m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
			int num = 10;
			foreach (Widget widget2 in this.m_inventoryGrid.Children)
			{
				InventorySlotWidget inventorySlotWidget = widget2 as InventorySlotWidget;
				if (inventorySlotWidget != null)
				{
					inventorySlotWidget.AssignInventorySlot(inventory, num++);
				}
			}
			this.m_inventorySlotWidget.AssignInventorySlot(inventory, slotIndex);
			this.m_inventorySlotWidget.CustomViewMatrix = new Matrix?(Matrix.CreateLookAt(new Vector3(-1f, 0.2f, 0.6f), new Vector3(0f, 0.2f, 0f), Vector3.UnitY));
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x000CC848 File Offset: 0x000CAA48
		public override void Update()
		{
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			int num = Terrain.ExtractContents(slotValue);
			this.m_instructionsLabel.Text = ((BowBlock.GetArrowType(Terrain.ExtractData(slotValue)) == null) ? LanguageControl.Get(BowWidget.fName, 0) : LanguageControl.Get(BowWidget.fName, 1));
			if (num != 191 || slotCount == 0)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x040011C4 RID: 4548
		public IInventory m_inventory;

		// Token: 0x040011C5 RID: 4549
		public int m_slotIndex;

		// Token: 0x040011C6 RID: 4550
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x040011C7 RID: 4551
		public InventorySlotWidget m_inventorySlotWidget;

		// Token: 0x040011C8 RID: 4552
		public LabelWidget m_instructionsLabel;

		// Token: 0x040011C9 RID: 4553
		public static string fName = "BowWidget";
	}
}
