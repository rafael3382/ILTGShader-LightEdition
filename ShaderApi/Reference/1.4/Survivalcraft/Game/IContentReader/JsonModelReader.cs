using System;

namespace Game.IContentReader
{
	// Token: 0x020003B3 RID: 947
	public class JsonModelReader : IContentReader
	{
		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x000E07C8 File Offset: 0x000DE9C8
		public override string Type
		{
			get
			{
				return "Game.JsonModel";
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06001D65 RID: 7525 RVA: 0x000E07CF File Offset: 0x000DE9CF
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"json"
				};
			}
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000E07DF File Offset: 0x000DE9DF
		public override object Get(ContentInfo[] contents)
		{
			return JsonModelReader.Load(contents[0].Duplicate());
		}
	}
}
