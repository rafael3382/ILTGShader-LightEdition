using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000152 RID: 338
	public static class WorldsManager
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x0002785B File Offset: 0x00025A5B
		public static ReadOnlyList<string> NewWorldNames
		{
			get
			{
				return WorldsManager.m_newWorldNames;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x00027862 File Offset: 0x00025A62
		public static ReadOnlyList<WorldInfo> WorldInfos
		{
			get
			{
				return new ReadOnlyList<WorldInfo>(WorldsManager.m_worldInfos);
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000731 RID: 1841 RVA: 0x00027870 File Offset: 0x00025A70
		// (remove) Token: 0x06000732 RID: 1842 RVA: 0x000278A4 File Offset: 0x00025AA4
		public static event Action<string> WorldDeleted;

		// Token: 0x06000733 RID: 1843 RVA: 0x000278D8 File Offset: 0x00025AD8
		public static void Initialize()
		{
			if (WorldsManager.Loaded)
			{
				return;
			}
			Storage.CreateDirectory(WorldsManager.WorldsDirectoryName);
			string text = ContentManager.Get<string>("NewWorldNames", null);
			WorldsManager.m_newWorldNames = new ReadOnlyList<string>(text.Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries));
			WorldsManager.Loaded = true;
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0002792C File Offset: 0x00025B2C
		public static string ImportWorld(Stream sourceStream)
		{
			if (MarketplaceManager.IsTrialMode)
			{
				throw new InvalidOperationException("Cannot import worlds in trial mode.");
			}
			if (WorldsManager.WorldInfos.Count >= 30)
			{
				throw new InvalidOperationException(string.Format("Too many worlds on device, maximum allowed is {0}. Delete some to free up space.", 30));
			}
			string unusedWorldDirectoryName = WorldsManager.GetUnusedWorldDirectoryName();
			Storage.CreateDirectory(unusedWorldDirectoryName);
			WorldsManager.UnpackWorld(unusedWorldDirectoryName, sourceStream, true);
			if (!WorldsManager.TestXmlFile(Storage.CombinePaths(new string[]
			{
				unusedWorldDirectoryName,
				"Project.xml"
			}), "Project"))
			{
				try
				{
					WorldsManager.DeleteWorld(unusedWorldDirectoryName);
				}
				catch
				{
				}
				throw new InvalidOperationException("Cannot import world because it does not contain valid world data.");
			}
			return unusedWorldDirectoryName;
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x000279D4 File Offset: 0x00025BD4
		public static void ExportWorld(string directoryName, Stream targetStream)
		{
			WorldsManager.PackWorld(directoryName, targetStream, null, true);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x000279DF File Offset: 0x00025BDF
		public static void DeleteWorld(string directoryName)
		{
			if (Storage.DirectoryExists(directoryName))
			{
				WorldsManager.DeleteWorldContents(directoryName, null);
				Storage.DeleteDirectory(directoryName);
			}
			Action<string> worldDeleted = WorldsManager.WorldDeleted;
			if (worldDeleted == null)
			{
				return;
			}
			worldDeleted(directoryName);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00027A08 File Offset: 0x00025C08
		public static void RepairWorldIfNeeded(string directoryName)
		{
			try
			{
				string text = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.xml"
				});
				if (!WorldsManager.TestXmlFile(text, "Project"))
				{
					Log.Warning("Project file at \"" + text + "\" is corrupt or nonexistent. Will try copying data from the backup file. If that fails, will try making a recovery project file.");
					string text2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						"Project.bak"
					});
					if (WorldsManager.TestXmlFile(text2, "Project"))
					{
						Storage.CopyFile(text2, text);
					}
					else
					{
						string path = Storage.CombinePaths(new string[]
						{
							directoryName,
							"Chunks.dat"
						});
						if (!Storage.FileExists(path))
						{
							throw new InvalidOperationException("Recovery project file could not be generated because chunks file does not exist.");
						}
						XElement xelement = ContentManager.Get<XElement>("RecoveryProject", null);
						using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
						{
							int num;
							int num2;
							int num3;
							TerrainSerializer14.ReadTOCEntry(stream, out num, out num2, out num3);
							Vector3 vector = new Vector3((float)(16 * num), 255f, (float)(16 * num2));
							xelement.Element("Subsystems").Element("Values").Element("Value").Attribute("Value").SetValue(HumanReadableConverter.ConvertToString(vector));
						}
						using (Stream stream2 = Storage.OpenFile(text, OpenFileMode.Create))
						{
							XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new InvalidOperationException("The world files are corrupt and could not be repaired.");
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00027BBC File Offset: 0x00025DBC
		public static void MakeQuickWorldBackup(string directoryName)
		{
			string text = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			if (Storage.FileExists(text))
			{
				string destinationPath = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.bak"
				});
				Storage.CopyFile(text, destinationPath);
			}
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00027C08 File Offset: 0x00025E08
		public static bool SnapshotExists(string directoryName, string snapshotName)
		{
			return Storage.FileExists(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName));
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00027C18 File Offset: 0x00025E18
		public static void TakeWorldSnapshot(string directoryName, string snapshotName)
		{
			using (Stream stream = Storage.OpenFile(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName), OpenFileMode.Create))
			{
				WorldsManager.PackWorld(directoryName, stream, (string fn) => Path.GetExtension(fn).ToLower() != ".snapshot", false);
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x00027C78 File Offset: 0x00025E78
		public static void RestoreWorldFromSnapshot(string directoryName, string snapshotName)
		{
			if (WorldsManager.SnapshotExists(directoryName, snapshotName))
			{
				WorldsManager.DeleteWorldContents(directoryName, (string fn) => Storage.GetExtension(fn).ToLower() != ".snapshot");
				using (Stream stream = Storage.OpenFile(WorldsManager.MakeSnapshotFilename(directoryName, snapshotName), OpenFileMode.Read))
				{
					WorldsManager.UnpackWorld(directoryName, stream, false);
				}
			}
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00027CE8 File Offset: 0x00025EE8
		public static void DeleteWorldSnapshot(string directoryName, string snapshotName)
		{
			string path = WorldsManager.MakeSnapshotFilename(directoryName, snapshotName);
			if (Storage.FileExists(path))
			{
				Storage.DeleteFile(path);
			}
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00027D0C File Offset: 0x00025F0C
		public static void UpdateWorldsList()
		{
			WorldsManager.m_worldInfos.Clear();
			foreach (string text in Storage.ListDirectoryNames(WorldsManager.WorldsDirectoryName))
			{
				WorldInfo worldInfo = WorldsManager.GetWorldInfo(Storage.CombinePaths(new string[]
				{
					WorldsManager.WorldsDirectoryName,
					text
				}));
				if (worldInfo != null)
				{
					WorldsManager.m_worldInfos.Add(worldInfo);
				}
			}
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00027D8C File Offset: 0x00025F8C
		public static bool ValidateWorldName(string name)
		{
			return !name.Contains("\\") && name.Length <= 128;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00027DAC File Offset: 0x00025FAC
		public static WorldInfo GetWorldInfo(string directoryName)
		{
			WorldInfo worldInfo = new WorldInfo();
			worldInfo.DirectoryName = directoryName;
			worldInfo.LastSaveTime = DateTime.MinValue;
			List<string> list = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, null, null);
			if (list.Count > 0)
			{
				foreach (string text in list)
				{
					DateTime fileLastWriteTime = Storage.GetFileLastWriteTime(text);
					if (fileLastWriteTime > worldInfo.LastSaveTime)
					{
						worldInfo.LastSaveTime = fileLastWriteTime;
					}
					try
					{
						worldInfo.Size += Storage.GetFileSize(text);
					}
					catch (Exception e)
					{
						Exception e3;
						Log.Error(ExceptionManager.MakeFullErrorMessage("Error getting size of file \"" + text + "\".", e3));
					}
				}
				string text2 = Storage.CombinePaths(new string[]
				{
					directoryName,
					"Project.xml"
				});
				try
				{
					if (Storage.FileExists(text2))
					{
						using (Stream stream = Storage.OpenFile(text2, OpenFileMode.Read))
						{
							XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
							worldInfo.SerializationVersion = XmlUtils.GetAttributeValue<string>(xelement, "Version", "1.0");
							VersionsManager.UpgradeProjectXml(xelement);
							XElement gameInfoNode = WorldsManager.GetGameInfoNode(xelement);
							ValuesDictionary valuesDictionary = new ValuesDictionary();
							valuesDictionary.ApplyOverrides(gameInfoNode);
							worldInfo.WorldSettings.Load(valuesDictionary);
							foreach (XContainer xcontainer in (from e in WorldsManager.GetPlayersNode(xelement).Elements()
							where XmlUtils.GetAttributeValue<string>(e, "Name") == "Players"
							select e).First<XElement>().Elements())
							{
								PlayerInfo playerInfo = new PlayerInfo();
								worldInfo.PlayerInfos.Add(playerInfo);
								XElement xelement2 = (from e in xcontainer.Elements()
								where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CharacterSkinName"
								select e).FirstOrDefault<XElement>();
								if (xelement2 != null)
								{
									playerInfo.CharacterSkinName = XmlUtils.GetAttributeValue<string>(xelement2, "Value", string.Empty);
								}
							}
							return worldInfo;
						}
					}
					return worldInfo;
				}
				catch (Exception e2)
				{
					Log.Error(ExceptionManager.MakeFullErrorMessage("Error getting data from project file \"" + text2 + "\".", e2));
					return worldInfo;
				}
			}
			return null;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x00028064 File Offset: 0x00026264
		public static WorldInfo CreateWorld(WorldSettings worldSettings)
		{
			string unusedWorldDirectoryName = WorldsManager.GetUnusedWorldDirectoryName();
			Storage.CreateDirectory(unusedWorldDirectoryName);
			if (!WorldsManager.ValidateWorldName(worldSettings.Name))
			{
				throw new InvalidOperationException("World name \"" + worldSettings.Name + "\" is invalid.");
			}
			int num;
			if (string.IsNullOrEmpty(worldSettings.Seed))
			{
				num = (int)((long)(Time.RealTime * 1000.0));
			}
			else if (worldSettings.Seed == "0")
			{
				num = 0;
			}
			else
			{
				num = 0;
				int num2 = 1;
				foreach (char c in worldSettings.Seed)
				{
					num += (int)c * num2;
					num2 += 29;
				}
			}
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			worldSettings.Save(valuesDictionary, false);
			valuesDictionary.SetValue<string>("WorldDirectoryName", unusedWorldDirectoryName);
			valuesDictionary.SetValue<int>("WorldSeed", num);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary2.SetValue<ValuesDictionary>("Players", new ValuesDictionary());
			DatabaseObject databaseObject = DatabaseManager.GameDatabase.Database.FindDatabaseObject("GameProject", DatabaseManager.GameDatabase.ProjectTemplateType, true);
			XElement xelement = new XElement("Project");
			XmlUtils.SetAttributeValue(xelement, "Guid", databaseObject.Guid);
			XmlUtils.SetAttributeValue(xelement, "Name", "GameProject");
			XmlUtils.SetAttributeValue(xelement, "Version", VersionsManager.SerializationVersion);
			XElement xelement2 = new XElement("Subsystems");
			xelement.Add(xelement2);
			XElement xelement3 = new XElement("Values");
			XmlUtils.SetAttributeValue(xelement3, "Name", "GameInfo");
			valuesDictionary.Save(xelement3);
			xelement2.Add(xelement3);
			XElement xelement4 = new XElement("Values");
			XmlUtils.SetAttributeValue(xelement4, "Name", "Players");
			valuesDictionary2.Save(xelement4);
			xelement2.Add(xelement4);
			using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				unusedWorldDirectoryName,
				"Project.xml"
			}), OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream, null, true);
			}
			return WorldsManager.GetWorldInfo(unusedWorldDirectoryName);
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x0002828C File Offset: 0x0002648C
		public static void ChangeWorld(string directoryName, WorldSettings worldSettings)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			if (!Storage.FileExists(path))
			{
				return;
			}
			XElement xelement = null;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			XElement gameInfoNode = WorldsManager.GetGameInfoNode(xelement);
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			valuesDictionary.ApplyOverrides(gameInfoNode);
			GameMode value = valuesDictionary.GetValue<GameMode>("GameMode");
			worldSettings.Save(valuesDictionary, true);
			gameInfoNode.RemoveNodes();
			valuesDictionary.Save(gameInfoNode);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
			if (worldSettings.GameMode != value)
			{
				if (worldSettings.GameMode == GameMode.Adventure)
				{
					WorldsManager.TakeWorldSnapshot(directoryName, "AdventureRestart");
					return;
				}
				WorldsManager.DeleteWorldSnapshot(directoryName, "AdventureRestart");
			}
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0002837C File Offset: 0x0002657C
		public static string GetUnusedWorldDirectoryName()
		{
			string text = Storage.CombinePaths(new string[]
			{
				WorldsManager.WorldsDirectoryName,
				"World"
			});
			for (int i = 0; i < 1000; i++)
			{
				string str = Storage.CombinePaths(new string[]
				{
					Storage.GetDirectoryName(text),
					Storage.GetFileNameWithoutExtension(text)
				});
				string extension = Storage.GetExtension(text);
				string text2 = str + ((i > 0) ? i.ToString() : string.Empty) + extension;
				if (!Storage.DirectoryExists(text2) && !Storage.FileExists(text2))
				{
					return text2;
				}
			}
			throw new InvalidOperationException("Out of filenames for root \"" + text + "\".");
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x0002841C File Offset: 0x0002661C
		public static void RecursiveEnumerateDirectory(string directoryName, List<string> files, List<string> directories, Func<string, bool> filesFilter)
		{
			try
			{
				foreach (string text in Storage.ListDirectoryNames(directoryName))
				{
					string text2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						text
					});
					WorldsManager.RecursiveEnumerateDirectory(text2, files, directories, filesFilter);
					if (directories != null)
					{
						directories.Add(text2);
					}
				}
				if (files != null)
				{
					foreach (string text3 in Storage.ListFileNames(directoryName))
					{
						string text4 = Storage.CombinePaths(new string[]
						{
							directoryName,
							text3
						});
						if (filesFilter == null || filesFilter(text4))
						{
							files.Add(text4);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error enumerating files/directories. Reason: " + ex.Message);
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00028514 File Offset: 0x00026714
		public static XElement GetGameInfoNode(XElement projectNode)
		{
			XElement xelement = (from n in projectNode.Element("Subsystems").Elements("Values")
			where XmlUtils.GetAttributeValue<string>(n, "Name", string.Empty) == "GameInfo"
			select n).FirstOrDefault<XElement>();
			if (xelement != null)
			{
				return xelement;
			}
			throw new InvalidOperationException("GameInfo node not found in project.");
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0002857C File Offset: 0x0002677C
		public static XElement GetPlayersNode(XElement projectNode)
		{
			XElement xelement = (from n in projectNode.Element("Subsystems").Elements("Values")
			where XmlUtils.GetAttributeValue<string>(n, "Name", string.Empty) == "Players"
			select n).FirstOrDefault<XElement>();
			if (xelement != null)
			{
				return xelement;
			}
			throw new InvalidOperationException("Players node not found in project.");
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x000285E4 File Offset: 0x000267E4
		public static void PackWorld(string directoryName, Stream targetStream, Func<string, bool> filter, bool embedExternalContent)
		{
			WorldInfo worldInfo = WorldsManager.GetWorldInfo(directoryName);
			if (worldInfo == null)
			{
				throw new InvalidOperationException("Directory does not contain a world.");
			}
			List<string> list = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, null, filter);
			using (ZipArchive zipArchive = ZipArchive.Create(targetStream, true))
			{
				foreach (string path in list)
				{
					using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
					{
						string fileName = Storage.GetFileName(path);
						zipArchive.AddStream(fileName, stream);
					}
				}
				if (embedExternalContent)
				{
					if (!BlocksTexturesManager.IsBuiltIn(worldInfo.WorldSettings.BlocksTextureName))
					{
						try
						{
							using (Stream stream2 = Storage.OpenFile(BlocksTexturesManager.GetFileName(worldInfo.WorldSettings.BlocksTextureName), OpenFileMode.Read))
							{
								string filenameInZip = Storage.CombinePaths(new string[]
								{
									"EmbeddedContent",
									Storage.GetFileNameWithoutExtension(worldInfo.WorldSettings.BlocksTextureName) + ".scbtex"
								});
								zipArchive.AddStream(filenameInZip, stream2);
							}
						}
						catch (Exception ex)
						{
							Log.Warning("Failed to embed blocks texture \"" + worldInfo.WorldSettings.BlocksTextureName + "\". Reason: " + ex.Message);
						}
					}
					foreach (PlayerInfo playerInfo in worldInfo.PlayerInfos)
					{
						if (!CharacterSkinsManager.IsBuiltIn(playerInfo.CharacterSkinName))
						{
							try
							{
								using (Stream stream3 = Storage.OpenFile(CharacterSkinsManager.GetFileName(playerInfo.CharacterSkinName), OpenFileMode.Read))
								{
									string filenameInZip2 = Storage.CombinePaths(new string[]
									{
										"EmbeddedContent",
										Storage.GetFileNameWithoutExtension(playerInfo.CharacterSkinName) + ".scskin"
									});
									zipArchive.AddStream(filenameInZip2, stream3);
								}
							}
							catch (Exception ex2)
							{
								Log.Warning("Failed to embed character skin \"" + playerInfo.CharacterSkinName + "\". Reason: " + ex2.Message);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x000288B8 File Offset: 0x00026AB8
		public static void UnpackWorld(string directoryName, Stream sourceStream, bool importEmbeddedExternalContent)
		{
			if (!Storage.DirectoryExists(directoryName))
			{
				throw new InvalidOperationException("Cannot import world into \"" + directoryName + "\" because this directory does not exist.");
			}
			using (ZipArchive zipArchive = ZipArchive.Open(sourceStream, true))
			{
				foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.ReadCentralDir())
				{
					string text = zipArchiveEntry.FilenameInZip.Replace('\\', '/');
					string extension = Storage.GetExtension(text);
					if (text.StartsWith("EmbeddedContent"))
					{
						try
						{
							if (importEmbeddedExternalContent)
							{
								MemoryStream memoryStream = new MemoryStream();
								zipArchive.ExtractFile(zipArchiveEntry, memoryStream);
								memoryStream.Position = 0L;
								ExternalContentType type = ExternalContentManager.ExtensionToType(extension);
								ExternalContentManager.ImportExternalContentSync(memoryStream, type, Storage.GetFileNameWithoutExtension(text));
							}
							continue;
						}
						catch (Exception ex)
						{
							Log.Warning("Failed to import embedded content \"" + text + "\". Reason: " + ex.Message);
							continue;
						}
					}
					using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileName(text)
					}), OpenFileMode.Create))
					{
						zipArchive.ExtractFile(zipArchiveEntry, stream);
					}
				}
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00028A14 File Offset: 0x00026C14
		public static void DeleteWorldContents(string directoryName, Func<string, bool> filter)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			WorldsManager.RecursiveEnumerateDirectory(directoryName, list, list2, filter);
			foreach (string path in list)
			{
				Storage.DeleteFile(path);
			}
			foreach (string path2 in list2)
			{
				Storage.DeleteDirectory(path2);
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00028AB0 File Offset: 0x00026CB0
		public static string MakeSnapshotFilename(string directoryName, string snapshotName)
		{
			return Storage.CombinePaths(new string[]
			{
				directoryName,
				snapshotName + ".snapshot"
			});
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00028AD0 File Offset: 0x00026CD0
		public static bool TestXmlFile(string fileName, string rootNodeName)
		{
			bool result;
			try
			{
				if (Storage.FileExists(fileName))
				{
					using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
					{
						XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, false);
						return xelement != null && xelement.Name == rootNodeName;
					}
				}
				result = false;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04000326 RID: 806
		public static List<WorldInfo> m_worldInfos = new List<WorldInfo>();

		// Token: 0x04000327 RID: 807
		public static ReadOnlyList<string> m_newWorldNames;

		// Token: 0x04000328 RID: 808
		public static string WorldsDirectoryName = "app:/Worlds";

		// Token: 0x04000329 RID: 809
		public const int MaxAllowedWorlds = 30;

		// Token: 0x0400032B RID: 811
		public static bool Loaded = false;
	}
}
