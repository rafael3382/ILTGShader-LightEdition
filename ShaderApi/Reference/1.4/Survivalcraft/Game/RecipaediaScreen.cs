using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200016F RID: 367
	public class RecipaediaScreen : Screen
	{
		// Token: 0x0600084E RID: 2126 RVA: 0x00032188 File Offset: 0x00030388
		public RecipaediaScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/RecipaediaScreen", null);
			base.LoadContents(this, node);
			this.m_blocksList = this.Children.Find<ListPanelWidget>("BlocksList", true);
			this.m_categoryLabel = this.Children.Find<LabelWidget>("Category", true);
			this.m_prevCategoryButton = this.Children.Find<ButtonWidget>("PreviousCategory", true);
			this.m_nextCategoryButton = this.Children.Find<ButtonWidget>("NextCategory", true);
			this.m_detailsButton = this.Children.Find<ButtonWidget>("DetailsButton", true);
			this.m_recipesButton = this.Children.Find<ButtonWidget>("RecipesButton", true);
			this.m_categories.Add(null);
			this.m_categories.AddRange(BlocksManager.Categories);
			this.m_blocksList.ItemWidgetFactory = delegate(object item)
			{
				int value = (int)item;
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				XElement node2 = ContentManager.Get<XElement>("Widgets/RecipaediaItem", null);
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				containerWidget.Children.Find<BlockIconWidget>("RecipaediaItem.Icon", true).Value = value;
				containerWidget.Children.Find<LabelWidget>("RecipaediaItem.Text", true).Text = block.GetDisplayName(null, value);
				containerWidget.Children.Find<LabelWidget>("RecipaediaItem.Details", true).Text = block.GetDescription(value);
				return containerWidget;
			};
			ListPanelWidget blocksList = this.m_blocksList;
			blocksList.ItemClicked = (Action<object>)Delegate.Combine(blocksList.ItemClicked, new Action<object>(delegate(object item)
			{
				if (this.m_blocksList.SelectedItem == item && item is int)
				{
					ScreensManager.SwitchScreen("RecipaediaDescription", new object[]
					{
						item,
						this.m_blocksList.Items.Cast<int>().ToList<int>()
					});
				}
			}));
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x000322AA File Offset: 0x000304AA
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("RecipaediaRecipes") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("RecipaediaDescription"))
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x000322DC File Offset: 0x000304DC
		public override void Update()
		{
			if (this.m_listCategoryIndex != this.m_categoryIndex)
			{
				this.PopulateBlocksList();
			}
			string arg = (this.m_categories[this.m_categoryIndex] == null) ? LanguageControl.Get(new string[]
			{
				"BlocksManager",
				"All Blocks"
			}) : LanguageControl.Get(new string[]
			{
				"BlocksManager",
				this.m_categories[this.m_categoryIndex]
			});
			this.m_categoryLabel.Text = string.Format("{0} ({1})", arg, this.m_blocksList.Items.Count);
			this.m_prevCategoryButton.IsEnabled = (this.m_categoryIndex > 0);
			this.m_nextCategoryButton.IsEnabled = (this.m_categoryIndex < this.m_categories.Count - 1);
			int? value = null;
			int num = 0;
			if (this.m_blocksList.SelectedItem is int)
			{
				value = new int?((int)this.m_blocksList.SelectedItem);
				num = CraftingRecipesManager.Recipes.Count(delegate(CraftingRecipe r)
				{
					int resultValue = r.ResultValue;
					int? value = value;
					return resultValue == value.GetValueOrDefault() & value != null;
				});
			}
			if (num > 0)
			{
				this.m_recipesButton.Text = string.Format("{0} {1}", num, (num == 1) ? LanguageControl.Get(base.GetType().Name, 1) : LanguageControl.Get(base.GetType().Name, 2));
				this.m_recipesButton.IsEnabled = true;
			}
			else
			{
				this.m_recipesButton.Text = LanguageControl.Get(base.GetType().Name, 3);
				this.m_recipesButton.IsEnabled = false;
			}
			this.m_detailsButton.IsEnabled = (value != null);
			if (this.m_prevCategoryButton.IsClicked || base.Input.Left)
			{
				this.m_categoryIndex = MathUtils.Max(this.m_categoryIndex - 1, 0);
			}
			if (this.m_nextCategoryButton.IsClicked || base.Input.Right)
			{
				this.m_categoryIndex = MathUtils.Min(this.m_categoryIndex + 1, this.m_categories.Count - 1);
			}
			if (value != null && this.m_detailsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("RecipaediaDescription", new object[]
				{
					value.Value,
					this.m_blocksList.Items.Cast<int>().ToList<int>()
				});
			}
			if (value != null && this.m_recipesButton.IsClicked)
			{
				ScreensManager.SwitchScreen("RecipaediaRecipes", new object[]
				{
					value.Value
				});
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x000325E8 File Offset: 0x000307E8
		public void PopulateBlocksList()
		{
			this.m_listCategoryIndex = this.m_categoryIndex;
			string text = this.m_categories[this.m_categoryIndex];
			this.m_blocksList.ScrollPosition = 0f;
			this.m_blocksList.ClearItems();
			List<RecipaediaScreen.Order> list = new List<RecipaediaScreen.Order>();
			foreach (Block block in BlocksManager.Blocks)
			{
				foreach (int num in block.GetCreativeValues())
				{
					if (string.IsNullOrEmpty(text) || block.GetCategory(num) == text)
					{
						list.Add(new RecipaediaScreen.Order(block, block.GetDisplayOrder(num), num));
					}
				}
			}
			foreach (RecipaediaScreen.Order order in from o in list
			orderby o.order
			select o)
			{
				this.m_blocksList.AddItem(order.value);
			}
		}

		// Token: 0x040003C2 RID: 962
		public ListPanelWidget m_blocksList;

		// Token: 0x040003C3 RID: 963
		public LabelWidget m_categoryLabel;

		// Token: 0x040003C4 RID: 964
		public ButtonWidget m_prevCategoryButton;

		// Token: 0x040003C5 RID: 965
		public ButtonWidget m_nextCategoryButton;

		// Token: 0x040003C6 RID: 966
		public ButtonWidget m_detailsButton;

		// Token: 0x040003C7 RID: 967
		public ButtonWidget m_recipesButton;

		// Token: 0x040003C8 RID: 968
		public Screen m_previousScreen;

		// Token: 0x040003C9 RID: 969
		public List<string> m_categories = new List<string>();

		// Token: 0x040003CA RID: 970
		public int m_categoryIndex;

		// Token: 0x040003CB RID: 971
		public int m_listCategoryIndex = -1;

		// Token: 0x0200046E RID: 1134
		internal class Order
		{
			// Token: 0x06002030 RID: 8240 RVA: 0x000E7479 File Offset: 0x000E5679
			public Order(Block b, int o, int v)
			{
				this.block = b;
				this.order = o;
				this.value = v;
			}

			// Token: 0x0400163C RID: 5692
			public Block block;

			// Token: 0x0400163D RID: 5693
			public int order;

			// Token: 0x0400163E RID: 5694
			public int value;
		}
	}
}
