using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C3 RID: 195
	public class PhotodiodeBlock : MountedElectricElementBlock
	{
		// Token: 0x060003DA RID: 986 RVA: 0x00016924 File Offset: 0x00014B24
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Photodiode", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Photodiode", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Photodiode", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Photodiode", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00016ABE File Offset: 0x00014CBE
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00016AC8 File Offset: 0x00014CC8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00016B08 File Offset: 0x00014D08
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00016B34 File Offset: 0x00014D34
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00016BA0 File Offset: 0x00014DA0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00016BBB File Offset: 0x00014DBB
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PhotodiodeElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00016BD4 File Offset: 0x00014DD4
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x040001BB RID: 443
		public const int Index = 151;

		// Token: 0x040001BC RID: 444
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040001BD RID: 445
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x040001BE RID: 446
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
