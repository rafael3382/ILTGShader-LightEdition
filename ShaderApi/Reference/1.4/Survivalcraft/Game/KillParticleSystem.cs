using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002B8 RID: 696
	public class KillParticleSystem : ParticleSystem<KillParticleSystem.Particle>
	{
		// Token: 0x06001572 RID: 5490 RVA: 0x000A169C File Offset: 0x0009F89C
		public KillParticleSystem(SubsystemTerrain terrain, Vector3 position, float size) : base(20)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/KillParticle", null);
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			int num4 = 0;
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num + 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num - 1, num2, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 + 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2 - 1, num3));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 + 1));
			num4 = MathUtils.Max(num4, terrain.Terrain.GetCellLight(num, num2, num3 - 1));
			base.TextureSlotsCount = 2;
			Color color = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			color *= s;
			color.A = byte.MaxValue;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				KillParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				particle.Position = position + 0.4f * size * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Color = color;
				particle.Size = new Vector2(0.3f * size);
				particle.TimeToLive = this.m_random.Float(0.5f, 3.5f);
				particle.Velocity = 1.2f * size * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.FlipX = this.m_random.Bool();
				particle.FlipY = this.m_random.Bool();
			}
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x000A18E0 File Offset: 0x0009FAE0
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.1f, dt);
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				KillParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						particle.Position += particle.Velocity * dt;
						KillParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + 1f * dt;
						particle.Velocity *= s;
						particle.TextureSlot = (int)(3.99f * MathUtils.Saturate(2f - particle.TimeToLive));
					}
					else
					{
						particle.IsActive = false;
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000DF1 RID: 3569
		public Game.Random m_random = new Game.Random();

		// Token: 0x0200051B RID: 1307
		public class Particle : Game.Particle
		{
			// Token: 0x04001882 RID: 6274
			public Vector3 Velocity;

			// Token: 0x04001883 RID: 6275
			public float TimeToLive;
		}
	}
}
