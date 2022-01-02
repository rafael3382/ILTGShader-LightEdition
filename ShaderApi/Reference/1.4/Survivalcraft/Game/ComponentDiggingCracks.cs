using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FC RID: 508
	public class ComponentDiggingCracks : Component, IDrawable
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0006B7D5 File Offset: 0x000699D5
		public int[] DrawOrders
		{
			get
			{
				return ComponentDiggingCracks.m_drawOrders;
			}
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0006B7DC File Offset: 0x000699DC
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentMiner.DigCellFace == null || this.m_componentMiner.DigProgress <= 0f || this.m_componentMiner.DigTime <= 0.2f)
			{
				return;
			}
			Point3 point = this.m_componentMiner.DigCellFace.Value.Point;
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			if (this.m_geometry == null || cellValue != this.m_value || point != this.m_point)
			{
				this.m_geometry = new ComponentDiggingCracks.Geometry();
				this.m_geometry.ClearSubsets(base.Project.FindSubsystem<SubsystemAnimatedTextures>());
				block.GenerateTerrainVertices(this.m_subsystemTerrain.BlockGeometryGenerator, this.m_geometry, cellValue, point.X, point.Y, point.Z);
				this.textureSlotCount = block.GetTextureSlotCount(cellValue);
				this.textureSlotSize = 32 * this.textureSlotCount;
				this.m_point = point;
				this.m_value = cellValue;
				DynamicArray<TerrainVertex> dynamicArray = new DynamicArray<TerrainVertex>();
				DynamicArray<ushort> dynamicArray2 = new DynamicArray<ushort>();
				foreach (KeyValuePair<Texture2D, TerrainGeometry> keyValuePair in this.m_geometry.Draws)
				{
					for (int i = 0; i < keyValuePair.Value.Subsets.Length; i++)
					{
						TerrainGeometrySubset terrainGeometrySubset = keyValuePair.Value.Subsets[i];
						if (terrainGeometrySubset.Indices.Count > 0)
						{
							for (int j = 0; j < terrainGeometrySubset.Indices.Count; j++)
							{
								dynamicArray2.Add((ushort)(terrainGeometrySubset.Indices[j] + dynamicArray.Count));
							}
							for (int k = 0; k < terrainGeometrySubset.Vertices.Count; k++)
							{
								TerrainVertex item = terrainGeometrySubset.Vertices[k];
								byte b = (item.Color.R + item.Color.G + item.Color.B) / 3;
								item.Color = new Color(b, b, b, 128);
								dynamicArray.Add(item);
							}
						}
					}
				}
				TerrainChunkGeometry.Buffer buffer = this.Buffer;
				if (buffer != null)
				{
					buffer.Dispose();
				}
				this.Buffer = new TerrainChunkGeometry.Buffer();
				this.Buffer.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, dynamicArray2.Count);
				this.Buffer.VertexBuffer = new VertexBuffer(TerrainVertex.VertexDeclaration, dynamicArray.Count);
				this.Buffer.IndexBuffer.SetData<ushort>(dynamicArray2.Array, 0, dynamicArray2.Count, 0);
				this.Buffer.VertexBuffer.SetData<TerrainVertex>(dynamicArray.Array, 0, dynamicArray.Count, 0);
			}
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			float x = this.m_subsystemSky.ViewFogRange.X;
			float y = this.m_subsystemSky.ViewFogRange.Y;
			int num2 = MathUtils.Clamp((int)(this.m_componentMiner.DigProgress * 8f), 0, 7);
			RenderTarget2D[] array;
			if (!this.CrackTextures.TryGetValue(this.textureSlotSize, out array))
			{
				array = new RenderTarget2D[8];
				this.CrackTextures.Add(this.textureSlotSize, array);
			}
			if (array[num2] == null)
			{
				RenderTarget2D renderTarget = Display.RenderTarget;
				RenderTarget2D renderTarget2D = new RenderTarget2D(this.textureSlotSize, this.textureSlotSize, 1, ColorFormat.Rgba8888, DepthFormat.None);
				Display.RenderTarget = renderTarget2D;
				PrimitivesRenderer2D primitivesRenderer2D = new PrimitivesRenderer2D();
				TexturedBatch2D texturedBatch2D = primitivesRenderer2D.TexturedBatch(this.m_textures[num2], true, 0, null, null, null, null);
				for (int l = 0; l < this.textureSlotCount; l++)
				{
					for (int m = 0; m < this.textureSlotCount; m++)
					{
						Vector2 corner = new Vector2((float)(l * 32), (float)(m * 32));
						Vector2 corner2 = new Vector2((float)((l + 1) * 32), (float)((m + 1) * 32));
						texturedBatch2D.QueueQuad(corner, corner2, 1f, Vector2.Zero, Vector2.One, Color.White);
					}
				}
				primitivesRenderer2D.Flush(true, int.MaxValue);
				Display.RenderTarget = renderTarget;
				array[num2] = renderTarget2D;
			}
			Display.BlendState = BlendState.NonPremultiplied;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			this.m_shader.GetParameter("u_origin", false).SetValue(v.XZ);
			this.m_shader.GetParameter("u_texture", false).SetValue(array[num2]);
			this.m_shader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			this.m_shader.GetParameter("u_viewPosition", false).SetValue(camera.ViewPosition);
			this.m_shader.GetParameter("u_samplerState", false).SetValue(SamplerState.PointWrap);
			this.m_shader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			this.m_shader.GetParameter("u_fogStartInvLength", false).SetValue(new Vector2(x, 1f / (y - x)));
			Display.DrawIndexed(PrimitiveType.TriangleList, this.m_shader, this.Buffer.VertexBuffer, this.Buffer.IndexBuffer, 0, this.Buffer.IndexBuffer.IndicesCount);
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0006BDC4 File Offset: 0x00069FC4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_shader = ContentManager.Get<Shader>("Shaders/AlphaTested", null);
			this.m_textures = new Texture2D[8];
			for (int i = 0; i < 8; i++)
			{
				this.m_textures[i] = ContentManager.Get<Texture2D>(string.Format("Textures/Cracks{0}", i + 1), null);
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0006BE50 File Offset: 0x0006A050
		public override void Dispose()
		{
			foreach (KeyValuePair<int, RenderTarget2D[]> keyValuePair in this.CrackTextures)
			{
				foreach (RenderTarget2D renderTarget2D in keyValuePair.Value)
				{
					if (renderTarget2D != null)
					{
						renderTarget2D.Dispose();
					}
				}
			}
		}

		// Token: 0x04000805 RID: 2053
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000806 RID: 2054
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000807 RID: 2055
		public ComponentMiner m_componentMiner;

		// Token: 0x04000808 RID: 2056
		public Texture2D[] m_textures;

		// Token: 0x04000809 RID: 2057
		public Shader m_shader;

		// Token: 0x0400080A RID: 2058
		public ComponentDiggingCracks.Geometry m_geometry;

		// Token: 0x0400080B RID: 2059
		public Point3 m_point;

		// Token: 0x0400080C RID: 2060
		public int m_value;

		// Token: 0x0400080D RID: 2061
		public static int[] m_drawOrders = new int[]
		{
			1
		};

		// Token: 0x0400080E RID: 2062
		public TerrainChunkGeometry.Buffer Buffer;

		// Token: 0x0400080F RID: 2063
		public int textureSlotCount;

		// Token: 0x04000810 RID: 2064
		public int textureSlotSize;

		// Token: 0x04000811 RID: 2065
		public Dictionary<int, RenderTarget2D[]> CrackTextures = new Dictionary<int, RenderTarget2D[]>();

		// Token: 0x020004C4 RID: 1220
		public class Geometry : TerrainGeometry
		{
			// Token: 0x06002100 RID: 8448 RVA: 0x000E9908 File Offset: 0x000E7B08
			public Geometry()
			{
				TerrainGeometrySubset terrainGeometrySubset = this.SubsetTransparent = (this.SubsetAlphaTest = (this.SubsetOpaque = new TerrainGeometrySubset()));
				this.OpaqueSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.AlphaTestSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.TransparentSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
			}
		}
	}
}
