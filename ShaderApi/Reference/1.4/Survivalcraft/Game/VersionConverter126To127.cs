using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034A RID: 842
	public class VersionConverter126To127 : VersionConverter
	{
		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060018C1 RID: 6337 RVA: 0x000C1990 File Offset: 0x000BFB90
		public override string SourceVersion
		{
			get
			{
				return "1.26";
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x000C1997 File Offset: 0x000BFB97
		public override string TargetVersion
		{
			get
			{
				return "1.27";
			}
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x000C199E File Offset: 0x000BFB9E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ConvertTypesToEngine(projectNode);
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x000C19B8 File Offset: 0x000BFBB8
		public override void ConvertWorld(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x000C1A3C File Offset: 0x000BFC3C
		public static void MigrateDataFromIsolatedStorageWithDialog()
		{
			try
			{
				if (Storage.DirectoryExists("app:/.config/.isolated-storage"))
				{
					Log.Information("1.26 data found, starting migration to 1.27.");
					BusyDialog dialog = new BusyDialog("Please wait", "Migrating 1.26 data to 1.27 format...");
					DialogsManager.ShowDialog(null, dialog);
					Task.Run(delegate()
					{
						string largeMessage = string.Empty;
						string smallMessage = string.Empty;
						try
						{
							int num = VersionConverter126To127.MigrateFolder("app:/.config/.isolated-storage", "data:");
							largeMessage = "Migration Successful";
							smallMessage = string.Format("{0} file(s) were migrated from 1.26 to 1.27.", num);
						}
						catch (Exception ex2)
						{
							largeMessage = "Migration Failed";
							smallMessage = ex2.Message;
							Log.Error("Migration to 1.27 failed, reason: {0}", new object[]
							{
								ex2.Message
							});
						}
						DialogsManager.HideDialog(dialog);
						DialogsManager.ShowDialog(null, new MessageDialog(largeMessage, smallMessage, "OK", null, null));
						Dispatcher.Dispatch(delegate
						{
							SettingsManager.LoadSettings();
						}, false);
					});
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to migrate data. Reason: {0}", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x000C1AC8 File Offset: 0x000BFCC8
		public void ConvertTypesToEngine(XElement node)
		{
			foreach (XElement xelement in node.DescendantsAndSelf("Value"))
			{
				XAttribute xattribute = xelement.Attribute("Type");
				if (xattribute != null)
				{
					if (xattribute.Value == "Microsoft.Xna.Framework.Vector2")
					{
						xattribute.Value = "Engine.Vector2";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Vector3")
					{
						xattribute.Value = "Engine.Vector3";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Vector4")
					{
						xattribute.Value = "Engine.Vector4";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Quaternion")
					{
						xattribute.Value = "Engine.Quaternion";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Matrix")
					{
						xattribute.Value = "Engine.Matrix";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Point")
					{
						xattribute.Value = "Engine.Point2";
					}
					else if (xattribute.Value == "Microsoft.Xna.Framework.Color")
					{
						xattribute.Value = "Engine.Color";
					}
					else if (xattribute.Value == "Game.Point3")
					{
						xattribute.Value = "Engine.Point3";
					}
				}
			}
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x000C1C40 File Offset: 0x000BFE40
		public static int MigrateFolder(string sourceFolderName, string targetFolderName)
		{
			int num = 0;
			Storage.CreateDirectory(targetFolderName);
			foreach (string text in Storage.ListDirectoryNames(sourceFolderName))
			{
				num += VersionConverter126To127.MigrateFolder(Storage.CombinePaths(new string[]
				{
					sourceFolderName,
					text
				}), Storage.CombinePaths(new string[]
				{
					targetFolderName,
					text
				}));
			}
			foreach (string text2 in Storage.ListFileNames(sourceFolderName))
			{
				VersionConverter126To127.MigrateFile(Storage.CombinePaths(new string[]
				{
					sourceFolderName,
					text2
				}), targetFolderName);
				num++;
			}
			Storage.DeleteDirectory(sourceFolderName);
			Log.Information("Migrated {0}", new object[]
			{
				sourceFolderName
			});
			return num;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000C1D2C File Offset: 0x000BFF2C
		public static void MigrateFile(string sourceFileName, string targetFolderName)
		{
			Storage.CopyFile(sourceFileName, Storage.CombinePaths(new string[]
			{
				targetFolderName,
				Storage.GetFileName(sourceFileName)
			}));
			Storage.DeleteFile(sourceFileName);
			Log.Information("Migrated {0}", new object[]
			{
				sourceFileName
			});
		}
	}
}
