using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004A RID: 74
	public class CraftingTableBlock : Block
	{
		// Token: 0x0600018D RID: 397 RVA: 0x0000B624 File Offset: 0x00009824
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/CraftingTable", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CraftingTable", true).ParentBone);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000B6EC File Offset: 0x000098EC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, null, geometry.SubsetOpaque);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000B721 File Offset: 0x00009921
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x040000CD RID: 205
		public const int Index = 27;

		// Token: 0x040000CE RID: 206
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x040000CF RID: 207
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
