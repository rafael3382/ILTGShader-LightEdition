using System;
using Engine;

namespace Game
{
	// Token: 0x02000015 RID: 21
	public class BasaltFenceBlock : FenceBlock
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x00008816 File Offset: 0x00006A16
		public BasaltFenceBlock() : base("Models/StoneFence", false, false, 40, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00008840 File Offset: 0x00006A40
		public override bool ShouldConnectTo(int value)
		{
			return !BlocksManager.Blocks[Terrain.ExtractContents(value)].IsTransparent_(value) || base.ShouldConnectTo(value);
		}

		// Token: 0x04000077 RID: 119
		public const int Index = 163;
	}
}
