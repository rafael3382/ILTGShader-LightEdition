using System;
using Engine;

namespace Game
{
	// Token: 0x020000DC RID: 220
	public class RottenMeatBlock : FoodBlock
	{
		// Token: 0x06000462 RID: 1122 RVA: 0x00019249 File Offset: 0x00017449
		public RottenMeatBlock() : base("Models/Meat", Matrix.CreateTranslation(-0.0625f, 0f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001F5 RID: 501
		public const int Index = 240;
	}
}
