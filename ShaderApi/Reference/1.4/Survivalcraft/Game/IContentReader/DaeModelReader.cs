using System;
using Engine.Graphics;

namespace Game.IContentReader
{
	// Token: 0x020003B0 RID: 944
	public class DaeModelReader : IContentReader
	{
		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06001D58 RID: 7512 RVA: 0x000E0723 File Offset: 0x000DE923
		public override string Type
		{
			get
			{
				return "Engine.Graphics.Model";
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06001D59 RID: 7513 RVA: 0x000E072A File Offset: 0x000DE92A
		public override string[] DefaultSuffix
		{
			get
			{
				return new string[]
				{
					"dae"
				};
			}
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x000E073A File Offset: 0x000DE93A
		public override object Get(ContentInfo[] contents)
		{
			return Model.Load(contents[0].Duplicate(), true);
		}
	}
}
