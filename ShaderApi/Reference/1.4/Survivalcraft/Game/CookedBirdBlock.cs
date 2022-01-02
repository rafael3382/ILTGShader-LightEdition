using System;
using Engine;

namespace Game
{
	// Token: 0x0200003B RID: 59
	public class CookedBirdBlock : FoodBlock
	{
		// Token: 0x0600016F RID: 367 RVA: 0x0000B156 File Offset: 0x00009356
		public CookedBirdBlock() : base("Models/Bird", Matrix.Identity, new Color(150, 69, 15), 239)
		{
		}

		// Token: 0x040000BE RID: 190
		public const int Index = 78;
	}
}
