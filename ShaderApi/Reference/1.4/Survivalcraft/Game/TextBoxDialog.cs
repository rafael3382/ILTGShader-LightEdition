using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200032A RID: 810
	public class TextBoxDialog : Dialog
	{
		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600183A RID: 6202 RVA: 0x000BFA04 File Offset: 0x000BDC04
		// (set) Token: 0x0600183B RID: 6203 RVA: 0x000BFA0C File Offset: 0x000BDC0C
		public bool AutoHide { get; set; }

		// Token: 0x0600183C RID: 6204 RVA: 0x000BFA18 File Offset: 0x000BDC18
		public TextBoxDialog(string title, string text, int maximumLength, Action<string> handler)
		{
			this.m_handler = handler;
			XElement node = ContentManager.Get<XElement>("Dialogs/TextBoxDialog", null);
			base.LoadContents(this, node);
			this.m_titleWidget = this.Children.Find<LabelWidget>("TextBoxDialog.Title", true);
			this.m_textBoxWidget = this.Children.Find<TextBoxWidget>("TextBoxDialog.TextBox", true);
			this.m_okButtonWidget = this.Children.Find<ButtonWidget>("TextBoxDialog.OkButton", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("TextBoxDialog.CancelButton", true);
			this.m_titleWidget.IsVisible = !string.IsNullOrEmpty(title);
			this.m_titleWidget.Text = (title ?? string.Empty);
			this.m_textBoxWidget.MaximumLength = maximumLength;
			this.m_textBoxWidget.Text = (text ?? string.Empty);
			this.m_textBoxWidget.HasFocus = true;
			this.m_textBoxWidget.Enter += delegate(TextBoxWidget <p0>)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
			};
			this.AutoHide = true;
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x000BFB18 File Offset: 0x000BDD18
		public override void Update()
		{
			if (base.Input.Cancel)
			{
				this.Dismiss(null);
				return;
			}
			if (base.Input.Ok)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
				return;
			}
			if (this.m_okButtonWidget.IsClicked)
			{
				this.Dismiss(this.m_textBoxWidget.Text);
				return;
			}
			if (this.m_cancelButtonWidget.IsClicked)
			{
				this.Dismiss(null);
			}
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x000BFB8C File Offset: 0x000BDD8C
		public void Dismiss(string result)
		{
			if (this.AutoHide)
			{
				DialogsManager.HideDialog(this);
			}
			Action<string> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(result);
		}

		// Token: 0x040010E4 RID: 4324
		public Action<string> m_handler;

		// Token: 0x040010E5 RID: 4325
		public LabelWidget m_titleWidget;

		// Token: 0x040010E6 RID: 4326
		public TextBoxWidget m_textBoxWidget;

		// Token: 0x040010E7 RID: 4327
		public ButtonWidget m_okButtonWidget;

		// Token: 0x040010E8 RID: 4328
		public ButtonWidget m_cancelButtonWidget;
	}
}
