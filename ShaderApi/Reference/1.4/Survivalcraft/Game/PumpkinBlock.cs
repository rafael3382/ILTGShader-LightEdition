using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x020000CB RID: 203
	public class PumpkinBlock : BasePumpkinBlock
	{
		// Token: 0x0600043A RID: 1082 RVA: 0x00018723 File Offset: 0x00016923
		public PumpkinBlock() : base(false)
		{
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0001872C File Offset: 0x0001692C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			int data = Terrain.ExtractData(oldValue);
			if (BasePumpkinBlock.GetSize(data) == 7 && !BasePumpkinBlock.GetIsDead(data) && this.Random.Bool(0.5f))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = 248,
					Count = 1
				});
			}
		}

		// Token: 0x040001DB RID: 475
		public const int Index = 131;
	}
}
