using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D1 RID: 465
	public class SubsystemSky : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00057E3B File Offset: 0x0005603B
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x00057E43 File Offset: 0x00056043
		public float SkyLightIntensity { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00057E4C File Offset: 0x0005604C
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x00057E54 File Offset: 0x00056054
		public int MoonPhase { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x00057E5D File Offset: 0x0005605D
		// (set) Token: 0x06000C63 RID: 3171 RVA: 0x00057E65 File Offset: 0x00056065
		public int SkyLightValue { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x00057E6E File Offset: 0x0005606E
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x00057E76 File Offset: 0x00056076
		public float VisibilityRange
		{
			get
			{
				return this.m_visibilityRange;
			}
			set
			{
				this.m_visibilityRange = value;
				this.VisibilityRangeSqr = MathUtils.Sqr(value);
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x00057E8B File Offset: 0x0005608B
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x00057E93 File Offset: 0x00056093
		public float VisibilityRangeSqr { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x00057E9C File Offset: 0x0005609C
		// (set) Token: 0x06000C69 RID: 3177 RVA: 0x00057EA4 File Offset: 0x000560A4
		public float VisibilityRangeYMultiplier { get; set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x00057EAD File Offset: 0x000560AD
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x00057EB5 File Offset: 0x000560B5
		public float ViewUnderWaterDepth { get; set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x00057EBE File Offset: 0x000560BE
		// (set) Token: 0x06000C6D RID: 3181 RVA: 0x00057EC6 File Offset: 0x000560C6
		public float ViewUnderMagmaDepth { get; set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x00057ECF File Offset: 0x000560CF
		public Color ViewFogColor
		{
			get
			{
				return this.m_viewFogColor;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000C6F RID: 3183 RVA: 0x00057ED7 File Offset: 0x000560D7
		public Vector2 ViewFogRange
		{
			get
			{
				return this.m_viewFogRange;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00057EDF File Offset: 0x000560DF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000C71 RID: 3185 RVA: 0x00057EE2 File Offset: 0x000560E2
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x00057EEC File Offset: 0x000560EC
		public void MakeLightningStrike(Vector3 targetPosition)
		{
			if (this.m_lightningStrikePosition != null || this.m_subsystemTime.GameTime - this.m_lastLightningStrikeTime <= 1.0)
			{
				return;
			}
			this.m_lastLightningStrikeTime = this.m_subsystemTime.GameTime;
			this.m_lightningStrikePosition = new Vector3?(targetPosition);
			this.m_lightningStrikeBrightness = 1f;
			float num = float.MaxValue;
			foreach (Vector3 vector in this.m_subsystemAudio.ListenerPositions)
			{
				float num2 = Vector2.Distance(new Vector2(vector.X, vector.Z), new Vector2(targetPosition.X, targetPosition.Z));
				if (num2 < num)
				{
					num = num2;
				}
			}
			float delay = this.m_subsystemAudio.CalculateDelay(num);
			if (num < 40f)
			{
				this.m_subsystemAudio.PlayRandomSound("Audio/ThunderNear", 1f, this.m_random.Float(-0.2f, 0.2f), 0f, delay);
			}
			else if (num < 200f)
			{
				this.m_subsystemAudio.PlayRandomSound("Audio/ThunderFar", 0.8f, this.m_random.Float(-0.2f, 0.2f), 0f, delay);
			}
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			DynamicArray<ComponentBody> dynamicArray = new DynamicArray<ComponentBody>();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(targetPosition.X, targetPosition.Z), 4f, dynamicArray);
			for (int i = 0; i < dynamicArray.Count; i++)
			{
				ComponentBody componentBody = dynamicArray.Array[i];
				if (componentBody.Position.Y > targetPosition.Y - 1.5f && Vector2.Distance(new Vector2(componentBody.Position.X, componentBody.Position.Z), new Vector2(targetPosition.X, targetPosition.Z)) < 4f)
				{
					ComponentOnFire componentOnFire = componentBody.Entity.FindComponent<ComponentOnFire>();
					if (componentOnFire != null)
					{
						componentOnFire.SetOnFire(null, this.m_random.Float(12f, 15f));
					}
				}
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && componentCreature.PlayerStats != null)
				{
					componentCreature.PlayerStats.StruckByLightning += 1L;
				}
			}
			int x = Terrain.ToCell(targetPosition.X);
			int num3 = Terrain.ToCell(targetPosition.Y);
			int z = Terrain.ToCell(targetPosition.Z);
			float pressure = (float)((this.m_random.Float(0f, 1f) < 0.2f) ? 39 : 19);
			base.Project.FindSubsystem<SubsystemExplosions>(true).AddExplosion(x, num3 + 1, z, pressure, false, true);
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000581C8 File Offset: 0x000563C8
		public void Update(float dt)
		{
			this.MoonPhase = ((int)MathUtils.Floor(this.m_subsystemTimeOfDay.Day - 0.5 + 5.0) % 8 + 8) % 8;
			this.UpdateLightAndViewParameters();
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00058204 File Offset: 0x00056404
		public void Draw(Camera camera, int drawOrder)
		{
			if (drawOrder == this.m_drawOrders[0])
			{
				this.ViewUnderWaterDepth = 0f;
				this.ViewUnderMagmaDepth = 0f;
				Vector3 viewPosition = camera.ViewPosition;
				int x = Terrain.ToCell(viewPosition.X);
				int y = Terrain.ToCell(viewPosition.Y);
				int z = Terrain.ToCell(viewPosition.Z);
				FluidBlock fluidBlock;
				float? surfaceHeight = this.m_subsystemFluidBlockBehavior.GetSurfaceHeight(x, y, z, out fluidBlock);
				if (surfaceHeight != null)
				{
					if (fluidBlock is WaterBlock)
					{
						this.ViewUnderWaterDepth = surfaceHeight.Value + 0.1f - viewPosition.Y;
					}
					else if (fluidBlock is MagmaBlock)
					{
						this.ViewUnderMagmaDepth = surfaceHeight.Value + 1f - viewPosition.Y;
					}
				}
				if (this.ViewUnderWaterDepth > 0f)
				{
					int seasonalHumidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
					int temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
					Color c = BlockColorsMap.WaterColorsMap.Lookup(temperature, seasonalHumidity);
					float num = MathUtils.Lerp(1f, 0.5f, (float)seasonalHumidity / 15f);
					float num2 = MathUtils.Lerp(1f, 0.2f, MathUtils.Saturate(0.075f * (this.ViewUnderWaterDepth - 2f)));
					float num3 = MathUtils.Lerp(20f, 1f, this.SkyLightIntensity);
					this.m_viewFogRange.X = 0f;
					this.m_viewFogRange.Y = MathUtils.Lerp(4f, 10f, num * num2 * num3);
					this.m_viewFogColor = Color.MultiplyColorOnly(c, 0.66f * num2 * num3);
					this.VisibilityRangeYMultiplier = 1f;
					this.m_viewIsSkyVisible = true;
				}
				else if (this.ViewUnderMagmaDepth > 0f)
				{
					this.m_viewFogRange.X = 0f;
					this.m_viewFogRange.Y = 0.1f;
					this.m_viewFogColor = new Color(255, 80, 0);
					this.VisibilityRangeYMultiplier = 1f;
					this.m_viewIsSkyVisible = false;
				}
				else
				{
					float num4 = 1024f;
					float num5 = 128f;
					int seasonalTemperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(Terrain.ToCell(viewPosition.X), Terrain.ToCell(viewPosition.Z));
					float num6 = MathUtils.Lerp(0.5f, 0f, this.m_subsystemWeather.GlobalPrecipitationIntensity);
					float num7 = MathUtils.Lerp(1f, 0.8f, this.m_subsystemWeather.GlobalPrecipitationIntensity);
					this.m_viewFogRange.X = this.VisibilityRange * num6;
					this.m_viewFogRange.Y = this.VisibilityRange * num7;
					this.m_viewFogColor = this.CalculateSkyColor(new Vector3(camera.ViewDirection.X, 0f, camera.ViewDirection.Z), this.m_subsystemTimeOfDay.TimeOfDay, this.m_subsystemWeather.GlobalPrecipitationIntensity, seasonalTemperature);
					this.VisibilityRangeYMultiplier = MathUtils.Lerp(this.VisibilityRange / num4, this.VisibilityRange / num5, MathUtils.Pow(this.m_subsystemWeather.GlobalPrecipitationIntensity, 4f));
					this.m_viewIsSkyVisible = true;
				}
				if (!this.FogEnabled)
				{
					this.m_viewFogRange = new Vector2(100000f, 100000f);
				}
				if (!this.DrawSkyEnabled || !this.m_viewIsSkyVisible || SettingsManager.SkyRenderingMode == SkyRenderingMode.Disabled)
				{
					FlatBatch2D flatBatch2D = this.m_primitivesRenderer2d.FlatBatch(-1, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.Opaque);
					int count = flatBatch2D.TriangleVertices.Count;
					flatBatch2D.QueueQuad(Vector2.Zero, camera.ViewportSize, 0f, this.m_viewFogColor);
					flatBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
					if (SubsystemSky.Shader != null && SubsystemSky.ShaderAlphaTest != null)
					{
						if (this.m_primitiveRender.Shader == null && this.m_primitiveRender.ShaderAlphaTest == null)
						{
							this.m_primitiveRender.Shader = SubsystemSky.Shader;
							this.m_primitiveRender.ShaderAlphaTest = SubsystemSky.ShaderAlphaTest;
							this.m_primitiveRender.Camera = camera;
						}
						this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
						return;
					}
					this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
					return;
				}
			}
			else if (drawOrder == this.m_drawOrders[1])
			{
				if (this.DrawSkyEnabled && this.m_viewIsSkyVisible && SettingsManager.SkyRenderingMode != SkyRenderingMode.Disabled)
				{
					this.DrawSkydome(camera);
					if (SubsystemSky.DrawGalaxyEnabled)
					{
						this.DrawStars(camera);
						this.DrawSunAndMoon(camera);
					}
					this.DrawClouds(camera);
					ModsManager.HookAction("SkyDrawExtra", delegate(ModLoader loader)
					{
						loader.SkyDrawExtra(this, camera);
						return false;
					});
					if (SubsystemSky.Shader != null && SubsystemSky.ShaderAlphaTest != null)
					{
						if (this.m_primitiveRender.Shader == null && this.m_primitiveRender.ShaderAlphaTest == null)
						{
							this.m_primitiveRender.Shader = SubsystemSky.Shader;
							this.m_primitiveRender.ShaderAlphaTest = SubsystemSky.ShaderAlphaTest;
							this.m_primitiveRender.Camera = camera;
						}
						this.m_primitiveRender.Flush(this.m_primitivesRenderer3d, camera.ViewProjectionMatrix, true, int.MaxValue);
						return;
					}
					this.m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
					return;
				}
			}
			else
			{
				this.DrawLightning(camera);
				this.m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000587AC File Offset: 0x000569AC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			this.m_sunTexture = ContentManager.Get<Texture2D>("Textures/Sun", null);
			this.m_glowTexture = ContentManager.Get<Texture2D>("Textures/SkyGlow", null);
			this.m_cloudsTexture = ContentManager.Get<Texture2D>("Textures/Clouds", null);
			this.m_primitiveRender = new SkyPrimitiveRender();
			for (int i = 0; i < 8; i++)
			{
				this.m_moonTextures[i] = ContentManager.Get<Texture2D>("Textures/Moon" + (i + 1).ToString(CultureInfo.InvariantCulture), null);
			}
			this.UpdateLightAndViewParameters();
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x000588D4 File Offset: 0x00056AD4
		public override void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
			Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
			Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
			foreach (SubsystemSky.SkyDome skyDome in this.m_skyDomes.Values)
			{
				skyDome.Dispose();
			}
			this.m_skyDomes.Clear();
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0005895C File Offset: 0x00056B5C
		public void Display_DeviceReset()
		{
			Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
			Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
			foreach (SubsystemSky.SkyDome skyDome in this.m_skyDomes.Values)
			{
				skyDome.Dispose();
			}
			this.m_skyDomes.Clear();
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x000589D4 File Offset: 0x00056BD4
		public void DrawSkydome(Camera camera)
		{
			SubsystemSky.SkyDome skyDome;
			if (!this.m_skyDomes.TryGetValue(camera.GameWidget, out skyDome))
			{
				skyDome = new SubsystemSky.SkyDome();
				this.m_skyDomes.Add(camera.GameWidget, skyDome);
			}
			if (skyDome.VertexBuffer == null || skyDome.IndexBuffer == null)
			{
				Utilities.Dispose<VertexBuffer>(ref skyDome.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref skyDome.IndexBuffer);
				skyDome.VertexBuffer = new VertexBuffer(this.m_skyVertexDeclaration, skyDome.Vertices.Length);
				skyDome.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, skyDome.Indices.Length);
				this.FillSkyIndexBuffer(skyDome);
				skyDome.LastUpdateTimeOfDay = null;
			}
			int x = Terrain.ToCell(camera.ViewPosition.X);
			int z = Terrain.ToCell(camera.ViewPosition.Z);
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			int seasonalTemperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z);
			if (skyDome.LastUpdateTimeOfDay != null && MathUtils.Abs(timeOfDay - skyDome.LastUpdateTimeOfDay.Value) <= 0.001f && skyDome.LastUpdatePrecipitationIntensity != null && MathUtils.Abs(globalPrecipitationIntensity - skyDome.LastUpdatePrecipitationIntensity.Value) <= 0.02f && ((globalPrecipitationIntensity != 0f && globalPrecipitationIntensity != 1f) || skyDome.LastUpdatePrecipitationIntensity.Value == globalPrecipitationIntensity) && this.m_lightningStrikeBrightness == skyDome.LastUpdateLightningStrikeBrightness && skyDome.LastUpdateTemperature != null)
			{
				int num = seasonalTemperature;
				int? lastUpdateTemperature = skyDome.LastUpdateTemperature;
				if (num == lastUpdateTemperature.GetValueOrDefault() & lastUpdateTemperature != null)
				{
					goto IL_1F1;
				}
			}
			skyDome.LastUpdateTimeOfDay = new float?(timeOfDay);
			skyDome.LastUpdatePrecipitationIntensity = new float?(globalPrecipitationIntensity);
			skyDome.LastUpdateLightningStrikeBrightness = this.m_lightningStrikeBrightness;
			skyDome.LastUpdateTemperature = new int?(seasonalTemperature);
			this.FillSkyVertexBuffer(skyDome, timeOfDay, globalPrecipitationIntensity, seasonalTemperature);
			IL_1F1:
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			Display.BlendState = BlendState.Opaque;
			SubsystemSky.ShaderFlat.Transforms.World[0] = Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix;
			SubsystemSky.ShaderFlat.Color = Vector4.One;
			ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.SetShaderParameter(SubsystemSky.ShaderFlat, camera);
				return true;
			});
			Display.DrawIndexed(PrimitiveType.TriangleList, SubsystemSky.ShaderFlat, skyDome.VertexBuffer, skyDome.IndexBuffer, 0, skyDome.IndexBuffer.IndicesCount);
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00058C70 File Offset: 0x00056E70
		public void DrawStars(Camera camera)
		{
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			if (this.m_starsVertexBuffer == null || this.m_starsIndexBuffer == null)
			{
				Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
				this.m_starsVertexBuffer = new VertexBuffer(this.m_starsVertexDeclaration, 600);
				this.m_starsIndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, 900);
				this.FillStarsBuffers();
			}
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			float num = MathUtils.Sqr((1f - this.CalculateLightIntensity(timeOfDay)) * (1f - globalPrecipitationIntensity));
			if (num > 0.01f)
			{
				Display.BlendState = BlendState.Additive;
				SubsystemSky.ShaderTextured.Transforms.World[0] = Matrix.CreateRotationZ(-2f * timeOfDay * 3.14159274f) * Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix;
				SubsystemSky.ShaderTextured.Color = new Vector4(1f, 1f, 1f, num);
				SubsystemSky.ShaderTextured.Texture = ContentManager.Get<Texture2D>("Textures/Star", null);
				SubsystemSky.ShaderTextured.SamplerState = SamplerState.LinearClamp;
				ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
				{
					modLoader.SetShaderParameter(SubsystemSky.ShaderTextured, camera);
					return true;
				});
				Display.DrawIndexed(PrimitiveType.TriangleList, SubsystemSky.ShaderTextured, this.m_starsVertexBuffer, this.m_starsIndexBuffer, 0, this.m_starsIndexBuffer.IndicesCount);
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x00058E04 File Offset: 0x00057004
		public void DrawSunAndMoon(Camera camera)
		{
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			float f = MathUtils.Max(SubsystemSky.CalculateDawnGlowIntensity(timeOfDay), SubsystemSky.CalculateDuskGlowIntensity(timeOfDay));
			float num = 2f * timeOfDay * 3.14159274f;
			float angle = num + 3.14159274f;
			float num2 = MathUtils.Lerp(90f, 160f, f);
			float num3 = MathUtils.Lerp(60f, 80f, f);
			Color color = Color.Lerp(new Color(255, 255, 255), new Color(255, 255, 160), f);
			Color color2 = Color.White;
			color2 *= 1f - this.SkyLightIntensity;
			color *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			color2 *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color3 = color * 0.6f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color4 = color * 0.2f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			TexturedBatch3D batch = this.m_primitivesRenderer3d.TexturedBatch(this.m_glowTexture, false, 0, DepthStencilState.DepthRead, null, BlendState.Additive, null);
			TexturedBatch3D batch2 = this.m_primitivesRenderer3d.TexturedBatch(this.m_sunTexture, false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, null);
			TexturedBatch3D batch3 = this.m_primitivesRenderer3d.TexturedBatch(this.m_moonTextures[this.MoonPhase], false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, null);
			this.QueueCelestialBody(batch, camera.ViewPosition, color3, 900f, 3.5f * num2, num);
			this.QueueCelestialBody(batch, camera.ViewPosition, color4, 900f, 3.5f * num3, angle);
			this.QueueCelestialBody(batch2, camera.ViewPosition, color, 900f, num2, num);
			this.QueueCelestialBody(batch3, camera.ViewPosition, color2, 900f, num3, angle);
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x00059014 File Offset: 0x00057214
		public void DrawLightning(Camera camera)
		{
			if (this.m_lightningStrikePosition == null)
			{
				return;
			}
			FlatBatch3D flatBatch3D = this.m_primitivesRenderer3d.FlatBatch(0, DepthStencilState.DepthRead, null, BlendState.Additive);
			Vector3 value = this.m_lightningStrikePosition.Value;
			Vector3 unitY = Vector3.UnitY;
			Vector3 v = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, unitY));
			Viewport viewport = Display.Viewport;
			float num = Vector4.Transform(new Vector4(value, 1f), camera.ViewProjectionMatrix).W * 2f / ((float)viewport.Width * camera.ProjectionMatrix.M11);
			for (int i = 0; i < (int)(this.m_lightningStrikeBrightness * 30f); i++)
			{
				float s = this.m_random.NormalFloat(0f, 1f * num);
				float s2 = this.m_random.NormalFloat(0f, 1f * num);
				Vector3 v2 = s * v + s2 * unitY;
				float num4;
				for (float num2 = 260f; num2 > value.Y; num2 -= num4)
				{
					uint num3 = MathUtils.Hash((uint)(this.m_lightningStrikePosition.Value.X + 100f * this.m_lightningStrikePosition.Value.Z + 200f * num2));
					num4 = MathUtils.Lerp(4f, 10f, (float)(num3 & 255U) / 255f);
					float s3 = (float)(((num3 & 1U) == 0U) ? 1 : -1);
					float s4 = MathUtils.Lerp(0.05f, 0.2f, (float)(num3 >> 8 & 255U) / 255f);
					float num5 = num2;
					float num6 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(num3 >> 16 & 255U) / 255f);
					float num7 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(num3 >> 24 & 255U) / 255f);
					float num8 = num5 - num4;
					Vector3 p = new Vector3(value.X, num5, value.Z) + v2;
					Vector3 vector = new Vector3(value.X, num6, value.Z) + v2 - num4 * v * s3 * s4;
					Vector3 vector2 = new Vector3(value.X, num7, value.Z) + v2 + num4 * v * s3 * s4;
					Vector3 p2 = new Vector3(value.X, num8, value.Z) + v2;
					Color color = Color.White * 0.2f * MathUtils.Saturate((260f - num5) * 0.2f);
					Color color2 = Color.White * 0.2f * MathUtils.Saturate((260f - num6) * 0.2f);
					Color color3 = Color.White * 0.2f * MathUtils.Saturate((260f - num7) * 0.2f);
					Color color4 = Color.White * 0.2f * MathUtils.Saturate((260f - num8) * 0.2f);
					flatBatch3D.QueueLine(p, vector, color, color2);
					flatBatch3D.QueueLine(vector, vector2, color2, color3);
					flatBatch3D.QueueLine(vector2, p2, color3, color4);
				}
			}
			float num9 = MathUtils.Lerp(0.3f, 0.75f, 0.5f * (float)MathUtils.Sin(MathUtils.Remainder(1.0 * this.m_subsystemTime.GameTime, 6.2831854820251465)) + 0.5f);
			this.m_lightningStrikeBrightness -= this.m_subsystemTime.GameTimeDelta / num9;
			if (this.m_lightningStrikeBrightness <= 0f)
			{
				this.m_lightningStrikePosition = null;
				this.m_lightningStrikeBrightness = 0f;
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00059418 File Offset: 0x00057618
		public void DrawClouds(Camera camera)
		{
			if (SettingsManager.SkyRenderingMode == SkyRenderingMode.NoClouds)
			{
				return;
			}
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float num = MathUtils.Lerp(0.03f, 1f, MathUtils.Sqr(this.SkyLightIntensity)) * MathUtils.Lerp(1f, 0.2f, globalPrecipitationIntensity);
			this.m_cloudsLayerColors[0] = Color.White * (num * 0.7f);
			this.m_cloudsLayerColors[1] = Color.White * (num * 0.6f);
			this.m_cloudsLayerColors[2] = this.ViewFogColor;
			this.m_cloudsLayerColors[3] = Color.Transparent;
			double gameTime = this.m_subsystemTime.GameTime;
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)MathUtils.Remainder(0.0070000000949949027 * gameTime - (double)(viewPosition.X / 1900f * 1.75f), 1.0) + viewPosition.X / 1900f * 1.75f, (float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.Z / 1900f * 1.75f), 1.0) + viewPosition.Z / 1900f * 1.75f);
			TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3d.TexturedBatch(this.m_cloudsTexture, false, 2, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
			DynamicArray<VertexPositionColorTexture> triangleVertices = texturedBatch3D.TriangleVertices;
			DynamicArray<ushort> triangleIndices = texturedBatch3D.TriangleIndices;
			int count = triangleVertices.Count;
			int count2 = triangleVertices.Count;
			int count3 = triangleIndices.Count;
			triangleVertices.Count += 49;
			triangleIndices.Count += 216;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					int num2 = j - 3;
					int num3 = i - 3;
					int num4 = MathUtils.Max(MathUtils.Abs(num2), MathUtils.Abs(num3));
					float num5 = this.m_cloudsLayerRadii[num4];
					float num6 = (num4 > 0) ? (num5 / MathUtils.Sqrt((float)(num2 * num2 + num3 * num3))) : 0f;
					float num7 = (float)num2 * num6;
					float num8 = (float)num3 * num6;
					float y = MathUtils.Lerp(800f, 60f, num5 * num5);
					Vector3 vector = new Vector3(viewPosition.X + num7 * 1900f, y, viewPosition.Z + num8 * 1900f);
					Vector2 texCoord = new Vector2(vector.X, vector.Z) / 1900f * 1.75f - v;
					Color color = this.m_cloudsLayerColors[num4];
					texturedBatch3D.TriangleVertices.Array[count2++] = new VertexPositionColorTexture(vector, color, texCoord);
					if (j > 0 && i > 0)
					{
						ushort num9 = (ushort)(count + j + i * 7);
						ushort num10 = (ushort)(count + (j - 1) + i * 7);
						ushort num11 = (ushort)(count + (j - 1) + (i - 1) * 7);
						ushort num12 = (ushort)(count + j + (i - 1) * 7);
						if ((num2 <= 0 && num3 <= 0) || (num2 > 0 && num3 > 0))
						{
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
						}
						else
						{
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
						}
					}
				}
			}
			bool drawCloudsWireframe = this.DrawCloudsWireframe;
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x00059874 File Offset: 0x00057A74
		public void QueueCelestialBody(TexturedBatch3D batch, Vector3 viewPosition, Color color, float distance, float radius, float angle)
		{
			if (color.A > 0)
			{
				Vector3 vector = new Vector3
				{
					X = -MathUtils.Sin(angle),
					Y = -MathUtils.Cos(angle)
				};
				Vector3 unitZ = Vector3.UnitZ;
				Vector3 v = Vector3.Cross(unitZ, vector);
				Vector3 p = viewPosition + vector * distance - radius * unitZ - radius * v;
				Vector3 p2 = viewPosition + vector * distance + radius * unitZ - radius * v;
				Vector3 p3 = viewPosition + vector * distance + radius * unitZ + radius * v;
				Vector3 p4 = viewPosition + vector * distance - radius * unitZ + radius * v;
				batch.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), color);
			}
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x000599B8 File Offset: 0x00057BB8
		public void UpdateLightAndViewParameters()
		{
			this.VisibilityRange = (float)SettingsManager.VisibilityRange;
			this.SkyLightIntensity = this.CalculateLightIntensity(this.m_subsystemTimeOfDay.TimeOfDay);
			this.SkyLightValue = ((this.MoonPhase == 4) ? SubsystemSky.m_lightValuesMoonless[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))] : SubsystemSky.m_lightValuesNormal[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))]);
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00059A3C File Offset: 0x00057C3C
		public float CalculateLightIntensity(float timeOfDay)
		{
			if (timeOfDay <= 0.2f || timeOfDay > 0.8f)
			{
				return 0f;
			}
			if (timeOfDay > 0.2f && timeOfDay <= 0.3f)
			{
				return (timeOfDay - 0.2f) / 0.100000009f;
			}
			if (timeOfDay > 0.3f && timeOfDay <= 0.7f)
			{
				return 1f;
			}
			return 1f - (timeOfDay - 0.7f) / 0.100000024f;
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x00059AA8 File Offset: 0x00057CA8
		public Color CalculateSkyColor(Vector3 direction, float timeOfDay, float precipitationIntensity, int temperature)
		{
			direction = Vector3.Normalize(direction);
			Vector2 vector = Vector2.Normalize(new Vector2(direction.X, direction.Z));
			float s = this.CalculateLightIntensity(timeOfDay);
			float f = (float)temperature / 15f;
			Vector3 v = new Vector3(0.65f, 0.68f, 0.7f) * s;
			Vector3 v2 = Vector3.Lerp(new Vector3(0.28f, 0.38f, 0.52f), new Vector3(0.15f, 0.3f, 0.56f), f);
			Vector3 v3 = Vector3.Lerp(new Vector3(0.7f, 0.79f, 0.88f), new Vector3(0.64f, 0.77f, 0.91f), f);
			Vector3 v4 = Vector3.Lerp(v2, v, precipitationIntensity) * s;
			Vector3 v5 = Vector3.Lerp(v3, v, precipitationIntensity) * s;
			Vector3 v6 = new Vector3(1f, 0.3f, -0.2f);
			Vector3 v7 = new Vector3(1f, 0.3f, -0.2f);
			if (this.m_lightningStrikePosition != null)
			{
				v4 = Vector3.Max(new Vector3(this.m_lightningStrikeBrightness), v4);
			}
			float num = MathUtils.Lerp(SubsystemSky.CalculateDawnGlowIntensity(timeOfDay), 0f, precipitationIntensity);
			float num2 = MathUtils.Lerp(SubsystemSky.CalculateDuskGlowIntensity(timeOfDay), 0f, precipitationIntensity);
			float f2 = MathUtils.Saturate((direction.Y - 0.1f) / 0.4f);
			float s2 = num * MathUtils.Sqr(MathUtils.Saturate(0f - vector.X));
			float s3 = num2 * MathUtils.Sqr(MathUtils.Saturate(vector.X));
			return new Color(Vector3.Lerp(v5 + v6 * s2 + v7 * s3, v4, f2));
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x00059C60 File Offset: 0x00057E60
		public void FillSkyVertexBuffer(SubsystemSky.SkyDome skyDome, float timeOfDay, float precipitationIntensity, int temperature)
		{
			for (int i = 0; i < 8; i++)
			{
				float x = 1.57079637f * MathUtils.Sqr((float)i / 7f);
				for (int j = 0; j < 10; j++)
				{
					int num = j + i * 10;
					float x2 = 6.28318548f * (float)j / 10f;
					float num2 = 1800f * MathUtils.Cos(x);
					skyDome.Vertices[num].Position.X = num2 * MathUtils.Sin(x2);
					skyDome.Vertices[num].Position.Z = num2 * MathUtils.Cos(x2);
					skyDome.Vertices[num].Position.Y = 1800f * MathUtils.Sin(x) - ((i == 0) ? 450f : 0f);
					skyDome.Vertices[num].Color = this.CalculateSkyColor(skyDome.Vertices[num].Position, timeOfDay, precipitationIntensity, temperature);
				}
			}
			skyDome.VertexBuffer.SetData<SubsystemSky.SkyVertex>(skyDome.Vertices, 0, skyDome.Vertices.Length, 0);
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x00059D84 File Offset: 0x00057F84
		public void FillSkyIndexBuffer(SubsystemSky.SkyDome skyDome)
		{
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					int num2 = j;
					int num3 = (j + 1) % 10;
					int num4 = i;
					int num5 = i + 1;
					skyDome.Indices[num++] = (ushort)(num2 + num4 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num4 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num2 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num2 + num4 * 10);
				}
			}
			for (int k = 2; k < 10; k++)
			{
				skyDome.Indices[num++] = 0;
				skyDome.Indices[num++] = (ushort)(k - 1);
				skyDome.Indices[num++] = (ushort)k;
			}
			skyDome.IndexBuffer.SetData<ushort>(skyDome.Indices, 0, skyDome.Indices.Length, 0);
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x00059EA0 File Offset: 0x000580A0
		public void FillStarsBuffers()
		{
			Game.Random random = new Game.Random(7);
			SubsystemSky.StarVertex[] array = new SubsystemSky.StarVertex[600];
			for (int i = 0; i < 150; i++)
			{
				Vector3 vector;
				do
				{
					vector = new Vector3(random.Float(-1f, 2f), random.Float(-1f, 2f), random.Float(-1f, 2f));
				}
				while (vector.LengthSquared() > 1f);
				vector = Vector3.Normalize(vector);
				float s = 10f * random.NormalFloat(1f, 0.1f);
				float w = MathUtils.Saturate(random.NormalFloat(0.6f, 0.4f));
				Color color = new Color(new Vector4(random.Float(0.6f, 1f), 0.7f, random.Float(0.8f, 1f), w));
				Vector3 v = 1000f * vector;
				Vector3 vector2 = Vector3.Normalize(Vector3.Cross((vector.X > vector.Y) ? Vector3.UnitY : Vector3.UnitX, vector));
				Vector3 v2 = Vector3.Normalize(Vector3.Cross(vector2, vector));
				Vector3 position = v + s * (-vector2 - v2);
				Vector3 position2 = v + s * (vector2 - v2);
				Vector3 position3 = v + s * (vector2 + v2);
				Vector3 position4 = v + s * (-vector2 + v2);
				array[i * 4] = new SubsystemSky.StarVertex
				{
					Position = position,
					TextureCoordinate = new Vector2(0f, 0f),
					Color = color
				};
				array[i * 4 + 1] = new SubsystemSky.StarVertex
				{
					Position = position2,
					TextureCoordinate = new Vector2(1f, 0f),
					Color = color
				};
				array[i * 4 + 2] = new SubsystemSky.StarVertex
				{
					Position = position3,
					TextureCoordinate = new Vector2(1f, 1f),
					Color = color
				};
				array[i * 4 + 3] = new SubsystemSky.StarVertex
				{
					Position = position4,
					TextureCoordinate = new Vector2(0f, 1f),
					Color = color
				};
			}
			this.m_starsVertexBuffer.SetData<SubsystemSky.StarVertex>(array, 0, array.Length, 0);
			ushort[] array2 = new ushort[900];
			for (int j = 0; j < 150; j++)
			{
				array2[j * 6] = (ushort)(j * 4);
				array2[j * 6 + 1] = (ushort)(j * 4 + 1);
				array2[j * 6 + 2] = (ushort)(j * 4 + 2);
				array2[j * 6 + 3] = (ushort)(j * 4 + 2);
				array2[j * 6 + 4] = (ushort)(j * 4 + 3);
				array2[j * 6 + 5] = (ushort)(j * 4);
			}
			this.m_starsIndexBuffer.SetData<ushort>(array2, 0, array2.Length, 0);
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0005A1BB File Offset: 0x000583BB
		public static float CalculateDawnGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.25f) / 0.100000009f * 2f, 0f);
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0005A1E5 File Offset: 0x000583E5
		public static float CalculateDuskGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.75f) / 0.100000024f * 2f, 0f);
		}

		// Token: 0x04000634 RID: 1588
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000635 RID: 1589
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000636 RID: 1590
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000637 RID: 1591
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000638 RID: 1592
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000639 RID: 1593
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400063A RID: 1594
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x0400063B RID: 1595
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x0400063C RID: 1596
		public PrimitivesRenderer2D m_primitivesRenderer2d = new PrimitivesRenderer2D();

		// Token: 0x0400063D RID: 1597
		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		// Token: 0x0400063E RID: 1598
		public SkyPrimitiveRender m_primitiveRender;

		// Token: 0x0400063F RID: 1599
		public static SkyShader Shader;

		// Token: 0x04000640 RID: 1600
		public static SkyShader ShaderAlphaTest;

		// Token: 0x04000641 RID: 1601
		public static UnlitShader ShaderFlat = new UnlitShader(ShaderCodeManager.GetFast("Shaders/Unlit.vsh"), ShaderCodeManager.GetFast("Shaders/Unlit.psh"), true, false, false);

		// Token: 0x04000642 RID: 1602
		public static UnlitShader ShaderTextured = new UnlitShader(ShaderCodeManager.GetFast("Shaders/Unlit.vsh"), ShaderCodeManager.GetFast("Shaders/Unlit.psh"), true, true, false);

		// Token: 0x04000643 RID: 1603
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000644 RID: 1604
		public Color m_viewFogColor;

		// Token: 0x04000645 RID: 1605
		public Vector2 m_viewFogRange;

		// Token: 0x04000646 RID: 1606
		public bool m_viewIsSkyVisible;

		// Token: 0x04000647 RID: 1607
		public Texture2D m_sunTexture;

		// Token: 0x04000648 RID: 1608
		public Texture2D m_glowTexture;

		// Token: 0x04000649 RID: 1609
		public Texture2D m_cloudsTexture;

		// Token: 0x0400064A RID: 1610
		public Texture2D[] m_moonTextures = new Texture2D[8];

		// Token: 0x0400064B RID: 1611
		public VertexDeclaration m_skyVertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});

		// Token: 0x0400064C RID: 1612
		public Dictionary<GameWidget, SubsystemSky.SkyDome> m_skyDomes = new Dictionary<GameWidget, SubsystemSky.SkyDome>();

		// Token: 0x0400064D RID: 1613
		public VertexBuffer m_starsVertexBuffer;

		// Token: 0x0400064E RID: 1614
		public IndexBuffer m_starsIndexBuffer;

		// Token: 0x0400064F RID: 1615
		public VertexDeclaration m_starsVertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(20, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});

		// Token: 0x04000650 RID: 1616
		public const int m_starsCount = 150;

		// Token: 0x04000651 RID: 1617
		public Vector3? m_lightningStrikePosition;

		// Token: 0x04000652 RID: 1618
		public float m_lightningStrikeBrightness;

		// Token: 0x04000653 RID: 1619
		public double m_lastLightningStrikeTime;

		// Token: 0x04000654 RID: 1620
		public const float DawnStart = 0.2f;

		// Token: 0x04000655 RID: 1621
		public const float DayStart = 0.3f;

		// Token: 0x04000656 RID: 1622
		public const float DuskStart = 0.7f;

		// Token: 0x04000657 RID: 1623
		public const float NightStart = 0.8f;

		// Token: 0x04000658 RID: 1624
		public bool DrawSkyEnabled = true;

		// Token: 0x04000659 RID: 1625
		public static bool DrawGalaxyEnabled = true;

		// Token: 0x0400065A RID: 1626
		public bool DrawCloudsWireframe;

		// Token: 0x0400065B RID: 1627
		public bool FogEnabled = true;

		// Token: 0x0400065C RID: 1628
		public int[] m_drawOrders = new int[]
		{
			-100,
			5,
			105
		};

		// Token: 0x0400065D RID: 1629
		public float[] m_cloudsLayerRadii = new float[]
		{
			0f,
			0.8f,
			0.95f,
			1f
		};

		// Token: 0x0400065E RID: 1630
		public Color[] m_cloudsLayerColors = new Color[5];

		// Token: 0x0400065F RID: 1631
		public static int[] m_lightValuesMoonless = new int[]
		{
			0,
			3,
			6,
			9,
			12,
			15
		};

		// Token: 0x04000660 RID: 1632
		public static int[] m_lightValuesNormal = new int[]
		{
			3,
			5,
			8,
			10,
			13,
			15
		};

		// Token: 0x04000664 RID: 1636
		public float m_visibilityRange;

		// Token: 0x020004A7 RID: 1191
		public struct SkyVertex
		{
			// Token: 0x0400172D RID: 5933
			public Vector3 Position;

			// Token: 0x0400172E RID: 5934
			public Color Color;
		}

		// Token: 0x020004A8 RID: 1192
		public class SkyDome : IDisposable
		{
			// Token: 0x060020CB RID: 8395 RVA: 0x000E9443 File Offset: 0x000E7643
			public void Dispose()
			{
				Utilities.Dispose<VertexBuffer>(ref this.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.IndexBuffer);
			}

			// Token: 0x0400172F RID: 5935
			public const int VerticesCountX = 10;

			// Token: 0x04001730 RID: 5936
			public const int VerticesCountY = 8;

			// Token: 0x04001731 RID: 5937
			public float? LastUpdateTimeOfDay;

			// Token: 0x04001732 RID: 5938
			public float? LastUpdatePrecipitationIntensity;

			// Token: 0x04001733 RID: 5939
			public int? LastUpdateTemperature;

			// Token: 0x04001734 RID: 5940
			public float LastUpdateLightningStrikeBrightness;

			// Token: 0x04001735 RID: 5941
			public SubsystemSky.SkyVertex[] Vertices = new SubsystemSky.SkyVertex[80];

			// Token: 0x04001736 RID: 5942
			public ushort[] Indices = new ushort[444];

			// Token: 0x04001737 RID: 5943
			public VertexBuffer VertexBuffer;

			// Token: 0x04001738 RID: 5944
			public IndexBuffer IndexBuffer;
		}

		// Token: 0x020004A9 RID: 1193
		public struct StarVertex
		{
			// Token: 0x04001739 RID: 5945
			public Vector3 Position;

			// Token: 0x0400173A RID: 5946
			public Vector2 TextureCoordinate;

			// Token: 0x0400173B RID: 5947
			public Color Color;
		}
	}
}
