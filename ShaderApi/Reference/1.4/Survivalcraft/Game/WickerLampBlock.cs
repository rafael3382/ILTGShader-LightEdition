using System;

namespace Game
{
	// Token: 0x0200011A RID: 282
	public class WickerLampBlock : AlphaTestCubeBlock
	{
		// Token: 0x0600057D RID: 1405 RVA: 0x0001FC3B File Offset: 0x0001DE3B
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 5)
			{
				return this.DefaultTextureSlot;
			}
			return 4;
		}

		// Token: 0x0400026C RID: 620
		public const int Index = 17;
	}
}
