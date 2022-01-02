using System;
using System.IO;

namespace Game.IContentReader
{
	// Token: 0x020003BA RID: 954
	public class StringReader : IContentReader
	{
		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x000E095B File Offset: 0x000DEB5B
		public override string Type
		{
			get
			{
				return "System.String";
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x000E0962 File Offset: 0x000DEB62
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"txt"
				};
			}
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x000E0972 File Offset: 0x000DEB72
		public override object Get(ContentInfo[] contents)
		{
			return new StreamReader(contents[0].Duplicate()).ReadToEnd();
		}
	}
}
