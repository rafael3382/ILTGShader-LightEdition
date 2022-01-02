using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000E1 RID: 225
	public class SaddleBlock : Block
	{
		// Token: 0x06000476 RID: 1142 RVA: 0x00019814 File Offset: 0x00017A14
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Saddle", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Saddle", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Saddle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.2f, 0f), false, false, false, false, new Color(224, 224, 224));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Saddle", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.2f, 0f), false, true, false, false, new Color(96, 96, 96));
			base.Initialize();
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x000198EF File Offset: 0x00017AEF
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x000198F1 File Offset: 0x00017AF1
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x040001FC RID: 508
		public const int Index = 158;

		// Token: 0x040001FD RID: 509
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
