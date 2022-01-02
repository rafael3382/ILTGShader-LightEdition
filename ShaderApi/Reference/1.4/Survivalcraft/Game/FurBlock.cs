using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200006E RID: 110
	public class FurBlock : Block
	{
		// Token: 0x06000278 RID: 632 RVA: 0x00010108 File Offset: 0x0000E308
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Fur", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fur", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Fur", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Fur", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, true, false, false, new Color(128, 128, 160));
			base.Initialize();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000101DD File Offset: 0x0000E3DD
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x000101DF File Offset: 0x0000E3DF
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x04000121 RID: 289
		public const int Index = 207;

		// Token: 0x04000122 RID: 290
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
