using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000308 RID: 776
	public class SortedMultiCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060016D4 RID: 5844 RVA: 0x000AC42C File Offset: 0x000AA62C
		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060016D5 RID: 5845 RVA: 0x000AC434 File Offset: 0x000AA634
		// (set) Token: 0x060016D6 RID: 5846 RVA: 0x000AC440 File Offset: 0x000AA640
		public int Capacity
		{
			get
			{
				return this.m_array.Length;
			}
			set
			{
				value = Math.Max(Math.Max(4, this.m_count), value);
				if (value != this.m_array.Length)
				{
					KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[value];
					Array.Copy(this.m_array, array, this.m_count);
					this.m_array = array;
				}
			}
		}

		// Token: 0x1700037A RID: 890
		public KeyValuePair<TKey, TValue> this[int i]
		{
			get
			{
				if (i < this.m_count)
				{
					return this.m_array[i];
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x000AC4A9 File Offset: 0x000AA6A9
		public SortedMultiCollection()
		{
			this.m_array = new KeyValuePair<TKey, TValue>[4];
			this.m_comparer = Comparer<TKey>.Default;
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x000AC4C8 File Offset: 0x000AA6C8
		public SortedMultiCollection(IComparer<TKey> comparer)
		{
			this.m_array = new KeyValuePair<TKey, TValue>[4];
			this.m_comparer = comparer;
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x000AC4E3 File Offset: 0x000AA6E3
		public SortedMultiCollection(int capacity) : this(capacity, null)
		{
			capacity = Math.Max(capacity, 4);
			this.m_array = new KeyValuePair<TKey, TValue>[capacity];
			this.m_comparer = Comparer<TKey>.Default;
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x000AC50D File Offset: 0x000AA70D
		public SortedMultiCollection(int capacity, IComparer<TKey> comparer)
		{
			capacity = Math.Max(capacity, 4);
			this.m_array = new KeyValuePair<TKey, TValue>[capacity];
			this.m_comparer = comparer;
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x000AC534 File Offset: 0x000AA734
		public void Add(TKey key, TValue value)
		{
			int num = this.Find(key);
			if (num < 0)
			{
				num = ~num;
			}
			this.EnsureCapacity(this.m_count + 1);
			Array.Copy(this.m_array, num, this.m_array, num + 1, this.m_count - num);
			this.m_array[num] = new KeyValuePair<TKey, TValue>(key, value);
			this.m_count++;
			this.m_version++;
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x000AC5AC File Offset: 0x000AA7AC
		public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in items)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x000AC604 File Offset: 0x000AA804
		public bool Remove(TKey key)
		{
			int num = this.Find(key);
			if (num >= 0)
			{
				Array.Copy(this.m_array, num + 1, this.m_array, num, this.m_count - num - 1);
				this.m_array[this.m_count - 1] = default(KeyValuePair<TKey, TValue>);
				this.m_count--;
				this.m_version++;
				return true;
			}
			return false;
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x000AC674 File Offset: 0x000AA874
		public void Clear()
		{
			for (int i = 0; i < this.m_count; i++)
			{
				this.m_array[i] = default(KeyValuePair<TKey, TValue>);
			}
			this.m_count = 0;
			this.m_version++;
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x000AC6BC File Offset: 0x000AA8BC
		public bool TryGetValue(TKey key, out TValue value)
		{
			int num = this.Find(key);
			if (num >= 0)
			{
				value = this.m_array[num].Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x000AC6F6 File Offset: 0x000AA8F6
		public bool ContainsKey(TKey key)
		{
			return this.Find(key) >= 0;
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x000AC705 File Offset: 0x000AA905
		public SortedMultiCollection<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x000AC70D File Offset: 0x000AA90D
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x000AC71A File Offset: 0x000AA91A
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x000AC727 File Offset: 0x000AA927
		public void EnsureCapacity(int capacity)
		{
			if (capacity > this.Capacity)
			{
				this.Capacity = Math.Max(capacity, 2 * this.Capacity);
			}
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x000AC748 File Offset: 0x000AA948
		public int Find(TKey key)
		{
			if (this.m_count > 0)
			{
				int i = 0;
				int num = this.m_count - 1;
				while (i <= num)
				{
					int num2 = i + num >> 1;
					int num3 = this.m_comparer.Compare(this.m_array[num2].Key, key);
					if (num3 == 0)
					{
						return num2;
					}
					if (num3 < 0)
					{
						i = num2 + 1;
					}
					else
					{
						num = num2 - 1;
					}
				}
				return ~i;
			}
			return -1;
		}

		// Token: 0x04000F92 RID: 3986
		public const int MinCapacity = 4;

		// Token: 0x04000F93 RID: 3987
		public KeyValuePair<TKey, TValue>[] m_array;

		// Token: 0x04000F94 RID: 3988
		public int m_count;

		// Token: 0x04000F95 RID: 3989
		public int m_version;

		// Token: 0x04000F96 RID: 3990
		public IComparer<TKey> m_comparer;

		// Token: 0x0200053E RID: 1342
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			// Token: 0x17000589 RID: 1417
			// (get) Token: 0x06002237 RID: 8759 RVA: 0x000EC035 File Offset: 0x000EA235
			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x1700058A RID: 1418
			// (get) Token: 0x06002238 RID: 8760 RVA: 0x000EC03D File Offset: 0x000EA23D
			object IEnumerator.Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x06002239 RID: 8761 RVA: 0x000EC04A File Offset: 0x000EA24A
			internal Enumerator(SortedMultiCollection<TKey, TValue> collection)
			{
				this.m_collection = collection;
				this.m_current = default(KeyValuePair<TKey, TValue>);
				this.m_index = 0;
				this.m_version = collection.m_version;
			}

			// Token: 0x0600223A RID: 8762 RVA: 0x000EC072 File Offset: 0x000EA272
			public void Dispose()
			{
			}

			// Token: 0x0600223B RID: 8763 RVA: 0x000EC074 File Offset: 0x000EA274
			public bool MoveNext()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				if (this.m_index < this.m_collection.m_count)
				{
					this.m_current = this.m_collection.m_array[this.m_index];
					this.m_index++;
					return true;
				}
				this.m_current = default(KeyValuePair<TKey, TValue>);
				return false;
			}

			// Token: 0x0600223C RID: 8764 RVA: 0x000EC0EB File Offset: 0x000EA2EB
			public void Reset()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				this.m_index = 0;
				this.m_current = default(KeyValuePair<TKey, TValue>);
			}

			// Token: 0x040018E5 RID: 6373
			public SortedMultiCollection<TKey, TValue> m_collection;

			// Token: 0x040018E6 RID: 6374
			public KeyValuePair<TKey, TValue> m_current;

			// Token: 0x040018E7 RID: 6375
			public int m_index;

			// Token: 0x040018E8 RID: 6376
			public int m_version;
		}
	}
}
