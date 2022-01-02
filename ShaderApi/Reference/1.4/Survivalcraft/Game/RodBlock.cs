using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000D5 RID: 213
	public class RodBlock : Block
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x00018CD4 File Offset: 0x00016ED4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Rod", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronRod", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00018D52 File Offset: 0x00016F52
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00018D54 File Offset: 0x00016F54
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001E8 RID: 488
		public const int Index = 195;

		// Token: 0x040001E9 RID: 489
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
