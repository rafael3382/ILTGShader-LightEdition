using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002C4 RID: 708
	public class MessageDialog : Dialog
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x000A2E78 File Offset: 0x000A1078
		// (set) Token: 0x0600159A RID: 5530 RVA: 0x000A2E80 File Offset: 0x000A1080
		public bool AutoHide { get; set; }

		// Token: 0x0600159B RID: 5531 RVA: 0x000A2E8C File Offset: 0x000A108C
		public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Vector2 size, Action<MessageDialogButton> handler)
		{
			this.m_handler = handler;
			XElement node = ContentManager.Get<XElement>("Dialogs/MessageDialog", null);
			base.LoadContents(this, node);
			base.Size = new Vector2((size.X >= 0f) ? size.X : base.Size.X, (size.Y >= 0f) ? size.Y : base.Size.Y);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("MessageDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("MessageDialog.SmallLabel", true);
			this.m_button1Widget = this.Children.Find<ButtonWidget>("MessageDialog.Button1", true);
			this.m_button2Widget = this.Children.Find<ButtonWidget>("MessageDialog.Button2", true);
			this.m_largeLabelWidget.IsVisible = !string.IsNullOrEmpty(largeMessage);
			this.m_largeLabelWidget.Text = (largeMessage ?? string.Empty);
			this.m_smallLabelWidget.IsVisible = !string.IsNullOrEmpty(smallMessage);
			this.m_smallLabelWidget.Text = (smallMessage ?? string.Empty);
			this.m_button1Widget.IsVisible = !string.IsNullOrEmpty(button1Text);
			this.m_button1Widget.Text = (button1Text ?? string.Empty);
			this.m_button2Widget.IsVisible = !string.IsNullOrEmpty(button2Text);
			this.m_button2Widget.Text = (button2Text ?? string.Empty);
			if (!this.m_button1Widget.IsVisible && !this.m_button2Widget.IsVisible)
			{
				throw new InvalidOperationException("MessageDialog must have at least one button.");
			}
			this.AutoHide = true;
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x000A3038 File Offset: 0x000A1238
		public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Action<MessageDialogButton> handler) : this(largeMessage, smallMessage, button1Text, button2Text, new Vector2(-1f), handler)
		{
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x000A3054 File Offset: 0x000A1254
		public override void Update()
		{
			if (base.Input.Cancel)
			{
				if (this.m_button2Widget.IsVisible)
				{
					this.Dismiss(MessageDialogButton.Button2);
					return;
				}
				this.Dismiss(MessageDialogButton.Button1);
				return;
			}
			else
			{
				if (base.Input.Ok || this.m_button1Widget.IsClicked)
				{
					this.Dismiss(MessageDialogButton.Button1);
					return;
				}
				if (this.m_button2Widget.IsClicked)
				{
					this.Dismiss(MessageDialogButton.Button2);
				}
				return;
			}
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000A30C1 File Offset: 0x000A12C1
		public void Dismiss(MessageDialogButton button)
		{
			if (this.AutoHide)
			{
				DialogsManager.HideDialog(this);
			}
			Action<MessageDialogButton> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(button);
		}

		// Token: 0x04000E17 RID: 3607
		public Action<MessageDialogButton> m_handler;

		// Token: 0x04000E18 RID: 3608
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000E19 RID: 3609
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000E1A RID: 3610
		public ButtonWidget m_button1Widget;

		// Token: 0x04000E1B RID: 3611
		public ButtonWidget m_button2Widget;
	}
}
