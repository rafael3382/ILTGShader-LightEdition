using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200039E RID: 926
	public class SliderWidget : CanvasWidget
	{
		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x000DA1B7 File Offset: 0x000D83B7
		// (set) Token: 0x06001BF7 RID: 7159 RVA: 0x000DA1BF File Offset: 0x000D83BF
		public bool IsSliding { get; set; }

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x000DA1C8 File Offset: 0x000D83C8
		// (set) Token: 0x06001BF9 RID: 7161 RVA: 0x000DA1D0 File Offset: 0x000D83D0
		public LayoutDirection LayoutDirection { get; set; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001BFA RID: 7162 RVA: 0x000DA1D9 File Offset: 0x000D83D9
		// (set) Token: 0x06001BFB RID: 7163 RVA: 0x000DA1E4 File Offset: 0x000D83E4
		public float MinValue
		{
			get
			{
				return this.m_minValue;
			}
			set
			{
				if (value != this.m_minValue)
				{
					this.m_minValue = value;
					this.MaxValue = MathUtils.Max(this.MinValue, this.MaxValue);
					this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
				}
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001BFC RID: 7164 RVA: 0x000DA235 File Offset: 0x000D8435
		// (set) Token: 0x06001BFD RID: 7165 RVA: 0x000DA240 File Offset: 0x000D8440
		public float MaxValue
		{
			get
			{
				return this.m_maxValue;
			}
			set
			{
				if (value != this.m_maxValue)
				{
					this.m_maxValue = value;
					this.MinValue = MathUtils.Min(this.MinValue, this.MaxValue);
					this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
				}
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001BFE RID: 7166 RVA: 0x000DA291 File Offset: 0x000D8491
		// (set) Token: 0x06001BFF RID: 7167 RVA: 0x000DA29C File Offset: 0x000D849C
		public float Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = ((this.m_granularity > 0f) ? (MathUtils.Round(MathUtils.Clamp(value, this.MinValue, this.MaxValue) / this.m_granularity) * this.m_granularity) : MathUtils.Clamp(value, this.MinValue, this.MaxValue));
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06001C00 RID: 7168 RVA: 0x000DA2F5 File Offset: 0x000D84F5
		// (set) Token: 0x06001C01 RID: 7169 RVA: 0x000DA2FD File Offset: 0x000D84FD
		public float Granularity
		{
			get
			{
				return this.m_granularity;
			}
			set
			{
				this.m_granularity = MathUtils.Max(value, 0f);
			}
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001C02 RID: 7170 RVA: 0x000DA310 File Offset: 0x000D8510
		// (set) Token: 0x06001C03 RID: 7171 RVA: 0x000DA31D File Offset: 0x000D851D
		public string Text
		{
			get
			{
				return this.m_labelWidget.Text;
			}
			set
			{
				this.m_labelWidget.Text = value;
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x000DA32B File Offset: 0x000D852B
		// (set) Token: 0x06001C05 RID: 7173 RVA: 0x000DA338 File Offset: 0x000D8538
		public BitmapFont Font
		{
			get
			{
				return this.m_labelWidget.Font;
			}
			set
			{
				this.m_labelWidget.Font = value;
			}
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06001C06 RID: 7174 RVA: 0x000DA346 File Offset: 0x000D8546
		// (set) Token: 0x06001C07 RID: 7175 RVA: 0x000DA34E File Offset: 0x000D854E
		public string SoundName { get; set; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001C08 RID: 7176 RVA: 0x000DA357 File Offset: 0x000D8557
		// (set) Token: 0x06001C09 RID: 7177 RVA: 0x000DA364 File Offset: 0x000D8564
		public bool IsLabelVisible
		{
			get
			{
				return this.m_labelCanvasWidget.IsVisible;
			}
			set
			{
				this.m_labelCanvasWidget.IsVisible = value;
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001C0A RID: 7178 RVA: 0x000DA372 File Offset: 0x000D8572
		// (set) Token: 0x06001C0B RID: 7179 RVA: 0x000DA384 File Offset: 0x000D8584
		public float LabelWidth
		{
			get
			{
				return this.m_labelCanvasWidget.Size.X;
			}
			set
			{
				this.m_labelCanvasWidget.Size = new Vector2(value, this.m_labelCanvasWidget.Size.Y);
			}
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x000DA3A8 File Offset: 0x000D85A8
		public SliderWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/SliderContents", null);
			base.LoadChildren(this, node);
			this.m_canvasWidget = this.Children.Find<CanvasWidget>("Slider.Canvas", true);
			this.m_labelCanvasWidget = this.Children.Find<CanvasWidget>("Slider.LabelCanvas", true);
			this.m_tabWidget = this.Children.Find<Widget>("Slider.Tab", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Slider.Label", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x000DA449 File Offset: 0x000D8649
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.MeasureOverride(parentAvailableSize);
			base.IsDrawRequired = true;
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x000DA45C File Offset: 0x000D865C
		public override void ArrangeOverride()
		{
			base.ArrangeOverride();
			float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_canvasWidget.ActualSize.X : this.m_canvasWidget.ActualSize.Y;
			float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_tabWidget.ActualSize.X : this.m_tabWidget.ActualSize.Y;
			float num3 = (this.MaxValue > this.MinValue) ? ((this.Value - this.MinValue) / (this.MaxValue - this.MinValue)) : 0f;
			if (this.LayoutDirection == LayoutDirection.Horizontal)
			{
				Vector2 zero = Vector2.Zero;
				zero.X = num3 * (num - num2);
				zero.Y = MathUtils.Max((base.ActualSize.Y - this.m_tabWidget.ActualSize.Y) / 2f, 0f);
				this.m_canvasWidget.SetWidgetPosition(this.m_tabWidget, new Vector2?(zero));
			}
			else
			{
				Vector2 zero2 = Vector2.Zero;
				zero2.X = MathUtils.Max(base.ActualSize.X - this.m_tabWidget.ActualSize.X, 0f) / 2f;
				zero2.Y = num3 * (num - num2);
				this.m_canvasWidget.SetWidgetPosition(this.m_tabWidget, new Vector2?(zero2));
			}
			base.ArrangeOverride();
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x000DA5C0 File Offset: 0x000D87C0
		public override void Update()
		{
			float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_canvasWidget.ActualSize.X : this.m_canvasWidget.ActualSize.Y;
			float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.m_tabWidget.ActualSize.X : this.m_tabWidget.ActualSize.Y;
			if (base.Input.Tap != null && base.HitTestGlobal(base.Input.Tap.Value, null) == this.m_tabWidget)
			{
				this.m_dragStartPoint = new Vector2?(base.ScreenToWidget(base.Input.Press.Value));
			}
			if (base.Input.Press != null)
			{
				if (this.m_dragStartPoint != null)
				{
					Vector2 vector = base.ScreenToWidget(base.Input.Press.Value);
					float value = this.Value;
					if (this.LayoutDirection == LayoutDirection.Horizontal)
					{
						float f = (vector.X - num2 / 2f) / (num - num2);
						this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f);
					}
					else
					{
						float f2 = (vector.Y - num2 / 2f) / (num - num2);
						this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f2);
					}
					if (this.Value != value && this.m_granularity > 0f && !string.IsNullOrEmpty(this.SoundName))
					{
						AudioManager.PlaySound(this.SoundName, 1f, 0f, 0f);
					}
				}
			}
			else
			{
				this.m_dragStartPoint = null;
			}
			this.IsSliding = (this.m_dragStartPoint != null && base.IsEnabledGlobal && base.IsVisibleGlobal);
			if (this.m_dragStartPoint != null)
			{
				base.Input.Clear();
			}
		}

		// Token: 0x040012F1 RID: 4849
		public CanvasWidget m_canvasWidget;

		// Token: 0x040012F2 RID: 4850
		public CanvasWidget m_labelCanvasWidget;

		// Token: 0x040012F3 RID: 4851
		public Widget m_tabWidget;

		// Token: 0x040012F4 RID: 4852
		public LabelWidget m_labelWidget;

		// Token: 0x040012F5 RID: 4853
		public float m_minValue;

		// Token: 0x040012F6 RID: 4854
		public float m_maxValue = 1f;

		// Token: 0x040012F7 RID: 4855
		public float m_granularity = 0.1f;

		// Token: 0x040012F8 RID: 4856
		public float m_value;

		// Token: 0x040012F9 RID: 4857
		public Vector2? m_dragStartPoint;
	}
}
