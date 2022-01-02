using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000078 RID: 120
	public class GravelBlock : CubeBlock
	{
		// Token: 0x060002B1 RID: 689 RVA: 0x000117E8 File Offset: 0x0000F9E8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel < this.RequiredToolLevel)
			{
				return;
			}
			if (this.Random.Float(0f, 1f) < 0.33f)
			{
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
				return;
			}
			int num = this.Random.Int(1, 3);
			for (int i = 0; i < num; i++)
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(79),
					Count = 1
				});
			}
		}

		// Token: 0x04000134 RID: 308
		public const int Index = 6;
	}
}
