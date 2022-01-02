using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200037C RID: 892
	public class CraftingRecipeWidget : CanvasWidget
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06001A7E RID: 6782 RVA: 0x000CE04C File Offset: 0x000CC24C
		// (set) Token: 0x06001A7F RID: 6783 RVA: 0x000CE054 File Offset: 0x000CC254
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

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001A80 RID: 6784 RVA: 0x000CE072 File Offset: 0x000CC272
		// (set) Token: 0x06001A81 RID: 6785 RVA: 0x000CE07A File Offset: 0x000CC27A
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

		// Token: 0x06001A82 RID: 6786 RVA: 0x000CE094 File Offset: 0x000CC294
		public CraftingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipe", null);
			base.LoadContents(this, node);
			this.m_nameWidget = this.Children.Find<LabelWidget>("CraftingRecipeWidget.Name", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("CraftingRecipeWidget.Description", true);
			this.m_gridWidget = this.Children.Find<GridPanelWidget>("CraftingRecipeWidget.Ingredients", true);
			this.m_resultWidget = this.Children.Find<CraftingRecipeSlotWidget>("CraftingRecipeWidget.Result", true);
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

		// Token: 0x06001A83 RID: 6787 RVA: 0x000CE174 File Offset: 0x000CC374
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_dirty)
			{
				this.UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x000CE18C File Offset: 0x000CC38C
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
				return;
			}
			this.m_nameWidget.IsVisible = false;
			this.m_descriptionWidget.IsVisible = false;
			foreach (Widget widget2 in this.m_gridWidget.Children)
			{
				((CraftingRecipeSlotWidget)widget2).SetIngredient(null);
			}
			this.m_resultWidget.SetResult(0, 0);
		}

		// Token: 0x040011FC RID: 4604
		public LabelWidget m_nameWidget;

		// Token: 0x040011FD RID: 4605
		public LabelWidget m_descriptionWidget;

		// Token: 0x040011FE RID: 4606
		public GridPanelWidget m_gridWidget;

		// Token: 0x040011FF RID: 4607
		public CraftingRecipeSlotWidget m_resultWidget;

		// Token: 0x04001200 RID: 4608
		public CraftingRecipe m_recipe;

		// Token: 0x04001201 RID: 4609
		public string m_nameSuffix;

		// Token: 0x04001202 RID: 4610
		public bool m_dirty = true;
	}
}
