using System;
using Engine;

namespace Game
{
	// Token: 0x02000071 RID: 113
	public class GermaniumChunkBlock : ChunkBlock
	{
		// Token: 0x060002A0 RID: 672 RVA: 0x0001140C File Offset: 0x0000F60C
		public GermaniumChunkBlock() : base(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.875f, 0.25f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x0400012A RID: 298
		public const int Index = 149;
	}
}
