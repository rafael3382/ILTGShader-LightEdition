using System;
using Engine;

namespace Game
{
	// Token: 0x0200000E RID: 14
	public abstract class AlphaTestCubeBlock : CubeBlock
	{
		// Token: 0x060000AA RID: 170 RVA: 0x0000781D File Offset: 0x00005A1D
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.AlphaTestSubsetsByFace);
		}
	}
}
