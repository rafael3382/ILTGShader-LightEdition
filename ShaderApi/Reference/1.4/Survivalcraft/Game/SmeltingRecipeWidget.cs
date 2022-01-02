using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200039F RID: 927
	public class SmeltingRecipeWidget : CanvasWidget
	{
		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001C10 RID: 7184 RVA: 0x000DA7B4 File Offset: 0x000D89B4
		// (set) Token: 0x06001C11 RID: 7185 RVA: 0x000DA7BC File Offset: 0x000D89BC
		public string NameSuffix
		{
			get
			{
				return this.m_nameSuffix;
			}
			set
			{
				if (value != this.m_nameSuffix)
				{
					this.m_nameSuffix = value;
					this.m_dirty = true;
				}
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001C12 RID: 7186 RVA: 0x000DA7DA File Offset: 0x000D89DA
		// (set) Token: 0x06001C13 RID: 7187 RVA: 0x000DA7E2 File Offset: 0x000D89E2
		public CraftingRecipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
			set
			{
				if (value != this.m_recipe)
				{
					this.m_recipe = value;
					this.m_dirty = true;
				}
			}
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x000DA7FC File Offset: 0x000D89FC
		public SmeltingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/SmeltingRecipe", null);
			base.LoadContents(this, node);
			this.m_nameWidget = this.Children.Find<LabelWidget>("SmeltingRecipeWidget.Name", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("SmeltingRecipeWidget.Description", true);
			this.m_gridWidget = this.Children.Find<GridPanelWidget>("SmeltingRecipeWidget.Ingredients", true);
			this.m_fireWidget = this.Children.Find<FireWidget>("SmeltingRecipeWidget.Fire", true);
			this.m_resultWidget = this.Children.Find<CraftingRecipeSlotWidget>("SmeltingRecipeWidget.Result", true);
			for (int i = 0; i < this.m_gridWidget.RowsCount; i++)
			{
				for (int j = 0; j < this.m_gridWidget.ColumnsCount; j++)
				{
					CraftingRecipeSlotWidget widget = new CraftingRecipeSlotWidget();
					this.m_gridWidget.Children.Add(widget);
					this.m_gridWidget.SetWidgetCell(widget, new Point2(j, i));
				}
			}
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x000DA8F3 File Offset: 0x000D8AF3
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_dirty)
			{
				this.UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x000DA90C File Offset: 0x000D8B0C
		public void UpdateWidgets()
		{
			this.m_dirty = false;
			if (this.m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.m_recipe.ResultValue)];
				this.m_nameWidget.Text = block.GetDisplayName(null, this.m_recipe.ResultValue) + ((!string.IsNullOrEmpty(this.NameSuffix)) ? this.NameSuffix : string.Empty);
				this.m_descriptionWidget.Text = this.m_recipe.Description;
				this.m_nameWidget.IsVisible = true;
				this.m_descriptionWidget.IsVisible = true;
				foreach (Widget widget in this.m_gridWidget.Children)
				{
					CraftingRecipeSlotWidget craftingRecipeSlotWidget = (CraftingRecipeSlotWidget)widget;
					Point2 widgetCell = this.m_gridWidget.GetWidgetCell(craftingRecipeSlotWidget);
					craftingRecipeSlotWidget.SetIngredient(this.m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 3]);
				}
				this.m_resultWidget.SetResult(this.m_recipe.ResultValue, this.m_recipe.ResultCount);
				this.m_fireWidget.ParticlesPerSecond = 40f;
				return;
			}
			this.m_nameWidget.IsVisible = false;
			this.m_descriptionWidget.IsVisible = false;
			foreach (Widget widget2 in this.m_gridWidget.Children)
			{
				((CraftingRecipeSlotWidget)widget2).SetIngredient(null);
			}
			this.m_resultWidget.SetResult(0, 0);
			this.m_fireWidget.ParticlesPerSecond = 0f;
		}

		// Token: 0x040012FD RID: 4861
		public LabelWidget m_nameWidget;

		// Token: 0x040012FE RID: 4862
		public LabelWidget m_descriptionWidget;

		// Token: 0x040012FF RID: 4863
		public GridPanelWidget m_gridWidget;

		// Token: 0x04001300 RID: 4864
		public FireWidget m_fireWidget;

		// Token: 0x04001301 RID: 4865
		public CraftingRecipeSlotWidget m_resultWidget;

		// Token: 0x04001302 RID: 4866
		public CraftingRecipe m_recipe;

		// Token: 0x04001303 RID: 4867
		public string m_nameSuffix;

		// Token: 0x04001304 RID: 4868
		public bool m_dirty = true;
	}
}
