using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200039B RID: 923
	public class RectangleWidget : Widget
	{
		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001BC8 RID: 7112 RVA: 0x000D910C File Offset: 0x000D730C
		// (set) Token: 0x06001BC9 RID: 7113 RVA: 0x000D9114 File Offset: 0x000D7314
		public Vector2 Size { get; set; }

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06001BCA RID: 7114 RVA: 0x000D911D File Offset: 0x000D731D
		// (set) Token: 0x06001BCB RID: 7115 RVA: 0x000D9125 File Offset: 0x000D7325
		public float Depth { get; set; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06001BCC RID: 7116 RVA: 0x000D912E File Offset: 0x000D732E
		// (set) Token: 0x06001BCD RID: 7117 RVA: 0x000D9136 File Offset: 0x000D7336
		public bool DepthWriteEnabled
		{
			get
			{
				return this.m_depthWriteEnabled;
			}
			set
			{
				if (value != this.m_depthWriteEnabled)
				{
					this.m_depthWriteEnabled = value;
				}
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06001BCE RID: 7118 RVA: 0x000D9148 File Offset: 0x000D7348
		// (set) Token: 0x06001BCF RID: 7119 RVA: 0x000D9150 File Offset: 0x000D7350
		public Subtexture Subtexture
		{
			get
			{
				return this.m_subtexture;
			}
			set
			{
				if (value != this.m_subtexture)
				{
					this.m_subtexture = value;
				}
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06001BD0 RID: 7120 RVA: 0x000D9162 File Offset: 0x000D7362
		// (set) Token: 0x06001BD1 RID: 7121 RVA: 0x000D916A File Offset: 0x000D736A
		public bool TextureWrap
		{
			get
			{
				return this.m_textureWrap;
			}
			set
			{
				if (value != this.m_textureWrap)
				{
					this.m_textureWrap = value;
				}
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06001BD2 RID: 7122 RVA: 0x000D917C File Offset: 0x000D737C
		// (set) Token: 0x06001BD3 RID: 7123 RVA: 0x000D9184 File Offset: 0x000D7384
		public bool TextureLinearFilter
		{
			get
			{
				return this.m_textureLinearFilter;
			}
			set
			{
				if (value != this.m_textureLinearFilter)
				{
					this.m_textureLinearFilter = value;
				}
			}
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06001BD4 RID: 7124 RVA: 0x000D9196 File Offset: 0x000D7396
		// (set) Token: 0x06001BD5 RID: 7125 RVA: 0x000D919E File Offset: 0x000D739E
		public bool FlipHorizontal { get; set; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06001BD6 RID: 7126 RVA: 0x000D91A7 File Offset: 0x000D73A7
		// (set) Token: 0x06001BD7 RID: 7127 RVA: 0x000D91AF File Offset: 0x000D73AF
		public bool FlipVertical { get; set; }

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06001BD8 RID: 7128 RVA: 0x000D91B8 File Offset: 0x000D73B8
		// (set) Token: 0x06001BD9 RID: 7129 RVA: 0x000D91C0 File Offset: 0x000D73C0
		public Color FillColor { get; set; }

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001BDA RID: 7130 RVA: 0x000D91C9 File Offset: 0x000D73C9
		// (set) Token: 0x06001BDB RID: 7131 RVA: 0x000D91D1 File Offset: 0x000D73D1
		public Color OutlineColor { get; set; }

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001BDC RID: 7132 RVA: 0x000D91DA File Offset: 0x000D73DA
		// (set) Token: 0x06001BDD RID: 7133 RVA: 0x000D91E2 File Offset: 0x000D73E2
		public float OutlineThickness { get; set; }

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001BDE RID: 7134 RVA: 0x000D91EB File Offset: 0x000D73EB
		// (set) Token: 0x06001BDF RID: 7135 RVA: 0x000D91F3 File Offset: 0x000D73F3
		public Vector2 Texcoord1 { get; set; }

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x000D91FC File Offset: 0x000D73FC
		// (set) Token: 0x06001BE1 RID: 7137 RVA: 0x000D9204 File Offset: 0x000D7404
		public Vector2 Texcoord2 { get; set; }

		// Token: 0x06001BE2 RID: 7138 RVA: 0x000D9210 File Offset: 0x000D7410
		public RectangleWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.TextureLinearFilter = true;
			this.FillColor = Color.Black;
			this.OutlineColor = Color.White;
			this.OutlineThickness = 1f;
			this.IsHitTestVisible = false;
			this.Texcoord1 = Vector2.Zero;
			this.Texcoord2 = Vector2.One;
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x000D9278 File Offset: 0x000D7478
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.FillColor.A == 0 && (this.OutlineColor.A == 0 || this.OutlineThickness <= 0f))
			{
				return;
			}
			DepthStencilState depthStencilState = this.DepthWriteEnabled ? DepthStencilState.DepthWrite : DepthStencilState.None;
			Matrix globalTransform = base.GlobalTransform;
			Vector2 zero = Vector2.Zero;
			Vector2 vector = new Vector2(base.ActualSize.X, 0f);
			Vector2 actualSize = base.ActualSize;
			Vector2 vector2 = new Vector2(0f, base.ActualSize.Y);
			Vector2 vector3;
			Vector2.Transform(ref zero, ref globalTransform, out vector3);
			Vector2 vector4;
			Vector2.Transform(ref vector, ref globalTransform, out vector4);
			Vector2 vector5;
			Vector2.Transform(ref actualSize, ref globalTransform, out vector5);
			Vector2 vector6;
			Vector2.Transform(ref vector2, ref globalTransform, out vector6);
			Color color = this.FillColor * base.GlobalColorTransform;
			if (color.A != 0)
			{
				if (this.Subtexture != null)
				{
					SamplerState samplerState = (!this.TextureWrap) ? (this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp) : (this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap);
					TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.Subtexture.Texture, true, 0, depthStencilState, null, null, samplerState);
					Vector2 vector7 = default(Vector2);
					Vector2 vector8 = default(Vector2);
					Vector2 texCoord;
					Vector2 texCoord2;
					if (this.TextureWrap)
					{
						vector7 = Vector2.Zero;
						texCoord = new Vector2(base.ActualSize.X / (float)this.Subtexture.Texture.Width, 0f);
						vector8 = new Vector2(base.ActualSize.X / (float)this.Subtexture.Texture.Width, base.ActualSize.Y / (float)this.Subtexture.Texture.Height);
						texCoord2 = new Vector2(0f, base.ActualSize.Y / (float)this.Subtexture.Texture.Height);
					}
					else
					{
						vector7.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord1.X);
						vector7.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord1.Y);
						vector8.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord2.X);
						vector8.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord2.Y);
						texCoord = new Vector2(vector8.X, vector7.Y);
						texCoord2 = new Vector2(vector7.X, vector8.Y);
					}
					if (this.FlipHorizontal)
					{
						Utilities.Swap<float>(ref vector7.X, ref texCoord.X);
						Utilities.Swap<float>(ref vector8.X, ref texCoord2.X);
					}
					if (this.FlipVertical)
					{
						Utilities.Swap<float>(ref vector7.Y, ref vector8.Y);
						Utilities.Swap<float>(ref texCoord.Y, ref texCoord2.Y);
					}
					texturedBatch2D.QueueQuad(vector3, vector4, vector5, vector6, this.Depth, vector7, texCoord, vector8, texCoord2, color);
				}
				else
				{
					dc.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null).QueueQuad(vector3, vector4, vector5, vector6, this.Depth, color);
				}
			}
			Color color2 = this.OutlineColor * base.GlobalColorTransform;
			if (color2.A != 0 && this.OutlineThickness > 0f)
			{
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null);
				Vector2 vector9 = Vector2.Normalize(base.GlobalTransform.Right.XY);
				Vector2 v = -Vector2.Normalize(base.GlobalTransform.Up.XY);
				int num = (int)MathUtils.Max(MathUtils.Round(this.OutlineThickness * base.GlobalTransform.Right.Length()), 1f);
				for (int i = 0; i < num; i++)
				{
					flatBatch2D.QueueLine(vector3, vector4, this.Depth, color2);
					flatBatch2D.QueueLine(vector4, vector5, this.Depth, color2);
					flatBatch2D.QueueLine(vector5, vector6, this.Depth, color2);
					flatBatch2D.QueueLine(vector6, vector3, this.Depth, color2);
					vector3 += vector9 - v;
					vector4 += -vector9 - v;
					vector5 += -vector9 + v;
					vector6 += vector9 + v;
				}
			}
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x000D977C File Offset: 0x000D797C
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.FillColor.A != 0 || (this.OutlineColor.A != 0 && this.OutlineThickness > 0f));
			base.DesiredSize = this.Size;
		}

		// Token: 0x040012DB RID: 4827
		public Subtexture m_subtexture;

		// Token: 0x040012DC RID: 4828
		public bool m_textureWrap;

		// Token: 0x040012DD RID: 4829
		public bool m_textureLinearFilter;

		// Token: 0x040012DE RID: 4830
		public bool m_depthWriteEnabled;
	}
}
