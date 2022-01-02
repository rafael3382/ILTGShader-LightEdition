using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000176 RID: 374
	public class ScreenSpaceFireRenderer
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000879 RID: 2169 RVA: 0x00033AC0 File Offset: 0x00031CC0
		// (set) Token: 0x0600087A RID: 2170 RVA: 0x00033AC8 File Offset: 0x00031CC8
		public float ParticlesPerSecond { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600087B RID: 2171 RVA: 0x00033AD1 File Offset: 0x00031CD1
		// (set) Token: 0x0600087C RID: 2172 RVA: 0x00033AD9 File Offset: 0x00031CD9
		public float ParticleSpeed { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x00033AE2 File Offset: 0x00031CE2
		// (set) Token: 0x0600087E RID: 2174 RVA: 0x00033AEA File Offset: 0x00031CEA
		public float MinTimeToLive { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600087F RID: 2175 RVA: 0x00033AF3 File Offset: 0x00031CF3
		// (set) Token: 0x06000880 RID: 2176 RVA: 0x00033AFB File Offset: 0x00031CFB
		public float MaxTimeToLive { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x00033B04 File Offset: 0x00031D04
		// (set) Token: 0x06000882 RID: 2178 RVA: 0x00033B0C File Offset: 0x00031D0C
		public float ParticleSize { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000883 RID: 2179 RVA: 0x00033B15 File Offset: 0x00031D15
		// (set) Token: 0x06000884 RID: 2180 RVA: 0x00033B1D File Offset: 0x00031D1D
		public float ParticleAnimationPeriod { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000885 RID: 2181 RVA: 0x00033B26 File Offset: 0x00031D26
		// (set) Token: 0x06000886 RID: 2182 RVA: 0x00033B2E File Offset: 0x00031D2E
		public float ParticleAnimationOffset { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000887 RID: 2183 RVA: 0x00033B37 File Offset: 0x00031D37
		// (set) Token: 0x06000888 RID: 2184 RVA: 0x00033B3F File Offset: 0x00031D3F
		public Vector2 Origin { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000889 RID: 2185 RVA: 0x00033B48 File Offset: 0x00031D48
		// (set) Token: 0x0600088A RID: 2186 RVA: 0x00033B50 File Offset: 0x00031D50
		public float Width { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600088B RID: 2187 RVA: 0x00033B59 File Offset: 0x00031D59
		// (set) Token: 0x0600088C RID: 2188 RVA: 0x00033B61 File Offset: 0x00031D61
		public float CutoffPosition { get; set; }

		// Token: 0x0600088D RID: 2189 RVA: 0x00033B6C File Offset: 0x00031D6C
		public ScreenSpaceFireRenderer(int particlesCount)
		{
			this.m_texture = ContentManager.Get<Texture2D>("Textures/FireParticle", null);
			for (int i = 0; i < particlesCount; i++)
			{
				this.m_particles.Add(new ScreenSpaceFireRenderer.Particle());
			}
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x00033BC4 File Offset: 0x00031DC4
		public void Update(float dt)
		{
			this.m_toGenerate += this.ParticlesPerSecond * dt;
			foreach (ScreenSpaceFireRenderer.Particle particle in this.m_particles)
			{
				if (particle.Active)
				{
					ScreenSpaceFireRenderer.Particle particle2 = particle;
					particle2.Position.Y = particle2.Position.Y + particle.Speed * dt;
					particle.AnimationTime += dt;
					particle.TimeToLive -= dt;
					particle.TextureSlot = (int)MathUtils.Max(9f * particle.AnimationTime / this.ParticleAnimationPeriod, 0f);
					if (particle.TimeToLive <= 0f || particle.TextureSlot > 8 || particle.Position.Y < this.CutoffPosition)
					{
						particle.Active = false;
					}
				}
				else if (this.m_toGenerate >= 1f)
				{
					particle.Active = true;
					particle.Position = new Vector2(this.m_random.Float(this.Origin.X, this.Origin.X + this.Width), this.Origin.Y);
					particle.Size = new Vector2(this.ParticleSize);
					particle.Speed = (0f - this.m_random.Float(0.75f, 1.25f)) * this.ParticleSpeed;
					particle.AnimationTime = this.m_random.Float(0f, this.ParticleAnimationOffset);
					particle.TimeToLive = MathUtils.Lerp(this.MinTimeToLive, this.MaxTimeToLive, this.m_random.Float(0f, 1f));
					particle.FlipX = (this.m_random.Int(0, 1) == 0);
					particle.FlipY = (this.m_random.Int(0, 1) == 0);
					this.m_toGenerate -= 1f;
				}
			}
			this.m_toGenerate = MathUtils.Remainder(this.m_toGenerate, 1f);
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00033DFC File Offset: 0x00031FFC
		public void Draw(PrimitivesRenderer2D primitivesRenderer, float depth, Matrix matrix, Color color)
		{
			TexturedBatch2D texturedBatch2D = primitivesRenderer.TexturedBatch(this.m_texture, false, 0, DepthStencilState.None, null, null, SamplerState.PointClamp);
			int count = texturedBatch2D.TriangleVertices.Count;
			foreach (ScreenSpaceFireRenderer.Particle particle in this.m_particles)
			{
				if (particle.Active)
				{
					this.DrawParticle(texturedBatch2D, particle, depth, color);
				}
			}
			texturedBatch2D.TransformTriangles(matrix, count, -1);
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x00033E8C File Offset: 0x0003208C
		public void DrawParticle(TexturedBatch2D batch, ScreenSpaceFireRenderer.Particle particle, float depth, Color color)
		{
			Vector2 corner = particle.Position - particle.Size / 2f;
			Vector2 corner2 = particle.Position + particle.Size / 2f;
			int textureSlot = particle.TextureSlot;
			Vector2 v = new Vector2((float)(textureSlot % 3), (float)(textureSlot / 3));
			float num = 0f;
			float num2 = 1f;
			float num3 = 0f;
			float num4 = 1f;
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
			Vector2 texCoord = (v + new Vector2(num, num3)) * 0.333333343f;
			Vector2 texCoord2 = (v + new Vector2(num2, num4)) * 0.333333343f;
			batch.QueueQuad(corner, corner2, depth, texCoord, texCoord2, color);
		}

		// Token: 0x040003EB RID: 1003
		public List<ScreenSpaceFireRenderer.Particle> m_particles = new List<ScreenSpaceFireRenderer.Particle>();

		// Token: 0x040003EC RID: 1004
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003ED RID: 1005
		public float m_toGenerate;

		// Token: 0x040003EE RID: 1006
		public Texture2D m_texture;

		// Token: 0x02000474 RID: 1140
		public class Particle
		{
			// Token: 0x0400164A RID: 5706
			public bool Active;

			// Token: 0x0400164B RID: 5707
			public Vector2 Position;

			// Token: 0x0400164C RID: 5708
			public Vector2 Size;

			// Token: 0x0400164D RID: 5709
			public float Speed;

			// Token: 0x0400164E RID: 5710
			public int TextureSlot;

			// Token: 0x0400164F RID: 5711
			public bool FlipX;

			// Token: 0x04001650 RID: 5712
			public bool FlipY;

			// Token: 0x04001651 RID: 5713
			public float AnimationTime;

			// Token: 0x04001652 RID: 5714
			public float TimeToLive;
		}
	}
}
