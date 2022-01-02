using System;
using Engine;

namespace Game
{
	// Token: 0x020000E2 RID: 226
	public class SaltpeterChunkBlock : ChunkBlock
	{
		// Token: 0x0600047A RID: 1146 RVA: 0x00019920 File Offset: 0x00017B20
		public SaltpeterChunkBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(0f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x040001FE RID: 510
		public const int Index = 102;
	}
}
