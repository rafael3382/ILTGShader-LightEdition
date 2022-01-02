using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200001D RID: 29
	public class BoatBlock : Block
	{
		// Token: 0x060000FB RID: 251 RVA: 0x000090E0 File Offset: 0x000072E0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/BoatItem", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Boat", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Boat", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, false, false, false, new Color(96, 96, 96));
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Boat", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.4f, 0f), false, true, false, false, new Color(255, 255, 255));
			base.Initialize();
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000091BB File Offset: 0x000073BB
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000091BD File Offset: 0x000073BD
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x04000085 RID: 133
		public const int Index = 178;

		// Token: 0x04000086 RID: 134
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
