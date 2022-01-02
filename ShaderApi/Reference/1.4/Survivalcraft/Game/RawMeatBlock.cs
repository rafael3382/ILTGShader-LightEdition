using System;
using Engine;

namespace Game
{
	// Token: 0x020000D2 RID: 210
	public class RawMeatBlock : FoodBlock
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x00018BBE File Offset: 0x00016DBE
		public RawMeatBlock() : base("Models/Meat", Matrix.Identity, Color.White, 240)
		{
		}

		// Token: 0x040001E5 RID: 485
		public const int Index = 88;
	}
}
