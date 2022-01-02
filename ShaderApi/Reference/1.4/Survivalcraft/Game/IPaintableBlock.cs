using System;

namespace Game
{
	// Token: 0x02000084 RID: 132
	public interface IPaintableBlock
	{
		// Token: 0x060002EC RID: 748
		int? GetPaintColor(int value);

		// Token: 0x060002ED RID: 749
		int Paint(SubsystemTerrain subsystemTerrain, int value, int? color);
	}
}
