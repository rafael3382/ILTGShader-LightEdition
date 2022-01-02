using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CE RID: 206
	public abstract class RakeBlock : Block
	{
		// Token: 0x06000442 RID: 1090 RVA: 0x00018916 File Offset: 0x00016B16
		public RakeBlock(int handleTextureSlot, int headTextureSlot)
		{
			this.m_handleTextureSlot = handleTextureSlot;
			this.m_headTextureSlot = headTextureSlot;
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00018938 File Offset: 0x00016B38
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Rake", null);
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

		// Token: 0x06000444 RID: 1092 RVA: 0x00018A97 File Offset: 0x00016C97
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00018A99 File Offset: 0x00016C99
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001DF RID: 479
		public int m_handleTextureSlot;

		// Token: 0x040001E0 RID: 480
		public int m_headTextureSlot;

		// Token: 0x040001E1 RID: 481
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
