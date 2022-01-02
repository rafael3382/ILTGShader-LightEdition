using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B3 RID: 179
	public class MotionDetectorBlock : MountedElectricElementBlock
	{
		// Token: 0x06000377 RID: 887 RVA: 0x00015200 File Offset: 0x00013400
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/MotionDetector", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("MotionDetector", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("MotionDetector", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("MotionDetector", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0001539A File Offset: 0x0001359A
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000153A4 File Offset: 0x000135A4
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000153E4 File Offset: 0x000135E4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00015410 File Offset: 0x00013610
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0001547C File Offset: 0x0001367C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00015497 File Offset: 0x00013697
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MotionDetectorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000154B0 File Offset: 0x000136B0
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x0400019B RID: 411
		public const int Index = 179;

		// Token: 0x0400019C RID: 412
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400019D RID: 413
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x0400019E RID: 414
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
