using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200036E RID: 878
	public class BitmapButtonWidget : ButtonWidget
	{
		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x060019EE RID: 6638 RVA: 0x000CC17E File Offset: 0x000CA37E
		public override bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x060019EF RID: 6639 RVA: 0x000CC18B File Offset: 0x000CA38B
		// (set) Token: 0x060019F0 RID: 6640 RVA: 0x000CC198 File Offset: 0x000CA398
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

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x060019F1 RID: 6641 RVA: 0x000CC1A6 File Offset: 0x000CA3A6
		// (set) Token: 0x060019F2 RID: 6642 RVA: 0x000CC1B3 File Offset: 0x000CA3B3
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

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x060019F3 RID: 6643 RVA: 0x000CC1C1 File Offset: 0x000CA3C1
		// (set) Token: 0x060019F4 RID: 6644 RVA: 0x000CC1CE File Offset: 0x000CA3CE
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

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x060019F5 RID: 6645 RVA: 0x000CC1DC File Offset: 0x000CA3DC
		// (set) Token: 0x060019F6 RID: 6646 RVA: 0x000CC1E9 File Offset: 0x000CA3E9
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

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x060019F7 RID: 6647 RVA: 0x000CC1F7 File Offset: 0x000CA3F7
		// (set) Token: 0x060019F8 RID: 6648 RVA: 0x000CC1FF File Offset: 0x000CA3FF
		public Subtexture NormalSubtexture { get; set; }

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x060019F9 RID: 6649 RVA: 0x000CC208 File Offset: 0x000CA408
		// (set) Token: 0x060019FA RID: 6650 RVA: 0x000CC210 File Offset: 0x000CA410
		public Subtexture ClickedSubtexture { get; set; }

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x060019FB RID: 6651 RVA: 0x000CC219 File Offset: 0x000CA419
		// (set) Token: 0x060019FC RID: 6652 RVA: 0x000CC221 File Offset: 0x000CA421
		public override Color Color { get; set; }

		// Token: 0x060019FD RID: 6653 RVA: 0x000CC22C File Offset: 0x000CA42C
		public BitmapButtonWidget()
		{
			this.Color = Color.White;
			XElement node = ContentManager.Get<XElement>("Widgets/BitmapButtonContents", null);
			base.LoadChildren(this, node);
			this.m_rectangleWidget = this.Children.Find<RectangleWidget>("Button.Rectangle", true);
			this.m_imageWidget = this.Children.Find<RectangleWidget>("Button.Image", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Button.Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Button.Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x000CC2C4 File Offset: 0x000CA4C4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			bool isEnabledGlobal = base.IsEnabledGlobal;
			this.m_labelWidget.Color = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_imageWidget.FillColor = (isEnabledGlobal ? this.Color : new Color(112, 112, 112));
			this.m_rectangleWidget.Subtexture = ((this.m_clickableWidget.IsPressed || this.IsChecked) ? this.ClickedSubtexture : this.NormalSubtexture);
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040011B5 RID: 4533
		public RectangleWidget m_rectangleWidget;

		// Token: 0x040011B6 RID: 4534
		public RectangleWidget m_imageWidget;

		// Token: 0x040011B7 RID: 4535
		public LabelWidget m_labelWidget;

		// Token: 0x040011B8 RID: 4536
		public ClickableWidget m_clickableWidget;
	}
}
