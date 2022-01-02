using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000153 RID: 339
	public class FastDebugModEntity : ModEntity
	{
		// Token: 0x0600074C RID: 1868 RVA: 0x00028B5C File Offset: 0x00026D5C
		public FastDebugModEntity()
		{
			this.modInfo = new ModInfo
			{
				Name = "[Debug]",
				PackageName = "debug"
			};
			this.InitResources();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00028B98 File Offset: 0x00026D98
		public override void InitResources()
		{
			this.ReadDirResouces(ModsManager.ModsPath, "");
			if (!this.GetFile("modinfo.json", delegate(Stream stream)
			{
				this.modInfo = ModsManager.DeserializeJson<ModInfo>(ModsManager.StreamToString(stream));
				this.modInfo.Name = "[Debug]" + this.modInfo.Name;
			}))
			{
				this.modInfo = new ModInfo
				{
					Name = "FastDebug",
					Version = "1.0.0",
					ApiVersion = "1.40",
					Author = "Mod",
					Description = "调试Mod插件",
					ScVersion = "2.2.10.4",
					PackageName = "com.fastdebug"
				};
			}
			this.GetFile("icon.png", delegate(Stream stream)
			{
				this.LoadIcon(stream);
			});
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00028C40 File Offset: 0x00026E40
		public void ReadDirResouces(string basepath, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = basepath;
			}
			foreach (string str in Storage.ListDirectoryNames(path))
			{
				this.ReadDirResouces(basepath, path + "/" + str);
			}
			foreach (string str2 in Storage.ListFileNames(path))
			{
				string text = path + "/" + str2;
				string text2 = text.Substring(basepath.Length + 1);
				if (text2.StartsWith("Assets/"))
				{
					ContentInfo contentInfo = new ContentInfo(text2.Substring(7));
					MemoryStream memoryStream = new MemoryStream();
					using (Stream stream = Storage.OpenFile(text, OpenFileMode.Read))
					{
						stream.CopyTo(memoryStream);
						contentInfo.SetContentStream(memoryStream);
						ContentManager.Add(contentInfo);
					}
				}
				this.FModFiles.Add(text2, new FileInfo(Storage.GetSystemPath(text)));
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00028D74 File Offset: 0x00026F74
		public override void LoadDll()
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text.EndsWith(".dll") && !text.StartsWith("EntitySystem") && !text.StartsWith("Engine") && !text.StartsWith("Survivalcraft") && !text.StartsWith("OpenTK"))
				{
					base.LoadDllLogic(Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read));
					break;
				}
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00028E24 File Offset: 0x00027024
		public override void LoadClo(ClothingBlock block, ref XElement xElement)
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text.EndsWith(".clo"))
				{
					ModsManager.CombineClo(xElement, Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read));
				}
			}
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00028EA0 File Offset: 0x000270A0
		public override void LoadCr(ref XElement xElement)
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text.EndsWith(".cr"))
				{
					ModsManager.CombineCr(xElement, Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read));
				}
			}
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00028F1C File Offset: 0x0002711C
		public override void LoadLauguage()
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text == ModsManager.Configs["Language"] + ".json")
				{
					LanguageControl.loadJson(Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read));
				}
			}
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00028FAC File Offset: 0x000271AC
		public override void LoadBlocksData()
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text.EndsWith(".csv"))
				{
					BlocksManager.LoadBlocksData(ModsManager.StreamToString(Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read)));
				}
			}
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0002902C File Offset: 0x0002722C
		public override void LoadXdb(ref XElement xElement)
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				if (text.EndsWith(".xdb"))
				{
					ModsManager.CombineDataBase(xElement, Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ModsManager.ModsPath,
						text
					}), OpenFileMode.Read));
				}
			}
			ModLoader loader = base.Loader;
			if (loader == null)
			{
				return;
			}
			loader.OnXdbLoad(xElement);
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x000290BC File Offset: 0x000272BC
		public override void GetFiles(string extension, Action<string, Stream> action)
		{
			foreach (string text in Storage.ListFileNames(ModsManager.ModsPath))
			{
				using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
				{
					ModsManager.ModsPath,
					text
				}), OpenFileMode.Read))
				{
					try
					{
						action(text, stream);
					}
					catch (Exception ex)
					{
						LoadingScreen.Error(string.Format("GetFile {0} Error:{1}", text, ex.Message));
					}
				}
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x0002916C File Offset: 0x0002736C
		public override bool GetFile(string filename, Action<Stream> stream)
		{
			FileInfo fileInfo;
			if (this.FModFiles.TryGetValue(filename, out fileInfo))
			{
				using (Stream stream2 = fileInfo.OpenRead())
				{
					try
					{
						if (stream != null)
						{
							stream(stream2);
						}
					}
					catch (Exception ex)
					{
						Log.Error(string.Format("GetFile {0} Error:{1}", filename, ex.Message));
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x000291E0 File Offset: 0x000273E0
		public override bool GetAssetsFile(string filename, Action<Stream> stream)
		{
			return this.GetFile("Assets/" + filename, stream);
		}

		// Token: 0x0400032C RID: 812
		public Dictionary<string, FileInfo> FModFiles = new Dictionary<string, FileInfo>();
	}
}
