using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using Engine.Audio;
using Engine.Graphics;
using Engine.Media;
using Game.IContentReader;
using SimpleJson;

namespace Game
{
	// Token: 0x02000365 RID: 869
	public static class ContentManager
	{
		// Token: 0x0600195D RID: 6493 RVA: 0x000C85D4 File Offset: 0x000C67D4
		public static void Initialize()
		{
			ContentManager.ReaderList.Clear();
			ContentManager.Resources.Clear();
			ContentManager.Caches.Clear();
			Display.DeviceReset += ContentManager.Display_DeviceReset;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x000C8605 File Offset: 0x000C6805
		public static T Get<T>(string name, string suffix = null) where T : class
		{
			return ContentManager.Get(typeof(T), name, suffix) as T;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x000C8624 File Offset: 0x000C6824
		public static object Get(Type type, string name, string suffix = null)
		{
			object obj = ContentManager.syncobj;
			object result;
			lock (obj)
			{
				object obj2 = null;
				string key = (suffix == null) ? name : (name + "." + suffix);
				if (type == typeof(Subtexture))
				{
					result = TextureAtlasManager.GetSubtexture(name);
				}
				else
				{
					List<object> list;
					if (ContentManager.Caches.TryGetValue(key, out list))
					{
						obj2 = list.Find((object f) => f.GetType() == type);
					}
					if (obj2 != null)
					{
						result = obj2;
					}
					else
					{
						IContentReader contentReader;
						if (ContentManager.ReaderList.TryGetValue(type.FullName, out contentReader))
						{
							List<ContentInfo> list2 = new List<ContentInfo>();
							string key2 = string.Empty;
							if (suffix == null)
							{
								for (int i = 0; i < contentReader.DefaultSuffix.Length; i++)
								{
									key2 = name + "." + contentReader.DefaultSuffix[i];
									ContentInfo item;
									if (ContentManager.Resources.TryGetValue(key2, out item))
									{
										list2.Add(item);
									}
								}
							}
							else
							{
								key2 = name + suffix;
								ContentInfo item2;
								if (ContentManager.Resources.TryGetValue(key2, out item2))
								{
									list2.Add(item2);
								}
							}
							if (list2.Count == 0)
							{
								throw new Exception(string.Concat(new string[]
								{
									"Not Found Res [",
									name,
									"][",
									type.FullName,
									"]"
								}));
							}
							obj2 = contentReader.Get(list2.ToArray());
						}
						if (list == null)
						{
							list = new List<object>();
							ContentManager.Caches.Add(key, list);
						}
						list.Add(obj2);
						result = obj2;
					}
				}
			}
			return result;
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x000C87F4 File Offset: 0x000C69F4
		public static object StreamConvertType(Type type, Stream stream)
		{
			string fullName = type.FullName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(fullName);
			if (num <= 2314460833U)
			{
				if (num <= 568916982U)
				{
					if (num != 29860882U)
					{
						if (num == 568916982U)
						{
							if (fullName == "Engine.Media.StreamingSource")
							{
								return SoundData.Stream(stream);
							}
						}
					}
					else if (fullName == "Engine.Media.Image")
					{
						return Image.Load(stream);
					}
				}
				else if (num != 1264629997U)
				{
					if (num != 1396463678U)
					{
						if (num == 2314460833U)
						{
							if (fullName == "System.Xml.Linq.XElement")
							{
								return XElement.Load(stream);
							}
						}
					}
					else if (fullName == "Game.JsonModel")
					{
						return JsonModelReader.Load(stream);
					}
				}
				else if (fullName == "Game.ObjModel")
				{
					return ObjModelReader.Load(stream);
				}
			}
			else if (num <= 3466117780U)
			{
				if (num != 2331779425U)
				{
					if (num != 2884932432U)
					{
						if (num == 3466117780U)
						{
							if (fullName == "SimpleJson.JsonObject")
							{
								return SimpleJson.DeserializeObject(new StreamReader(stream).ReadToEnd());
							}
						}
					}
					else if (fullName == "Engine.Audio.SoundBuffer")
					{
						return SoundBuffer.Load(stream);
					}
				}
				else if (fullName == "Engine.Graphics.Texture2D")
				{
					return Texture2D.Load(stream, false, 1);
				}
			}
			else if (num != 3467816364U)
			{
				if (num != 4201364391U)
				{
					if (num == 4268904329U)
					{
						if (fullName == "Engine.Graphics.Model")
						{
							return Model.Load(stream, true);
						}
					}
				}
				else if (fullName == "System.String")
				{
					return new StreamReader(stream).ReadToEnd();
				}
			}
			else if (fullName == "Game.MtllibStruct")
			{
				return MtllibStruct.Load(stream);
			}
			return null;
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x000C89D8 File Offset: 0x000C6BD8
		public static void Add(ContentInfo contentInfo)
		{
			object obj = ContentManager.syncobj;
			lock (obj)
			{
				ContentInfo contentInfo2;
				if (!ContentManager.Resources.TryGetValue(contentInfo.AbsolutePath, out contentInfo2))
				{
					ContentManager.Resources.Add(contentInfo.AbsolutePath, contentInfo);
				}
				else
				{
					ContentManager.Resources[contentInfo.AbsolutePath] = contentInfo;
				}
			}
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x000C8A4C File Offset: 0x000C6C4C
		public static void Dispose(string name)
		{
			object obj = ContentManager.syncobj;
			lock (obj)
			{
				List<object> list;
				if (ContentManager.Caches.TryGetValue(name, out list))
				{
					foreach (object obj2 in list)
					{
						IDisposable disposable = obj2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					list.Remove(name);
				}
			}
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x000C8AE4 File Offset: 0x000C6CE4
		public static bool ContainsKey(string key)
		{
			return ContentManager.Resources.ContainsKey(key);
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x000C8AF4 File Offset: 0x000C6CF4
		public static bool IsContent(object content)
		{
			foreach (List<object> list in ContentManager.Caches.Values)
			{
				using (List<object>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == content)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x000C8B84 File Offset: 0x000C6D84
		public static void Display_DeviceReset()
		{
			foreach (KeyValuePair<string, List<object>> keyValuePair in ContentManager.Caches)
			{
				string key = keyValuePair.Key;
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					object obj = keyValuePair.Value[i];
					if (obj is Texture2D || obj is Model || obj is BitmapFont)
					{
						keyValuePair.Value[i] = ContentManager.Get(obj.GetType(), key, null);
					}
				}
			}
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000C8C34 File Offset: 0x000C6E34
		public static ReadOnlyList<ContentInfo> List()
		{
			return new ReadOnlyList<ContentInfo>(ContentManager.Resources.Values.ToDynamicArray<ContentInfo>());
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000C8C4C File Offset: 0x000C6E4C
		public static ReadOnlyList<ContentInfo> List(string directory)
		{
			List<ContentInfo> list = new List<ContentInfo>();
			if (!directory.EndsWith("/"))
			{
				directory += "/";
			}
			foreach (ContentInfo contentInfo in ContentManager.Resources.Values)
			{
				if (contentInfo.ContentPath.StartsWith(directory))
				{
					list.Add(contentInfo);
				}
			}
			return new ReadOnlyList<ContentInfo>(list);
		}

		// Token: 0x0400117E RID: 4478
		internal static Dictionary<string, ContentInfo> Resources = new Dictionary<string, ContentInfo>();

		// Token: 0x0400117F RID: 4479
		internal static Dictionary<string, IContentReader> ReaderList = new Dictionary<string, IContentReader>();

		// Token: 0x04001180 RID: 4480
		internal static Dictionary<string, List<object>> Caches = new Dictionary<string, List<object>>();

		// Token: 0x04001181 RID: 4481
		internal static object syncobj = new object();
	}
}
