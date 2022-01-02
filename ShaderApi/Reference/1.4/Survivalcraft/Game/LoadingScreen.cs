using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000165 RID: 357
	public class LoadingScreen : Screen
	{
		// Token: 0x060007E7 RID: 2023 RVA: 0x0002E760 File Offset: 0x0002C960
		static LoadingScreen()
		{
			LoadingScreen.LogList.ItemWidgetFactory = delegate(object obj)
			{
				LoadingScreen.LogItem logItem = obj as LoadingScreen.LogItem;
				CanvasWidget canvasWidget = new CanvasWidget();
				canvasWidget.Size = new Vector2((float)Display.Viewport.Width, 40f);
				canvasWidget.Margin = new Vector2(0f, 2f);
				canvasWidget.HorizontalAlignment = WidgetAlignment.Near;
				FontTextWidget widget = new FontTextWidget
				{
					FontScale = 0.6f,
					Text = logItem.Message,
					Color = LoadingScreen.GetColor(logItem.LogType),
					VerticalAlignment = WidgetAlignment.Center,
					HorizontalAlignment = WidgetAlignment.Near
				};
				canvasWidget.Children.Add(widget);
				canvasWidget.IsVisible = SettingsManager.DisplayLog;
				LoadingScreen.LogList.IsEnabled = SettingsManager.DisplayLog;
				return canvasWidget;
			};
			LoadingScreen.LogList.ItemSize = 30f;
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0002E7AE File Offset: 0x0002C9AE
		public static Color GetColor(LoadingScreen.LogType type)
		{
			switch (type)
			{
			case LoadingScreen.LogType.Info:
				return Color.White;
			case LoadingScreen.LogType.Warning:
				return Color.Yellow;
			case LoadingScreen.LogType.Error:
				return Color.Red;
			case LoadingScreen.LogType.Advice:
				return Color.Cyan;
			default:
				return Color.White;
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x0002E7E8 File Offset: 0x0002C9E8
		public LoadingScreen()
		{
			this.Canvas.Size = new Vector2(float.PositiveInfinity);
			this.Canvas.AddChildren(this.Background);
			this.Canvas.AddChildren(LoadingScreen.LogList);
			base.AddChildren(this.Canvas);
			LoadingScreen.Info("Initilizing Mods Manager. Api Version: 1.40");
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0002E8A0 File Offset: 0x0002CAA0
		public void ContentLoaded()
		{
			if (SettingsManager.DisplayLog)
			{
				return;
			}
			base.ClearChildren();
			RectangleWidget rectangleWidget = new RectangleWidget
			{
				FillColor = Color.White,
				OutlineColor = Color.Transparent,
				Size = new Vector2(256f),
				VerticalAlignment = WidgetAlignment.Center,
				HorizontalAlignment = WidgetAlignment.Center
			};
			rectangleWidget.Subtexture = ContentManager.Get<Subtexture>("Textures/Gui/CandyRufusLogo", null);
			RectangleWidget rectangleWidget2 = new RectangleWidget
			{
				FillColor = Color.White,
				OutlineColor = Color.Transparent,
				Size = new Vector2(80f),
				VerticalAlignment = WidgetAlignment.Far,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(10f)
			};
			rectangleWidget2.Subtexture = ContentManager.Get<Subtexture>("Textures/Gui/EngineLogo", null);
			BusyBarWidget widget = new BusyBarWidget
			{
				VerticalAlignment = WidgetAlignment.Far,
				HorizontalAlignment = WidgetAlignment.Center,
				Margin = new Vector2(0f, 40f)
			};
			this.Canvas.AddChildren(this.Background);
			this.Canvas.AddChildren(rectangleWidget);
			this.Canvas.AddChildren(rectangleWidget2);
			this.Canvas.AddChildren(widget);
			this.Canvas.AddChildren(LoadingScreen.LogList);
			base.AddChildren(this.Canvas);
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0002E9DB File Offset: 0x0002CBDB
		public static void Error(string mesg)
		{
			LoadingScreen.Add(LoadingScreen.LogType.Error, "[Error]" + mesg);
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0002E9EE File Offset: 0x0002CBEE
		public static void Info(string mesg)
		{
			LoadingScreen.Add(LoadingScreen.LogType.Info, "[Info]" + mesg);
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0002EA01 File Offset: 0x0002CC01
		public static void Warning(string mesg)
		{
			LoadingScreen.Add(LoadingScreen.LogType.Warning, "[Warning]" + mesg);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0002EA14 File Offset: 0x0002CC14
		public static void Advice(string mesg)
		{
			LoadingScreen.Add(LoadingScreen.LogType.Advice, "[Advice]" + mesg);
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0002EA27 File Offset: 0x0002CC27
		public static void Add(LoadingScreen.LogType type, string mesg)
		{
			Dispatcher.Dispatch(delegate
			{
				LoadingScreen.LogItem item = new LoadingScreen.LogItem(type, mesg);
				LoadingScreen.LogList.AddItem(item);
				switch (type)
				{
				case LoadingScreen.LogType.Info:
				case LoadingScreen.LogType.Advice:
					Log.Information(mesg);
					break;
				case LoadingScreen.LogType.Warning:
					Log.Warning(mesg);
					break;
				case LoadingScreen.LogType.Error:
					Log.Error(mesg);
					break;
				}
				LoadingScreen.LogList.ScrollToItem(item);
			}, false);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0002EA50 File Offset: 0x0002CC50
		private void InitActions()
		{
			this.AddLoadAction(delegate
			{
				ContentManager.Initialize();
				ModsManager.Initialize();
			});
			this.AddLoadAction(new Action(this.ContentLoaded));
			this.AddLoadAction(delegate
			{
				ModsManager.ModListAllDo(delegate(ModEntity modEntity)
				{
					modEntity.CheckDependencies();
				});
			});
			this.AddLoadAction(delegate
			{
				ReadOnlyList<ContentInfo> readOnlyList = ContentManager.List("Lang");
				LanguageControl.LanguageTypes.Clear();
				foreach (ContentInfo contentInfo in readOnlyList)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(contentInfo.Filename);
					if (!LanguageControl.LanguageTypes.Contains(fileNameWithoutExtension))
					{
						LanguageControl.LanguageTypes.Add(fileNameWithoutExtension);
					}
				}
				if (ModsManager.Configs.ContainsKey("Language"))
				{
					LanguageControl.Initialize(ModsManager.Configs["Language"]);
				}
				else
				{
					LanguageControl.Initialize("zh-CN");
				}
				ModsManager.ModListAllDo(delegate(ModEntity modEntity)
				{
					modEntity.LoadLauguage();
				});
			});
			this.AddLoadAction(delegate
			{
				ModsManager.ModListAllDo(delegate(ModEntity modEntity)
				{
					modEntity.LoadDll();
				});
			});
			this.AddLoadAction(delegate
			{
				LoadingScreen.Info("执行初始化任务");
				List<Action> actions = new List<Action>();
				ModsManager.ModListAllDo(delegate(ModEntity modEntity)
				{
					ModLoader loader = modEntity.Loader;
					if (loader == null)
					{
						return;
					}
					loader.OnLoadingStart(actions);
				});
				foreach (Action item in actions)
				{
					this.ModLoadingActoins.Add(item);
				}
			});
			this.AddLoadAction(delegate
			{
				LoadingScreen.Info("初始化纹理地图");
				TextureAtlasManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				try
				{
					DatabaseManager.Initialize();
					ModsManager.ModListAllDo(delegate(ModEntity modEntity)
					{
						modEntity.LoadXdb(ref DatabaseManager.DatabaseNode);
					});
				}
				catch (Exception ex)
				{
					LoadingScreen.Warning(ex.Message);
				}
			});
			this.AddLoadAction(delegate
			{
				LoadingScreen.Info("读取数据库");
				try
				{
					DatabaseManager.LoadDataBaseFromXml(DatabaseManager.DatabaseNode);
				}
				catch (Exception ex)
				{
					LoadingScreen.Warning(ex.Message);
				}
			});
			this.AddLoadAction(delegate
			{
				LoadingScreen.Info("初始化方块管理器");
				BlocksManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				CraftingRecipesManager.Initialize();
			});
			this.InitScreens();
			this.AddLoadAction(delegate
			{
				BlocksTexturesManager.Initialize();
				CharacterSkinsManager.Initialize();
				CommunityContentManager.Initialize();
				ExternalContentManager.Initialize();
				FurniturePacksManager.Initialize();
				LightingManager.Initialize();
				MotdManager.Initialize();
				VersionsManager.Initialize();
				WorldsManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				LoadingScreen.Info("初始化Mod设置参数");
				if (Storage.FileExists("app:/ModSettings.xml"))
				{
					using (Stream stream = Storage.OpenFile("app:/ModSettings.xml", OpenFileMode.Read))
					{
						try
						{
							ModsManager.LoadModSettings(XElement.Load(stream));
						}
						catch (Exception ex)
						{
							LoadingScreen.Warning(ex.Message);
						}
					}
				}
			});
			this.AddLoadAction(delegate
			{
				ModsManager.ModListAllDo(delegate(ModEntity modEntity)
				{
					string str = "等待剩下的任务完成:";
					ModInfo modInfo = modEntity.modInfo;
					LoadingScreen.Info(str + ((modInfo != null) ? modInfo.PackageName : null));
					ModLoader loader = modEntity.Loader;
					if (loader == null)
					{
						return;
					}
					loader.OnLoadingFinished(this.ModLoadingActoins);
				});
			});
			this.AddLoadAction(delegate
			{
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			});
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0002EC58 File Offset: 0x0002CE58
		private void InitScreens()
		{
			this.AddLoadAction(delegate
			{
				this.AddScreen("Nag", new NagScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("MainMenu", new MainMenuScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Recipaedia", new RecipaediaScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("RecipaediaRecipes", new RecipaediaRecipesScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("RecipaediaDescription", new RecipaediaDescriptionScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Bestiary", new BestiaryScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("BestiaryDescription", new BestiaryDescriptionScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Help", new HelpScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("HelpTopic", new HelpTopicScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Settings", new SettingsScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsPerformance", new SettingsPerformanceScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsGraphics", new SettingsGraphicsScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsUi", new SettingsUiScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsCompatibility", new SettingsCompatibilityScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsAudio", new SettingsAudioScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("SettingsControls", new SettingsControlsScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Play", new PlayScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("NewWorld", new NewWorldScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("ModifyWorld", new ModifyWorldScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("WorldOptions", new WorldOptionsScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("GameLoading", new GameLoadingScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Game", new GameScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("TrialEnded", new TrialEndedScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("ExternalContent", new ExternalContentScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("CommunityContent", new CommunityContentScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Content", new ContentScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("ManageContent", new ManageContentScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("ModsManageContent", new ModsManageContentScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Players", new PlayersScreen());
			});
			this.AddLoadAction(delegate
			{
				this.AddScreen("Player", new PlayerScreen());
			});
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0002EE81 File Offset: 0x0002D081
		public void AddScreen(string name, Screen screen)
		{
			ScreensManager.AddScreen(name, screen);
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0002EE8A File Offset: 0x0002D08A
		private void AddLoadAction(Action action)
		{
			this.LoadingActoins.Add(action);
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0002EE98 File Offset: 0x0002D098
		public override void Leave()
		{
			LoadingScreen.LogList.ClearItems();
			Window.PresentationInterval = SettingsManager.PresentationInterval;
			ContentManager.Dispose("Textures/Gui/CandyRufusLogo");
			ContentManager.Dispose("Textures/Gui/EngineLogo");
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0002EEC4 File Offset: 0x0002D0C4
		public override void Enter(object[] parameters)
		{
			Window.PresentationInterval = 0;
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Screen> keyValuePair in ScreensManager.m_screens)
			{
				if (keyValuePair.Value != this)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (string key in list)
			{
				ScreensManager.m_screens.Remove(key);
			}
			this.InitActions();
			base.Enter(parameters);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0002EF84 File Offset: 0x0002D184
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Warning, "Quit?", LanguageControl.Ok, LanguageControl.No, delegate(MessageDialogButton vt)
				{
					if (vt == MessageDialogButton.Button1)
					{
						Environment.Exit(0);
						return;
					}
					DialogsManager.HideAllDialogs();
				}));
			}
			if (!ModsManager.GetAllowContinue())
			{
				return;
			}
			if (this.ModLoadingActoins.Count > 0)
			{
				try
				{
					this.ModLoadingActoins[0]();
					return;
				}
				catch (Exception ex)
				{
					LoadingScreen.Error(ex.Message);
					return;
				}
				finally
				{
					this.ModLoadingActoins.RemoveAt(0);
				}
			}
			if (this.LoadingActoins.Count > 0)
			{
				try
				{
					this.LoadingActoins[0]();
				}
				catch (Exception ex2)
				{
					LoadingScreen.Error(ex2.Message);
				}
				finally
				{
					this.LoadingActoins.RemoveAt(0);
				}
			}
		}

		// Token: 0x04000378 RID: 888
		private List<Action> LoadingActoins = new List<Action>();

		// Token: 0x04000379 RID: 889
		private List<Action> ModLoadingActoins = new List<Action>();

		// Token: 0x0400037A RID: 890
		private CanvasWidget Canvas = new CanvasWidget();

		// Token: 0x0400037B RID: 891
		private RectangleWidget Background = new RectangleWidget
		{
			FillColor = (SettingsManager.DisplayLog ? Color.Black : Color.White),
			OutlineThickness = 0f,
			DepthWriteEnabled = true
		};

		// Token: 0x0400037C RID: 892
		private static ListPanelWidget LogList = new ListPanelWidget
		{
			Direction = LayoutDirection.Vertical,
			PlayClickSound = false
		};

		// Token: 0x0200045F RID: 1119
		public enum LogType
		{
			// Token: 0x0400160B RID: 5643
			Info,
			// Token: 0x0400160C RID: 5644
			Warning,
			// Token: 0x0400160D RID: 5645
			Error,
			// Token: 0x0400160E RID: 5646
			Advice
		}

		// Token: 0x02000460 RID: 1120
		private class LogItem
		{
			// Token: 0x06002000 RID: 8192 RVA: 0x000E6C81 File Offset: 0x000E4E81
			public LogItem(LoadingScreen.LogType type, string log)
			{
				this.LogType = type;
				this.Message = log;
			}

			// Token: 0x0400160F RID: 5647
			public LoadingScreen.LogType LogType;

			// Token: 0x04001610 RID: 5648
			public string Message;
		}
	}
}
