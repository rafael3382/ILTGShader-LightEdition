using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using SimpleJson.Reflection;

namespace SimpleJson
{
	// Token: 0x02000009 RID: 9
	[GeneratedCode("simple-json", "1.0.0")]
	internal static class SimpleJson
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00006403 File Offset: 0x00004603
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00006419 File Offset: 0x00004619
		public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
		{
			get
			{
				IJsonSerializerStrategy result;
				if ((result = SimpleJson._currentJsonSerializerStrategy) == null)
				{
					result = (SimpleJson._currentJsonSerializerStrategy = SimpleJson.PocoJsonSerializerStrategy);
				}
				return result;
			}
			set
			{
				SimpleJson._currentJsonSerializerStrategy = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00006421 File Offset: 0x00004621
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
		{
			get
			{
				PocoJsonSerializerStrategy result;
				if ((result = SimpleJson._pocoJsonSerializerStrategy) == null)
				{
					result = (SimpleJson._pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
				}
				return result;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00006438 File Offset: 0x00004638
		static SimpleJson()
		{
			SimpleJson.EscapeTable = new char[93];
			SimpleJson.EscapeTable[34] = '"';
			SimpleJson.EscapeTable[92] = '\\';
			SimpleJson.EscapeTable[8] = 'b';
			SimpleJson.EscapeTable[12] = 'f';
			SimpleJson.EscapeTable[10] = 'n';
			SimpleJson.EscapeTable[13] = 'r';
			SimpleJson.EscapeTable[9] = 't';
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000064AC File Offset: 0x000046AC
		public static object DeserializeObject(string json)
		{
			object result;
			if (SimpleJson.TryDeserializeObject(json, out result))
			{
				return result;
			}
			throw new SerializationException("Invalid JSON string");
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000064D0 File Offset: 0x000046D0
		public static bool TryDeserializeObject(string json, out object obj)
		{
			bool result = true;
			if (json != null)
			{
				char[] json2 = json.ToCharArray();
				int num = 0;
				obj = SimpleJson.ParseValue(json2, ref num, ref result);
			}
			else
			{
				obj = null;
			}
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00006500 File Offset: 0x00004700
		public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			object obj = SimpleJson.DeserializeObject(json);
			if (!(type == null) && (obj == null || !ReflectionUtils.IsAssignableFrom(obj.GetType(), type)))
			{
				return (jsonSerializerStrategy ?? SimpleJson.CurrentJsonSerializerStrategy).DeserializeObject(obj, type);
			}
			return obj;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00006541 File Offset: 0x00004741
		public static object DeserializeObject(string json, Type type)
		{
			return SimpleJson.DeserializeObject(json, type, null);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000654B File Offset: 0x0000474B
		public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), jsonSerializerStrategy));
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006563 File Offset: 0x00004763
		public static T DeserializeObject<T>(string json)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), null));
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000657C File Offset: 0x0000477C
		public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			if (!SimpleJson.SerializeValue(jsonSerializerStrategy, json, stringBuilder))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000065A6 File Offset: 0x000047A6
		public static string SerializeObject(object json)
		{
			return SimpleJson.SerializeObject(json, SimpleJson.CurrentJsonSerializerStrategy);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000065B4 File Offset: 0x000047B4
		public static string EscapeToJavascriptString(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				return jsonString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < jsonString.Length)
			{
				char c = jsonString[i++];
				if (c == '\\')
				{
					if (jsonString.Length - i >= 2)
					{
						char c2 = jsonString[i];
						if (c2 <= 'b')
						{
							if (c2 != '"')
							{
								if (c2 != '\\')
								{
									if (c2 == 'b')
									{
										stringBuilder.Append('\b');
										i++;
									}
								}
								else
								{
									stringBuilder.Append('\\');
									i++;
								}
							}
							else
							{
								stringBuilder.Append("\"");
								i++;
							}
						}
						else if (c2 != 'n')
						{
							if (c2 != 'r')
							{
								if (c2 == 't')
								{
									stringBuilder.Append('\t');
									i++;
								}
							}
							else
							{
								stringBuilder.Append('\r');
								i++;
							}
						}
						else
						{
							stringBuilder.Append('\n');
							i++;
						}
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000066A0 File Offset: 0x000048A0
		public static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
		{
			IDictionary<string, object> dictionary = new JsonObject();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 2)
				{
					SimpleJson.NextToken(json, ref index);
					return dictionary;
				}
				if (num != 6)
				{
					string key = SimpleJson.ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					if (SimpleJson.NextToken(json, ref index) != 5)
					{
						success = false;
						return null;
					}
					object value = SimpleJson.ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					dictionary[key] = value;
				}
				else
				{
					SimpleJson.NextToken(json, ref index);
				}
			}
			return dictionary;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006734 File Offset: 0x00004934
		public static JsonArray ParseArray(char[] json, ref int index, ref bool success)
		{
			JsonArray jsonArray = new JsonArray();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 4)
				{
					SimpleJson.NextToken(json, ref index);
					break;
				}
				if (num != 6)
				{
					object item = SimpleJson.ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					jsonArray.Add(item);
				}
				else
				{
					SimpleJson.NextToken(json, ref index);
				}
			}
			return jsonArray;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000067A0 File Offset: 0x000049A0
		public static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (SimpleJson.LookAhead(json, index))
			{
			case 1:
				return SimpleJson.ParseObject(json, ref index, ref success);
			case 3:
				return SimpleJson.ParseArray(json, ref index, ref success);
			case 7:
				return SimpleJson.ParseString(json, ref index, ref success);
			case 8:
				return SimpleJson.ParseNumber(json, ref index, ref success);
			case 9:
				SimpleJson.NextToken(json, ref index);
				return true;
			case 10:
				SimpleJson.NextToken(json, ref index);
				return false;
			case 11:
				SimpleJson.NextToken(json, ref index);
				return null;
			}
			success = false;
			return null;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000683C File Offset: 0x00004A3C
		public static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			SimpleJson.EatWhitespace(json, ref index);
			int num = index;
			index = num + 1;
			char c = json[num];
			bool flag = false;
			while (!flag && index != json.Length)
			{
				num = index;
				index = num + 1;
				c = json[num];
				if (c == '"')
				{
					flag = true;
					break;
				}
				if (c != '\\')
				{
					stringBuilder.Append(c);
				}
				else
				{
					if (index == json.Length)
					{
						break;
					}
					num = index;
					index = num + 1;
					char c2 = json[num];
					if (c2 <= '\\')
					{
						if (c2 != '"')
						{
							if (c2 != '/')
							{
								if (c2 == '\\')
								{
									stringBuilder.Append('\\');
								}
							}
							else
							{
								stringBuilder.Append('/');
							}
						}
						else
						{
							stringBuilder.Append('"');
						}
					}
					else if (c2 <= 'f')
					{
						if (c2 != 'b')
						{
							if (c2 == 'f')
							{
								stringBuilder.Append('\f');
							}
						}
						else
						{
							stringBuilder.Append('\b');
						}
					}
					else if (c2 != 'n')
					{
						switch (c2)
						{
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							if (json.Length - index < 4)
							{
								goto IL_214;
							}
							uint num2;
							if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
							{
								return "";
							}
							if (55296U <= num2 && num2 <= 56319U)
							{
								index += 4;
								uint num3;
								if (json.Length - index < 6 || !(new string(json, index, 2) == "\\u") || !uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3) || 56320U > num3 || num3 > 57343U)
								{
									success = false;
									return "";
								}
								stringBuilder.Append((char)num2);
								stringBuilder.Append((char)num3);
								index += 6;
							}
							else
							{
								stringBuilder.Append(SimpleJson.ConvertFromUtf32((int)num2));
								index += 4;
							}
							break;
						}
						}
					}
					else
					{
						stringBuilder.Append('\n');
					}
				}
			}
			IL_214:
			if (!flag)
			{
				success = false;
				return null;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00006A6C File Offset: 0x00004C6C
		public static string ConvertFromUtf32(int utf32)
		{
			if (utf32 < 0 || utf32 > 1114111)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
			}
			if (55296 <= utf32 && utf32 <= 57343)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
			}
			if (utf32 < 65536)
			{
				return new string((char)utf32, 1);
			}
			utf32 -= 65536;
			return new string(new char[]
			{
				(char)((utf32 >> 10) + 55296),
				(char)(utf32 % 1024 + 56320)
			});
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006AFC File Offset: 0x00004CFC
		public static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			SimpleJson.EatWhitespace(json, ref index);
			int lastIndexOfNumber = SimpleJson.GetLastIndexOfNumber(json, index);
			int length = lastIndexOfNumber - index + 1;
			string text = new string(json, index, length);
			object result;
			if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
			{
				double num;
				success = double.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num);
				result = num;
			}
			else
			{
				long num2;
				success = long.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
				result = num2;
			}
			index = lastIndexOfNumber + 1;
			return result;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006B98 File Offset: 0x00004D98
		public static int GetLastIndexOfNumber(char[] json, int index)
		{
			int num = index;
			while (num < json.Length && "0123456789+-.eE".IndexOf(json[num]) != -1)
			{
				num++;
			}
			return num - 1;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006BC6 File Offset: 0x00004DC6
		public static void EatWhitespace(char[] json, ref int index)
		{
			while (index < json.Length && " \t\n\r\b\f".IndexOf(json[index]) != -1)
			{
				index++;
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00006BE8 File Offset: 0x00004DE8
		public static int LookAhead(char[] json, int index)
		{
			int num = index;
			return SimpleJson.NextToken(json, ref num);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006C00 File Offset: 0x00004E00
		public static int NextToken(char[] json, ref int index)
		{
			SimpleJson.EatWhitespace(json, ref index);
			if (index == json.Length)
			{
				return 0;
			}
			char c = json[index];
			index++;
			if (c <= '[')
			{
				switch (c)
				{
				case '"':
					return 7;
				case '#':
				case '$':
				case '%':
				case '&':
				case '\'':
				case '(':
				case ')':
				case '*':
				case '+':
				case '.':
				case '/':
					break;
				case ',':
					return 6;
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return 8;
				case ':':
					return 5;
				default:
					if (c == '[')
					{
						return 3;
					}
					break;
				}
			}
			else
			{
				if (c == ']')
				{
					return 4;
				}
				if (c == '{')
				{
					return 1;
				}
				if (c == '}')
				{
					return 2;
				}
			}
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00006D74 File Offset: 0x00004F74
		public static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
		{
			bool flag = true;
			string text = value as string;
			if (text != null)
			{
				flag = SimpleJson.SerializeString(text, builder);
			}
			else
			{
				IDictionary<string, object> dictionary = value as IDictionary<string, object>;
				if (dictionary != null)
				{
					flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder);
				}
				else
				{
					IDictionary<string, string> dictionary2 = value as IDictionary<string, string>;
					if (dictionary2 != null)
					{
						flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder);
					}
					else
					{
						IEnumerable enumerable = value as IEnumerable;
						if (enumerable != null)
						{
							flag = SimpleJson.SerializeArray(jsonSerializerStrategy, enumerable, builder);
						}
						else if (SimpleJson.IsNumeric(value))
						{
							flag = SimpleJson.SerializeNumber(value, builder);
						}
						else if (value is bool)
						{
							builder.Append(((bool)value) ? "true" : "false");
						}
						else if (value == null)
						{
							builder.Append("null");
						}
						else
						{
							object value2;
							flag = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out value2);
							if (flag)
							{
								SimpleJson.SerializeValue(jsonSerializerStrategy, value2, builder);
							}
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006E58 File Offset: 0x00005058
		public static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
		{
			builder.Append("{");
			IEnumerator enumerator = keys.GetEnumerator();
			IEnumerator enumerator2 = values.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				object obj = enumerator.Current;
				object value = enumerator2.Current;
				if (!flag)
				{
					builder.Append(",");
				}
				string text = obj as string;
				if (text != null)
				{
					SimpleJson.SerializeString(text, builder);
				}
				else if (!SimpleJson.SerializeValue(jsonSerializerStrategy, value, builder))
				{
					return false;
				}
				builder.Append(":");
				if (!SimpleJson.SerializeValue(jsonSerializerStrategy, value, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("}");
			return true;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006EF8 File Offset: 0x000050F8
		public static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
		{
			builder.Append("[");
			bool flag = true;
			foreach (object value in anArray)
			{
				if (!flag)
				{
					builder.Append(",");
				}
				if (!SimpleJson.SerializeValue(jsonSerializerStrategy, value, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("]");
			return true;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00006F80 File Offset: 0x00005180
		public static bool SerializeString(string aString, StringBuilder builder)
		{
			if (aString.IndexOfAny(SimpleJson.EscapeCharacters) == -1)
			{
				builder.Append('"');
				builder.Append(aString);
				builder.Append('"');
				return true;
			}
			builder.Append('"');
			int num = 0;
			char[] array = aString.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if ((int)c >= SimpleJson.EscapeTable.Length || SimpleJson.EscapeTable[(int)c] == '\0')
				{
					num++;
				}
				else
				{
					if (num > 0)
					{
						builder.Append(array, i - num, num);
						num = 0;
					}
					builder.Append('\\');
					builder.Append(SimpleJson.EscapeTable[(int)c]);
				}
			}
			if (num > 0)
			{
				builder.Append(array, array.Length - num, num);
			}
			builder.Append('"');
			return true;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000703C File Offset: 0x0000523C
		public static bool SerializeNumber(object number, StringBuilder builder)
		{
			if (number is long)
			{
				builder.Append(((long)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is ulong)
			{
				builder.Append(((ulong)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is int)
			{
				builder.Append(((int)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is uint)
			{
				builder.Append(((uint)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is decimal)
			{
				builder.Append(((decimal)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is float)
			{
				builder.Append(((float)number).ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
			}
			return true;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00007154 File Offset: 0x00005354
		public static bool IsNumeric(object value)
		{
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
		}

		// Token: 0x0400003F RID: 63
		public const int TOKEN_NONE = 0;

		// Token: 0x04000040 RID: 64
		public const int TOKEN_CURLY_OPEN = 1;

		// Token: 0x04000041 RID: 65
		public const int TOKEN_CURLY_CLOSE = 2;

		// Token: 0x04000042 RID: 66
		public const int TOKEN_SQUARED_OPEN = 3;

		// Token: 0x04000043 RID: 67
		public const int TOKEN_SQUARED_CLOSE = 4;

		// Token: 0x04000044 RID: 68
		public const int TOKEN_COLON = 5;

		// Token: 0x04000045 RID: 69
		public const int TOKEN_COMMA = 6;

		// Token: 0x04000046 RID: 70
		public const int TOKEN_STRING = 7;

		// Token: 0x04000047 RID: 71
		public const int TOKEN_NUMBER = 8;

		// Token: 0x04000048 RID: 72
		public const int TOKEN_TRUE = 9;

		// Token: 0x04000049 RID: 73
		public const int TOKEN_FALSE = 10;

		// Token: 0x0400004A RID: 74
		public const int TOKEN_NULL = 11;

		// Token: 0x0400004B RID: 75
		public const int BUILDER_CAPACITY = 2000;

		// Token: 0x0400004C RID: 76
		public static readonly char[] EscapeTable;

		// Token: 0x0400004D RID: 77
		public static readonly char[] EscapeCharacters = new char[]
		{
			'"',
			'\\',
			'\b',
			'\f',
			'\n',
			'\r',
			'\t'
		};

		// Token: 0x0400004E RID: 78
		public static IJsonSerializerStrategy _currentJsonSerializerStrategy;

		// Token: 0x0400004F RID: 79
		public static PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;
	}
}
