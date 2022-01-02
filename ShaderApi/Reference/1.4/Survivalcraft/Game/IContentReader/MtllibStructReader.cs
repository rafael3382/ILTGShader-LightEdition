using System;

namespace Game.IContentReader
{
	// Token: 0x020003B5 RID: 949
	public class MtllibStructReader : IContentReader
	{
		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06001D6C RID: 7532 RVA: 0x000E082E File Offset: 0x000DEA2E
		public override string Type
		{
			get
			{
				return "Game.MtllibStruct";
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06001D6D RID: 7533 RVA: 0x000E0835 File Offset: 0x000DEA35
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"mtl"
				};
			}
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x000E0845 File Offset: 0x000DEA45
		public override object Get(ContentInfo[] contents)
		{
			return MtllibStruct.Load(contents[0].Duplicate());
		}
	}
}
