using System;
using Engine.Audio;

namespace Game.IContentReader
{
	// Token: 0x020003B8 RID: 952
	public class SoundBufferReader : IContentReader
	{
		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06001D78 RID: 7544 RVA: 0x000E08EF File Offset: 0x000DEAEF
		public override string Type
		{
			get
			{
				return "Engine.Audio.SoundBuffer";
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001D79 RID: 7545 RVA: 0x000E08F6 File Offset: 0x000DEAF6
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

		// Token: 0x06001D7A RID: 7546 RVA: 0x000E090E File Offset: 0x000DEB0E
		public override object Get(ContentInfo[] contents)
		{
			return SoundBuffer.Load(contents[0].Duplicate());
		}
	}
}
