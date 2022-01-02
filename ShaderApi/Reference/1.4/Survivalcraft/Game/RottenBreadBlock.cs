using System;
using Engine;

namespace Game
{
	// Token: 0x020000D8 RID: 216
	public class RottenBreadBlock : FoodBlock
	{
		// Token: 0x0600045E RID: 1118 RVA: 0x0001919D File Offset: 0x0001739D
		public RottenBreadBlock() : base("Models/Bread", Matrix.CreateTranslation(-0.375f, -0.25f, 0f), Color.White, FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001F1 RID: 497
		public const int Index = 242;
	}
}
