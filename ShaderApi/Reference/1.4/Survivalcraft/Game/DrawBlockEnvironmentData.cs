using System;
using Engine;

namespace Game
{
	// Token: 0x02000276 RID: 630
	public class DrawBlockEnvironmentData
	{
		// Token: 0x0600143B RID: 5179 RVA: 0x00096E0D File Offset: 0x0009500D
		public DrawBlockEnvironmentData()
		{
			this.InWorldMatrix = Matrix.Identity;
			this.Humidity = 15;
			this.Temperature = 8;
			this.Light = 15;
		}

		// Token: 0x04000CC8 RID: 3272
		public SubsystemTerrain SubsystemTerrain;

		// Token: 0x04000CC9 RID: 3273
		public Matrix InWorldMatrix;

		// Token: 0x04000CCA RID: 3274
		public Matrix? ViewProjectionMatrix;

		// Token: 0x04000CCB RID: 3275
		public Vector3? BillboardDirection;

		// Token: 0x04000CCC RID: 3276
		public int Humidity;

		// Token: 0x04000CCD RID: 3277
		public int Temperature;

		// Token: 0x04000CCE RID: 3278
		public int Light;
	}
}
