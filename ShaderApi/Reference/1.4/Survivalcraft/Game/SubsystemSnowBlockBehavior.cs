using System;

namespace Game
{
	// Token: 0x020001D2 RID: 466
	public class SubsystemSnowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x0005A381 File Offset: 0x00058581
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					61
				};
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0005A38E File Offset: 0x0005858E
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (!SubsystemSnowBlockBehavior.CanSupportSnow(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z)))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0005A3C0 File Offset: 0x000585C0
		public static bool CanSupportSnow(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return !block.IsTransparent_(value) || block is LeavesBlock;
		}
	}
}
