using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200016E RID: 366
	public class RecipaediaRecipesScreen : Screen
	{
		// Token: 0x0600084B RID: 2123 RVA: 0x00031EF0 File Offset: 0x000300F0
		public RecipaediaRecipesScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/RecipaediaRecipesScreen", null);
			base.LoadContents(this, node);
			this.m_craftingRecipeWidget = this.Children.Find<CraftingRecipeWidget>("CraftingRecipe", true);
			this.m_smeltingRecipeWidget = this.Children.Find<SmeltingRecipeWidget>("SmeltingRecipe", true);
			this.m_prevRecipeButton = this.Children.Find<ButtonWidget>("PreviousRecipe", true);
			this.m_nextRecipeButton = this.Children.Find<ButtonWidget>("NextRecipe", true);
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00031F80 File Offset: 0x00030180
		public override void Enter(object[] parameters)
		{
			int value = (int)parameters[0];
			this.m_craftingRecipes.Clear();
			this.m_craftingRecipes.AddRange(from r in CraftingRecipesManager.Recipes
			where r.ResultValue == value && r.ResultValue != 0
			select r);
			this.m_recipeIndex = 0;
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x00031FDC File Offset: 0x000301DC
		public override void Update()
		{
			if (this.m_recipeIndex < this.m_craftingRecipes.Count)
			{
				CraftingRecipe craftingRecipe = this.m_craftingRecipes[this.m_recipeIndex];
				if (craftingRecipe.RequiredHeatLevel == 0f)
				{
					this.m_craftingRecipeWidget.Recipe = craftingRecipe;
					this.m_craftingRecipeWidget.NameSuffix = string.Format(LanguageControl.GetContentWidgets(base.GetType().Name, 1), this.m_recipeIndex + 1);
					this.m_craftingRecipeWidget.IsVisible = true;
					this.m_smeltingRecipeWidget.IsVisible = false;
				}
				else
				{
					this.m_smeltingRecipeWidget.Recipe = craftingRecipe;
					this.m_smeltingRecipeWidget.NameSuffix = string.Format(LanguageControl.GetContentWidgets(base.GetType().Name, 1), this.m_recipeIndex + 1);
					this.m_smeltingRecipeWidget.IsVisible = true;
					this.m_craftingRecipeWidget.IsVisible = false;
				}
			}
			this.m_prevRecipeButton.IsEnabled = (this.m_recipeIndex > 0);
			this.m_nextRecipeButton.IsEnabled = (this.m_recipeIndex < this.m_craftingRecipes.Count - 1);
			if (this.m_prevRecipeButton.IsClicked)
			{
				this.m_recipeIndex = MathUtils.Max(this.m_recipeIndex - 1, 0);
			}
			if (this.m_nextRecipeButton.IsClicked)
			{
				this.m_recipeIndex = MathUtils.Min(this.m_recipeIndex + 1, this.m_craftingRecipes.Count - 1);
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x040003BC RID: 956
		public CraftingRecipeWidget m_craftingRecipeWidget;

		// Token: 0x040003BD RID: 957
		public SmeltingRecipeWidget m_smeltingRecipeWidget;

		// Token: 0x040003BE RID: 958
		public ButtonWidget m_prevRecipeButton;

		// Token: 0x040003BF RID: 959
		public ButtonWidget m_nextRecipeButton;

		// Token: 0x040003C0 RID: 960
		public int m_recipeIndex;

		// Token: 0x040003C1 RID: 961
		public List<CraftingRecipe> m_craftingRecipes = new List<CraftingRecipe>();
	}
}
