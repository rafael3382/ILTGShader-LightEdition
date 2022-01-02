using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Engine;

namespace Game
{
	// Token: 0x02000140 RID: 320
	public static class ExternalContentManager
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x000233D4 File Offset: 0x000215D4
		public static IExternalContentProvider DefaultProvider
		{
			get
			{
				if (ExternalContentManager.Providers.Count <= 0)
				{
					return null;
				}
				return ExternalContentManager.Providers[0];
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x00023401 File Offset: 0x00021601
		public static ReadOnlyList<IExternalContentProvider> Providers
		{
			get
			{
				return new ReadOnlyList<IExternalContentProvider>(ExternalContentManager.m_providers);
			}
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00023410 File Offset: 0x00021610
		public static void Initialize()
		{
			ExternalContentManager.m_providers = new List<IExternalContentProvider>();
			ExternalContentManager.m_providers.Add(new DiskExternalContentProvider());
			ExternalContentManager.m_providers.Add(new SPMBoxExternalContentProvider());
			ExternalContentManager.m_providers.Add(new DropboxExternalContentProvider());
			ExternalContentManager.m_providers.Add(new TransferShExternalContentProvider());
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00023464 File Offset: 0x00021664
		public static ExternalContentType ExtensionToType(string extension)
		{
			extension = extension.ToLower();
			Func<string, bool> <>9__0;
			foreach (object obj in Enum.GetValues(typeof(ExternalContentType)))
			{
				ExternalContentType externalContentType = (ExternalContentType)obj;
				IEnumerable<string> entryTypeExtensions = ExternalContentManager.GetEntryTypeExtensions(externalContentType);
				Func<string, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((string e) => e == extension));
				}
				if (entryTypeExtensions.FirstOrDefault(predicate) != null)
				{
					return externalContentType;
				}
			}
			return ExternalContentType.Unknown;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00023518 File Offset: 0x00021718
		public static IEnumerable<string> GetEntryTypeExtensions(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.World:
				yield return ".scworld";
				break;
			case ExternalContentType.BlocksTexture:
				yield return ".scbtex";
				yield return ".png";
				break;
			case ExternalContentType.CharacterSkin:
				yield return ".scskin";
				break;
			case ExternalContentType.FurniturePack:
				yield return ".scfpack";
				break;
			}
			yield break;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00023528 File Offset: 0x00021728
		public static Subtexture GetEntryTypeIcon(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.Directory:
				return ContentManager.Get<Subtexture>("Textures/Atlas/FolderIcon", null);
			case ExternalContentType.World:
				return ContentManager.Get<Subtexture>("Textures/Atlas/WorldIcon", null);
			case ExternalContentType.BlocksTexture:
				return ContentManager.Get<Subtexture>("Textures/Atlas/TexturePackIcon", null);
			case ExternalContentType.CharacterSkin:
				return ContentManager.Get<Subtexture>("Textures/Atlas/CharacterSkinIcon", null);
			case ExternalContentType.FurniturePack:
				return ContentManager.Get<Subtexture>("Textures/Atlas/FurnitureIcon", null);
			default:
				return ContentManager.Get<Subtexture>("Textures/Atlas/QuestionMarkIcon", null);
			}
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0002359C File Offset: 0x0002179C
		public static string GetEntryTypeDescription(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.Directory:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"Directory"
				});
			case ExternalContentType.World:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"World"
				});
			case ExternalContentType.BlocksTexture:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"Blocks Texture"
				});
			case ExternalContentType.CharacterSkin:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"Character Skin"
				});
			case ExternalContentType.FurniturePack:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"Furniture Pack"
				});
			case ExternalContentType.Mod:
				return LanguageControl.Get(new string[]
				{
					ExternalContentManager.fName,
					"Mod"
				});
			default:
				return string.Empty;
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0002367B File Offset: 0x0002187B
		public static bool IsEntryTypeDownloadSupported(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.World:
				return true;
			case ExternalContentType.BlocksTexture:
				return true;
			case ExternalContentType.CharacterSkin:
				return true;
			case ExternalContentType.FurniturePack:
				return true;
			case ExternalContentType.Mod:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x000236A6 File Offset: 0x000218A6
		public static bool DoesEntryTypeRequireName(ExternalContentType type)
		{
			switch (type)
			{
			case ExternalContentType.BlocksTexture:
				return true;
			case ExternalContentType.CharacterSkin:
				return true;
			case ExternalContentType.FurniturePack:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x000236C8 File Offset: 0x000218C8
		public static Exception VerifyExternalContentName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 1));
			}
			if (name.Length > 50)
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 2));
			}
			if (name[0] == ' ' || name[name.Length - 1] == ' ')
			{
				return new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 3));
			}
			return null;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00023738 File Offset: 0x00021938
		public static void DeleteExternalContent(ExternalContentType type, string name)
		{
			switch (type)
			{
			case ExternalContentType.World:
				WorldsManager.DeleteWorld(name);
				return;
			case ExternalContentType.BlocksTexture:
				BlocksTexturesManager.DeleteBlocksTexture(name);
				return;
			case ExternalContentType.CharacterSkin:
				CharacterSkinsManager.DeleteCharacterSkin(name);
				return;
			case ExternalContentType.FurniturePack:
				FurniturePacksManager.DeleteFurniturePack(name);
				return;
			default:
				throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 4));
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0002378B File Offset: 0x0002198B
		public static void ImportExternalContent(Stream stream, ExternalContentType type, string name, Action<string> success, Action<Exception> failure)
		{
			Task.Run(delegate()
			{
				try
				{
					success(ExternalContentManager.ImportExternalContentSync(stream, type, name));
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			});
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x000237C8 File Offset: 0x000219C8
		public static string ImportExternalContentSync(Stream stream, ExternalContentType type, string name)
		{
			switch (type)
			{
			case ExternalContentType.World:
				return WorldsManager.ImportWorld(stream);
			case ExternalContentType.BlocksTexture:
				return BlocksTexturesManager.ImportBlocksTexture(name, stream);
			case ExternalContentType.CharacterSkin:
				return CharacterSkinsManager.ImportCharacterSkin(name, stream);
			case ExternalContentType.FurniturePack:
				return FurniturePacksManager.ImportFurniturePack(name, stream);
			case ExternalContentType.Mod:
				return ModsManager.ImportMod(name, stream);
			default:
				throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 4));
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0002382C File Offset: 0x00021A2C
		public static void ShowLoginUiIfNeeded(IExternalContentProvider provider, bool showWarningDialog, Action handler)
		{
			if (provider.RequiresLogin && !provider.IsLoggedIn)
			{
				Action loginAction = delegate()
				{
					CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentManager.fName, 5), true);
					DialogsManager.ShowDialog(null, busyDialog);
					provider.Login(busyDialog.Progress, delegate
					{
						DialogsManager.HideDialog(busyDialog);
						Action handler3 = handler;
						if (handler3 == null)
						{
							return;
						}
						handler3();
					}, delegate(Exception error)
					{
						DialogsManager.HideDialog(busyDialog);
						if (error != null)
						{
							DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
						}
					});
				};
				if (showWarningDialog)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(ExternalContentManager.fName, 6), string.Format(LanguageControl.Get(ExternalContentManager.fName, 7), provider.DisplayName), LanguageControl.Get(ExternalContentManager.fName, 8), LanguageControl.Cancel, delegate(MessageDialogButton b)
					{
						if (b == MessageDialogButton.Button1)
						{
							loginAction();
						}
					}));
					return;
				}
				loginAction();
				return;
			}
			else
			{
				Action handler2 = handler;
				if (handler2 == null)
				{
					return;
				}
				handler2();
				return;
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00023904 File Offset: 0x00021B04
		public static void ShowUploadUi(ExternalContentType type, string name)
		{
			DialogsManager.ShowDialog(null, new SelectExternalContentProviderDialog(LanguageControl.Get(ExternalContentManager.fName, 9), false, delegate(IExternalContentProvider provider)
			{
				try
				{
					if (provider != null)
					{
						ExternalContentManager.ShowLoginUiIfNeeded(provider, true, delegate
						{
							CancellableBusyDialog busyDialog = new CancellableBusyDialog(LanguageControl.Get(ExternalContentManager.fName, 10), false);
							DialogsManager.ShowDialog(null, busyDialog);
							Task.Run(delegate()
							{
								bool needsDelete = false;
								string sourcePath = null;
								Stream stream = null;
								Action cleanup = delegate()
								{
									Utilities.Dispose<Stream>(ref stream);
									if (needsDelete && sourcePath != null)
									{
										try
										{
											Storage.DeleteFile(sourcePath);
										}
										catch
										{
										}
									}
								};
								try
								{
									string path;
									if (type == ExternalContentType.BlocksTexture)
									{
										sourcePath = BlocksTexturesManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else if (type == ExternalContentType.CharacterSkin)
									{
										sourcePath = CharacterSkinsManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else if (type == ExternalContentType.FurniturePack)
									{
										sourcePath = FurniturePacksManager.GetFileName(name);
										if (sourcePath == null)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 11));
										}
										path = Storage.GetFileName(sourcePath);
									}
									else
									{
										if (type != ExternalContentType.World)
										{
											throw new InvalidOperationException(LanguageControl.Get(ExternalContentManager.fName, 12));
										}
										busyDialog.LargeMessage = LanguageControl.Get(ExternalContentManager.fName, 13);
										sourcePath = Storage.CombinePaths(new string[]
										{
											"app:",
											"WorldUpload.tmp"
										});
										needsDelete = true;
										path = WorldsManager.GetWorldInfo(name).WorldSettings.Name + ".scworld";
										using (Stream stream2 = Storage.OpenFile(sourcePath, OpenFileMode.Create))
										{
											WorldsManager.ExportWorld(name, stream2);
										}
									}
									busyDialog.LargeMessage = LanguageControl.Get(ExternalContentManager.fName, 14);
									stream = Storage.OpenFile(sourcePath, OpenFileMode.Read);
									provider.Upload(path, stream, busyDialog.Progress, delegate(string link)
									{
										long length = stream.Length;
										cleanup();
										DialogsManager.HideDialog(busyDialog);
										if (string.IsNullOrEmpty(link))
										{
											DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Success, string.Format(LanguageControl.Get(ExternalContentManager.fName, 15), DataSizeFormatter.Format(length)), LanguageControl.Ok, null, null));
											return;
										}
										DialogsManager.ShowDialog(null, new ExternalContentLinkDialog(link));
									}, delegate(Exception error)
									{
										cleanup();
										DialogsManager.HideDialog(busyDialog);
										DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, error.Message, LanguageControl.Ok, null, null));
									});
								}
								catch (Exception ex2)
								{
									cleanup();
									DialogsManager.HideDialog(busyDialog);
									DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, ex2.Message, LanguageControl.Ok, null, null));
								}
							});
						});
					}
				}
				catch (Exception ex)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Error, ex.Message, LanguageControl.Ok, null, null));
				}
			}));
		}

		// Token: 0x040002B4 RID: 692
		public static List<IExternalContentProvider> m_providers;

		// Token: 0x040002B5 RID: 693
		public static string fName = "ExternalContentManager";
	}
}
