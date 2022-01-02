using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C4 RID: 196
	public abstract class PickaxeBlock : Block
	{
		// Token: 0x060003E3 RID: 995 RVA: 0x00016C3B File Offset: 0x00014E3B
		public PickaxeBlock(int handleTextureSlot, int headTextureSlot)
		{
			this.m_handleTextureSlot = handleTextureSlot;
			this.m_headTextureSlot = headTextureSlot;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00016C5C File Offset: 0x00014E5C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pickaxe", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Handle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_handleTextureSlot % 16) / 16f, (float)(this.m_handleTextureSlot / 16) / 16f, 0f), -1);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("Head", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_headTextureSlot % 16) / 16f, (float)(this.m_headTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
			base.Initialize();
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00016DBB File Offset: 0x00014FBB
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00016DBD File Offset: 0x00014FBD
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001BF RID: 447
		public int m_handleTextureSlot;

		// Token: 0x040001C0 RID: 448
		public int m_headTextureSlot;

		// Token: 0x040001C1 RID: 449
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
