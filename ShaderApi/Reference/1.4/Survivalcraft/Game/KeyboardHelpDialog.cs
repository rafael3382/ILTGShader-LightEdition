using System;
using System.Xml.Linq;
using Engine.Input;

namespace Game
{
	// Token: 0x020002B7 RID: 695
	public class KeyboardHelpDialog : Dialog
	{
		// Token: 0x06001570 RID: 5488 RVA: 0x000A15C8 File Offset: 0x0009F7C8
		public KeyboardHelpDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/KeyboardHelpDialog", null);
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
			this.m_helpButton = this.Children.Find<ButtonWidget>("HelpButton", true);
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000A1620 File Offset: 0x0009F820
		public override void Update()
		{
			this.m_helpButton.IsVisible = !(ScreensManager.CurrentScreen is HelpScreen);
			if (this.m_okButton.IsClicked || base.Input.Cancel || base.Input.IsKeyDownOnce(Key.H))
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_helpButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
		}

		// Token: 0x04000DEF RID: 3567
		public ButtonWidget m_okButton;

		// Token: 0x04000DF0 RID: 3568
		public ButtonWidget m_helpButton;
	}
}
