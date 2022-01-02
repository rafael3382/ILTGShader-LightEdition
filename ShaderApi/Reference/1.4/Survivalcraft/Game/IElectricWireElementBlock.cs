using System;

namespace Game
{
	// Token: 0x02000080 RID: 128
	public interface IElectricWireElementBlock : IElectricElementBlock
	{
		// Token: 0x060002DB RID: 731
		int GetConnectedWireFacesMask(int value, int face);
	}
}
