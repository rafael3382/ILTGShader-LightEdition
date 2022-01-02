using System;
using Engine;

namespace Game
{
	// Token: 0x02000090 RID: 144
	public class IronOreChunkBlock : ChunkBlock
	{
		// Token: 0x060002F9 RID: 761 RVA: 0x00012A80 File Offset: 0x00010C80
		public IronOreChunkBlock() : base(Matrix.CreateRotationX(0f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(136, 74, 36), false)
		{
		}

		// Token: 0x0400015A RID: 346
		public const int Index = 249;
	}
}
