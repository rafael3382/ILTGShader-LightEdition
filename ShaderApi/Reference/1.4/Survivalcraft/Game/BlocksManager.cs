using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000363 RID: 867
	public static class BlocksManager
	{
		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06001945 RID: 6469 RVA: 0x000C70F8 File Offset: 0x000C52F8
		public static Block[] Blocks
		{
			get
			{
				return BlocksManager.m_blocks;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001946 RID: 6470 RVA: 0x000C70FF File Offset: 0x000C52FF
		public static FluidBlock[] FluidBlocks
		{
			get
			{
				return BlocksManager.m_fluidBlocks;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x000C7106 File Offset: 0x000C5306
		public static ReadOnlyList<string> Categories
		{
			get
			{
				return new ReadOnlyList<string>(BlocksManager.m_categories);
			}
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x000C7114 File Offset: 0x000C5314
		public static void Initialize()
		{
			for (int i = 0; i < BlocksManager.m_blocks.Length; i++)
			{
				BlocksManager.m_blocks[i] = null;
			}
			BlocksManager.m_categories.Clear();
			BlocksManager.m_categories.Add("Terrain");
			BlocksManager.m_categories.Add("Plants");
			BlocksManager.m_categories.Add("Construction");
			BlocksManager.m_categories.Add("Items");
			BlocksManager.m_categories.Add("Tools");
			BlocksManager.m_categories.Add("Weapons");
			BlocksManager.m_categories.Add("Clothes");
			BlocksManager.m_categories.Add("Electrics");
			BlocksManager.m_categories.Add("Food");
			BlocksManager.m_categories.Add("Spawner Eggs");
			BlocksManager.m_categories.Add("Painted");
			BlocksManager.m_categories.Add("Dyed");
			BlocksManager.m_categories.Add("Fireworks");
			BlocksManager.CalculateSlotTexCoordTables();
			foreach (ModEntity modEntity in ModsManager.ModList)
			{
				for (int j = 0; j < modEntity.Blocks.Count; j++)
				{
					Block block = modEntity.Blocks[j];
					BlocksManager.m_blocks[block.BlockIndex] = block;
					if (block is FluidBlock)
					{
						BlocksManager.m_fluidBlocks[block.BlockIndex] = (block as FluidBlock);
					}
				}
			}
			for (int k = 0; k < BlocksManager.m_blocks.Length; k++)
			{
				if (BlocksManager.m_blocks[k] == null)
				{
					BlocksManager.m_blocks[k] = BlocksManager.Blocks[0];
				}
			}
			foreach (ModEntity modEntity2 in ModsManager.ModList)
			{
				modEntity2.LoadBlocksData();
			}
			for (int l = 0; l < BlocksManager.m_blocks.Length; l++)
			{
				Block block2 = BlocksManager.m_blocks[l];
				try
				{
					block2.Initialize();
				}
				catch (Exception ex)
				{
					LoadingScreen.Warning("Loading Block " + block2.GetType().FullName + " errro." + ex.Message);
				}
				foreach (int value in block2.GetCreativeValues())
				{
					BlocksManager.AddCategory(block2.GetCategory(value));
				}
			}
			ModsManager.HookAction("BlocksInitalized", delegate(ModLoader modLoader)
			{
				modLoader.BlocksInitalized();
				return false;
			});
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x000C73E4 File Offset: 0x000C55E4
		public static void AddCategory(string category)
		{
			if (!BlocksManager.m_categories.Contains(category))
			{
				BlocksManager.m_categories.Add(category);
			}
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x000C7400 File Offset: 0x000C5600
		public static Block FindBlockByTypeName(string typeName, bool throwIfNotFound)
		{
			Block block = BlocksManager.Blocks.FirstOrDefault((Block b) => b.GetType().Name == typeName);
			if (block == null && throwIfNotFound)
			{
				throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 1), typeName));
			}
			return block;
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x000C7454 File Offset: 0x000C5654
		public static Block[] FindBlocksByCraftingId(string craftingId)
		{
			List<Block> list = new List<Block>();
			foreach (Block block in BlocksManager.Blocks)
			{
				if (block.MatchCrafingId(craftingId))
				{
					list.Add(block);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x000C7495 File Offset: 0x000C5695
		public static void DrawCubeBlock(PrimitivesRenderer3D primitivesRenderer, int value, Vector3 size, ref Matrix matrix, Color color, Color topColor, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, size, ref matrix, color, topColor, environmentData, (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture);
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000C74C8 File Offset: 0x000C56C8
		public static void DrawCubeBlock(PrimitivesRenderer3D primitivesRenderer, int value, Vector3 size, ref Matrix matrix, Color color, Color topColor, DrawBlockEnvironmentData environmentData, Texture2D texture)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			float s = LightingManager.LightIntensityByLightValue[environmentData.Light];
			color = Color.MultiplyColorOnly(color, s);
			topColor = Color.MultiplyColorOnly(topColor, s);
			Vector3 translation = matrix.Translation;
			Vector3 vector = matrix.Right * size.X;
			Vector3 v = matrix.Up * size.Y;
			Vector3 v2 = matrix.Forward * size.Z;
			Vector3 p = translation + 0.5f * (-vector - v - v2);
			Vector3 vector2 = translation + 0.5f * (vector - v - v2);
			Vector3 vector3 = translation + 0.5f * (-vector + v - v2);
			Vector3 vector4 = translation + 0.5f * (vector + v - v2);
			Vector3 vector5 = translation + 0.5f * (-vector - v + v2);
			Vector3 vector6 = translation + 0.5f * (vector - v + v2);
			Vector3 vector7 = translation + 0.5f * (-vector + v + v2);
			Vector3 p2 = translation + 0.5f * (vector + v + v2);
			if (environmentData.ViewProjectionMatrix != null)
			{
				Matrix value2 = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref p, ref value2, out p);
				Vector3.Transform(ref vector2, ref value2, out vector2);
				Vector3.Transform(ref vector3, ref value2, out vector3);
				Vector3.Transform(ref vector4, ref value2, out vector4);
				Vector3.Transform(ref vector5, ref value2, out vector5);
				Vector3.Transform(ref vector6, ref value2, out vector6);
				Vector3.Transform(ref vector7, ref value2, out vector7);
				Vector3.Transform(ref p2, ref value2, out p2);
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			int textureSlotCount = block.GetTextureSlotCount(value);
			Vector4[] array;
			if (textureSlotCount != 16)
			{
				array = BlocksManager.GetslotTexCoords(textureSlotCount);
			}
			else
			{
				array = BlocksManager.m_slotTexCoords;
			}
			Vector4 vector8 = array[block.GetFaceTextureSlot(0, value)];
			Color color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Forward));
			texturedBatch3D.QueueQuad(p, vector3, vector4, vector2, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
			vector8 = array[block.GetFaceTextureSlot(2, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Forward));
			texturedBatch3D.QueueQuad(vector5, vector6, p2, vector7, new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), color2);
			vector8 = array[block.GetFaceTextureSlot(5, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Up));
			texturedBatch3D.QueueQuad(p, vector2, vector6, vector5, new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), color2);
			vector8 = array[block.GetFaceTextureSlot(4, value)];
			color2 = Color.MultiplyColorOnly(topColor, LightingManager.CalculateLighting(matrix.Up));
			texturedBatch3D.QueueQuad(vector3, vector7, p2, vector4, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
			vector8 = array[block.GetFaceTextureSlot(1, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Right));
			texturedBatch3D.QueueQuad(p, vector5, vector7, vector3, new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), color2);
			vector8 = array[block.GetFaceTextureSlot(3, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Right));
			texturedBatch3D.QueueQuad(vector2, vector4, p2, vector6, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x000C7A28 File Offset: 0x000C5C28
		public static void DrawFlatBlock(PrimitivesRenderer3D primitivesRenderer, int value, float size, ref Matrix matrix, Texture2D texture, Color color, bool isEmissive, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			if (!isEmissive)
			{
				float s = LightingManager.LightIntensityByLightValue[environmentData.Light];
				color = Color.MultiplyColorOnly(color, s);
			}
			Vector3 translation = matrix.Translation;
			Vector3 vector;
			Vector3 v;
			if (environmentData.BillboardDirection != null)
			{
				vector = Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, Vector3.UnitY));
				v = -Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, vector));
			}
			else
			{
				vector = matrix.Right;
				v = matrix.Up;
			}
			Vector3 p = translation + 0.85f * size * (-vector - v);
			Vector3 vector2 = translation + 0.85f * size * (vector - v);
			Vector3 vector3 = translation + 0.85f * size * (-vector + v);
			Vector3 p2 = translation + 0.85f * size * (vector + v);
			if (environmentData.ViewProjectionMatrix != null)
			{
				Matrix value2 = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref p, ref value2, out p);
				Vector3.Transform(ref vector2, ref value2, out vector2);
				Vector3.Transform(ref vector3, ref value2, out vector3);
				Vector3.Transform(ref p2, ref value2, out p2);
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			Vector4 vector4;
			if (texture != null)
			{
				vector4 = new Vector4(0f, 0f, 1f, 1f);
			}
			else
			{
				texture = ((environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture);
				vector4 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(-1, value)];
			}
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			texturedBatch3D.QueueQuad(p, vector3, p2, vector2, new Vector2(vector4.X, vector4.W), new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.Z, vector4.W), color);
			if (environmentData.BillboardDirection == null)
			{
				texturedBatch3D.QueueQuad(p, vector2, p2, vector3, new Vector2(vector4.X, vector4.W), new Vector2(vector4.Z, vector4.W), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.X, vector4.Y), color);
			}
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x000C7CC0 File Offset: 0x000C5EC0
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			Texture2D texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			BlocksManager.DrawMeshBlock(primitivesRenderer, blockMesh, texture, Color.White, size, ref matrix, environmentData);
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x000C7D10 File Offset: 0x000C5F10
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			Texture2D texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			BlocksManager.DrawMeshBlock(primitivesRenderer, blockMesh, texture, color, size, ref matrix, environmentData);
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x000C7D5C File Offset: 0x000C5F5C
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, Texture2D texture, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			float num = LightingManager.LightIntensityByLightValue[environmentData.Light];
			Vector4 vector = new Vector4(color);
			vector.X *= num;
			vector.Y *= num;
			vector.Z *= num;
			bool flag = vector == Vector4.One;
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			bool flag2 = false;
			Matrix matrix2 = (environmentData.ViewProjectionMatrix == null) ? matrix : (matrix * environmentData.ViewProjectionMatrix.Value);
			if (size != 1f)
			{
				matrix2 = Matrix.CreateScale(size) * matrix2;
			}
			if (matrix2.M14 != 0f || matrix2.M24 != 0f || matrix2.M34 != 0f || matrix2.M44 != 1f)
			{
				flag2 = true;
			}
			int count = blockMesh.Vertices.Count;
			BlockMeshVertex[] array = blockMesh.Vertices.Array;
			int count2 = blockMesh.Indices.Count;
			ushort[] array2 = blockMesh.Indices.Array;
			DynamicArray<VertexPositionColorTexture> triangleVertices = texturedBatch3D.TriangleVertices;
			int count3 = triangleVertices.Count;
			int count4 = triangleVertices.Count;
			triangleVertices.Count += count;
			for (int i = 0; i < count; i++)
			{
				BlockMeshVertex blockMeshVertex = array[i];
				if (flag2)
				{
					Vector4 vector2 = new Vector4(blockMeshVertex.Position, 1f);
					Vector4.Transform(ref vector2, ref matrix2, out vector2);
					float num2 = 1f / vector2.W;
					blockMeshVertex.Position = new Vector3(vector2.X * num2, vector2.Y * num2, vector2.Z * num2);
				}
				else
				{
					Vector3.Transform(ref blockMeshVertex.Position, ref matrix2, out blockMeshVertex.Position);
				}
				if (flag || blockMeshVertex.IsEmissive)
				{
					triangleVertices.Array[count4++] = new VertexPositionColorTexture(blockMeshVertex.Position, blockMeshVertex.Color, blockMeshVertex.TextureCoordinates);
				}
				else
				{
					Color color2 = new Color((byte)((float)blockMeshVertex.Color.R * vector.X), (byte)((float)blockMeshVertex.Color.G * vector.Y), (byte)((float)blockMeshVertex.Color.B * vector.Z), (byte)((float)blockMeshVertex.Color.A * vector.W));
					triangleVertices.Array[count4++] = new VertexPositionColorTexture(blockMeshVertex.Position, color2, blockMeshVertex.TextureCoordinates);
				}
			}
			DynamicArray<ushort> triangleIndices = texturedBatch3D.TriangleIndices;
			int count5 = triangleIndices.Count;
			triangleIndices.Count += count2;
			for (int j = 0; j < count2; j++)
			{
				triangleIndices.Array[count5++] = (ushort)(count3 + (int)array2[j]);
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x000C8058 File Offset: 0x000C6258
		public static int DamageItem(int value, int damageCount)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.Durability < 0)
			{
				return value;
			}
			int num2 = block.GetDamage(value) + damageCount;
			if (num2 <= block.Durability)
			{
				return block.SetDamage(value, num2);
			}
			return block.GetDamageDestructionValue(value);
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x000C80A4 File Offset: 0x000C62A4
		public static void LoadBlocksData(string data)
		{
			Dictionary<Block, bool> dictionary = new Dictionary<Block, bool>();
			data = data.Replace("\r", string.Empty);
			string[] array = data.Split(new char[]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					string[] array3 = array[i].Split(new char[]
					{
						';'
					});
					if (i == 0)
					{
						array2 = new string[array3.Length - 1];
						Array.Copy(array3, 1, array2, 0, array3.Length - 1);
					}
					else
					{
						if (array3.Length != array2.Length + 1)
						{
							throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 2), (array3.Length != 0) ? array3[0] : LanguageControl.Unknown));
						}
						string typeName = array3[0];
						if (!string.IsNullOrEmpty(typeName))
						{
							Block block = BlocksManager.m_blocks.FirstOrDefault((Block v) => v.GetType().Name == typeName);
							if (block == null)
							{
								throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 3), typeName));
							}
							dictionary.Add(block, true);
							Dictionary<string, FieldInfo> dictionary2 = new Dictionary<string, FieldInfo>();
							foreach (FieldInfo fieldInfo in block.GetType().GetRuntimeFields())
							{
								if (fieldInfo.IsPublic && !fieldInfo.IsStatic)
								{
									dictionary2.Add(fieldInfo.Name, fieldInfo);
								}
							}
							for (int j = 1; j < array3.Length; j++)
							{
								string text = array2[j - 1];
								string text2 = array3[j];
								if (!string.IsNullOrEmpty(text2))
								{
									FieldInfo fieldInfo2;
									if (!dictionary2.TryGetValue(text, out fieldInfo2))
									{
										throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 5), text));
									}
									object value;
									if (text2.StartsWith("#"))
									{
										string refTypeName = text2.Substring(1);
										object obj;
										if (string.IsNullOrEmpty(refTypeName))
										{
											obj = block.BlockIndex;
										}
										else
										{
											Block block2 = BlocksManager.m_blocks.FirstOrDefault((Block v) => v.GetType().Name == refTypeName);
											if (block2 == null)
											{
												throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 6), refTypeName));
											}
											obj = block2.BlockIndex;
										}
										value = obj;
									}
									else
									{
										value = HumanReadableConverter.ConvertFromString(fieldInfo2.FieldType, text2);
									}
									fieldInfo2.SetValue(block, value);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x000C833C File Offset: 0x000C653C
		public static void CalculateSlotTexCoordTables()
		{
			for (int i = 0; i < 256; i++)
			{
				BlocksManager.m_slotTexCoords[i] = BlocksManager.TextureSlotToTextureCoords(i);
			}
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x000C836C File Offset: 0x000C656C
		public static Vector4 TextureSlotToTextureCoords(int slot)
		{
			int num = slot % 16;
			int num2 = slot / 16;
			float x = ((float)num + 0.001f) / 16f;
			float y = ((float)num2 + 0.001f) / 16f;
			float z = ((float)(num + 1) - 0.001f) / 16f;
			float w = ((float)(num2 + 1) - 0.001f) / 16f;
			return new Vector4(x, y, z, w);
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x000C83CC File Offset: 0x000C65CC
		public static Vector4[] GetslotTexCoords(int textureSlotCount)
		{
			int num = textureSlotCount * textureSlotCount;
			Vector4[] array = new Vector4[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i % textureSlotCount;
				int num3 = i / textureSlotCount;
				float x = ((float)num2 + 0.001f) / (float)textureSlotCount;
				float y = ((float)num3 + 0.001f) / (float)textureSlotCount;
				float z = ((float)(num2 + 1) - 0.001f) / (float)textureSlotCount;
				float w = ((float)(num3 + 1) - 0.001f) / (float)textureSlotCount;
				array[i] = new Vector4(x, y, z, w);
			}
			return array;
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000C8444 File Offset: 0x000C6644
		public static Block GetBlock(string ModSpace, string TypeFullName)
		{
			ModEntity modEntity;
			if (ModsManager.GetModEntity(ModSpace, out modEntity))
			{
				Block block = modEntity.Blocks.Find((Block p) => p.GetType().Name == TypeFullName);
				if (block != null)
				{
					return block;
				}
			}
			return null;
		}

		// Token: 0x04001174 RID: 4468
		public static Block[] m_blocks = new Block[1024];

		// Token: 0x04001175 RID: 4469
		public static FluidBlock[] m_fluidBlocks = new FluidBlock[1024];

		// Token: 0x04001176 RID: 4470
		public static List<string> m_categories = new List<string>();

		// Token: 0x04001177 RID: 4471
		public static DrawBlockEnvironmentData m_defaultEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04001178 RID: 4472
		public static Vector4[] m_slotTexCoords = new Vector4[256];
	}
}
