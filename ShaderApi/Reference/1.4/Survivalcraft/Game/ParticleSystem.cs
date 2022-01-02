using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002D8 RID: 728
	public class ParticleSystem<T> : ParticleSystemBase where T : Particle, new()
	{
		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060015E1 RID: 5601 RVA: 0x000A4E59 File Offset: 0x000A3059
		public T[] Particles
		{
			get
			{
				return this.m_particles;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x000A4E61 File Offset: 0x000A3061
		// (set) Token: 0x060015E3 RID: 5603 RVA: 0x000A4E69 File Offset: 0x000A3069
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
			set
			{
				if (value != this.m_texture)
				{
					this.m_texture = value;
					this.AdditiveBatch = null;
					this.AlphaBlendedBatch = null;
				}
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x000A4E89 File Offset: 0x000A3089
		// (set) Token: 0x060015E5 RID: 5605 RVA: 0x000A4E91 File Offset: 0x000A3091
		public int TextureSlotsCount { get; set; }

		// Token: 0x060015E6 RID: 5606 RVA: 0x000A4E9C File Offset: 0x000A309C
		public ParticleSystem(int particlesCount)
		{
			this.m_particles = new T[particlesCount];
			for (int i = 0; i < this.m_particles.Length; i++)
			{
				this.m_particles[i] = Activator.CreateInstance<T>();
			}
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x000A4F04 File Offset: 0x000A3104
		public override void Draw(Camera camera)
		{
			if (this.AdditiveBatch == null || this.AlphaBlendedBatch == null)
			{
				this.AdditiveBatch = this.SubsystemParticles.PrimitivesRenderer.TexturedBatch(this.m_texture, true, 0, DepthStencilState.DepthRead, null, BlendState.Additive, SamplerState.PointClamp);
				this.AlphaBlendedBatch = this.SubsystemParticles.PrimitivesRenderer.TexturedBatch(this.m_texture, true, 0, DepthStencilState.Default, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			}
			this.m_front[0] = camera.ViewDirection;
			this.m_right[0] = Vector3.Normalize(Vector3.Cross(this.m_front[0], Vector3.UnitY));
			this.m_up[0] = Vector3.Normalize(Vector3.Cross(this.m_right[0], this.m_front[0]));
			this.m_front[1] = camera.ViewDirection;
			this.m_right[1] = Vector3.Normalize(Vector3.Cross(this.m_front[1], Vector3.UnitY));
			this.m_up[1] = Vector3.UnitY;
			this.m_front[2] = Vector3.UnitY;
			this.m_right[2] = Vector3.UnitX;
			this.m_up[2] = Vector3.UnitZ;
			float s = 1f / (float)this.TextureSlotsCount;
			for (int i = 0; i < this.m_particles.Length; i++)
			{
				Particle particle = this.m_particles[i];
				if (particle.IsActive)
				{
					Vector3 position = particle.Position;
					Vector2 size = particle.Size;
					float rotation = particle.Rotation;
					int textureSlot = particle.TextureSlot;
					int billboardingMode = (int)particle.BillboardingMode;
					Vector3 p;
					Vector3 p2;
					Vector3 p3;
					Vector3 p4;
					if (rotation != 0f)
					{
						Vector3 vector = (this.m_front[billboardingMode].X * this.m_front[billboardingMode].X > this.m_front[billboardingMode].Z * this.m_front[billboardingMode].Z) ? new Vector3(0f, MathUtils.Cos(rotation), MathUtils.Sin(rotation)) : new Vector3(MathUtils.Sin(rotation), MathUtils.Cos(rotation), 0f);
						Vector3 vector2 = Vector3.Normalize(Vector3.Cross(this.m_front[(int)particle.BillboardingMode], vector));
						vector = Vector3.Normalize(Vector3.Cross(this.m_front[(int)particle.BillboardingMode], vector2));
						vector2 *= size.Y;
						vector *= size.X;
						p = position + (-vector2 - vector);
						p2 = position + (vector2 - vector);
						p3 = position + (vector2 + vector);
						p4 = position + (-vector2 + vector);
					}
					else
					{
						Vector3 vector3 = this.m_right[billboardingMode] * size.X;
						Vector3 v = this.m_up[billboardingMode] * size.Y;
						p = position + (-vector3 - v);
						p2 = position + (vector3 - v);
						p3 = position + (vector3 + v);
						p4 = position + (-vector3 + v);
					}
					TexturedBatch3D texturedBatch3D = particle.UseAdditiveBlending ? this.AdditiveBatch : this.AlphaBlendedBatch;
					Vector2 v2 = new Vector2((float)(textureSlot % this.TextureSlotsCount), (float)(textureSlot / this.TextureSlotsCount));
					float num = 0f;
					float num2 = 1f;
					float num3 = 1f;
					float num4 = 0f;
					if (particle.FlipX)
					{
						num = 1f - num;
						num2 = 1f - num2;
					}
					if (particle.FlipY)
					{
						num3 = 1f - num3;
						num4 = 1f - num4;
					}
					Vector2 texCoord = (v2 + new Vector2(num, num3)) * s;
					Vector2 texCoord2 = (v2 + new Vector2(num2, num3)) * s;
					Vector2 texCoord3 = (v2 + new Vector2(num2, num4)) * s;
					Vector2 texCoord4 = (v2 + new Vector2(num, num4)) * s;
					texturedBatch3D.QueueQuad(p, p2, p3, p4, texCoord, texCoord2, texCoord3, texCoord4, particle.Color);
				}
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x000A5386 File Offset: 0x000A3586
		public override bool Simulate(float dt)
		{
			return false;
		}

		// Token: 0x04000E71 RID: 3697
		public T[] m_particles;

		// Token: 0x04000E72 RID: 3698
		public Texture2D m_texture;

		// Token: 0x04000E73 RID: 3699
		public Vector3[] m_front = new Vector3[3];

		// Token: 0x04000E74 RID: 3700
		public Vector3[] m_right = new Vector3[3];

		// Token: 0x04000E75 RID: 3701
		public Vector3[] m_up = new Vector3[3];

		// Token: 0x04000E76 RID: 3702
		public TexturedBatch3D AdditiveBatch;

		// Token: 0x04000E77 RID: 3703
		public TexturedBatch3D AlphaBlendedBatch;
	}
}
