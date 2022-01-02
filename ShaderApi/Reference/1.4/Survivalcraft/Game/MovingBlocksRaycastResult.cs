using System;
using Engine;

namespace Game
{
	// Token: 0x020000B6 RID: 182
	public struct MovingBlocksRaycastResult
	{
		// Token: 0x06000385 RID: 901 RVA: 0x00015526 File Offset: 0x00013726
		public Vector3 HitPoint()
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance;
		}

		// Token: 0x040001A1 RID: 417
		public Ray3 Ray;

		// Token: 0x040001A2 RID: 418
		public IMovingBlockSet MovingBlockSet;

		// Token: 0x040001A3 RID: 419
		public float Distance;
	}
}
