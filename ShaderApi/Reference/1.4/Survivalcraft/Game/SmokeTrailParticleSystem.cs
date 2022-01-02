using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000306 RID: 774
	public class SmokeTrailParticleSystem : ParticleSystem<SmokeTrailParticleSystem.Particle>, ITrailParticleSystem
	{
		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060016CB RID: 5835 RVA: 0x000ABEFA File Offset: 0x000AA0FA
		// (set) Token: 0x060016CC RID: 5836 RVA: 0x000ABF02 File Offset: 0x000AA102
		public Vector3 Position { get; set; }

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060016CD RID: 5837 RVA: 0x000ABF0B File Offset: 0x000AA10B
		// (set) Token: 0x060016CE RID: 5838 RVA: 0x000ABF13 File Offset: 0x000AA113
		public bool IsStopped { get; set; }

		// Token: 0x060016CF RID: 5839 RVA: 0x000ABF1C File Offset: 0x000AA11C
		public SmokeTrailParticleSystem(int particlesCount, float size, float maxDuration, Color color) : base(particlesCount)
		{
			this.m_size = size;
			this.m_maxDuration = maxDuration;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
			this.m_textureSlotMultiplier = this.m_random.Float(1.1f, 1.9f);
			this.m_textureSlotOffset = (float)((this.m_random.Float(0f, 1f) < 0.33f) ? 3 : 0);
			this.m_color = color;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x000ABFAC File Offset: 0x000AA1AC
		public override bool Simulate(float dt)
		{
			this.m_duration += dt;
			if (this.m_duration > this.m_maxDuration)
			{
				this.IsStopped = true;
			}
			float num = MathUtils.Clamp(50f / this.m_size, 10f, 40f);
			this.m_toGenerate += num * dt;
			float s = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SmokeTrailParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.Velocity *= s;
						SmokeTrailParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + 10f * dt;
						particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration * this.m_textureSlotMultiplier + this.m_textureSlotOffset, 8f);
						particle.Size = new Vector2(this.m_size * (0.15f + 0.8f * particle.Time / particle.Duration));
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					particle.IsActive = true;
					Vector3 v = new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
					particle.Position = this.Position + 0.025f * v;
					particle.Color = this.m_color;
					particle.Velocity = 0.2f * v;
					particle.Time = 0f;
					particle.Size = new Vector2(0.15f * this.m_size);
					particle.Duration = (float)base.Particles.Length / num * this.m_random.Float(0.8f, 1.05f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x04000F86 RID: 3974
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F87 RID: 3975
		public float m_toGenerate;

		// Token: 0x04000F88 RID: 3976
		public float m_textureSlotMultiplier;

		// Token: 0x04000F89 RID: 3977
		public float m_textureSlotOffset;

		// Token: 0x04000F8A RID: 3978
		public float m_duration;

		// Token: 0x04000F8B RID: 3979
		public float m_size;

		// Token: 0x04000F8C RID: 3980
		public float m_maxDuration;

		// Token: 0x04000F8D RID: 3981
		public Color m_color;

		// Token: 0x0200053C RID: 1340
		public class Particle : Game.Particle
		{
			// Token: 0x040018DF RID: 6367
			public float Time;

			// Token: 0x040018E0 RID: 6368
			public float Duration;

			// Token: 0x040018E1 RID: 6369
			public Vector3 Velocity;
		}
	}
}
