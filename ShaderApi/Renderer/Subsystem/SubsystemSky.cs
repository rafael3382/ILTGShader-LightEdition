using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;
using Game;

namespace ShaderApi
{
	public class SubsystemSky : Game.SubsystemSky, IDrawable, IUpdateable
	{
		
		
		//public Image Sky2D = new Image(512, 512);
		
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		
		public void Update(float dt)
		{
			this.MoonPhase = ((int)MathUtils.Floor(this.m_subsystemTimeOfDay.Day - 0.5 + 5.0) % 8 + 8) % 8;
			this.UpdateLightAndViewParameters();
		}

		
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
					float num3 = MathUtils.Lerp(0.33f, 1f, this.SkyLightIntensity);
					this.m_viewFogRange.X = 0f;
					this.m_viewFogRange.Y = MathUtils.Lerp(4f, 10f, num * num2 * num3);
					this.m_viewFogColor = Color.MultiplyColorOnly(c, 0.66f * num2 * num3);
					this.VisibilityRangeYMultiplier = 1f;
					this.m_viewIsSkyVisible = false;
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
					this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
					return;
				}
			}
			else if (drawOrder == this.m_drawOrders[1])
			{
				if (this.DrawSkyEnabled && this.m_viewIsSkyVisible && SettingsManager.SkyRenderingMode != SkyRenderingMode.Disabled)
				{
					this.DrawSkydome(camera);
					
					this.NDrawSunAndMoon(camera);
					this.NDrawClouds(camera);
					this.NDrawStars(camera);
					this.DrawStars(camera);
					m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
					return;
				}
			}
			else
			{
			    if (drawOrder == this.m_drawOrders[2])
				{
				    this.DrawLightning(camera);
				    this.m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
					
				} else { 
				
				SubsystemReflections.Draw(camera, this, drawOrder);
					
				}
			}
		}
        public int[] m_drawOrders = new int[]
		{
			-100,
			5,
			105,
			int.MinValue,
			1250
		};
		
		private Texture2D m_starsTexture;
		
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
			this.m_sunTexture = ContentManager.Get<Texture2D>("Textures/Sun");
			this.m_glowTexture = ContentManager.Get<Texture2D>("Textures/SkyGlow");
			this.m_cloudsTexture = ContentManager.Get<Texture2D>("Textures/Clouds");
			this.m_starsTexture = ContentManager.Get<Texture2D>("Textures/Night");

			SubsystemReflections.Load(this);
			
			
			
			for (int i = 0; i < 8; i++)
			{
				this.m_moonTextures[i] = ContentManager.Get<Texture2D>("Textures/Moon" + (i + 1).ToString(CultureInfo.InvariantCulture));
			}
			this.UpdateLightAndViewParameters();
			Display.DeviceReset += new Action(this.Display_DeviceReset);
		}

		

		
		

		
		
		
		public void NDrawSunAndMoon(Camera camera)
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

		
		public void NDrawLightning(Camera camera)
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
				this.m_lightningStrikePosition = default(Vector3?);
				this.m_lightningStrikeBrightness = 0f;
			}
		}

		
		public void NDrawClouds(Camera camera)
		{
			if (SettingsManager.SkyRenderingMode == SkyRenderingMode.NoClouds)
			{
				return;
			}
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float num = MathUtils.Lerp(0.03f, 1f, MathUtils.Sqr(this.SkyLightIntensity)) * MathUtils.Lerp(1f, 0.2f, globalPrecipitationIntensity);
			this.m_cloudsLayerColors[0] = Color.White * (num * 0.75f);
			this.m_cloudsLayerColors[1] = Color.White * (num * 0.66f);
			this.m_cloudsLayerColors[2] = this.ViewFogColor;
			this.m_cloudsLayerColors[3] = Color.Transparent;
			double gameTime = this.m_subsystemTime.GameTime;
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)MathUtils.Remainder(0.0020400000949949026 * gameTime - (double)(viewPosition.X / 1900f * 1.75f), 1.0) + viewPosition.X / 1900f * 1.75f, (float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.Z / 1900f * 1.75f), 1.0) + viewPosition.Z / 1900f * 1.75f);
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
					float y = MathUtils.Lerp(600f, 60f, num5 * num5);
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
		public void NDrawStars(Camera camera)
		{
			if (SettingsManager.SkyRenderingMode == SkyRenderingMode.NoClouds)
			{
				return;
			}
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float num = MathUtils.Sqr((1f - this.CalculateLightIntensity(timeOfDay)) * (1f - globalPrecipitationIntensity));
			if (num < 0.5f)
            {
                return;
            }
            
            
			
			
			
			double gameTime = this.m_subsystemTime.GameTime;
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)MathUtils.Remainder(0.0020400000949949026 * gameTime - (double)(viewPosition.X / 1900f * 1.75f), 1.0) + viewPosition.X / 1900f * 1.75f, (float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.Z / 1900f * 1.75f), 1.0) + viewPosition.Z / 1900f * 1.75f);
			TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3d.TexturedBatch(m_starsTexture, false, 2, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
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
					float y = MathUtils.Lerp(600f, 60f, num5 * num5);
					Vector3 vector = new Vector3(viewPosition.X + num7 * 1900f, y, viewPosition.Z + num8 * 1900f);
					Vector2 texCoord = new Vector2(vector.X, vector.Z) / 1900f * 1.75f - v;
					Color color = Color.White * 0.2f;
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
			for (int i = 0; i > 7; i++)
			{
				for (int j = 0; j > 7; j++)
				{
					int num2 = j - 3;
					int num3 = i - 3;
					int num4 = MathUtils.Max(MathUtils.Abs(num2), MathUtils.Abs(num3));
					float num5 = this.m_cloudsLayerRadii[num4];
					float num6 = (num4 > 0) ? (num5 / MathUtils.Sqrt((float)(num2 * num2 + num3 * num3))) : 0f;
					float num7 = (float)num2 * num6;
					float num8 = (float)num3 * num6;
					float y = MathUtils.Lerp(600f, 60f, num5 * num5);
					Vector3 vector = new Vector3(viewPosition.X + num7 * 1900f, -y, viewPosition.Z + num8 * 1900f);
					Vector2 texCoord = new Vector2(vector.X, vector.Z) / 1900f * 1.75f - v;
					Color color = Color.White;
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
		}

		
		public void QueueCelestialBody(TexturedBatch3D batch, Vector3 viewPosition, Color color, float distance, float radius, float angle)
		{
			if (color.A > 0)
			{
				Vector3 vector = new Vector3
				{
					X = 0f - MathUtils.Sin(angle),
					Y = 0f - MathUtils.Cos(angle),
					Z = 0f
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

		
		public void UpdateLightAndViewParameters()
		{
			this.VisibilityRange = (float)SettingsManager.VisibilityRange;
			this.SkyLightIntensity = this.CalculateLightIntensity(this.m_subsystemTimeOfDay.TimeOfDay);
			if (this.MoonPhase == 4)
			{
				this.SkyLightValue = SubsystemSky.m_lightValuesMoonless[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))];
				return;
			}
			this.SkyLightValue = SubsystemSky.m_lightValuesNormal[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))];
			
			
			
			
			
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			
			float num = 2f * timeOfDay * 3.14159274f;
			float angle = num + 3.14159274f;
            
            
            
            
			
		}

		
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

		
		public void FillStarsBuffers()
		{
			Game.Random random = new Game.Random(7);
			SubsystemSky.StarVertex[] array = new SubsystemSky.StarVertex[600];
			for (int i = 0; i < 150; i++)
			{
				Vector3 vector;
				do
				{
					vector = new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f));
				}
				while (vector.LengthSquared() > 1f);
				vector = Vector3.Normalize(vector);
				float s = 9f * random.NormalFloat(1f, 0.1f);
				float w = MathUtils.Saturate(random.NormalFloat(0.6f, 0.4f));
				Color color = new Color(new Vector4(random.Float(0.6f, 1f), 0.7f, random.Float(0.8f, 1f), w));
				Vector3 v = 900f * vector;
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

		
		public static float CalculateDawnGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.25f) / 0.100000009f * 2f, 0f);
		}

		
		public static float CalculateDuskGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.75f) / 0.100000024f * 2f, 0f);
		}

		
		
	}
}
