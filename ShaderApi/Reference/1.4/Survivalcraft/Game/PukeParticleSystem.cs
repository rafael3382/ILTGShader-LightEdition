using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002EF RID: 751
	public class PukeParticleSystem : ParticleSystem<PukeParticleSystem.Particle>
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x0600166A RID: 5738 RVA: 0x000A8CC3 File Offset: 0x000A6EC3
		// (set) Token: 0x0600166B RID: 5739 RVA: 0x000A8CCB File Offset: 0x000A6ECB
		public Vector3 Position { get; set; }

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x0600166C RID: 5740 RVA: 0x000A8CD4 File Offset: 0x000A6ED4
		// (set) Token: 0x0600166D RID: 5741 RVA: 0x000A8CDC File Offset: 0x000A6EDC
		public Vector3 Direction { get; set; }

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x000A8CE5 File Offset: 0x000A6EE5
		// (set) Token: 0x0600166F RID: 5743 RVA: 0x000A8CED File Offset: 0x000A6EED
		public bool IsStopped { get; set; }

		// Token: 0x06001670 RID: 5744 RVA: 0x000A8CF6 File Offset: 0x000A6EF6
		public PukeParticleSystem(SubsystemTerrain terrain) : base(80)
		{
			this.m_subsystemTerrain = terrain;
			base.Texture = ContentManager.Get<Texture2D>("Textures/PukeParticle", null);
			base.TextureSlotsCount = 3;
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x000A8D2C File Offset: 0x000A6F2C
		public override bool Simulate(float dt)
		{
			int num = Terrain.ToCell(this.Position.X);
			int num2 = Terrain.ToCell(this.Position.Y);
			int num3 = Terrain.ToCell(this.Position.Z);
			int num4 = 0;
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num + 1, num2, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num - 1, num2, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2 + 1, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2 - 1, num3));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2, num3 + 1));
			num4 = MathUtils.Max(num4, this.m_subsystemTerrain.Terrain.GetCellLight(num, num2, num3 - 1));
			Color c = Color.White;
			float s = LightingManager.LightIntensityByLightValue[num4];
			c *= s;
			c.A = byte.MaxValue;
			dt = MathUtils.Clamp(dt, 0f, 0.1f);
			float s2 = MathUtils.Pow(0.03f, dt);
			this.m_duration += dt;
			if (this.m_duration > 3.5f)
			{
				this.IsStopped = true;
			}
			float num5 = MathUtils.Saturate(1.3f * SimplexNoise.Noise(3f * this.m_duration + (float)(this.GetHashCode() % 100)) - 0.3f);
			float num6 = 30f * num5;
			this.m_toGenerate += num6 * dt;
			bool flag = false;
			for (int i = 0; i < base.Particles.Length; i++)
			{
				PukeParticleSystem.Particle particle = base.Particles[i];
				if (particle.IsActive)
				{
					flag = true;
					particle.TimeToLive -= dt;
					if (particle.TimeToLive > 0f)
					{
						Vector3 position = particle.Position;
						Vector3 vector = position + particle.Velocity * dt;
						TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(position, vector, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value));
						if (terrainRaycastResult != null)
						{
							Plane plane = terrainRaycastResult.Value.CellFace.CalculatePlane();
							vector = position;
							if (plane.Normal.X != 0f)
							{
								particle.Velocity *= new Vector3(-0.05f, 0.05f, 0.05f);
							}
							if (plane.Normal.Y != 0f)
							{
								particle.Velocity *= new Vector3(0.05f, -0.05f, 0.05f);
							}
							if (plane.Normal.Z != 0f)
							{
								particle.Velocity *= new Vector3(0.05f, 0.05f, -0.05f);
							}
						}
						particle.Position = vector;
						PukeParticleSystem.Particle particle2 = particle;
						particle2.Velocity.Y = particle2.Velocity.Y + -9.81f * dt;
						particle.Velocity *= s2;
						particle.Color *= MathUtils.Saturate(particle.TimeToLive);
						particle.TextureSlot = (int)(8.99f * MathUtils.Saturate(3f - particle.TimeToLive));
					}
					else
					{
						particle.IsActive = false;
					}
				}
				else if (!this.IsStopped && this.m_toGenerate >= 1f)
				{
					Vector3 v = this.m_random.Vector3(0f, 1f);
					particle.IsActive = true;
					particle.Position = this.Position + 0.05f * v;
					particle.Color = Color.MultiplyColorOnly(c, this.m_random.Float(0.7f, 1f));
					particle.Velocity = MathUtils.Lerp(1f, 2.5f, num5) * Vector3.Normalize(this.Direction + 0.25f * v);
					particle.TimeToLive = 3f;
					particle.Size = new Vector2(0.1f);
					particle.FlipX = this.m_random.Bool();
					particle.FlipY = this.m_random.Bool();
					this.m_toGenerate -= 1f;
				}
			}
			return this.IsStopped && !flag;
		}

		// Token: 0x04000F35 RID: 3893
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F36 RID: 3894
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000F37 RID: 3895
		public float m_duration;

		// Token: 0x04000F38 RID: 3896
		public float m_toGenerate;

		// Token: 0x02000530 RID: 1328
		public class Particle : Game.Particle
		{
			// Token: 0x040018C3 RID: 6339
			public Vector3 Velocity;

			// Token: 0x040018C4 RID: 6340
			public float TimeToLive;
		}
	}
}
