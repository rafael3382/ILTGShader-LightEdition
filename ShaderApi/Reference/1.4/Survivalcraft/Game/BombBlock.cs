using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200001E RID: 30
	public class BombBlock : Block
	{
		// Token: 0x060000FF RID: 255 RVA: 0x000091EC File Offset: 0x000073EC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bomb", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bomb", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bomb", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, new Color(0.3f, 0.3f, 0.3f));
			base.Initialize();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00009279 File Offset: 0x00007479
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000927B File Offset: 0x0000747B
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000087 RID: 135
		public const int Index = 201;

		// Token: 0x04000088 RID: 136
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
