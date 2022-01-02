using System;
using Engine;
using Engine.Graphics;

namespace Game.IContentReader
{
	// Token: 0x020003BB RID: 955
	public class SubtextureReader : IContentReader
	{
		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06001D84 RID: 7556 RVA: 0x000E098E File Offset: 0x000DEB8E
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"png",
					"txt"
				};
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001D85 RID: 7557 RVA: 0x000E09A6 File Offset: 0x000DEBA6
		public override string Type
		{
			get
			{
				return "Game.Subtexture";
			}
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x000E09B0 File Offset: 0x000DEBB0
		public override object Get(ContentInfo[] contents)
		{
			if (contents[0].ContentPath.Contains("Textures/Atlas/"))
			{
				return TextureAtlasManager.GetSubtexture(contents[0].ContentPath);
			}
			return new Subtexture(ContentManager.Get<Texture2D>(contents[0].ContentPath, null), Vector2.Zero, Vector2.One);
		}
	}
}
