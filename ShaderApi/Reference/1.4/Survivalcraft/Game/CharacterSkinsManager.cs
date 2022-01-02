using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Engine.Media;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200013B RID: 315
	public static class CharacterSkinsManager
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x00021ECA File Offset: 0x000200CA
		public static ReadOnlyList<string> CharacterSkinsNames
		{
			get
			{
				return new ReadOnlyList<string>(CharacterSkinsManager.m_characterSkinNames);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x00021ED6 File Offset: 0x000200D6
		public static string CharacterSkinsDirectoryName
		{
			get
			{
				return "app:/CharacterSkins";
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060005FB RID: 1531 RVA: 0x00021EE0 File Offset: 0x000200E0
		// (remove) Token: 0x060005FC RID: 1532 RVA: 0x00021F14 File Offset: 0x00020114
		public static event Action<string> CharacterSkinDeleted;

		// Token: 0x060005FD RID: 1533 RVA: 0x00021F47 File Offset: 0x00020147
		public static void Initialize()
		{
			Storage.CreateDirectory(CharacterSkinsManager.CharacterSkinsDirectoryName);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00021F53 File Offset: 0x00020153
		public static bool IsBuiltIn(string name)
		{
			return name.StartsWith("$");
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00021F60 File Offset: 0x00020160
		public static PlayerClass? GetPlayerClass(string name)
		{
			name = name.ToLower();
			if (name.Contains("female") || name.Contains("girl") || name.Contains("woman"))
			{
				return new PlayerClass?(PlayerClass.Female);
			}
			if (name.Contains("male") || name.Contains("boy") || name.Contains("man"))
			{
				return new PlayerClass?(PlayerClass.Male);
			}
			return null;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00021FDA File Offset: 0x000201DA
		public static string GetFileName(string name)
		{
			if (CharacterSkinsManager.IsBuiltIn(name))
			{
				return null;
			}
			return Storage.CombinePaths(new string[]
			{
				CharacterSkinsManager.CharacterSkinsDirectoryName,
				name
			});
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00022000 File Offset: 0x00020200
		public static string GetDisplayName(string name)
		{
			if (!CharacterSkinsManager.IsBuiltIn(name))
			{
				return Storage.GetFileNameWithoutExtension(name);
			}
			if (name.Contains("Female"))
			{
				if (name.Contains("1"))
				{
					return "Doris";
				}
				if (name.Contains("2"))
				{
					return "Mabel";
				}
				if (name.Contains("3"))
				{
					return "Ada";
				}
				return "Shirley";
			}
			else
			{
				if (name.Contains("1"))
				{
					return "Walter";
				}
				if (name.Contains("2"))
				{
					return "Basil";
				}
				if (name.Contains("3"))
				{
					return "Geoffrey";
				}
				return "Zachary";
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x000220AC File Offset: 0x000202AC
		public static DateTime GetCreationDate(string name)
		{
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					return Storage.GetFileLastWriteTime(fileName);
				}
			}
			catch
			{
			}
			return new DateTime(2000, 1, 1);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x000220F4 File Offset: 0x000202F4
		public static Texture2D LoadTexture(string name)
		{
			Texture2D texture2D = null;
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
					{
						CharacterSkinsManager.ValidateCharacterSkin(stream);
						stream.Position = 0L;
						texture2D = Texture2D.Load(stream, false, 1);
						goto IL_63;
					}
				}
				texture2D = ContentManager.Get<Texture2D>("Textures/Creatures/Human" + name.Substring(1).Replace(" ", ""), null);
				IL_63:;
			}
			catch (Exception ex)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Could not load character skin \"",
					name,
					"\". Reason: ",
					ex.Message,
					"."
				}));
			}
			if (texture2D == null)
			{
				texture2D = ContentManager.Get<Texture2D>("Textures/Creatures/HumanMale1", null);
			}
			return texture2D;
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x000221CC File Offset: 0x000203CC
		public static string ImportCharacterSkin(string name, Stream stream)
		{
			Exception ex = ExternalContentManager.VerifyExternalContentName(name);
			if (ex != null)
			{
				throw ex;
			}
			if (Storage.GetExtension(name) != ".scskin")
			{
				name += ".scskin";
			}
			CharacterSkinsManager.ValidateCharacterSkin(stream);
			stream.Position = 0L;
			string result;
			using (Stream stream2 = Storage.OpenFile(CharacterSkinsManager.GetFileName(name), OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00022248 File Offset: 0x00020448
		public static void DeleteCharacterSkin(string name)
		{
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					Storage.DeleteFile(fileName);
					Action<string> characterSkinDeleted = CharacterSkinsManager.CharacterSkinDeleted;
					if (characterSkinDeleted != null)
					{
						characterSkinDeleted(name);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete character skin \"" + name + "\"", e);
			}
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x000222A8 File Offset: 0x000204A8
		public static void UpdateCharacterSkinsList()
		{
			CharacterSkinsManager.m_characterSkinNames.Clear();
			CharacterSkinsManager.m_characterSkinNames.Add("$Male1");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male2");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male3");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male4");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female1");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female2");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female3");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female4");
			foreach (string text in Storage.ListFileNames(CharacterSkinsManager.CharacterSkinsDirectoryName))
			{
				if (Storage.GetExtension(text).ToLower() == ".scskin")
				{
					CharacterSkinsManager.m_characterSkinNames.Add(text);
				}
			}
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00022398 File Offset: 0x00020598
		public static Model GetPlayerModel(PlayerClass playerClass)
		{
			Model model;
			if (!CharacterSkinsManager.m_playerModels.TryGetValue(playerClass, out model))
			{
				ValuesDictionary valuesDictionary;
				if (playerClass != PlayerClass.Male)
				{
					if (playerClass != PlayerClass.Female)
					{
						throw new InvalidOperationException("Unknown player class.");
					}
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("FemalePlayer", true);
				}
				else
				{
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("MalePlayer", true);
				}
				model = ContentManager.Get<Model>(valuesDictionary.GetValue<ValuesDictionary>("HumanModel").GetValue<string>("ModelName"), null);
				CharacterSkinsManager.m_playerModels.Add(playerClass, model);
			}
			return model;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00022410 File Offset: 0x00020610
		public static Model GetOuterClothingModel(PlayerClass playerClass)
		{
			Model model;
			if (!CharacterSkinsManager.m_outerClothingModels.TryGetValue(playerClass, out model))
			{
				ValuesDictionary valuesDictionary;
				if (playerClass != PlayerClass.Male)
				{
					if (playerClass != PlayerClass.Female)
					{
						throw new InvalidOperationException("Unknown player class.");
					}
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("FemalePlayer", true);
				}
				else
				{
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("MalePlayer", true);
				}
				model = ContentManager.Get<Model>(valuesDictionary.GetValue<ValuesDictionary>("OuterClothingModel").GetValue<string>("ModelName"), null);
				CharacterSkinsManager.m_outerClothingModels.Add(playerClass, model);
			}
			return model;
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00022488 File Offset: 0x00020688
		public static void ValidateCharacterSkin(Stream stream)
		{
			Image image = Image.Load(stream);
			if (image.Width > 256 || image.Height > 256)
			{
				throw new InvalidOperationException(string.Format("Character skin is larger than 256x256 pixels (size={0}x{1})", image.Width, image.Height));
			}
			if (!MathUtils.IsPowerOf2((long)image.Width) || !MathUtils.IsPowerOf2((long)image.Height))
			{
				throw new InvalidOperationException(string.Format("Character skin does not have power-of-two size (size={0}x{1})", image.Width, image.Height));
			}
		}

		// Token: 0x040002A9 RID: 681
		public static List<string> m_characterSkinNames = new List<string>();

		// Token: 0x040002AA RID: 682
		public static Dictionary<PlayerClass, Model> m_playerModels = new Dictionary<PlayerClass, Model>();

		// Token: 0x040002AB RID: 683
		public static Dictionary<PlayerClass, Model> m_outerClothingModels = new Dictionary<PlayerClass, Model>();
	}
}
