using System;
using Engine;

namespace Game
{
	// Token: 0x020000DA RID: 218
	public class RottenEggBlock : FoodBlock
	{
		// Token: 0x06000460 RID: 1120 RVA: 0x00019202 File Offset: 0x00017402
		public RottenEggBlock() : base("Models/RottenEgg", Matrix.Identity, Color.White, 246)
		{
		}

		// Token: 0x040001F3 RID: 499
		public const int Index = 246;
	}
}
