using System;

namespace Game
{
	// Token: 0x02000280 RID: 640
	public class ElectricConnectionPath
	{
		// Token: 0x0600145F RID: 5215 RVA: 0x00099030 File Offset: 0x00097230
		public ElectricConnectionPath(int neighborOffsetX, int neighborOffsetY, int neighborOffsetZ, int neighborFace, int connectorFace, int neighborConnectorFace)
		{
			this.NeighborOffsetX = neighborOffsetX;
			this.NeighborOffsetY = neighborOffsetY;
			this.NeighborOffsetZ = neighborOffsetZ;
			this.NeighborFace = neighborFace;
			this.ConnectorFace = connectorFace;
			this.NeighborConnectorFace = neighborConnectorFace;
		}

		// Token: 0x04000D2B RID: 3371
		public readonly int NeighborOffsetX;

		// Token: 0x04000D2C RID: 3372
		public readonly int NeighborOffsetY;

		// Token: 0x04000D2D RID: 3373
		public readonly int NeighborOffsetZ;

		// Token: 0x04000D2E RID: 3374
		public readonly int NeighborFace;

		// Token: 0x04000D2F RID: 3375
		public readonly int ConnectorFace;

		// Token: 0x04000D30 RID: 3376
		public readonly int NeighborConnectorFace;
	}
}
