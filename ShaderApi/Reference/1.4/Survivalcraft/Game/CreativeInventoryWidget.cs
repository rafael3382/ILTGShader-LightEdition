using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200037E RID: 894
	public class CreativeInventoryWidget : CanvasWidget
	{
		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001A87 RID: 6791 RVA: 0x000CE4F6 File Offset: 0x000CC6F6
		public Entity Entity
		{
			get
			{
				return this.m_componentCreativeInventory.Entity;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001A88 RID: 6792 RVA: 0x000CE503 File Offset: 0x000CC703
		public ButtonWidget PageDownButton
		{
			get
			{
				return this.m_pageDownButton;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001A89 RID: 6793 RVA: 0x000CE50B File Offset: 0x000CC70B
		public ButtonWidget PageUpButton
		{
			get
			{
				return this.m_pageUpButton;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06001A8A RID: 6794 RVA: 0x000CE513 File Offset: 0x000CC713
		public LabelWidget PageLabel
		{
			get
			{
				return this.m_pageLabel;
			}
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x000CE51C File Offset: 0x000CC71C
		public CreativeInventoryWidget(Entity entity)
		{
			this.m_componentCreativeInventory = entity.FindComponent<ComponentCreativeInventory>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/CreativeInventoryWidget", null);
			base.LoadContents(this, node);
			this.m_categoryLeftButton = this.Children.Find<ButtonWidget>("CategoryLeftButton", true);
			this.m_categoryRightButton = this.Children.Find<ButtonWidget>("CategoryRightButton", true);
			this.m_categoryButton = this.Children.Find<ButtonWidget>("CategoryButton", true);
			this.m_pageUpButton = this.Children.Find<ButtonWidget>("PageUpButton", true);
			this.m_pageDownButton = this.Children.Find<ButtonWidget>("PageDownButton", true);
			this.m_pageLabel = this.Children.Find<LabelWidget>("PageLabel", true);
			this.m_panelContainer = this.Children.Find<ContainerWidget>("PanelContainer", true);
			CreativeInventoryPanel creativeInventoryPanel = new CreativeInventoryPanel(this)
			{
				IsVisible = false
			};
			this.m_panelContainer.Children.Add(creativeInventoryPanel);
			FurnitureInventoryPanel furnitureInventoryPanel = new FurnitureInventoryPanel(this)
			{
				IsVisible = false
			};
			this.m_panelContainer.Children.Add(furnitureInventoryPanel);
			foreach (string name in BlocksManager.Categories)
			{
				this.m_categories.Add(new CreativeInventoryWidget.Category
				{
					Name = name,
					Panel = creativeInventoryPanel
				});
			}
			this.m_categories.Add(new CreativeInventoryWidget.Category
			{
				Name = LanguageControl.Get(CreativeInventoryWidget.fName, 1),
				Panel = furnitureInventoryPanel
			});
			this.m_categories.Add(new CreativeInventoryWidget.Category
			{
				Name = LanguageControl.Get(CreativeInventoryWidget.fName, 2),
				Panel = creativeInventoryPanel
			});
			for (int i = 0; i < this.m_categories.Count; i++)
			{
				if (this.m_categories[i].Name == "Electrics")
				{
					this.m_categories[i].Color = new Color(128, 140, 255);
				}
				if (this.m_categories[i].Name == "Plants")
				{
					this.m_categories[i].Color = new Color(64, 160, 64);
				}
				if (this.m_categories[i].Name == "Weapons")
				{
					this.m_categories[i].Color = new Color(255, 128, 112);
				}
			}
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x000CE7D8 File Offset: 0x000CC9D8
		public string GetCategoryName(int index)
		{
			return this.m_categories[index].Name;
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x000CE7EC File Offset: 0x000CC9EC
		public override void Update()
		{
			if (this.m_categoryLeftButton.IsClicked || base.Input.Left)
			{
				ComponentCreativeInventory componentCreativeInventory = this.m_componentCreativeInventory;
				int categoryIndex = componentCreativeInventory.CategoryIndex - 1;
				componentCreativeInventory.CategoryIndex = categoryIndex;
			}
			if (this.m_categoryRightButton.IsClicked || base.Input.Right)
			{
				ComponentCreativeInventory componentCreativeInventory2 = this.m_componentCreativeInventory;
				int categoryIndex = componentCreativeInventory2.CategoryIndex + 1;
				componentCreativeInventory2.CategoryIndex = categoryIndex;
			}
			if (this.m_categoryButton.IsClicked)
			{
				ComponentPlayer componentPlayer = this.Entity.FindComponent<ComponentPlayer>();
				if (componentPlayer != null)
				{
					DialogsManager.ShowDialog(componentPlayer.GuiWidget, new ListSelectionDialog(string.Empty, this.m_categories, 56f, (object c) => new LabelWidget
					{
						Text = LanguageControl.Get(new string[]
						{
							"BlocksManager",
							((CreativeInventoryWidget.Category)c).Name
						}),
						Color = ((CreativeInventoryWidget.Category)c).Color,
						HorizontalAlignment = WidgetAlignment.Center,
						VerticalAlignment = WidgetAlignment.Center
					}, delegate(object c)
					{
						if (c != null)
						{
							this.m_componentCreativeInventory.CategoryIndex = this.m_categories.IndexOf((CreativeInventoryWidget.Category)c);
						}
					}));
				}
			}
			this.m_componentCreativeInventory.CategoryIndex = MathUtils.Clamp(this.m_componentCreativeInventory.CategoryIndex, 0, this.m_categories.Count - 1);
			this.m_categoryButton.Text = LanguageControl.Get(new string[]
			{
				"BlocksManager",
				this.m_categories[this.m_componentCreativeInventory.CategoryIndex].Name
			});
			this.m_categoryLeftButton.IsEnabled = (this.m_componentCreativeInventory.CategoryIndex > 0);
			this.m_categoryRightButton.IsEnabled = (this.m_componentCreativeInventory.CategoryIndex < this.m_categories.Count - 1);
			if (this.m_componentCreativeInventory.CategoryIndex != this.m_activeCategoryIndex)
			{
				foreach (CreativeInventoryWidget.Category category in this.m_categories)
				{
					category.Panel.IsVisible = false;
				}
				this.m_categories[this.m_componentCreativeInventory.CategoryIndex].Panel.IsVisible = true;
				this.m_activeCategoryIndex = this.m_componentCreativeInventory.CategoryIndex;
			}
		}

		// Token: 0x04001208 RID: 4616
		public List<CreativeInventoryWidget.Category> m_categories = new List<CreativeInventoryWidget.Category>();

		// Token: 0x04001209 RID: 4617
		public int m_activeCategoryIndex = -1;

		// Token: 0x0400120A RID: 4618
		public ComponentCreativeInventory m_componentCreativeInventory;

		// Token: 0x0400120B RID: 4619
		public ButtonWidget m_pageUpButton;

		// Token: 0x0400120C RID: 4620
		public ButtonWidget m_pageDownButton;

		// Token: 0x0400120D RID: 4621
		public LabelWidget m_pageLabel;

		// Token: 0x0400120E RID: 4622
		public ButtonWidget m_categoryLeftButton;

		// Token: 0x0400120F RID: 4623
		public ButtonWidget m_categoryRightButton;

		// Token: 0x04001210 RID: 4624
		public static string fName = "CreativeInventoryWidget";

		// Token: 0x04001211 RID: 4625
		public ButtonWidget m_categoryButton;

		// Token: 0x04001212 RID: 4626
		public ContainerWidget m_panelContainer;

		// Token: 0x0200057A RID: 1402
		public class Category
		{
			// Token: 0x040019D1 RID: 6609
			public string Name;

			// Token: 0x040019D2 RID: 6610
			public Color Color = Color.White;

			// Token: 0x040019D3 RID: 6611
			public ContainerWidget Panel;
		}
	}
}
