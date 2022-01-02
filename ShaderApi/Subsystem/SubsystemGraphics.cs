using System;
using Engine;
using Engine.Graphics;
using TemplatesDatabase;
using GameEntitySystem;
using Game;


namespace ShaderApi
{
	public class SubsystemGraphics
	{
	    public SubsystemBlocksTexture m_subsystemBlocksTexture;
        
        public SubsystemSky m_subsystemSky;
        
        public SubsystemTerrain m_subsystemTerrain;
        
         private Texture2D shaderTexture;
        
        public Shader m_transparentShader;
        
        public PrimitivesRenderer2D m_primitiveRender;
        
        // Texture Effects
        public CustomUnlitShader m_textureShader;
        
        public RenderTarget2D m_textureWithEffects;
        public RenderTarget2D m_rainTexture;
        
      
        
        
        
        public SubsystemGraphics(Project project)
		{
		    m_textureShader = new CustomUnlitShader("TextureEffects", false, true, false);
            m_primitiveRender = new PrimitivesRenderer2D();
            
            m_subsystemBlocksTexture = project.FindSubsystem<SubsystemBlocksTexture>(true);
            
            
            m_subsystemTerrain = project.FindSubsystem<SubsystemTerrain>(true);
            
            m_transparentShader = new NTransparentShader();
            
            shaderTexture = ShaderReader.GetImage("shader_tex");
            
            m_subsystemSky = project.FindSubsystem<SubsystemSky>(true);
            
            m_textureWithEffects = GenerateShadedTexture(new Vector4(1f, 1f, 1f, 1f));
            
            m_rainTexture = GenerateShadedTexture(new Vector4(0.5f, 0.6f, 1f, 1f));
            
            m_subsystemBlocksTexture.BlocksTexture = m_textureWithEffects;
            
            
		}
        
        
        public RenderTarget2D GenerateShadedTexture(Vector4 ColorMask)
        {
            RenderTarget2D EffectTexture = new RenderTarget2D(m_subsystemBlocksTexture.BlocksTexture.Width, m_subsystemBlocksTexture.BlocksTexture.Height, 1, ColorFormat.Rgba8888, DepthFormat.None);
            
            
            RenderTarget2D OldRenderTarget = Display.RenderTarget;
            Display.RenderTarget = EffectTexture;
            
            
            
            TexturedBatch2D m_texturedBatch = m_primitiveRender.TexturedBatch(m_subsystemBlocksTexture.BlocksTexture);
			m_texturedBatch.QueueQuad(new Vector2(0f, 0f), new Vector2(m_subsystemBlocksTexture.BlocksTexture.Width, m_subsystemBlocksTexture.BlocksTexture.Height), 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), Color.White);
			Display.DepthStencilState = m_texturedBatch.DepthStencilState;
			Display.RasterizerState = m_texturedBatch.RasterizerState;
			Display.BlendState = BlendState.Opaque;
			
			
			
			m_textureShader.Texture = m_texturedBatch.Texture;
			m_textureShader.SamplerState = m_texturedBatch.SamplerState;
			m_textureShader.Transforms.World[0] = PrimitivesRenderer2D.ViewportMatrix();
			m_textureShader.Color = ColorMask;
			m_texturedBatch.FlushWithCurrentStateAndShader(m_textureShader, true);
			
            Display.RenderTarget = OldRenderTarget;
            
