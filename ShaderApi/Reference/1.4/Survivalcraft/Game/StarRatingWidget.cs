using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020003A1 RID: 929
	public class StarRatingWidget : Widget
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001C1E RID: 7198 RVA: 0x000DAF90 File Offset: 0x000D9190
		// (set) Token: 0x06001C1F RID: 7199 RVA: 0x000DAF98 File Offset: 0x000D9198
		public float StarSize { get; set; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06001C20 RID: 7200 RVA: 0x000DAFA1 File Offset: 0x000D91A1
		// (set) Token: 0x06001C21 RID: 7201 RVA: 0x000DAFA9 File Offset: 0x000D91A9
		public Color ForeColor { get; set; }

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001C22 RID: 7202 RVA: 0x000DAFB2 File Offset: 0x000D91B2
		// (set) Token: 0x06001C23 RID: 7203 RVA: 0x000DAFBA File Offset: 0x000D91BA
		public Color BackColor { get; set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001C24 RID: 7204 RVA: 0x000DAFC3 File Offset: 0x000D91C3
		// (set) Token: 0x06001C25 RID: 7205 RVA: 0x000DAFCB File Offset: 0x000D91CB
		public float Rating
		{
			get
			{
				return this.m_rating;
			}
			set
			{
				this.m_rating = MathUtils.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x000DAFE4 File Offset: 0x000D91E4
		public StarRatingWidget()
		{
			this.m_texture = ContentManager.Get<Texture2D>("Textures/Gui/RatingStar", null);
			this.ForeColor = new Color(255, 192, 0);
			this.BackColor = new Color(96, 96, 96);
			this.StarSize = 64f;
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x000DB03C File Offset: 0x000D923C
		public override void Update()
		{
			if (base.Input.Press != null && base.HitTestGlobal(base.Input.Press.Value, null) == this)
			{
				Vector2 vector = base.ScreenToWidget(base.Input.Press.Value);
				this.Rating = (float)((int)MathUtils.Floor(5f * vector.X / base.ActualSize.X + 1f));
			}
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x000DB0C0 File Offset: 0x000D92C0
		public override void Draw(Widget.DrawContext dc)
		{
			TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.m_texture, false, 0, DepthStencilState.None, null, null, SamplerState.LinearWrap);
			float x = 0f;
			float x2 = base.ActualSize.X * this.Rating / 5f;
			float x3 = base.ActualSize.X;
			float y = 0f;
			float y2 = base.ActualSize.Y;
			int count = texturedBatch2D.TriangleVertices.Count;
			texturedBatch2D.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), 0f, new Vector2(0f, 0f), new Vector2(this.Rating, 1f), this.ForeColor * base.GlobalColorTransform);
			texturedBatch2D.QueueQuad(new Vector2(x2, y), new Vector2(x3, y2), 0f, new Vector2(this.Rating, 0f), new Vector2(5f, 1f), this.BackColor * base.GlobalColorTransform);
			texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x000DB1D9 File Offset: 0x000D93D9
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = new Vector2(5f * this.StarSize, this.StarSize);
		}

		// Token: 0x04001309 RID: 4873
		public Texture2D m_texture;

		// Token: 0x0400130A RID: 4874
		public float m_rating;
	}
}
