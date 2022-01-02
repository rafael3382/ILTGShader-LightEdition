using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200030A RID: 778
	public class SoundParticleSystem : ParticleSystem<SoundParticleSystem.Particle>
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x000ACD48 File Offset: 0x000AAF48
		public SoundParticleSystem(SubsystemTerrain terrain, Vector3 position, Vector3 direction) : base(15)
		{
			this.m_position = position;
			this.m_direction = direction;
			base.Texture = ContentManager.Get<Texture2D>("Textures/SoundParticle", null);
			base.TextureSlotsCount = 2;
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x000ACD84 File Offset: 0x000AAF84
		public void AddNote(Color color)
		{
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SoundParticleSystem.Particle particle = base.Particles[i];
				if (!base.Particles[i].IsActive)
				{
					particle.IsActive = true;
					particle.Position = this.m_position;
					particle.Color = Color.White;
					particle.Size = new Vector2(0.1f);
					particle.TimeToLive = this.m_random.Float(1f, 1.5f);
					particle.Velocity = 3f * (this.m_direction + this.m_random.Vector3(0.5f));
					particle.BaseColor = color;
					particle.TextureSlot = this.m_random.Int(0, base.TextureSlotsCount * base.TextureSlotsCount - 1);
					particle.BillboardingMode = ParticleBillboardingMode.Vertical;
					return;
				}
			}
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x000ACE60 File Offset: 0x000AB060
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.02f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				SoundParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						particle.Velocity += new Vector3(0f, 5f, 0f) * dt;
						particle.Velocity *= s;
						particle.Position += particle.Velocity * dt;
						particle.Color = particle.BaseColor * MathUtils.Saturate(2f * particle.TimeToLive);
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000FA0 RID: 4000
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000FA1 RID: 4001
		public Vector3 m_position;

		// Token: 0x04000FA2 RID: 4002
		public Vector3 m_direction;

		// Token: 0x0200053F RID: 1343
		public class Particle : Game.Particle
		{
			// Token: 0x040018E9 RID: 6377
			public float TimeToLive;

			// Token: 0x040018EA RID: 6378
			public Vector3 Velocity;

			// Token: 0x040018EB RID: 6379
			public Color BaseColor;
		}
	}
}
