using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using Game;

// Token: 0x02000003 RID: 3
public class ModsManageContentScreen : Screen
{
	// Token: 0x0600000A RID: 10 RVA: 0x00002E78 File Offset: 0x00001078
	public ModsManageContentScreen()
	{
		this.androidSystem = false;
		if (this.androidSystem)
		{
			this.m_uninstallPath = this.m_uninstallPath.Replace("app:", "android:/SurvivalCraft2.2");
			this.m_installPath = this.m_installPath.Replace("app:", "android:/SurvivalCraft2.2");
		}
		XElement node = ContentManager.Get<XElement>("Screens/ModsManageContentScreen", null);
		base.LoadContents(this, node);
		this.m_modsContentList = this.Children.Find<ListPanelWidget>("ModsContentList", true);
		this.m_topBarLabel = this.Children.Find<LabelWidget>("TopBar.Label", true);
		this.m_modsContentLabel = this.Children.Find<LabelWidget>("ModsContentLabel", true);
		this.m_filterLabel = this.Children.Find<LabelWidget>("Filter", true);
		this.m_actionButton = this.Children.Find<ButtonWidget>("ActionButton", true);
		this.m_actionButton2 = this.Children.Find<ButtonWidget>("ActionButton2", true);
		this.m_changeFilterButton = this.Children.Find<ButtonWidget>("ChangeFilter", true);
		this.m_upDirectoryButton = this.Children.Find<ButtonWidget>("UpDirectory", true);
		this.m_topBarLabel.Text = LanguageControl.Get(ModsManageContentScreen.fName, 1);
		this.m_filterLabel.Text = LanguageControl.Get(ModsManageContentScreen.fName, 36);
		this.m_modsContentList.ItemWidgetFactory = delegate(object item)
		{
			ModsManageContentScreen.ModItem modItem = (ModsManageContentScreen.ModItem)item;
			XElement node2 = ContentManager.Get<XElement>("Widgets/ExternalContentItem", null);
			ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
			string text = Storage.GetFileName(modItem.ExternalContentEntry.Path);
			string text2 = LanguageControl.Get(ModsManageContentScreen.fName, 2);
			if (modItem.ExternalContentEntry.Type == ExternalContentType.Mod)
			{
				text = modItem.ModInfo.Name;
				text2 = string.Format(LanguageControl.Get(ModsManageContentScreen.fName, 3), modItem.ModInfo.Version, modItem.ModInfo.Author, MathUtils.Round((float)(modItem.ExternalContentEntry.Size / 1000L)));
			}
			containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Text", true).Text = text;
			containerWidget.Children.Find<LabelWidget>("ExternalContentItem.Details", true).Text = text2;
			RectangleWidget rectangleWidget = containerWidget.Children.Find<RectangleWidget>("ExternalContentItem.Icon", true);
			rectangleWidget.Subtexture = modItem.Subtexture;
			rectangleWidget.Size = new Vector2(50f, 50f);
			rectangleWidget.Margin = new Vector2(10f, 10f);
			return containerWidget;
		};
		ListPanelWidget modsContentList = this.m_modsContentList;
		modsContentList.ItemClicked = (Action<object>)Delegate.Combine(modsContentList.ItemClicked, new Action<object>(delegate(object item)
		{
			if (item != null && this.m_modsContentList.SelectedItem == item)
			{
				ModsManageContentScreen.ModItem modItem = (ModsManageContentScreen.ModItem)item;
				if (modItem.ExternalContentEntry.Type == ExternalContentType.Directory && modItem.ExternalContentEntry.Path != this.m_installPath)
				{
					try
					{
						this.SetPath(modItem.ExternalContentEntry.Path);
						this.UpdateList();
						return;
					}
					catch
					{
						DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 4), LanguageControl.Get(ModsManageContentScreen.fName, 5) + "\n" + modItem.ExternalContentEntry.Path, LanguageControl.Get(new string[]
						{
							"Usual",
							"ok"
						}), null, null));
						return;
					}
				}
				if (modItem.ExternalContentEntry.Type == ExternalContentType.Mod && this.m_filter == ModsManageContentScreen.StateFilter.UninstallState)
				{
					string fileName = Storage.GetFileName(modItem.ExternalContentEntry.Path);
					string smallMessage = string.Concat(new string[]
					{
						LanguageControl.Get(ModsManageContentScreen.fName, 6),
						modItem.ModInfo.Description,
						"\n",
						LanguageControl.Get(ModsManageContentScreen.fName, 7),
						modItem.ModInfo.PackageName,
						"，",
						LanguageControl.Get(ModsManageContentScreen.fName, 8)
					});
					DialogsManager.ShowDialog(null, new MessageDialog(fileName, smallMessage, LanguageControl.Get(ModsManageContentScreen.fName, 9), LanguageControl.Get(ModsManageContentScreen.fName, 10), delegate(MessageDialogButton result)
					{
						if (result == MessageDialogButton.Button1)
						{
							Storage.DeleteFile(modItem.ExternalContentEntry.Path);
							this.UpdateList();
						}
					}));
				}
			}
		}));
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00003094 File Offset: 0x00001294
	public override void Enter(object[] parameters)
	{
		if (!Storage.DirectoryExists(this.m_uninstallPath))
		{
			Storage.CreateDirectory(this.m_uninstallPath);
		}
		this.SetPath(this.m_installPath);
		this.m_filter = ModsManageContentScreen.StateFilter.InstallState;
		this.UpdateList();
		this.SetPath(this.m_uninstallPath);
		this.m_filter = ModsManageContentScreen.StateFilter.UninstallState;
		this.UpdateList();
		foreach (ModInfo item in this.m_installModInfo)
		{
			this.m_lastInstallModInfo.Add(item);
		}
		if (this.m_modsContentList.Items.Count == 0)
		{
			List<string> list = new List<string>();
			foreach (string text in this.m_commonPaths)
			{
				if ((this.androidSystem && text.StartsWith("android:")) || (!this.androidSystem && !text.StartsWith("android:")))
				{
					list.Add(text);
				}
			}
			string text2 = LanguageControl.Get(ModsManageContentScreen.fName, 11);
			for (int j = 0; j < list.Count; j++)
			{
				text2 = string.Concat(new string[]
				{
					text2,
					"\n",
					(j + 1).ToString(),
					". ",
					list[j]
				});
			}
			text2 = text2 + "\n\n" + LanguageControl.Get(ModsManageContentScreen.fName, 12);
			if (list.Count == 0)
			{
				text2 = LanguageControl.Get(ModsManageContentScreen.fName, 13);
			}
			DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 14), text2, LanguageControl.Get(ModsManageContentScreen.fName, 15), null, null));
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00003264 File Offset: 0x00001464
	public override void Leave()
	{
		this.m_installModInfo.Clear();
		this.m_lastInstallModInfo.Clear();
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000327C File Offset: 0x0000147C
	public override void Update()
	{
		if (this.m_filter == ModsManageContentScreen.StateFilter.InstallState)
		{
			this.m_upDirectoryButton.IsVisible = false;
			this.m_actionButton2.IsVisible = false;
		}
		else
		{
			this.m_upDirectoryButton.IsVisible = true;
			this.m_upDirectoryButton.IsEnabled = true;
			this.m_actionButton2.IsVisible = true;
			this.m_actionButton2.IsEnabled = true;
			this.m_actionButton2.Text = ((this.m_path == this.m_uninstallPath) ? LanguageControl.Get(ModsManageContentScreen.fName, 16) : LanguageControl.Get(ModsManageContentScreen.fName, 17));
		}
		ModsManageContentScreen.ModItem modItem = null;
		if (this.m_modsContentList.SelectedIndex != null)
		{
			modItem = (this.m_modsContentList.Items[this.m_modsContentList.SelectedIndex.Value] as ModsManageContentScreen.ModItem);
		}
		if (modItem != null && modItem.ExternalContentEntry.Type == ExternalContentType.Mod)
		{
			this.m_actionButton.Text = ((this.m_filter == ModsManageContentScreen.StateFilter.InstallState) ? LanguageControl.Get(ModsManageContentScreen.fName, 18) : LanguageControl.Get(ModsManageContentScreen.fName, 19));
			this.m_actionButton.IsEnabled = true;
		}
		else if (modItem != null && modItem.ExternalContentEntry.Type == ExternalContentType.Directory)
		{
			this.m_actionButton.Text = LanguageControl.Get(ModsManageContentScreen.fName, 20);
			this.m_actionButton.IsEnabled = (modItem.ExternalContentEntry.Path != "android:/Android");
		}
		else
		{
			this.m_actionButton.Text = LanguageControl.Get(ModsManageContentScreen.fName, 21);
			this.m_actionButton.IsEnabled = false;
		}
		if (this.m_actionButton.IsClicked)
		{
			if (modItem != null && modItem.ExternalContentEntry.Type == ExternalContentType.Mod)
			{
				string fileName = Storage.GetFileName(modItem.ExternalContentEntry.Path);
				string text = Storage.CombinePaths(new string[]
				{
					this.m_installPath,
					fileName
				});
				string path = modItem.ExternalContentEntry.Path;
				if (this.m_filter == ModsManageContentScreen.StateFilter.InstallState)
				{
					try
					{
						Storage.DeleteFile(text);
						this.UpdateList();
						DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 22), fileName, LanguageControl.Get(new string[]
						{
							"Usual",
							"ok"
						}), null, null));
						goto IL_405;
					}
					catch (Exception ex)
					{
						Log.Warning(ModsManageContentScreen.fName + ":" + ex.Message);
						goto IL_405;
					}
				}
				bool flag = true;
				using (List<ModInfo>.Enumerator enumerator = this.m_installModInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.PackageName == modItem.ModInfo.PackageName)
						{
							flag = false;
						}
					}
				}
				if (!Storage.FileExists(text) && flag)
				{
					Storage.CopyFile(path, text);
					this.m_installModInfo.Add(modItem.ModInfo);
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 23), fileName, LanguageControl.Get(new string[]
					{
						"Usual",
						"ok"
					}), null, null));
				}
				else
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 24), fileName + LanguageControl.Get(ModsManageContentScreen.fName, 25), LanguageControl.Get(new string[]
					{
						"Usual",
						"ok"
					}), null, null));
				}
			}
			else if (modItem != null && modItem.ExternalContentEntry.Type == ExternalContentType.Directory)
			{
				BusyDialog dialog = new BusyDialog(LanguageControl.Get(ModsManageContentScreen.fName, 26), LanguageControl.Get(ModsManageContentScreen.fName, 27));
				DialogsManager.ShowDialog(null, dialog);
				this.count = 0;
				int num = this.ScanModFile(modItem.ExternalContentEntry.Path);
				DialogsManager.HideDialog(dialog);
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 28), string.Format(LanguageControl.Get(ModsManageContentScreen.fName, 29), num), LanguageControl.Get(ModsManageContentScreen.fName, 30), LanguageControl.Get(ModsManageContentScreen.fName, 31), delegate(MessageDialogButton result)
				{
					if (result == MessageDialogButton.Button1)
					{
						this.SetPath(this.m_uninstallPath);
						this.UpdateList();
					}
				}));
			}
		}
		IL_405:
		if (this.m_actionButton2.IsClicked)
		{
			if (this.m_path == this.m_uninstallPath)
			{
				BusyDialog dialog2 = new BusyDialog(LanguageControl.Get(ModsManageContentScreen.fName, 26), LanguageControl.Get(ModsManageContentScreen.fName, 32));
				DialogsManager.ShowDialog(null, dialog2);
				this.count = 0;
				int num2 = this.FastScanModFile();
				DialogsManager.HideDialog(dialog2);
				if (num2 == 0)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 4), LanguageControl.Get(ModsManageContentScreen.fName, 33), LanguageControl.Get(ModsManageContentScreen.fName, 34), LanguageControl.Get(ModsManageContentScreen.fName, 10), delegate(MessageDialogButton result)
					{
						if (result == MessageDialogButton.Button1)
						{
							WebBrowserManager.LaunchBrowser("https://m.schub.top/com/mods/viewlist");
						}
					}));
				}
				else
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 28), string.Format(LanguageControl.Get(ModsManageContentScreen.fName, 35), num2), LanguageControl.Get(ModsManageContentScreen.fName, 30), null, delegate(MessageDialogButton result)
					{
						this.SetPath(this.m_uninstallPath);
						this.UpdateList();
					}));
				}
			}
			else
			{
				this.SetPath(this.m_uninstallPath);
				this.UpdateList();
			}
		}
		if (this.m_changeFilterButton.IsClicked)
		{
			if (this.m_filter == ModsManageContentScreen.StateFilter.UninstallState)
			{
				this.m_filter = ModsManageContentScreen.StateFilter.InstallState;
				this.SetPath(this.m_installPath);
			}
			else
			{
				this.m_filter = ModsManageContentScreen.StateFilter.UninstallState;
				this.SetPath(this.m_uninstallPath);
			}
			this.UpdateList();
		}
		if (this.m_upDirectoryButton.IsClicked)
		{
			string text2 = Storage.GetDirectoryName(this.m_path);
			if (this.m_path != "android:" && this.m_path != "app:")
			{
				if (text2.StartsWith("system:") && !text2.Contains("/"))
				{
					text2 += "/";
				}
				this.SetPath(text2);
				this.UpdateList();
			}
			if (this.m_path == "app:")
			{
				string text3 = Storage.GetSystemPath(this.m_path);
				text3 = text3.Replace("\\", "/");
				int length = text3.LastIndexOf('/');
				text2 = "system:" + text3.Substring(0, length);
				this.SetPath(text2);
				this.UpdateList();
			}
		}
		if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
		{
			if (this.InstallModChange())
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 4), LanguageControl.Get(ModsManageContentScreen.fName, 38), LanguageControl.Get(ModsManageContentScreen.fName, 39), LanguageControl.Get(ModsManageContentScreen.fName, 31), delegate(MessageDialogButton result)
				{
					if (result == MessageDialogButton.Button1)
					{
						ModsManager.Reboot();
					}
					if (result == MessageDialogButton.Button2)
					{
						ScreensManager.SwitchScreen("Content", Array.Empty<object>());
					}
				}));
				return;
			}
			ScreensManager.SwitchScreen("Content", Array.Empty<object>());
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00003988 File Offset: 0x00001B88
	public void UpdateList()
	{
		this.m_modsContentList.ClearItems();
		this.m_modsContentLabel.Text = LanguageControl.Get(ModsManageContentScreen.fName, 40) + this.SetPathText(this.m_path);
		this.m_filterLabel.Text = ((this.m_filter == ModsManageContentScreen.StateFilter.UninstallState) ? LanguageControl.Get(ModsManageContentScreen.fName, 36) : LanguageControl.Get(ModsManageContentScreen.fName, 37));
		if (this.m_filter == ModsManageContentScreen.StateFilter.InstallState)
		{
			this.m_installModInfo.Clear();
		}
		foreach (string text in Storage.ListFileNames(this.m_path))
		{
			string extension = Storage.GetExtension(text);
			if (!string.IsNullOrEmpty(extension) && extension.ToLower() == ".scmod")
			{
				string path = Storage.CombinePaths(new string[]
				{
					this.m_path,
					text
				});
				Stream stream = Storage.OpenFile(path, OpenFileMode.Read);
				try
				{
					ModEntity modEntity = new ModEntity(ZipArchive.Open(stream, true));
					if (modEntity.modInfo != null && !string.IsNullOrEmpty(modEntity.modInfo.PackageName) && modEntity.modInfo.ApiVersion.Contains("1.4"))
					{
						ExternalContentEntry externalContentEntry = new ExternalContentEntry
						{
							Type = ExternalContentType.Mod,
							Path = path,
							Size = Storage.GetFileSize(path),
							Time = Storage.GetFileLastWriteTime(path)
						};
						Subtexture subtexture = ExternalContentManager.GetEntryTypeIcon(ExternalContentType.Mod);
						if (modEntity.Icon != null)
						{
							subtexture = new Subtexture(modEntity.Icon, Vector2.Zero, Vector2.One);
						}
						if (this.m_filter == ModsManageContentScreen.StateFilter.InstallState)
						{
							this.m_installModInfo.Add(modEntity.modInfo);
						}
						this.m_modsContentList.AddItem(new ModsManageContentScreen.ModItem(modEntity.modInfo, externalContentEntry, subtexture));
					}
				}
				catch
				{
					throw new InvalidOperationException("Mod file acquisition failed");
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
				}
			}
		}
		foreach (string text2 in Storage.ListDirectoryNames(this.m_path))
		{
			string path2 = Storage.CombinePaths(new string[]
			{
				this.m_path,
				text2
			});
			ModInfo modInfo = null;
			Subtexture entryTypeIcon = ExternalContentManager.GetEntryTypeIcon(ExternalContentType.Directory);
			ExternalContentEntry externalContentEntry2 = new ExternalContentEntry
			{
				Type = ExternalContentType.Directory,
				Path = path2,
				Size = 0L,
				Time = Storage.GetFileLastWriteTime(path2)
			};
			this.m_modsContentList.AddItem(new ModsManageContentScreen.ModItem(modInfo, externalContentEntry2, entryTypeIcon));
		}
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00003C70 File Offset: 0x00001E70
	public int ScanModFile(string path)
	{
		foreach (string text in Storage.ListFileNames(path))
		{
			if (!(path == this.m_uninstallPath))
			{
				string extension = Storage.GetExtension(text);
				if (!string.IsNullOrEmpty(extension) && extension.ToLower() == ".scmod")
				{
					string text2 = Storage.CombinePaths(new string[]
					{
						path,
						text
					});
					Stream stream = null;
					try
					{
						ModEntity modEntity = null;
						try
						{
							stream = Storage.OpenFile(text2, OpenFileMode.Read);
							modEntity = new ModEntity(ZipArchive.Open(stream, true));
						}
						catch
						{
						}
						if (stream != null && modEntity != null)
						{
							if (modEntity.modInfo != null && !string.IsNullOrEmpty(modEntity.modInfo.PackageName) && modEntity.modInfo.ApiVersion.Contains("1.4"))
							{
								string text3 = Storage.CombinePaths(new string[]
								{
									this.m_uninstallPath,
									text
								});
								if (!Storage.FileExists(text3))
								{
									Storage.CopyFile(text2, text3);
									this.count++;
								}
							}
						}
					}
					catch
					{
						Log.Error("Mod file scan failed");
						throw new InvalidOperationException("Mod file scan failed");
					}
					finally
					{
						if (stream != null)
						{
							stream.Close();
						}
					}
				}
			}
		}
		foreach (string text4 in Storage.ListDirectoryNames(path))
		{
			this.ScanModFile(Storage.CombinePaths(new string[]
			{
				path,
				text4
			}));
		}
		return this.count;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00003E84 File Offset: 0x00002084
	public int FastScanModFile()
	{
		int num = 0;
		foreach (string text in this.m_commonPaths)
		{
			if ((this.androidSystem && text.StartsWith("android:")) || (!this.androidSystem && !text.StartsWith("android:")))
			{
				try
				{
					if (Storage.DirectoryExists(text))
					{
						this.count = 0;
						int num2 = this.ScanModFile(text);
						num += num2;
					}
				}
				catch
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ModsManageContentScreen.fName, 4), string.Format(LanguageControl.Get(ModsManageContentScreen.fName, 41), text), LanguageControl.Get(ModsManageContentScreen.fName, 15), null, null));
				}
			}
		}
		return num;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00003F48 File Offset: 0x00002148
	public bool InstallModChange()
	{
		bool result = false;
		if (this.m_installModInfo.Count != this.m_lastInstallModInfo.Count)
		{
			return true;
		}
		foreach (ModInfo item in this.m_installModInfo)
		{
			if (!this.m_lastInstallModInfo.Contains(item))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00003FC4 File Offset: 0x000021C4
	public void SetPath(string path)
	{
		path = path.Replace("\\", "/");
		if (path != this.m_path)
		{
			this.m_lastPath = this.m_path;
			this.m_path = path;
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00003FFC File Offset: 0x000021FC
	public string SetPathText(string path)
	{
		string result = Storage.GetSystemPath(path);
		string[] array = path.Split(new char[]
		{
			'/'
		});
		if (array.Length > 5)
		{
			result = string.Concat(new string[]
			{
				".../",
				array[array.Length - 3],
				"/",
				array[array.Length - 2],
				"/",
				array[array.Length - 1]
			});
		}
		return result;
	}

	// Token: 0x0400000B RID: 11
	public static string fName = "ModsManageContentScreen";

	// Token: 0x0400000C RID: 12
	public ListPanelWidget m_modsContentList;

	// Token: 0x0400000D RID: 13
	public LabelWidget m_topBarLabel;

	// Token: 0x0400000E RID: 14
	public LabelWidget m_modsContentLabel;

	// Token: 0x0400000F RID: 15
	public LabelWidget m_filterLabel;

	// Token: 0x04000010 RID: 16
	public ButtonWidget m_actionButton;

	// Token: 0x04000011 RID: 17
	public ButtonWidget m_actionButton2;

	// Token: 0x04000012 RID: 18
	public ButtonWidget m_changeFilterButton;

	// Token: 0x04000013 RID: 19
	public ButtonWidget m_upDirectoryButton;

	// Token: 0x04000014 RID: 20
	public ModsManageContentScreen.StateFilter m_filter;

	// Token: 0x04000015 RID: 21
	public List<ModInfo> m_installModInfo = new List<ModInfo>();

	// Token: 0x04000016 RID: 22
	public List<ModInfo> m_lastInstallModInfo = new List<ModInfo>();

	// Token: 0x04000017 RID: 23
	public int count;

	// Token: 0x04000018 RID: 24
	public bool androidSystem;

	// Token: 0x04000019 RID: 25
	public string m_path;

	// Token: 0x0400001A RID: 26
	public string m_lastPath;

	// Token: 0x0400001B RID: 27
	public string m_uninstallPath = "app:/ModsCache";

	// Token: 0x0400001C RID: 28
	public string m_installPath = "app:/Mods";

	// Token: 0x0400001D RID: 29
	public string[] m_commonPaths = new string[]
	{
		"android:/Download",
		"android:/Android/data/com.tencent.mobileqq/Tencent/QQfile_recv",
		"android:/Android/data/com.tencent.tim/Tencent/TIMfile_recv",
		"android:tencent/TIMfile_recv",
		"android:tencent/QQfile_recv",
		"android:/Quark/Download",
		"android:/BaiduNetdisk",
		"android:/UCDownloads",
		"android:/baidu/searchbox/downloads",
		"android:/SurvivalCraft2.2/Mods"
	};

	// Token: 0x020003C6 RID: 966
	public enum StateFilter
	{
		// Token: 0x0400140F RID: 5135
		UninstallState,
		// Token: 0x04001410 RID: 5136
		InstallState
	}

	// Token: 0x020003C7 RID: 967
	public class ModItem
	{
		// Token: 0x06001DA5 RID: 7589 RVA: 0x000E0D1E File Offset: 0x000DEF1E
		public ModItem(ModInfo ModInfo, ExternalContentEntry ExternalContentEntry, Subtexture Subtexture)
		{
			this.ModInfo = ModInfo;
			this.ExternalContentEntry = ExternalContentEntry;
			this.Subtexture = Subtexture;
		}

		// Token: 0x04001411 RID: 5137
		public ModInfo ModInfo;

		// Token: 0x04001412 RID: 5138
		public ExternalContentEntry ExternalContentEntry;

		// Token: 0x04001413 RID: 5139
		public Subtexture Subtexture;
	}
}
