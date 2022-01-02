using System;
using Engine;

namespace Game
{
	// Token: 0x0200005D RID: 93
	public class DoughBlock : FoodBlock
	{
		// Token: 0x060001DF RID: 479 RVA: 0x0000CABD File Offset: 0x0000ACBD
		public DoughBlock() : base("Models/Bread", Matrix.CreateTranslation(0.5625f, -0.875f, 0f), new Color(241, 231, 214), 247)
		{
		}

		// Token: 0x040000E9 RID: 233
		public const int Index = 176;
	}
}
