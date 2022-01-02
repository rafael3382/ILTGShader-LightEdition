using System;
using Engine;

namespace Game
{
	// Token: 0x0200007A RID: 122
	public class GunpowderBlock : ChunkBlock
	{
		// Token: 0x060002BF RID: 703 RVA: 0x00011E44 File Offset: 0x00010044
		public GunpowderBlock() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(3f), Matrix.CreateScale(1f) * Matrix.CreateTranslation(0.0625f, 0.875f, 0f), new Color(255, 255, 255), false)
		{
		}

		// Token: 0x04000139 RID: 313
		public const int Index = 109;
	}
}
