using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000061 RID: 97
	public class ExperienceBlock : Block
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0000D1CC File Offset: 0x0000B3CC
		public override void Initialize()
		{
			base.Initialize();
			this.m_texture = ContentManager.Get<Texture2D>("Textures/Experience", null);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000D1E5 File Offset: 0x0000B3E5
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000D1E7 File Offset: 0x0000B3E7
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size * 0.18f, ref matrix, this.m_texture, Color.White, true, environmentData);
		}

		// Token: 0x040000F0 RID: 240
		public const int Index = 248;

		// Token: 0x040000F1 RID: 241
		public Texture2D m_texture;
	}
}
