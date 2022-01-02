using System;
using Engine;

namespace Game
{
	// Token: 0x02000324 RID: 804
	public struct TerrainRaycastResult
	{
		// Token: 0x060017F2 RID: 6130 RVA: 0x000BB798 File Offset: 0x000B9998
		public Vector3 HitPoint(float offsetFromSurface = 0f)
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance + CellFace.FaceToVector3(this.CellFace.Face) * offsetFromSurface;
		}

		// Token: 0x0400109F RID: 4255
		public Ray3 Ray;

		// Token: 0x040010A0 RID: 4256
		public int Value;

		// Token: 0x040010A1 RID: 4257
		public CellFace CellFace;

		// Token: 0x040010A2 RID: 4258
		public int CollisionBoxIndex;

		// Token: 0x040010A3 RID: 4259
		public float Distance;
	}
}
