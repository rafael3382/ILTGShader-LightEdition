using System;
using System.Xml.Linq;

namespace Game.IContentReader
{
	// Token: 0x020003BD RID: 957
	public class XmlReader : IContentReader
	{
		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001D8C RID: 7564 RVA: 0x000E0A44 File Offset: 0x000DEC44
		public override string Type
		{
			get
			{
				return "System.Xml.Linq.XElement";
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06001D8D RID: 7565 RVA: 0x000E0A4B File Offset: 0x000DEC4B
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"xml",
					"xdb"
				};
			}
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x000E0A63 File Offset: 0x000DEC63
		public override object Get(ContentInfo[] contents)
		{
			return XElement.Load(contents[0].Duplicate());
		}
	}
}
