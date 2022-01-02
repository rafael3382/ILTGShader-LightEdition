using System;
using Engine;

namespace Game
{
	// Token: 0x02000105 RID: 261
	public class StoneFenceBlock : FenceBlock
	{
		// Token: 0x06000524 RID: 1316 RVA: 0x0001DA52 File Offset: 0x0001BC52
		public StoneFenceBlock() : base("Models/StoneFence", false, false, 24, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001DA7C File Offset: 0x0001BC7C
		public override bool ShouldConnectTo(int value)
		{
			return !BlocksManager.Blocks[Terrain.ExtractContents(value)].IsTransparent_(value) || base.ShouldConnectTo(value);
		}

		// Token: 0x04000246 RID: 582
		public const int Index = 202;
	}
}
