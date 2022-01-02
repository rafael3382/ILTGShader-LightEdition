using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000258 RID: 600
	public class CancellableBusyDialog : Dialog
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060013B2 RID: 5042 RVA: 0x00093D4C File Offset: 0x00091F4C
		// (set) Token: 0x060013B3 RID: 5043 RVA: 0x00093D54 File Offset: 0x00091F54
		public CancellableProgress Progress { get; set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060013B4 RID: 5044 RVA: 0x00093D5D File Offset: 0x00091F5D
		// (set) Token: 0x060013B5 RID: 5045 RVA: 0x00093D6A File Offset: 0x00091F6A
		public string LargeMessage
		{
			get
			{
				return this.m_largeLabelWidget.Text;
			}
			set
			{
				this.m_largeLabelWidget.Text = (value ?? string.Empty);
				this.m_largeLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060013B6 RID: 5046 RVA: 0x00093D95 File Offset: 0x00091F95
		// (set) Token: 0x060013B7 RID: 5047 RVA: 0x00093DA2 File Offset: 0x00091FA2
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = (value ?? string.Empty);
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060013B8 RID: 5048 RVA: 0x00093DB9 File Offset: 0x00091FB9
		// (set) Token: 0x060013B9 RID: 5049 RVA: 0x00093DC6 File Offset: 0x00091FC6
		public bool IsCancelButtonEnabled
		{
			get
			{
				return this.m_cancelButtonWidget.IsEnabled;
			}
			set
			{
				this.m_cancelButtonWidget.IsEnabled = value;
			}
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x00093DD4 File Offset: 0x00091FD4
		public CancellableBusyDialog(string largeMessage, bool autoHideOnCancel)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/CancellableBusyDialog", null);
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("CancellableBusyDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("CancellableBusyDialog.SmallLabel", true);
			this.m_cancelButtonWidget = this.Children.Find<ButtonWidget>("CancellableBusyDialog.CancelButton", true);
			this.Progress = new CancellableProgress();
			this.m_autoHideOnCancel = autoHideOnCancel;
			this.LargeMessage = largeMessage;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x00093E5C File Offset: 0x0009205C
		public override void Update()
		{
			this.SmallMessage = ((this.Progress.Completed > 0f && this.Progress.Total > 0f) ? string.Format("{0:0}%", this.Progress.Completed / this.Progress.Total * 100f) : string.Empty);
			if (this.m_cancelButtonWidget.IsClicked)
			{
				this.Progress.Cancel();
				if (this.m_autoHideOnCancel)
				{
					DialogsManager.HideDialog(this);
				}
			}
			if (base.Input.Cancel)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x04000C4D RID: 3149
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000C4E RID: 3150
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000C4F RID: 3151
		public ButtonWidget m_cancelButtonWidget;

		// Token: 0x04000C50 RID: 3152
		public bool m_autoHideOnCancel;
	}
}
