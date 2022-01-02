using System;

namespace Game
{
	// Token: 0x02000336 RID: 822
	public enum UpdateOrder
	{
		// Token: 0x04001110 RID: 4368
		Reset = -100,
		// Token: 0x04001111 RID: 4369
		SubsystemPlayers = -20,
		// Token: 0x04001112 RID: 4370
		Input = -10,
		// Token: 0x04001113 RID: 4371
		Default = 0,
		// Token: 0x04001114 RID: 4372
		Locomotion,
		// Token: 0x04001115 RID: 4373
		Body,
		// Token: 0x04001116 RID: 4374
		CreatureModels = 10,
		// Token: 0x04001117 RID: 4375
		FirstPersonModels = 20,
		// Token: 0x04001118 RID: 4376
		BlocksScanner = 99,
		// Token: 0x04001119 RID: 4377
		Terrain,
		// Token: 0x0400111A RID: 4378
		Views = 200,
		// Token: 0x0400111B RID: 4379
		BlockHighlight
	}
}
