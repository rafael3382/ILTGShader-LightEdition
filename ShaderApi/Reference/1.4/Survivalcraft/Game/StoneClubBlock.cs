using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000104 RID: 260
	public class StoneClubBlock : Block
	{
		// Token: 0x06000520 RID: 1312 RVA: 0x0001D8CC File Offset: 0x0001BACC
		public override void Initialize()
		{
			int num = 47;
			int num2 = 1;
			Model model = ContentManager.Get<Model>("Models/StoneClub", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Spikes", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Handle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("Spikes", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num2 % 16) / 16f, (float)(num2 / 16) / 16f, 0f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
			base.Initialize();
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001DA22 File Offset: 0x0001BC22
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001DA24 File Offset: 0x0001BC24
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000244 RID: 580
		public const int Index = 123;

		// Token: 0x04000245 RID: 581
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
