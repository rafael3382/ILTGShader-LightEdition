using System;
using Engine;

namespace Game
{
	// Token: 0x020000A8 RID: 168
	public class MalachiteChunkBlock : ChunkBlock
	{
		// Token: 0x06000367 RID: 871 RVA: 0x00014EF8 File Offset: 0x000130F8
		public MalachiteChunkBlock() : base(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(0.1875f, 0.6875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x0400018F RID: 399
		public const int Index = 43;
	}
}
