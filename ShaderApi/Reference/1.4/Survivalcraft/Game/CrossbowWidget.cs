using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200037F RID: 895
	public class CrossbowWidget : CanvasWidget
	{
		// Token: 0x06001A90 RID: 6800 RVA: 0x000CEA20 File Offset: 0x000CCC20
		public CrossbowWidget(IInventory inventory, int slotIndex)
		{
			this.m_inventory = inventory;
			this.m_slotIndex = slotIndex;
			XElement node = ContentManager.Get<XElement>("Widgets/CrossbowWidget", null);
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
			this.m_inventorySlotWidget.CustomViewMatrix = new Matrix?(Matrix.CreateLookAt(new Vector3(0f, 1f, 0.2f), new Vector3(0f, 0f, 0.2f), -Vector3.UnitZ));
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x000CEBB0 File Offset: 0x000CCDB0
		public override void Update()
		{
			int slotValue = this.m_inventory.GetSlotValue(this.m_slotIndex);
			int slotCount = this.m_inventory.GetSlotCount(this.m_slotIndex);
			int num = Terrain.ExtractContents(slotValue);
			int data = Terrain.ExtractData(slotValue);
			int draw = CrossbowBlock.GetDraw(data);
			ArrowBlock.ArrowType? arrowType = CrossbowBlock.GetArrowType(data);
			if (num == 200 && slotCount > 0)
			{
				if (draw < 15)
				{
					this.m_instructionsLabel.Text = LanguageControl.Get(CrossbowWidget.fName, 0);
				}
				else
				{
					this.m_instructionsLabel.Text = ((arrowType == null) ? LanguageControl.Get(CrossbowWidget.fName, 1) : LanguageControl.Get(CrossbowWidget.fName, 2));
				}
				if ((draw < 15 || arrowType == null) && base.Input.Tap != null && base.HitTestGlobal(base.Input.Tap.Value, null) == this.m_inventorySlotWidget)
				{
					Vector2 vector = this.m_inventorySlotWidget.ScreenToWidget(base.Input.Press.Value);
					float num2 = vector.Y - CrossbowWidget.DrawToPosition(draw);
					if (MathUtils.Abs(vector.X - this.m_inventorySlotWidget.ActualSize.X / 2f) < 25f && MathUtils.Abs(num2) < 25f)
					{
						this.m_dragStartOffset = new float?(num2);
					}
				}
				if (this.m_dragStartOffset == null)
				{
					return;
				}
				if (base.Input.Press != null)
				{
					int num3 = CrossbowWidget.PositionToDraw(this.m_inventorySlotWidget.ScreenToWidget(base.Input.Press.Value).Y - this.m_dragStartOffset.Value);
					this.SetDraw(num3);
					if (draw <= 9 && num3 > 9)
					{
						AudioManager.PlaySound("Audio/CrossbowDraw", 1f, this.m_random.Float(-0.2f, 0.2f), 0f);
						return;
					}
				}
				else
				{
					this.m_dragStartOffset = null;
					if (draw == 15)
					{
						AudioManager.PlaySound("Audio/UI/ItemMoved", 1f, 0f, 0f);
						return;
					}
					this.SetDraw(0);
					AudioManager.PlaySound("Audio/CrossbowBoing", MathUtils.Saturate((float)(draw - 3) / 10f), this.m_random.Float(-0.1f, 0.1f), 0f);
					return;
				}
			}
			else
			{
				base.ParentWidget.Children.Remove(this);
			}
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x000CEE28 File Offset: 0x000CD028
		public void SetDraw(int draw)
		{
			int data = Terrain.ExtractData(this.m_inventory.GetSlotValue(this.m_slotIndex));
			int value = Terrain.MakeBlockValue(200, 0, CrossbowBlock.SetDraw(data, draw));
			this.m_inventory.RemoveSlotItems(this.m_slotIndex, 1);
			this.m_inventory.AddSlotItems(this.m_slotIndex, value, 1);
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x000CEE85 File Offset: 0x000CD085
		public static float DrawToPosition(int draw)
		{
			return (float)draw * 5.4f + 85f;
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x000CEE95 File Offset: 0x000CD095
		public static int PositionToDraw(float position)
		{
			return (int)MathUtils.Clamp(MathUtils.Round((position - 85f) / 5.4f), 0f, 15f);
		}

		// Token: 0x04001213 RID: 4627
		public IInventory m_inventory;

		// Token: 0x04001214 RID: 4628
		public int m_slotIndex;

		// Token: 0x04001215 RID: 4629
		public float? m_dragStartOffset;

		// Token: 0x04001216 RID: 4630
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04001217 RID: 4631
		public InventorySlotWidget m_inventorySlotWidget;

		// Token: 0x04001218 RID: 4632
		public LabelWidget m_instructionsLabel;

		// Token: 0x04001219 RID: 4633
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400121A RID: 4634
		public static string fName = "CrossbowWidget";
	}
}
