using System;
using System.IO;
using SimpleJson;

namespace Game.IContentReader
{
	// Token: 0x020003B4 RID: 948
	public class JsonObjectReader : IContentReader
	{
		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06001D68 RID: 7528 RVA: 0x000E07F6 File Offset: 0x000DE9F6
		public override string Type
		{
			get
			{
				return "SimpleJson.JsonObject";
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06001D69 RID: 7529 RVA: 0x000E07FD File Offset: 0x000DE9FD
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

		// Token: 0x06001D6A RID: 7530 RVA: 0x000E080D File Offset: 0x000DEA0D
		public override object Get(ContentInfo[] contents)
		{
			return SimpleJson.DeserializeObject<JsonObject>(new StreamReader(contents[0].Duplicate()).ReadToEnd());
		}
	}
}
