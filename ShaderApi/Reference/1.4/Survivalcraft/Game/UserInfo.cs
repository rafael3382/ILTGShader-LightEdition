using System;

namespace Game
{
	// Token: 0x02000337 RID: 823
	public class UserInfo
	{
		// Token: 0x06001862 RID: 6242 RVA: 0x000C05C3 File Offset: 0x000BE7C3
		public UserInfo(string uniqueId, string displayName)
		{
			this.UniqueId = uniqueId;
			this.DisplayName = displayName;
		}

		// Token: 0x0400111C RID: 4380
		public readonly string UniqueId;

		// Token: 0x0400111D RID: 4381
		public readonly string DisplayName;
	}
}
