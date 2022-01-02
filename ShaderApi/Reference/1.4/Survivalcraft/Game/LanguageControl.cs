using System;
using System.Collections.Generic;
using System.IO;
using SimpleJson;

namespace Game
{
	// Token: 0x02000315 RID: 789
	public static class LanguageControl
	{
		// Token: 0x06001713 RID: 5907 RVA: 0x000ADA24 File Offset: 0x000ABC24
		public static void Initialize(string languageType)
		{
			LanguageControl.Ok = null;
			LanguageControl.Cancel = null;
			LanguageControl.None = null;
			LanguageControl.Nothing = null;
			LanguageControl.Error = null;
			LanguageControl.On = null;
			LanguageControl.Off = null;
			LanguageControl.Disable = null;
			LanguageControl.Enable = null;
			LanguageControl.Warning = null;
			LanguageControl.Back = null;
			LanguageControl.Allowed = null;
			LanguageControl.NAllowed = null;
			LanguageControl.Unknown = null;
			LanguageControl.Yes = null;
			LanguageControl.No = null;
			LanguageControl.Unavailable = null;
			LanguageControl.Exists = null;
			LanguageControl.Success = null;
			LanguageControl.Delete = null;
			LanguageControl.KeyWords.Clear();
			ModsManager.SetConfig("Language", languageType);
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x000ADAC0 File Offset: 0x000ABCC0
		public static void loadJson(Stream stream)
		{
			string text = new StreamReader(stream).ReadToEnd();
			if (text.Length > 0)
			{
				object obj = SimpleJson.DeserializeObject(text);
				LanguageControl.loadJsonLogic(LanguageControl.KeyWords, obj);
			}
			if (LanguageControl.Ok == null)
			{
				LanguageControl.Ok = LanguageControl.Get(new string[]
				{
					"Usual",
					"ok"
				});
			}
			if (LanguageControl.Cancel == null)
			{
				LanguageControl.Cancel = LanguageControl.Get(new string[]
				{
					"Usual",
					"cancel"
				});
			}
			if (LanguageControl.None == null)
			{
				LanguageControl.None = LanguageControl.Get(new string[]
				{
					"Usual",
					"none"
				});
			}
			if (LanguageControl.Nothing == null)
			{
				LanguageControl.Nothing = LanguageControl.Get(new string[]
				{
					"Usual",
					"nothing"
				});
			}
			if (LanguageControl.Error == null)
			{
				LanguageControl.Error = LanguageControl.Get(new string[]
				{
					"Usual",
					"error"
				});
			}
			if (LanguageControl.On == null)
			{
				LanguageControl.On = LanguageControl.Get(new string[]
				{
					"Usual",
					"on"
				});
			}
			if (LanguageControl.Off == null)
			{
				LanguageControl.Off = LanguageControl.Get(new string[]
				{
					"Usual",
					"off"
				});
			}
			if (LanguageControl.Disable == null)
			{
				LanguageControl.Disable = LanguageControl.Get(new string[]
				{
					"Usual",
					"disable"
				});
			}
			if (LanguageControl.Enable == null)
			{
				LanguageControl.Enable = LanguageControl.Get(new string[]
				{
					"Usual",
					"enable"
				});
			}
			if (LanguageControl.Warning == null)
			{
				LanguageControl.Warning = LanguageControl.Get(new string[]
				{
					"Usual",
					"warning"
				});
			}
			if (LanguageControl.Back == null)
			{
				LanguageControl.Back = LanguageControl.Get(new string[]
				{
					"Usual",
					"back"
				});
			}
			if (LanguageControl.Allowed == null)
			{
				LanguageControl.Allowed = LanguageControl.Get(new string[]
				{
					"Usual",
					"allowed"
				});
			}
			if (LanguageControl.NAllowed == null)
			{
				LanguageControl.NAllowed = LanguageControl.Get(new string[]
				{
					"Usual",
					"not allowed"
				});
			}
			if (LanguageControl.Unknown == null)
			{
				LanguageControl.Unknown = LanguageControl.Get(new string[]
				{
					"Usual",
					"unknown"
				});
			}
			if (LanguageControl.Yes == null)
			{
				LanguageControl.Yes = LanguageControl.Get(new string[]
				{
					"Usual",
					"yes"
				});
			}
			if (LanguageControl.No == null)
			{
				LanguageControl.No = LanguageControl.Get(new string[]
				{
					"Usual",
					"no"
				});
			}
			if (LanguageControl.Unavailable == null)
			{
				LanguageControl.Unavailable = LanguageControl.Get(new string[]
				{
					"Usual",
					"Unavailable"
				});
			}
			if (LanguageControl.Exists == null)
			{
				LanguageControl.Exists = LanguageControl.Get(new string[]
				{
					"Usual",
					"exist"
				});
			}
			if (LanguageControl.Success == null)
			{
				LanguageControl.Success = LanguageControl.Get(new string[]
				{
					"Usual",
					"success"
				});
			}
			if (LanguageControl.Delete == null)
			{
				LanguageControl.Success = LanguageControl.Get(new string[]
				{
					"Usual",
					"delete"
				});
			}
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x000ADE00 File Offset: 0x000AC000
		public static void loadJsonLogic(JsonObject node, object obj)
		{
			if (obj is JsonObject)
			{
				using (IEnumerator<KeyValuePair<string, object>> enumerator = (obj as JsonObject).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> keyValuePair = enumerator.Current;
						if (keyValuePair.Value is string)
						{
							if (node.ContainsKey(keyValuePair.Key))
							{
								node[keyValuePair.Key] = keyValuePair.Value;
							}
							else
							{
								node.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
						else if (node.ContainsKey(keyValuePair.Key))
						{
							LanguageControl.loadJsonLogic(node[keyValuePair.Key] as JsonObject, keyValuePair.Value);
						}
						else
						{
							node.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					return;
				}
			}
			if (obj is JsonArray)
			{
				JsonArray jsonArray = obj as JsonArray;
				for (int i = 0; i < jsonArray.Count; i++)
				{
					if (jsonArray[i] is string)
					{
						if (node.ContainsKey(i.ToString()))
						{
							node[i.ToString()] = jsonArray[i];
						}
						else
						{
							node.Add(i.ToString(), jsonArray[i]);
						}
					}
					else
					{
						JsonObject jsonObject = new JsonObject();
						if (node.ContainsKey(i.ToString()))
						{
							node[i.ToString()] = jsonObject;
						}
						else
						{
							node.Add(i.ToString(), jsonArray[i]);
						}
						LanguageControl.loadJsonLogic(jsonObject, jsonArray[i]);
					}
				}
			}
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x000ADFA4 File Offset: 0x000AC1A4
		public static string LName()
		{
			return ModsManager.Configs["Language"];
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000ADFB5 File Offset: 0x000AC1B5
		public static string Get(string className, int key)
		{
			return LanguageControl.Get(new string[]
			{
				className,
				key.ToString()
			});
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x000ADFD0 File Offset: 0x000AC1D0
		public static string GetWorldPalette(int index)
		{
			return LanguageControl.Get(new string[]
			{
				"WorldPalette",
				"Colors",
				index.ToString()
			});
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x000ADFF8 File Offset: 0x000AC1F8
		public static string Get(params string[] key)
		{
			bool flag;
			return LanguageControl.Get(out flag, key);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x000AE010 File Offset: 0x000AC210
		public static string Get(out bool r, params string[] key)
		{
			r = false;
			JsonObject jsonObject = LanguageControl.KeyWords;
			JsonArray jsonArray = null;
			for (int i = 0; i < key.Length; i++)
			{
				bool flag = false;
				object obj2;
				if (jsonArray != null)
				{
					int index;
					int.TryParse(key[i], out index);
					object obj = jsonArray[index];
					JsonObject jsonObject2 = obj as JsonObject;
					if (jsonObject2 != null)
					{
						jsonObject = jsonObject2;
						jsonArray = null;
						flag = true;
					}
					else
					{
						JsonArray jsonArray2 = obj as JsonArray;
						if (jsonArray2 == null)
						{
							r = true;
							return obj.ToString();
						}
						jsonObject = null;
						jsonArray = jsonArray2;
						flag = true;
					}
				}
				else if (jsonObject.TryGetValue(key[i], out obj2))
				{
					JsonObject jsonObject3 = obj2 as JsonObject;
					if (jsonObject3 != null)
					{
						jsonObject = jsonObject3;
						jsonArray = null;
						flag = true;
					}
					else
					{
						JsonArray jsonArray3 = obj2 as JsonArray;
						if (jsonArray3 == null)
						{
							r = true;
							return obj2.ToString();
						}
						jsonObject = null;
						jsonArray = jsonArray3;
						flag = true;
					}
				}
				if (!flag)
				{
					return key[i];
				}
			}
			string text = "";
			foreach (string str in key)
			{
				text = text + str + ":";
			}
			return text;
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x000AE110 File Offset: 0x000AC310
		public static string GetBlock(string blockName, string prop)
		{
			string result;
			LanguageControl.TryGetBlock(blockName, prop, out result);
			return result;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x000AE128 File Offset: 0x000AC328
		public static bool TryGetBlock(string blockName, string prop, out string result)
		{
			string[] array = blockName.Split(new char[]
			{
				':'
			}, StringSplitOptions.None);
			bool flag;
			result = LanguageControl.Get(out flag, new string[]
			{
				"Blocks",
				(array.Length < 2) ? (blockName + ":0") : blockName,
				prop
			});
			if (!flag)
			{
				result = LanguageControl.Get(out flag, new string[]
				{
					"Blocks",
					array[0] + ":0",
					prop
				});
			}
			return flag;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x000AE1A9 File Offset: 0x000AC3A9
		public static string GetContentWidgets(string name, string prop)
		{
			return LanguageControl.Get(new string[]
			{
				"ContentWidgets",
				name,
				prop
			});
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x000AE1C6 File Offset: 0x000AC3C6
		public static string GetContentWidgets(string name, int pos)
		{
			return LanguageControl.Get(new string[]
			{
				"ContentWidgets",
				name,
				pos.ToString()
			});
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x000AE1E9 File Offset: 0x000AC3E9
		public static string GetDatabase(string name, string prop)
		{
			return LanguageControl.Get(new string[]
			{
				"Database",
				name,
				prop
			});
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x000AE206 File Offset: 0x000AC406
		public static string GetFireworks(string name, string prop)
		{
			return LanguageControl.Get(new string[]
			{
				"FireworksBlock",
				name,
				prop
			});
		}

		// Token: 0x04000FC3 RID: 4035
		public static JsonObject KeyWords = new JsonObject();

		// Token: 0x04000FC4 RID: 4036
		public static string Ok = null;

		// Token: 0x04000FC5 RID: 4037
		public static string Cancel = null;

		// Token: 0x04000FC6 RID: 4038
		public static string None = null;

		// Token: 0x04000FC7 RID: 4039
		public static string Nothing = null;

		// Token: 0x04000FC8 RID: 4040
		public static string Error = null;

		// Token: 0x04000FC9 RID: 4041
		public static string On = null;

		// Token: 0x04000FCA RID: 4042
		public static string Off = null;

		// Token: 0x04000FCB RID: 4043
		public static string Disable = null;

		// Token: 0x04000FCC RID: 4044
		public static string Enable = null;

		// Token: 0x04000FCD RID: 4045
		public static string Warning = null;

		// Token: 0x04000FCE RID: 4046
		public static string Back = null;

		// Token: 0x04000FCF RID: 4047
		public static string Allowed = null;

		// Token: 0x04000FD0 RID: 4048
		public static string NAllowed = null;

		// Token: 0x04000FD1 RID: 4049
		public static string Unknown = null;

		// Token: 0x04000FD2 RID: 4050
		public static string Yes = null;

		// Token: 0x04000FD3 RID: 4051
		public static string No = null;

		// Token: 0x04000FD4 RID: 4052
		public static string Unavailable = null;

		// Token: 0x04000FD5 RID: 4053
		public static string Exists = null;

		// Token: 0x04000FD6 RID: 4054
		public static string Success = null;

		// Token: 0x04000FD7 RID: 4055
		public static string Delete = null;

		// Token: 0x04000FD8 RID: 4056
		public static List<string> LanguageTypes = new List<string>();
	}
}
