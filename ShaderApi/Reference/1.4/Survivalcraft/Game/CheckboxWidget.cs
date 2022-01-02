using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000374 RID: 884
	public class CheckboxWidget : CanvasWidget
	{
		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000CCE23 File Offset: 0x000CB023
		public bool IsPressed
		{
			get
			{
				return this.m_clickableWidget.IsPressed;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001A36 RID: 6710 RVA: 0x000CCE30 File Offset: 0x000CB030
		public bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x000CCE3D File Offset: 0x000CB03D
		public bool IsTapped
		{
			get
			{
				return this.m_clickableWidget.IsTapped;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001A38 RID: 6712 RVA: 0x000CCE4A File Offset: 0x000CB04A
		// (set) Token: 0x06001A39 RID: 6713 RVA: 0x000CCE52 File Offset: 0x000CB052
		public bool IsChecked { get; set; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06001A3A RID: 6714 RVA: 0x000CCE5B File Offset: 0x000CB05B
		// (set) Token: 0x06001A3B RID: 6715 RVA: 0x000CCE63 File Offset: 0x000CB063
		public bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001A3C RID: 6716 RVA: 0x000CCE6C File Offset: 0x000CB06C
		// (set) Token: 0x06001A3D RID: 6717 RVA: 0x000CCE79 File Offset: 0x000CB079
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

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06001A3E RID: 6718 RVA: 0x000CCE87 File Offset: 0x000CB087
		// (set) Token: 0x06001A3F RID: 6719 RVA: 0x000CCE94 File Offset: 0x000CB094
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

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06001A40 RID: 6720 RVA: 0x000CCEA2 File Offset: 0x000CB0A2
		// (set) Token: 0x06001A41 RID: 6721 RVA: 0x000CCEAF File Offset: 0x000CB0AF
		public Subtexture TickSubtexture
		{
			get
			{
				return this.m_tickWidget.Subtexture;
			}
			set
			{
				this.m_tickWidget.Subtexture = value;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06001A42 RID: 6722 RVA: 0x000CCEBD File Offset: 0x000CB0BD
		// (set) Token: 0x06001A43 RID: 6723 RVA: 0x000CCEC5 File Offset: 0x000CB0C5
		public Color Color { get; set; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06001A44 RID: 6724 RVA: 0x000CCECE File Offset: 0x000CB0CE
		// (set) Token: 0x06001A45 RID: 6725 RVA: 0x000CCEDB File Offset: 0x000CB0DB
		public Vector2 CheckboxSize
		{
			get
			{
				return this.m_canvasWidget.Size;
			}
			set
			{
				this.m_canvasWidget.Size = value;
			}
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x000CCEEC File Offset: 0x000CB0EC
		public CheckboxWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CheckboxContents", null);
			base.LoadChildren(this, node);
			this.m_canvasWidget = this.Children.Find<CanvasWidget>("Checkbox.Canvas", true);
			this.m_rectangleWidget = this.Children.Find<RectangleWidget>("Checkbox.Rectangle", true);
			this.m_tickWidget = this.Children.Find<RectangleWidget>("Checkbox.Tick", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Checkbox.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Checkbox.Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x000CCF8E File Offset: 0x000CB18E
		public override void Update()
		{
			if (this.IsClicked && this.IsAutoCheckingEnabled)
			{
				this.IsChecked = !this.IsChecked;
			}
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x000CCFB0 File Offset: 0x000CB1B0
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_rectangleWidget.FillColor = new Color(0, 0, 0, 128);
			this.m_rectangleWidget.OutlineColor = (isEnabledGlobal ? new Color(128, 128, 128) : new Color(112, 112, 112));
			this.m_tickWidget.IsVisible = this.IsChecked;
			this.m_tickWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_tickWidget.OutlineColor = Color.Transparent;
			this.m_tickWidget.Subtexture = this.TickSubtexture;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040011D3 RID: 4563
		public CanvasWidget m_canvasWidget;

		// Token: 0x040011D4 RID: 4564
		public RectangleWidget m_rectangleWidget;

		// Token: 0x040011D5 RID: 4565
		public RectangleWidget m_tickWidget;

		// Token: 0x040011D6 RID: 4566
		public LabelWidget m_labelWidget;

		// Token: 0x040011D7 RID: 4567
		public ClickableWidget m_clickableWidget;
	}
}
