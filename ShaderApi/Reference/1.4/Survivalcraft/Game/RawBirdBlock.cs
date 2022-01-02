using System;
using Engine;

namespace Game
{
	// Token: 0x020000D0 RID: 208
	public class RawBirdBlock : FoodBlock
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x00018B77 File Offset: 0x00016D77
		public RawBirdBlock() : base("Models/Bird", Matrix.Identity, new Color(224, 170, 164), 239)
		{
		}

		// Token: 0x040001E3 RID: 483
		public const int Index = 77;
	}
}
