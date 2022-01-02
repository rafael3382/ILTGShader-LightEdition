using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000160 RID: 352
	public class ExternalContentScreen : Screen
	{
		// Token: 0x060007C4 RID: 1988 RVA: 0x0002D698 File Offset: 0x0002B898
		public ExternalContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ExternalContentScreen", null);
			base.LoadContents(this, node);
			this.m_directoryLabel = this.Children.Find<LabelWidget>("TopBar.Label", true);
			this.m_directoryList = this.Children.Find<ListPanelWidget>("DirectoryList", true);
			this.m_providerNameLabel = this.Children.Find<LabelWidget>("ProviderName", true);
			this.m_changeProviderButton = this.Children.Find<ButtonWidget>("ChangeProvider", true);
			this.m_loginLogoutButton = this.Children.Find<ButtonWidget>("LoginLogout", true);
			this.m_upDirectoryButton = this.Children.Find<ButtonWidget>("UpDirectory", true);
			this.m_actionButton = this.Children.Find<ButtonWidget>("Action", true);
			this.m_copyLinkButton = this.Children.Find<ButtonWidget>("CopyLink", true);
			this.m_directoryList.ItemWidgetFactory = delegate(object item)
			{
				ExternalContentEntry externalContentEntry = (ExternalContentEntry)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/ExternalContentItem", null);
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				string fileName = Storage.GetFileName(externalContentEntry.Path);
				string text = this.m_downloadedFiles.ContainsKey(externalContentEntry.Path) ? LanguageControl.Get(base.GetType().Name, 11) : string.Empty;
				string text2 = (externalContentEntry.Type != ExternalContentType.Directory) ? string.Format("{0} | {1} | {2:dd-MMM-yyyy HH:mm}{3}", new object[]
				{
					ExternalContentManager.GetEntryTypeDescription(externalContentEntry.Type),
					DataSizeFormatter.Format(externalContentEntry.Size),
					externalContentEntry.Time,
					text
				}) : ExternalContentManager.GetEntryTypeDescription(externalContentEntry.Type);
				containerWidget.Children.Find<RectangleWidget>("ExternalContentItem.Icon", true).Subtexture = ExternalContentManager.GetEntryTypeIcon(externalContentEntry.Type);
				containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Text", true).Text = fileName;
				containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Details", true).Text = text2;
				return containerWidget;
			};
			ListPanelWidget directoryList = this.m_directoryList;
			directoryList.ItemClicked = (Action<object>)Delegate.Combine(directoryList.ItemClicked, new Action<object>(delegate(object item)
			{
				if (this.m_directoryList.SelectedItem == item)
				{
					ExternalContentEntry externalContentEntry = item as ExternalContentEntry;
					if (externalContentEntry != null && externalContentEntry.Type == ExternalContentType.Directory)
					{
						this.SetPath(externalContentEntry.Path);
					}
				}
			}));
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0002D7CB File Offset: 0x0002B9CB
		public override void Enter(object[] parameters)
		{
			this.m_directoryList.ClearItems();
			this.SetPath(null);
			this.m_listDirty = true;
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0002D7E8 File Offset: 0x0002B9E8
		public override void Update()
		{
			if (this.m_listDirty)
			{
				this.m_listDirty = false;
				this.UpdateList();
			}
			ExternalContentEntry externalContentEntry = null;
			if (this.m_directoryList.SelectedIndex != null)
			{
				externalContentEntry = (this.m_directoryList.Items[this.m_directoryList.SelectedIndex.Value] as ExternalContentEntry);
			}
			if (externalContentEntry != null)
			{
				this.m_actionButton.IsVisible = true;
				if (externalContentEntry.Type == ExternalContentType.Directory)
				{
					this.m_actionButton.Text = LanguageControl.Get(base.GetType().Name, 1);
					this.m_actionButton.IsEnabled = true;
					this.m_copyLinkButton.IsEnabled = false;
				}
				else
				{
					this.m_actionButton.Text = LanguageControl.Get(base.GetType().Name, 2);
					if (ExternalContentManager.IsEntryTypeDownloadSupported(ExternalContentManager.ExtensionToType(Storage.GetExtension(externalContentEntry.Path).ToLower())))
					{
						this.m_actionButton.IsEnabled = true;
						this.m_copyLinkButton.IsEnabled = true;
					}
					else
					{
						this.m_actionButton.IsEnabled = false;
						this.m_copyLinkButton.IsEnabled = false;
					}
				}
			}
			else
			{
				this.m_actionButton.IsVisible = false;
				this.m_copyLinkButton.IsVisible = false;
			}
			this.m_directoryLabel.Text = (this.m_externalContentProvider.IsLoggedIn ? string.Format(LanguageControl.Get(base.GetType().Name, 3), this.m_path) : LanguageControl.Get(base.GetType().Name, 4));
			this.m_providerNameLabel.Text = this.m_externalContentProvider.DisplayName;
			this.m_upDirectoryButton.IsEnabled = (this.m_externalContentProvider.IsLoggedIn && this.m_path != "/");
			this.m_loginLogoutButton.Text = (this.m_externalContentProvider.IsLoggedIn ? LanguageControl.Get(base.GetType().Name, 5) : LanguageControl.Get(base.GetType().Name, 6));
			this.m_loginLogoutButton.IsVisible = this.m_externalContentProvider.RequiresLogin;
			this.m_copyLinkButton.IsVisible = this.m_externalContentProvider.SupportsLinks;
			this.m_copyLinkButton.IsEnabled = (externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type));
			if (this.m_changeProviderButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new SelectExternalContentProviderDialog(LanguageControl.Get(base.GetType().Name, 7), true, delegate(IExternalContentProvider provider)
				{
					this.m_externalContentProvider = provider;
					this.m_listDirty = true;
					this.SetPath(null);
				}));
			}
			if (this.m_upDirectoryButton.IsClicked)
			{
				string directoryName = Storage.GetDirectoryName(this.m_path);
				this.SetPath(directoryName);
			}
			if (this.m_actionButton.IsClicked && externalContentEntry != null)
			{
				if (externalContentEntry.Type == ExternalContentType.Directory)
				{
					this.SetPath(externalContentEntry.Path);
				}
				else
				{
					this.DownloadEntry(externalContentEntry);
				}
			}
			if (this.m_copyLinkButton.IsClicked && externalContentEntry != null && ExternalContentManager.IsEntryTypeDownloadSupported(externalContentEntry.Type))
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(base.GetType().Name, 8), false);
				DialogsManager.ShowDialog(null, busyDialog);
				this.m_externalContentProvider.Link(externalContentEntry.Path, busyDialog.Progress, delegate(string link)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new ExternalContentLinkDialog(link));
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
				});
			}
			if (this.m_loginLogoutButton.IsClicked)
			{
				if (this.m_externalContentProvider.IsLoggedIn)
				{
					this.m_externalContentProvider.Logout();
					this.SetPath(null);
					this.m_listDirty = true;
				}
				else
				{
					ExternalContentManager.ShowLoginUiIfNeeded(this.m_externalContentProvider, false, delegate
					{
						this.SetPath(null);
						this.m_listDirty = true;
					});
				}
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x0002DBC9 File Offset: 0x0002BDC9
		public void SetPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = DiskExternalContentProvider.LocalPath;
			}
			path = path.Replace("\\", "/");
			if (path != this.m_path)
			{
				this.m_path = path;
				this.m_listDirty = true;
			}
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0002DC08 File Offset: 0x0002BE08
		public void UpdateList()
		{
			this.m_directoryList.ClearItems();
			if (this.m_externalContentProvider != null && this.m_externalContentProvider.IsLoggedIn)
			{
				CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(base.GetType().Name, 9), false);
				DialogsManager.ShowDialog(null, busyDialog);
				this.m_externalContentProvider.List(this.m_path, busyDialog.Progress, delegate(ExternalContentEntry entry)
				{
					DialogsManager.HideDialog(busyDialog);
					List<ExternalContentEntry> list = new List<ExternalContentEntry>((from e in entry.ChildEntries
					where ExternalContentScreen.EntryFilter(e)
					select e).Take(1000));
					this.m_directoryList.ClearItems();
					list.Sort(delegate(ExternalContentEntry e1, ExternalContentEntry e2)
					{
						if (e1.Type == ExternalContentType.Directory && e2.Type != ExternalContentType.Directory)
						{
							return -1;
						}
						if (e1.Type == ExternalContentType.Directory || e2.Type != ExternalContentType.Directory)
						{
							return string.Compare(e1.Path, e2.Path);
						}
						return 1;
					});
					foreach (ExternalContentEntry item in list)
					{
						this.m_directoryList.AddItem(item);
					}
				}, delegate(Exception error)
				{
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
				});
			}
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x0002DCA0 File Offset: 0x0002BEA0
		public void DownloadEntry(ExternalContentEntry entry)
		{
			CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(base.GetType().Name, 10), false);
			DialogsManager.ShowDialog(null, busyDialog);
			this.m_externalContentProvider.Download(entry.Path, busyDialog.Progress, delegate(Stream stream)
			{
				busyDialog.LargeMessage = LanguageControl.Get(this.GetType().Name, 12);
				ExternalContentManager.ImportExternalContent(stream, entry.Type, Storage.GetFileName(entry.Path), delegate
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
				}, delegate(Exception error)
				{
					stream.Dispose();
					DialogsManager.HideDialog(busyDialog);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
				});
			}, delegate(Exception error)
			{
				DialogsManager.HideDialog(busyDialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
			});
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x0002DD24 File Offset: 0x0002BF24
		public static bool EntryFilter(ExternalContentEntry entry)
		{
			return entry.Type > ExternalContentType.Unknown;
		}

		// Token: 0x0400035F RID: 863
		public LabelWidget m_directoryLabel;

		// Token: 0x04000360 RID: 864
		public ListPanelWidget m_directoryList;

		// Token: 0x04000361 RID: 865
		public LabelWidget m_providerNameLabel;

		// Token: 0x04000362 RID: 866
		public ButtonWidget m_changeProviderButton;

		// Token: 0x04000363 RID: 867
		public ButtonWidget m_loginLogoutButton;

		// Token: 0x04000364 RID: 868
		public ButtonWidget m_upDirectoryButton;

		// Token: 0x04000365 RID: 869
		public ButtonWidget m_actionButton;

		// Token: 0x04000366 RID: 870
		public ButtonWidget m_copyLinkButton;

		// Token: 0x04000367 RID: 871
		public string m_path;

		// Token: 0x04000368 RID: 872
		public bool m_listDirty;

		// Token: 0x04000369 RID: 873
		public Dictionary<string, bool> m_downloadedFiles = new Dictionary<string, bool>();

		// Token: 0x0400036A RID: 874
		public IExternalContentProvider m_externalContentProvider = ExternalContentManager.DefaultProvider;
	}
}
