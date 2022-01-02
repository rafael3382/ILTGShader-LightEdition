using System;

namespace Game
{
	// Token: 0x020001B2 RID: 434
	public class SubsystemInWaterBlockBehavior : SubsystemWaterBlockBehavior
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0004AF75 File Offset: 0x00049175
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0004AF80 File Offset: 0x00049180
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			int level = FluidBlock.GetLevel(Terrain.ExtractData(blockValue));
			newBlockValue = Terrain.MakeBlockValue(18, 0, FluidBlock.SetLevel(0, level));
			dropValue.Value = Terrain.MakeBlockValue(Terrain.ExtractContents(blockValue));
			dropValue.Count = 1;
		}
	}
}
