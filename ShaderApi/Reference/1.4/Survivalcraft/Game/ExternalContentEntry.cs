using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000289 RID: 649
	public class ExternalContentEntry
	{
		// Token: 0x04000D49 RID: 3401
		public ExternalContentType Type;

		// Token: 0x04000D4A RID: 3402
		public string Path;

		// Token: 0x04000D4B RID: 3403
		public long Size;

		// Token: 0x04000D4C RID: 3404
		public DateTime Time;

		// Token: 0x04000D4D RID: 3405
		public List<ExternalContentEntry> ChildEntries = new List<ExternalContentEntry>();
	}
}
