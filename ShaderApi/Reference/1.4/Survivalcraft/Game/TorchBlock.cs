using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000111 RID: 273
	public class TorchBlock : Block
	{
		// Token: 0x06000554 RID: 1364 RVA: 0x0001ED70 File Offset: 0x0001CF70
		public override void Initialize()
		{
			for (int i = 0; i < this.m_blockMeshesByVariant.Length; i++)
			{
				this.m_blockMeshesByVariant[i] = new BlockMesh();
			}
			Model model = ContentManager.Get<Model>("Models/Torch", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Torch", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Flame", true).ParentBone);
			Matrix m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(0.5f, 0.15f, -0.05f);
			this.m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(1.57079637f) * Matrix.CreateTranslation(-0.05f, 0.15f, 0.5f);
			this.m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(3.14159274f) * Matrix.CreateTranslation(0.5f, 0.15f, 1.05f);
			this.m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(4.712389f) * Matrix.CreateTranslation(1.05f, 0.15f, 0.5f);
			this.m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			m = Matrix.CreateTranslation(0.5f, 0f, 0.5f);
			this.m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
			this.m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Flame", true).MeshParts[0], boneAbsoluteTransform2 * m, true, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Torch", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, Color.White);
			for (int j = 0; j < 5; j++)
			{
				this.m_collisionBoxes[j] = new BoundingBox[]
				{
					this.m_blockMeshesByVariant[j].CalculateBoundingBox()
				};
			}
			base.Initialize();
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x0001F170 File Offset: 0x0001D370
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByVariant.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByVariant[num], Color.White, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0001F1B8 File Offset: 0x0001D3B8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = 0;
			if (raycastResult.CellFace.Face == 0)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 0);
			}
			if (raycastResult.CellFace.Face == 1)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 1);
			}
			if (raycastResult.CellFace.Face == 2)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 2);
			}
			if (raycastResult.CellFace.Face == 3)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 3);
			}
			if (raycastResult.CellFace.Face == 4)
			{
				value2 = Terrain.ReplaceData(Terrain.ReplaceContents(0, 31), 4);
			}
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0001F27C File Offset: 0x0001D47C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxes.Length)
			{
				return this.m_collisionBoxes[num];
			}
			return null;
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0001F2A5 File Offset: 0x0001D4A5
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400025B RID: 603
		public const int Index = 31;

		// Token: 0x0400025C RID: 604
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400025D RID: 605
		public BlockMesh[] m_blockMeshesByVariant = new BlockMesh[5];

		// Token: 0x0400025E RID: 606
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[5][];
	}
}
