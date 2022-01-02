using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000202 RID: 514
	public class ComponentFirstPersonModel : Component, IDrawable, IUpdateable
	{
		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000F00 RID: 3840 RVA: 0x0006DBDC File Offset: 0x0006BDDC
		// (set) Token: 0x06000F01 RID: 3841 RVA: 0x0006DBE4 File Offset: 0x0006BDE4
		public Vector3 ItemOffsetOrder { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000F02 RID: 3842 RVA: 0x0006DBED File Offset: 0x0006BDED
		// (set) Token: 0x06000F03 RID: 3843 RVA: 0x0006DBF5 File Offset: 0x0006BDF5
		public Vector3 ItemRotationOrder { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x0006DBFE File Offset: 0x0006BDFE
		public int[] DrawOrders
		{
			get
			{
				return ComponentFirstPersonModel.m_drawOrders;
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000F05 RID: 3845 RVA: 0x0006DC05 File Offset: 0x0006BE05
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.FirstPersonModels;
			}
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0006DC0C File Offset: 0x0006BE0C
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentPlayer.ComponentHealth.Health > 0f && camera.GameWidget.IsEntityFirstPersonTarget(base.Entity))
			{
				Viewport viewport = Display.Viewport;
				Viewport viewport2 = viewport;
				viewport2.MaxDepth *= 0.1f;
				Display.Viewport = viewport2;
				try
				{
					Matrix matrix = Matrix.Identity;
					if (this.m_swapAnimationTime > 0f)
					{
						float num = MathUtils.Pow(MathUtils.Sin(this.m_swapAnimationTime * 3.14159274f), 3f);
						matrix *= Matrix.CreateTranslation(0f, -0.8f * num, 0.2f * num);
					}
					if (this.m_pokeAnimationTime > 0f)
					{
						float num2 = MathUtils.Sin(MathUtils.Sqrt(this.m_pokeAnimationTime) * 3.14159274f);
						if (this.m_value != 0)
						{
							matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(90f)) * num2);
							matrix *= Matrix.CreateTranslation(-0.5f * num2, 0.1f * num2, 0f * num2);
						}
						else
						{
							matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(45f)) * num2);
							matrix *= Matrix.CreateTranslation(-0.1f * num2, 0.2f * num2, -0.05f * num2);
						}
					}
					if (this.m_componentRider.Mount != null)
					{
						ComponentCreatureModel componentCreatureModel = this.m_componentRider.Mount.Entity.FindComponent<ComponentCreatureModel>();
						if (componentCreatureModel != null)
						{
							float num3 = componentCreatureModel.MovementAnimationPhase * 3.14159274f * 2f + 0.5f;
							Vector3 position = default(Vector3);
							position.Y = 0.02f * MathUtils.Sin(num3);
							position.Z = 0.02f * MathUtils.Sin(num3);
							matrix *= Matrix.CreateRotationX(0.05f * MathUtils.Sin(num3 * 1f)) * Matrix.CreateTranslation(position);
						}
					}
					else
					{
						float num4 = this.m_componentPlayer.ComponentCreatureModel.MovementAnimationPhase * 3.14159274f * 2f;
						Vector3 vector = default(Vector3);
						vector.X = 0.03f * MathUtils.Sin(num4 * 1f);
						vector.Y = 0.02f * MathUtils.Sin(num4 * 2f);
						vector.Z = 0.02f * MathUtils.Sin(num4 * 1f);
						matrix *= Matrix.CreateRotationZ(1f * vector.X) * Matrix.CreateTranslation(vector);
					}
					Vector3 eyePosition = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
					int x = Terrain.ToCell(eyePosition.X);
					int num5 = Terrain.ToCell(eyePosition.Y);
					int z = Terrain.ToCell(eyePosition.Z);
					Matrix m = Matrix.CreateFromQuaternion(this.m_componentPlayer.ComponentCreatureModel.EyeRotation);
					m.Translation = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
					if (this.m_value != 0)
					{
						if (num5 >= 0 && num5 <= 255)
						{
							TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
							if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1)
							{
								this.m_itemLight = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5, z);
							}
						}
						int num6 = Terrain.ExtractContents(this.m_value);
						Block block = BlocksManager.Blocks[num6];
						Vector3 vector2 = block.FirstPersonRotation * 0.0174532924f + this.m_itemRotation;
						Vector3 vector3 = block.GetFirstPersonOffset(this.m_value) + this.m_itemOffset;
						vector3 += this.m_itemOffset;
						Matrix inWorldMatrix = Matrix.CreateFromYawPitchRoll(vector2.Y, vector2.X, vector2.Z) * matrix * Matrix.CreateTranslation(vector3) * Matrix.CreateFromYawPitchRoll(this.m_lagAngles.X, this.m_lagAngles.Y, 0f) * m;
						this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
						this.m_drawBlockEnvironmentData.InWorldMatrix = inWorldMatrix;
						this.m_drawBlockEnvironmentData.Light = this.m_itemLight;
						this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
						this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num5);
						block.DrawBlock(this.m_primitivesRenderer, this.m_value, Color.White, block.GetFirstPersonScale(this.m_value), ref inWorldMatrix, this.m_drawBlockEnvironmentData);
						this.PrimitiveRender.Flush(this.m_primitivesRenderer, camera.ViewProjectionMatrix, true, int.MaxValue);
					}
					else
					{
						if (Time.FrameStartTime >= this.m_nextHandLightTime)
						{
							float? num7 = LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, eyePosition);
							if (num7 != null)
							{
								this.m_nextHandLightTime = Time.FrameStartTime + 0.1;
								this.m_handLight = num7.Value;
							}
						}
						Vector3 position2 = new Vector3(0.25f, -0.3f, -0.05f);
						Matrix matrix2 = Matrix.CreateScale(0.01f) * Matrix.CreateRotationX(0.8f) * Matrix.CreateRotationY(0.4f) * matrix * Matrix.CreateTranslation(position2) * Matrix.CreateFromYawPitchRoll(this.m_lagAngles.X, this.m_lagAngles.Y, 0f) * m * camera.ViewMatrix;
						Display.DepthStencilState = DepthStencilState.Default;
						Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
						ComponentFirstPersonModel.LitShader.Texture = this.m_componentPlayer.ComponentCreatureModel.TextureOverride;
						ComponentFirstPersonModel.LitShader.SamplerState = SamplerState.PointClamp;
						ComponentFirstPersonModel.LitShader.MaterialColor = Vector4.One;
						ComponentFirstPersonModel.LitShader.AmbientLightColor = new Vector3(this.m_handLight * LightingManager.LightAmbient);
						ComponentFirstPersonModel.LitShader.DiffuseLightColor1 = new Vector3(this.m_handLight);
						ComponentFirstPersonModel.LitShader.DiffuseLightColor2 = new Vector3(this.m_handLight);
						ComponentFirstPersonModel.LitShader.LightDirection1 = Vector3.TransformNormal(LightingManager.DirectionToLight1, camera.ViewMatrix);
						ComponentFirstPersonModel.LitShader.LightDirection2 = Vector3.TransformNormal(LightingManager.DirectionToLight2, camera.ViewMatrix);
						ComponentFirstPersonModel.LitShader.Transforms.World[0] = matrix2;
						ComponentFirstPersonModel.LitShader.Transforms.View = Matrix.Identity;
						ComponentFirstPersonModel.LitShader.Transforms.Projection = camera.ProjectionMatrix;
						foreach (ModelMesh modelMesh in this.m_handModel.Meshes)
						{
							foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
							{
								Display.DrawIndexed(PrimitiveType.TriangleList, ComponentFirstPersonModel.LitShader, modelMeshPart.VertexBuffer, modelMeshPart.IndexBuffer, modelMeshPart.StartIndex, modelMeshPart.IndicesCount);
							}
						}
					}
				}
				finally
				{
					Display.Viewport = viewport;
				}
			}
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0006E3A8 File Offset: 0x0006C5A8
		public void Update(float dt)
		{
			Vector3 vector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.ToYawPitchRoll();
			this.m_lagAngles *= MathUtils.Pow(0.2f, dt);
			if (this.m_lastYpr != null)
			{
				Vector3 vector2 = vector - this.m_lastYpr.Value;
				this.m_lagAngles.X = MathUtils.Clamp(this.m_lagAngles.X - 0.08f * MathUtils.NormalizeAngle(vector2.X), -0.1f, 0.1f);
				this.m_lagAngles.Y = MathUtils.Clamp(this.m_lagAngles.Y - 0.08f * MathUtils.NormalizeAngle(vector2.Y), -0.1f, 0.1f);
			}
			this.m_lastYpr = new Vector3?(vector);
			int activeBlockValue = this.m_componentMiner.ActiveBlockValue;
			if (this.m_swapAnimationTime == 0f && activeBlockValue != this.m_value)
			{
				if (BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)].IsSwapAnimationNeeded(this.m_value, activeBlockValue))
				{
					this.m_swapAnimationTime = 0.0001f;
				}
				else
				{
					this.m_value = activeBlockValue;
				}
			}
			if (this.m_swapAnimationTime > 0f)
			{
				float swapAnimationTime = this.m_swapAnimationTime;
				this.m_swapAnimationTime += 2f * dt;
				if (swapAnimationTime < 0.5f && this.m_swapAnimationTime >= 0.5f)
				{
					this.m_value = activeBlockValue;
				}
				if (this.m_swapAnimationTime > 1f)
				{
					this.m_swapAnimationTime = 0f;
				}
			}
			this.m_pokeAnimationTime = this.m_componentMiner.PokingPhase;
			this.m_itemOffset = Vector3.Lerp(this.m_itemOffset, this.ItemOffsetOrder, MathUtils.Saturate(10f * dt));
			this.m_itemRotation = Vector3.Lerp(this.m_itemRotation, this.ItemRotationOrder, MathUtils.Saturate(10f * dt));
			this.ItemOffsetOrder = Vector3.Zero;
			this.ItemRotationOrder = Vector3.Zero;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0006E5A0 File Offset: 0x0006C7A0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_componentRider = base.Entity.FindComponent<ComponentRider>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_handModel = ContentManager.Get<Model>(valuesDictionary.GetValue<string>("HandModelName"), null);
			this.PrimitiveRender.Shader = ComponentFirstPersonModel.UnlitShader;
			this.PrimitiveRender.ShaderAlphaTest = ComponentFirstPersonModel.UnlitShaderAlphaTest;
		}

		// Token: 0x04000855 RID: 2133
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000856 RID: 2134
		public ComponentMiner m_componentMiner;

		// Token: 0x04000857 RID: 2135
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000858 RID: 2136
		public ComponentRider m_componentRider;

		// Token: 0x04000859 RID: 2137
		public int m_value;

		// Token: 0x0400085A RID: 2138
		public Model m_handModel;

		// Token: 0x0400085B RID: 2139
		public Vector3? m_lastYpr;

		// Token: 0x0400085C RID: 2140
		public Vector2 m_lagAngles;

		// Token: 0x0400085D RID: 2141
		public float m_swapAnimationTime;

		// Token: 0x0400085E RID: 2142
		public float m_pokeAnimationTime;

		// Token: 0x0400085F RID: 2143
		public Vector3 m_itemOffset;

		// Token: 0x04000860 RID: 2144
		public Vector3 m_itemRotation;

		// Token: 0x04000861 RID: 2145
		public double m_nextHandLightTime;

		// Token: 0x04000862 RID: 2146
		public float m_handLight;

		// Token: 0x04000863 RID: 2147
		public int m_itemLight;

		// Token: 0x04000864 RID: 2148
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04000865 RID: 2149
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000866 RID: 2150
		public PrimitiveRender PrimitiveRender = new PrimitiveRender();

		// Token: 0x04000867 RID: 2151
		public static UnlitShader UnlitShader = new UnlitShader(ShaderCodeManager.GetFast("Shaders/Unlit.vsh"), ShaderCodeManager.GetFast("Shaders/Unlit.psh"), true, true, false);

		// Token: 0x04000868 RID: 2152
		public static UnlitShader UnlitShaderAlphaTest = new UnlitShader(ShaderCodeManager.GetFast("Shaders/Unlit.vsh"), ShaderCodeManager.GetFast("Shaders/Unlit.psh"), true, true, true);

		// Token: 0x04000869 RID: 2153
		public static LitShader LitShader = new LitShader(ShaderCodeManager.GetFast("Shaders/Lit.vsh"), ShaderCodeManager.GetFast("Shaders/Lit.psh"), 2, false, false, true, false, false, 1);

		// Token: 0x0400086A RID: 2154
		public static int[] m_drawOrders = new int[]
		{
			1
		};
	}
}
