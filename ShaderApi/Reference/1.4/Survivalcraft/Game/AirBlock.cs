using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200000D RID: 13
	public class AirBlock : Block
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00007811 File Offset: 0x00005A11
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007813 File Offset: 0x00005A13
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x04000058 RID: 88
		public const int Index = 0;
	}
}
