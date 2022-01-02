using System;
using Engine;

namespace Game
{
	// Token: 0x020000D1 RID: 209
	public class RawFishBlock : FoodBlock
	{
		// Token: 0x0600044A RID: 1098 RVA: 0x00018BA2 File Offset: 0x00016DA2
		public RawFishBlock() : base("Models/Fish", Matrix.Identity, Color.White, 241)
		{
		}

		// Token: 0x040001E4 RID: 484
		public const int Index = 161;
	}
}
