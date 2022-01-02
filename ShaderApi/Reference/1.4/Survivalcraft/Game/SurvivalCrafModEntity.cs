using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Media;
using Game.IContentReader;

namespace Game
{
	// Token: 0x0200015A RID: 346
	public class SurvivalCrafModEntity : ModEntity
	{
		// Token: 0x0600079C RID: 1948 RVA: 0x0002B128 File Offset: 0x00029328
		public SurvivalCrafModEntity()
		{
			List<IContentReader> list = new List<IContentReader>();
			list.Add(new BitmapFontReader());
			list.Add(new DaeModelReader());
			list.Add(new ImageReader());
			list.Add(new JsonArrayReader());
			list.Add(new JsonObjectReader());
			list.Add(new JsonModelReader());
			list.Add(new MtllibStructReader());
			list.Add(new ObjModelReader());
			list.Add(new ShaderReader());
			list.Add(new SoundBufferReader());
			list.Add(new StreamingSourceReader());
			list.Add(new Game.IContentReader.StringReader());
			list.Add(new SubtextureReader());
			list.Add(new Texture2DReader());
			list.Add(new XmlReader());
			for (int i = 0; i < list.Count; i++)
			{
				ContentManager.ReaderList.Add(list[i].Type, list[i]);
			}
			Stream stream = Storage.OpenFile("app:Content.zip", OpenFileMode.Read);
			MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			stream.Close();
			memoryStream.Position = 0L;
			this.ModArchive = ZipArchive.Open(memoryStream, true);
			this.InitResources();
			LabelWidget.BitmapFont = ContentManager.Get<BitmapFont>("Fonts/Pericles", null);
			string str = "加载资源:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x0002B27D File Offset: 0x0002947D
		public override void LoadBlocksData()
		{
			string str = "加载方块数据:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			BlocksManager.LoadBlocksData(ContentManager.Get<string>("BlocksData", null));
			ContentManager.Dispose("BlocksData");
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0002B2BC File Offset: 0x000294BC
		public override void LoadDll()
		{
			List<Type> list = new List<Type>();
			Type[] types = typeof(BlocksManager).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsSubclassOf(typeof(ModLoader)) && !type.IsAbstract)
				{
					ModLoader modLoader = Activator.CreateInstance(types[i]) as ModLoader;
					modLoader.Entity = this;
					modLoader.__ModInitialize();
					base.Loader = modLoader;
					ModsManager.ModLoaders.Add(modLoader);
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
					ModsManager.AddException(new InvalidOperationException("Block type \"" + type2.FullName + "\" does not have static field Index of type int."), false);
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

		// Token: 0x0600079F RID: 1951 RVA: 0x0002B437 File Offset: 0x00029637
		public override void LoadXdb(ref XElement xElement)
		{
			string str = "加载数据库:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			xElement = ContentManager.Get<XElement>("Database", null);
			ContentManager.Dispose("Database");
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0002B471 File Offset: 0x00029671
		public override void LoadCr(ref XElement xElement)
		{
			string str = "加载合成谱:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			xElement = ContentManager.Get<XElement>("CraftingRecipes", null);
			ContentManager.Dispose("CraftingRecipes");
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0002B4AB File Offset: 0x000296AB
		public override void LoadClo(ClothingBlock block, ref XElement xElement)
		{
			string str = "加载衣物数据:";
			ModInfo modInfo = this.modInfo;
			LoadingScreen.Info(str + ((modInfo != null) ? modInfo.Name : null));
			xElement = ContentManager.Get<XElement>("Clothes", null);
			ContentManager.Dispose("Clothes");
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0002B4E5 File Offset: 0x000296E5
		public override void SaveSettings(XElement xElement)
		{
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0002B4E7 File Offset: 0x000296E7
		public override void LoadSettings(XElement xElement)
		{
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0002B4EC File Offset: 0x000296EC
		public override void OnBlocksInitalized()
		{
			BlocksManager.AddCategory("Terrain");
			BlocksManager.AddCategory("Plants");
			BlocksManager.AddCategory("Construction");
			BlocksManager.AddCategory("Items");
			BlocksManager.AddCategory("Tools");
			BlocksManager.AddCategory("Weapons");
			BlocksManager.AddCategory("Clothes");
			BlocksManager.AddCategory("Electrics");
			BlocksManager.AddCategory("Food");
			BlocksManager.AddCategory("Spawner Eggs");
			BlocksManager.AddCategory("Painted");
			BlocksManager.AddCategory("Dyed");
			BlocksManager.AddCategory("Fireworks");
		}
	}
}
