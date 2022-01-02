using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000389 RID: 905
	public class FurnitureSetItemWidget : CanvasWidget, IDragTargetWidget
	{
		// Token: 0x06001AEB RID: 6891 RVA: 0x000D1F10 File Offset: 0x000D0110
		public FurnitureSetItemWidget(FurnitureInventoryPanel furnitureInventoryWidget, FurnitureSet furnitureSet)
		{
			this.m_furnitureInventoryPanel = furnitureInventoryWidget;
			this.m_furnitureSet = furnitureSet;
			XElement node = ContentManager.Get<XElement>("Widgets/FurnitureSetItemWidget", null);
			base.LoadContents(this, node);
			FontTextWidget fontTextWidget = this.Children.Find<LabelWidget>("FurnitureSetItem.Name", true);
			LabelWidget labelWidget = this.Children.Find<LabelWidget>("FurnitureSetItem.DesignsCount", true);
			fontTextWidget.Text = ((furnitureSet == null) ? LanguageControl.Get(base.GetType().Name, 0) : furnitureSet.Name);
			labelWidget.Text = string.Format(LanguageControl.Get(base.GetType().Name, 1), this.CountFurnitureDesigns());
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x000D1FB0 File Offset: 0x000D01B0
		public void DragDrop(Widget dragWidget, object data)
		{
			FurnitureDesign furnitureDesign = this.GetFurnitureDesign(data);
			if (furnitureDesign != null)
			{
				this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.AddToFurnitureSet(furnitureDesign, this.m_furnitureSet);
				this.m_furnitureInventoryPanel.Invalidate();
			}
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x000D1FEA File Offset: 0x000D01EA
		public void DragOver(Widget dragWidget, object data)
		{
			this.m_highlighted = (this.GetFurnitureDesign(data) != null);
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x000D1FFC File Offset: 0x000D01FC
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = this.m_highlighted;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x000D2014 File Offset: 0x000D0214
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_highlighted)
			{
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(100, DepthStencilState.None, null, null);
				int count = flatBatch2D.TriangleVertices.Count;
				flatBatch2D.QueueQuad(Vector2.Zero, base.ActualSize, 0f, new Color(128, 128, 128, 128));
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				this.m_highlighted = false;
			}
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x000D208C File Offset: 0x000D028C
		public FurnitureDesign GetFurnitureDesign(object dragData)
		{
			InventoryDragData inventoryDragData = dragData as InventoryDragData;
			if (inventoryDragData != null)
			{
				int slotValue = inventoryDragData.Inventory.GetSlotValue(inventoryDragData.SlotIndex);
				if (Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					return this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				}
			}
			return null;
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x000D20E4 File Offset: 0x000D02E4
		public int CountFurnitureDesigns()
		{
			int num = 0;
			for (int i = 0; i < this.m_furnitureInventoryPanel.ComponentFurnitureInventory.SlotsCount; i++)
			{
				int slotValue = this.m_furnitureInventoryPanel.ComponentFurnitureInventory.GetSlotValue(i);
				if (Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					FurnitureDesign design = this.m_furnitureInventoryPanel.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
					if (design != null && design.FurnitureSet == this.m_furnitureSet)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0400125B RID: 4699
		public FurnitureInventoryPanel m_furnitureInventoryPanel;

		// Token: 0x0400125C RID: 4700
		public FurnitureSet m_furnitureSet;

		// Token: 0x0400125D RID: 4701
		public bool m_highlighted;
	}
}
