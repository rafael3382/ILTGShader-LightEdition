using System;
using Engine;

namespace Game
{
	// Token: 0x02000073 RID: 115
	public class GermaniumOreChunkBlock : ChunkBlock
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x0001146C File Offset: 0x0000F66C
		public GermaniumOreChunkBlock() : base(Matrix.CreateRotationX(-1f) * Matrix.CreateRotationZ(1f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(204, 181, 162), false)
		{
		}

		// Token: 0x0400012C RID: 300
		public const int Index = 250;
	}
}
