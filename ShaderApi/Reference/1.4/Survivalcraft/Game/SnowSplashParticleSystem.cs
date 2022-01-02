using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000307 RID: 775
	public class SnowSplashParticleSystem : ParticleSystem<SnowSplashParticleSystem.Particle>
	{
		// Token: 0x060016D1 RID: 5841 RVA: 0x000AC25E File Offset: 0x000AA45E
		public SnowSplashParticleSystem() : base(100)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/SnowParticle", null);
			base.TextureSlotsCount = 4;
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000AC28C File Offset: 0x000AA48C
		public void AddSplash(int value, Vector3 position, Vector2 size, Color color, int textureSlot)
		{
			int i = 0;
			while (i < base.Particles.Length)
			{
				SnowSplashParticleSystem.Particle particle = base.Particles[i];
				if (!particle.IsActive)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					particle.IsActive = true;
					particle.Position = position;
					particle.BaseColor = color;
					particle.BillboardingMode = ParticleBillboardingMode.Horizontal;
					particle.Size = size;
					particle.TextureSlot = textureSlot;
					if (block is WaterBlock)
					{
						((WaterBlock)block).GetLevelHeight(FluidBlock.GetLevel(Terrain.ExtractData(value)));
						particle.TimeToLive = this.m_random.Float(0.2f, 0.3f);
						particle.FadeFactor = 1f;
						break;
					}
					if (block.IsCollidable_(value) || block is SnowBlock)
					{
						particle.TimeToLive = this.m_random.Float(0.8f, 1.2f);
						particle.FadeFactor = 1f;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.m_isActive = true;
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x000AC388 File Offset: 0x000AA588
		public override bool Simulate(float dt)
		{
			if (this.m_isActive)
			{
				dt = MathUtils.Clamp(dt, 0f, 0.1f);
				bool flag = false;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					SnowSplashParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						particle.Color = particle.BaseColor * MathUtils.Saturate(particle.FadeFactor * particle.TimeToLive);
						particle.TimeToLive -= dt;
						if (particle.TimeToLive <= 0f)
						{
							particle.IsActive = false;
						}
						else
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					this.m_isActive = false;
				}
			}
			return false;
		}

		// Token: 0x04000F90 RID: 3984
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F91 RID: 3985
		public bool m_isActive;

		// Token: 0x0200053D RID: 1341
		public class Particle : Game.Particle
		{
			// Token: 0x040018E2 RID: 6370
			public float TimeToLive;

			// Token: 0x040018E3 RID: 6371
			public Color BaseColor;

			// Token: 0x040018E4 RID: 6372
			public float FadeFactor;
		}
	}
}
