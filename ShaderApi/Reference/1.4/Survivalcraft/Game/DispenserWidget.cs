using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000380 RID: 896
	public class DispenserWidget : CanvasWidget
	{
		// Token: 0x06001A96 RID: 6806 RVA: 0x000CEEC8 File Offset: 0x000CD0C8
		public DispenserWidget(IInventory inventory, ComponentDispenser componentDispenser)
		{
			this.m_componentDispenser = componentDispenser;
			this.m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(true);
			this.m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/DispenserWidget", null);
			base.LoadContents(this, node);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_dispenserGrid = this.Children.Find<GridPanelWidget>("DispenserGrid", true);
			this.m_dispenseButton = this.Children.Find<ButtonWidget>("DispenseButton", true);
			this.m_shootButton = this.Children.Find<ButtonWidget>("ShootButton", true);
			this.m_acceptsDropsBox = this.Children.Find<CheckboxWidget>("AcceptsDropsBox", true);
			int num = 0;
			for (int i = 0; i < this.m_dispenserGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_dispenserGrid.ColumnsCount; j++)
				{
					InventorySlotWidget inventorySlotWidget = new InventorySlotWidget();
					inventorySlotWidget.AssignInventorySlot(componentDispenser, num++);
					this.m_dispenserGrid.Children.Add(inventorySlotWidget);
					this.m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
				}
			}
			num = 10;
			for (int k = 0; k < this.m_inventoryGrid.RowsCount; k++)
			{
				for (int l = 0; l < this.m_inventoryGrid.ColumnsCount; l++)
				{
					InventorySlotWidget inventorySlotWidget2 = new InventorySlotWidget();
					inventorySlotWidget2.AssignInventorySlot(inventory, num++);
					this.m_inventoryGrid.Children.Add(inventorySlotWidget2);
					this.m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
				}
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x000CF068 File Offset: 0x000CD268
		public override void Update()
		{
			int value = this.m_subsystemTerrain.Terrain.GetCellValue(this.m_componentBlockEntity.Coordinates.X, this.m_componentBlockEntity.Coordinates.Y, this.m_componentBlockEntity.Coordinates.Z);
			int data = Terrain.ExtractData(value);
			if (this.m_dispenseButton.IsClicked)
			{
				data = DispenserBlock.SetMode(data, DispenserBlock.Mode.Dispense);
				value = Terrain.ReplaceData(value, data);
				this.m_subsystemTerrain.ChangeCell(this.m_componentBlockEntity.Coordinates.X, this.m_componentBlockEntity.Coordinates.Y, this.m_componentBlockEntity.Coordinates.Z, value, true);
			}
			if (this.m_shootButton.IsClicked)
			{
				data = DispenserBlock.SetMode(data, DispenserBlock.Mode.Shoot);
				value = Terrain.ReplaceData(value, data);
				this.m_subsystemTerrain.ChangeCell(this.m_componentBlockEntity.Coordinates.X, this.m_componentBlockEntity.Coordinates.Y, this.m_componentBlockEntity.Coordinates.Z, value, true);
			}
			if (this.m_acceptsDropsBox.IsClicked)
			{
				data = DispenserBlock.SetAcceptsDrops(data, !DispenserBlock.GetAcceptsDrops(data));
				value = Terrain.ReplaceData(value, data);
				this.m_subsystemTerrain.ChangeCell(this.m_componentBlockEntity.Coordinates.X, this.m_componentBlockEntity.Coordinates.Y, this.m_componentBlockEntity.Coordinates.Z, value, true);
			}
			DispenserBlock.Mode mode = DispenserBlock.GetMode(data);
			this.m_dispenseButton.IsChecked = (mode == DispenserBlock.Mode.Dispense);
			this.m_shootButton.IsChecked = (mode == DispenserBlock.Mode.Shoot);
			this.m_acceptsDropsBox.IsChecked = DispenserBlock.GetAcceptsDrops(data);
			if (!this.m_componentDispenser.IsAddedToProject)
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x0400121B RID: 4635
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400121C RID: 4636
		public ComponentDispenser m_componentDispenser;

		// Token: 0x0400121D RID: 4637
		public ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x0400121E RID: 4638
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x0400121F RID: 4639
		public GridPanelWidget m_dispenserGrid;

		// Token: 0x04001220 RID: 4640
		public ButtonWidget m_dispenseButton;

		// Token: 0x04001221 RID: 4641
		public ButtonWidget m_shootButton;

		// Token: 0x04001222 RID: 4642
		public CheckboxWidget m_acceptsDropsBox;
	}
}
