using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200015E RID: 350
	public class CommunityContentScreen : Screen
	{
		// Token: 0x060007B5 RID: 1973 RVA: 0x0002CA44 File Offset: 0x0002AC44
		public CommunityContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/CommunityContentScreen", null);
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ListPanelWidget>("List", true);
			this.m_orderLabel = this.Children.Find<LabelWidget>("Order", true);
			this.m_changeOrderButton = this.Children.Find<ButtonWidget>("ChangeOrder", true);
			this.m_filterLabel = this.Children.Find<LabelWidget>("Filter", true);
			this.m_changeFilterButton = this.Children.Find<ButtonWidget>("ChangeFilter", true);
			this.m_downloadButton = this.Children.Find<ButtonWidget>("Download", true);
			this.m_deleteButton = this.Children.Find<ButtonWidget>("Delete", true);
			this.m_moreOptionsButton = this.Children.Find<ButtonWidget>("MoreOptions", true);
			this.m_inputKey = this.Children.Find<TextBoxWidget>("key", true);
			this.m_placeHolder = this.Children.Find<LabelWidget>("placeholder", true);
			this.m_searchKey = this.Children.Find<ButtonWidget>("Search", true);
			this.m_listPanel.ItemWidgetFactory = delegate(object item)
			{
				CommunityContentEntry communityContentEntry = item as CommunityContentEntry;
				if (communityContentEntry != null)
				{
					XElement node2 = ContentManager.Get<XElement>("Widgets/CommunityContentItem", null);
					ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
					containerWidget.Children.Find<RectangleWidget>("CommunityContentItem.Icon", true).Subtexture = ExternalContentManager.GetEntryTypeIcon(communityContentEntry.Type);
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.Text", true).Text = communityContentEntry.Name;
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.Details", true).Text = ExternalContentManager.GetEntryTypeDescription(communityContentEntry.Type) + " " + DataSizeFormatter.Format(communityContentEntry.Size);
					containerWidget.Children.Find<StarRatingWidget>("CommunityContentItem.Rating", true).Rating = communityContentEntry.RatingsAverage;
					containerWidget.Children.Find<StarRatingWidget>("CommunityContentItem.Rating", true).IsVisible = (communityContentEntry.RatingsAverage > 0f);
					containerWidget.Children.Find<LabelWidget>("CommunityContentItem.ExtraText", true).Text = communityContentEntry.ExtraText;
					return containerWidget;
				}
				XElement node3 = ContentManager.Get<XElement>("Widgets/CommunityContentItemMore", null);
				ContainerWidget containerWidget2 = (ContainerWidget)Widget.LoadWidget(this, node3, null);
				this.m_moreLink = containerWidget2.Children.Find<LinkWidget>("CommunityContentItemMore.Link", true);
				this.m_moreLink.Tag = (item as string);
				return containerWidget2;
			};
			ListPanelWidget listPanel = this.m_listPanel;
			listPanel.SelectionChanged = (Action)Delegate.Combine(listPanel.SelectionChanged, new Action(delegate()
			{
				if (this.m_listPanel.SelectedItem != null && !(this.m_listPanel.SelectedItem is CommunityContentEntry))
				{
					this.m_listPanel.SelectedItem = null;
				}
			}));
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0002CBB1 File Offset: 0x0002ADB1
		public override void Enter(object[] parameters)
		{
			this.m_filter = string.Empty;
			this.m_order = CommunityContentScreen.Order.ByRank;
			this.m_inputKey.Text = string.Empty;
			this.PopulateList(null);
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0002CBDC File Offset: 0x0002ADDC
		public override void Update()
		{
			this.m_placeHolder.IsVisible = string.IsNullOrEmpty(this.m_inputKey.Text);
			CommunityContentEntry communityContentEntry = this.m_listPanel.SelectedItem as CommunityContentEntry;
			this.m_downloadButton.IsEnabled = (communityContentEntry != null);
			this.m_deleteButton.IsEnabled = (UserManager.ActiveUser != null && communityContentEntry != null && communityContentEntry.UserId == UserManager.ActiveUser.UniqueId);
			this.m_orderLabel.Text = CommunityContentScreen.GetOrderDisplayName(this.m_order);
			this.m_filterLabel.Text = CommunityContentScreen.GetFilterDisplayName(this.m_filter);
			if (this.m_changeOrderButton.IsClicked)
			{
				List<CommunityContentScreen.Order> items = EnumUtils.GetEnumValues(typeof(CommunityContentScreen.Order)).Cast<CommunityContentScreen.Order>().ToList<CommunityContentScreen.Order>();
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"Order Type"
				}), items, 60f, (object item) => CommunityContentScreen.GetOrderDisplayName((CommunityContentScreen.Order)item), delegate(object item)
				{
					this.m_order = (CommunityContentScreen.Order)item;
					this.PopulateList(null);
				}));
			}
			if (this.m_searchKey.IsClicked)
			{
				this.PopulateList(null);
			}
			if (this.m_changeFilterButton.IsClicked)
			{
				List<object> list = new List<object>();
				list.Add(string.Empty);
				foreach (ExternalContentType externalContentType in from ExternalContentType t in EnumUtils.GetEnumValues(typeof(ExternalContentType))
				where ExternalContentManager.IsEntryTypeDownloadSupported(t)
				select t)
				{
					list.Add(externalContentType);
				}
				if (UserManager.ActiveUser != null)
				{
					list.Add(UserManager.ActiveUser.UniqueId);
				}
				DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"Filter"
				}), list, 60f, (object item) => CommunityContentScreen.GetFilterDisplayName(item), delegate(object item)
				{
					this.m_filter = item;
					this.PopulateList(null);
				}));
			}
			if (this.m_downloadButton.IsClicked && communityContentEntry != null)
			{
				this.DownloadEntry(communityContentEntry);
			}
			if (this.m_deleteButton.IsClicked && communityContentEntry != null)
			{
				this.DeleteEntry(communityContentEntry);
			}
			if (this.m_moreOptionsButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new MoreCommunityLinkDialog());
			}
			if (this.m_moreLink != null && this.m_moreLink.IsClicked)
			{
				this.PopulateList((string)this.m_moreLink.Tag);
			}
			if (base.Input.Back || this.Children.Find<BevelledButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
			if (base.Input.Hold != null && base.Input.HoldTime > 2f && base.Input.Hold.Value.Y < 20f)
			{
				this.m_contentExpiryTime = 0.0;
				Task.Delay(250).Wait();
			}
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x0002CF30 File Offset: 0x0002B130
		public void PopulateList(string cursor)
		{
			string text = string.Empty;
			if (SettingsManager.CommunityContentMode == CommunityContentMode.Strict)
			{
				text = "1";
			}
			if (SettingsManager.CommunityContentMode == CommunityContentMode.Normal)
			{
				text = "0";
			}
			string text2 = (this.m_filter is string) ? ((string)this.m_filter) : string.Empty;
			string text3 = (this.m_filter is ExternalContentType) ? LanguageControl.Get(new string[]
			{
				base.GetType().Name,
				this.m_filter.ToString()
			}) : string.Empty;
			string text4 = LanguageControl.Get(new string[]
			{
				base.GetType().Name,
				this.m_order.ToString()
			});
			string cacheKey = string.Concat(new string[]
			{
				text2,
				"\n",
				text3,
				"\n",
				text4,
				"\n",
				text,
				"\n",
				this.m_inputKey.Text
			});
			this.m_moreLink = null;
			if (string.IsNullOrEmpty(cursor))
			{
				this.m_listPanel.ClearItems();
				this.m_listPanel.ScrollPosition = 0f;
				IEnumerable<object> enumerable;
				if (this.m_contentExpiryTime != 0.0 && Time.RealTime < this.m_contentExpiryTime && this.m_itemsCache.TryGetValue(cacheKey, out enumerable))
				{
					foreach (object item in enumerable)
					{
						this.m_listPanel.AddItem(item);
					}
					return;
				}
			}
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(base.GetType().Name, 2), false);
			DialogsManager.ShowDialog(null, busyDialog);
			CommunityContentManager.List(cursor, text2, text3, text, text4, this.m_inputKey.Text, busyDialog.Progress, delegate(List<CommunityContentEntry> list, string nextCursor)
			{
				DialogsManager.HideDialog(busyDialog);
				this.m_contentExpiryTime = Time.RealTime + 300.0;
				while (this.m_listPanel.Items.Count > 0 && !(this.m_listPanel.Items[this.m_listPanel.Items.Count - 1] is CommunityContentEntry))
				{
					this.m_listPanel.RemoveItemAt(this.m_listPanel.Items.Count - 1);
				}
				foreach (CommunityContentEntry item2 in list)
				{
					this.m_listPanel.AddItem(item2);
				}
				if (list.Count > 0 && !string.IsNullOrEmpty(nextCursor))
				{
					this.m_listPanel.AddItem(nextCursor);
				}
				this.m_itemsCache[cacheKey] = new List<object>(this.m_listPanel.Items);
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
			});
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0002D154 File Offset: 0x0002B354
		public void DownloadEntry(CommunityContentEntry entry)
		{
			string userId = (UserManager.ActiveUser != null) ? UserManager.ActiveUser.UniqueId : string.Empty;
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(string.Format(LanguageControl.Get(base.GetType().Name, 1), entry.Name), false);
			DialogsManager.ShowDialog(null, busyDialog);
			CommunityContentManager.Download(entry.Address, entry.Name, entry.Type, userId, busyDialog.Progress, delegate
			{
				DialogsManager.HideDialog(busyDialog);
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
			});
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0002D1F0 File Offset: 0x0002B3F0
		public void DeleteEntry(CommunityContentEntry entry)
		{
			if (UserManager.ActiveUser != null)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(base.GetType().Name, 4), LanguageControl.Get(base.GetType().Name, 5), LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						CancellableBusyDialog busyDialog = new CancellableBusyDialog(string.Format(LanguageControl.Get(this.GetType().Name, 3), entry.Name), false);
						DialogsManager.ShowDialog(null, busyDialog);
						CommunityContentManager.Delete(entry.Address, UserManager.ActiveUser.UniqueId, busyDialog.Progress, delegate
						{
							DialogsManager.HideDialog(busyDialog);
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(this.GetType().Name, 6), LanguageControl.Get(this.GetType().Name, 7), LanguageControl.Ok, null, null));
						}, delegate(Exception error)
						{
							DialogsManager.HideDialog(busyDialog);
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
						});
					}
				}));
			}
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0002D25C File Offset: 0x0002B45C
		public static string GetFilterDisplayName(object filter)
		{
			if (filter is string)
			{
				if (!string.IsNullOrEmpty((string)filter))
				{
					return LanguageControl.Get(typeof(CommunityContentScreen).Name, 8);
				}
				return LanguageControl.Get(typeof(CommunityContentScreen).Name, 9);
			}
			else
			{
				if (filter is ExternalContentType)
				{
					return ExternalContentManager.GetEntryTypeDescription((ExternalContentType)filter);
				}
				throw new InvalidOperationException(LanguageControl.Get(typeof(CommunityContentScreen).Name, 10));
			}
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0002D2DC File Offset: 0x0002B4DC
		public static string GetOrderDisplayName(CommunityContentScreen.Order order)
		{
			if (order == CommunityContentScreen.Order.ByRank)
			{
				return LanguageControl.Get(typeof(CommunityContentScreen).Name, 11);
			}
			if (order != CommunityContentScreen.Order.ByTime)
			{
				throw new InvalidOperationException(LanguageControl.Get(typeof(CommunityContentScreen).Name, 13));
			}
			return LanguageControl.Get(typeof(CommunityContentScreen).Name, 12);
		}

		// Token: 0x0400034A RID: 842
		public ListPanelWidget m_listPanel;

		// Token: 0x0400034B RID: 843
		public LinkWidget m_moreLink;

		// Token: 0x0400034C RID: 844
		public LabelWidget m_orderLabel;

		// Token: 0x0400034D RID: 845
		public ButtonWidget m_changeOrderButton;

		// Token: 0x0400034E RID: 846
		public LabelWidget m_filterLabel;

		// Token: 0x0400034F RID: 847
		public ButtonWidget m_changeFilterButton;

		// Token: 0x04000350 RID: 848
		public ButtonWidget m_downloadButton;

		// Token: 0x04000351 RID: 849
		public ButtonWidget m_deleteButton;

		// Token: 0x04000352 RID: 850
		public ButtonWidget m_moreOptionsButton;

		// Token: 0x04000353 RID: 851
		public ButtonWidget m_searchKey;

		// Token: 0x04000354 RID: 852
		public TextBoxWidget m_inputKey;

		// Token: 0x04000355 RID: 853
		public LabelWidget m_placeHolder;

		// Token: 0x04000356 RID: 854
		public object m_filter;

		// Token: 0x04000357 RID: 855
		public CommunityContentScreen.Order m_order;

		// Token: 0x04000358 RID: 856
		public double m_contentExpiryTime;

		// Token: 0x04000359 RID: 857
		public Dictionary<string, IEnumerable<object>> m_itemsCache = new Dictionary<string, IEnumerable<object>>();

		// Token: 0x02000451 RID: 1105
		public enum Order
		{
			// Token: 0x040015EA RID: 5610
			ByRank,
			// Token: 0x040015EB RID: 5611
			ByTime
		}
	}
}
