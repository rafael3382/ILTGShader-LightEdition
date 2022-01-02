using System;
using System.Diagnostics;
using Engine;

namespace Game
{
	// Token: 0x020002F1 RID: 753
	public class Random
	{
		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06001675 RID: 5749 RVA: 0x000A9585 File Offset: 0x000A7785
		// (set) Token: 0x06001676 RID: 5750 RVA: 0x000A9599 File Offset: 0x000A7799
		public ulong State
		{
			get
			{
				return (ulong)this.m_s0 + ((ulong)this.m_s1 << 32);
			}
			set
			{
				this.m_s0 = (uint)value;
				this.m_s1 = (uint)(value >> 32);
			}
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x000A95AE File Offset: 0x000A77AE
		public Random()
		{
			this.Seed();
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x000A95BC File Offset: 0x000A77BC
		public Random(int seed)
		{
			this.Seed(seed);
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x000A95CB File Offset: 0x000A77CB
		public void Seed()
		{
			this.Seed(Game.Random.m_counter++);
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x000A95E0 File Offset: 0x000A77E0
		public void Seed(int seed)
		{
			this.m_s0 = MathUtils.Hash((uint)seed);
			this.m_s1 = MathUtils.Hash((uint)(seed + 1));
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x000A95FC File Offset: 0x000A77FC
		public int Sign()
		{
			return this.Int() % 2 * 2 - 1;
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x000A960A File Offset: 0x000A780A
		public bool Bool()
		{
			return (this.Int() & 1) != 0;
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x000A9617 File Offset: 0x000A7817
		public bool Bool(float probability)
		{
			return (float)this.Int() / 2.147484E+09f < probability;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x000A962C File Offset: 0x000A782C
		public uint UInt()
		{
			uint s = this.m_s0;
			uint num = this.m_s1;
			num ^= s;
			this.m_s0 = (Game.Random.RotateLeft(s, 26) ^ num ^ num << 9);
			this.m_s1 = Game.Random.RotateLeft(num, 13);
			return Game.Random.RotateLeft(s * 2654435771U, 5) * 5U;
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x000A967D File Offset: 0x000A787D
		public int Int()
		{
			return (int)(this.UInt() & 2147483647U);
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x000A968B File Offset: 0x000A788B
		public int Int(int bound)
		{
			return (int)((long)this.Int() * (long)bound / (long)((ulong)int.MinValue));
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x000A969F File Offset: 0x000A789F
		public int Int(int min, int max)
		{
			return (int)((long)min + (long)this.Int() * (long)(max - min + 1) / (long)((ulong)int.MinValue));
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x000A96BA File Offset: 0x000A78BA
		public float Float()
		{
			return (float)this.Int() / 2.147484E+09f;
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x000A96C9 File Offset: 0x000A78C9
		public float Float(float min, float max)
		{
			return min + this.Float() * (max - min);
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x000A96D8 File Offset: 0x000A78D8
		public float NormalFloat(float mean, float stddev)
		{
			float num = this.Float();
			if ((double)num < 0.5)
			{
				float num2 = MathUtils.Sqrt(-2f * MathUtils.Log(num));
				float num3 = 0.322232425f + num2 * (1f + num2 * (0.3422421f + num2 * (0.0204231218f + num2 * 4.536422E-05f)));
				float num4 = 0.09934846f + num2 * (0.588581562f + num2 * (0.5311035f + num2 * (0.103537753f + num2 * 0.00385607f)));
				return mean + stddev * (num3 / num4 - num2);
			}
			float num5 = MathUtils.Sqrt(-2f * MathUtils.Log(1f - num));
			float num6 = 0.322232425f + num5 * (1f + num5 * (0.3422421f + num5 * (0.0204231218f + num5 * 4.536422E-05f)));
			float num7 = 0.09934846f + num5 * (0.588581562f + num5 * (0.5311035f + num5 * (0.103537753f + num5 * 0.00385607f)));
			return mean - stddev * (num6 / num7 - num5);
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x000A97DC File Offset: 0x000A79DC
		public Vector2 Vector2()
		{
			float num;
			float num2;
			float num3;
			float num4;
			float num5;
			do
			{
				num = 2f * this.Float() - 1f;
				num2 = 2f * this.Float() - 1f;
				num3 = num * num;
				num4 = num2 * num2;
				num5 = num3 + num4;
			}
			while (num5 >= 1f);
			float num6 = 1f / num5;
			return new Vector2((num3 - num4) * num6, 2f * num * num2 * num6);
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x000A9846 File Offset: 0x000A7A46
		public Vector2 Vector2(float length)
		{
			return Engine.Vector2.Normalize(this.Vector2()) * length;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x000A9859 File Offset: 0x000A7A59
		public Vector2 Vector2(float minLength, float maxLength)
		{
			return Engine.Vector2.Normalize(this.Vector2()) * this.Float(minLength, maxLength);
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x000A9874 File Offset: 0x000A7A74
		public Vector3 Vector3()
		{
			float num;
			float num2;
			float num3;
			do
			{
				num = 2f * this.Float() - 1f;
				num2 = 2f * this.Float() - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f);
			float num4 = MathUtils.Sqrt(1f - num3);
			return new Vector3(2f * num * num4, 2f * num2 * num4, 1f - 2f * num3);
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x000A98E8 File Offset: 0x000A7AE8
		public Vector3 Vector3(float length)
		{
			return Engine.Vector3.Normalize(this.Vector3()) * length;
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x000A98FB File Offset: 0x000A7AFB
		public Vector3 Vector3(float minLength, float maxLength)
		{
			return Engine.Vector3.Normalize(this.Vector3()) * this.Float(minLength, maxLength);
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x000A9915 File Offset: 0x000A7B15
		public static uint RotateLeft(uint x, int k)
		{
			return x << k | x >> 32 - k;
		}

		// Token: 0x04000F3E RID: 3902
		public static int m_counter = (int)(Stopwatch.GetTimestamp() + DateTime.Now.Ticks);

		// Token: 0x04000F3F RID: 3903
		public uint m_s0;

		// Token: 0x04000F40 RID: 3904
		public uint m_s1;
	}
}
