using System;
using Engine;

namespace Game
{
	// Token: 0x02000383 RID: 899
	public class FireWidget : CanvasWidget
	{
		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x000D01AB File Offset: 0x000CE3AB
		// (set) Token: 0x06001AB4 RID: 6836 RVA: 0x000D01B8 File Offset: 0x000CE3B8
		public float ParticlesPerSecond
		{
			get
			{
				return this.m_fireRenderer.ParticlesPerSecond;
			}
			set
			{
				this.m_fireRenderer.ParticlesPerSecond = value;
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000D01C6 File Offset: 0x000CE3C6
		public FireWidget()
		{
			base.ClampToBounds = true;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x000D01E2 File Offset: 0x000CE3E2
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x000D01F2 File Offset: 0x000CE3F2
		public override void Draw(Widget.DrawContext dc)
		{
			this.m_fireRenderer.Draw(dc.PrimitivesRenderer2D, 0f, base.GlobalTransform, base.GlobalColorTransform);
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x000D0218 File Offset: 0x000CE418
		public override void Update()
		{
			float dt = MathUtils.Clamp(Time.FrameDuration, 0f, 0.1f);
			this.m_fireRenderer.Origin = new Vector2(0f, base.ActualSize.Y);
			this.m_fireRenderer.CutoffPosition = float.NegativeInfinity;
			this.m_fireRenderer.ParticleSize = 32f;
			this.m_fireRenderer.ParticleSpeed = 32f;
			this.m_fireRenderer.Width = base.ActualSize.X;
			this.m_fireRenderer.MinTimeToLive = 0.5f;
			this.m_fireRenderer.MaxTimeToLive = 2f;
			this.m_fireRenderer.ParticleAnimationPeriod = 1.25f;
			this.m_fireRenderer.Update(dt);
		}

		// Token: 0x04001232 RID: 4658
		public ScreenSpaceFireRenderer m_fireRenderer = new ScreenSpaceFireRenderer(100);
	}
}
