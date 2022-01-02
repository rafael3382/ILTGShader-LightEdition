using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200017C RID: 380
	public class SettingsScreen : Screen
	{
		// Token: 0x0600089E RID: 2206 RVA: 0x0003534C File Offset: 0x0003354C
		public SettingsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsScreen", null);
			base.LoadContents(this, node);
			this.m_performanceButton = this.Children.Find<ButtonWidget>("Performance", true);
			this.m_graphicsButton = this.Children.Find<ButtonWidget>("Graphics", true);
			this.m_uiButton = this.Children.Find<ButtonWidget>("Ui", true);
			this.m_compatibilityButton = this.Children.Find<ButtonWidget>("Compatibility", true);
			this.m_audioButton = this.Children.Find<ButtonWidget>("Audio", true);
			this.m_controlsButton = this.Children.Find<ButtonWidget>("Controls", true);
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000353FD File Offset: 0x000335FD
		public override void Enter(object[] parameters)
		{
			if (this.m_previousScreen == null)
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00035414 File Offset: 0x00033614
		public override void Update()
		{
			if (this.m_performanceButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsPerformance", Array.Empty<object>());
			}
			if (this.m_graphicsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsGraphics", Array.Empty<object>());
			}
			if (this.m_uiButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsUi", Array.Empty<object>());
			}
			if (this.m_compatibilityButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsCompatibility", Array.Empty<object>());
			}
			if (this.m_audioButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsAudio", Array.Empty<object>());
			}
			if (this.m_controlsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("SettingsControls", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
				this.m_previousScreen = null;
			}
		}

		// Token: 0x04000425 RID: 1061
		public Screen m_previousScreen;

		// Token: 0x04000426 RID: 1062
		public ButtonWidget m_performanceButton;

		// Token: 0x04000427 RID: 1063
		public ButtonWidget m_graphicsButton;

		// Token: 0x04000428 RID: 1064
		public ButtonWidget m_uiButton;

		// Token: 0x04000429 RID: 1065
		public ButtonWidget m_compatibilityButton;

		// Token: 0x0400042A RID: 1066
		public ButtonWidget m_audioButton;

		// Token: 0x0400042B RID: 1067
		public ButtonWidget m_controlsButton;
	}
}
