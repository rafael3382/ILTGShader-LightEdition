using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200009A RID: 154
	public class LargeDryBushBlock : CrossBlock
	{
		// Token: 0x0600031B RID: 795 RVA: 0x00013918 File Offset: 0x00011B18
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			dropValues.Add(new BlockDropValue
			{
				Value = 23,
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0400016C RID: 364
		public const int Index = 99;
	}
}
