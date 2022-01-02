using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000141 RID: 321
	public static class FurniturePacksManager
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x00023955 File Offset: 0x00021B55
		public static ReadOnlyList<string> FurniturePackNames
		{
			get
			{
				return new ReadOnlyList<string>(FurniturePacksManager.m_furniturePackNames);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x00023961 File Offset: 0x00021B61
		public static string FurniturePacksDirectoryName
		{
			get
			{
				return "app:/FurniturePacks";
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000640 RID: 1600 RVA: 0x00023968 File Offset: 0x00021B68
		// (remove) Token: 0x06000641 RID: 1601 RVA: 0x0002399C File Offset: 0x00021B9C
		public static event Action<string> FurniturePackDeleted;

		// Token: 0x06000643 RID: 1603 RVA: 0x000239DB File Offset: 0x00021BDB
		public static void Initialize()
		{
			Storage.CreateDirectory(FurniturePacksManager.FurniturePacksDirectoryName);
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x000239E7 File Offset: 0x00021BE7
		public static string GetFileName(string name)
		{
			return Storage.CombinePaths(new string[]
			{
				FurniturePacksManager.FurniturePacksDirectoryName,
				name
			});
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00023A00 File Offset: 0x00021C00
		public static string GetDisplayName(string name)
		{
			return Storage.GetFileNameWithoutExtension(name);
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00023A08 File Offset: 0x00021C08
		public static DateTime GetCreationDate(string name)
		{
			DateTime result;
			try
			{
				result = Storage.GetFileLastWriteTime(FurniturePacksManager.GetFileName(name));
			}
			catch
			{
				result = new DateTime(2000, 1, 1);
			}
			return result;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00023A44 File Offset: 0x00021C44
		public static string ImportFurniturePack(string name, Stream stream)
		{
			if (MarketplaceManager.IsTrialMode)
			{
				throw new InvalidOperationException("Cannot import furniture packs in trial mode.");
			}
			FurniturePacksManager.ValidateFurniturePack(stream);
			stream.Position = 0L;
			string fileNameWithoutExtension = Storage.GetFileNameWithoutExtension(name);
			name = fileNameWithoutExtension + ".scfpack";
			string fileName = FurniturePacksManager.GetFileName(name);
			int num = 0;
			while (Storage.FileExists(fileName))
			{
				num++;
				if (num > 9)
				{
					throw new InvalidOperationException("Duplicate name. Delete existing content with conflicting names.");
				}
				name = string.Format("{0} ({1}).scfpack", fileNameWithoutExtension, num);
				fileName = FurniturePacksManager.GetFileName(name);
			}
			string result;
			using (Stream stream2 = Storage.OpenFile(fileName, OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00023AF8 File Offset: 0x00021CF8
		public static void ExportFurniturePack(string name, Stream stream)
		{
			using (Stream stream2 = Storage.OpenFile(FurniturePacksManager.GetFileName(name), OpenFileMode.Read))
			{
				stream2.CopyTo(stream);
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00023B38 File Offset: 0x00021D38
		public static string CreateFurniturePack(string name, ICollection<FurnitureDesign> designs)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (ZipArchive zipArchive = ZipArchive.Create(memoryStream, true))
			{
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				SubsystemFurnitureBlockBehavior.SaveFurnitureDesigns(valuesDictionary, designs);
				XElement xelement = new XElement("FurnitureDesigns");
				valuesDictionary.Save(xelement);
				MemoryStream memoryStream2 = new MemoryStream();
				xelement.Save(memoryStream2);
				memoryStream2.Position = 0L;
				zipArchive.AddStream("FurnitureDesigns.xml", memoryStream2);
			}
			memoryStream.Position = 0L;
			return FurniturePacksManager.ImportFurniturePack(name, memoryStream);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00023BC4 File Offset: 0x00021DC4
		public static void DeleteFurniturePack(string name)
		{
			try
			{
				Storage.DeleteFile(FurniturePacksManager.GetFileName(name));
				Action<string> furniturePackDeleted = FurniturePacksManager.FurniturePackDeleted;
				if (furniturePackDeleted != null)
				{
					furniturePackDeleted(name);
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete furniture pack \"" + name + "\"", e);
			}
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00023C18 File Offset: 0x00021E18
		public static void UpdateFurniturePacksList()
		{
			FurniturePacksManager.m_furniturePackNames.Clear();
			foreach (string text in Storage.ListFileNames(FurniturePacksManager.FurniturePacksDirectoryName))
			{
				if (Storage.GetExtension(text).ToLower() == ".scfpack")
				{
					FurniturePacksManager.m_furniturePackNames.Add(text);
				}
			}
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00023C90 File Offset: 0x00021E90
		public static List<FurnitureDesign> LoadFurniturePack(SubsystemTerrain subsystemTerrain, string name)
		{
			List<FurnitureDesign> result;
			using (Stream stream = Storage.OpenFile(FurniturePacksManager.GetFileName(name), OpenFileMode.Read))
			{
				result = FurniturePacksManager.LoadFurniturePack(subsystemTerrain, stream);
			}
			return result;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00023CD0 File Offset: 0x00021ED0
		public static void ValidateFurniturePack(Stream stream)
		{
			FurniturePacksManager.LoadFurniturePack(null, stream);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00023CDC File Offset: 0x00021EDC
		public static List<FurnitureDesign> LoadFurniturePack(SubsystemTerrain subsystemTerrain, Stream stream)
		{
			List<FurnitureDesign> result;
			using (ZipArchive zipArchive = ZipArchive.Open(stream, true))
			{
				List<ZipArchiveEntry> list = zipArchive.ReadCentralDir();
				if (list.Count != 1 || list[0].FilenameInZip != "FurnitureDesigns.xml")
				{
					throw new InvalidOperationException("Invalid furniture pack.");
				}
				MemoryStream memoryStream = new MemoryStream();
				zipArchive.ExtractFile(list[0], memoryStream);
				memoryStream.Position = 0L;
				XElement overridesNode = XElement.Load(memoryStream);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.ApplyOverrides(overridesNode);
				result = SubsystemFurnitureBlockBehavior.LoadFurnitureDesigns(subsystemTerrain, valuesDictionary);
			}
			return result;
		}

		// Token: 0x040002B6 RID: 694
		public static List<string> m_furniturePackNames = new List<string>();
	}
}
