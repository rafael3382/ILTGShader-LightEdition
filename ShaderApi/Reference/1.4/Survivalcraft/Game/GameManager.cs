using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000142 RID: 322
	public static class GameManager
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600064F RID: 1615 RVA: 0x00023D80 File Offset: 0x00021F80
		public static Project Project
		{
			get
			{
				return GameManager.m_project;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x00023D87 File Offset: 0x00021F87
		public static WorldInfo WorldInfo
		{
			get
			{
				return GameManager.m_worldInfo;
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00023D90 File Offset: 0x00021F90
		public static void LoadProject(WorldInfo worldInfo, ContainerWidget gamesWidget)
		{
			GameManager.DisposeProject();
			WorldsManager.RepairWorldIfNeeded(worldInfo.DirectoryName);
			VersionsManager.UpgradeWorld(worldInfo.DirectoryName);
			using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				worldInfo.DirectoryName,
				"Project.xml"
			}), OpenFileMode.Read))
			{
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				ValuesDictionary valuesDictionary2 = new ValuesDictionary();
				valuesDictionary.SetValue<ValuesDictionary>("GameInfo", valuesDictionary2);
				valuesDictionary2.SetValue<string>("WorldDirectoryName", worldInfo.DirectoryName);
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary.SetValue<ValuesDictionary>("Views", valuesDictionary3);
				valuesDictionary3.SetValue<ContainerWidget>("GamesWidget", gamesWidget);
				XElement projectNode = XmlUtils.LoadXmlFromStream(stream, null, true);
				ModsManager.HookAction("ProjectXmlLoad", delegate(ModLoader loader)
				{
					loader.ProjectXmlLoad(projectNode);
					return false;
				});
				Project.EntityAdded += delegate(object s, EntityAddRemoveEventArgs arg)
				{
					ModsManager.HookAction("OnEntityAdd", delegate(ModLoader loader)
					{
						loader.OnEntityAdd(arg.Entity);
						return false;
					});
				};
				Project.EntityRemoved += delegate(object s, EntityAddRemoveEventArgs arg)
				{
					ModsManager.HookAction("OnEntityRemove", delegate(ModLoader loader)
					{
						loader.OnEntityRemove(arg.Entity);
						return false;
					});
				};
				ProjectData projectData = new ProjectData(DatabaseManager.GameDatabase, projectNode, valuesDictionary, true);
				GameManager.m_project = new Project(DatabaseManager.GameDatabase, projectData);
				GameManager.m_subsystemUpdate = GameManager.m_project.FindSubsystem<SubsystemUpdate>(true);
				ModsManager.HookAction("OnProjectLoaded", delegate(ModLoader loader)
				{
					loader.OnProjectLoaded(GameManager.m_project);
					return false;
				});
			}
			GameManager.m_worldInfo = worldInfo;
			Log.Information("Loaded world, GameMode={0}, StartingPosition={1}, WorldName={2}, VisibilityRange={3}, Resolution={4}", new object[]
			{
				worldInfo.WorldSettings.GameMode,
				worldInfo.WorldSettings.StartingPositionMode,
				worldInfo.WorldSettings.Name,
				SettingsManager.VisibilityRange.ToString(),
				SettingsManager.ResolutionMode.ToString()
			});
			GC.Collect();
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00023F94 File Offset: 0x00022194
		public static void SaveProject(bool waitForCompletion, bool showErrorDialog)
		{
			if (GameManager.m_project != null)
			{
				double realTime = Time.RealTime;
				ProjectData projectData = GameManager.m_project.Save();
				GameManager.m_saveCompleted.WaitOne();
				GameManager.m_saveCompleted.Reset();
				SubsystemGameInfo subsystemGameInfo = GameManager.m_project.FindSubsystem<SubsystemGameInfo>(true);
				string projectFileName = Storage.CombinePaths(new string[]
				{
					subsystemGameInfo.DirectoryName,
					"Project.xml"
				});
				Exception e = null;
				Task.Run(delegate()
				{
					try
					{
						WorldsManager.MakeQuickWorldBackup(subsystemGameInfo.DirectoryName);
						XElement xElement = new XElement("Project");
						ModsManager.HookAction("ProjectXmlSave", delegate(ModLoader loader)
						{
							loader.ProjectXmlSave(xElement);
							return false;
						});
						projectData.Save(xElement);
						XmlUtils.SetAttributeValue(xElement, "Version", VersionsManager.SerializationVersion);
						Storage.CreateDirectory(subsystemGameInfo.DirectoryName);
						using (Stream stream = Storage.OpenFile(projectFileName, OpenFileMode.Create))
						{
							XmlUtils.SaveXmlToStream(xElement, stream, null, true);
						}
					}
					catch (Exception e)
					{
						e = e;
						if (showErrorDialog)
						{
							Dispatcher.Dispatch(delegate
							{
								DialogsManager.ShowDialog(null, new MessageDialog("Error saving game", e.Message, "OK", null, null));
							}, false);
						}
					}
					finally
					{
						GameManager.m_saveCompleted.Set();
					}
				});
				if (waitForCompletion)
				{
					GameManager.m_saveCompleted.WaitOne();
				}
				double realTime2 = Time.RealTime;
				Log.Verbose(string.Format("Saved project, {0}ms", MathUtils.Round((realTime2 - realTime) * 1000.0)));
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0002406F File Offset: 0x0002226F
		public static void UpdateProject()
		{
			if (GameManager.SyncDispatcher.Count > 0 && GameManager.SyncDispatcher[0]())
			{
				GameManager.SyncDispatcher.RemoveAt(0);
			}
			if (GameManager.m_project != null)
			{
				GameManager.m_subsystemUpdate.Update();
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x000240AC File Offset: 0x000222AC
		public static void DisposeProject()
		{
			if (GameManager.m_project != null)
			{
				GameManager.m_project.Dispose();
				GameManager.m_project = null;
				GameManager.m_subsystemUpdate = null;
				GameManager.m_worldInfo = null;
				ModsManager.HookAction("OnProjectDisposed", delegate(ModLoader loader)
				{
					loader.OnProjectDisposed();
					return false;
				});
				GC.Collect();
			}
		}

		// Token: 0x040002B8 RID: 696
		public static WorldInfo m_worldInfo;

		// Token: 0x040002B9 RID: 697
		public static Project m_project;

		// Token: 0x040002BA RID: 698
		public static SubsystemUpdate m_subsystemUpdate;

		// Token: 0x040002BB RID: 699
		public static ManualResetEvent m_saveCompleted = new ManualResetEvent(true);

		// Token: 0x040002BC RID: 700
		public static List<Func<bool>> SyncDispatcher = new List<Func<bool>>();
	}
}
