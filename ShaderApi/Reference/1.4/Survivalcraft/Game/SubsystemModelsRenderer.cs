using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BB RID: 443
	public class SubsystemModelsRenderer : Subsystem, IDrawable
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x0004CB55 File Offset: 0x0004AD55
		public PrimitivesRenderer3D PrimitivesRenderer
		{
			get
			{
				return this.m_primitivesRenderer;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x0004CB5D File Offset: 0x0004AD5D
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0004CB68 File Offset: 0x0004AD68
		public void Draw(Camera camera, int drawOrder)
		{
			if (drawOrder == this.m_drawOrders[0])
			{
				this.ModelsDrawn = 0;
				List<SubsystemModelsRenderer.ModelData>[] modelsToDraw = this.m_modelsToDraw;
				for (int i = 0; i < modelsToDraw.Length; i++)
				{
					modelsToDraw[i].Clear();
				}
				this.m_modelsToPrepare.Clear();
				foreach (SubsystemModelsRenderer.ModelData modelData in this.m_componentModels.Values)
				{
					if (modelData.ComponentModel.Model != null)
					{
						modelData.ComponentModel.CalculateIsVisible(camera);
						if (modelData.ComponentModel.IsVisibleForCamera)
						{
							this.m_modelsToPrepare.Add(modelData);
						}
					}
				}
				this.m_modelsToPrepare.Sort();
				foreach (SubsystemModelsRenderer.ModelData modelData2 in this.m_modelsToPrepare)
				{
					this.PrepareModel(modelData2, camera);
					this.m_modelsToDraw[(int)modelData2.ComponentModel.RenderingMode].Add(modelData2);
				}
			}
			if (!SubsystemModelsRenderer.DisableDrawingModels)
			{
				this.m_sunLightDirection = 1.25f * Vector3.TransformNormal(this.SunVector(this.m_subsystemSky), camera.ViewMatrix);
				if (drawOrder == this.m_drawOrders[1])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
					Display.BlendState = BlendState.Opaque;
					this.DrawModels(camera, this.m_modelsToDraw[0], null);
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					this.DrawModels(camera, this.m_modelsToDraw[1], new float?(0f));
					Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
					this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, 0);
					return;
				}
				if (drawOrder == this.m_drawOrders[2])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					Display.BlendState = BlendState.AlphaBlend;
					this.DrawModels(camera, this.m_modelsToDraw[2], null);
					return;
				}
				if (drawOrder == this.m_drawOrders[3])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					Display.BlendState = BlendState.AlphaBlend;
					this.DrawModels(camera, this.m_modelsToDraw[3], null);
					if (SubsystemModelsRenderer.ShaderOpaque != null && SubsystemModelsRenderer.ShaderAlphaTested != null)
					{
						this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, int.MaxValue);
						return;
					}
					this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, int.MaxValue);
					return;
				}
			}
			else
			{
				this.m_primitivesRenderer.Clear();
			}
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0004CE18 File Offset: 0x0004B018
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemShadows = base.Project.FindSubsystem<SubsystemShadows>(true);
			ModsManager.HookAction("GetMaxInstancesCount", delegate(ModLoader modLoader)
			{
				this.MaxInstancesCount = Math.Max(modLoader.GetMaxInstancesCount(), this.MaxInstancesCount);
				return false;
			});
			this.m_shaderOpaque = new ModelShader(ShaderCodeManager.GetFast("Shaders/Model.vsh"), ShaderCodeManager.GetFast("Shaders/Model.psh"), false, 7);
			this.m_shaderAlphaTested = new ModelShader(ShaderCodeManager.GetFast("Shaders/Model.vsh"), ShaderCodeManager.GetFast("Shaders/Model.psh"), true, 7);
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0004CEC8 File Offset: 0x0004B0C8
		public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentModel componentModel in entity.FindComponents<ComponentModel>())
			{
				SubsystemModelsRenderer.ModelData value = new SubsystemModelsRenderer.ModelData
				{
					ComponentModel = componentModel,
					ComponentBody = componentModel.Entity.FindComponent<ComponentBody>(),
					Light = this.m_subsystemSky.SkyLightIntensity
				};
				this.m_componentModels.Add(componentModel, value);
			}
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0004CF54 File Offset: 0x0004B154
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentModel key in entity.FindComponents<ComponentModel>())
			{
				this.m_componentModels.Remove(key);
			}
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0004CFB0 File Offset: 0x0004B1B0
		public void PrepareModel(SubsystemModelsRenderer.ModelData modelData, Camera camera)
		{
			if (Time.FrameIndex > modelData.LastAnimateFrame)
			{
				modelData.ComponentModel.Animate();
				modelData.LastAnimateFrame = Time.FrameIndex;
			}
			if (Time.FrameStartTime >= modelData.NextLightTime)
			{
				float? num = this.CalculateModelLight(modelData);
				if (num != null)
				{
					modelData.Light = num.Value;
				}
				modelData.NextLightTime = Time.FrameStartTime + 0.1;
			}
			modelData.ComponentModel.CalculateAbsoluteBonesTransforms(camera);
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0004D02C File Offset: 0x0004B22C
		public void DrawModels(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData, float? alphaThreshold)
		{
			this.DrawInstancedModels(camera, modelsData, alphaThreshold);
			this.DrawModelsExtras(camera, modelsData);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0004D040 File Offset: 0x0004B240
		public void DrawInstancedModels(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData, float? alphaThreshold)
		{
			ModelShader modelShader = null;
			if (SubsystemModelsRenderer.ShaderOpaque != null && SubsystemModelsRenderer.ShaderAlphaTested != null)
			{
				modelShader = ((alphaThreshold != null) ? SubsystemModelsRenderer.ShaderAlphaTested : SubsystemModelsRenderer.ShaderOpaque);
			}
			else
			{
				modelShader = ((alphaThreshold != null) ? this.m_shaderAlphaTested : this.m_shaderOpaque);
			}
			modelShader.LightDirection1 = -Vector3.TransformNormal(LightingManager.DirectionToLight1, camera.ViewMatrix);
			modelShader.LightDirection2 = -Vector3.TransformNormal(LightingManager.DirectionToLight2, camera.ViewMatrix);
			modelShader.FogColor = new Vector3(this.m_subsystemSky.ViewFogColor);
			modelShader.FogStartInvLength = new Vector2(this.m_subsystemSky.ViewFogRange.X, 1f / (this.m_subsystemSky.ViewFogRange.Y - this.m_subsystemSky.ViewFogRange.X));
			modelShader.FogYMultiplier = this.m_subsystemSky.VisibilityRangeYMultiplier;
			modelShader.WorldUp = Vector3.TransformNormal(Vector3.UnitY, camera.ViewMatrix);
			modelShader.Transforms.View = Matrix.Identity;
			modelShader.Transforms.Projection = camera.ProjectionMatrix;
			modelShader.SamplerState = SamplerState.PointClamp;
			if (alphaThreshold != null)
			{
				modelShader.AlphaThreshold = alphaThreshold.Value;
			}
			ModsManager.HookAction("ModelShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.ModelShaderParameter(modelShader, camera, modelsData, alphaThreshold);
				return true;
			});
			ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.SetShaderParameter(modelShader, camera);
				return true;
			});
			foreach (SubsystemModelsRenderer.ModelData modelData in modelsData)
			{
				ComponentModel componentModel = modelData.ComponentModel;
				Vector3 v = (componentModel.DiffuseColor != null) ? componentModel.DiffuseColor.Value : Vector3.One;
				float num = (componentModel.Opacity != null) ? componentModel.Opacity.Value : 1f;
				modelShader.InstancesCount = componentModel.AbsoluteBoneTransformsForCamera.Length;
				modelShader.MaterialColor = new Vector4(v * num, num);
				modelShader.EmissionColor = ((componentModel.EmissionColor != null) ? componentModel.EmissionColor.Value : Vector4.Zero);
				modelShader.AmbientLightColor = new Vector3(LightingManager.LightAmbient * modelData.Light);
				modelShader.DiffuseLightColor1 = new Vector3(modelData.Light);
				modelShader.DiffuseLightColor2 = new Vector3(modelData.Light);
				modelShader.Texture = componentModel.TextureOverride;
				Array.Copy(componentModel.AbsoluteBoneTransformsForCamera, modelShader.Transforms.World, componentModel.AbsoluteBoneTransformsForCamera.Length);
				InstancedModelData instancedModelData = InstancedModelsManager.GetInstancedModelData(componentModel.Model, componentModel.MeshDrawOrders);
				Display.DrawIndexed(PrimitiveType.TriangleList, modelShader, instancedModelData.VertexBuffer, instancedModelData.IndexBuffer, 0, instancedModelData.IndexBuffer.IndicesCount);
				this.ModelsDrawn++;
				ModsManager.HookAction("OnModelRendererDrawExtra", delegate(ModLoader modLoader)
				{
					modLoader.OnModelRendererDrawExtra(this, componentModel, camera, alphaThreshold);
					return false;
				});
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0004D498 File Offset: 0x0004B698
		public void DrawModelsExtras(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData)
		{
			foreach (SubsystemModelsRenderer.ModelData modelData in modelsData)
			{
				if (modelData.ComponentBody != null && modelData.ComponentModel.CastsShadow)
				{
					Vector3 shadowPosition = modelData.ComponentBody.Position + new Vector3(0f, 0.1f, 0f);
					BoundingBox boundingBox = modelData.ComponentBody.BoundingBox;
					float shadowDiameter = 2.25f * (boundingBox.Max.X - boundingBox.Min.X);
					this.m_subsystemShadows.QueueShadow(camera, shadowPosition, shadowDiameter, modelData.ComponentModel.Opacity ?? 1f);
				}
				modelData.ComponentModel.DrawExtras(camera);
			}
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0004D590 File Offset: 0x0004B790
		public float? CalculateModelLight(SubsystemModelsRenderer.ModelData modelData)
		{
			Vector3 p;
			if (modelData.ComponentBody != null)
			{
				p = modelData.ComponentBody.Position;
				p.Y += 0.95f * (modelData.ComponentBody.BoundingBox.Max.Y - modelData.ComponentBody.BoundingBox.Min.Y);
			}
			else
			{
				Matrix? boneTransform = modelData.ComponentModel.GetBoneTransform(modelData.ComponentModel.Model.RootBone.Index);
				p = ((boneTransform == null) ? Vector3.Zero : (boneTransform.Value.Translation + new Vector3(0f, 0.9f, 0f)));
			}
			return LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, p);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0004D658 File Offset: 0x0004B858
		public Vector3 SunVector(SubsystemSky subsystemSky)
		{
			float timeOfDay = subsystemSky.m_subsystemTimeOfDay.TimeOfDay;
			float num = 2f * timeOfDay * 3.14159274f;
			float x = num + 3.14159274f;
			float f = MathUtils.Max(SubsystemSky.CalculateDawnGlowIntensity(timeOfDay), SubsystemSky.CalculateDuskGlowIntensity(timeOfDay));
			float s = MathUtils.Lerp(90f, 160f, f);
			Vector3 vector = new Vector3
			{
				X = 0f - MathUtils.Sin(x),
				Y = 0f - MathUtils.Cos(x),
				Z = 0f
			};
			Vector3 unitZ = Vector3.UnitZ;
			Vector3 v = Vector3.Cross(unitZ, vector);
			return Vector3.Normalize(vector * 900f - s * unitZ - num * v);
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0004D728 File Offset: 0x0004B928
		public void ShadowDraw(SubsystemShadows subsystemShadows, Camera camera, Vector3 shadowPosition, float shadowDiameter, float alpha)
		{
			if (!SettingsManager.ObjectsShadowsEnabled)
			{
				return;
			}
			float num = Vector3.DistanceSquared(camera.ViewPosition, shadowPosition);
			if (num > 1024f)
			{
				return;
			}
			float num2 = MathUtils.Sqrt(num);
			float num3 = MathUtils.Saturate(4f * (1f - num2 / 32f));
			float num4 = shadowDiameter / 2f;
			int num5 = Terrain.ToCell(shadowPosition.X - num4);
			int num6 = Terrain.ToCell(shadowPosition.Z - num4);
			int num7 = Terrain.ToCell(shadowPosition.X + num4);
			int num8 = Terrain.ToCell(shadowPosition.Z + num4);
			for (int i = num5; i <= num7; i++)
			{
				for (int j = num6; j <= num8; j++)
				{
					int num9 = MathUtils.Min(Terrain.ToCell(shadowPosition.Y), 255);
					int num10 = MathUtils.Max(num9 - 2, 0);
					for (int k = num9; k >= num10; k--)
					{
						int cellValueFast = subsystemShadows.m_subsystemTerrain.Terrain.GetCellValueFast(i, k, j);
						int num11 = Terrain.ExtractContents(cellValueFast);
						Block block = BlocksManager.Blocks[num11];
						if (block.ObjectShadowStrength > 0f)
						{
							foreach (BoundingBox boundingBox in block.GetCustomCollisionBoxes(subsystemShadows.m_subsystemTerrain, cellValueFast))
							{
								float num12 = boundingBox.Max.Y + (float)k;
								if (shadowPosition.Y - num12 > -0.5f)
								{
									float num13 = camera.ViewPosition.Y - num12;
									if (num13 > 0f)
									{
										float num14 = MathUtils.Max(num13 * 0.01f, 0.005f);
										float num15 = MathUtils.Saturate(1f - (shadowPosition.Y - num12) / 2f);
										Vector3 p = new Vector3(boundingBox.Min.X + (float)i, num12 + num14, boundingBox.Min.Z + (float)j);
										Vector3 p2 = new Vector3(boundingBox.Max.X + (float)i, num12 + num14, boundingBox.Min.Z + (float)j);
										Vector3 p3 = new Vector3(boundingBox.Max.X + (float)i, num12 + num14, boundingBox.Max.Z + (float)j);
										Vector3 p4 = new Vector3(boundingBox.Min.X + (float)i, num12 + num14, boundingBox.Max.Z + (float)j);
										subsystemShadows.DrawShadowOverQuad(p, p2, p3, p4, shadowPosition, shadowDiameter, 0.45f * block.ObjectShadowStrength * alpha * num3 * num15);
									}
								}
							}
							break;
						}
						if (num11 == 18)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x0400058C RID: 1420
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400058D RID: 1421
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400058E RID: 1422
		public SubsystemShadows m_subsystemShadows;

		// Token: 0x0400058F RID: 1423
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000590 RID: 1424
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000591 RID: 1425
		public static ModelShader ShaderOpaque;

		// Token: 0x04000592 RID: 1426
		public static ModelShader ShaderAlphaTested;

		// Token: 0x04000593 RID: 1427
		private ModelShader m_shaderOpaque;

		// Token: 0x04000594 RID: 1428
		private ModelShader m_shaderAlphaTested;

		// Token: 0x04000595 RID: 1429
		private Vector3 m_sunLightDirection;

		// Token: 0x04000596 RID: 1430
		public int MaxInstancesCount;

		// Token: 0x04000597 RID: 1431
		public Dictionary<ComponentModel, SubsystemModelsRenderer.ModelData> m_componentModels = new Dictionary<ComponentModel, SubsystemModelsRenderer.ModelData>();

		// Token: 0x04000598 RID: 1432
		public List<SubsystemModelsRenderer.ModelData> m_modelsToPrepare = new List<SubsystemModelsRenderer.ModelData>();

		// Token: 0x04000599 RID: 1433
		public List<SubsystemModelsRenderer.ModelData>[] m_modelsToDraw = new List<SubsystemModelsRenderer.ModelData>[]
		{
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>()
		};

		// Token: 0x0400059A RID: 1434
		public static bool DisableDrawingModels;

		// Token: 0x0400059B RID: 1435
		public int ModelsDrawn;

		// Token: 0x0400059C RID: 1436
		public int[] m_drawOrders = new int[]
		{
			-10000,
			1,
			99,
			201
		};

		// Token: 0x02000497 RID: 1175
		public class ModelData : IComparable<SubsystemModelsRenderer.ModelData>
		{
			// Token: 0x06002093 RID: 8339 RVA: 0x000E86D0 File Offset: 0x000E68D0
			public int CompareTo(SubsystemModelsRenderer.ModelData other)
			{
				int num = (this.ComponentModel != null) ? this.ComponentModel.PrepareOrder : 0;
				int num2 = (other.ComponentModel != null) ? other.ComponentModel.PrepareOrder : 0;
				return num - num2;
			}

			// Token: 0x040016D7 RID: 5847
			public ComponentModel ComponentModel;

			// Token: 0x040016D8 RID: 5848
			public ComponentBody ComponentBody;

			// Token: 0x040016D9 RID: 5849
			public float Light;

			// Token: 0x040016DA RID: 5850
			public double NextLightTime;

			// Token: 0x040016DB RID: 5851
			public int LastAnimateFrame;
		}
	}
}
