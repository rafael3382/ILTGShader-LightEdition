using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200015F RID: 351
	public class ContentScreen : Screen
	{
		// Token: 0x060007C1 RID: 1985 RVA: 0x0002D4D4 File Offset: 0x0002B6D4
		public ContentScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ContentScreen", null);
			base.LoadContents(this, node);
			this.m_externalContentButton = this.Children.Find<ButtonWidget>("External", true);
			this.m_communityContentButton = this.Children.Find<ButtonWidget>("Community", true);
			this.m_linkButton = this.Children.Find<ButtonWidget>("Link", true);
			this.m_manageButton = this.Children.Find<ButtonWidget>("Manage", true);
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0002D558 File Offset: 0x0002B758
		public override void Update()
		{
			this.m_communityContentButton.IsEnabled = (SettingsManager.CommunityContentMode > CommunityContentMode.Disabled);
			if (this.m_externalContentButton.IsClicked)
			{
				ScreensManager.SwitchScreen("ExternalContent", Array.Empty<object>());
			}
			if (this.m_communityContentButton.IsClicked)
			{
				ScreensManager.SwitchScreen("CommunityContent", Array.Empty<object>());
			}
			if (this.m_linkButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new DownloadContentFromLinkDialog());
			}
			if (this.m_manageButton.IsClicked)
			{
				ContainerWidget parentWidget = null;
				string title = null;
				List<string> list = new List<string>();
				list.Add(LanguageControl.Get(ContentScreen.fName, 1));
				list.Add(LanguageControl.Get(ContentScreen.fName, 2));
				DialogsManager.ShowDialog(parentWidget, new ListSelectionDialog(title, list, 70f, (object item) => (string)item, delegate(object item)
				{
					if ((string)item == LanguageControl.Get(ContentScreen.fName, 1))
					{
						ScreensManager.SwitchScreen("ModsManageContent", Array.Empty<object>());
						return;
					}
					ScreensManager.SwitchScreen("ManageContent", Array.Empty<object>());
				}));
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x0400035A RID: 858
		public static string fName = "ContentScreen";

		// Token: 0x0400035B RID: 859
		public ButtonWidget m_externalContentButton;

		// Token: 0x0400035C RID: 860
		public ButtonWidget m_communityContentButton;

		// Token: 0x0400035D RID: 861
		public ButtonWidget m_linkButton;

		// Token: 0x0400035E RID: 862
		public ButtonWidget m_manageButton;
	}
}
