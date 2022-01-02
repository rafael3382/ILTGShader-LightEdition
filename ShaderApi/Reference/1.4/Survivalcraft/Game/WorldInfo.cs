using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200035C RID: 860
	public class WorldInfo
	{
		// Token: 0x04001135 RID: 4405
		public string DirectoryName = string.Empty;

		// Token: 0x04001136 RID: 4406
		public long Size;

		// Token: 0x04001137 RID: 4407
		public DateTime LastSaveTime;

		// Token: 0x04001138 RID: 4408
		public string SerializationVersion = string.Empty;

		// Token: 0x04001139 RID: 4409
		public WorldSettings WorldSettings = new WorldSettings();

		// Token: 0x0400113A RID: 4410
		public List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
	}
}
