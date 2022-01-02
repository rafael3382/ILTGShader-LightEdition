using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200017E RID: 382
	public class TrialEndedScreen : Screen
	{
		// Token: 0x060008A5 RID: 2213 RVA: 0x000359C0 File Offset: 0x00033BC0
		public TrialEndedScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/TrialEndedScreen", null);
			base.LoadContents(this, node);
			this.m_buyButton = this.Children.Find<ButtonWidget>("Buy", false);
			this.m_quitButton = this.Children.Find<ButtonWidget>("Quit", false);
			this.m_newWorldButton = this.Children.Find<ButtonWidget>("NewWorld", false);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00035A2C File Offset: 0x00033C2C
		public override void Update()
		{
			if (this.m_buyButton != null && this.m_buyButton.IsClicked)
			{
				MarketplaceManager.ShowMarketplace();
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if ((this.m_quitButton != null && this.m_quitButton.IsClicked) || base.Input.Back)
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if (this.m_newWorldButton != null && this.m_newWorldButton.IsClicked)
			{
				ScreensManager.SwitchScreen("NewWorld", Array.Empty<object>());
			}
		}

		// Token: 0x04000438 RID: 1080
		public ButtonWidget m_buyButton;

		// Token: 0x04000439 RID: 1081
		public ButtonWidget m_quitButton;

		// Token: 0x0400043A RID: 1082
		public ButtonWidget m_newWorldButton;
	}
}
