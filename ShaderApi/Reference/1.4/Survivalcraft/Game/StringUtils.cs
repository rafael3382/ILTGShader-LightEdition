using System;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x02000314 RID: 788
	public static class StringUtils
	{
		// Token: 0x06001708 RID: 5896 RVA: 0x000AD6D0 File Offset: 0x000AB8D0
		public static int Compare(StringBuilder s1, string s2)
		{
			int num = 0;
			while (num < s1.Length || num < s2.Length)
			{
				if (num > s1.Length)
				{
					return -1;
				}
				if (num > s2.Length)
				{
					return 1;
				}
				char c = s1[num];
				char c2 = s2[num];
				if (c < c2)
				{
					return -1;
				}
				if (c > c2)
				{
					return 1;
				}
				num++;
			}
			return 0;
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x000AD72C File Offset: 0x000AB92C
		public static int CalculateNumberLength(uint value, int numberBase)
		{
			if (numberBase < 2 || numberBase > 16)
			{
				throw new ArgumentException("Number base is out of range.");
			}
			int num = 0;
			do
			{
				num++;
				value /= (uint)numberBase;
			}
			while (value != 0U);
			return num;
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x000AD75C File Offset: 0x000AB95C
		public static int CalculateNumberLength(int value, int numberBase)
		{
			if (value >= 0)
			{
				return StringUtils.CalculateNumberLength((uint)value, numberBase);
			}
			return StringUtils.CalculateNumberLength((uint)(-(uint)value), numberBase) + 1;
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x000AD774 File Offset: 0x000AB974
		public static int CalculateNumberLength(ulong value, int numberBase)
		{
			if (numberBase < 2 || numberBase > 16)
			{
				throw new ArgumentException("Number base is out of range.");
			}
			int num = 0;
			do
			{
				num++;
				value /= (ulong)numberBase;
			}
			while (value != 0UL);
			return num;
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x000AD7A5 File Offset: 0x000AB9A5
		public static int CalculateNumberLength(long value, int numberBase)
		{
			if (value >= 0L)
			{
				return StringUtils.CalculateNumberLength((ulong)value, numberBase);
			}
			return StringUtils.CalculateNumberLength((ulong)(-(ulong)value), numberBase) + 1;
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x000AD7C0 File Offset: 0x000AB9C0
		public static void AppendNumber(this StringBuilder stringBuilder, uint value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			int val = StringUtils.CalculateNumberLength(value, numberBase);
			int repeatCount = Math.Max(padding, val);
			stringBuilder.Append(paddingCharacter, repeatCount);
			int num = 0;
			do
			{
				char value2 = StringUtils.m_digits[(int)(value % (uint)numberBase)];
				stringBuilder[stringBuilder.Length - num - 1] = value2;
				value /= (uint)numberBase;
				num++;
			}
			while (value != 0U);
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x000AD812 File Offset: 0x000ABA12
		public static void AppendNumber(this StringBuilder stringBuilder, int value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			if (value >= 0)
			{
				stringBuilder.AppendNumber((uint)value, padding, paddingCharacter, numberBase);
				return;
			}
			stringBuilder.Append('-');
			stringBuilder.AppendNumber((uint)(-(uint)value), padding - 1, paddingCharacter, numberBase);
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x000AD83C File Offset: 0x000ABA3C
		public static void AppendNumber(this StringBuilder stringBuilder, ulong value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			int val = StringUtils.CalculateNumberLength(value, numberBase);
			int repeatCount = Math.Max(padding, val);
			stringBuilder.Append(paddingCharacter, repeatCount);
			int num = 0;
			do
			{
				char value2 = StringUtils.m_digits[(int)(checked((IntPtr)(value % unchecked((ulong)numberBase))))];
				stringBuilder[stringBuilder.Length - num - 1] = value2;
				value /= (ulong)numberBase;
				num++;
			}
			while (value != 0UL);
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x000AD891 File Offset: 0x000ABA91
		public static void AppendNumber(this StringBuilder stringBuilder, long value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
		{
			if (value >= 0L)
			{
				stringBuilder.AppendNumber((ulong)value, padding, paddingCharacter, numberBase);
				return;
			}
			stringBuilder.Append('-');
			stringBuilder.AppendNumber((ulong)(-(ulong)value), padding - 1, paddingCharacter, numberBase);
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x000AD8BC File Offset: 0x000ABABC
		public static void AppendNumber(this StringBuilder stringBuilder, float value, int precision)
		{
			precision = Math.Min(Math.Max(precision, -30), 30);
			if (float.IsNegativeInfinity(value))
			{
				stringBuilder.Append("Infinity");
				return;
			}
			if (float.IsPositiveInfinity(value))
			{
				stringBuilder.Append("-Infinity");
				return;
			}
			if (float.IsNaN(value))
			{
				stringBuilder.Append("NaN");
				return;
			}
			float num = Math.Abs(value);
			if (num > 1E+19f)
			{
				stringBuilder.Append("NumberTooLarge");
				return;
			}
			float num2 = MathUtils.Pow(10f, (float)Math.Abs(precision));
			ulong num3 = (ulong)MathUtils.Floor(num);
			ulong num4 = (ulong)MathUtils.Round((num - MathUtils.Floor(num)) * num2);
			if ((float)num4 >= num2)
			{
				num3 += 1UL;
				num4 = 0UL;
			}
			if (value < 0f)
			{
				stringBuilder.Append('-');
			}
			stringBuilder.AppendNumber(num3, 0, '0', 10);
			if (precision > 0)
			{
				stringBuilder.Append('.');
				stringBuilder.AppendNumber(num4, precision, '0', 10);
				return;
			}
			if (precision < 0)
			{
				stringBuilder.Append('.');
				stringBuilder.AppendNumber(num4, -precision, '0', 10);
				while (stringBuilder[stringBuilder.Length - 1] == '0')
				{
					int length = stringBuilder.Length - 1;
					stringBuilder.Length = length;
				}
				if (stringBuilder[stringBuilder.Length - 1] == '.')
				{
					int length = stringBuilder.Length - 1;
					stringBuilder.Length = length;
				}
			}
		}

		// Token: 0x04000FC2 RID: 4034
		public static char[] m_digits = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};
	}
}
