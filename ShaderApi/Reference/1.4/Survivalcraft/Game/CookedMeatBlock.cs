using System;
using Engine;

namespace Game
{
	// Token: 0x0200003D RID: 61
	public class CookedMeatBlock : FoodBlock
	{
		// Token: 0x06000171 RID: 369 RVA: 0x0000B1A0 File Offset: 0x000093A0
		public CookedMeatBlock() : base("Models/Meat", Matrix.Identity, new Color(155, 122, 51), 240)
		{
		}

		// Token: 0x040000C0 RID: 192
		public const int Index = 89;
	}
}
