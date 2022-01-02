using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000325 RID: 805
	public class TerrainRenderer : IDisposable
	{
		// Token: 0x17000387 RID: 903
		// (get) Token: 0x060017F3 RID: 6131 RVA: 0x000BB7E8 File Offset: 0x000B99E8
		public string ChunksGpuMemoryUsage
		{
			get
			{
				long num = 0L;
				foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
				{
					if (terrainChunk.Geometry != null)
					{
						foreach (TerrainChunkGeometry.Buffer buffer in terrainChunk.Geometry.Buffers)
						{
							long num2 = num;
							VertexBuffer vertexBuffer = buffer.VertexBuffer;
							num = num2 + (long)((vertexBuffer != null) ? vertexBuffer.GetGpuMemoryUsage() : 0);
							long num3 = num;
							IndexBuffer indexBuffer = buffer.IndexBuffer;
							num = num3 + (long)((indexBuffer != null) ? indexBuffer.GetGpuMemoryUsage() : 0);
						}
					}
				}
				return string.Format("{0:0.0}MB", num / 1024L / 1024L);
			}
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x000BB8B8 File Offset: 0x000B9AB8
		public TerrainRenderer(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_subsystemSky = subsystemTerrain.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemAnimatedTextures = subsystemTerrain.SubsystemAnimatedTextures;
			if (TerrainRenderer.OpaqueShader == null)
			{
				TerrainRenderer.OpaqueShader = new Shader(ShaderCodeManager.GetFast("Shaders/Opaque.vsh"), ShaderCodeManager.GetFast("Shaders/Opaque.psh"), new ShaderMacro[]
				{
					new ShaderMacro("Opaque")
				});
			}
			if (TerrainRenderer.AlphatestedShader == null)
			{
				TerrainRenderer.AlphatestedShader = new Shader(ShaderCodeManager.GetFast("Shaders/AlphaTested.vsh"), ShaderCodeManager.GetFast("Shaders/AlphaTested.psh"), new ShaderMacro[]
				{
					new ShaderMacro("ALPHATESTED")
				});
			}
			if (TerrainRenderer.TransparentShader == null)
			{
				TerrainRenderer.TransparentShader = new Shader(ShaderCodeManager.GetFast("Shaders/Transparent.vsh"), ShaderCodeManager.GetFast("Shaders/Transparent.psh"), new ShaderMacro[]
				{
					new ShaderMacro("Transparent")
				});
			}
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x000BBA0C File Offset: 0x000B9C0C
		public void PrepareForDrawing(Camera camera)
		{
			Vector2 xz = camera.ViewPosition.XZ;
			float num = MathUtils.Sqr(this.m_subsystemSky.VisibilityRange);
			BoundingFrustum viewFrustum = camera.ViewFrustum;
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			this.m_chunksToDraw.Clear();
			foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
			{
				if (terrainChunk.NewGeometryData)
				{
					TerrainChunkGeometry geometry = terrainChunk.Geometry;
					lock (geometry)
					{
						if (terrainChunk.NewGeometryData)
						{
							terrainChunk.NewGeometryData = false;
							this.SetupTerrainChunkGeometryVertexIndexBuffers(terrainChunk);
						}
					}
				}
				terrainChunk.DrawDistanceSquared = Vector2.DistanceSquared(xz, terrainChunk.Center);
				if (terrainChunk.DrawDistanceSquared <= num)
				{
					if (viewFrustum.Intersection(terrainChunk.BoundingBox))
					{
						this.m_chunksToDraw.Add(terrainChunk);
					}
					if (terrainChunk.State == TerrainChunkState.Valid)
					{
						float num2 = terrainChunk.FogEnds[gameWidgetIndex];
						if (num2 != 3.40282347E+38f)
						{
							if (num2 == 0f)
							{
								this.StartChunkFadeIn(camera, terrainChunk);
							}
							else
							{
								this.RunChunkFadeIn(camera, terrainChunk);
							}
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

		// Token: 0x060017F6 RID: 6134 RVA: 0x000BBB80 File Offset: 0x000B9D80
		public void DrawOpaque(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.Opaque;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			TerrainRenderer.OpaqueShader.GetParameter("u_origin", false).SetValue(v.XZ);
			TerrainRenderer.OpaqueShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			TerrainRenderer.OpaqueShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			TerrainRenderer.OpaqueShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			TerrainRenderer.OpaqueShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			TerrainRenderer.OpaqueShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = TerrainRenderer.OpaqueShader.GetParameter("u_fogStartInvLength", false);
			ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.SetShaderParameter(TerrainRenderer.OpaqueShader, camera);
				return true;
			});
			Point2 point = Terrain.ToChunk(camera.ViewPosition.XZ);
			this.m_subsystemTerrain.Terrain.GetChunkAtCoords(point.X, point.Y);
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int num3 = 16;
				if (viewPosition.Z > terrainChunk.BoundingBox.Min.Z)
				{
					num3 |= 1;
				}
				if (viewPosition.X > terrainChunk.BoundingBox.Min.X)
				{
					num3 |= 2;
				}
				if (viewPosition.Z < terrainChunk.BoundingBox.Max.Z)
				{
					num3 |= 4;
				}
				if (viewPosition.X < terrainChunk.BoundingBox.Max.X)
				{
					num3 |= 8;
				}
				this.DrawTerrainChunkGeometrySubsets(TerrainRenderer.OpaqueShader, terrainChunk.Geometry, num3, true);
				TerrainRenderer.ChunksDrawn++;
			}
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x000BBE70 File Offset: 0x000BA070
		public void DrawAlphaTested(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.Opaque;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			TerrainRenderer.AlphatestedShader.GetParameter("u_origin", false).SetValue(v.XZ);
			TerrainRenderer.AlphatestedShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			TerrainRenderer.AlphatestedShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			TerrainRenderer.AlphatestedShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			TerrainRenderer.AlphatestedShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			TerrainRenderer.AlphatestedShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = TerrainRenderer.AlphatestedShader.GetParameter("u_fogStartInvLength", false);
			ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.SetShaderParameter(TerrainRenderer.AlphatestedShader, camera);
				return true;
			});
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int subsetsMask = 32;
				this.DrawTerrainChunkGeometrySubsets(TerrainRenderer.AlphatestedShader, terrainChunk.Geometry, subsetsMask, true);
			}
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x000BC0A0 File Offset: 0x000BA2A0
		public void DrawTransparent(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.AlphaBlend;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = ((this.m_subsystemSky.ViewUnderWaterDepth > 0f) ? RasterizerState.CullClockwiseScissor : RasterizerState.CullCounterClockwiseScissor);
			TerrainRenderer.TransparentShader.GetParameter("u_origin", false).SetValue(v.XZ);
			TerrainRenderer.TransparentShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			TerrainRenderer.TransparentShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			TerrainRenderer.TransparentShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			TerrainRenderer.TransparentShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			TerrainRenderer.TransparentShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = TerrainRenderer.TransparentShader.GetParameter("u_fogStartInvLength", false);
			ModsManager.HookAction("SetShaderParameter", delegate(ModLoader modLoader)
			{
				modLoader.SetShaderParameter(TerrainRenderer.TransparentShader, camera);
				return true;
			});
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int subsetsMask = 64;
				this.DrawTerrainChunkGeometrySubsets(TerrainRenderer.TransparentShader, terrainChunk.Geometry, subsetsMask, true);
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000BC2E7 File Offset: 0x000BA4E7
		public void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000BC2FC File Offset: 0x000BA4FC
		public void Display_DeviceReset()
		{
			this.m_subsystemTerrain.TerrainUpdater.DowngradeAllChunksState(TerrainChunkState.InvalidVertices1, false);
			foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
			{
				this.DisposeTerrainChunkGeometryVertexIndexBuffers(terrainChunk.Geometry);
			}
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x000BC34C File Offset: 0x000BA54C
		public void DisposeTerrainChunkGeometryVertexIndexBuffers(TerrainChunkGeometry geometry)
		{
			foreach (TerrainChunkGeometry.Buffer buffer in geometry.Buffers)
			{
				buffer.Dispose();
			}
			geometry.Buffers.Clear();
			geometry.InvalidateSliceContentsHashes();
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x000BC3B0 File Offset: 0x000BA5B0
		public void SetupTerrainChunkGeometryVertexIndexBuffers(TerrainChunk chunk)
		{
			TerrainChunkGeometry geometry = chunk.Geometry;
			this.DisposeTerrainChunkGeometryVertexIndexBuffers(geometry);
			Dictionary<Texture2D, TerrainGeometrySubset[]> draws = chunk.Draws;
			draws.Clear();
			for (int i = 0; i < geometry.Slices.Length; i++)
			{
				foreach (KeyValuePair<Texture2D, TerrainGeometry> keyValuePair in geometry.Slices[i].Draws)
				{
					TerrainGeometrySubset[] array;
					if (!draws.TryGetValue(keyValuePair.Key, out array))
					{
						array = new TerrainGeometrySubset[7];
						for (int j = 0; j < 7; j++)
						{
							array[j] = new TerrainGeometrySubset();
						}
						draws.Add(keyValuePair.Key, array);
					}
					for (int j = 0; j < 7; j++)
					{
						TerrainGeometrySubset terrainGeometrySubset = keyValuePair.Value.Subsets[j];
						TerrainGeometrySubset terrainGeometrySubset2 = array[j];
						if (terrainGeometrySubset2.Vertices.Count > 0)
						{
							for (int k = 0; k < terrainGeometrySubset.Indices.Count; k++)
							{
								terrainGeometrySubset2.Indices.Add(terrainGeometrySubset.Indices[k] + terrainGeometrySubset2.Vertices.Count);
							}
						}
						else
						{
							terrainGeometrySubset2.Indices.AddRange(terrainGeometrySubset.Indices);
						}
						terrainGeometrySubset2.Vertices.AddRange(terrainGeometrySubset.Vertices);
					}
				}
			}
			foreach (KeyValuePair<Texture2D, TerrainGeometrySubset[]> keyValuePair2 in draws)
			{
				TerrainChunkGeometry.Buffer buffer = new TerrainChunkGeometry.Buffer();
				TerrainGeometrySubset[] value = keyValuePair2.Value;
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < 7; j++)
				{
					TerrainGeometrySubset terrainGeometrySubset3 = value[j];
					if (terrainGeometrySubset3.Indices.Count > 0)
					{
						buffer.SubsetIndexBufferStarts[j] = num;
						buffer.SubsetVertexBufferStarts[j] = num2;
						num += terrainGeometrySubset3.Indices.Count;
						num2 += terrainGeometrySubset3.Vertices.Count;
						buffer.SubsetIndexBufferEnds[j] = num;
					}
				}
				if (num != 0)
				{
					buffer.Texture = keyValuePair2.Key;
					geometry.Buffers.Add(buffer);
					buffer.IndexBuffer = new IndexBuffer(IndexFormat.ThirtyTwoBits, num);
					buffer.VertexBuffer = new VertexBuffer(TerrainVertex.VertexDeclaration, num2);
					num2 = 0;
					for (int j = 0; j < 7; j++)
					{
						TerrainGeometrySubset terrainGeometrySubset4 = value[j];
						if (terrainGeometrySubset4.Indices.Count > 0)
						{
							if (num2 > 0)
							{
								for (int k = 0; k < terrainGeometrySubset4.Indices.Count; k++)
								{
									terrainGeometrySubset4.Indices[k] = terrainGeometrySubset4.Indices[k] + num2;
								}
							}
							buffer.VertexBuffer.SetData<TerrainVertex>(terrainGeometrySubset4.Vertices.Array, 0, terrainGeometrySubset4.Vertices.Count, buffer.SubsetVertexBufferStarts[j]);
							buffer.IndexBuffer.SetData<int>(terrainGeometrySubset4.Indices.Array, 0, terrainGeometrySubset4.Indices.Count, buffer.SubsetIndexBufferStarts[j]);
							num2 += terrainGeometrySubset4.Vertices.Count;
						}
					}
				}
			}
			geometry.CopySliceContentsHashes(chunk);
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000BC714 File Offset: 0x000BA914
		public void DrawTerrainChunkGeometrySubsets(Shader shader, TerrainChunkGeometry geometry, int subsetsMask, bool ApplyTexture = true)
		{
			foreach (TerrainChunkGeometry.Buffer buffer in geometry.Buffers)
			{
				int num = int.MaxValue;
				int num2 = 0;
				for (int i = 0; i < 8; i++)
				{
					if (i < 7 && (subsetsMask & 1 << i) != 0)
					{
						if (buffer.SubsetIndexBufferEnds[i] > 0)
						{
							if (num == 2147483647)
							{
								num = buffer.SubsetIndexBufferStarts[i];
							}
							num2 = buffer.SubsetIndexBufferEnds[i];
						}
					}
					else
					{
						if (num2 > num)
						{
							if (ApplyTexture)
							{
								shader.GetParameter("u_texture", false).SetValue(buffer.Texture);
							}
							Display.DrawIndexed(PrimitiveType.TriangleList, shader, buffer.VertexBuffer, buffer.IndexBuffer, num, num2 - num);
							TerrainRenderer.ChunkTrianglesDrawn += (num2 - num) / 3;
							TerrainRenderer.ChunkDrawCalls++;
						}
						num = int.MaxValue;
					}
				}
			}
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x000BC818 File Offset: 0x000BAA18
		public void StartChunkFadeIn(Camera camera, TerrainChunk chunk)
		{
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)chunk.Origin.X, (float)chunk.Origin.Y);
			Vector2 v2 = new Vector2((float)(chunk.Origin.X + 16), (float)chunk.Origin.Y);
			Vector2 v3 = new Vector2((float)chunk.Origin.X, (float)(chunk.Origin.Y + 16));
			Vector2 v4 = new Vector2((float)(chunk.Origin.X + 16), (float)(chunk.Origin.Y + 16));
			float x = Vector2.Distance(viewPosition.XZ, v);
			float x2 = Vector2.Distance(viewPosition.XZ, v2);
			float x3 = Vector2.Distance(viewPosition.XZ, v3);
			float x4 = Vector2.Distance(viewPosition.XZ, v4);
			chunk.FogEnds[camera.GameWidget.GameWidgetIndex] = MathUtils.Max(MathUtils.Min(x, x2, x3, x4), 0.001f);
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000BC91C File Offset: 0x000BAB1C
		public void RunChunkFadeIn(Camera camera, TerrainChunk chunk)
		{
			chunk.FogEnds[camera.GameWidget.GameWidgetIndex] += 32f * Time.FrameDuration;
			if (chunk.FogEnds[camera.GameWidget.GameWidgetIndex] >= this.m_subsystemSky.ViewFogRange.Y)
			{
				chunk.FogEnds[camera.GameWidget.GameWidgetIndex] = float.MaxValue;
			}
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x000BC98C File Offset: 0x000BAB8C
		public static void ShiftIndices(int[] source, int[] destination, int shift, int count)
		{
			for (int i = 0; i < count; i++)
			{
				destination[i] = source[i] + shift;
			}
		}

		// Token: 0x040010A4 RID: 4260
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040010A5 RID: 4261
		public SubsystemSky m_subsystemSky;

		// Token: 0x040010A6 RID: 4262
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x040010A7 RID: 4263
		public static Shader OpaqueShader;

		// Token: 0x040010A8 RID: 4264
		public static Shader AlphatestedShader;

		// Token: 0x040010A9 RID: 4265
		public static Shader TransparentShader;

		// Token: 0x040010AA RID: 4266
		public SamplerState m_samplerState = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.Point,
			MaxLod = 0f
		};

		// Token: 0x040010AB RID: 4267
		public SamplerState m_samplerStateMips = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.PointMipLinear,
			MaxLod = 4f
		};

		// Token: 0x040010AC RID: 4268
		public DynamicArray<TerrainChunk> m_chunksToDraw = new DynamicArray<TerrainChunk>();

		// Token: 0x040010AD RID: 4269
		public static DynamicArray<ushort> m_tmpIndices = new DynamicArray<ushort>();

		// Token: 0x040010AE RID: 4270
		public static bool DrawChunksMap;

		// Token: 0x040010AF RID: 4271
		public static int ChunksDrawn;

		// Token: 0x040010B0 RID: 4272
		public static int ChunkDrawCalls;

		// Token: 0x040010B1 RID: 4273
		public static int ChunkTrianglesDrawn;
	}
}
