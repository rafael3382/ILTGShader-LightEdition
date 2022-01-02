using System;
using Engine.Media;

namespace Game.IContentReader
{
	// Token: 0x020003B9 RID: 953
	public class StreamingSourceReader : IContentReader
	{
		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000E0925 File Offset: 0x000DEB25
		public override string Type
		{
			get
			{
				return "Engine.Media.StreamingSource";
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06001D7D RID: 7549 RVA: 0x000E092C File Offset: 0x000DEB2C
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"wav",
					"ogg"
				};
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x000E0944 File Offset: 0x000DEB44
		public override object Get(ContentInfo[] contents)
		{
			return SoundData.Stream(contents[0].Duplicate());
		}
	}
}
