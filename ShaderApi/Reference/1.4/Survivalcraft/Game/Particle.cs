using System;
using Engine;

namespace Game
{
	// Token: 0x020002D6 RID: 726
	public class Particle
	{
		// Token: 0x04000E63 RID: 3683
		public bool IsActive;

		// Token: 0x04000E64 RID: 3684
		public Vector3 Position;

		// Token: 0x04000E65 RID: 3685
		public Vector2 Size;

		// Token: 0x04000E66 RID: 3686
		public float Rotation;

		// Token: 0x04000E67 RID: 3687
		public Color Color;

		// Token: 0x04000E68 RID: 3688
		public int TextureSlot;

		// Token: 0x04000E69 RID: 3689
		public bool UseAdditiveBlending;

		// Token: 0x04000E6A RID: 3690
		public bool FlipX;

		// Token: 0x04000E6B RID: 3691
		public bool FlipY;

		// Token: 0x04000E6C RID: 3692
		public ParticleBillboardingMode BillboardingMode;
	}
}
