using System;
using Engine;

namespace Game
{
	// Token: 0x02000266 RID: 614
	public static class DataSizeFormatter
	{
		// Token: 0x060013ED RID: 5101 RVA: 0x000951CC File Offset: 0x000933CC
		public static string Format(long bytes)
		{
			if (bytes < 1024L)
			{
				return string.Format("{0}B", bytes);
			}
			if (bytes < 1048576L)
			{
				float num = (float)bytes / 1024f;
				return string.Format(DataSizeFormatter.PrepareFormatString(num, "kB"), num);
			}
			if (bytes < 1073741824L)
			{
				float num2 = (float)bytes / 1024f / 1024f;
				return string.Format(DataSizeFormatter.PrepareFormatString(num2, "MB"), num2);
			}
			float num3 = (float)bytes / 1024f / 1024f / 1024f;
			return string.Format(DataSizeFormatter.PrepareFormatString(num3, "GB"), num3);
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x00095278 File Offset: 0x00093478
		public static string PrepareFormatString(float value, string unit)
		{
			int num = (int)(MathUtils.Log10(value) + 1f);
			return "{0:F" + MathUtils.Max(3 - num, 0).ToString() + "}" + unit;
		}
	}
}
