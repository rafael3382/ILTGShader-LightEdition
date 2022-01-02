using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FE RID: 254
	public class StickBlock : Block
	{
		// Token: 0x06000514 RID: 1300 RVA: 0x0001D600 File Offset: 0x0001B800
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Stick", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Stick", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Stick", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001D67E File Offset: 0x0001B87E
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001D680 File Offset: 0x0001B880
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400023C RID: 572
		public const int Index = 23;

		// Token: 0x0400023D RID: 573
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
