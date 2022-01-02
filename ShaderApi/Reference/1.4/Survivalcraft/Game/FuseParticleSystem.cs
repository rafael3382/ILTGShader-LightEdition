using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200029D RID: 669
	public class FuseParticleSystem : ParticleSystem<FuseParticleSystem.Particle>
	{
		// Token: 0x06001507 RID: 5383 RVA: 0x0009EEB0 File Offset: 0x0009D0B0
		public FuseParticleSystem(Vector3 position) : base(15)
		{
			this.m_position = position;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x0009EEE4 File Offset: 0x0009D0E4
		public override bool Simulate(float dt)
		{
			if (this.m_visible)
			{
				this.m_toGenerate += 15f * dt;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					FuseParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						particle.Time += dt;
						particle.TimeToLive -= dt;
						if (particle.TimeToLive > 0f)
						{
							FuseParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
							particle.Speed = MathUtils.Max(particle.Speed - 1.5f * dt, particle.TargetSpeed);
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 0.75f, 8f);
							particle.Size = new Vector2(0.07f * (1f + 2f * particle.Time));
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = this.m_position + 0.02f * new Vector3(0f, this.m_random.Float(-1f, 1f), 0f);
						particle.Color = Color.White;
						particle.TargetSpeed = this.m_random.Float(0.45f, 0.55f) * 0.4f;
						particle.Speed = this.m_random.Float(0.45f, 0.55f) * 2.5f;
						particle.Time = 0f;
						particle.Size = Vector2.Zero;
						particle.TimeToLive = this.m_random.Float(0.3f, 1f);
						particle.FlipX = (this.m_random.Int(0, 1) == 0);
						particle.FlipY = (this.m_random.Int(0, 1) == 0);
						this.m_toGenerate -= 1f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			}
			this.m_visible = false;
			return false;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x0009F128 File Offset: 0x0009D328
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.m_position - camera.ViewPosition, camera.ViewDirection);
			if (num > -0.5f && num <= 32f && Vector3.DistanceSquared(this.m_position, camera.ViewPosition) <= 1024f)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000DB4 RID: 3508
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000DB5 RID: 3509
		public Vector3 m_position;

		// Token: 0x04000DB6 RID: 3510
		public float m_toGenerate;

		// Token: 0x04000DB7 RID: 3511
		public bool m_visible;

		// Token: 0x02000516 RID: 1302
		public class Particle : Game.Particle
		{
			// Token: 0x0400186C RID: 6252
			public float Time;

			// Token: 0x0400186D RID: 6253
			public float TimeToLive;

			// Token: 0x0400186E RID: 6254
			public float Speed;

			// Token: 0x0400186F RID: 6255
			public float TargetSpeed;
		}
	}
}
