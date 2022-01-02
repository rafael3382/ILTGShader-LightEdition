using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000244 RID: 580
	public class BestiaryCreatureInfo
	{
		// Token: 0x04000B9B RID: 2971
		public int Order;

		// Token: 0x04000B9C RID: 2972
		public string DisplayName;

		// Token: 0x04000B9D RID: 2973
		public string Description;

		// Token: 0x04000B9E RID: 2974
		public string ModelName;

		// Token: 0x04000B9F RID: 2975
		public string TextureOverride;

		// Token: 0x04000BA0 RID: 2976
		public float Mass;

		// Token: 0x04000BA1 RID: 2977
		public float AttackResilience;

		// Token: 0x04000BA2 RID: 2978
		public float AttackPower;

		// Token: 0x04000BA3 RID: 2979
		public float MovementSpeed;

		// Token: 0x04000BA4 RID: 2980
		public float JumpHeight;

		// Token: 0x04000BA5 RID: 2981
		public bool IsHerding;

		// Token: 0x04000BA6 RID: 2982
		public bool CanBeRidden;

		// Token: 0x04000BA7 RID: 2983
		public bool HasSpawnerEgg;

		// Token: 0x04000BA8 RID: 2984
		public List<ComponentLoot.Loot> Loot;
	}
}
