using System;
using Engine;

namespace Game
{
	// Token: 0x020002B5 RID: 693
	public interface ITrailParticleSystem
	{
		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600156A RID: 5482
		// (set) Token: 0x0600156B RID: 5483
		Vector3 Position { get; set; }

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600156C RID: 5484
		// (set) Token: 0x0600156D RID: 5485
		bool IsStopped { get; set; }
	}
}
