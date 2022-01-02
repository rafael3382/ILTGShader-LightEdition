using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000286 RID: 646
	public static class EnumUtils
	{
		// Token: 0x06001481 RID: 5249 RVA: 0x00099698 File Offset: 0x00097898
		public static string GetEnumName(Type type, int value)
		{
			int num = EnumUtils.GetEnumValues(type).IndexOf(value);
			if (num >= 0)
			{
				return EnumUtils.GetEnumNames(type)[num];
			}
			return "<invalid enum>";
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000996C8 File Offset: 0x000978C8
		public static IList<string> GetEnumNames(Type type)
		{
			return EnumUtils.Cache.Query(type).Names;
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000996DA File Offset: 0x000978DA
		public static IList<int> GetEnumValues(Type type)
		{
			return EnumUtils.Cache.Query(type).Values;
		}

		// Token: 0x0200050A RID: 1290
		public struct NamesValues
		{
			// Token: 0x04001841 RID: 6209
			public ReadOnlyList<string> Names;

			// Token: 0x04001842 RID: 6210
			public ReadOnlyList<int> Values;
		}

		// Token: 0x0200050B RID: 1291
		public static class Cache
		{
			// Token: 0x060021B5 RID: 8629 RVA: 0x000EB0AC File Offset: 0x000E92AC
			public static EnumUtils.NamesValues Query(Type type)
			{
				Dictionary<Type, EnumUtils.NamesValues> namesValuesByType = EnumUtils.Cache.m_namesValuesByType;
				EnumUtils.NamesValues result;
				lock (namesValuesByType)
				{
					EnumUtils.NamesValues namesValues;
					if (!EnumUtils.Cache.m_namesValuesByType.TryGetValue(type, out namesValues))
					{
						namesValues = new EnumUtils.NamesValues
						{
							Names = new ReadOnlyList<string>(new List<string>(Enum.GetNames(type))),
							Values = new ReadOnlyList<int>(new List<int>(Enum.GetValues(type).Cast<int>()))
						};
						EnumUtils.Cache.m_namesValuesByType.Add(type, namesValues);
					}
					EnumUtils.NamesValues namesValues2 = namesValues;
					result = namesValues2;
				}
				return result;
			}

			// Token: 0x04001843 RID: 6211
			public static Dictionary<Type, EnumUtils.NamesValues> m_namesValuesByType = new Dictionary<Type, EnumUtils.NamesValues>();
		}
	}
}
