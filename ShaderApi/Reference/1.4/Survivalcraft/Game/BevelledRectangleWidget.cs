using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200036D RID: 877
	public class BevelledRectangleWidget : Widget
	{
		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x060019D6 RID: 6614 RVA: 0x000CB801 File Offset: 0x000C9A01
		// (set) Token: 0x060019D7 RID: 6615 RVA: 0x000CB809 File Offset: 0x000C9A09
		public Vector2 Size { get; set; }

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x060019D8 RID: 6616 RVA: 0x000CB812 File Offset: 0x000C9A12
		// (set) Token: 0x060019D9 RID: 6617 RVA: 0x000CB81A File Offset: 0x000C9A1A
		public float BevelSize { get; set; }

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x060019DA RID: 6618 RVA: 0x000CB823 File Offset: 0x000C9A23
		// (set) Token: 0x060019DB RID: 6619 RVA: 0x000CB82B File Offset: 0x000C9A2B
		public float DirectionalLight { get; set; }

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x060019DC RID: 6620 RVA: 0x000CB834 File Offset: 0x000C9A34
		// (set) Token: 0x060019DD RID: 6621 RVA: 0x000CB83C File Offset: 0x000C9A3C
		public float AmbientLight { get; set; }

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x060019DE RID: 6622 RVA: 0x000CB845 File Offset: 0x000C9A45
		// (set) Token: 0x060019DF RID: 6623 RVA: 0x000CB84D File Offset: 0x000C9A4D
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
			set
			{
				if (value != this.m_texture)
				{
					this.m_texture = value;
				}
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x060019E0 RID: 6624 RVA: 0x000CB85F File Offset: 0x000C9A5F
		// (set) Token: 0x060019E1 RID: 6625 RVA: 0x000CB867 File Offset: 0x000C9A67
		public float TextureScale { get; set; }

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060019E2 RID: 6626 RVA: 0x000CB870 File Offset: 0x000C9A70
		// (set) Token: 0x060019E3 RID: 6627 RVA: 0x000CB878 File Offset: 0x000C9A78
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

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x060019E4 RID: 6628 RVA: 0x000CB88A File Offset: 0x000C9A8A
		// (set) Token: 0x060019E5 RID: 6629 RVA: 0x000CB892 File Offset: 0x000C9A92
		public Color CenterColor { get; set; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x060019E6 RID: 6630 RVA: 0x000CB89B File Offset: 0x000C9A9B
		// (set) Token: 0x060019E7 RID: 6631 RVA: 0x000CB8A3 File Offset: 0x000C9AA3
		public Color BevelColor { get; set; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x060019E8 RID: 6632 RVA: 0x000CB8AC File Offset: 0x000C9AAC
		// (set) Token: 0x060019E9 RID: 6633 RVA: 0x000CB8B4 File Offset: 0x000C9AB4
		public Color ShadowColor { get; set; }

		// Token: 0x060019EA RID: 6634 RVA: 0x000CB8C0 File Offset: 0x000C9AC0
		public BevelledRectangleWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.BevelSize = 2f;
			this.AmbientLight = 0.6f;
			this.DirectionalLight = 0.4f;
			this.TextureScale = 1f;
			this.TextureLinearFilter = false;
			this.CenterColor = new Color(181, 172, 154);
			this.BevelColor = new Color(181, 172, 154);
			this.ShadowColor = new Color(0, 0, 0, 80);
			this.IsHitTestVisible = false;
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x000CB964 File Offset: 0x000C9B64
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Texture != null)
			{
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap;
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
				TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.Texture, false, 0, DepthStencilState.None, null, null, samplerState);
				int count = flatBatch2D.TriangleVertices.Count;
				int count2 = texturedBatch2D.TriangleVertices.Count;
				BevelledRectangleWidget.QueueBevelledRectangle(texturedBatch2D, flatBatch2D, Vector2.Zero, base.ActualSize, 0f, this.BevelSize, this.CenterColor * base.GlobalColorTransform, this.BevelColor * base.GlobalColorTransform, this.ShadowColor * base.GlobalColorTransform, this.AmbientLight, this.DirectionalLight, this.TextureScale);
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				texturedBatch2D.TransformTriangles(base.GlobalTransform, count2, -1);
				return;
			}
			FlatBatch2D flatBatch2D2 = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
			int count3 = flatBatch2D2.TriangleVertices.Count;
			BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D2, Vector2.Zero, base.ActualSize, 0f, this.BevelSize, this.CenterColor * base.GlobalColorTransform, this.BevelColor * base.GlobalColorTransform, this.ShadowColor * base.GlobalColorTransform, this.AmbientLight, this.DirectionalLight, 0f);
			flatBatch2D2.TransformTriangles(base.GlobalTransform, count3, -1);
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x000CBAEC File Offset: 0x000C9CEC
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.BevelColor.A != 0 || this.CenterColor.A > 0);
			base.DesiredSize = this.Size;
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x000CBB30 File Offset: 0x000C9D30
		public static void QueueBevelledRectangle(TexturedBatch2D texturedBatch, FlatBatch2D flatBatch, Vector2 c1, Vector2 c2, float depth, float bevelSize, Color color, Color bevelColor, Color shadowColor, float ambientLight, float directionalLight, float textureScale)
		{
			float num = MathUtils.Abs(bevelSize);
			Vector2 vector = c1 + new Vector2(num);
			Vector2 vector2 = c2 - new Vector2(num);
			Vector2 vector3 = c2 + new Vector2(1.5f * num);
			float x = c1.X;
			float x2 = vector.X;
			float x3 = vector2.X;
			float x4 = c2.X;
			float x5 = vector3.X;
			float y = c1.Y;
			float y2 = vector.Y;
			float y3 = vector2.Y;
			float y4 = c2.Y;
			float y5 = vector3.Y;
			float num2 = MathUtils.Saturate(((bevelSize > 0f) ? 1f : -0.75f) * directionalLight + ambientLight);
			float num3 = MathUtils.Saturate(((bevelSize > 0f) ? -0.75f : 1f) * directionalLight + ambientLight);
			float num4 = MathUtils.Saturate(((bevelSize > 0f) ? -0.375f : 0.5f) * directionalLight + ambientLight);
			float num5 = MathUtils.Saturate(((bevelSize > 0f) ? 0.5f : -0.375f) * directionalLight + ambientLight);
			float num6 = MathUtils.Saturate(0f * directionalLight + ambientLight);
			Color color2 = new Color((byte)(num4 * (float)bevelColor.R), (byte)(num4 * (float)bevelColor.G), (byte)(num4 * (float)bevelColor.B), bevelColor.A);
			Color color3 = new Color((byte)(num5 * (float)bevelColor.R), (byte)(num5 * (float)bevelColor.G), (byte)(num5 * (float)bevelColor.B), bevelColor.A);
			Color color4 = new Color((byte)(num2 * (float)bevelColor.R), (byte)(num2 * (float)bevelColor.G), (byte)(num2 * (float)bevelColor.B), bevelColor.A);
			Color color5 = new Color((byte)(num3 * (float)bevelColor.R), (byte)(num3 * (float)bevelColor.G), (byte)(num3 * (float)bevelColor.B), bevelColor.A);
			Color color6 = new Color((byte)(num6 * (float)color.R), (byte)(num6 * (float)color.G), (byte)(num6 * (float)color.B), color.A);
			if (texturedBatch != null)
			{
				float num7 = textureScale / (float)texturedBatch.Texture.Width;
				float num8 = textureScale / (float)texturedBatch.Texture.Height;
				float num9 = x * num7;
				float num10 = y * num8;
				float x6 = num9;
				float x7 = (x2 - x) * num7 + num9;
				float x8 = (x3 - x) * num7 + num9;
				float x9 = (x4 - x) * num7 + num9;
				float y6 = num10;
				float y7 = (y2 - y) * num8 + num10;
				float y8 = (y3 - y) * num8 + num10;
				float y9 = (y4 - y) * num8 + num10;
				if (bevelColor.A > 0)
				{
					texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, new Vector2(x6, y6), new Vector2(x7, y7), new Vector2(x8, y7), new Vector2(x9, y6), color4);
					texturedBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, new Vector2(x8, y7), new Vector2(x8, y8), new Vector2(x9, y9), new Vector2(x9, y6), color3);
					texturedBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, new Vector2(x6, y9), new Vector2(x9, y9), new Vector2(x8, y8), new Vector2(x7, y8), color5);
					texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, new Vector2(x6, y6), new Vector2(x6, y9), new Vector2(x7, y8), new Vector2(x7, y7), color2);
				}
				if (color6.A > 0)
				{
					texturedBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, new Vector2(x7, y7), new Vector2(x8, y8), color6);
				}
			}
			else if (flatBatch != null)
			{
				if (bevelColor.A > 0)
				{
					flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, color4);
					flatBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, color3);
					flatBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, color5);
					flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, color2);
				}
				if (color6.A > 0)
				{
					flatBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, color6);
				}
			}
			if (bevelSize > 0f && flatBatch != null && shadowColor.A > 0)
			{
				Color color7 = shadowColor;
				Color color8 = new Color(0, 0, 0, 0);
				flatBatch.QueueTriangle(new Vector2(x, y4), new Vector2(x2, y5), new Vector2(x2, y4), depth, color8, color8, color7);
				flatBatch.QueueTriangle(new Vector2(x4, y), new Vector2(x4, y2), new Vector2(x5, y2), depth, color8, color7, color8);
				flatBatch.QueueTriangle(new Vector2(x4, y4), new Vector2(x4, y5), new Vector2(x5, y4), depth, color7, color8, color8);
				flatBatch.QueueQuad(new Vector2(x2, y4), new Vector2(x2, y5), new Vector2(x4, y5), new Vector2(x4, y4), depth, color7, color8, color8, color7);
				flatBatch.QueueQuad(new Vector2(x4, y2), new Vector2(x4, y4), new Vector2(x5, y4), new Vector2(x5, y2), depth, color7, color7, color8, color8);
			}
		}

		// Token: 0x040011AB RID: 4523
		public Texture2D m_texture;

		// Token: 0x040011AC RID: 4524
		public bool m_textureLinearFilter;
	}
}
