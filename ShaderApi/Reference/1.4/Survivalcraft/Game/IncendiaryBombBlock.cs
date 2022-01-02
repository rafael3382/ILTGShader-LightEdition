using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000082 RID: 130
	public class IncendiaryBombBlock : Block
	{
		// Token: 0x060002E4 RID: 740 RVA: 0x00012814 File Offset: 0x00010A14
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bomb", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bomb", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bomb", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), false, false, false, false, new Color(0.4f, 0.2f, 0.2f));
			base.Initialize();
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x000128A1 File Offset: 0x00010AA1
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x000128A3 File Offset: 0x00010AA3
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400014B RID: 331
		public const int Index = 228;

		// Token: 0x0400014C RID: 332
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
