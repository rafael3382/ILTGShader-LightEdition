using System;
using Engine;

namespace Game
{
	// Token: 0x0200010A RID: 266
	public class SulphurChunkBlock : ChunkBlock
	{
		// Token: 0x0600052A RID: 1322 RVA: 0x0001DAC4 File Offset: 0x0001BCC4
		public SulphurChunkBlock() : base(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(1f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(255, 255, 140), true)
		{
		}

		// Token: 0x0400024B RID: 587
		public const int Index = 103;
	}
}
