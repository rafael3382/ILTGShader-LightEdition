using System;
using Engine;

namespace Game
{
	// Token: 0x0200024F RID: 591
	public struct BodyRaycastResult
	{
		// Token: 0x06001374 RID: 4980 RVA: 0x000929F3 File Offset: 0x00090BF3
		public Vector3 HitPoint()
		{
			return this.Ray.Position + this.Ray.Direction * this.Distance;
		}

		// Token: 0x04000C24 RID: 3108
		public Ray3 Ray;

		// Token: 0x04000C25 RID: 3109
		public ComponentBody ComponentBody;

		// Token: 0x04000C26 RID: 3110
		public float Distance;
	}
}
