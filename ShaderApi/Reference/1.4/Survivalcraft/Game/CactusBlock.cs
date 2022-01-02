using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200002A RID: 42
	public class CactusBlock : Block
	{
		// Token: 0x06000134 RID: 308 RVA: 0x00009DA4 File Offset: 0x00007FA4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Cactus", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Cactus", true).ParentBone);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("Cactus", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Cactus", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00009E6C File Offset: 0x0000806C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetAlphaTest);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00009EA0 File Offset: 0x000080A0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00009EB5 File Offset: 0x000080B5
		public override bool ShouldAvoid(int value)
		{
			return true;
		}

		// Token: 0x0400009C RID: 156
		public const int Index = 127;

		// Token: 0x0400009D RID: 157
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x0400009E RID: 158
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
