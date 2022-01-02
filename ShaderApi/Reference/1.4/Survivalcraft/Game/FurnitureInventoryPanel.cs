using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200029B RID: 667
	public class FurnitureInventoryPanel : CanvasWidget
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x0009E1BC File Offset: 0x0009C3BC
		// (set) Token: 0x060014E9 RID: 5353 RVA: 0x0009E1C4 File Offset: 0x0009C3C4
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x0009E1CD File Offset: 0x0009C3CD
		// (set) Token: 0x060014EB RID: 5355 RVA: 0x0009E1D5 File Offset: 0x0009C3D5
		public SubsystemFurnitureBlockBehavior SubsystemFurnitureBlockBehavior { get; set; }

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060014EC RID: 5356 RVA: 0x0009E1DE File Offset: 0x0009C3DE
		// (set) Token: 0x060014ED RID: 5357 RVA: 0x0009E1E6 File Offset: 0x0009C3E6
		public ComponentFurnitureInventory ComponentFurnitureInventory { get; set; }

		// Token: 0x060014EE RID: 5358 RVA: 0x0009E1F0 File Offset: 0x0009C3F0
		public FurnitureInventoryPanel(CreativeInventoryWidget creativeInventoryWidget)
		{
			this.m_creativeInventoryWidget = creativeInventoryWidget;
			this.ComponentFurnitureInventory = creativeInventoryWidget.Entity.FindComponent<ComponentFurnitureInventory>(true);
			this.m_componentPlayer = creativeInventoryWidget.Entity.FindComponent<ComponentPlayer>(true);
			this.SubsystemFurnitureBlockBehavior = this.ComponentFurnitureInventory.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			this.SubsystemTerrain = this.ComponentFurnitureInventory.Project.FindSubsystem<SubsystemTerrain>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/FurnitureInventoryPanel", null);
			base.LoadContents(this, node);
			this.m_furnitureSetList = this.Children.Find<ListPanelWidget>("FurnitureSetList", true);
			this.m_inventoryGrid = this.Children.Find<GridPanelWidget>("InventoryGrid", true);
			this.m_addButton = this.Children.Find<ButtonWidget>("AddButton", true);
			this.m_moreButton = this.Children.Find<ButtonWidget>("MoreButton", true);
			for (int i = 0; i < this.m_inventoryGrid.RowsCount; i++)
			{
				for (int j = 0; j < this.m_inventoryGrid.ColumnsCount; j++)
				{
					InventorySlotWidget widget = new InventorySlotWidget();
					this.m_inventoryGrid.Children.Add(widget);
					this.m_inventoryGrid.SetWidgetCell(widget, new Point2(j, i));
				}
			}
			ListPanelWidget furnitureSetList = this.m_furnitureSetList;
			furnitureSetList.ItemWidgetFactory = (Func<object, Widget>)Delegate.Combine(furnitureSetList.ItemWidgetFactory, new Func<object, Widget>((object item) => new FurnitureSetItemWidget(this, (FurnitureSet)item)));
			ListPanelWidget furnitureSetList2 = this.m_furnitureSetList;
			furnitureSetList2.SelectionChanged = (Action)Delegate.Combine(furnitureSetList2.SelectionChanged, new Action(delegate()
			{
				if (!this.m_ignoreSelectionChanged && this.ComponentFurnitureInventory.FurnitureSet != this.m_furnitureSetList.SelectedItem as FurnitureSet)
				{
					this.ComponentFurnitureInventory.PageIndex = 0;
					this.ComponentFurnitureInventory.FurnitureSet = (this.m_furnitureSetList.SelectedItem as FurnitureSet);
					if (this.ComponentFurnitureInventory.FurnitureSet == null)
					{
						this.m_furnitureSetList.SelectedIndex = new int?(0);
					}
					this.AssignInventorySlots();
				}
			}));
			this.m_populateNeeded = true;
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0009E378 File Offset: 0x0009C578
		public override void Update()
		{
			if (this.m_populateNeeded)
			{
				this.Populate();
				this.m_populateNeeded = false;
			}
			if (this.ComponentFurnitureInventory.PageIndex != this.m_assignedPage)
			{
				this.AssignInventorySlots();
			}
			this.m_creativeInventoryWidget.PageUpButton.IsEnabled = (this.ComponentFurnitureInventory.PageIndex > 0);
			this.m_creativeInventoryWidget.PageDownButton.IsEnabled = (this.ComponentFurnitureInventory.PageIndex < this.m_pagesCount - 1);
			this.m_creativeInventoryWidget.PageLabel.Text = ((this.m_pagesCount > 0) ? string.Format("{0}/{1}", this.ComponentFurnitureInventory.PageIndex + 1, this.m_pagesCount) : string.Empty);
			this.m_moreButton.IsEnabled = (this.ComponentFurnitureInventory.FurnitureSet != null);
			if (base.Input.Scroll != null && base.HitTestGlobal(base.Input.Scroll.Value.XY, null).IsChildWidgetOf(this.m_inventoryGrid))
			{
				this.ComponentFurnitureInventory.PageIndex -= (int)base.Input.Scroll.Value.Z;
			}
			if (this.m_creativeInventoryWidget.PageUpButton.IsClicked)
			{
				ComponentFurnitureInventory componentFurnitureInventory = this.ComponentFurnitureInventory;
				int pageIndex = componentFurnitureInventory.PageIndex - 1;
				componentFurnitureInventory.PageIndex = pageIndex;
			}
			if (this.m_creativeInventoryWidget.PageDownButton.IsClicked)
			{
				ComponentFurnitureInventory componentFurnitureInventory2 = this.ComponentFurnitureInventory;
				int pageIndex = componentFurnitureInventory2.PageIndex + 1;
				componentFurnitureInventory2.PageIndex = pageIndex;
			}
			this.ComponentFurnitureInventory.PageIndex = ((this.m_pagesCount > 0) ? MathUtils.Clamp(this.ComponentFurnitureInventory.PageIndex, 0, this.m_pagesCount - 1) : 0);
			if (this.m_addButton.IsClicked)
			{
				List<Tuple<string, Action>> list = new List<Tuple<string, Action>>();
				list.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 6), delegate()
				{
					if (this.SubsystemFurnitureBlockBehavior.FurnitureSets.Count < 32)
					{
						this.NewFurnitureSet();
						return;
					}
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 24), LanguageControl.Get(FurnitureInventoryPanel.fName, 25), LanguageControl.Ok, null, null));
				}));
				list.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 7), delegate()
				{
					this.ImportFurnitureSet(this.SubsystemTerrain);
				}));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 8), list, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
				{
					((Tuple<string, Action>)t).Item2();
				}));
			}
			if (this.m_moreButton.IsClicked && this.ComponentFurnitureInventory.FurnitureSet != null)
			{
				List<Tuple<string, Action>> list2 = new List<Tuple<string, Action>>();
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 9), delegate()
				{
					this.RenameFurnitureSet();
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 10), delegate()
				{
					if (this.SubsystemFurnitureBlockBehavior.GetFurnitureSetDesigns(this.ComponentFurnitureInventory.FurnitureSet).Count<FurnitureDesign>() > 0)
					{
						DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Warning, LanguageControl.Get(FurnitureInventoryPanel.fName, 26), LanguageControl.Get(FurnitureInventoryPanel.fName, 27), LanguageControl.Get(FurnitureInventoryPanel.fName, 28), delegate(MessageDialogButton b)
						{
							if (b == MessageDialogButton.Button1)
							{
								this.DeleteFurnitureSet();
							}
						}));
						return;
					}
					this.DeleteFurnitureSet();
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 11), delegate()
				{
					this.MoveFurnitureSet(-1);
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 12), delegate()
				{
					this.MoveFurnitureSet(1);
				}));
				list2.Add(new Tuple<string, Action>(LanguageControl.Get(FurnitureInventoryPanel.fName, 13), delegate()
				{
					this.ExportFurnitureSet();
				}));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 14), list2, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
				{
					((Tuple<string, Action>)t).Item2();
				}));
			}
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x0009E742 File Offset: 0x0009C942
		public override void UpdateCeases()
		{
			base.UpdateCeases();
			this.ComponentFurnitureInventory.ClearSlots();
			this.m_populateNeeded = true;
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x0009E75C File Offset: 0x0009C95C
		public void Invalidate()
		{
			this.m_populateNeeded = true;
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0009E768 File Offset: 0x0009C968
		public void Populate()
		{
			this.ComponentFurnitureInventory.FillSlots();
			try
			{
				this.m_ignoreSelectionChanged = true;
				this.m_furnitureSetList.ClearItems();
				this.m_furnitureSetList.AddItem(null);
				foreach (FurnitureSet item in this.SubsystemFurnitureBlockBehavior.FurnitureSets)
				{
					this.m_furnitureSetList.AddItem(item);
				}
			}
			finally
			{
				this.m_ignoreSelectionChanged = false;
			}
			this.m_furnitureSetList.SelectedItem = this.ComponentFurnitureInventory.FurnitureSet;
			this.AssignInventorySlots();
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0009E824 File Offset: 0x0009CA24
		public void AssignInventorySlots()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.ComponentFurnitureInventory.SlotsCount; i++)
			{
				int slotValue = this.ComponentFurnitureInventory.GetSlotValue(i);
				int slotCount = this.ComponentFurnitureInventory.GetSlotCount(i);
				if (slotValue != 0 && slotCount > 0 && Terrain.ExtractContents(slotValue) == 227)
				{
					int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(slotValue));
					FurnitureDesign design = this.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
					if (design != null && design.FurnitureSet == this.ComponentFurnitureInventory.FurnitureSet)
					{
						list.Add(i);
					}
				}
			}
			List<InventorySlotWidget> list2 = new List<InventorySlotWidget>((from w in this.m_inventoryGrid.Children
			select w as InventorySlotWidget into w
			where w != null
			select w).Cast<InventorySlotWidget>());
			int num = this.ComponentFurnitureInventory.PageIndex * list2.Count;
			for (int j = 0; j < list2.Count; j++)
			{
				if (num < list.Count)
				{
					list2[j].AssignInventorySlot(this.ComponentFurnitureInventory, list[num]);
				}
				else
				{
					list2[j].AssignInventorySlot(null, 0);
				}
				num++;
			}
			this.m_pagesCount = (list.Count + list2.Count - 1) / list2.Count;
			this.m_assignedPage = this.ComponentFurnitureInventory.PageIndex;
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0009E9AC File Offset: 0x0009CBAC
		public void NewFurnitureSet()
		{
			this.ComponentFurnitureInventory.Entity.FindComponent<ComponentPlayer>(true);
			DialogsManager.ShowDialog(null, new TextBoxDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 15), LanguageControl.Get(FurnitureInventoryPanel.fName, 16), 30, delegate(string s)
			{
				if (s != null)
				{
					FurnitureSet furnitureSet = this.SubsystemFurnitureBlockBehavior.NewFurnitureSet(s, null);
					this.ComponentFurnitureInventory.FurnitureSet = furnitureSet;
					this.Populate();
					this.m_furnitureSetList.ScrollToItem(furnitureSet);
				}
			}));
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0009E9FC File Offset: 0x0009CBFC
		public void DeleteFurnitureSet()
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				int num = this.SubsystemFurnitureBlockBehavior.FurnitureSets.IndexOf(furnitureSet);
				this.SubsystemFurnitureBlockBehavior.DeleteFurnitureSet(furnitureSet);
				this.SubsystemFurnitureBlockBehavior.GarbageCollectDesigns();
				this.ComponentFurnitureInventory.FurnitureSet = ((num > 0) ? this.SubsystemFurnitureBlockBehavior.FurnitureSets[num - 1] : null);
				this.Invalidate();
			}
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0009EA78 File Offset: 0x0009CC78
		public void RenameFurnitureSet()
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				this.ComponentFurnitureInventory.Entity.FindComponent<ComponentPlayer>(true);
				DialogsManager.ShowDialog(null, new TextBoxDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 15), LanguageControl.Get(FurnitureInventoryPanel.fName, 16), 30, delegate(string s)
				{
					if (s != null)
					{
						furnitureSet.Name = s;
						this.Invalidate();
					}
				}));
			}
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0009EAF4 File Offset: 0x0009CCF4
		public void MoveFurnitureSet(int move)
		{
			FurnitureSet furnitureSet = this.m_furnitureSetList.SelectedItem as FurnitureSet;
			if (furnitureSet != null)
			{
				this.SubsystemFurnitureBlockBehavior.MoveFurnitureSet(furnitureSet, move);
				this.Invalidate();
			}
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0009EB28 File Offset: 0x0009CD28
		public void ImportFurnitureSet(SubsystemTerrain subsystemTerrain)
		{
			FurniturePacksManager.UpdateFurniturePacksList();
			if (FurniturePacksManager.FurniturePackNames.Count<string>() == 0)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 18), LanguageControl.Get(FurnitureInventoryPanel.fName, 19), LanguageControl.Ok, null, null));
				return;
			}
			DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new ListSelectionDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 20), FurniturePacksManager.FurniturePackNames, 64f, (object s) => FurniturePacksManager.GetDisplayName((string)s), delegate(object s)
			{
				try
				{
					int num = 0;
					int num2 = 0;
					string text = (string)s;
					List<List<FurnitureDesign>> list = FurnitureDesign.ListChains(FurniturePacksManager.LoadFurniturePack(subsystemTerrain, text));
					List<FurnitureDesign> list2 = new List<FurnitureDesign>();
					this.SubsystemFurnitureBlockBehavior.GarbageCollectDesigns();
					foreach (List<FurnitureDesign> list3 in list)
					{
						FurnitureDesign furnitureDesign = this.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list3[0], false);
						if (furnitureDesign == list3[0])
						{
							list2.Add(furnitureDesign);
						}
						else if (furnitureDesign == null)
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
					if (list2.Count > 0)
					{
						FurnitureSet furnitureSet = this.SubsystemFurnitureBlockBehavior.NewFurnitureSet(FurniturePacksManager.GetDisplayName(text), text);
						foreach (FurnitureDesign design in list2)
						{
							this.SubsystemFurnitureBlockBehavior.AddToFurnitureSet(design, furnitureSet);
						}
						this.ComponentFurnitureInventory.FurnitureSet = furnitureSet;
					}
					this.Invalidate();
					string text2 = string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 1), list2.Count);
					if (num > 0)
					{
						text2 += string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 2), num);
					}
					if (num2 > 0)
					{
						text2 += string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 3), num2, 65535);
					}
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 4), text2.Trim(), LanguageControl.Ok, null, null));
				}
				catch (Exception ex)
				{
					DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 5), ex.Message, LanguageControl.Ok, null, null));
				}
			}));
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0009EBF0 File Offset: 0x0009CDF0
		public void ExportFurnitureSet()
		{
			try
			{
				FurnitureDesign[] designs = this.SubsystemFurnitureBlockBehavior.GetFurnitureSetDesigns(this.ComponentFurnitureInventory.FurnitureSet).ToArray<FurnitureDesign>();
				string displayName = FurniturePacksManager.GetDisplayName(FurniturePacksManager.CreateFurniturePack(this.ComponentFurnitureInventory.FurnitureSet.Name, designs));
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 21), string.Format(LanguageControl.Get(FurnitureInventoryPanel.fName, 22), displayName), LanguageControl.Ok, null, null));
			}
			catch (Exception ex)
			{
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(LanguageControl.Get(FurnitureInventoryPanel.fName, 23), ex.Message, LanguageControl.Ok, null, null));
			}
		}

		// Token: 0x04000DA4 RID: 3492
		public CreativeInventoryWidget m_creativeInventoryWidget;

		// Token: 0x04000DA5 RID: 3493
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000DA6 RID: 3494
		public ListPanelWidget m_furnitureSetList;

		// Token: 0x04000DA7 RID: 3495
		public GridPanelWidget m_inventoryGrid;

		// Token: 0x04000DA8 RID: 3496
		public ButtonWidget m_addButton;

		// Token: 0x04000DA9 RID: 3497
		public ButtonWidget m_moreButton;

		// Token: 0x04000DAA RID: 3498
		public int m_pagesCount;

		// Token: 0x04000DAB RID: 3499
		public int m_assignedPage;

		// Token: 0x04000DAC RID: 3500
		public bool m_ignoreSelectionChanged;

		// Token: 0x04000DAD RID: 3501
		public bool m_populateNeeded;

		// Token: 0x04000DAE RID: 3502
		public static string fName = "FurnitureInventoryPanel";
	}
}
