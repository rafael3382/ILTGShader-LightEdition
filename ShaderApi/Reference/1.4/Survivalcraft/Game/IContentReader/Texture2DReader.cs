using System;
using Engine.Graphics;

namespace Game.IContentReader
{
	// Token: 0x020003BC RID: 956
	public class Texture2DReader : IContentReader
	{
		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001D88 RID: 7560 RVA: 0x000E0A04 File Offset: 0x000DEC04
		public override string Type
		{
			get
			{
				return "Engine.Graphics.Texture2D";
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001D89 RID: 7561 RVA: 0x000E0A0B File Offset: 0x000DEC0B
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"png",
					"jpg",
					"jpeg"
				};
			}
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x000E0A2B File Offset: 0x000DEC2B
		public override object Get(ContentInfo[] contents)
		{
			return Texture2D.Load(contents[0].Duplicate(), false, 1);
		}
	}
}
