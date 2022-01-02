using System;
using Engine;

namespace Game
{
	// Token: 0x02000103 RID: 259
	public class StoneChunkBlock : ChunkBlock
	{
		// Token: 0x0600051F RID: 1311 RVA: 0x0001D858 File Offset: 0x0001BA58
		public StoneChunkBlock() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(0f) * Matrix.CreateRotationZ(1f), Matrix.CreateScale(0.75f) * Matrix.CreateTranslation(0.1875f, 0.0625f, 0f), new Color(255, 255, 255), true)
		{
		}

		// Token: 0x04000243 RID: 579
		public const int Index = 79;
	}
}
