using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000069 RID: 105
	public abstract class FlowerBlock : CrossBlock
	{
		// Token: 0x06000247 RID: 583 RVA: 0x0000F6A6 File Offset: 0x0000D8A6
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!FlowerBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return base.GetFaceTextureSlot(face, value);
			}
			return 11;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000F6C0 File Offset: 0x0000D8C0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (!FlowerBlock.GetIsSmall(data))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(Terrain.ExtractContents(oldValue), 0, data),
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000F70C File Offset: 0x0000D90C
		public override int GetShadowStrength(int value)
		{
			if (!FlowerBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return this.DefaultShadowStrength / 2;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000F72A File Offset: 0x0000D92A
		public static bool GetIsSmall(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000F732 File Offset: 0x0000D932
		public static int SetIsSmall(int data, bool isSmall)
		{
			if (!isSmall)
			{
				return data & -2;
			}
			return data | 1;
		}
	}
}
