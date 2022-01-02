using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EB RID: 235
	public class SeedsBlock : FlatBlock
	{
		// Token: 0x06000490 RID: 1168 RVA: 0x00019F8C File Offset: 0x0001818C
		public override IEnumerable<int> GetCreativeValues()
		{
			List<int> list = new List<int>();
			foreach (int data in EnumUtils.GetEnumValues(typeof(SeedsBlock.SeedType)))
			{
				list.Add(Terrain.MakeBlockValue(173, 0, data));
			}
			return list;
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00019FF4 File Offset: 0x000181F4
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return "高草种子";
			case 1:
				return "红花种子";
			case 2:
				return "紫花";
			case 3:
				return "白花种子";
			case 4:
				return "野生小麦种子";
			case 5:
				return "小麦种子";
			case 6:
				return "棉花种子";
			case 7:
				return "南瓜种子";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001A068 File Offset: 0x00018268
		public override int GetFaceTextureSlot(int face, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num == 5 || num == 4)
			{
				return 74;
			}
			return 75;
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0001A08C File Offset: 0x0001828C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			BlockPlacementData result = default(BlockPlacementData);
			result.CellFace = raycastResult.CellFace;
			if (raycastResult.CellFace.Face == 4)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					result.Value = Terrain.MakeBlockValue(19, 0, TallGrassBlock.SetIsSmall(0, true));
					break;
				case 1:
					result.Value = Terrain.MakeBlockValue(20, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 2:
					result.Value = Terrain.MakeBlockValue(24, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 3:
					result.Value = Terrain.MakeBlockValue(25, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 4:
					result.Value = Terrain.MakeBlockValue(174, 0, RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0));
					break;
				case 5:
					result.Value = Terrain.MakeBlockValue(174, 0, RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0));
					break;
				case 6:
					result.Value = Terrain.MakeBlockValue(204, 0, CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0));
					break;
				case 7:
					result.Value = Terrain.MakeBlockValue(131, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0));
					break;
				}
			}
			return result;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001A1E4 File Offset: 0x000183E4
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				color *= new Color(160, 150, 125);
				break;
			case 1:
				color *= new Color(192, 160, 160);
				break;
			case 2:
				color *= new Color(192, 160, 192);
				break;
			case 3:
				color *= new Color(192, 192, 192);
				break;
			case 4:
				color *= new Color(60, 138, 76);
				break;
			case 6:
				color *= new Color(255, 255, 255);
				break;
			case 7:
				color *= new Color(240, 225, 190);
				break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x0400020C RID: 524
		public const int Index = 173;

		// Token: 0x02000408 RID: 1032
		public enum SeedType
		{
			// Token: 0x040014F1 RID: 5361
			TallGrass,
			// Token: 0x040014F2 RID: 5362
			RedFlower,
			// Token: 0x040014F3 RID: 5363
			PurpleFlower,
			// Token: 0x040014F4 RID: 5364
			WhiteFlower,
			// Token: 0x040014F5 RID: 5365
			WildRye,
			// Token: 0x040014F6 RID: 5366
			Rye,
			// Token: 0x040014F7 RID: 5367
			Cotton,
			// Token: 0x040014F8 RID: 5368
			Pumpkin
		}
	}
}
