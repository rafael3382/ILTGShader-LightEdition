using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200010F RID: 271
	public class TargetBlock : MountedElectricElementBlock
	{
		// Token: 0x06000541 RID: 1345 RVA: 0x0001E0C0 File Offset: 0x0001C2C0
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = TargetBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= 0 && mountingFace < 4)
			{
				return this.m_boundingBoxes[mountingFace];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001E0F4 File Offset: 0x0001C2F4
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			BlockPlacementData result = default(BlockPlacementData);
			if (raycastResult.CellFace.Face < 4)
			{
				result.CellFace = raycastResult.CellFace;
				result.Value = Terrain.MakeBlockValue(199, 0, TargetBlock.SetMountingFace(0, raycastResult.CellFace.Face));
			}
			return result;
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001E14C File Offset: 0x0001C34C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			TerrainGeometrySubset subsetAlphaTest = geometry.SubsetAlphaTest;
			DynamicArray<TerrainVertex> vertices = subsetAlphaTest.Vertices;
			DynamicArray<int> indices = subsetAlphaTest.Indices;
			int count = vertices.Count;
			int data = Terrain.ExtractData(value);
			int num = Terrain.ExtractLight(value);
			int mountingFace = TargetBlock.GetMountingFace(data);
			float s = LightingManager.LightIntensityByLightValueAndFace[num + 16 * mountingFace];
			Color color = Color.White * s;
			switch (mountingFace)
			{
			case 0:
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
			case 1:
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
			case 2:
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
			case 3:
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
			default:
				return;
			}
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001E5FF File Offset: 0x0001C7FF
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001E611 File Offset: 0x0001C811
		public static int GetMountingFace(int data)
		{
			return data & 3;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001E616 File Offset: 0x0001C816
		public static int SetMountingFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001E620 File Offset: 0x0001C820
		public override int GetFace(int value)
		{
			return TargetBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001E62D File Offset: 0x0001C82D
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new TargetElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001E648 File Offset: 0x0001C848
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x04000253 RID: 595
		public const int Index = 199;

		// Token: 0x04000254 RID: 596
		public BoundingBox[][] m_boundingBoxes = new BoundingBox[][]
		{
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.0625f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.0625f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0.9375f), new Vector3(1f, 1f, 1f))
			},
			new BoundingBox[]
			{
				new BoundingBox(new Vector3(0.9375f, 0f, 0f), new Vector3(1f, 1f, 1f))
			}
		};
	}
}
