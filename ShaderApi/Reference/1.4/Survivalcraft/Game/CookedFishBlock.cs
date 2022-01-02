using System;
using Engine;

namespace Game
{
	// Token: 0x0200003C RID: 60
	public class CookedFishBlock : FoodBlock
	{
		// Token: 0x06000170 RID: 368 RVA: 0x0000B17B File Offset: 0x0000937B
		public CookedFishBlock() : base("Models/Fish", Matrix.Identity, new Color(160, 80, 40), 241)
		{
		}

		// Token: 0x040000BF RID: 191
		public const int Index = 162;
	}
}
