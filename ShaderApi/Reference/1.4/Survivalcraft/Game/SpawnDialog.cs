using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200030C RID: 780
	public class SpawnDialog : Dialog
	{
		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x000ACF73 File Offset: 0x000AB173
		// (set) Token: 0x060016EE RID: 5870 RVA: 0x000ACF80 File Offset: 0x000AB180
		public string LargeMessage
		{
			get
			{
				return this.m_largeLabelWidget.Text;
			}
			set
			{
				this.m_largeLabelWidget.Text = value;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060016EF RID: 5871 RVA: 0x000ACF8E File Offset: 0x000AB18E
		// (set) Token: 0x060016F0 RID: 5872 RVA: 0x000ACF9B File Offset: 0x000AB19B
		public string SmallMessage
		{
			get
			{
				return this.m_smallLabelWidget.Text;
			}
			set
			{
				this.m_smallLabelWidget.Text = value;
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x000ACFA9 File Offset: 0x000AB1A9
		// (set) Token: 0x060016F2 RID: 5874 RVA: 0x000ACFB6 File Offset: 0x000AB1B6
		public float Progress
		{
			get
			{
				return this.m_progressWidget.Value;
			}
			set
			{
				this.m_progressWidget.Value = value;
			}
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x000ACFC4 File Offset: 0x000AB1C4
		public SpawnDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/SpawnDialog", null);
			base.LoadContents(this, node);
			this.m_largeLabelWidget = this.Children.Find<LabelWidget>("SpawnDialog.LargeLabel", true);
			this.m_smallLabelWidget = this.Children.Find<LabelWidget>("SpawnDialog.SmallLabel", true);
			this.m_progressWidget = this.Children.Find<ValueBarWidget>("SpawnDialog.Progress", true);
		}

		// Token: 0x04000FA7 RID: 4007
		public LabelWidget m_largeLabelWidget;

		// Token: 0x04000FA8 RID: 4008
		public LabelWidget m_smallLabelWidget;

		// Token: 0x04000FA9 RID: 4009
		public ValueBarWidget m_progressWidget;
	}
}
