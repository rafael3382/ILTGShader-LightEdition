using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002E6 RID: 742
	public class PrecipitationShaftParticleSystem : ParticleSystemBase
	{
		// Token: 0x17000366 RID: 870
		// (get) Token: 0x0600163B RID: 5691 RVA: 0x000A70E4 File Offset: 0x000A52E4
		// (set) Token: 0x0600163C RID: 5692 RVA: 0x000A70EC File Offset: 0x000A52EC
		public Point2 Point { get; set; }

		// Token: 0x0600163D RID: 5693 RVA: 0x000A70F8 File Offset: 0x000A52F8
		public PrecipitationShaftParticleSystem(GameWidget gameWidget, SubsystemWeather subsystemWeather, Game.Random random, Point2 point, PrecipitationType precipitationType)
		{
			this.m_gameWidget = gameWidget;
			this.m_subsystemWeather = subsystemWeather;
			this.m_random = random;
			this.Point = point;
			this.m_precipitationType = precipitationType;
			for (int i = 0; i < this.m_particles.Length; i++)
			{
				this.m_particles[i] = new PrecipitationShaftParticleSystem.Particle();
			}
			this.Initialize();
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x000A717C File Offset: 0x000A537C
		public override bool Simulate(float dt)
		{
			if (this.m_subsystemWeather.SubsystemTime.GameTime - this.m_lastUpdateTime > 1.0 || MathUtils.Abs(this.m_lastSkylightIntensity - this.m_subsystemWeather.SubsystemSky.SkyLightIntensity) > 0.1f)
			{
				this.m_lastUpdateTime = this.m_subsystemWeather.SubsystemTime.GameTime;
				this.m_lastSkylightIntensity = this.m_subsystemWeather.SubsystemSky.SkyLightIntensity;
				PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(this.Point.X, this.Point.Y);
				this.m_intensity = precipitationShaftInfo.Intensity;
				this.m_yLimit = precipitationShaftInfo.YLimit;
				this.m_topmostValue = this.m_subsystemWeather.SubsystemTerrain.Terrain.GetCellValue(this.Point.X, precipitationShaftInfo.YLimit - 1, this.Point.Y);
				this.m_topmostBelowValue = this.m_subsystemWeather.SubsystemTerrain.Terrain.GetCellValue(this.Point.X, precipitationShaftInfo.YLimit - 2, this.Point.Y);
			}
			Camera activeCamera = this.m_gameWidget.ActiveCamera;
			if (!this.m_isEmpty || (this.m_intensity > 0f && (float)this.m_yLimit < activeCamera.ViewPosition.Y + 5f))
			{
				Vector2 vector = Vector2.Normalize(new Vector2(activeCamera.ViewDirection.X, activeCamera.ViewDirection.Z));
				Vector2 v = Vector2.Normalize(new Vector2((float)this.Point.X + 0.5f - activeCamera.ViewPosition.X + 0.7f * vector.X, (float)this.Point.Y + 0.5f - activeCamera.ViewPosition.Z + 0.7f * vector.Y));
				float num = Vector2.Dot(vector, v);
				this.m_isVisible = (num > 0.5f);
				if (this.m_isVisible)
				{
					if (this.m_needsInitialize)
					{
						this.m_needsInitialize = false;
						this.Initialize();
					}
					float y = activeCamera.ViewPosition.Y;
					float num2 = y - 5f;
					float num3 = y + 5f;
					float num4;
					float num5;
					if (this.m_lastViewY != null)
					{
						if (y < this.m_lastViewY.Value)
						{
							num4 = num2;
							num5 = this.m_lastViewY.Value - 5f;
						}
						else
						{
							num4 = this.m_lastViewY.Value + 5f;
							num5 = num3;
						}
					}
					else
					{
						num4 = num2;
						num5 = num3;
					}
					float num6 = (num5 - num4) / 10f * (float)this.m_particles.Length * this.m_intensity;
					int num7 = (int)num6 + ((this.m_random.Float(0f, 1f) < num6 - (float)((int)num6)) ? 1 : 0);
					this.m_lastViewY = new float?(y);
					this.m_toCreate += (float)this.m_particles.Length * this.m_intensity / 10f * this.m_averageSpeed * dt;
					this.m_isEmpty = true;
					float num8 = (this.m_precipitationType == PrecipitationType.Rain) ? 0f : 0.03f;
					for (int i = 0; i < this.m_particles.Length; i++)
					{
						PrecipitationShaftParticleSystem.Particle particle = this.m_particles[i];
						if (particle.IsActive)
						{
							if (particle.YLimit == 0f && particle.Position.Y <= (float)this.m_yLimit + num8)
							{
								this.RaycastParticle(particle);
							}
							bool flag = particle.YLimit != 0f && particle.Position.Y <= particle.YLimit + num8;
							if (!flag && particle.Position.Y >= num2 && particle.Position.Y <= num3)
							{
								PrecipitationShaftParticleSystem.Particle particle2 = particle;
								particle2.Position.Y = particle2.Position.Y - particle.Speed * dt;
								this.m_isEmpty = false;
							}
							else
							{
								particle.IsActive = false;
								if (particle.GenerateSplash && flag)
								{
									if (this.m_precipitationType == PrecipitationType.Rain && this.m_random.Bool(0.5f))
									{
										this.m_subsystemWeather.RainSplashParticleSystem.AddSplash(this.m_topmostValue, new Vector3(particle.Position.X, particle.YLimit + num8, particle.Position.Z), this.m_subsystemWeather.RainColor);
									}
									if (this.m_precipitationType == PrecipitationType.Snow)
									{
										this.m_subsystemWeather.SnowSplashParticleSystem.AddSplash(this.m_topmostValue, new Vector3(particle.Position.X, particle.YLimit + num8, particle.Position.Z), this.m_size, this.m_subsystemWeather.SnowColor, (int)particle.TextureSlot);
									}
								}
							}
						}
						else if (num7 > 0)
						{
							particle.Position.X = (float)this.Point.X + this.m_random.Float(0f, 1f);
							particle.Position.Y = this.m_random.Float(num4, num5);
							particle.Position.Z = (float)this.Point.Y + this.m_random.Float(0f, 1f);
							particle.IsActive = (particle.Position.Y >= (float)this.m_yLimit);
							particle.YLimit = 0f;
							num7--;
						}
						else if (this.m_toCreate >= 1f)
						{
							particle.Position.X = (float)this.Point.X + this.m_random.Float(0f, 1f);
							particle.Position.Y = this.m_random.Float(num3 - this.m_averageSpeed * dt, num3);
							particle.Position.Z = (float)this.Point.Y + this.m_random.Float(0f, 1f);
							particle.IsActive = (particle.Position.Y >= (float)this.m_yLimit);
							particle.YLimit = 0f;
							this.m_toCreate -= 1f;
						}
					}
					this.m_toCreate -= MathUtils.Floor(this.m_toCreate);
				}
				else
				{
					this.m_needsInitialize = true;
				}
			}
			return false;
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x000A7824 File Offset: 0x000A5A24
		public override void Draw(Camera camera)
		{
			if (!this.m_isVisible || this.m_isEmpty || camera.GameWidget != this.m_gameWidget)
			{
				return;
			}
			if (this.m_batch == null)
			{
				this.m_batch = this.SubsystemParticles.PrimitivesRenderer.TexturedBatch(this.m_texture, false, 0, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			}
			float num = camera.ViewPosition.Y + 5f;
			Vector3 viewDirection = camera.ViewDirection;
			Vector3 vector = Vector3.Normalize(Vector3.Cross(viewDirection, Vector3.UnitY));
			Vector3 v = (this.m_precipitationType == PrecipitationType.Rain) ? Vector3.UnitY : Vector3.Normalize(Vector3.Cross(viewDirection, vector));
			Vector3 vector2 = vector * this.m_size.X;
			Vector3 vector3 = v * this.m_size.Y;
			if (this.m_precipitationType == PrecipitationType.Rain)
			{
				Vector3 v2 = -vector2 - vector3;
				Vector3 v3 = vector2 - vector3;
				Vector3 v4 = vector3;
				for (int i = 0; i < this.m_particles.Length; i++)
				{
					PrecipitationShaftParticleSystem.Particle particle = this.m_particles[i];
					if (particle.IsActive)
					{
						Vector3 p = particle.Position + v2;
						Vector3 p2 = particle.Position + v3;
						Vector3 p3 = particle.Position + v4;
						Color color = this.m_subsystemWeather.RainColor * MathUtils.Min(0.6f * (num - particle.Position.Y), 1f);
						this.m_batch.QueueTriangle(p, p2, p3, particle.TexCoord1, particle.TexCoord2, particle.TexCoord3, color);
					}
				}
				return;
			}
			Vector3 v5 = -vector2 - vector3;
			Vector3 v6 = vector2 - vector3;
			Vector3 v7 = vector2 + vector3;
			Vector3 v8 = -vector2 + vector3;
			for (int j = 0; j < this.m_particles.Length; j++)
			{
				PrecipitationShaftParticleSystem.Particle particle2 = this.m_particles[j];
				if (particle2.IsActive)
				{
					Vector3 p4 = particle2.Position + v5;
					Vector3 p5 = particle2.Position + v6;
					Vector3 p6 = particle2.Position + v7;
					Vector3 p7 = particle2.Position + v8;
					Color color2 = this.m_subsystemWeather.SnowColor * MathUtils.Min(0.6f * (num - particle2.Position.Y), 1f);
					this.m_batch.QueueQuad(p4, p5, p6, p7, particle2.TexCoord1, particle2.TexCoord2, particle2.TexCoord3, particle2.TexCoord4, color2);
				}
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x000A7ADC File Offset: 0x000A5CDC
		public void RaycastParticle(PrecipitationShaftParticleSystem.Particle particle)
		{
			particle.YLimit = (float)this.m_yLimit;
			particle.GenerateSplash = true;
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.m_topmostValue)];
			if (!block.IsTransparent_(this.m_topmostValue))
			{
				return;
			}
			Ray3 ray = new Ray3(new Vector3(particle.Position.X - (float)this.Point.X, 1f, particle.Position.Z - (float)this.Point.Y), -Vector3.UnitY);
			int num2;
			BoundingBox boundingBox;
			float? num = block.Raycast(ray, this.m_subsystemWeather.SubsystemTerrain, this.m_topmostValue, false, out num2, out boundingBox);
			if (num != null)
			{
				particle.YLimit -= num.Value;
				return;
			}
			particle.YLimit -= 1f;
			if (BlocksManager.Blocks[Terrain.ExtractContents(this.m_topmostBelowValue)].IsFaceTransparent(this.m_subsystemWeather.SubsystemTerrain, 4, this.m_topmostBelowValue))
			{
				particle.GenerateSplash = false;
			}
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x000A7BE8 File Offset: 0x000A5DE8
		public void Initialize()
		{
			this.m_lastViewY = null;
			this.m_toCreate = this.m_random.Float(0f, 0.9f);
			this.m_batch = null;
			this.m_lastSkylightIntensity = float.MinValue;
			PrecipitationType precipitationType = this.m_precipitationType;
			if (precipitationType == PrecipitationType.Rain)
			{
				float num = 8f;
				float num2 = 12f;
				this.m_averageSpeed = (num + num2) / 2f;
				this.m_size = new Vector2(0.02f, 0.15f);
				this.m_texture = ContentManager.Get<Texture2D>("Textures/RainParticle", null);
				for (int i = 0; i < this.m_particles.Length; i++)
				{
					PrecipitationShaftParticleSystem.Particle particle = this.m_particles[i];
					particle.IsActive = false;
					particle.TexCoord1 = new Vector2(0f, 1f);
					particle.TexCoord2 = new Vector2(1f, 1f);
					particle.TexCoord3 = new Vector2(0.5f, 0f);
					particle.Speed = this.m_random.Float(num, num2);
				}
				return;
			}
			if (precipitationType != PrecipitationType.Snow)
			{
				throw new InvalidOperationException("Unknown precipitation type.");
			}
			float num3 = 0.25f;
			float num4 = 0.5f;
			float num5 = 3f;
			this.m_averageSpeed = (num4 + num5) / 2f;
			this.m_size = new Vector2(0.07f, 0.07f);
			this.m_texture = ContentManager.Get<Texture2D>("Textures/SnowParticle", null);
			for (int j = 0; j < this.m_particles.Length; j++)
			{
				PrecipitationShaftParticleSystem.Particle particle2 = this.m_particles[j];
				particle2.IsActive = false;
				particle2.TextureSlot = (byte)this.m_random.Int(0, 15);
				Vector2 v = new Vector2((float)(particle2.TextureSlot % 4), (float)(particle2.TextureSlot / 4)) * num3;
				particle2.TexCoord1 = v + new Vector2(0f, 0f);
				particle2.TexCoord2 = v + new Vector2(num3, 0f);
				particle2.TexCoord3 = v + new Vector2(num3, num3);
				particle2.TexCoord4 = v + new Vector2(0f, num3);
				particle2.Speed = this.m_random.Float(num4, num5);
			}
		}

		// Token: 0x04000EF2 RID: 3826
		public const float m_viewHeight = 10f;

		// Token: 0x04000EF3 RID: 3827
		public const int m_particlesCount = 4;

		// Token: 0x04000EF4 RID: 3828
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000EF5 RID: 3829
		public GameWidget m_gameWidget;

		// Token: 0x04000EF6 RID: 3830
		public Game.Random m_random;

		// Token: 0x04000EF7 RID: 3831
		public TexturedBatch3D m_batch;

		// Token: 0x04000EF8 RID: 3832
		public PrecipitationShaftParticleSystem.Particle[] m_particles = new PrecipitationShaftParticleSystem.Particle[4];

		// Token: 0x04000EF9 RID: 3833
		public PrecipitationType m_precipitationType;

		// Token: 0x04000EFA RID: 3834
		public bool m_isVisible;

		// Token: 0x04000EFB RID: 3835
		public bool m_isEmpty;

		// Token: 0x04000EFC RID: 3836
		public float? m_lastViewY;

		// Token: 0x04000EFD RID: 3837
		public float m_toCreate;

		// Token: 0x04000EFE RID: 3838
		public float m_averageSpeed;

		// Token: 0x04000EFF RID: 3839
		public Texture2D m_texture;

		// Token: 0x04000F00 RID: 3840
		public Vector2 m_size;

		// Token: 0x04000F01 RID: 3841
		public float m_intensity;

		// Token: 0x04000F02 RID: 3842
		public int m_yLimit;

		// Token: 0x04000F03 RID: 3843
		public int m_topmostValue;

		// Token: 0x04000F04 RID: 3844
		public int m_topmostBelowValue;

		// Token: 0x04000F05 RID: 3845
		public double m_lastUpdateTime = double.MinValue;

		// Token: 0x04000F06 RID: 3846
		public float m_lastSkylightIntensity = float.MinValue;

		// Token: 0x04000F07 RID: 3847
		public bool m_needsInitialize;

		// Token: 0x0200052B RID: 1323
		public class Particle
		{
			// Token: 0x040018AD RID: 6317
			public bool IsActive;

			// Token: 0x040018AE RID: 6318
			public bool GenerateSplash;

			// Token: 0x040018AF RID: 6319
			public byte TextureSlot;

			// Token: 0x040018B0 RID: 6320
			public Vector3 Position;

			// Token: 0x040018B1 RID: 6321
			public Vector2 TexCoord1;

			// Token: 0x040018B2 RID: 6322
			public Vector2 TexCoord2;

			// Token: 0x040018B3 RID: 6323
			public Vector2 TexCoord3;

			// Token: 0x040018B4 RID: 6324
			public Vector2 TexCoord4;

			// Token: 0x040018B5 RID: 6325
			public float Speed;

			// Token: 0x040018B6 RID: 6326
			public float YLimit;
		}
	}
}
