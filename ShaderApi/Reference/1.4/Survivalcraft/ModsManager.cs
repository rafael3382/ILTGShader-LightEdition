using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Engine;
using Game;
using SimpleJson;
using XmlUtilities;

// Token: 0x02000004 RID: 4
public static class ModsManager
{
	// Token: 0x06000019 RID: 25 RVA: 0x00004394 File Offset: 0x00002594
	public static bool GetModEntity(string packagename, out ModEntity modEntity)
	{
		modEntity = ModsManager.ModList.Find((ModEntity px) => px.modInfo.PackageName == packagename);
		return modEntity != null;
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000043CB File Offset: 0x000025CB
	public static bool GetAllowContinue()
	{
		return ModsManager.AllowContinue;
	}

	// Token: 0x0600001B RID: 27 RVA: 0x000043D4 File Offset: 0x000025D4
	internal static void Reboot()
	{
		SettingsManager.SaveSettings();
		SettingsManager.LoadSettings();
		foreach (ModEntity modEntity in ModsManager.ModList)
		{
			modEntity.Dispose();
		}
		ScreensManager.SwitchScreen("Loading", Array.Empty<object>());
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000443C File Offset: 0x0000263C
	public static void HookAction(string HookName, Func<ModLoader, bool> action)
	{
		ModsManager.ModHook modHook;
		if (ModsManager.ModHooks.TryGetValue(HookName, out modHook))
		{
			foreach (ModLoader arg in modHook.Loaders.Keys)
			{
				try
				{
					if (action(arg))
					{
						break;
					}
				}
				catch (Exception ex)
				{
					Log.Warning(HookName + " Method has an error, error message:" + ex.Message);
				}
			}
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000044D0 File Offset: 0x000026D0
	public static void RegisterHook(string HookName, ModLoader modLoader)
	{
		ModsManager.ModHook modHook;
		if (!ModsManager.ModHooks.TryGetValue(HookName, out modHook))
		{
			modHook = new ModsManager.ModHook(HookName);
			ModsManager.ModHooks.Add(HookName, modHook);
		}
		modHook.Add(modLoader);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00004508 File Offset: 0x00002708
	public static void DisableHook(ModLoader from, string HookName, string packageName, string reason)
	{
		ModEntity modEntity = ModsManager.ModList.Find((ModEntity p) => p.modInfo.PackageName == packageName);
		ModsManager.ModHook modHook;
		if (ModsManager.ModHooks.TryGetValue(HookName, out modHook))
		{
			modHook.Disable(from, modEntity.Loader, reason);
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00004556 File Offset: 0x00002756
	public static T GetInPakOrStorageFile<T>(string filepath, string suffix = ".txt") where T : class
	{
		return ContentManager.Get<T>(filepath, suffix);
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00004560 File Offset: 0x00002760
	public static T DeserializeJson<T>(string text) where T : class
	{
		JsonObject jsonObject = (JsonObject)SimpleJson.DeserializeObject(text, typeof(JsonObject));
		T t = Activator.CreateInstance(typeof(T)) as T;
		Type type = t.GetType();
		foreach (KeyValuePair<string, object> keyValuePair in jsonObject)
		{
			FieldInfo field = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public);
			if (!(field == null))
			{
				if (keyValuePair.Value is JsonArray)
				{
					List<object> list = keyValuePair.Value as JsonArray;
					Type[] genericArguments = field.FieldType.GetGenericArguments();
					object obj = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArguments));
					foreach (object obj2 in list)
					{
						MethodInfo method = obj.GetType().GetMethod("Add");
						if (genericArguments.Length == 1)
						{
							string a = genericArguments[0].Name.ToLower();
							if (!(a == "int32"))
							{
								if (!(a == "int64"))
								{
									if (!(a == "single"))
									{
										if (!(a == "double"))
										{
											if (!(a == "bool"))
											{
												method.Invoke(obj, new object[]
												{
													obj2
												});
											}
											else
											{
												bool flag;
												bool.TryParse(obj2.ToString(), out flag);
												method.Invoke(obj, new object[]
												{
													flag
												});
											}
										}
										else
										{
											double num;
											double.TryParse(obj2.ToString(), out num);
											method.Invoke(obj, new object[]
											{
												num
											});
										}
									}
									else
									{
										float num2;
										float.TryParse(obj2.ToString(), out num2);
										method.Invoke(obj, new object[]
										{
											num2
										});
									}
								}
								else
								{
									long num3;
									long.TryParse(obj2.ToString(), out num3);
									method.Invoke(obj, new object[]
									{
										num3
									});
								}
							}
							else
							{
								int num4;
								int.TryParse(obj2.ToString(), out num4);
								method.Invoke(obj, new object[]
								{
									num4
								});
							}
						}
					}
					if (obj != null)
					{
						field.SetValue(t, obj);
					}
				}
				else
				{
					field.SetValue(t, keyValuePair.Value);
				}
			}
		}
		return t;
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00004834 File Offset: 0x00002A34
	public static void SaveModSettings(XElement xElement)
	{
		foreach (ModEntity modEntity in ModsManager.ModList)
		{
			modEntity.SaveSettings(xElement);
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00004884 File Offset: 0x00002A84
	public static void SaveSettings(XElement xElement)
	{
		XElement xelement = new XElement("Configs");
		foreach (KeyValuePair<string, string> keyValuePair in ModsManager.Configs)
		{
			xelement.SetAttributeValue(keyValuePair.Key, keyValuePair.Value);
		}
		xElement.Add(xelement);
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00004900 File Offset: 0x00002B00
	public static void LoadSettings(XElement xElement)
	{
		foreach (XAttribute xattribute in xElement.Element("Configs").Attributes())
		{
			if (!ModsManager.Configs.ContainsKey(xattribute.Name.LocalName))
			{
				ModsManager.SetConfig(xattribute.Name.LocalName, xattribute.Value);
			}
		}
		ModsManager.ConfigLoaded = true;
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00004988 File Offset: 0x00002B88
	public static void LoadModSettings(XElement xElement)
	{
		foreach (ModEntity modEntity in ModsManager.ModList)
		{
			modEntity.LoadSettings(xElement);
		}
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000049D8 File Offset: 0x00002BD8
	public static void SetConfig(string key, string value)
	{
		string text;
		if (!ModsManager.Configs.TryGetValue(key, out text))
		{
			ModsManager.Configs.Add(key, value);
		}
		ModsManager.Configs[key] = value;
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00004A0C File Offset: 0x00002C0C
	public static string ImportMod(string name, Stream stream)
	{
		if (Storage.DirectoryExists("app:/ModsCache"))
		{
			Storage.CreateDirectory("app:/ModsCache");
		}
		using (Stream stream2 = Storage.OpenFile(Storage.CombinePaths(new string[]
		{
			"app:/ModsCache",
			name + ".scmod"
		}), OpenFileMode.CreateOrOpen))
		{
			stream.CopyTo(stream2);
			stream.Close();
		}
		return "下载成功,请到Mod管理中进行手动安装";
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00004A88 File Offset: 0x00002C88
	public static void ModListAllDo(Action<ModEntity> entity)
	{
		for (int i = 0; i < ModsManager.ModList.Count; i++)
		{
			if (entity != null)
			{
				entity(ModsManager.ModList[i]);
			}
		}
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00004AC0 File Offset: 0x00002CC0
	public static void Initialize()
	{
		if (!Storage.DirectoryExists(ModsManager.ModsPath))
		{
			Storage.CreateDirectory(ModsManager.ModsPath);
		}
		ModsManager.ModHooks.Clear();
		ModsManager.ModListAll.Clear();
		ModsManager.ModList.Clear();
		ModsManager.ModLoaders.Clear();
		ModsManager.SurvivalCrafModEntity = new SurvivalCrafModEntity();
		ModEntity modEntity = new FastDebugModEntity();
		ModsManager.ModList.Add(ModsManager.SurvivalCrafModEntity);
		ModsManager.ModList.Add(modEntity);
		ModsManager.GetScmods(ModsManager.ModsPath);
		ModsManager.ModListAll.AddRange(ModsManager.ModList);
		List<ModInfo> list = new List<ModInfo>();
		list.AddRange(ModsManager.DisabledMods);
		ModsManager.DisabledMods.Clear();
		float num = float.Parse("1.40");
		List<ModEntity> list2 = new List<ModEntity>();
		foreach (ModEntity modEntity2 in ModsManager.ModList)
		{
			ModInfo modInfo = modEntity2.modInfo;
			ModInfo modInfo2 = list.Find((ModInfo l) => l.PackageName == modInfo.PackageName);
			if (modInfo2 != null && modInfo2.PackageName != ModsManager.SurvivalCrafModEntity.modInfo.PackageName && modInfo2.PackageName != modEntity.modInfo.PackageName)
			{
				list.Add(modInfo);
				list2.Add(modEntity2);
			}
			else if (!modEntity2.IsChecked)
			{
				float num2;
				float.TryParse(modInfo.ApiVersion, out num2);
				if (num2 < num)
				{
					list.Add(modInfo);
					list2.Add(modEntity2);
					ModsManager.AddException(new Exception(string.Concat(new string[]
					{
						"[",
						modEntity2.modInfo.PackageName,
						"]Target version ",
						modInfo.Version,
						" is less than api version 1.40."
					})), true);
				}
				if (ModsManager.ModList.FindAll((ModEntity px) => px.modInfo.PackageName == modInfo.PackageName).Count > 1)
				{
					ModsManager.AddException(new Exception("Multiple installed [" + modInfo.PackageName + "]"), false);
				}
				modEntity2.IsChecked = true;
			}
		}
		ModsManager.DisabledMods.Clear();
		foreach (ModInfo item in list)
		{
			ModsManager.DisabledMods.Add(item);
		}
		foreach (ModEntity item2 in list2)
		{
			ModsManager.ModList.Remove(item2);
		}
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00004DC8 File Offset: 0x00002FC8
	public static void AddException(Exception e, bool AllowContinue_ = false)
	{
		LoadingScreen.Error(e.Message);
		ModsManager.AllowContinue = (!SettingsManager.DisplayLog || AllowContinue_);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00004DE8 File Offset: 0x00002FE8
	public static void GetScmods(string path)
	{
		foreach (string text in Storage.ListFileNames(path))
		{
			string extension = Storage.GetExtension(text);
			Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				path,
				text
			}), OpenFileMode.Read);
			try
			{
				if (extension == ".scmod")
				{
					ModEntity modEntity = new ModEntity(ZipArchive.Open(stream, true));
					if (modEntity.modInfo != null)
					{
						if (!string.IsNullOrEmpty(modEntity.modInfo.PackageName))
						{
							ModsManager.ModList.Add(modEntity);
						}
					}
				}
			}
			catch (Exception e)
			{
				ModsManager.AddException(e, false);
				stream.Close();
			}
		}
		foreach (string text2 in Storage.ListDirectoryNames(path))
		{
			ModsManager.GetScmods(Storage.CombinePaths(new string[]
			{
				path,
				text2
			}));
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00004F04 File Offset: 0x00003104
	public static string StreamToString(Stream stream)
	{
		stream.Seek(0L, SeekOrigin.Begin);
		return new StreamReader(stream).ReadToEnd();
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00004F1C File Offset: 0x0000311C
	public static byte[] StreamToBytes(Stream stream)
	{
		byte[] array = new byte[stream.Length];
		stream.Seek(0L, SeekOrigin.Begin);
		stream.Read(array, 0, array.Length);
		return array;
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00004F50 File Offset: 0x00003150
	public static string GetMd5(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00004FA8 File Offset: 0x000031A8
	public static bool FindElement(XElement xElement, Func<XElement, bool> func, out XElement elementout)
	{
		foreach (XElement xelement in xElement.Elements())
		{
			if (func(xelement))
			{
				elementout = xelement;
				return true;
			}
			XElement xelement2;
			if (ModsManager.FindElement(xelement, func, out xelement2))
			{
				elementout = xelement2;
				return true;
			}
		}
		elementout = null;
		return false;
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00005018 File Offset: 0x00003218
	public static bool FindElementByGuid(XElement xElement, string guid, out XElement elementout)
	{
		foreach (XElement xelement in xElement.Elements())
		{
			foreach (XAttribute xattribute in xelement.Attributes())
			{
				if (xattribute.Name.ToString() == "Guid" && xattribute.Value == guid)
				{
					elementout = xelement;
					return true;
				}
			}
			XElement xelement2;
			if (ModsManager.FindElementByGuid(xelement, guid, out xelement2))
			{
				elementout = xelement2;
				return true;
			}
		}
		elementout = null;
		return false;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000050E0 File Offset: 0x000032E0
	public static bool HasAttribute(XElement element, Func<string, bool> func, out XAttribute xAttributeout)
	{
		foreach (XAttribute xattribute in element.Attributes())
		{
			if (func(xattribute.Name.LocalName))
			{
				xAttributeout = xattribute;
				return true;
			}
		}
		xAttributeout = null;
		return false;
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00005148 File Offset: 0x00003348
	public static void CombineClo(XElement xElement, Stream cloorcr)
	{
		XElement xelement = XmlUtils.LoadXmlFromStream(cloorcr, Encoding.UTF8, true);
		using (IEnumerator<XElement> enumerator = xelement.Elements().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement element = enumerator.Current;
				XAttribute xattribute;
				XAttribute xattribute2;
				if (ModsManager.HasAttribute(element, (string name) => name.StartsWith("new-"), out xattribute))
				{
					XElement xelement2;
					XAttribute xAttribute;
					if (ModsManager.HasAttribute(element, (string name) => name == "Index", out xAttribute) && ModsManager.FindElement(xElement, (XElement ele) => element.Attribute("Index").Value == xAttribute.Value, out xelement2))
					{
						string[] array = xattribute.Name.ToString().Split(new string[]
						{
							"new-"
						}, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length == 1)
						{
							xelement2.SetAttributeValue(array[0], xattribute.Value);
						}
					}
				}
				else if (ModsManager.HasAttribute(element, (string name) => name.StartsWith("r-"), out xattribute2))
				{
					XElement xelement3;
					XAttribute xAttribute;
					if (ModsManager.HasAttribute(element, (string name) => name == "Index", out xAttribute) && ModsManager.FindElement(xElement, (XElement ele) => element.Attribute("Index").Value == xAttribute.Value, out xelement3))
					{
						xelement3.Remove();
						element.Remove();
					}
				}
				xElement.Add(xelement);
			}
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00005340 File Offset: 0x00003540
	public static void CombineCr(XElement xElement, Stream cloorcr)
	{
		XElement needCombine = XmlUtils.LoadXmlFromStream(cloorcr, Encoding.UTF8, true);
		ModsManager.CombineCrLogic(xElement, needCombine);
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00005364 File Offset: 0x00003564
	public static void CombineCrLogic(XElement xElement, XElement needCombine)
	{
		using (IEnumerator<XElement> enumerator = needCombine.Elements().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement element = enumerator.Current;
				XAttribute xattribute;
				if (ModsManager.HasAttribute(element, (string name) => name == "Result", out xattribute))
				{
					XAttribute attribute;
					if (ModsManager.HasAttribute(element, (string name) => name.StartsWith("new-"), out attribute))
					{
						string[] array = attribute.Name.ToString().Split(new string[]
						{
							"new-"
						}, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length == 1)
						{
							string text = array[0];
						}
						XElement xelement;
						if (ModsManager.FindElement(xElement, delegate(XElement ele)
						{
							using (IEnumerator<XAttribute> enumerator2 = element.Attributes().GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									XAttribute xAttribute = enumerator2.Current;
									XAttribute xattribute2;
									if (!(xAttribute.Name == attribute.Name) && !ModsManager.HasAttribute(ele, (string tname) => tname == xAttribute.Name, out xattribute2))
									{
										return false;
									}
								}
							}
							return true;
						}, out xelement) && array.Length == 1)
						{
							xelement.SetAttributeValue(array[0], attribute.Value);
							xelement.SetValue(element.Value);
						}
					}
					else
					{
						XAttribute attribute1;
						if (ModsManager.HasAttribute(element, (string name) => name.StartsWith("r-"), out attribute1))
						{
							XElement xelement2;
							if (ModsManager.FindElement(xElement, delegate(XElement ele)
							{
								using (IEnumerator<XAttribute> enumerator2 = element.Attributes().GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										XAttribute xAttribute = enumerator2.Current;
										XAttribute xattribute2;
										if (!(xAttribute.Name == attribute1.Name) && !ModsManager.HasAttribute(ele, (string tname) => tname == xAttribute.Name, out xattribute2))
										{
											return false;
										}
									}
								}
								return true;
							}, out xelement2))
							{
								xelement2.Remove();
								element.Remove();
							}
						}
						else
						{
							xElement.Add(element);
						}
					}
				}
				ModsManager.CombineCrLogic(xElement, element);
			}
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00005548 File Offset: 0x00003748
	public static void Modify(XElement source, XElement change)
	{
		XElement source2;
		if (ModsManager.FindElement(source, (XElement item) => item.Name.LocalName == change.Name.LocalName && item.Attribute("Guid") != null && change.Attribute("Guid") != null && item.Attribute("Guid").Value == change.Attribute("Guid").Value, out source2))
		{
			using (IEnumerator<XElement> enumerator = change.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement change2 = enumerator.Current;
					ModsManager.Modify(source2, change2);
				}
				return;
			}
		}
		source.Add(change);
	}

	// Token: 0x06000035 RID: 53 RVA: 0x000055CC File Offset: 0x000037CC
	public static void CombineDataBase(XElement DataBaseXml, Stream Xdb)
	{
		XContainer xcontainer = XmlUtils.LoadXmlFromStream(Xdb, Encoding.UTF8, true);
		XElement xelement = DataBaseXml.Element("DatabaseObjects");
		foreach (XElement xelement2 in xcontainer.Elements())
		{
			XAttribute xattribute;
			if (ModsManager.HasAttribute(xelement2, (string str) => str.Contains("new-"), out xattribute))
			{
				XAttribute xattribute2;
				XElement xelement3;
				if (ModsManager.HasAttribute(xelement2, (string str) => str == "Guid", out xattribute2) && ModsManager.FindElementByGuid(xelement, xattribute2.Value, out xelement3))
				{
					string[] array = xattribute.Name.ToString().Split(new string[]
					{
						"new-"
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 1)
					{
						xelement3.SetAttributeValue(array[0], xattribute.Value);
					}
				}
			}
			ModsManager.Modify(xelement, xelement2);
		}
	}

	// Token: 0x0400001E RID: 30
	public const string APIVersion = "1.40";

	// Token: 0x0400001F RID: 31
	public const string SCVersion = "2.2.10.4";

	// Token: 0x04000020 RID: 32
	public const int Apiv = 3;

	// Token: 0x04000021 RID: 33
	public const string ExternelPath = "app:";

	// Token: 0x04000022 RID: 34
	public const string UserDataPath = "app:/UserId.dat";

	// Token: 0x04000023 RID: 35
	public const string CharacterSkinsDirectoryName = "app:/CharacterSkins";

	// Token: 0x04000024 RID: 36
	public const string FurniturePacksDirectoryName = "app:/FurniturePacks";

	// Token: 0x04000025 RID: 37
	public const string BlockTexturesDirectoryName = "app:/TexturePacks";

	// Token: 0x04000026 RID: 38
	public const string WorldsDirectoryName = "app:/Worlds";

	// Token: 0x04000027 RID: 39
	public const string CommunityContentCachePath = "app:CommunityContentCache.xml";

	// Token: 0x04000028 RID: 40
	public const string ModsSetPath = "app:/ModSettings.xml";

	// Token: 0x04000029 RID: 41
	public const string SettingPath = "app:/Settings.xml";

	// Token: 0x0400002A RID: 42
	public const string ModCachePath = "app:/ModsCache";

	// Token: 0x0400002B RID: 43
	public const string LogPath = "app:/Bugs";

	// Token: 0x0400002C RID: 44
	public const bool IsAndroid = false;

	// Token: 0x0400002D RID: 45
	public static string ModsPath = "app:/Mods";

	// Token: 0x0400002E RID: 46
	public static string path;

	// Token: 0x0400002F RID: 47
	internal static ModEntity SurvivalCrafModEntity;

	// Token: 0x04000030 RID: 48
	internal static bool ConfigLoaded = false;

	// Token: 0x04000031 RID: 49
	private static bool AllowContinue = true;

	// Token: 0x04000032 RID: 50
	public static Dictionary<string, string> Configs = new Dictionary<string, string>();

	// Token: 0x04000033 RID: 51
	public static List<ModEntity> ModListAll = new List<ModEntity>();

	// Token: 0x04000034 RID: 52
	public static List<ModEntity> ModList = new List<ModEntity>();

	// Token: 0x04000035 RID: 53
	public static List<ModLoader> ModLoaders = new List<ModLoader>();

	// Token: 0x04000036 RID: 54
	public static List<ModInfo> DisabledMods = new List<ModInfo>();

	// Token: 0x04000037 RID: 55
	public static Dictionary<string, ModsManager.ModHook> ModHooks = new Dictionary<string, ModsManager.ModHook>();

	// Token: 0x020003CA RID: 970
	public class ModSettings
	{
		// Token: 0x04001419 RID: 5145
		public string languageType;
	}

	// Token: 0x020003CB RID: 971
	public class ModHook
	{
		// Token: 0x06001DAD RID: 7597 RVA: 0x000E0DB0 File Offset: 0x000DEFB0
		public ModHook(string name)
		{
			this.HookName = name;
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x000E0DD8 File Offset: 0x000DEFD8
		public void Add(ModLoader modLoader)
		{
			bool flag;
			if (!this.Loaders.TryGetValue(modLoader, out flag))
			{
				this.Loaders.Add(modLoader, true);
			}
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x000E0E04 File Offset: 0x000DF004
		public void Remove(ModLoader modLoader)
		{
			bool flag;
			if (this.Loaders.TryGetValue(modLoader, out flag))
			{
				this.Loaders.Remove(modLoader);
			}
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x000E0E30 File Offset: 0x000DF030
		public void Disable(ModLoader from, ModLoader toDisable, string reason)
		{
			bool flag;
			string text;
			if (this.Loaders.TryGetValue(toDisable, out flag) && !this.DisableReason.TryGetValue(from, out text))
			{
				this.DisableReason.Add(from, reason);
			}
		}

		// Token: 0x0400141A RID: 5146
		public string HookName;

		// Token: 0x0400141B RID: 5147
		public Dictionary<ModLoader, bool> Loaders = new Dictionary<ModLoader, bool>();

		// Token: 0x0400141C RID: 5148
		public Dictionary<ModLoader, string> DisableReason = new Dictionary<ModLoader, string>();
	}
}
