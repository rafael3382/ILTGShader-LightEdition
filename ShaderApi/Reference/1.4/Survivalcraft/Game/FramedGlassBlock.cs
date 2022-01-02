using System;

namespace Game
{
	// Token: 0x0200006D RID: 109
	public class FramedGlassBlock : AlphaTestCubeBlock
	{
		// Token: 0x06000276 RID: 630 RVA: 0x000100E6 File Offset: 0x0000E2E6
		public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			return Terrain.ExtractContents(neighborValue) != 44 && base.ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}

		// Token: 0x04000120 RID: 288
		public const int Index = 44;
	}
}
