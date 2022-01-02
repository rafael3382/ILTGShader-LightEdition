using System;
using Engine;

namespace Game
{
	// Token: 0x020002DC RID: 732
	public class Pickable : WorldItem
	{
		// Token: 0x04000E80 RID: 3712
		public int Count;

		// Token: 0x04000E81 RID: 3713
		public Vector3? FlyToPosition;

		// Token: 0x04000E82 RID: 3714
		public Matrix? StuckMatrix;

		// Token: 0x04000E83 RID: 3715
		public bool SplashGenerated = true;
	}
}
