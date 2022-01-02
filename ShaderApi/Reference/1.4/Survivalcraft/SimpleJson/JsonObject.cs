using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000007 RID: 7
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x17000001 RID: 1
		public object this[int index]
		{
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600003D RID: 61 RVA: 0x0000576B File Offset: 0x0000396B
		public ICollection<string> Keys
		{
			get
			{
				return this._members.Keys;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00005778 File Offset: 0x00003978
		public ICollection<object> Values
		{
			get
			{
				return this._members.Values;
			}
		}

		// Token: 0x17000004 RID: 4
		public object this[string key]
		{
			get
			{
				return this._members[key];
			}
			set
			{
				this._members[key] = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000057A2 File Offset: 0x000039A2
		public int Count
		{
			get
			{
				return this._members.Count;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000057AF File Offset: 0x000039AF
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000057B2 File Offset: 0x000039B2
		public JsonObject()
		{
			this._members = new Dictionary<string, object>();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000057C5 File Offset: 0x000039C5
		public JsonObject(IEqualityComparer<string> comparer)
		{
			this._members = new Dictionary<string, object>(comparer);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000057DC File Offset: 0x000039DC
		internal static object GetAtIndex(IDictionary<string, object> obj, int index)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (index >= obj.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = 0;
			foreach (KeyValuePair<string, object> keyValuePair in obj)
			{
				if (num++ == index)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005858 File Offset: 0x00003A58
		public void Add(string key, object value)
		{
			this._members.Add(key, value);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005867 File Offset: 0x00003A67
		public bool ContainsKey(string key)
		{
			return this._members.ContainsKey(key);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005875 File Offset: 0x00003A75
		public bool Remove(string key)
		{
			return this._members.Remove(key);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00005883 File Offset: 0x00003A83
		public bool TryGetValue(string key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005892 File Offset: 0x00003A92
		public void Add(KeyValuePair<string, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000058AD File Offset: 0x00003AAD
		public void Clear()
		{
			this._members.Clear();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000058BA File Offset: 0x00003ABA
		public bool Contains(KeyValuePair<string, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000058F0 File Offset: 0x00003AF0
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = this.Count;
			foreach (KeyValuePair<string, object> keyValuePair in this)
			{
				array[arrayIndex++] = keyValuePair;
				if (--num <= 0)
				{
					break;
				}
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005960 File Offset: 0x00003B60
		public bool Remove(KeyValuePair<string, object> item)
		{
			return this._members.Remove(item.Key);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005974 File Offset: 0x00003B74
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005986 File Offset: 0x00003B86
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005998 File Offset: 0x00003B98
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this);
		}

		// Token: 0x04000038 RID: 56
		public readonly Dictionary<string, object> _members;
	}
}
