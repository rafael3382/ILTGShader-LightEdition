using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009D RID: 157
	public class LeatherBlock : Block
	{
		// Token: 0x0600031F RID: 799 RVA: 0x00013970 File Offset: 0x00011B70
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leather", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Leather", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leather", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leather", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, true, false, false, new Color(128, 128, 160));
			base.Initialize();
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00013A45 File Offset: 0x00011C45
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00013A47 File Offset: 0x00011C47
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x0400016F RID: 367
		public const int Index = 159;

		// Token: 0x04000170 RID: 368
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
