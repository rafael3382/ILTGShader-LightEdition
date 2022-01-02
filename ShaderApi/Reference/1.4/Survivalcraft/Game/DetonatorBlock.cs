using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004F RID: 79
	public class DetonatorBlock : MountedElectricElementBlock
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x0000BCF8 File Offset: 0x00009EF8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Detonator", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Detonator", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				int num = i;
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.14159274f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.57079637f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.14159274f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_blockMeshesByData[num] = new BlockMesh();
				this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh("Detonator", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_collisionBoxesByData[num] = new BoundingBox[]
				{
					this.m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateRotationZ(1.57079637f);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Detonator", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, false, false, Color.White);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000BE92 File Offset: 0x0000A092
		public override int GetFace(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000BE9C File Offset: 0x0000A09C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000BEDC File Offset: 0x0000A0DC
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000BF08 File Offset: 0x0000A108
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.125f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000BF74 File Offset: 0x0000A174
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 4f * size, ref matrix, environmentData);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000BF8F File Offset: 0x0000A18F
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DetonatorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000BFA8 File Offset: 0x0000A1A8
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x040000D3 RID: 211
		public const int Index = 147;

		// Token: 0x040000D4 RID: 212
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000D5 RID: 213
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		// Token: 0x040000D6 RID: 214
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];
	}
}
