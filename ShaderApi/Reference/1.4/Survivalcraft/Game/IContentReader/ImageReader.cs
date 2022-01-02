using System;
using Engine.Media;

namespace Game.IContentReader
{
	// Token: 0x020003B1 RID: 945
	public class ImageReader : IContentReader
	{
		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06001D5C RID: 7516 RVA: 0x000E0752 File Offset: 0x000DE952
		public override string Type
		{
			get
			{
				return "Engine.Media.Image";
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06001D5D RID: 7517 RVA: 0x000E0759 File Offset: 0x000DE959
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"png",
					"jpeg",
					"jpg"
				};
			}
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x000E0779 File Offset: 0x000DE979
		public override object Get(ContentInfo[] contents)
		{
			return Image.Load(contents[0].Duplicate());
		}
	}
}
