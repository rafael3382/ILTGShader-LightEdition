using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000052 RID: 82
	public class DiamondChunkBlock : Block
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000C028 File Offset: 0x0000A228
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Diamond", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Diamond", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Diamond", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C0A6 File Offset: 0x0000A2A6
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040000D9 RID: 217
		public const int Index = 111;

		// Token: 0x040000DA RID: 218
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
