using System;

namespace Game
{
	// Token: 0x020001C8 RID: 456
	public abstract class SubsystemPollableBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x06000C0C RID: 3084
		public abstract void OnPoll(int value, int x, int y, int z, int pollPass);
	}
}
