using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SimpleJson;

namespace Game
{
	// Token: 0x02000163 RID: 355
	public class HelpScreen : Screen
	{
		// Token: 0x060007DC RID: 2012 RVA: 0x0002E218 File Offset: 0x0002C418
		public HelpScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/HelpScreen", null);
			base.LoadContents(this, node);
			this.m_topicsList = this.Children.Find<ListPanelWidget>("TopicsList", true);
			this.m_recipaediaButton = this.Children.Find<ButtonWidget>("RecipaediaButton", true);
			this.m_bestiaryButton = this.Children.Find<ButtonWidget>("BestiaryButton", true);
			this.m_topicsList.ItemWidgetFactory = delegate(object item)
			{
				HelpTopic helpTopic2 = (HelpTopic)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/HelpTopicItem", null);
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				containerWidget.Children.Find<LabelWidget>("HelpTopicItem.Title", true).Text = helpTopic2.Title;
				return containerWidget;
			};
			ListPanelWidget topicsList = this.m_topicsList;
			topicsList.ItemClicked = (Action<object>)Delegate.Combine(topicsList.ItemClicked, new Action<object>(delegate(object item)
			{
				HelpTopic helpTopic2 = item as HelpTopic;
				if (helpTopic2 != null)
				{
					this.ShowTopic(helpTopic2);
				}
			}));
			foreach (KeyValuePair<string, object> keyValuePair in (LanguageControl.KeyWords["Help"] as JsonObject))
			{
				JsonObject jsonObject = keyValuePair.Value as JsonObject;
				if (jsonObject.ContainsKey("DisabledPlatforms"))
				{
					object obj;
					jsonObject.TryGetValue("DisabledPlatforms", out obj);
					if (((string)obj).Split(new string[]
					{
						","
					}, StringSplitOptions.None).FirstOrDefault((string s) => s.Trim().ToLower() == VersionsManager.Platform.ToString().ToLower()) == null)
					{
						continue;
					}
				}
				object obj2;
				jsonObject.TryGetValue("Title", out obj2);
				object obj3;
				jsonObject.TryGetValue("Name", out obj3);
				object obj4;
				jsonObject.TryGetValue("value", out obj4);
				string name = obj3 as string;
				string title = obj2 as string;
				string text = string.Empty;
				foreach (string text2 in ((string)obj4).Split(new string[]
				{
					"\n"
				}, StringSplitOptions.None))
				{
					text = text + text2.Trim() + " ";
				}
				text = text.Replace("\r", "");
				text = text.Replace("’", "'");
				text = text.Replace("\\n", "\n");
				HelpTopic helpTopic = new HelpTopic
				{
					Name = name,
					Title = title,
					Text = text
				};
				if (!string.IsNullOrEmpty(helpTopic.Name))
				{
					this.m_topics.Add(helpTopic.Name, helpTopic);
				}
				this.m_topicsList.AddItem(helpTopic);
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0002E4A8 File Offset: 0x0002C6A8
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("HelpTopic") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("Recipaedia") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("Bestiary"))
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0002E4E8 File Offset: 0x0002C6E8
		public override void Leave()
		{
			this.m_topicsList.SelectedItem = null;
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0002E4F8 File Offset: 0x0002C6F8
		public override void Update()
		{
			if (this.m_recipaediaButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Recipaedia", Array.Empty<object>());
			}
			if (this.m_bestiaryButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Bestiary", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0002E57F File Offset: 0x0002C77F
		public HelpTopic GetTopic(string name)
		{
			return this.m_topics[name];
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0002E590 File Offset: 0x0002C790
		public void ShowTopic(HelpTopic helpTopic)
		{
			if (helpTopic.Name == "Keyboard")
			{
				DialogsManager.ShowDialog(null, new KeyboardHelpDialog());
				return;
			}
			if (helpTopic.Name == "Gamepad")
			{
				DialogsManager.ShowDialog(null, new GamepadHelpDialog());
				return;
			}
			ScreensManager.SwitchScreen("HelpTopic", new object[]
			{
				helpTopic
			});
		}

		// Token: 0x04000370 RID: 880
		public ListPanelWidget m_topicsList;

		// Token: 0x04000371 RID: 881
		public ButtonWidget m_recipaediaButton;

		// Token: 0x04000372 RID: 882
		public ButtonWidget m_bestiaryButton;

		// Token: 0x04000373 RID: 883
		public Screen m_previousScreen;

		// Token: 0x04000374 RID: 884
		public Dictionary<string, HelpTopic> m_topics = new Dictionary<string, HelpTopic>();
	}
}
