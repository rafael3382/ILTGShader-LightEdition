using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007C RID: 124
	public abstract class HammerBlock : Block
	{
		// Token: 0x060002CA RID: 714 RVA: 0x0001211C File Offset: 0x0001031C
		public HammerBlock(int handleTextureSlot, int headTextureSlot)
		{
			this.m_handleTextureSlot = handleTextureSlot;
			this.m_headTextureSlot = headTextureSlot;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00012140 File Offset: 0x00010340
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Hammer", null);
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

		// Token: 0x060002CC RID: 716 RVA: 0x0001229F File Offset: 0x0001049F
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060002CD RID: 717 RVA: 0x000122A1 File Offset: 0x000104A1
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000140 RID: 320
		public int m_handleTextureSlot;

		// Token: 0x04000141 RID: 321
		public int m_headTextureSlot;

		// Token: 0x04000142 RID: 322
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
