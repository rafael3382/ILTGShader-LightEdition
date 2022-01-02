using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000390 RID: 912
	public class LinkWidget : FixedSizePanelWidget
	{
		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001B3F RID: 6975 RVA: 0x000D5AE8 File Offset: 0x000D3CE8
		// (set) Token: 0x06001B40 RID: 6976 RVA: 0x000D5AF5 File Offset: 0x000D3CF5
		public Vector2 Size
		{
			get
			{
				return this.m_labelWidget.Size;
			}
			set
			{
				this.m_labelWidget.Size = value;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001B41 RID: 6977 RVA: 0x000D5B03 File Offset: 0x000D3D03
		public bool IsClicked
		{
			get
			{
				return this.m_clickableWidget.IsClicked;
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001B42 RID: 6978 RVA: 0x000D5B10 File Offset: 0x000D3D10
		public bool IsPressed
		{
			get
			{
				return this.m_clickableWidget.IsPressed;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001B43 RID: 6979 RVA: 0x000D5B1D File Offset: 0x000D3D1D
		// (set) Token: 0x06001B44 RID: 6980 RVA: 0x000D5B2A File Offset: 0x000D3D2A
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

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001B45 RID: 6981 RVA: 0x000D5B38 File Offset: 0x000D3D38
		// (set) Token: 0x06001B46 RID: 6982 RVA: 0x000D5B45 File Offset: 0x000D3D45
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

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001B47 RID: 6983 RVA: 0x000D5B53 File Offset: 0x000D3D53
		// (set) Token: 0x06001B48 RID: 6984 RVA: 0x000D5B60 File Offset: 0x000D3D60
		public TextAnchor TextAnchor
		{
			get
			{
				return this.m_labelWidget.TextAnchor;
			}
			set
			{
				this.m_labelWidget.TextAnchor = value;
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001B49 RID: 6985 RVA: 0x000D5B6E File Offset: 0x000D3D6E
		// (set) Token: 0x06001B4A RID: 6986 RVA: 0x000D5B7B File Offset: 0x000D3D7B
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

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x000D5B89 File Offset: 0x000D3D89
		// (set) Token: 0x06001B4C RID: 6988 RVA: 0x000D5B96 File Offset: 0x000D3D96
		public Color Color
		{
			get
			{
				return this.m_labelWidget.Color;
			}
			set
			{
				this.m_labelWidget.Color = value;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x000D5BA4 File Offset: 0x000D3DA4
		// (set) Token: 0x06001B4E RID: 6990 RVA: 0x000D5BB1 File Offset: 0x000D3DB1
		public bool DropShadow
		{
			get
			{
				return this.m_labelWidget.DropShadow;
			}
			set
			{
				this.m_labelWidget.DropShadow = value;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x000D5BBF File Offset: 0x000D3DBF
		// (set) Token: 0x06001B50 RID: 6992 RVA: 0x000D5BC7 File Offset: 0x000D3DC7
		public string Url { get; set; }

		// Token: 0x06001B51 RID: 6993 RVA: 0x000D5BD0 File Offset: 0x000D3DD0
		public LinkWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/LinkContents", null);
			base.LoadChildren(this, node);
			this.m_labelWidget = this.Children.Find<LabelWidget>("Label", true);
			this.m_clickableWidget = this.Children.Find<ClickableWidget>("Clickable", true);
			base.LoadProperties(this, node);
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x000D5C2D File Offset: 0x000D3E2D
		public override void Update()
		{
			if (!string.IsNullOrEmpty(this.Url) && this.IsClicked)
			{
				WebBrowserManager.LaunchBrowser(this.Url);
			}
		}

		// Token: 0x04001286 RID: 4742
		public LabelWidget m_labelWidget;

		// Token: 0x04001287 RID: 4743
		public ClickableWidget m_clickableWidget;
	}
}
