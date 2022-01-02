using System;

namespace Game
{
	// Token: 0x0200027F RID: 639
	public class ElectricConnection
	{
		// Token: 0x04000D24 RID: 3364
		public CellFace CellFace;

		// Token: 0x04000D25 RID: 3365
		public int ConnectorFace;

		// Token: 0x04000D26 RID: 3366
		public ElectricConnectorType ConnectorType;

		// Token: 0x04000D27 RID: 3367
		public ElectricElement NeighborElectricElement;

		// Token: 0x04000D28 RID: 3368
		public CellFace NeighborCellFace;

		// Token: 0x04000D29 RID: 3369
		public int NeighborConnectorFace;

		// Token: 0x04000D2A RID: 3370
		public ElectricConnectorType NeighborConnectorType;
	}
}
