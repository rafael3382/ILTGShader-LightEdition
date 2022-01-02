using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000254 RID: 596
	public class BusyDialog : Dialog
	{
		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06001389 RID: 5001 RVA: 0x00093975 File Offset: 0x00091B75
		// (set) Token: 0x0600138A RID: 5002 RVA: 0x00093982 File Offset: 0x00091B82
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

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x0600138B RID: 5003 RVA: 0x000939AD File Offset: 0x00091BAD
		// (set) Token: 0x0600138C RID: 5004 RVA: 0x000939BA File Offset: 0x00091BBA
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = (value ?? string.Empty);
				this.m_smallLabelWidget.IsVisible = !string.IsNullOrEmpty(value);
			}
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x000939E8 File Offset: 0x00091BE8
		public BusyDialog(string largeMessage, string smallMessage)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/BusyDialog", null);
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("BusyDialog.SmallLabel", true);
			this.LargeMessage = largeMessage;
			this.SmallMessage = smallMessage;
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x00093A4B File Offset: 0x00091C4B
		public override void Update()
		{
			if (base.Input.Back)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x04000C46 RID: 3142
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000C47 RID: 3143
		public LabelWidget m_smallLabelWidget;
	}
}
