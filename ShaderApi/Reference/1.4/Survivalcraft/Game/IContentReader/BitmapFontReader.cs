using System;
using Engine.Media;

namespace Game.IContentReader
{
	// Token: 0x020003AF RID: 943
	public class BitmapFontReader : IContentReader
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06001D54 RID: 7508 RVA: 0x000E06D4 File Offset: 0x000DE8D4
		public override string Type
		{
			get
			{
				return "Engine.Media.BitmapFont";
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06001D55 RID: 7509 RVA: 0x000E06DB File Offset: 0x000DE8DB
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"lst",
					"png"
				};
			}
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000E06F3 File Offset: 0x000DE8F3
		public override object Get(ContentInfo[] contents)
		{
			if (contents.Length != 2)
			{
				throw new Exception("not matches content count");
			}
			return BitmapFont.Initialize(contents[1].Duplicate(), contents[0].Duplicate());
		}
	}
}
