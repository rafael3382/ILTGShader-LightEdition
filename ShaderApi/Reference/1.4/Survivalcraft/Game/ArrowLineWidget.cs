using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
	// Token: 0x0200036B RID: 875
	public class ArrowLineWidget : Widget
	{
		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060019AD RID: 6573 RVA: 0x000CAEFC File Offset: 0x000C90FC
		// (set) Token: 0x060019AE RID: 6574 RVA: 0x000CAF04 File Offset: 0x000C9104
		public float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060019AF RID: 6575 RVA: 0x000CAF14 File Offset: 0x000C9114
		// (set) Token: 0x060019B0 RID: 6576 RVA: 0x000CAF1C File Offset: 0x000C911C
		public float ArrowWidth
		{
			get
			{
				return this.m_arrowWidth;
			}
			set
			{
				this.m_arrowWidth = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x000CAF2C File Offset: 0x000C912C
		// (set) Token: 0x060019B2 RID: 6578 RVA: 0x000CAF34 File Offset: 0x000C9134
		public Color Color { get; set; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060019B3 RID: 6579 RVA: 0x000CAF3D File Offset: 0x000C913D
		// (set) Token: 0x060019B4 RID: 6580 RVA: 0x000CAF45 File Offset: 0x000C9145
		public string PointsString
		{
			get
			{
				return this.m_pointsString;
			}
			set
			{
				this.m_pointsString = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x000CAF55 File Offset: 0x000C9155
		// (set) Token: 0x060019B6 RID: 6582 RVA: 0x000CAF5D File Offset: 0x000C915D
		public bool AbsoluteCoordinates
		{
			get
			{
				return this.m_absoluteCoordinates;
			}
			set
			{
				this.m_absoluteCoordinates = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x000CAF70 File Offset: 0x000C9170
		public ArrowLineWidget()
		{
			this.Width = 6f;
			this.ArrowWidth = 0f;
			this.Color = Color.White;
			this.IsHitTestVisible = false;
			this.PointsString = "0, 0; 50, 0";
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x000CAFC4 File Offset: 0x000C91C4
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_parsingPending)
			{
				this.ParsePoints();
			}
			Color color = this.Color * base.GlobalColorTransform;
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, null);
			int count = flatBatch2D.TriangleVertices.Count;
			for (int i = 0; i < this.m_vertices.Count; i += 3)
			{
				Vector2 p = this.m_startOffset + this.m_vertices[i];
				Vector2 p2 = this.m_startOffset + this.m_vertices[i + 1];
				Vector2 p3 = this.m_startOffset + this.m_vertices[i + 2];
				flatBatch2D.QueueTriangle(p, p2, p3, 0f, color);
			}
			flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x000CB098 File Offset: 0x000C9298
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_parsingPending)
			{
				this.ParsePoints();
			}
			base.IsDrawRequired = (this.Color.A > 0 && this.Width > 0f);
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x000CB0DC File Offset: 0x000C92DC
		public void ParsePoints()
		{
			this.m_parsingPending = false;
			List<Vector2> list = new List<Vector2>();
			foreach (string data in this.m_pointsString.Split(new string[]
			{
				";"
			}, StringSplitOptions.None))
			{
				list.Add(HumanReadableConverter.ConvertFromString<Vector2>(data));
			}
			this.m_vertices.Clear();
			for (int j = 0; j < list.Count; j++)
			{
				if (j >= 1)
				{
					Vector2 vector = list[j - 1];
					Vector2 vector2 = list[j];
					Vector2 vector3 = Vector2.Normalize(vector2 - vector);
					Vector2 vector4 = vector3;
					Vector2 v = vector3;
					if (j >= 2)
					{
						vector4 = Vector2.Normalize(vector - list[j - 2]);
					}
					if (j <= list.Count - 2)
					{
						v = Vector2.Normalize(list[j + 1] - vector2);
					}
					Vector2 v2 = Vector2.Perpendicular(vector4);
					Vector2 v3 = Vector2.Perpendicular(vector3);
					float num = 3.14159274f - Vector2.Angle(vector4, vector3);
					float s = 0.5f * this.Width / MathUtils.Tan(num / 2f);
					Vector2 v4 = 0.5f * v2 * this.Width - vector4 * s;
					float num2 = 3.14159274f - Vector2.Angle(vector3, v);
					float s2 = 0.5f * this.Width / MathUtils.Tan(num2 / 2f);
					Vector2 v5 = 0.5f * v3 * this.Width - vector3 * s2;
					this.m_vertices.Add(vector + v4);
					this.m_vertices.Add(vector - v4);
					this.m_vertices.Add(vector2 - v5);
					this.m_vertices.Add(vector2 - v5);
					this.m_vertices.Add(vector2 + v5);
					this.m_vertices.Add(vector + v4);
					if (j == list.Count - 1)
					{
						this.m_vertices.Add(vector2 - 0.5f * this.ArrowWidth * v3);
						this.m_vertices.Add(vector2 + 0.5f * this.ArrowWidth * v3);
						this.m_vertices.Add(vector2 + 0.5f * this.ArrowWidth * vector3);
					}
				}
			}
			if (this.m_vertices.Count <= 0)
			{
				base.DesiredSize = Vector2.Zero;
				this.m_startOffset = Vector2.Zero;
				return;
			}
			float? num3 = null;
			float? num4 = null;
			float? num5 = null;
			float? num6 = null;
			int k = 0;
			while (k < this.m_vertices.Count)
			{
				if (num3 == null)
				{
					goto IL_2FB;
				}
				float x = this.m_vertices[k].X;
				float? num7 = num3;
				if (x < num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_2FB;
				}
				IL_314:
				if (num4 == null)
				{
					goto IL_346;
				}
				float y = this.m_vertices[k].Y;
				num7 = num4;
				if (y < num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_346;
				}
				IL_35F:
				if (num5 == null)
				{
					goto IL_391;
				}
				float x2 = this.m_vertices[k].X;
				num7 = num5;
				if (x2 > num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_391;
				}
				IL_3AA:
				if (num6 == null)
				{
					goto IL_3DC;
				}
				float y2 = this.m_vertices[k].Y;
				num7 = num6;
				if (y2 > num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_3DC;
				}
				IL_3F5:
				k++;
				continue;
				IL_3DC:
				num6 = new float?(this.m_vertices[k].Y);
				goto IL_3F5;
				IL_391:
				num5 = new float?(this.m_vertices[k].X);
				goto IL_3AA;
				IL_346:
				num4 = new float?(this.m_vertices[k].Y);
				goto IL_35F;
				IL_2FB:
				num3 = new float?(this.m_vertices[k].X);
				goto IL_314;
			}
			if (this.AbsoluteCoordinates)
			{
				base.DesiredSize = new Vector2(num5.Value, num6.Value);
				this.m_startOffset = Vector2.Zero;
				return;
			}
			base.DesiredSize = new Vector2(num5.Value - num3.Value, num6.Value - num4.Value);
			this.m_startOffset = -new Vector2(num3.Value, num4.Value);
		}

		// Token: 0x0400119D RID: 4509
		public string m_pointsString;

		// Token: 0x0400119E RID: 4510
		public float m_width;

		// Token: 0x0400119F RID: 4511
		public float m_arrowWidth;

		// Token: 0x040011A0 RID: 4512
		public bool m_absoluteCoordinates;

		// Token: 0x040011A1 RID: 4513
		public List<Vector2> m_vertices = new List<Vector2>();

		// Token: 0x040011A2 RID: 4514
		public bool m_parsingPending;

		// Token: 0x040011A3 RID: 4515
		public Vector2 m_startOffset;
	}
}
