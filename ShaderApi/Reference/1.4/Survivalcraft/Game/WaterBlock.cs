using System;
using Engine;

namespace Game
{
	// Token: 0x02000115 RID: 277
	public class WaterBlock : FluidBlock
	{
		// Token: 0x06000570 RID: 1392 RVA: 0x0001F8B6 File Offset: 0x0001DAB6
		public WaterBlock() : base(WaterBlock.MaxLevel)
		{
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0001F8C4 File Offset: 0x0001DAC4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color sideColor;
			Color color = sideColor = BlockColorsMap.WaterColorsMap.Lookup(generator.Terrain, x, y, z);
			sideColor.A = byte.MaxValue;
			Color topColor = color;
			topColor.A = 0;
			base.GenerateFluidTerrainVertices(generator, value, x, y, z, sideColor, topColor, geometry.TransparentSubsetsByFace);
		}

		// Token: 0x04000265 RID: 613
		public const int Index = 18;

		// Token: 0x04000266 RID: 614
		public new static int MaxLevel = 7;
	}
}
