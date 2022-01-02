using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200025F RID: 607
	public static class CollectionUtils
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x000947DC File Offset: 0x000929DC
		public static T ElementAt<T, E>(E enumerator, int index) where E : IEnumerator<T>
		{
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
				if (num > index)
				{
					return enumerator.Current;
				}
			}
			throw new IndexOutOfRangeException("ElementAt() index out of range.");
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0009481C File Offset: 0x00092A1C
		public static void RandomShuffle<T>(this IList<T> list, Func<int, int> random)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int index = random(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x00094864 File Offset: 0x00092A64
		public static int FirstIndex<T>(this IEnumerable<T> collection, T value)
		{
			int num = 0;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (object.Equals(enumerator.Current, value))
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x000948C4 File Offset: 0x00092AC4
		public static int FirstIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num = 0;
			foreach (T arg in collection)
			{
				if (predicate(arg))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0009491C File Offset: 0x00092B1C
		public static T SelectNth<T>(this IList<T> list, int n, IComparer<T> comparer)
		{
			if (list == null || list.Count <= n)
			{
				throw new ArgumentException();
			}
			int i = 0;
			int num = list.Count - 1;
			while (i < num)
			{
				int j = i;
				int num2 = num;
				T y = list[(j + num2) / 2];
				while (j < num2)
				{
					if (comparer.Compare(list[j], y) >= 0)
					{
						T value = list[num2];
						list[num2] = list[j];
						list[j] = value;
						num2--;
					}
					else
					{
						j++;
					}
				}
				if (comparer.Compare(list[j], y) > 0)
				{
					j--;
				}
				if (n <= j)
				{
					num = j;
				}
				else
				{
					i = j + 1;
				}
			}
			return list[n];
		}
	}
}
