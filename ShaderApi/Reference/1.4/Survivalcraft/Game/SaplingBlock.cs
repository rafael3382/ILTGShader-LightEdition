using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x020000E8 RID: 232
	public class SaplingBlock : CrossBlock
	{
		// Token: 0x06000480 RID: 1152 RVA: 0x000199A4 File Offset: 0x00017BA4
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return "橡树树苗";
			case 1:
				return "白桦树苗";
			case 2:
				return "云杉树苗";
			case 3:
				return "高云杉树苗";
			case 4:
				return "合金欢树幼苗";
			default:
				return "树苗";
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x000199F8 File Offset: 0x00017BF8
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return 56;
			case 1:
				return 72;
			case 2:
				return 73;
			case 3:
				return 73;
			case 4:
				return 72;
			default:
				return 56;
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00019A39 File Offset: 0x00017C39
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(119, 0, 0);
			yield return Terrain.MakeBlockValue(119, 0, 1);
			yield return Terrain.MakeBlockValue(119, 0, 2);
			yield return Terrain.MakeBlockValue(119, 0, 3);
			yield return Terrain.MakeBlockValue(119, 0, 4);
			yield break;
		}

		// Token: 0x04000204 RID: 516
		public const int Index = 119;
	}
}
