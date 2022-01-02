using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200035A RID: 858
	public class WhalePlumeParticleSystem : ParticleSystem<WhalePlumeParticleSystem.Particle>
	{
		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001915 RID: 6421 RVA: 0x000C533B File Offset: 0x000C353B
		// (set) Token: 0x06001916 RID: 6422 RVA: 0x000C5343 File Offset: 0x000C3543
		public bool IsStopped { get; set; }

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06001917 RID: 6423 RVA: 0x000C534C File Offset: 0x000C354C
		// (set) Token: 0x06001918 RID: 6424 RVA: 0x000C5354 File Offset: 0x000C3554
		public Vector3 Position { get; set; }

		// Token: 0x06001919 RID: 6425 RVA: 0x000C535D File Offset: 0x000C355D
		public WhalePlumeParticleSystem(SubsystemTerrain terrain, float size, float duration) : base(100)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/WaterSplashParticle", null);
			base.TextureSlotsCount = 2;
			this.m_size = size;
			this.m_duration = duration;
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x000C5398 File Offset: 0x000C3598
		public override bool Simulate(float dt)
		{
			this.m_time += dt;
			if (this.m_time < this.m_duration && !this.IsStopped)
			{
				this.m_toGenerate += 60f * dt;
			}
			else
			{
				this.m_toGenerate = 0f;
			}
			float s = MathUtils.Pow(0.001f, dt);
			float num = MathUtils.Lerp(4f, 10f, MathUtils.Saturate(2f * this.m_time / this.m_duration));
			Vector3 v = new Vector3(0f, 1f, 2f);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				WhalePlumeParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= s;
						particle.Velocity += v * dt;
						particle.TextureSlot = (int)MathUtils.Min(4f * particle.Time / particle.Duration * 1.2f, 3f);
						particle.Size = new Vector2(this.m_size) * MathUtils.Lerp(0.1f, 0.2f, particle.Time / particle.Duration);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v2 = 0.1f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(0f, 2f), this.m_random.Float(-1f, 1f));
					particle.Position = this.Position + v2;
					particle.Color = new Color(200, 220, 210);
					particle.Velocity = 1f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), num * this.m_random.Float(0.3f, 1f), this.m_random.Float(-1f, 1f));
					particle.Size = Vector2.Zero;
					particle.Time = 0f;
					particle.Duration = this.m_random.Float(1f, 3f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			return !flag && (this.m_time >= this.m_duration || this.IsStopped);
		}

		// Token: 0x0400112D RID: 4397
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400112E RID: 4398
		public float m_time;

		// Token: 0x0400112F RID: 4399
		public float m_duration;

		// Token: 0x04001130 RID: 4400
		public float m_size;

		// Token: 0x04001131 RID: 4401
		public float m_toGenerate;

		// Token: 0x02000563 RID: 1379
		public class Particle : Game.Particle
		{
			// Token: 0x04001990 RID: 6544
			public Vector3 Velocity;

			// Token: 0x04001991 RID: 6545
			public float Time;

			// Token: 0x04001992 RID: 6546
			public float Duration;
		}
	}
}
