using System;

namespace Game
{
	// Token: 0x02000074 RID: 116
	public class GlassBlock : AlphaTestCubeBlock
	{
		// Token: 0x060002A3 RID: 675 RVA: 0x000114C1 File Offset: 0x0000F6C1
		public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			return Terrain.ExtractContents(neighborValue) != 15 && base.ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}

		// Token: 0x0400012D RID: 301
		public const int Index = 15;
	}
}
