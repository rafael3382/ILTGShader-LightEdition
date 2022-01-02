using System;
using System.IO;
using Engine.Graphics;

namespace Game.IContentReader
{
	// Token: 0x020003B7 RID: 951
	public class ShaderReader : IContentReader
	{
		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06001D74 RID: 7540 RVA: 0x000E088A File Offset: 0x000DEA8A
		public override string Type
		{
			get
			{
				return "Engine.Graphics.Shader";
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06001D75 RID: 7541 RVA: 0x000E0891 File Offset: 0x000DEA91
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"vsh",
					"psh"
				};
			}
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x000E08A9 File Offset: 0x000DEAA9
		public override object Get(ContentInfo[] contents)
		{
			return new Shader(new StreamReader(contents[0].Duplicate()).ReadToEnd(), new StreamReader(contents[1].Duplicate()).ReadToEnd(), new ShaderMacro[]
			{
				new ShaderMacro("empty")
			});
		}
	}
}