            return EffectTexture;
            
        }
        public void DisposeTerrainChunkGeometryVertexIndexBuffers(TerrainChunkGeometry geometry)
		{
			foreach (TerrainChunkGeometry.Buffer buffer in geometry.Buffers)
			{
				buffer.Dispose();
			}
			geometry.Buffers.Clear();
			geometry.InvalidateSliceContentsHashes();
		}
		public void SetupTerrainChunkGeometryVertexIndexBuffers(TerrainChunk chunk)
		{
		    float SkyLight = m_subsystemSky.CalculateLightIntensity(m_subsystemSky.m_subsystemTimeOfDay.TimeOfDay);
			TerrainChunkGeometry geometry = chunk.Geometry;
			DisposeTerrainChunkGeometryVertexIndexBuffers(geometry);
			var Draws = chunk.Draws;
			Draws.Clear();
			int i, j, k; int IndicesCount; int VerticesCount;
			for (i = 0; i < geometry.Slices.Length; i++)
			{
				var slice = geometry.Slices[i];
				foreach (var c in slice.Draws)
				{
					if (!Draws.TryGetValue(c.Key, out var v))
					{
						v = new TerrainGeometrySubset[7];
						for (j = 0; j < 7; j++) v[j] = new TerrainGeometrySubset();
						Draws.Add(c.Key, v);
					}
					VerticesCount = 0;
					for (j = 0; j < 7; j++)
					{
						var source = c.Value.Subsets[j];
						var target = v[j];
						if (target.Vertices.Count > 0)
						{
							for (k = 0; k < source.Indices.Count; k++)
							{
								target.Indices.Add((source.Indices[k] + target.Vertices.Count));//shift indices
							}
						}
						else target.Indices.AddRange(source.Indices);
						
						    for (int vi = 0; vi < source.Vertices.Count; vi++)
						    {
						        TerrainVertex vertex = source.Vertices[vi];
						        
						        float LightAmount = MathUtils.Max(((float) vertex.Color.R/(byte) 255), ((float) vertex.Color.G/(byte) 255), ((float) vertex.Color.B/(byte) 255));
						        
						        if (LightAmount > SkyLight && LightAmount - SkyLight > 0.235f)
						        {
						            vertex.Color *= new Color(1.0f, 0.5f, 0.0f);
						        }
						       
						       target.Vertices.Add(vertex);
						    }
						
					}
				}
			}
			foreach (var row in Draws)
			{
				TerrainChunkGeometry.Buffer buffer = new TerrainChunkGeometry.Buffer();
				TerrainGeometrySubset[] subsets = row.Value;
				IndicesCount = 0;VerticesCount = 0;
				for (j = 0; j < 7; j++)
				{
					var subset = subsets[j];
					if (subset.Indices.Count > 0)
					{
						buffer.SubsetIndexBufferStarts[j] = IndicesCount;
						buffer.SubsetVertexBufferStarts[j] = VerticesCount;
						IndicesCount += subset.Indices.Count;
						VerticesCount += subset.Vertices.Count;
						buffer.SubsetIndexBufferEnds[j] = IndicesCount;
					}
				}
				if (IndicesCount == 0) continue;
				buffer.Texture = row.Key;
				geometry.Buffers.Add(buffer);
				buffer.IndexBuffer = new IndexBuffer(IndexFormat.ThirtyTwoBits, IndicesCount);
				buffer.VertexBuffer = new VertexBuffer(TerrainVertex.VertexDeclaration, VerticesCount);
				VerticesCount = 0;
				for (j = 0; j < 7; j++)
				{
					var subset = subsets[j];
					if (subset.Indices.Count > 0)
					{
						if (VerticesCount > 0)
						{
							for (k = 0; k < subset.Indices.Count; k++)
							{
								subset.Indices[k] = ((subset.Indices[k] + VerticesCount));//shift indices
							}
						}
						buffer.VertexBuffer.SetData(subset.Vertices.Array, 0, subset.Vertices.Count, buffer.SubsetVertexBufferStarts[j]);
						buffer.IndexBuffer.SetData(subset.Indices.Array, 0, subset.Indices.Count, buffer.SubsetIndexBufferStarts[j]);
						VerticesCount += subset.Vertices.Count;
					}
				}
			}
			geometry.CopySliceContentsHashes(chunk);
		}
        public float m_lastSkyLight;
        public void Update(float dt)
        {
            if (m_subsystemSky.m_subsystemWeather.GlobalPrecipitationIntensity > 0.1f)
            {
                m_subsystemBlocksTexture.BlocksTexture = m_rainTexture;
            }
            else
            {
                m_subsystemBlocksTexture.BlocksTexture = m_textureWithEffects;
            }
            
		}
		public void PrepareForDrawing(TerrainRenderer terrainRender, Camera camera)
		{
		   Vector2 xZ = camera.ViewPosition.XZ;
			float num = MathUtils.Sqr(m_subsystemSky.VisibilityRange);
			BoundingFrustum viewFrustum = camera.ViewFrustum;
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			terrainRender.m_chunksToDraw.Clear();
			TerrainChunk[] allocatedChunks = m_subsystemTerrain.Terrain.AllocatedChunks;
			foreach (TerrainChunk terrainChunk in allocatedChunks)
			{
				if (terrainChunk.NewGeometryData)
				{
					lock (terrainChunk.Geometry)
					{
						if (terrainChunk.NewGeometryData)
						{
							terrainChunk.NewGeometryData = false;
							
							SetupTerrainChunkGeometryVertexIndexBuffers(terrainChunk);
						}
					}
				}
				terrainChunk.DrawDistanceSquared = Vector2.DistanceSquared(xZ, terrainChunk.Center);
				if (terrainChunk.DrawDistanceSquared <= num)
				{
					if (viewFrustum.Intersection(terrainChunk.BoundingBox))
					{
						terrainRender.m_chunksToDraw.Add(terrainChunk);
					}
					if (terrainChunk.State != TerrainChunkState.Valid)
					{
						continue;
					}
					float num2 = terrainChunk.FogEnds[gameWidgetIndex];
					if (num2 != 3.40282347E+38f)
					{
						if (num2 == 0f)
						{
							terrainRender.StartChunkFadeIn(camera, terrainChunk);
						}
						else
						{
							terrainRender.RunChunkFadeIn(camera, terrainChunk);
						}
					}
				}
				else
				{
					terrainChunk.FogEnds[gameWidgetIndex] = 0f;
				}
			}
			TerrainRenderer.ChunksDrawn = 0;
			TerrainRenderer.ChunkDrawCalls = 0;
			TerrainRenderer.ChunkTrianglesDrawn = 0;
		}
		
		
		
