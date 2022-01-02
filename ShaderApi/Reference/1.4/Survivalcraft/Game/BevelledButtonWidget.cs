using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200036C RID: 876
	public class BevelledButtonWidget : ButtonWidget
	{
		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060019BB RID: 6587 RVA: 0x000CB581 File Offset: 0x000C9781
		// (set) Token: 0x060019BC RID: 6588 RVA: 0x000CB58E File Offset: 0x000C978E
		public float FontScale
		{
			get
			{
				return this.m_labelWidget.FontScale;
			}
			set
			{
				this.m_labelWidget.FontScale = value;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060019BD RID: 6589 RVA: 0x000CB59C File Offset: 0x000C979C
		public override bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060019BE RID: 6590 RVA: 0x000CB5A9 File Offset: 0x000C97A9
		// (set) Token: 0x060019BF RID: 6591 RVA: 0x000CB5B6 File Offset: 0x000C97B6
		public override bool IsChecked
		{
			get
			{
				return this.m_clickableWidget.IsChecked;
			}
			set
			{
				this.m_clickableWidget.IsChecked = value;
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060019C0 RID: 6592 RVA: 0x000CB5C4 File Offset: 0x000C97C4
		// (set) Token: 0x060019C1 RID: 6593 RVA: 0x000CB5D1 File Offset: 0x000C97D1
		public override bool IsAutoCheckingEnabled
		{
			get
			{
				return this.m_clickableWidget.IsAutoCheckingEnabled;
			}
			set
			{
				this.m_clickableWidget.IsAutoCheckingEnabled = value;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060019C2 RID: 6594 RVA: 0x000CB5DF File Offset: 0x000C97DF
		// (set) Token: 0x060019C3 RID: 6595 RVA: 0x000CB5EC File Offset: 0x000C97EC
		public override string Text
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

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x060019C4 RID: 6596 RVA: 0x000CB5FA File Offset: 0x000C97FA
		// (set) Token: 0x060019C5 RID: 6597 RVA: 0x000CB607 File Offset: 0x000C9807
		public override BitmapFont Font
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

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x060019C6 RID: 6598 RVA: 0x000CB615 File Offset: 0x000C9815
		// (set) Token: 0x060019C7 RID: 6599 RVA: 0x000CB622 File Offset: 0x000C9822
		public Subtexture Subtexture
		{
			get
			{
				return this.m_imageWidget.Subtexture;
			}
			set
			{
				this.m_imageWidget.Subtexture = value;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x060019C8 RID: 6600 RVA: 0x000CB630 File Offset: 0x000C9830
		// (set) Token: 0x060019C9 RID: 6601 RVA: 0x000CB638 File Offset: 0x000C9838
		public override Color Color { get; set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x060019CA RID: 6602 RVA: 0x000CB641 File Offset: 0x000C9841
		// (set) Token: 0x060019CB RID: 6603 RVA: 0x000CB64E File Offset: 0x000C984E
		public Color BevelColor
		{
			get
			{
				return this.m_rectangleWidget.BevelColor;
			}
			set
			{
				this.m_rectangleWidget.BevelColor = value;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x060019CC RID: 6604 RVA: 0x000CB65C File Offset: 0x000C985C
		// (set) Token: 0x060019CD RID: 6605 RVA: 0x000CB669 File Offset: 0x000C9869
		public Color CenterColor
		{
			get
			{
				return this.m_rectangleWidget.CenterColor;
			}
			set
			{
				this.m_rectangleWidget.CenterColor = value;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x000CB677 File Offset: 0x000C9877
		// (set) Token: 0x060019CF RID: 6607 RVA: 0x000CB684 File Offset: 0x000C9884
		public float AmbientLight
		{
			get
			{
				return this.m_rectangleWidget.AmbientLight;
			}
			set
			{
				this.m_rectangleWidget.AmbientLight = value;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x000CB692 File Offset: 0x000C9892
		// (set) Token: 0x060019D1 RID: 6609 RVA: 0x000CB69F File Offset: 0x000C989F
		public float DirectionalLight
		{
			get
			{
				return this.m_rectangleWidget.DirectionalLight;
			}
			set
			{
				this.m_rectangleWidget.DirectionalLight = value;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x000CB6AD File Offset: 0x000C98AD
		// (set) Token: 0x060019D3 RID: 6611 RVA: 0x000CB6B5 File Offset: 0x000C98B5
		public float BevelSize { get; set; }

		// Token: 0x060019D4 RID: 6612 RVA: 0x000CB6C0 File Offset: 0x000C98C0
		public BevelledButtonWidget()
		{
			this.Color = Color.White;
			this.BevelSize = 2f;
			XElement node = ContentManager.Get<XElement>("Widgets/BevelledButtonContents", null);
			base.LoadChildren(this, node);
			this.m_rectangleWidget = this.Children.Find<BevelledRectangleWidget>("BevelledButton.Rectangle", true);
			this.m_imageWidget = this.Children.Find<RectangleWidget>("BevelledButton.Image", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("BevelledButton.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("BevelledButton.Clickable", true);
			this.m_labelWidget.VerticalAlignment = WidgetAlignment.Center;
			base.LoadProperties(this, node);
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x000CB770 File Offset: 0x000C9970
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_imageWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_rectangleWidget.BevelSize = ((this.m_clickableWidget.IsPressed || this.IsChecked) ? (-0.5f * this.BevelSize) : this.BevelSize);
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040011A5 RID: 4517
		public BevelledRectangleWidget m_rectangleWidget;

		// Token: 0x040011A6 RID: 4518
		public RectangleWidget m_imageWidget;

		// Token: 0x040011A7 RID: 4519
		public LabelWidget m_labelWidget;

		// Token: 0x040011A8 RID: 4520
		public ClickableWidget m_clickableWidget;
	}
}
