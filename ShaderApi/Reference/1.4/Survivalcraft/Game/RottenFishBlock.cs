using System;
using Engine;

namespace Game
{
	// Token: 0x020000DB RID: 219
	public class RottenFishBlock : FoodBlock
	{
		// Token: 0x06000461 RID: 1121 RVA: 0x0001921E File Offset: 0x0001741E
		public RottenFishBlock() : base("Models/Fish", Matrix.CreateTranslation(-0.125f, 0.125f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001F4 RID: 500
		public const int Index = 241;
	}
}
