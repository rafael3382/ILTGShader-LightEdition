﻿using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002C0 RID: 704
	public class MagmaSplashParticleSystem : ParticleSystem<MagmaSplashParticleSystem.Particle>
	{
		// Token: 0x06001588 RID: 5512 RVA: 0x000A22E0 File Offset: 0x000A04E0
		public MagmaSplashParticleSystem(SubsystemTerrain terrain, Vector3 position, bool large) : base(40)
		{
			this.m_subsystemTerrain = terrain;
			this.m_position = position;
			base.Texture = ContentManager.Get<Texture2D>("Textures/MagmaSplashParticle", null);
			base.TextureSlotsCount = 2;
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
			Color color = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			color *= s;
			color.A = byte.MaxValue;
			float num5 = large ? 1.5f : 1f;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				MagmaSplashParticleSystem.Particle particle = base.Particles[i];
				particle.IsActive = true;
				particle.Position = position;
				particle.Color = color;
				particle.Size = new Vector2(0.2f * num5);
				particle.TimeToLive = (particle.Duration = this.m_random.Float(0.5f, 3.5f));
				Vector3 v = 4f * this.m_random.Float(0.1f, 1f) * Vector3.Normalize(new Vector3(this.m_random.Float(-1f, 1f), 0f, this.m_random.Float(-1f, 1f)));
				particle.Velocity = num5 * (v + new Vector3(0f, this.m_random.Float(0f, 4f), 0f));
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x000A2518 File Offset: 0x000A0718
		public override bool Simulate(float dt)
		{
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s = MathUtils.Pow(0.015f, dt);
			this.m_time += dt;
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				MagmaSplashParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Position += particle.Velocity * dt;
					MagmaSplashParticleSystem.Particle particle2 = particle;
					particle2.Velocity.Y = particle2.Velocity.Y + -10f * dt;
					particle.Velocity *= s;
					particle.Color *= MathUtils.Saturate(particle.TimeToLive);
					particle.TimeToLive -= dt;
					particle.TextureSlot = (int)(3.99f * particle.TimeToLive / particle.Duration);
					particle.FlipX = (this.m_random.Sign() > 0);
					particle.FlipY = (this.m_random.Sign() > 0);
					if (particle.TimeToLive <= 0f || particle.Size.X <= 0f)
					{
						particle.IsActive = false;
					}
					else
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(particle.Position.X), Terrain.ToCell(particle.Position.Y), Terrain.ToCell(particle.Position.Z));
						int num = Terrain.ExtractContents(cellValue);
						if (num != 0)
						{
							Block block = BlocksManager.Blocks[num];
							if (block.IsCollidable_(cellValue))
							{
								particle.IsActive = true;
							}
							else if (block is MagmaBlock)
							{
								int level = FluidBlock.GetLevel(Terrain.ExtractData(cellValue));
								float levelHeight = ((MagmaBlock)block).GetLevelHeight(level);
								if (particle.Position.Y <= MathUtils.Floor(particle.Position.Y) + levelHeight)
								{
									particle.Velocity.Y = 0f;
									float num2 = Vector2.Distance(new Vector2(particle.Position.X, particle.Position.Z), new Vector2(this.m_position.X, this.m_position.Z));
									float num3 = 0.02f * MathUtils.Sin(2f * num2 + 10f * this.m_time);
									particle.Position.Y = MathUtils.Floor(particle.Position.Y) + levelHeight + num3;
									particle.TimeToLive -= 1f * dt;
									particle.Size -= new Vector2(0.04f * dt);
								}
							}
						}
					}
				}
			}
			return !flag;
		}

		// Token: 0x04000E0C RID: 3596
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000E0D RID: 3597
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000E0E RID: 3598
		public Vector3 m_position;

		// Token: 0x04000E0F RID: 3599
		public float m_time;

		// Token: 0x0200051D RID: 1309
		public class Particle : Game.Particle
		{
			// Token: 0x04001885 RID: 6277
			public Vector3 Velocity;

			// Token: 0x04001886 RID: 6278
			public float TimeToLive;

			// Token: 0x04001887 RID: 6279
			public float Duration;
		}
	}
}
