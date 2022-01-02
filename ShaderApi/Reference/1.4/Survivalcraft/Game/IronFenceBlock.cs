using System;
using Engine;

namespace Game
{
	// Token: 0x02000089 RID: 137
	public class IronFenceBlock : FenceBlock
	{
		// Token: 0x060002F2 RID: 754 RVA: 0x000129C4 File Offset: 0x00010BC4
		public IronFenceBlock() : base("Models/IronFence", true, true, 58, new Color(192, 192, 192), new Color(80, 80, 80))
		{
		}

		// Token: 0x04000153 RID: 339
		public const int Index = 193;
	}
}
