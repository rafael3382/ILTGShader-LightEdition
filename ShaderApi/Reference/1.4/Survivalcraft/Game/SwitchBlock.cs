using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200010C RID: 268
	public class SwitchBlock : MountedElectricElementBlock
	{
		// Token: 0x0600052C RID: 1324 RVA: 0x0001DB24 File Offset: 0x0001BD24
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Switch", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Lever", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int num = i << 1 | j;
					Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					Matrix m2 = Matrix.CreateRotationX((j == 0) ? MathUtils.DegToRad(30f) : MathUtils.DegToRad(-30f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Lever", true).MeshParts[0], boneAbsoluteTransform2 * m2 * m, false, false, false, false, Color.White);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
			}
			Matrix m3 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Body", true).MeshParts[0], boneAbsoluteTransform * m3, false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Lever", true).MeshParts[0], boneAbsoluteTransform2 * m3, false, false, false, false, Color.White);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001DD90 File Offset: 0x0001BF90
		public static bool GetLeverState(int value)
		{
			return (Terrain.ExtractData(value) & 1) != 0;
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001DD9D File Offset: 0x0001BF9D
		public static int SetLeverState(int value, bool state)
		{
			return Terrain.ReplaceData(value, state ? (Terrain.ExtractData(value) | 1) : (Terrain.ExtractData(value) & -2));
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001DDBB File Offset: 0x0001BFBB
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) >> 1 & 7;
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001DDC8 File Offset: 0x0001BFC8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face << 1),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001DE08 File Offset: 0x0001C008
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001DE34 File Offset: 0x0001C034
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001DEA0 File Offset: 0x0001C0A0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001DEBB File Offset: 0x0001C0BB
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SwitchElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)), value);
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001DED8 File Offset: 0x0001C0D8
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x0400024D RID: 589
		public const int Index = 141;

		// Token: 0x0400024E RID: 590
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400024F RID: 591
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[12];

		// Token: 0x04000250 RID: 592
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[12][];
	}
}
