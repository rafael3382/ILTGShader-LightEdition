using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000288 RID: 648
	public class ExplosionParticleSystem : ParticleSystem<ExplosionParticleSystem.Particle>
	{
		// Token: 0x06001484 RID: 5252 RVA: 0x000996EC File Offset: 0x000978EC
		public ExplosionParticleSystem() : base(1000)
		{
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
			this.m_inactiveParticles.AddRange(base.Particles);
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00099750 File Offset: 0x00097950
		public void SetExplosionCell(Point3 point, float strength)
		{
			ExplosionParticleSystem.Particle particle;
			if (!this.m_particlesByPoint.TryGetValue(point, out particle))
			{
				if (this.m_inactiveParticles.Count > 0)
				{
					particle = this.m_inactiveParticles[this.m_inactiveParticles.Count - 1];
					this.m_inactiveParticles.RemoveAt(this.m_inactiveParticles.Count - 1);
				}
				else
				{
					for (int i = 0; i < 5; i++)
					{
						int num = this.m_random.Int(0, base.Particles.Length - 1);
						if (strength > base.Particles[num].Strength)
						{
							particle = base.Particles[num];
						}
					}
				}
				if (particle != null)
				{
					this.m_particlesByPoint.Add(point, particle);
				}
			}
			if (particle != null)
			{
				particle.IsActive = true;
				particle.Position = new Vector3((float)point.X, (float)point.Y, (float)point.Z) + new Vector3(0.5f) + 0.2f * new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f), this.m_random.Float(-1f, 1f));
				particle.Size = new Vector2(this.m_random.Float(0.6f, 0.9f));
				particle.Strength = strength;
				particle.Color = Color.White;
				this.m_isEmpty = false;
			}
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000998C8 File Offset: 0x00097AC8
		public override bool Simulate(float dt)
		{
			if (!this.m_isEmpty)
			{
				this.m_isEmpty = true;
				for (int i = 0; i < base.Particles.Length; i++)
				{
					ExplosionParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						this.m_isEmpty = false;
						particle.Strength -= dt / 1.5f;
						if (particle.Strength > 0f)
						{
							particle.TextureSlot = (int)MathUtils.Min(9f * (1f - particle.Strength) * 0.6f, 8f);
							ExplosionParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + 2f * MathUtils.Max(1f - particle.Strength - 0.25f, 0f) * dt;
						}
						else
						{
							particle.IsActive = false;
							this.m_inactiveParticles.Add(particle);
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000999AE File Offset: 0x00097BAE
		public override void Draw(Camera camera)
		{
			if (!this.m_isEmpty)
			{
				base.Draw(camera);
			}
		}

		// Token: 0x04000D44 RID: 3396
		public Dictionary<Point3, ExplosionParticleSystem.Particle> m_particlesByPoint = new Dictionary<Point3, ExplosionParticleSystem.Particle>();

		// Token: 0x04000D45 RID: 3397
		public List<ExplosionParticleSystem.Particle> m_inactiveParticles = new List<ExplosionParticleSystem.Particle>();

		// Token: 0x04000D46 RID: 3398
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D47 RID: 3399
		public const float m_duration = 1.5f;

		// Token: 0x04000D48 RID: 3400
		public bool m_isEmpty;

		// Token: 0x0200050C RID: 1292
		public class Particle : Game.Particle
		{
			// Token: 0x04001844 RID: 6212
			public float Strength;
		}
	}
}
