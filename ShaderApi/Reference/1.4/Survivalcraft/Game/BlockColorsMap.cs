using System;
using Engine;

namespace Game
{
	// Token: 0x02000246 RID: 582
	public class BlockColorsMap
	{
		// Token: 0x0600133B RID: 4923 RVA: 0x0008C504 File Offset: 0x0008A704
		public BlockColorsMap(Color th11, Color th21, Color th12, Color th22)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					float f = MathUtils.Saturate((float)i / 8f);
					float f2 = MathUtils.Saturate((float)(j - 4) / 10f);
					Color c = Color.Lerp(th11, th21, f);
					Color c2 = Color.Lerp(th12, th22, f);
					Color color = Color.Lerp(c, c2, f2);
					int num = i + j * 16;
					this.m_map[num] = color;
				}
			}
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x0008C594 File Offset: 0x0008A794
		public Color Lookup(int temperature, int humidity)
		{
			int num = MathUtils.Clamp(temperature, 0, 15) + 16 * MathUtils.Clamp(humidity, 0, 15);
			return this.m_map[num];
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x0008C5C4 File Offset: 0x0008A7C4
		public Color Lookup(Terrain terrain, int x, int y, int z)
		{
			int shaftValue = terrain.GetShaftValue(x, z);
			int temperature = terrain.GetSeasonalTemperature(shaftValue) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
			int seasonalHumidity = terrain.GetSeasonalHumidity(shaftValue);
			return this.Lookup(temperature, seasonalHumidity);
		}

		// Token: 0x04000BFA RID: 3066
		public Color[] m_map = new Color[256];

		// Token: 0x04000BFB RID: 3067
		public static BlockColorsMap WaterColorsMap = new BlockColorsMap(new Color(0, 0, 128), new Color(0, 80, 100), new Color(0, 45, 85), new Color(0, 113, 97));

		// Token: 0x04000BFC RID: 3068
		public static BlockColorsMap GrassColorsMap = new BlockColorsMap(new Color(141, 198, 166), new Color(210, 201, 93), new Color(141, 198, 166), new Color(79, 225, 56));

		// Token: 0x04000BFD RID: 3069
		public static BlockColorsMap OakLeavesColorsMap = new BlockColorsMap(new Color(96, 161, 123), new Color(174, 164, 42), new Color(96, 161, 123), new Color(30, 191, 1));

		// Token: 0x04000BFE RID: 3070
		public static BlockColorsMap BirchLeavesColorsMap = new BlockColorsMap(new Color(96, 161, 96), new Color(174, 109, 42), new Color(96, 161, 96), new Color(107, 191, 1));

		// Token: 0x04000BFF RID: 3071
		public static BlockColorsMap SpruceLeavesColorsMap = new BlockColorsMap(new Color(96, 161, 150), new Color(129, 174, 42), new Color(96, 161, 150), new Color(1, 191, 53));

		// Token: 0x04000C00 RID: 3072
		public static BlockColorsMap TallSpruceLeavesColorsMap = new BlockColorsMap(new Color(90, 141, 160), new Color(119, 152, 51), new Color(86, 141, 162), new Color(1, 158, 65));

		// Token: 0x04000C01 RID: 3073
		public static BlockColorsMap MimosaLeavesColorsMap = new BlockColorsMap(new Color(146, 191, 176), new Color(160, 191, 176), new Color(146, 191, 166), new Color(150, 201, 141));

		// Token: 0x04000C02 RID: 3074
		public static BlockColorsMap IvyColorsMap = new BlockColorsMap(new Color(96, 161, 123), new Color(174, 164, 42), new Color(96, 161, 123), new Color(30, 191, 1));

		// Token: 0x04000C03 RID: 3075
		public static BlockColorsMap KelpColorsMap = new BlockColorsMap(new Color(80, 110, 90), new Color(110, 110, 50), new Color(80, 110, 90), new Color(110, 110, 50));

		// Token: 0x04000C04 RID: 3076
		public static BlockColorsMap SeagrassColorsMap = new BlockColorsMap(new Color(50, 120, 110), new Color(80, 120, 70), new Color(50, 120, 110), new Color(80, 120, 70));
	}
}