		public void Dispose()
		{
		    m_textureWithEffects.Dispose();
		    m_rainTexture.Dispose();
		    
		}
		public void DrawTransparent(Camera camera, TerrainRenderer terrainRend) {
            int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
            Vector3 viewPosition = camera.ViewPosition;
            Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
            Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
            Display.BlendState = BlendState.AlphaBlend;
            Display.DepthStencilState = DepthStencilState.Default;
            Display.RasterizerState = ((terrainRend.m_subsystemSky.ViewUnderWaterDepth > 0f) ? RasterizerState.CullClockwiseScissor : RasterizerState.CullCounterClockwiseScissor);
            m_transparentShader.GetParameter("u_origin", true).SetValue(v.XZ);
            m_transparentShader.GetParameter("u_viewProjectionMatrix", true).SetValue(value);
            m_transparentShader.GetParameter("u_viewPosition", true).SetValue(viewPosition);
            m_transparentShader.GetParameter("u_fogYMultiplier", true).SetValue(terrainRend.m_subsystemSky.VisibilityRangeYMultiplier);
            m_transparentShader.GetParameter("u_fogColor", true).SetValue(new Vector3(terrainRend.m_subsystemSky.ViewFogColor));
            
            
            m_transparentShader.GetParameter("u_screen", true).SetValue(SubsystemReflections.Screen);
            m_transparentShader.GetParameter("u_screenSampler", true).SetValue(SettingsManager.TerrainMipmapsEnabled ? terrainRend.m_samplerStateMips : terrainRend.m_samplerState);
            
            if (camera is BasePerspectiveCamera)
            {
            try {m_transparentShader.GetParameter("u_viewDir", true).SetValue(((BasePerspectiveCamera) camera).ViewDirection);} catch {}
            }
            
            try { m_transparentShader.GetParameter("u_screenSize", true).SetValue(new Vector2((float)Display.Viewport.Width, (float)Display.Viewport.Height)); } catch {}
            
            
             
            m_transparentShader.GetParameter("u_shaderTex", true).SetValue(shaderTexture);
            m_transparentShader.GetParameter("u_shaderTexSampler", true).SetValue(SettingsManager.TerrainMipmapsEnabled ? terrainRend.m_samplerStateMips : terrainRend.m_samplerState);


            ShaderParameter parameter = m_transparentShader.GetParameter("u_fogStartInvLength", true);
            for (int i = 0; i < terrainRend.m_chunksToDraw.Count; i++) {
                TerrainChunk terrainChunk = terrainRend.m_chunksToDraw[i];
                float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], terrainRend.m_subsystemSky.ViewFogRange.Y);
                float num2 = MathUtils.Min(terrainRend.m_subsystemSky.ViewFogRange.X, num - 1f);
                parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
                
                terrainRend.DrawTerrainChunkGeometrySubsets(m_transparentShader, terrainChunk.Geometry, 64, false);
            }
        }
	}
}
