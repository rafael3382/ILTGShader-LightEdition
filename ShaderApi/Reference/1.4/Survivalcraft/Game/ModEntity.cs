using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Game.IContentReader;

namespace Game
{
	// Token: 0x02000369 RID: 873
	public class ModEntity
	{
		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001991 RID: 6545 RVA: 0x000CA500 File Offset: 0x000C8700
		// (set) Token: 0x06001992 RID: 6546 RVA: 0x000CA508 File Offset: 0x000C8708
		public ModLoader Loader
		{
			get
			{
				return this.ModLoader_;
			}
			set
			{
				this.ModLoader_ = value;
			}
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000CA511 File Offset: 0x000C8711
		public ModEntity()
		{
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x000CA52F File Offset: 0x000C872F
		public ModEntity(ZipArchive zipArchive)
		{
			this.ModArchive = zipArchive;
			this.InitResources();
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000CA55A File Offset: 0x000C875A
		public virtual void LoadIcon(Stream stream)
		{
			this.Icon = Texture2D.Load(stream, false, 1);
			stream.Close();
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x000CA570 File Offset: 0x000C8770
		public virtual void GetFiles(string extension, Action<string, Stream> action)
		{
			new List<Stream>();
			foreach (ZipArchiveEntry zipArchiveEntry in this.ModArchive.ReadCentralDir())
			{
				if (Storage.GetExtension(zipArchiveEntry.FilenameInZip) == extension)
				{
					MemoryStream memoryStream = new MemoryStream();
					this.ModArchive.ExtractFile(zipArchiveEntry, memoryStream);
					memoryStream.Position = 0L;
					try
					{
						action(zipArchiveEntry.FilenameInZip, memoryStream);
					}
					catch (Exception ex)
					{
						Log.Error(string.Format("GetFile {0} Error:{1}", zipArchiveEntry.FilenameInZip, ex.Message));
					}
					finally
					{
						memoryStream.Dispose();
					}
				}
			}
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x000CA644 File Offset: 0x000C8844
		public virtual bool GetFile(string filename, Action<Stream> stream)
		{
			ZipArchiveEntry zfe;
			if (this.ModFiles.TryGetValue(filename, out zfe))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					this.ModArchive.ExtractFile(zfe, memoryStream);
					memoryStream.Position = 0L;
					try
					{
						if (stream != null)
						{
							stream(memoryStream);
						}
					}
					catch (Exception ex)
					{
						LoadingScreen.Error("GetFile " + filename + " Error:" + ex.Message);
					}
				}
			}
			return false;
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x000CA6D0 File Offset: 0x000C88D0
		public virtual bool GetAssetsFile(string filename, Action<Stream> stream)
		{
			return this.GetFile("Assets/" + filename, stream);
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x000CA6E4 File Offset: 0x000C88E4
		public virtual void LoadLauguage()
		{
			string str = "加载语言:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			this.GetAssetsFile("Lang/" + ModsManager.Configs["Language"] + ".json", delegate(Stream stream)
			{
				LanguageControl.loadJson(stream);
			});
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x000CA756 File Offset: 0x000C8956
		public virtual void ModInitialize()
		{
			string str = "初始化Mod方法:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			ModLoader modLoader_ = this.ModLoader_;
			if (modLoader_ == null)
			{
				return;
			}
			modLoader_.__ModInitialize();
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x000CA78C File Offset: 0x000C898C
		public virtual void InitResources()
		{
			this.ModFiles.Clear();
			if (this.ModArchive == null)
			{
				return;
			}
			foreach (ZipArchiveEntry zipArchiveEntry in this.ModArchive.ReadCentralDir())
			{
				if (zipArchiveEntry.FileSize > 0U)
				{
					this.ModFiles.Add(zipArchiveEntry.FilenameInZip, zipArchiveEntry);
				}
			}
			this.GetFile("modinfo.json", delegate(Stream stream)
			{
				this.modInfo = ModsManager.DeserializeJson<ModInfo>(ModsManager.StreamToString(stream));
			});
			if (this.modInfo == null)
			{
				return;
			}
			this.GetFile("icon.png", delegate(Stream stream)
			{
				this.LoadIcon(stream);
			});
			foreach (KeyValuePair<string, ZipArchiveEntry> keyValuePair in this.ModFiles)
			{
				ZipArchiveEntry value = keyValuePair.Value;
				string filenameInZip = value.FilenameInZip;
				if (!value.IsFilenameUtf8)
				{
					ModsManager.AddException(new Exception(string.Concat(new string[]
					{
						"文件名[",
						value.FilenameInZip,
						"]编码不是Utf8，请进行修正，相关Mod[",
						this.modInfo.Name,
						"]"
					})), false);
				}
				if (filenameInZip.StartsWith("Assets/"))
				{
					MemoryStream memoryStream = new MemoryStream();
					ContentInfo contentInfo = new ContentInfo(filenameInZip.Substring(7));
					this.ModArchive.ExtractFile(value, memoryStream);
					contentInfo.SetContentStream(memoryStream);
					ContentManager.Add(contentInfo);
				}
			}
			string[] array = new string[5];
			array[0] = "加载资源:";
			int num = 1;
			ModInfo modInfo = this.modInfo;
			array[num] = ((modInfo != null) ? modInfo.Name : null);
			array[2] = " 共";
			array[3] = this.ModFiles.Count.ToString();
			array[4] = "文件";
			LoadingScreen.Info(string.Concat(array));
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x000CA97C File Offset: 0x000C8B7C
		public virtual void LoadBlocksData()
		{
			string str = "加载方块数据:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			this.GetFiles(".csv", delegate(string filename, Stream stream)
			{
				BlocksManager.LoadBlocksData(ModsManager.StreamToString(stream));
			});
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x000CA9D4 File Offset: 0x000C8BD4
		public virtual void LoadXdb(ref XElement xElement)
		{
			XElement element = xElement;
			string str = "加载数据库:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			this.GetFiles(".xdb", delegate(string filename, Stream stream)
			{
				ModsManager.CombineDataBase(element, stream);
			});
			if (this.Loader != null)
			{
				this.Loader.OnXdbLoad(xElement);
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x000CAA3C File Offset: 0x000C8C3C
		public virtual void LoadClo(ClothingBlock block, ref XElement xElement)
		{
			string str = "加载衣物数据:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			this.GetFiles(".clo", delegate(string filename, Stream stream)
			{
				ModsManager.CombineClo(xElement, stream);
			});
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x000CAA90 File Offset: 0x000C8C90
		public virtual void LoadCr(ref XElement xElement)
		{
			string str = "加载合成谱:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			this.GetFiles(".cr", delegate(string filename, Stream stream)
			{
				ModsManager.CombineCr(xElement, stream);
			});
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x000CAAE3 File Offset: 0x000C8CE3
		public virtual void LoadDll()
		{
			string str = "加载程序集:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.PackageName : null));
			this.GetFiles(".dll", delegate(string filename, Stream stream)
			{
				this.LoadDllLogic(stream);
			});
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x000CAB20 File Offset: 0x000C8D20
		public void LoadDllLogic(Stream stream)
		{
			Assembly assembly = Assembly.Load(ModsManager.StreamToBytes(stream));
			List<Type> list = new List<Type>();
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsSubclassOf(typeof(ModLoader)) && !type.IsAbstract)
				{
					ModLoader modLoader = Activator.CreateInstance(types[i]) as ModLoader;
					modLoader.Entity = this;
					this.Loader = modLoader;
					modLoader.__ModInitialize();
					ModsManager.ModLoaders.Add(modLoader);
				}
				if (type.IsSubclassOf(typeof(IContentReader)) && !type.IsAbstract)
				{
					IContentReader contentReader = Activator.CreateInstance(type) as IContentReader;
					if (!ContentManager.ReaderList.ContainsKey(contentReader.Type))
					{
						ContentManager.ReaderList.Add(contentReader.Type, contentReader);
					}
				}
				if (type.IsSubclassOf(typeof(Block)) && !type.IsAbstract)
				{
					list.Add(type);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Type type2 = list[j];
				FieldInfo fieldInfo = type2.GetRuntimeFields().FirstOrDefault((FieldInfo p) => p.Name == "Index" && p.IsPublic && p.IsStatic);
				if (fieldInfo == null || fieldInfo.FieldType != typeof(int))
				{
					LoadingScreen.Warning("Block type \"" + type2.FullName + "\" does not have static field Index of type int.");
				}
				else
				{
					int blockIndex = (int)fieldInfo.GetValue(null);
					Block block = (Block)Activator.CreateInstance(type2.GetTypeInfo().AsType());
					block.BlockIndex = blockIndex;
					this.Blocks.Add(block);
				}
			}
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x000CACE4 File Offset: 0x000C8EE4
		public virtual void CheckDependencies()
		{
			string str = "检查依赖项:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.PackageName : null));
			for (int i = 0; i < this.modInfo.Dependencies.Count; i++)
			{
				int index = i;
				string text = this.modInfo.Dependencies[index];
				string dn = "";
				Version dnversion = new Version();
				if (text.Contains(":"))
				{
					string[] array = text.Split(new char[]
					{
						':'
					});
					if (array.Length == 2)
					{
						dn = array[0];
						dnversion = new Version(array[1]);
					}
				}
				else
				{
					dn = text;
				}
				ModEntity modEntity = ModsManager.ModList.Find((ModEntity px) => px.modInfo.PackageName == dn && new Version(px.modInfo.Version) == dnversion);
				if (modEntity == null)
				{
					ModsManager.AddException(new Exception("[" + this.modInfo.Name + "]缺少依赖项" + text), false);
					return;
				}
				modEntity.CheckDependencies();
				this.IsChecked = true;
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x000CADFF File Offset: 0x000C8FFF
		public virtual void SaveSettings(XElement xElement)
		{
			if (this.Loader != null)
			{
				this.Loader.SaveSettings(xElement);
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x000CAE15 File Offset: 0x000C9015
		public virtual void LoadSettings(XElement xElement)
		{
			if (this.Loader != null)
			{
				this.Loader.LoadSettings(xElement);
			}
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x000CAE2B File Offset: 0x000C902B
		public virtual void OnBlocksInitalized()
		{
			if (this.Loader != null)
			{
				this.Loader.BlocksInitalized();
			}
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x000CAE40 File Offset: 0x000C9040
		public virtual void Dispose()
		{
			try
			{
				if (this.Loader != null)
				{
					this.Loader.ModDispose();
				}
			}
			catch
			{
			}
			ZipArchive modArchive = this.ModArchive;
			if (modArchive == null)
			{
				return;
			}
			modArchive.ZipFileStream.Close();
		}

		// Token: 0x0400118D RID: 4493
		public ModInfo modInfo;

		// Token: 0x0400118E RID: 4494
		public Texture2D Icon;

		// Token: 0x0400118F RID: 4495
		public ZipArchive ModArchive;

		// Token: 0x04001190 RID: 4496
		public Dictionary<string, ZipArchiveEntry> ModFiles = new Dictionary<string, ZipArchiveEntry>();

		// Token: 0x04001191 RID: 4497
		public List<Block> Blocks = new List<Block>();

		// Token: 0x04001192 RID: 4498
		public bool IsChecked;

		// Token: 0x04001193 RID: 4499
		private ModLoader ModLoader_;
	}
}
