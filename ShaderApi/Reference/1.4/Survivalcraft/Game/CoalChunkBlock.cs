using System;
using Engine;

namespace Game
{
	// Token: 0x02000035 RID: 53
	public class CoalChunkBlock : ChunkBlock
	{
		// Token: 0x06000166 RID: 358 RVA: 0x0000AF00 File Offset: 0x00009100
		public CoalChunkBlock() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.875f, 0.1875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x040000B6 RID: 182
		public const int Index = 22;
	}
}
