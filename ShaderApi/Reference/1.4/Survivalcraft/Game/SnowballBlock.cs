using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F3 RID: 243
	public class SnowballBlock : Block
	{
		// Token: 0x060004C4 RID: 1220 RVA: 0x0001B008 File Offset: 0x00019208
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Snowball", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Snowball", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Snowball", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001B086 File Offset: 0x00019286
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001B088 File Offset: 0x00019288
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 2.5f * size, ref matrix, environmentData);
		}

		// Token: 0x0400021E RID: 542
		public const int Index = 85;

		// Token: 0x0400021F RID: 543
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
