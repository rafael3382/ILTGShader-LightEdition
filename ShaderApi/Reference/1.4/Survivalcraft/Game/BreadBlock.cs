using System;
using Engine;

namespace Game
{
	// Token: 0x02000021 RID: 33
	public class BreadBlock : FoodBlock
	{
		// Token: 0x06000114 RID: 276 RVA: 0x00009787 File Offset: 0x00007987
		public BreadBlock() : base("Models/Bread", Matrix.Identity, Color.White, 242)
		{
		}

		// Token: 0x0400008B RID: 139
		public const int Index = 177;
	}
}
