using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200030B RID: 779
	public class SpawnChunk
	{
		// Token: 0x04000FA3 RID: 4003
		public Point2 Point;

		// Token: 0x04000FA4 RID: 4004
		public bool IsSpawned;

		// Token: 0x04000FA5 RID: 4005
		public double? LastVisitedTime;

		// Token: 0x04000FA6 RID: 4006
		public List<SpawnEntityData> SpawnsData = new List<SpawnEntityData>();
	}
}
