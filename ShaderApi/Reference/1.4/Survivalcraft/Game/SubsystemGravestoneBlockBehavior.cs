using System;

namespace Game
{
	// Token: 0x020001AF RID: 431
	public class SubsystemGravestoneBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0004AE26 File Offset: 0x00049026
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					189
				};
			}
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0004AE38 File Offset: 0x00049038
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
