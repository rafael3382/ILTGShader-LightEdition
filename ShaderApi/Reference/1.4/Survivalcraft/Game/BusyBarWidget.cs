using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000371 RID: 881
	public class BusyBarWidget : Widget
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06001A19 RID: 6681 RVA: 0x000CC8DE File Offset: 0x000CAADE
		// (set) Token: 0x06001A1A RID: 6682 RVA: 0x000CC8E6 File Offset: 0x000CAAE6
		public Color LitBarColor { get; set; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001A1B RID: 6683 RVA: 0x000CC8EF File Offset: 0x000CAAEF
		// (set) Token: 0x06001A1C RID: 6684 RVA: 0x000CC8F7 File Offset: 0x000CAAF7
		public Color UnlitBarColor { get; set; }

		// Token: 0x06001A1D RID: 6685 RVA: 0x000CC900 File Offset: 0x000CAB00
		public BusyBarWidget()
		{
			this.IsHitTestVisible = false;
			this.LitBarColor = new Color(16, 140, 0);
			this.UnlitBarColor = new Color(48, 48, 48);
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x000CC933 File Offset: 0x000CAB33
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = new Vector2(120f, 12f);
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x000CC954 File Offset: 0x000CAB54
		public override void Draw(Widget.DrawContext dc)
		{
			if (Time.RealTime - this.m_lastBoxesStepTime > 0.25)
			{
				this.m_boxIndex++;
				this.m_lastBoxesStepTime = Time.RealTime;
			}
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, null, null, null);
			int count = flatBatch2D.TriangleVertices.Count;
			for (int i = 0; i < 5; i++)
			{
				Vector2 v = new Vector2(((float)i + 0.5f) * 24f, 6f);
				Color c = (i == this.m_boxIndex % 5) ? this.LitBarColor : this.UnlitBarColor;
				float v2 = (i == this.m_boxIndex % 5) ? 12f : 8f;
				flatBatch2D.QueueQuad(v - new Vector2(v2) / 2f, v + new Vector2(v2) / 2f, 0f, c * base.GlobalColorTransform);
			}
			flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x040011CA RID: 4554
		public const int m_barsCount = 5;

		// Token: 0x040011CB RID: 4555
		public const float m_barSize = 8f;

		// Token: 0x040011CC RID: 4556
		public const float m_barsSpacing = 24f;

		// Token: 0x040011CD RID: 4557
		public int m_boxIndex;

		// Token: 0x040011CE RID: 4558
		public double m_lastBoxesStepTime;
	}
}
