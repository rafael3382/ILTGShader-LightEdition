using System;
using Engine;

namespace Game
{
	// Token: 0x020002B0 RID: 688
	public interface INoiseListener
	{
		// Token: 0x0600154E RID: 5454
		void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness);
	}
}
