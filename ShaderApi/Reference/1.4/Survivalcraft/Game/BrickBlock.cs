using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000022 RID: 34
	public class BrickBlock : Block
	{
		// Token: 0x06000115 RID: 277 RVA: 0x000097A4 File Offset: 0x000079A4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Brick", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Brick", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Brick", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.075f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00009822 File Offset: 0x00007A22
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00009824 File Offset: 0x00007A24
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2.5f * size, ref matrix, environmentData);
		}

		// Token: 0x0400008C RID: 140
		public const int Index = 74;

		// Token: 0x0400008D RID: 141
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
