using System;
using System.IO;
using SimpleJson;

namespace Game.IContentReader
{
	// Token: 0x020003B2 RID: 946
	public class JsonArrayReader : IContentReader
	{
		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06001D60 RID: 7520 RVA: 0x000E0790 File Offset: 0x000DE990
		public override string Type
		{
			get
			{
				return "SimpleJson.JsonArray";
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06001D61 RID: 7521 RVA: 0x000E0797 File Offset: 0x000DE997
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

		// Token: 0x06001D62 RID: 7522 RVA: 0x000E07A7 File Offset: 0x000DE9A7
		public override object Get(ContentInfo[] contents)
		{
			return SimpleJson.DeserializeObject<JsonArray>(new StreamReader(contents[0].Duplicate()).ReadToEnd());
		}
	}
}
