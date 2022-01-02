using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200028A RID: 650
	public class ExternalContentLinkDialog : Dialog
	{
		// Token: 0x06001489 RID: 5257 RVA: 0x000999D4 File Offset: 0x00097BD4
		public ExternalContentLinkDialog(string link)
		{
			ClipboardManager.ClipboardString = link;
			XElement node = ContentManager.Get<XElement>("Dialogs/ExternalContentLinkDialog", null);
			base.LoadContents(this, node);
			this.m_textBoxWidget = this.Children.Find<TextBoxWidget>("ExternalContentLinkDialog.TextBox", true);
			this.m_okButtonWidget = this.Children.Find<ButtonWidget>("ExternalContentLinkDialog.OkButton", true);
			this.m_textBoxWidget.Text = link;
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x00099A3B File Offset: 0x00097C3B
		public override void Update()
		{
			if (base.Input.Cancel || this.m_okButtonWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000D4E RID: 3406
		public TextBoxWidget m_textBoxWidget;

		// Token: 0x04000D4F RID: 3407
		public ButtonWidget m_okButtonWidget;
	}
}
