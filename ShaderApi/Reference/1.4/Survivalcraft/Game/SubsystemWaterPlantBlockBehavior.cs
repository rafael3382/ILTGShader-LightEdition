using System;

namespace Game
{
	// Token: 0x020001E2 RID: 482
	public class SubsystemWaterPlantBlockBehavior : SubsystemInWaterBlockBehavior
	{
		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000D24 RID: 3364 RVA: 0x0005F3C0 File Offset: 0x0005D5C0
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0005F3C8 File Offset: 0x0005D5C8
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			int num = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z));
			int num2 = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z));
			if (num2 != 2 && num2 != 7 && num2 != 72 && num2 != num)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
