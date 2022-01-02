using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000125 RID: 293
	public class WoodenClubBlock : Block
	{
		// Token: 0x060005AC RID: 1452 RVA: 0x000206A8 File Offset: 0x0001E8A8
		public override void Initialize()
		{
			int num = 47;
			Model model = ContentManager.Get<Model>("Models/WoodenClub", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Handle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			this.m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			base.Initialize();
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0002075E File Offset: 0x0001E95E
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00020760 File Offset: 0x0001E960
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400027C RID: 636
		public const int Index = 122;

		// Token: 0x0400027D RID: 637
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
