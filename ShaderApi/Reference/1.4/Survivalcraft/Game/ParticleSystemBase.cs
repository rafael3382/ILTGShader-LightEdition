using System;

namespace Game
{
	// Token: 0x020002D9 RID: 729
	public abstract class ParticleSystemBase
	{
		// Token: 0x060015E9 RID: 5609
		public abstract void Draw(Camera camera);

		// Token: 0x060015EA RID: 5610
		public abstract bool Simulate(float dt);

		// Token: 0x060015EB RID: 5611 RVA: 0x000A5389 File Offset: 0x000A3589
		public virtual void OnAdded()
		{
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x000A538B File Offset: 0x000A358B
		public virtual void OnRemoved()
		{
		}

		// Token: 0x04000E79 RID: 3705
		public SubsystemParticles SubsystemParticles;
	}
}
