using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002D2 RID: 722
	public class OnFireParticleSystem : ParticleSystem<OnFireParticleSystem.Particle>
	{
		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x000A429C File Offset: 0x000A249C
		// (set) Token: 0x060015CE RID: 5582 RVA: 0x000A42A4 File Offset: 0x000A24A4
		public bool IsStopped { get; set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x000A42AD File Offset: 0x000A24AD
		// (set) Token: 0x060015D0 RID: 5584 RVA: 0x000A42B5 File Offset: 0x000A24B5
		public Vector3 Position { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x000A42BE File Offset: 0x000A24BE
		// (set) Token: 0x060015D2 RID: 5586 RVA: 0x000A42C6 File Offset: 0x000A24C6
		public float Radius { get; set; }

		// Token: 0x060015D3 RID: 5587 RVA: 0x000A42CF File Offset: 0x000A24CF
		public OnFireParticleSystem() : base(25)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000A42FC File Offset: 0x000A24FC
		public override bool Simulate(float dt)
		{
			bool flag = false;
			if (this.m_visible)
			{
				this.m_toGenerate += 20f * dt;
				float s = MathUtils.Pow(0.02f, dt);
				for (int i = 0; i < base.Particles.Length; i++)
				{
					OnFireParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						if (particle.Time <= particle.Duration)
						{
							particle.Position += particle.Velocity * dt;
							particle.Velocity *= s;
							OnFireParticleSystem.Particle particle2 = particle;
							particle2.Velocity.Y = particle2.Velocity.Y + 10f * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / particle.Duration * 1.2f, 8f);
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (!this.IsStopped)
					{
						if (this.m_toGenerate >= 1f)
						{
							particle.IsActive = true;
							Vector3 v = new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(0f, 1f), this.m_random.Float(-1f, 1f));
							particle.Position = this.Position + 0.75f * this.Radius * v;
							particle.Color = Color.White;
							particle.Velocity = 1.5f * v;
							particle.Size = new Vector2(0.5f);
							particle.Time = 0f;
							particle.Duration = this.m_random.Float(0.5f, 1.5f);
							particle.FlipX = this.m_random.Bool();
							particle.FlipY = this.m_random.Bool();
							this.m_toGenerate -= 1f;
						}
					}
					else
					{
						this.m_toGenerate = 0f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
				this.m_visible = false;
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x000A454C File Offset: 0x000A274C
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.Position - camera.ViewPosition, camera.ViewDirection);
			if (num > -5f && num <= 48f)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000E56 RID: 3670
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000E57 RID: 3671
		public float m_toGenerate;

		// Token: 0x04000E58 RID: 3672
		public bool m_visible;

		// Token: 0x02000520 RID: 1312
		public class Particle : Game.Particle
		{
			// Token: 0x0400188D RID: 6285
			public float Time;

			// Token: 0x0400188E RID: 6286
			public float Duration;

			// Token: 0x0400188F RID: 6287
			public Vector3 Velocity;
		}
	}
}
