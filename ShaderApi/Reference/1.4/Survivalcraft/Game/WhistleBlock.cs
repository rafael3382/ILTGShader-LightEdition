using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000118 RID: 280
	public class WhistleBlock : Block
	{
		// Token: 0x06000578 RID: 1400 RVA: 0x0001FB28 File Offset: 0x0001DD28
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Whistle", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Whistle", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), false, false, false, false, new Color(255, 255, 255));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), false, true, false, false, new Color(64, 64, 64));
			base.Initialize();
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001FC03 File Offset: 0x0001DE03
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0001FC05 File Offset: 0x0001DE05
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 9f * size, ref matrix, environmentData);
		}

		// Token: 0x04000269 RID: 617
		public const int Index = 160;

		// Token: 0x0400026A RID: 618
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
