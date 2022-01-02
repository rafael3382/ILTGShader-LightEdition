using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020003A6 RID: 934
	public class ValueBarWidget : Widget
	{
		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06001C6E RID: 7278 RVA: 0x000DC7A0 File Offset: 0x000DA9A0
		// (set) Token: 0x06001C6F RID: 7279 RVA: 0x000DC7A8 File Offset: 0x000DA9A8
		public float Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001C70 RID: 7280 RVA: 0x000DC7B6 File Offset: 0x000DA9B6
		// (set) Token: 0x06001C71 RID: 7281 RVA: 0x000DC7BE File Offset: 0x000DA9BE
		public int BarsCount
		{
			get
			{
				return this.m_barsCount;
			}
			set
			{
				this.m_barsCount = MathUtils.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001C72 RID: 7282 RVA: 0x000DC7D2 File Offset: 0x000DA9D2
		// (set) Token: 0x06001C73 RID: 7283 RVA: 0x000DC7DA File Offset: 0x000DA9DA
		public bool FlipDirection { get; set; }

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06001C74 RID: 7284 RVA: 0x000DC7E3 File Offset: 0x000DA9E3
		// (set) Token: 0x06001C75 RID: 7285 RVA: 0x000DC7EB File Offset: 0x000DA9EB
		public Vector2 BarSize { get; set; }

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06001C76 RID: 7286 RVA: 0x000DC7F4 File Offset: 0x000DA9F4
		// (set) Token: 0x06001C77 RID: 7287 RVA: 0x000DC7FC File Offset: 0x000DA9FC
		public float Spacing { get; set; }

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000DC805 File Offset: 0x000DAA05
		// (set) Token: 0x06001C79 RID: 7289 RVA: 0x000DC80D File Offset: 0x000DAA0D
		public Color LitBarColor
		{
			get
			{
				return this.m_litBarColor;
			}
			set
			{
				this.m_litBarColor = value;
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06001C7A RID: 7290 RVA: 0x000DC816 File Offset: 0x000DAA16
		// (set) Token: 0x06001C7B RID: 7291 RVA: 0x000DC81E File Offset: 0x000DAA1E
		public Color LitBarColor2
		{
			get
			{
				return this.m_litBarColor2;
			}
			set
			{
				this.m_litBarColor2 = value;
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06001C7C RID: 7292 RVA: 0x000DC827 File Offset: 0x000DAA27
		// (set) Token: 0x06001C7D RID: 7293 RVA: 0x000DC82F File Offset: 0x000DAA2F
		public Color UnlitBarColor
		{
			get
			{
				return this.m_unlitBarColor;
			}
			set
			{
				this.m_unlitBarColor = value;
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06001C7E RID: 7294 RVA: 0x000DC838 File Offset: 0x000DAA38
		// (set) Token: 0x06001C7F RID: 7295 RVA: 0x000DC840 File Offset: 0x000DAA40
		public bool BarBlending { get; set; }

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06001C80 RID: 7296 RVA: 0x000DC849 File Offset: 0x000DAA49
		// (set) Token: 0x06001C81 RID: 7297 RVA: 0x000DC851 File Offset: 0x000DAA51
		public bool HalfBars { get; set; }

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06001C82 RID: 7298 RVA: 0x000DC85A File Offset: 0x000DAA5A
		// (set) Token: 0x06001C83 RID: 7299 RVA: 0x000DC862 File Offset: 0x000DAA62
		public Subtexture BarSubtexture
		{
			get
			{
				return this.m_barSubtexture;
			}
			set
			{
				if (value != this.m_barSubtexture)
				{
					this.m_barSubtexture = value;
				}
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06001C84 RID: 7300 RVA: 0x000DC874 File Offset: 0x000DAA74
		// (set) Token: 0x06001C85 RID: 7301 RVA: 0x000DC87C File Offset: 0x000DAA7C
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

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06001C86 RID: 7302 RVA: 0x000DC88E File Offset: 0x000DAA8E
		// (set) Token: 0x06001C87 RID: 7303 RVA: 0x000DC896 File Offset: 0x000DAA96
		public LayoutDirection LayoutDirection
		{
			get
			{
				return this.m_layoutDirection;
			}
			set
			{
				this.m_layoutDirection = value;
			}
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x000DC8A0 File Offset: 0x000DAAA0
		public ValueBarWidget()
		{
			this.IsHitTestVisible = false;
			this.BarSize = new Vector2(24f);
			this.BarBlending = true;
			this.TextureLinearFilter = true;
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x000DC90E File Offset: 0x000DAB0E
		public void Flash(int count)
		{
			this.m_flashCount = MathUtils.Max(this.m_flashCount, (float)count);
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x000DC924 File Offset: 0x000DAB24
		public override void Draw(Widget.DrawContext dc)
		{
			BaseBatch baseBatch = (this.BarSubtexture == null) ? dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null) : dc.PrimitivesRenderer2D.TexturedBatch(this.BarSubtexture.Texture, false, 0, DepthStencilState.None, null, null, this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp);
			int start = 0;
			int count;
			if (baseBatch is TexturedBatch2D)
			{
				count = ((TexturedBatch2D)baseBatch).TriangleVertices.Count;
			}
			else
			{
				start = ((FlatBatch2D)baseBatch).LineVertices.Count;
				count = ((FlatBatch2D)baseBatch).TriangleVertices.Count;
			}
			Vector2 zero = Vector2.Zero;
			if (this.m_layoutDirection == LayoutDirection.Horizontal)
			{
				zero.X += this.Spacing / 2f;
			}
			else
			{
				zero.Y += this.Spacing / 2f;
			}
			int num = this.HalfBars ? 1 : 2;
			for (int i = 0; i < 2 * this.BarsCount; i += num)
			{
				bool flag = i % 2 == 0;
				float num2 = 0.5f * (float)i;
				float num3 = (!this.FlipDirection) ? MathUtils.Clamp((this.Value - num2 / (float)this.BarsCount) * (float)this.BarsCount, 0f, 1f) : MathUtils.Clamp((this.Value - ((float)this.BarsCount - num2 - 1f) / (float)this.BarsCount) * (float)this.BarsCount, 0f, 1f);
				if (!this.BarBlending)
				{
					num3 = MathUtils.Ceiling(num3);
				}
				float s = (this.m_flashCount > 0f) ? (1f - MathUtils.Abs(MathUtils.Sin(this.m_flashCount * 3.14159274f))) : 1f;
				Color c = this.LitBarColor;
				if (this.LitBarColor2 != Color.Transparent && this.BarsCount > 1)
				{
					c = Color.Lerp(this.LitBarColor, this.LitBarColor2, num2 / (float)(this.BarsCount - 1));
				}
				Color color = Color.Lerp(this.UnlitBarColor, c, num3) * s * base.GlobalColorTransform;
				if (this.HalfBars)
				{
					if (flag)
					{
						Vector2 zero2 = Vector2.Zero;
						Vector2 vector = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 1f) : new Vector2(1f, 0.5f);
						if (baseBatch is TexturedBatch2D)
						{
							Vector2 topLeft = this.BarSubtexture.TopLeft;
							Vector2 texCoord = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector.Y));
							((TexturedBatch2D)baseBatch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, topLeft, texCoord, color);
						}
						else
						{
							((FlatBatch2D)baseBatch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, color);
						}
					}
					else
					{
						Vector2 vector2 = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 0f) : new Vector2(0f, 0.5f);
						Vector2 one = Vector2.One;
						if (baseBatch is TexturedBatch2D)
						{
							Vector2 texCoord2 = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector2.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector2.Y));
							Vector2 bottomRight = this.BarSubtexture.BottomRight;
							((TexturedBatch2D)baseBatch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, texCoord2, bottomRight, color);
						}
						else
						{
							((FlatBatch2D)baseBatch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, color);
						}
					}
				}
				else
				{
					Vector2 zero3 = Vector2.Zero;
					Vector2 one2 = Vector2.One;
					if (baseBatch is TexturedBatch2D)
					{
						Vector2 topLeft2 = this.BarSubtexture.TopLeft;
						Vector2 bottomRight2 = this.BarSubtexture.BottomRight;
						((TexturedBatch2D)baseBatch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, topLeft2, bottomRight2, color);
					}
					else
					{
						((FlatBatch2D)baseBatch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, color);
						((FlatBatch2D)baseBatch).QueueRectangle(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, Color.MultiplyColorOnly(color, 0.75f));
					}
				}
				if (!flag || !this.HalfBars)
				{
					if (this.m_layoutDirection == LayoutDirection.Horizontal)
					{
						zero.X += this.BarSize.X + this.Spacing;
					}
					else
					{
						zero.Y += this.BarSize.Y + this.Spacing;
					}
				}
			}
			if (baseBatch is TexturedBatch2D)
			{
				((TexturedBatch2D)baseBatch).TransformTriangles(base.GlobalTransform, count, -1);
			}
			else
			{
				((FlatBatch2D)baseBatch).TransformLines(base.GlobalTransform, start, -1);
				((FlatBatch2D)baseBatch).TransformTriangles(base.GlobalTransform, count, -1);
			}
			this.m_flashCount = MathUtils.Max(this.m_flashCount - 4f * Time.FrameDuration, 0f);
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x000DCF58 File Offset: 0x000DB158
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = ((this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2((this.BarSize.X + this.Spacing) * (float)this.BarsCount, this.BarSize.Y) : new Vector2(this.BarSize.X, (this.BarSize.Y + this.Spacing) * (float)this.BarsCount));
		}

		// Token: 0x04001331 RID: 4913
		public float m_value;

		// Token: 0x04001332 RID: 4914
		public int m_barsCount = 8;

		// Token: 0x04001333 RID: 4915
		public Color m_litBarColor = new Color(16, 140, 0);

		// Token: 0x04001334 RID: 4916
		public Color m_litBarColor2 = Color.Transparent;

		// Token: 0x04001335 RID: 4917
		public Color m_unlitBarColor = new Color(48, 48, 48);

		// Token: 0x04001336 RID: 4918
		public Subtexture m_barSubtexture;

		// Token: 0x04001337 RID: 4919
		public LayoutDirection m_layoutDirection;

		// Token: 0x04001338 RID: 4920
		public float m_flashCount;

		// Token: 0x04001339 RID: 4921
		public bool m_textureLinearFilter;
	}
}
