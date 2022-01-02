using System;
using Engine;

namespace Game
{
	// Token: 0x020000D9 RID: 217
	public class RottenDoughBlock : FoodBlock
	{
		// Token: 0x0600045F RID: 1119 RVA: 0x000191C8 File Offset: 0x000173C8
		public RottenDoughBlock() : base("Models/Bread", Matrix.CreateTranslation(-0.375f, -0.25f, 0f), new Color(192, 255, 212), FoodBlock.m_compostValue)
		{
		}

		// Token: 0x040001F2 RID: 498
		public const int Index = 247;
	}
}
