using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200028D RID: 653
	public class FireParticleSystem : ParticleSystem<FireParticleSystem.Particle>
	{
		// Token: 0x17000309 RID: 777
		// (get) Token: 0x0600148D RID: 5261 RVA: 0x00099BA0 File Offset: 0x00097DA0
		// (set) Token: 0x0600148E RID: 5262 RVA: 0x00099BA8 File Offset: 0x00097DA8
		public bool IsStopped { get; set; }

		// Token: 0x0600148F RID: 5263 RVA: 0x00099BB4 File Offset: 0x00097DB4
		public FireParticleSystem(Vector3 position, float size, float maxVisibilityDistance) : base(10)
		{
			this.m_position = position;
			this.m_size = size;
			this.m_maxVisibilityDistance = maxVisibilityDistance;
			base.Texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00099C04 File Offset: 0x00097E04
		public override bool Simulate(float dt)
		{
			this.m_age += dt;
			bool flag = false;
			if (this.m_visible || this.m_age < 2f)
			{
				this.m_toGenerate += (this.IsStopped ? 0f : (5f * dt));
				for (int i = 0; i < base.Particles.Length; i++)
				{
					FireParticleSystem.Particle particle = base.Particles[i];
					if (particle.IsActive)
					{
						flag = true;
						particle.Time += dt;
						particle.TimeToLive -= dt;
						if (particle.TimeToLive > 0f)
						{
							FireParticleSystem.Particle particle2 = particle;
							particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
							particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 8f);
						}
						else
						{
							particle.IsActive = false;
						}
					}
					else if (this.m_toGenerate >= 1f)
					{
						particle.IsActive = true;
						particle.Position = this.m_position + 0.25f * this.m_size * new Vector3(this.m_random.Float(-1f, 1f), 0f, this.m_random.Float(-1f, 1f));
						particle.Color = Color.White;
						particle.Size = new Vector2(this.m_size);
						particle.Speed = this.m_random.Float(0.45f, 0.55f) * this.m_size / 0.15f;
						particle.Time = 0f;
						particle.TimeToLive = this.m_random.Float(0.5f, 2f);
						particle.FlipX = (this.m_random.Int(0, 1) == 0);
						particle.FlipY = (this.m_random.Int(0, 1) == 0);
						this.m_toGenerate -= 1f;
					}
				}
				this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
			}
			this.m_visible = false;
			return this.IsStopped && !flag;
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00099E3C File Offset: 0x0009803C
		public override void Draw(Camera camera)
		{
			float num = Vector3.Dot(this.m_position - camera.ViewPosition, camera.ViewDirection);
			if (num > -0.5f && num <= this.m_maxVisibilityDistance && Vector3.DistanceSquared(this.m_position, camera.ViewPosition) <= this.m_maxVisibilityDistance * this.m_maxVisibilityDistance)
			{
				this.m_visible = true;
				base.Draw(camera);
			}
		}

		// Token: 0x04000D5B RID: 3419
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000D5C RID: 3420
		public Vector3 m_position;

		// Token: 0x04000D5D RID: 3421
		public float m_size;

		// Token: 0x04000D5E RID: 3422
		public float m_toGenerate;

		// Token: 0x04000D5F RID: 3423
		public bool m_visible;

		// Token: 0x04000D60 RID: 3424
		public float m_maxVisibilityDistance;

		// Token: 0x04000D61 RID: 3425
		public float m_age;

		// Token: 0x0200050D RID: 1293
		public class Particle : Game.Particle
		{
			// Token: 0x04001845 RID: 6213
			public float Time;

			// Token: 0x04001846 RID: 6214
			public float TimeToLive;

			// Token: 0x04001847 RID: 6215
			public float Speed;
		}
	}
}
