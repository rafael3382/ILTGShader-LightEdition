using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200016C RID: 364
	public class PlayScreen : Screen
	{
		// Token: 0x0600083E RID: 2110 RVA: 0x00031190 File Offset: 0x0002F390
		public PlayScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/PlayScreen", null);
			base.LoadContents(this, node);
			this.m_worldsListWidget = this.Children.Find<ListPanelWidget>("WorldsList", true);
			ListPanelWidget worldsListWidget = this.m_worldsListWidget;
			worldsListWidget.ItemWidgetFactory = (Func<object, Widget>)Delegate.Combine(worldsListWidget.ItemWidgetFactory, new Func<object, Widget>(delegate(object item)
			{
				WorldInfo worldInfo = (WorldInfo)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/SavedWorldItem", null);
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				LabelWidget labelWidget = containerWidget.Children.Find<LabelWidget>("WorldItem.Name", true);
				LabelWidget labelWidget2 = containerWidget.Children.Find<LabelWidget>("WorldItem.Details", true);
				containerWidget.Tag = worldInfo;
				labelWidget.Text = worldInfo.WorldSettings.Name;
				labelWidget2.Text = string.Format("{0} | {1:dd MMM yyyy HH:mm} | {2} | {3} | {4}", new object[]
				{
					DataSizeFormatter.Format(worldInfo.Size),
					worldInfo.LastSaveTime.ToLocalTime(),
					(worldInfo.PlayerInfos.Count > 1) ? string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 9), worldInfo.PlayerInfos.Count) : string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 10), 1),
					LanguageControl.Get(new string[]
					{
						"GameMode",
						worldInfo.WorldSettings.GameMode.ToString()
					}),
					LanguageControl.Get(new string[]
					{
						"EnvironmentBehaviorMode",
						worldInfo.WorldSettings.EnvironmentBehaviorMode.ToString()
					})
				});
				if (worldInfo.SerializationVersion != VersionsManager.SerializationVersion)
				{
					labelWidget2.Text = labelWidget2.Text + " | " + (string.IsNullOrEmpty(worldInfo.SerializationVersion) ? LanguageControl.GetContentWidgets("Usual", "Unknown") : ("(" + worldInfo.SerializationVersion + ")"));
				}
				return containerWidget;
			}));
			this.m_worldsListWidget.ScrollPosition = 0f;
			this.m_worldsListWidget.ScrollSpeed = 0f;
			ListPanelWidget worldsListWidget2 = this.m_worldsListWidget;
			worldsListWidget2.ItemClicked = (Action<object>)Delegate.Combine(worldsListWidget2.ItemClicked, new Action<object>(delegate(object item)
			{
				if (item != null && this.m_worldsListWidget.SelectedItem == item)
				{
					this.Play(item);
				}
			}));
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0003123C File Offset: 0x0002F43C
		public override void Enter(object[] parameters)
		{
			BusyDialog dialog = new BusyDialog(LanguageControl.GetContentWidgets(PlayScreen.fName, 5), null);
			DialogsManager.ShowDialog(null, dialog);
			Task.Run(delegate()
			{
				WorldInfo selectedItem = (WorldInfo)this.m_worldsListWidget.SelectedItem;
				WorldsManager.UpdateWorldsList();
				List<WorldInfo> worldInfos = new List<WorldInfo>(WorldsManager.WorldInfos);
				worldInfos.Sort((WorldInfo w1, WorldInfo w2) => DateTime.Compare(w2.LastSaveTime, w1.LastSaveTime));
				Dispatcher.Dispatch(delegate
				{
					this.m_worldsListWidget.ClearItems();
					foreach (WorldInfo item in worldInfos)
					{
						this.m_worldsListWidget.AddItem(item);
					}
					if (selectedItem != null)
					{
						this.m_worldsListWidget.SelectedItem = worldInfos.FirstOrDefault((WorldInfo wi) => wi.DirectoryName == selectedItem.DirectoryName);
					}
					DialogsManager.HideDialog(dialog);
				}, false);
			});
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0003128C File Offset: 0x0002F48C
		public override void Update()
		{
			if (this.m_worldsListWidget.SelectedItem != null && WorldsManager.WorldInfos.IndexOf((WorldInfo)this.m_worldsListWidget.SelectedItem) < 0)
			{
				this.m_worldsListWidget.SelectedItem = null;
			}
			this.Children.Find<LabelWidget>("TopBar.Label", true).Text = string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 6), this.m_worldsListWidget.Items.Count);
			this.Children.Find("Play", true).IsEnabled = (this.m_worldsListWidget.SelectedItem != null);
			this.Children.Find("Properties", true).IsEnabled = (this.m_worldsListWidget.SelectedItem != null);
			if (this.Children.Find<ButtonWidget>("Play", true).IsClicked && this.m_worldsListWidget.SelectedItem != null)
			{
				this.Play(this.m_worldsListWidget.SelectedItem);
			}
			if (this.Children.Find<ButtonWidget>("NewWorld", true).IsClicked)
			{
				if (WorldsManager.WorldInfos.Count >= PlayScreen.MaxWorlds)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.GetContentWidgets(PlayScreen.fName, 7), string.Format(LanguageControl.GetContentWidgets(PlayScreen.fName, 8), PlayScreen.MaxWorlds), LanguageControl.GetContentWidgets("Usual", "ok"), null, null));
				}
				else
				{
					ScreensManager.SwitchScreen("NewWorld", Array.Empty<object>());
					this.m_worldsListWidget.SelectedItem = null;
				}
			}
			if (this.Children.Find<ButtonWidget>("Properties", true).IsClicked && this.m_worldsListWidget.SelectedItem != null)
			{
				WorldInfo worldInfo = (WorldInfo)this.m_worldsListWidget.SelectedItem;
				ScreensManager.SwitchScreen("ModifyWorld", new object[]
				{
					worldInfo.DirectoryName,
					worldInfo.WorldSettings
				});
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
				this.m_worldsListWidget.SelectedItem = null;
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x000314C0 File Offset: 0x0002F6C0
		public void Play(object item)
		{
			ModsManager.HookAction("BeforeGameLoading", delegate(ModLoader loader)
			{
				item = loader.BeforeGameLoading(this, item);
				return true;
			});
			string name = "GameLoading";
			object[] array = new object[2];
			array[0] = item;
			ScreensManager.SwitchScreen(name, array);
			this.m_worldsListWidget.SelectedItem = null;
		}

		// Token: 0x040003AD RID: 941
		public ListPanelWidget m_worldsListWidget;

		// Token: 0x040003AE RID: 942
		public static int MaxWorlds = 300;

		// Token: 0x040003AF RID: 943
		public static string fName = "PlayScreen";
	}
}
