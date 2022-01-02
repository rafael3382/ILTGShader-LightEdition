using System;
using Engine;

namespace Game
{
	// Token: 0x020002DA RID: 730
	public class PathfindingResult
	{
		// Token: 0x04000E7A RID: 3706
		public volatile bool IsCompleted;

		// Token: 0x04000E7B RID: 3707
		public bool IsInProgress;

		// Token: 0x04000E7C RID: 3708
		public float PathCost;

		// Token: 0x04000E7D RID: 3709
		public int PositionsChecked;

		// Token: 0x04000E7E RID: 3710
		public DynamicArray<Vector3> Path = new DynamicArray<Vector3>();
	}
}
