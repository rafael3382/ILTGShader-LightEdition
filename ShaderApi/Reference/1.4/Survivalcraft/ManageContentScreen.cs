using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Game;

// Token: 0x02000002 RID: 2
public class ManageContentScreen : Screen
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public ManageContentScreen()
	{
		XElement node = ContentManager.Get<XElement>("Screens/ManageContentScreen", null);
		base.LoadContents(this, node);
		this.m_contentList = this.Children.Find<ListPanelWidget>("ContentList", true);
		this.m_deleteButton = this.Children.Find<ButtonWidget>("DeleteButton", true);
		this.m_uploadButton = this.Children.Find<ButtonWidget>("UploadButton", true);
		this.m_changeFilterButton = this.Children.Find<ButtonWidget>("ChangeFilter", true);
		this.m_filterLabel = this.Children.Find<LabelWidget>("Filter", true);
		this.m_contentList.ItemWidgetFactory = delegate(object obj)
		{
			ManageContentScreen.ListItem listItem = (ManageContentScreen.ListItem)obj;
			ContainerWidget containerWidget;
			switch (listItem.Type)
			{
			case ExternalContentType.BlocksTexture:
			{
				XElement node2 = ContentManager.Get<XElement>("Widgets/BlocksTextureItem", null);
				containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				RectangleWidget rectangleWidget = containerWidget.Children.Find<RectangleWidget>("BlocksTextureItem.Icon", true);
				LabelWidget labelWidget = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Text", true);
				LabelWidget labelWidget2 = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Details", true);
				Texture2D texture = this.m_blocksTexturesCache.GetTexture(listItem.Name);
				BlocksTexturesManager.GetCreationDate(listItem.Name);
				rectangleWidget.Subtexture = new Subtexture(texture, Vector2.Zero, Vector2.One);
				labelWidget.Text = listItem.DisplayName;
				labelWidget2.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 1), texture.Width, texture.Height);
				if (listItem.IsBuiltIn)
				{
					return containerWidget;
				}
				LabelWidget labelWidget3 = labelWidget2;
				labelWidget3.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
				if (listItem.UseCount > 0)
				{
					LabelWidget labelWidget4 = labelWidget2;
					labelWidget4.Text += string.Format(LanguageControl.Get(ManageContentScreen.fName, 2), listItem.UseCount);
					return containerWidget;
				}
				return containerWidget;
			}
			case ExternalContentType.CharacterSkin:
				break;
			case ExternalContentType.FurniturePack:
			{
				XElement node3 = ContentManager.Get<XElement>("Widgets/FurniturePackItem", null);
				containerWidget = (ContainerWidget)Widget.LoadWidget(this, node3, null);
				LabelWidget labelWidget5 = containerWidget.Children.Find<LabelWidget>("FurniturePackItem.Text", true);
				LabelWidget labelWidget6 = containerWidget.Children.Find<LabelWidget>("FurniturePackItem.Details", true);
				labelWidget5.Text = listItem.DisplayName;
				try
				{
					List<FurnitureDesign> designs = FurniturePacksManager.LoadFurniturePack(null, listItem.Name);
					labelWidget6.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 3), FurnitureDesign.ListChains(designs).Count);
					if (string.IsNullOrEmpty(listItem.Name))
					{
						return containerWidget;
					}
					LabelWidget labelWidget7 = labelWidget6;
					labelWidget7.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
					return containerWidget;
				}
				catch (Exception ex)
				{
					labelWidget6.Text = labelWidget6.Text + LanguageControl.Error + ex.Message;
					return containerWidget;
				}
				break;
			}
			case ExternalContentType.Mod:
			{
				XElement node4 = ContentManager.Get<XElement>("Widgets/BlocksTextureItem", null);
				containerWidget = (ContainerWidget)Widget.LoadWidget(this, node4, null);
				RectangleWidget rectangleWidget2 = containerWidget.Children.Find<RectangleWidget>("BlocksTextureItem.Icon", true);
				LabelWidget labelWidget8 = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Text", true);
				LabelWidget labelWidget9 = containerWidget.Children.Find<LabelWidget>("BlocksTextureItem.Details", true);
				rectangleWidget2.Subtexture = ((listItem.Texture == null) ? ContentManager.Get<Subtexture>("Textures/Atlas/WorldIcon", null) : new Subtexture(listItem.Texture, Vector2.Zero, Vector2.One));
				rectangleWidget2.TextureLinearFilter = true;
				labelWidget8.Text = listItem.DisplayName;
				labelWidget9.Text = listItem.Name;
				if (listItem.IsBuiltIn)
				{
					return containerWidget;
				}
				LabelWidget labelWidget10 = labelWidget9;
				labelWidget10.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
				if (listItem.UseCount > 0)
				{
					LabelWidget labelWidget11 = labelWidget9;
					labelWidget11.Text += string.Format(LanguageControl.Get(ManageContentScreen.fName, 2), listItem.UseCount);
					return containerWidget;
				}
				return containerWidget;
			}
			default:
				throw new InvalidOperationException(LanguageControl.Get(ManageContentScreen.fName, 10));
			}
			XElement node5 = ContentManager.Get<XElement>("Widgets/CharacterSkinItem", null);
			containerWidget = (ContainerWidget)Widget.LoadWidget(this, node5, null);
			PlayerModelWidget playerModelWidget = containerWidget.Children.Find<PlayerModelWidget>("CharacterSkinItem.Model", true);
			LabelWidget labelWidget12 = containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Text", true);
			LabelWidget labelWidget13 = containerWidget.Children.Find<LabelWidget>("CharacterSkinItem.Details", true);
			Texture2D texture2 = this.m_characterSkinsCache.GetTexture(listItem.Name);
			playerModelWidget.PlayerClass = PlayerClass.Male;
			playerModelWidget.CharacterSkinTexture = texture2;
			labelWidget12.Text = listItem.DisplayName;
			labelWidget13.Text = string.Format(LanguageControl.Get(ManageContentScreen.fName, 4), texture2.Width, texture2.Height);
			if (!listItem.IsBuiltIn)
			{
				LabelWidget labelWidget14 = labelWidget13;
				labelWidget14.Text += string.Format(" | {0:dd MMM yyyy HH:mm}", listItem.CreationTime.ToLocalTime());
				if (listItem.UseCount > 0)
				{
					LabelWidget labelWidget15 = labelWidget13;
					labelWidget15.Text += string.Format(LanguageControl.Get(ManageContentScreen.fName, 2), listItem.UseCount);
				}
			}
			return containerWidget;
		};
		ListPanelWidget contentList = this.m_contentList;
		contentList.ItemClicked = (Action<object>)Delegate.Combine(contentList.ItemClicked, new Action<object>(delegate(object obj)
		{
			ManageContentScreen.ListItem listItem = (ManageContentScreen.ListItem)obj;
			if (listItem.Type == ExternalContentType.Mod && listItem.IsClick)
			{
				MessageDialog dialog = new MessageDialog(listItem.ModEntity.modInfo.Name, listItem.ModEntity.modInfo.Description, LanguageControl.Ok, LanguageControl.Cancel, delegate(MessageDialogButton btn)
				{
					DialogsManager.HideAllDialogs();
					listItem.IsClick = false;
				});
				DialogsManager.ShowDialog(this, dialog);
				return;
			}
			listItem.IsClick = true;
		}));
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000213E File Offset: 0x0000033E
	public override void Enter(object[] parameters)
	{
		this.UpdateList();
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002146 File Offset: 0x00000346
	public override void Leave()
	{
		this.m_blocksTexturesCache.Clear();
		this.m_characterSkinsCache.Clear();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002160 File Offset: 0x00000360
	public override void Update()
	{
		ManageContentScreen.ListItem selectedItem = (ManageContentScreen.ListItem)this.m_contentList.SelectedItem;
		if (selectedItem != null)
		{
			this.m_deleteButton.IsEnabled = !selectedItem.IsBuiltIn;
			this.m_uploadButton.IsEnabled = !selectedItem.IsBuiltIn;
			if (selectedItem.Type == ExternalContentType.Mod)
			{
				this.m_deleteButton.Text = (ModsManager.DisabledMods.Contains(selectedItem.ModEntity.modInfo) ? LanguageControl.Enable : LanguageControl.Disable);
				this.m_deleteButton.IsEnabled = (!(selectedItem.ModEntity is SurvivalCrafModEntity) && !(selectedItem.ModEntity is FastDebugModEntity));
			}
			else
			{
				this.m_deleteButton.Text = LanguageControl.Delete;
			}
		}
		this.m_filterLabel.Text = ManageContentScreen.GetFilterDisplayName(this.m_filter);
		if (this.m_deleteButton.IsClicked)
		{
			string smallMessage = (selectedItem.UseCount <= 0) ? string.Format(LanguageControl.Get(ManageContentScreen.fName, 5), selectedItem.DisplayName) : string.Format(LanguageControl.Get(ManageContentScreen.fName, 6), selectedItem.DisplayName, selectedItem.UseCount);
			if (selectedItem.Type == ExternalContentType.Mod)
			{
				smallMessage = (ModsManager.DisabledMods.Contains(selectedItem.ModEntity.modInfo) ? LanguageControl.Enable : LanguageControl.Disable) + "[" + selectedItem.ModEntity.modInfo.Name + "]?";
			}
			DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ManageContentScreen.fName, 9), smallMessage, LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton button)
			{
				if (button == MessageDialogButton.Button1)
				{
					if (selectedItem.Type == ExternalContentType.Mod)
					{
						this.changeed = true;
						if (ModsManager.DisabledMods.Contains(selectedItem.ModEntity.modInfo))
						{
							ModsManager.DisabledMods.Remove(selectedItem.ModEntity.modInfo);
							ModsManager.ModList.Add(selectedItem.ModEntity);
						}
						else
						{
							ModsManager.DisabledMods.Add(selectedItem.ModEntity.modInfo);
							ModsManager.ModList.Remove(selectedItem.ModEntity);
						}
					}
					else
					{
						ExternalContentManager.DeleteExternalContent(selectedItem.Type, selectedItem.Name);
					}
					this.UpdateList();
				}
			}));
		}
		if (this.m_uploadButton.IsClicked)
		{
			ExternalContentManager.ShowUploadUi(selectedItem.Type, selectedItem.Name);
		}
		if (this.m_changeFilterButton.IsClicked)
		{
			List<ExternalContentType> list = new List<ExternalContentType>();
			list.Add(ExternalContentType.Unknown);
			list.Add(ExternalContentType.BlocksTexture);
			list.Add(ExternalContentType.CharacterSkin);
			list.Add(ExternalContentType.FurniturePack);
			DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.Get(ManageContentScreen.fName, 7), list, 60f, (object item) => ManageContentScreen.GetFilterDisplayName((ExternalContentType)item), delegate(object item)
			{
				if ((ExternalContentType)item != this.m_filter)
				{
					this.m_filter = (ExternalContentType)item;
					this.UpdateList();
				}
			}));
		}
		if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
		{
			if (this.changeed)
			{
				DialogsManager.ShowDialog(this, new MessageDialog(LanguageControl.Warning, LanguageControl.Get(base.GetType().Name, 11), LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton btn)
				{
					DialogsManager.HideAllDialogs();
					if (btn == MessageDialogButton.Button1)
					{
						ModsManager.Reboot();
						return;
					}
					ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
				}));
				return;
			}
			ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
		}
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002490 File Offset: 0x00000690
	public void UpdateList()
	{
		WorldsManager.UpdateWorldsList();
		List<ManageContentScreen.ListItem> list = new List<ManageContentScreen.ListItem>();
		if (this.m_filter == ExternalContentType.BlocksTexture || this.m_filter == ExternalContentType.Unknown)
		{
			BlocksTexturesManager.UpdateBlocksTexturesList();
			foreach (string name2 in BlocksTexturesManager.BlockTexturesNames)
			{
				string name2;
				list.Add(new ManageContentScreen.ListItem
				{
					Name = name2,
					IsBuiltIn = BlocksTexturesManager.IsBuiltIn(name2),
					Type = ExternalContentType.BlocksTexture,
					DisplayName = BlocksTexturesManager.GetDisplayName(name2),
					CreationTime = BlocksTexturesManager.GetCreationDate(name2),
					UseCount = WorldsManager.WorldInfos.Count((WorldInfo wi) => wi.WorldSettings.BlocksTextureName == name2)
				});
			}
		}
		if (this.m_filter == ExternalContentType.CharacterSkin || this.m_filter == ExternalContentType.Unknown)
		{
			CharacterSkinsManager.UpdateCharacterSkinsList();
			using (ReadOnlyList<string>.Enumerator enumerator = CharacterSkinsManager.CharacterSkinsNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string name = enumerator.Current;
					Func<PlayerInfo, bool> <>9__3;
					list.Add(new ManageContentScreen.ListItem
					{
						Name = name,
						IsBuiltIn = CharacterSkinsManager.IsBuiltIn(name),
						Type = ExternalContentType.CharacterSkin,
						DisplayName = CharacterSkinsManager.GetDisplayName(name),
						CreationTime = CharacterSkinsManager.GetCreationDate(name),
						UseCount = WorldsManager.WorldInfos.Count(delegate(WorldInfo wi)
						{
							IEnumerable<PlayerInfo> playerInfos = wi.PlayerInfos;
							Func<PlayerInfo, bool> predicate;
							if ((predicate = <>9__3) == null)
							{
								predicate = (<>9__3 = ((PlayerInfo pi) => pi.CharacterSkinName == name));
							}
							return playerInfos.Any(predicate);
						})
					});
				}
			}
		}
		if (this.m_filter == ExternalContentType.FurniturePack || this.m_filter == ExternalContentType.Unknown)
		{
			FurniturePacksManager.UpdateFurniturePacksList();
			foreach (string name2 in FurniturePacksManager.FurniturePackNames)
			{
				list.Add(new ManageContentScreen.ListItem
				{
					Name = name2,
					IsBuiltIn = false,
					Type = ExternalContentType.FurniturePack,
					DisplayName = FurniturePacksManager.GetDisplayName(name2),
					CreationTime = FurniturePacksManager.GetCreationDate(name2)
				});
			}
		}
		if (this.m_filter == ExternalContentType.Mod || this.m_filter == ExternalContentType.Unknown)
		{
			foreach (ModEntity modEntity in ModsManager.ModListAll)
			{
				string str = string.Empty;
				if (ModsManager.DisabledMods.Contains(modEntity.modInfo))
				{
					str = "[已禁用]";
				}
				string text = string.IsNullOrEmpty(modEntity.modInfo.Author) ? "无" : modEntity.modInfo.Author;
				list.Add(new ManageContentScreen.ListItem
				{
					Name = string.Concat(new string[]
					{
						"[模组]",
						modEntity.modInfo.Description,
						"<",
						text,
						">"
					}),
					IsBuiltIn = false,
					Type = ExternalContentType.Mod,
					DisplayName = str + modEntity.modInfo.Name + " 版本:" + modEntity.modInfo.Version,
					CreationTime = DateTime.Now,
					Texture = modEntity.Icon,
					ModEntity = modEntity
				});
			}
		}
		list.Sort(delegate(ManageContentScreen.ListItem o1, ManageContentScreen.ListItem o2)
		{
			if (o1.IsBuiltIn && !o2.IsBuiltIn)
			{
				return -1;
			}
			if (o2.IsBuiltIn && !o1.IsBuiltIn)
			{
				return 1;
			}
			if (string.IsNullOrEmpty(o1.Name) && !string.IsNullOrEmpty(o2.Name))
			{
				return -1;
			}
			if (string.IsNullOrEmpty(o1.Name) || !string.IsNullOrEmpty(o2.Name))
			{
				return string.Compare(o1.DisplayName, o2.DisplayName);
			}
			return 1;
		});
		this.m_contentList.ClearItems();
		foreach (ManageContentScreen.ListItem item in list)
		{
			this.m_contentList.AddItem(item);
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000028F0 File Offset: 0x00000AF0
	public static string GetFilterDisplayName(ExternalContentType filter)
	{
		if (filter != ExternalContentType.Unknown)
		{
			return ExternalContentManager.GetEntryTypeDescription(filter);
		}
		return LanguageControl.Get(ManageContentScreen.fName, 8);
	}

	// Token: 0x04000001 RID: 1
	public static string fName = "ManageContentScreen";

	// Token: 0x04000002 RID: 2
	public ListPanelWidget m_contentList;

	// Token: 0x04000003 RID: 3
	public ButtonWidget m_deleteButton;

	// Token: 0x04000004 RID: 4
	public ButtonWidget m_uploadButton;

	// Token: 0x04000005 RID: 5
	public LabelWidget m_filterLabel;

	// Token: 0x04000006 RID: 6
	public ButtonWidget m_changeFilterButton;

	// Token: 0x04000007 RID: 7
	public BlocksTexturesCache m_blocksTexturesCache = new BlocksTexturesCache();

	// Token: 0x04000008 RID: 8
	public CharacterSkinsCache m_characterSkinsCache = new CharacterSkinsCache();

	// Token: 0x04000009 RID: 9
	public bool changeed;

	// Token: 0x0400000A RID: 10
	public ExternalContentType m_filter;

	// Token: 0x020003C0 RID: 960
	public class ListItem
	{
		// Token: 0x040013FB RID: 5115
		public ExternalContentType Type;

		// Token: 0x040013FC RID: 5116
		public bool IsBuiltIn;

		// Token: 0x040013FD RID: 5117
		public string Name;

		// Token: 0x040013FE RID: 5118
		public string DisplayName;

		// Token: 0x040013FF RID: 5119
		public DateTime CreationTime;

		// Token: 0x04001400 RID: 5120
		public int UseCount;

		// Token: 0x04001401 RID: 5121
		public bool IsClick;

		// Token: 0x04001402 RID: 5122
		public Texture2D Texture;

		// Token: 0x04001403 RID: 5123
		public ModEntity ModEntity;
	}
}
