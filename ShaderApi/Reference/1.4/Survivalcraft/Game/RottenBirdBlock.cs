using System;
using Engine;

namespace Game
{
	// Token: 0x020000D7 RID: 215
	public class RottenBirdBlock : FoodBlock
	{
		// Token: 0x0600045D RID: 1117 RVA: 0x00019172 File Offset: 0x00017372
		public RottenBirdBlock() : base("Models/Bird", Matrix.CreateTranslation(-0.9375f, 0.4375f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001F0 RID: 496
		public const int Index = 239;
	}
}
