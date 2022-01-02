using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000029 RID: 41
	public class ButtonBlock : MountedElectricElementBlock
	{
		// Token: 0x0600012B RID: 299 RVA: 0x00009A8C File Offset: 0x00007C8C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Button", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Button", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Button", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Button", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00009C26 File Offset: 0x00007E26
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00009C30 File Offset: 0x00007E30
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00009C70 File Offset: 0x00007E70
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00009C9C File Offset: 0x00007E9C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00009D08 File Offset: 0x00007F08
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00009D23 File Offset: 0x00007F23
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new ButtonElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00009D3C File Offset: 0x00007F3C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x04000098 RID: 152
		public const int Index = 142;

		// Token: 0x04000099 RID: 153
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400009A RID: 154
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x0400009B RID: 155
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
