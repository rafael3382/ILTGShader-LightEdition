using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000113 RID: 275
	public class TreasureGeneratorBlock : Block
	{
		// Token: 0x0600056A RID: 1386 RVA: 0x0001F7B2 File Offset: 0x0001D9B2
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001F7B4 File Offset: 0x0001D9B4
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x04000263 RID: 611
		public const int Index = 190;
	}
}
