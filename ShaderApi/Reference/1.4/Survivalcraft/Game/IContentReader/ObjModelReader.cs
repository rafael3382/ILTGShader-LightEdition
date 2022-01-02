using System;

namespace Game.IContentReader
{
	// Token: 0x020003B6 RID: 950
	public class ObjModelReader : IContentReader
	{
		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06001D70 RID: 7536 RVA: 0x000E085C File Offset: 0x000DEA5C
		public override string Type
		{
			get
			{
				return "Game.ObjModel";
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06001D71 RID: 7537 RVA: 0x000E0863 File Offset: 0x000DEA63
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"obj"
				};
			}
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x000E0873 File Offset: 0x000DEA73
		public override object Get(ContentInfo[] contents)
		{
			return ObjModelReader.Load(contents[0].Duplicate());
		}
	}
}
