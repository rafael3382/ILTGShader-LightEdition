using System;

namespace Game
{
	// Token: 0x02000263 RID: 611
	public class CraftingRecipe
	{
		// Token: 0x04000C89 RID: 3209
		public const int MaxSize = 3;

		// Token: 0x04000C8A RID: 3210
		public int ResultValue;

		// Token: 0x04000C8B RID: 3211
		public int ResultCount;

		// Token: 0x04000C8C RID: 3212
		public int RemainsValue;

		// Token: 0x04000C8D RID: 3213
		public int RemainsCount;

		// Token: 0x04000C8E RID: 3214
		public float RequiredHeatLevel;

		// Token: 0x04000C8F RID: 3215
		public float RequiredPlayerLevel;

		// Token: 0x04000C90 RID: 3216
		public string[] Ingredients = new string[9];

		// Token: 0x04000C91 RID: 3217
		public string Description;

		// Token: 0x04000C92 RID: 3218
		public string Message;
	}
}
