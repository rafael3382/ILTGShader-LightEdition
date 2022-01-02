using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System;
using System.IO;
using System.Collections.Generic;
using TemplatesDatabase;
using Game;


namespace ShaderApi
{
    public class SubsystemModelsRenderer : Game.SubsystemModelsRenderer, IDrawable
    {
        public static NModelShader m_NshaderOpaque;

        public static  NModelShader m_NshaderAlphaTested;
        
        public static Vector3 sunPos;
        
       
       
        public SubsystemTimeOfDay m_subsystemTimeOfDay;

        public override void Load(ValuesDictionary valuesDictionary)
        {
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
            m_subsystemSky = Project.FindSubsystem<SubsystemSky>(throwOnError: true);
            m_subsystemShadows = Project.FindSubsystem<SubsystemShadows>(throwOnError: true);
            m_subsystemTimeOfDay = Project.FindSubsystem<SubsystemTimeOfDay>(throwOnError: true);

            int MaxInstancesCount = valuesDictionary.GetValue<int>("maxInstancesCount", 14);;
            
            
            m_NshaderOpaque = new NModelShader(useAlphaThreshold: false, MaxInstancesCount);
            m_NshaderAlphaTested = new NModelShader(useAlphaThreshold: true, MaxInstancesCount);
            
            
     
           

            
			
    }
       public void Draw(Camera camera, int drawOrder)
        {
            
            float angle = (2f * m_subsystemSky.m_subsystemTimeOfDay.TimeOfDay * 3.14159274f) + 3.14159274f;
            
            sunPos = new Vector3
				{
					X = 0f - MathUtils.Sin(angle),
					Y = 0f - MathUtils.Cos(angle),
					Z = 0f
				};
            if (drawOrder == m_drawOrders[0])
            {
                ModelsDrawn = 0;
                List<ModelData>[] modelsToDraw = m_modelsToDraw;
                for (int i = 0; i < modelsToDraw.Length; i++)
                {
                    modelsToDraw[i].Clear();
                }
                m_modelsToPrepare.Clear();
                foreach (ModelData value in m_componentModels.Values)
                {
                    if (value.ComponentModel.Model != null)
                    {
                        value.ComponentModel.CalculateIsVisible(camera);
                        if (value.ComponentModel.IsVisibleForCamera)
                        {
                            m_modelsToPrepare.Add(value);
                        }
                    }
                }
                m_modelsToPrepare.Sort();
                foreach (ModelData item in m_modelsToPrepare)
                {
                    PrepareModel(item, camera);
                    m_modelsToDraw[(int)item.ComponentModel.RenderingMode].Add(item);
                }
            }
            else if (!DisableDrawingModels)
            {
                if (drawOrder == m_drawOrders[1])
                {
                    Display.DepthStencilState = DepthStencilState.Default;
                    Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
                    Display.BlendState = BlendState.Opaque;
                    NDrawModels(camera, m_modelsToDraw[0], null);
                    Display.RasterizerState = RasterizerState.CullNoneScissor;
                    NDrawModels(camera, m_modelsToDraw[1], 0f);
                    Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
                    m_primitivesRenderer.Flush(camera.ProjectionMatrix, clearAfterFlush: true, 0);
                }
                else if (drawOrder == m_drawOrders[2])
                {
                    Display.DepthStencilState = DepthStencilState.Default;
                    Display.RasterizerState = RasterizerState.CullNoneScissor;
                    Display.BlendState = BlendState.AlphaBlend;
                    NDrawModels(camera, m_modelsToDraw[2], null);
                }
                else if (drawOrder == m_drawOrders[3])
                {
                    Display.DepthStencilState = DepthStencilState.Default;
                    Display.RasterizerState = RasterizerState.CullNoneScissor;
                    Display.BlendState = BlendState.AlphaBlend;
                    NDrawModels(camera, m_modelsToDraw[3], null);
                    
                    m_primitivesRenderer.Flush(camera.ProjectionMatrix);
                }
            }
            else
            {
                m_primitivesRenderer.Clear();
            }
        }
        public void NDrawModels(Camera camera, List<ModelData> modelsData, float? alphaThreshold)
        {
            NDrawInstancedModels(camera, modelsData, alphaThreshold);
            NDrawModelsExtras(camera, modelsData);
        }
        public void NDrawModelsExtras(Camera camera, List<ModelData> modelsData)
        {
            
            
            foreach (ModelData modelsDatum in modelsData)
            {
                
                if (modelsDatum.ComponentBody != null && modelsDatum.ComponentModel.CastsShadow)
                {
                    Vector3 shadowPosition = modelsDatum.ComponentBody.Position + new Vector3(0f, 0.1f, 0f);
                    BoundingBox boundingBox = modelsDatum.ComponentBody.BoundingBox;
                    float shadowDiameter = 2.25f * (boundingBox.Max.X - boundingBox.Min.X);
                    m_subsystemShadows.QueueShadow(camera, shadowPosition, shadowDiameter, modelsDatum.ComponentModel.Opacity ?? 1f);
                    //(camera, shadowPosition, shadowDiameter, modelsDatum.ComponentModel.Opacity ?? 1f)
                    
                    //NDrawInstancedModels(camera, new List<ModelData>()  { modelsDatum }, 0.5f, null);
                    
                }
                modelsDatum.ComponentModel.DrawExtras(camera);
            }
        }
        public void NDrawInstancedModels(Camera camera, List<ModelData> modelsData, float? alphaThreshold)
        {
          
			NModelShader modelShader = (alphaThreshold != null) ? SubsystemModelsRenderer.m_NshaderAlphaTested : SubsystemModelsRenderer.m_NshaderOpaque;
			modelShader.GetParameter("u_time").SetValue(m_subsystemSky.m_subsystemTimeOfDay.TimeOfDay);
			modelShader.LightDirection1 = 1.25f * Vector3.TransformNormal(sunPos, camera.ViewMatrix);
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
			}
        }
        public Matrix CalculateBaseProjectionMatrix(Point2 ViewSize)
		{
			
				float num = 90f;
				float num2 = 0.9f;
				
				
				float num3 = ViewSize.X / ViewSize.Y;
				float num4 = MathUtils.Min(num * num3, num);
				float num5 = num4 * num3;
				if (num5 < 90f)
				{
					num4 *= 90f / num5;
				}
				else if (num5 > 175f)
				{
					num4 *= 175f / num5;
				}
				return Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(num4 * num2), num3, 0.1f, 2048f);
			
		}
        

       public Vector3? get_shadow(Camera camera, Vector3 shadowPosition, float shadowDiameter, float alpha) {
            
            /*if (!SettingsManager.ObjectsShadowsEnabled)
            {
                return null;
            }
            float num = Vector3.DistanceSquared(camera.ViewPosition, shadowPosition);
            if (!(num <= 1024f))
            {
                return null;
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
                    for (int num11 = num9; num11 >= num10; num11--)
                    {
                        int cellValueFast = m_subsystemTerrain.Terrain.GetCellValueFast(i, num11, j);
                        int num12 = Terrain.ExtractContents(cellValueFast);
                        Block block = BlocksManager.Blocks[num12];
                        if (block.GetObjectShadowStrength(cellValueFast) > 0f)
                        {
                            BoundingBox[] customCollisionBoxes = block.GetCustomCollisionBoxes(m_subsystemTerrain, cellValueFast);
                            for (int k = 0; k < customCollisionBoxes.Length; k++)
                            {
                                BoundingBox boundingBox = customCollisionBoxes[k];
                                float num13 = boundingBox.Max.Y + num11;
                                if (shadowPosition.Y - num13 > -0.5f)
                                {
                                    float num14 = camera.ViewPosition.Y - num13;
                                    if (num14 > 0f)
                                    {
                                        float num15 = MathUtils.Max(num14 * 0.01f, 0.005f);
                                        float num16 = MathUtils.Saturate(1f - (shadowPosition.Y - num13) / 2f);
                                        var p = new Vector3(boundingBox.Min.X + i, num13 + num15, boundingBox.Min.Z + j);
                                        var p2 = new Vector3(boundingBox.Max.X + i, num13 + num15, boundingBox.Min.Z + j);
                                        var p3 = new Vector3(boundingBox.Max.X + i, num13 + num15, boundingBox.Max.Z + j);
                                        var p4 = new Vector3(boundingBox.Min.X + i, num13 + num15, boundingBox.Max.Z + j);
                                        //DrawShadowOverQuad(p, p2, p3, p4, shadowPosition, shadowDiameter, 0.45f * block.GetObjectShadowStrength(cellValueFast) * alpha * num3 * num16);
                                    }
                                }
                            }
                            break;
                        }
                        if (num12 == 18)
                        {
                            break;
                        }
                    }
                }
            }*/
            return null;
        }
        
    }
}
