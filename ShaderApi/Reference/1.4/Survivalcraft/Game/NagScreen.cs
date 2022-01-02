using System;
using System.Xml.Linq;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x02000168 RID: 360
	public class NagScreen : Screen
	{
		// Token: 0x06000821 RID: 2081 RVA: 0x0002FC20 File Offset: 0x0002DE20
		public NagScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/NagScreen", null);
			base.LoadContents(this, node);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0002FC48 File Offset: 0x0002DE48
		public override void Enter(object[] parameters)
		{
			Keyboard.BackButtonQuitsApp = true;
			this.Children.Find<Widget>("Quit", true).IsVisible = true;
			this.Children.Find<Widget>("QuitLabel_Wp81", true).IsVisible = false;
			this.Children.Find<Widget>("QuitLabel_Win81", true).IsVisible = false;
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0002FCA0 File Offset: 0x0002DEA0
		public override void Leave()
		{
			Keyboard.BackButtonQuitsApp = false;
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x0002FCA8 File Offset: 0x0002DEA8
		public override void Update()
		{
			if (this.Children.Find<ButtonWidget>("Buy", true).IsClicked)
			{
				MarketplaceManager.ShowMarketplace();
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Quit", true).IsClicked || base.Input.Back)
			{
				Window.Close();
			}
		}
	}
}
