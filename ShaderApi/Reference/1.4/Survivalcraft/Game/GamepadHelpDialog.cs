using System;
using System.Xml.Linq;
using Engine.Input;

namespace Game
{
	// Token: 0x020002A1 RID: 673
	public class GamepadHelpDialog : Dialog
	{
		// Token: 0x0600151A RID: 5402 RVA: 0x000A0C1C File Offset: 0x0009EE1C
		public GamepadHelpDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/GamepadHelpDialog", null);
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
			this.m_helpButton = this.Children.Find<ButtonWidget>("HelpButton", true);
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x000A0C74 File Offset: 0x0009EE74
		public override void Update()
		{
			this.m_helpButton.IsVisible = !(ScreensManager.CurrentScreen is HelpScreen);
			if (this.m_okButton.IsClicked || base.Input.Cancel || base.Input.IsPadButtonDownOnce(GamePadButton.Start))
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_helpButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
		}

		// Token: 0x04000DC6 RID: 3526
		public ButtonWidget m_okButton;

		// Token: 0x04000DC7 RID: 3527
		public ButtonWidget m_helpButton;
	}
}
