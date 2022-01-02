using System;

namespace Game
{
	// Token: 0x020001A3 RID: 419
	public class SubsystemFenceBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x000458ED File Offset: 0x00043AED
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x000458F8 File Offset: 0x00043AF8
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			this.UpdateVariant(cellValue, x, y, z);
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00045923 File Offset: 0x00043B23
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateVariant(value, x, y, z);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00045934 File Offset: 0x00043B34
		public void UpdateVariant(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractContents(value);
			FenceBlock fenceBlock = BlocksManager.Blocks[num] as FenceBlock;
			if (fenceBlock != null)
			{
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + 1, y, z);
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x - 1, y, z);
				int cellValue3 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z + 1);
				int cellValue4 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z - 1);
				int num2 = 0;
				if (fenceBlock.ShouldConnectTo(cellValue))
				{
					num2++;
				}
				if (fenceBlock.ShouldConnectTo(cellValue2))
				{
					num2 += 2;
				}
				if (fenceBlock.ShouldConnectTo(cellValue3))
				{
					num2 += 4;
				}
				if (fenceBlock.ShouldConnectTo(cellValue4))
				{
					num2 += 8;
				}
				int data = Terrain.ExtractData(value);
				int value2 = Terrain.ReplaceData(value, FenceBlock.SetVariant(data, num2));
				base.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
			}
		}
	}
}
