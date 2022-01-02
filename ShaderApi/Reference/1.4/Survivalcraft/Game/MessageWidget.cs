using System;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000393 RID: 915
	public class MessageWidget : CanvasWidget
	{
		// Token: 0x06001B77 RID: 7031 RVA: 0x000D6A20 File Offset: 0x000D4C20
		public MessageWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/MessageWidget", null);
			base.LoadContents(this, node);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Label", true);
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x000D6A60 File Offset: 0x000D4C60
		public void DisplayMessage(string text, Color color, bool blinking)
		{
			this.m_message = text;
			this.m_messageStartTime = Time.RealTime;
			float duration;
			if (!blinking)
			{
				duration = 4f + MathUtils.Min(1f * (float)this.m_message.Count((char c) => c == '\n'), 4f);
			}
			else
			{
				duration = 6f;
			}
			this.m_duration = duration;
			this.m_color = color;
			this.m_blinking = blinking;
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x000D6AE0 File Offset: 0x000D4CE0
		public override void Update()
		{
			double realTime = Time.RealTime;
			if (!string.IsNullOrEmpty(this.m_message))
			{
				float num;
				if (this.m_blinking)
				{
					num = MathUtils.Saturate(1f * (float)(this.m_messageStartTime + (double)this.m_duration - realTime));
					if (realTime - this.m_messageStartTime < 0.417)
					{
						num *= MathUtils.Lerp(0.25f, 1f, 0.5f * (1f - MathUtils.Cos(37.6991119f * (float)(realTime - this.m_messageStartTime))));
					}
				}
				else
				{
					num = MathUtils.Saturate(MathUtils.Min(3f * (float)(realTime - this.m_messageStartTime), 1f * (float)(this.m_messageStartTime + (double)this.m_duration - realTime)));
				}
				this.m_labelWidget.Color = this.m_color * num;
				this.m_labelWidget.IsVisible = true;
				this.m_labelWidget.Text = this.m_message;
				if (realTime - this.m_messageStartTime > (double)this.m_duration)
				{
					this.m_message = null;
					return;
				}
			}
			else
			{
				this.m_labelWidget.IsVisible = false;
			}
		}

		// Token: 0x040012A1 RID: 4769
		public LabelWidget m_labelWidget;

		// Token: 0x040012A2 RID: 4770
		public string m_message;

		// Token: 0x040012A3 RID: 4771
		public double m_messageStartTime;

		// Token: 0x040012A4 RID: 4772
		public float m_duration;

		// Token: 0x040012A5 RID: 4773
		public Color m_color;

		// Token: 0x040012A6 RID: 4774
		public bool m_blinking;
	}
}
