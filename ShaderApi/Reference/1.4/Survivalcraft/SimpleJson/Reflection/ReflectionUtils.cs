using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SimpleJson.Reflection
{
	// Token: 0x0200000A RID: 10
	[GeneratedCode("reflection-utils", "1.0.0")]
	internal class ReflectionUtils
	{
		// Token: 0x0600007A RID: 122 RVA: 0x000071D0 File Offset: 0x000053D0
		public static TypeInfo GetTypeInfo(Type type)
		{
			return type.GetTypeInfo();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000071D8 File Offset: 0x000053D8
		public static Attribute GetAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !info.IsDefined(type))
			{
				return null;
			}
			return info.GetCustomAttribute(type);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00007200 File Offset: 0x00005400
		public static Type GetGenericListElementType(Type type)
		{
			foreach (Type type2 in type.GetTypeInfo().ImplementedInterfaces)
			{
				if (ReflectionUtils.IsTypeGeneric(type2) && type2.GetGenericTypeDefinition() == typeof(IList<>))
				{
					return ReflectionUtils.GetGenericTypeArguments(type2)[0];
				}
			}
			return ReflectionUtils.GetGenericTypeArguments(type)[0];
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00007280 File Offset: 0x00005480
		public static Attribute GetAttribute(Type objectType, Type attributeType)
		{
			if (objectType == null || attributeType == null || !objectType.GetTypeInfo().IsDefined(attributeType))
			{
				return null;
			}
			return objectType.GetTypeInfo().GetCustomAttribute(attributeType);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000072B0 File Offset: 0x000054B0
		public static Type[] GetGenericTypeArguments(Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000072BD File Offset: 0x000054BD
		public static bool IsTypeGeneric(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000072CC File Offset: 0x000054CC
		public static bool IsTypeGenericeCollectionInterface(Type type)
		{
			if (!ReflectionUtils.IsTypeGeneric(type))
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007320 File Offset: 0x00005520
		public static bool IsAssignableFrom(Type type1, Type type2)
		{
			return ReflectionUtils.GetTypeInfo(type1).IsAssignableFrom(ReflectionUtils.GetTypeInfo(type2));
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007334 File Offset: 0x00005534
		public static bool IsTypeDictionary(Type type)
		{
			return typeof(IDictionary<, >).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) || (ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<, >));
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00007383 File Offset: 0x00005583
		public static bool IsNullableType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000073A9 File Offset: 0x000055A9
		public static object ToNullableType(object obj, Type nullableType)
		{
			if (obj != null)
			{
				return Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
			}
			return null;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000073C1 File Offset: 0x000055C1
		public static bool IsValueType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsValueType;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000073CE File Offset: 0x000055CE
		public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
		{
			return type.GetTypeInfo().DeclaredConstructors;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000073DC File Offset: 0x000055DC
		public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
		{
			foreach (ConstructorInfo constructorInfo in ReflectionUtils.GetConstructors(type))
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (argsType.Length == parameters.Length)
				{
					int num = 0;
					bool flag = true;
					ParameterInfo[] parameters2 = constructorInfo.GetParameters();
					for (int i = 0; i < parameters2.Length; i++)
					{
						if (parameters2[i].ParameterType != argsType[num])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00007478 File Offset: 0x00005678
		public static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetRuntimeProperties();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00007480 File Offset: 0x00005680
		public static IEnumerable<FieldInfo> GetFields(Type type)
		{
			return type.GetRuntimeFields();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007488 File Offset: 0x00005688
		public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetMethod;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007490 File Offset: 0x00005690
		public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.SetMethod;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007498 File Offset: 0x00005698
		public static ReflectionUtils.ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
		{
			return ReflectionUtils.GetConstructorByReflection(constructorInfo);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000074A0 File Offset: 0x000056A0
		public static ReflectionUtils.ConstructorDelegate GetContructor(Type type, params Type[] argsType)
		{
			return ReflectionUtils.GetConstructorByReflection(type, argsType);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000074A9 File Offset: 0x000056A9
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
		{
			return (object[] args) => constructorInfo.Invoke(args);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000074C4 File Offset: 0x000056C4
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
		{
			ConstructorInfo constructorInfo = ReflectionUtils.GetConstructorInfo(type, argsType);
			if (!(constructorInfo == null))
			{
				return ReflectionUtils.GetConstructorByReflection(constructorInfo);
			}
			return null;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000074EA File Offset: 0x000056EA
		public static ReflectionUtils.GetDelegate GetGetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetGetMethodByReflection(propertyInfo);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000074F2 File Offset: 0x000056F2
		public static ReflectionUtils.GetDelegate GetGetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetGetMethodByReflection(fieldInfo);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000074FA File Offset: 0x000056FA
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
			return (object source) => methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00007518 File Offset: 0x00005718
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
		{
			return (object source) => fieldInfo.GetValue(source);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00007531 File Offset: 0x00005731
		public static ReflectionUtils.SetDelegate GetSetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetSetMethodByReflection(propertyInfo);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00007539 File Offset: 0x00005739
		public static ReflectionUtils.SetDelegate GetSetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetSetMethodByReflection(fieldInfo);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007541 File Offset: 0x00005741
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
			return delegate(object source, object value)
			{
				methodInfo.Invoke(source, new object[]
				{
					value
				});
			};
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000755F File Offset: 0x0000575F
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
		{
			return delegate(object source, object value)
			{
				fieldInfo.SetValue(source, value);
			};
		}

		// Token: 0x04000050 RID: 80
		public static readonly object[] EmptyObjects = new object[0];

		// Token: 0x020003D8 RID: 984
		// (Invoke) Token: 0x06001DD3 RID: 7635
		public delegate object GetDelegate(object source);

		// Token: 0x020003D9 RID: 985
		// (Invoke) Token: 0x06001DD7 RID: 7639
		public delegate void SetDelegate(object source, object value);

		// Token: 0x020003DA RID: 986
		// (Invoke) Token: 0x06001DDB RID: 7643
		public delegate object ConstructorDelegate(params object[] args);

		// Token: 0x020003DB RID: 987
		// (Invoke) Token: 0x06001DDF RID: 7647
		public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);

		// Token: 0x020003DC RID: 988
		public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
		{
			// Token: 0x1700052D RID: 1325
			// (get) Token: 0x06001DE2 RID: 7650 RVA: 0x000E11FD File Offset: 0x000DF3FD
			public ICollection<TKey> Keys
			{
				get
				{
					return this._dictionary.Keys;
				}
			}

			// Token: 0x1700052E RID: 1326
			// (get) Token: 0x06001DE3 RID: 7651 RVA: 0x000E120A File Offset: 0x000DF40A
			public ICollection<TValue> Values
			{
				get
				{
					return this._dictionary.Values;
				}
			}

			// Token: 0x1700052F RID: 1327
			public TValue this[TKey key]
			{
				get
				{
					return this.Get(key);
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17000530 RID: 1328
			// (get) Token: 0x06001DE6 RID: 7654 RVA: 0x000E1227 File Offset: 0x000DF427
			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			// Token: 0x17000531 RID: 1329
			// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x000E1234 File Offset: 0x000DF434
			public bool IsReadOnly
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x06001DE8 RID: 7656 RVA: 0x000E123B File Offset: 0x000DF43B
			public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
			{
				this._valueFactory = valueFactory;
			}

			// Token: 0x06001DE9 RID: 7657 RVA: 0x000E1258 File Offset: 0x000DF458
			public TValue Get(TKey key)
			{
				if (this._dictionary == null)
				{
					return this.AddValue(key);
				}
				TValue result;
				if (!this._dictionary.TryGetValue(key, out result))
				{
					return this.AddValue(key);
				}
				return result;
			}

			// Token: 0x06001DEA RID: 7658 RVA: 0x000E1290 File Offset: 0x000DF490
			public TValue AddValue(TKey key)
			{
				TValue tvalue = this._valueFactory(key);
				object @lock = this._lock;
				TValue result;
				lock (@lock)
				{
					if (this._dictionary != null)
					{
						TValue tvalue2;
						if (this._dictionary.TryGetValue(key, out tvalue2))
						{
							result = tvalue2;
						}
						else
						{
							Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary);
							dictionary[key] = tvalue;
							this._dictionary = dictionary;
							result = tvalue;
						}
					}
					else
					{
						this._dictionary = new Dictionary<TKey, TValue>();
						this._dictionary[key] = tvalue;
						result = tvalue;
					}
				}
				return result;
			}

			// Token: 0x06001DEB RID: 7659 RVA: 0x000E1334 File Offset: 0x000DF534
			public void Add(TKey key, TValue value)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DEC RID: 7660 RVA: 0x000E133B File Offset: 0x000DF53B
			public bool ContainsKey(TKey key)
			{
				return this._dictionary.ContainsKey(key);
			}

			// Token: 0x06001DED RID: 7661 RVA: 0x000E1349 File Offset: 0x000DF549
			public bool Remove(TKey key)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DEE RID: 7662 RVA: 0x000E1350 File Offset: 0x000DF550
			public bool TryGetValue(TKey key, out TValue value)
			{
				value = this[key];
				return true;
			}

			// Token: 0x06001DEF RID: 7663 RVA: 0x000E1360 File Offset: 0x000DF560
			public void Add(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DF0 RID: 7664 RVA: 0x000E1367 File Offset: 0x000DF567
			public void Clear()
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DF1 RID: 7665 RVA: 0x000E136E File Offset: 0x000DF56E
			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DF2 RID: 7666 RVA: 0x000E1375 File Offset: 0x000DF575
			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DF3 RID: 7667 RVA: 0x000E137C File Offset: 0x000DF57C
			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001DF4 RID: 7668 RVA: 0x000E1383 File Offset: 0x000DF583
			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x06001DF5 RID: 7669 RVA: 0x000E1395 File Offset: 0x000DF595
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x04001436 RID: 5174
			public readonly object _lock = new object();

			// Token: 0x04001437 RID: 5175
			public readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

			// Token: 0x04001438 RID: 5176
			public Dictionary<TKey, TValue> _dictionary;
		}
	}
}
