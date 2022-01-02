using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000096 RID: 150
	public class IvyBlock : Block
	{
		// Token: 0x060002FF RID: 767 RVA: 0x00012B14 File Offset: 0x00010D14
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int face = IvyBlock.GetFace(Terrain.ExtractData(value));
			if (face >= 0 && face < 4)
			{
				return this.m_boundingBoxes[face];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00012B48 File Offset: 0x00010D48
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			BlockPlacementData result = default(BlockPlacementData);
			if (raycastResult.CellFace.Face < 4)
			{
				result.CellFace = raycastResult.CellFace;
				result.Value = Terrain.MakeBlockValue(197, 0, IvyBlock.SetFace(0, CellFace.OppositeFace(raycastResult.CellFace.Face)));
			}
			return result;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00012BA4 File Offset: 0x00010DA4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			TerrainGeometrySubset subsetAlphaTest = geometry.SubsetAlphaTest;
			DynamicArray<TerrainVertex> vertices = subsetAlphaTest.Vertices;
			DynamicArray<int> indices = subsetAlphaTest.Indices;
			int count = vertices.Count;
			int data = Terrain.ExtractData(value);
			int num = Terrain.ExtractLight(value);
			int face = IvyBlock.GetFace(data);
			float s = LightingManager.LightIntensityByLightValueAndFace[num + 16 * CellFace.OppositeFace(face)];
			Color color = BlockColorsMap.IvyColorsMap.Lookup(generator.Terrain, x, y, z) * s;
			color.A = byte.MaxValue;
			switch (face)
			{
			case 0:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 3]);
				indices.Add(count);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count + 2);
				indices.Add(count + 1);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 3);
				indices.Add(count);
				indices.Add(count);
				indices.Add(count + 3);
				indices.Add(count + 2);
				return;
			case 1:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 3]);
				indices.Add(count);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count + 2);
				indices.Add(count + 1);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 3);
				indices.Add(count);
				indices.Add(count);
				indices.Add(count + 3);
				indices.Add(count + 2);
				return;
			case 2:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)y, (float)z, color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)(x + 1), (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 3]);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 1);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 3);
				indices.Add(count + 3);
				indices.Add(count);
				indices.Add(count + 2);
				return;
			case 3:
				vertices.Count += 4;
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)z, color, this.DefaultTextureSlot, 0, ref vertices.Array[count]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)z, color, this.DefaultTextureSlot, 3, ref vertices.Array[count + 1]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)(y + 1), (float)(z + 1), color, this.DefaultTextureSlot, 2, ref vertices.Array[count + 2]);
				BlockGeometryGenerator.SetupLitCornerVertex((float)x, (float)y, (float)(z + 1), color, this.DefaultTextureSlot, 1, ref vertices.Array[count + 3]);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 1);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 3);
				indices.Add(count + 3);
				indices.Add(count);
				indices.Add(count + 2);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00013079 File Offset: 0x00011279
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= BlockColorsMap.IvyColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000303 RID: 771 RVA: 0x000130AC File Offset: 0x000112AC
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.IvyColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.DefaultTextureSlot);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00013101 File Offset: 0x00011301
		public static int GetFace(int data)
		{
			return data & 3;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00013106 File Offset: 0x00011306
		public static int SetFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00013110 File Offset: 0x00011310
		public static bool IsGrowthStopCell(int x, int y, int z)
		{
			return MathUtils.Hash((uint)(x + y * 451 + z * 77437)) % 5U == 0U;
		}

		// Token: 0x04000160 RID: 352
		public const int Index = 197;

		// Token: 0x04000161 RID: 353
		public BoundingBox[][] m_boundingBoxes = new BoundingBox[][]
		{
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0.9375f), new Vector3(1f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0.9375f, 0f, 0f), new Vector3(1f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.0625f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.0625f, 1f, 1f))
			}
		};
	}
}
