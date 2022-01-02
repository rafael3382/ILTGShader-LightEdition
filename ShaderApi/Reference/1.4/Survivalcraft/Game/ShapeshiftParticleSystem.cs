using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000301 RID: 769
	public class ShapeshiftParticleSystem : ParticleSystem<ShapeshiftParticleSystem.Particle>
	{
		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x000AB188 File Offset: 0x000A9388
		// (set) Token: 0x060016B7 RID: 5815 RVA: 0x000AB190 File Offset: 0x000A9390
		public bool Stopped { get; set; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060016B8 RID: 5816 RVA: 0x000AB199 File Offset: 0x000A9399
		// (set) Token: 0x060016B9 RID: 5817 RVA: 0x000AB1A1 File Offset: 0x000A93A1
		public Vector3 Position { get; set; }

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060016BA RID: 5818 RVA: 0x000AB1AA File Offset: 0x000A93AA
		// (set) Token: 0x060016BB RID: 5819 RVA: 0x000AB1B2 File Offset: 0x000A93B2
		public BoundingBox BoundingBox { get; set; }

		// Token: 0x060016BC RID: 5820 RVA: 0x000AB1BB File Offset: 0x000A93BB
		public ShapeshiftParticleSystem() : base(40)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/ShapeshiftParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x000AB1E8 File Offset: 0x000A93E8
		public override bool Simulate(float dt)
		{
			bool flag = false;
			this.m_generationSpeed = MathUtils.Min(this.m_generationSpeed + 15f * dt, 35f);
			this.m_toGenerate += this.m_generationSpeed * dt;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				ShapeshiftParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.Time += dt;
					if (particle.Time <= particle.Duration)
					{
						particle.Position += particle.Velocity * dt;
						particle.FlipX = this.m_random.Bool();
						particle.FlipY = this.m_random.Bool();
						particle.TextureSlot = (int)MathUtils.Min(9.900001f * particle.Time / particle.Duration, 8f);
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.Stopped)
				{
					while (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position.X = this.m_random.Float(this.BoundingBox.Min.X, this.BoundingBox.Max.X);
						particle.Position.Y = this.m_random.Float(this.BoundingBox.Min.Y, this.BoundingBox.Max.Y);
						particle.Position.Z = this.m_random.Float(this.BoundingBox.Min.Z, this.BoundingBox.Max.Z);
						particle.Velocity = new Vector3(0f, this.m_random.Float(0.5f, 1.5f), 0f);
						particle.Color = Color.White;
						particle.Size = new Vector2(0.4f);
						particle.Time = 0f;
						particle.Duration = this.m_random.Float(0.75f, 1.5f);
						this.m_toGenerate -= 1f;
					}
				}
				else
				{
					this.m_toGenerate = 0f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			return this.Stopped && !flag;
		}

		// Token: 0x04000F75 RID: 3957
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F76 RID: 3958
		public float m_generationSpeed;

		// Token: 0x04000F77 RID: 3959
		public float m_toGenerate;

		// Token: 0x0200053B RID: 1339
		public class Particle : Game.Particle
		{
			// Token: 0x040018DC RID: 6364
			public float Time;

			// Token: 0x040018DD RID: 6365
			public float Duration;

			// Token: 0x040018DE RID: 6366
			public Vector3 Velocity;
		}
	}
}
