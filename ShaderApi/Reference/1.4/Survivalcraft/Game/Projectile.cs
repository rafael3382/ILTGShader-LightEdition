using System;
using Engine;

namespace Game
{
	// Token: 0x020002EC RID: 748
	public class Projectile : WorldItem
	{
		// Token: 0x04000F1E RID: 3870
		public Vector3 Rotation;

		// Token: 0x04000F1F RID: 3871
		public Vector3 AngularVelocity;

		// Token: 0x04000F20 RID: 3872
		public bool IsInWater;

		// Token: 0x04000F21 RID: 3873
		public double LastNoiseTime;

		// Token: 0x04000F22 RID: 3874
		public ComponentCreature Owner;

		// Token: 0x04000F23 RID: 3875
		public ProjectileStoppedAction ProjectileStoppedAction;

		// Token: 0x04000F24 RID: 3876
		public ITrailParticleSystem TrailParticleSystem;

		// Token: 0x04000F25 RID: 3877
		public Vector3 TrailOffset;

		// Token: 0x04000F26 RID: 3878
		public bool NoChunk;

		// Token: 0x04000F27 RID: 3879
		public bool IsIncendiary;
	}
}
