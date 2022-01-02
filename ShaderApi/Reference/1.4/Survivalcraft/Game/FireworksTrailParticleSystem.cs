using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200028F RID: 655
	public class FireworksTrailParticleSystem : ParticleSystem<FireworksTrailParticleSystem.Particle>, ITrailParticleSystem
	{
		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06001494 RID: 5268 RVA: 0x0009AAD0 File Offset: 0x00098CD0
		// (set) Token: 0x06001495 RID: 5269 RVA: 0x0009AAD8 File Offset: 0x00098CD8
		public Vector3 Position { get; set; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x0009AAE1 File Offset: 0x00098CE1
		// (set) Token: 0x06001497 RID: 5271 RVA: 0x0009AAE9 File Offset: 0x00098CE9
		public bool IsStopped { get; set; }

		// Token: 0x06001498 RID: 5272 RVA: 0x0009AAF2 File Offset: 0x00098CF2
		public FireworksTrailParticleSystem() : base(60)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0009AB20 File Offset: 0x00098D20
		public override bool Simulate(float dt)
		{
			float num = 120f;
			this.m_toGenerate += num * dt;
			if (this.m_lastPosition == null)
			{
				this.m_lastPosition = new Vector3?(this.Position);
			}
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				FireworksTrailParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration, 8f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					particle.Position = Vector3.Lerp(this.m_lastPosition.Value, this.Position, this.m_random.Float(0f, 1f));
					particle.Color = Color.White;
					particle.Time = this.m_random.Float(0f, 0.75f);
					particle.Size = new Vector2(this.m_random.Float(0.12f, 0.16f));
					particle.Duration = 1f;
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			this.m_lastPosition = new Vector3?(this.Position);
			return this.IsStopped && !flag;
		}

		// Token: 0x04000D67 RID: 3431
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D68 RID: 3432
		public float m_toGenerate;

		// Token: 0x04000D69 RID: 3433
		public Vector3? m_lastPosition;

		// Token: 0x0200050F RID: 1295
		public class Particle : Game.Particle
		{
			// Token: 0x04001850 RID: 6224
			public float Time;

			// Token: 0x04001851 RID: 6225
			public float Duration;
		}
	}
}
