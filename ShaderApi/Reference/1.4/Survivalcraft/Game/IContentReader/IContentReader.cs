using System;

namespace Game.IContentReader
{
	// Token: 0x020003BE RID: 958
	public abstract class IContentReader
	{
		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06001D90 RID: 7568
		public abstract string Type { get; }

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001D91 RID: 7569
		public abstract string[] DefaultSuffix { get; }

		// Token: 0x06001D92 RID: 7570
		public abstract object Get(ContentInfo[] contents);
	}
}
