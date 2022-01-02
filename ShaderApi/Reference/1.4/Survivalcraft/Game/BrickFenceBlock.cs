using System;
using Engine;

namespace Game
{
	// Token: 0x02000023 RID: 35
	public class BrickFenceBlock : FenceBlock
	{
		// Token: 0x06000119 RID: 281 RVA: 0x00009852 File Offset: 0x00007A52
		public BrickFenceBlock() : base("Models/StoneFence", false, false, 39, new Color(212, 212, 212), Color.White)
		{
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000987C File Offset: 0x00007A7C
		public override bool ShouldConnectTo(int value)
		{
			return !BlocksManager.Blocks[Terrain.ExtractContents(value)].IsTransparent_(value) || base.ShouldConnectTo(value);
		}

		// Token: 0x0400008E RID: 142
		public const int Index = 164;
	}
}
