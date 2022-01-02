using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200017D RID: 381
	public class SettingsUiScreen : Screen
	{
		// Token: 0x060008A1 RID: 2209 RVA: 0x00035514 File Offset: 0x00033714
		public SettingsUiScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsUiScreen", null);
			base.LoadContents(this, node);
			this.m_windowModeContainer = this.Children.Find<ContainerWidget>("WindowModeContainer", true);
			this.m_languageButton = this.Children.Find<ButtonWidget>("LanguageButton", true);
			this.m_displayLogButton = this.Children.Find<ButtonWidget>("DisplayLogButton", true);
			this.m_windowModeButton = this.Children.Find<ButtonWidget>("WindowModeButton", true);
			this.m_uiSizeButton = this.Children.Find<ButtonWidget>("UiSizeButton", true);
			this.m_upsideDownButton = this.Children.Find<ButtonWidget>("UpsideDownButton", true);
			this.m_hideMoveLookPadsButton = this.Children.Find<ButtonWidget>("HideMoveLookPads", true);
			this.m_showGuiInScreenshotsButton = this.Children.Find<ButtonWidget>("ShowGuiInScreenshotsButton", true);
			this.m_showLogoInScreenshotsButton = this.Children.Find<ButtonWidget>("ShowLogoInScreenshotsButton", true);
			this.m_screenshotSizeButton = this.Children.Find<ButtonWidget>("ScreenshotSizeButton", true);
			this.m_communityContentModeButton = this.Children.Find<ButtonWidget>("CommunityContentModeButton", true);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00035638 File Offset: 0x00033838
		public override void Enter(object[] parameters)
		{
			this.m_windowModeContainer.IsVisible = true;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00035648 File Offset: 0x00033848
		public override void Update()
		{
			if (this.m_windowModeButton.IsClicked)
			{
				SettingsManager.WindowMode = (SettingsManager.WindowMode + 1) % (WindowMode)EnumUtils.GetEnumValues(typeof(WindowMode)).Count;
			}
			if (this.m_languageButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(SettingsUiScreen.fName, 1), LanguageControl.Get(SettingsUiScreen.fName, 2), LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						int num = LanguageControl.LanguageTypes.IndexOf(ModsManager.Configs["Language"]) + 1;
						if (num == LanguageControl.LanguageTypes.Count)
						{
							num = 0;
						}
						LanguageControl.Initialize(LanguageControl.LanguageTypes[num]);
						foreach (ModEntity modEntity in ModsManager.ModList)
						{
							modEntity.LoadLauguage();
						}
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						foreach (KeyValuePair<string, Screen> keyValuePair in ScreensManager.m_screens)
						{
							object value = Activator.CreateInstance(keyValuePair.Value.GetType());
							dictionary.Add(keyValuePair.Key, value);
						}
						foreach (KeyValuePair<string, object> keyValuePair2 in dictionary)
						{
							ScreensManager.m_screens[keyValuePair2.Key] = (keyValuePair2.Value as Screen);
						}
						CraftingRecipesManager.Initialize();
						ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
					}
				}));
			}
			if (this.m_displayLogButton.IsClicked)
			{
				SettingsManager.DisplayLog = !SettingsManager.DisplayLog;
			}
			if (this.m_uiSizeButton.IsClicked)
			{
				SettingsManager.GuiSize = (SettingsManager.GuiSize + 1) % (GuiSize)EnumUtils.GetEnumValues(typeof(GuiSize)).Count;
			}
			if (this.m_upsideDownButton.IsClicked)
			{
				SettingsManager.UpsideDownLayout = !SettingsManager.UpsideDownLayout;
			}
			if (this.m_hideMoveLookPadsButton.IsClicked)
			{
				SettingsManager.HideMoveLookPads = !SettingsManager.HideMoveLookPads;
			}
			if (this.m_showGuiInScreenshotsButton.IsClicked)
			{
				SettingsManager.ShowGuiInScreenshots = !SettingsManager.ShowGuiInScreenshots;
			}
			if (this.m_showLogoInScreenshotsButton.IsClicked)
			{
				SettingsManager.ShowLogoInScreenshots = !SettingsManager.ShowLogoInScreenshots;
			}
			if (this.m_screenshotSizeButton.IsClicked)
			{
				SettingsManager.ScreenshotSize = (SettingsManager.ScreenshotSize + 1) % (ScreenshotSize)EnumUtils.GetEnumValues(typeof(ScreenshotSize)).Count;
			}
			if (this.m_communityContentModeButton.IsClicked)
			{
				SettingsManager.CommunityContentMode = (SettingsManager.CommunityContentMode + 1) % (CommunityContentMode)EnumUtils.GetEnumValues(typeof(CommunityContentMode)).Count;
			}
			this.m_windowModeButton.Text = LanguageControl.Get(new string[]
			{
				"WindowMode",
				SettingsManager.WindowMode.ToString()
			});
			this.m_uiSizeButton.Text = LanguageControl.Get(new string[]
			{
				"GuiSize",
				SettingsManager.GuiSize.ToString()
			});
			this.m_languageButton.Text = LanguageControl.Get(new string[]
			{
				"Language",
				"Name"
			});
			this.m_displayLogButton.Text = (SettingsManager.DisplayLog ? LanguageControl.Yes : LanguageControl.No);
			this.m_upsideDownButton.Text = (SettingsManager.UpsideDownLayout ? LanguageControl.Yes : LanguageControl.No);
			this.m_hideMoveLookPadsButton.Text = (SettingsManager.HideMoveLookPads ? LanguageControl.Yes : LanguageControl.No);
			this.m_showGuiInScreenshotsButton.Text = (SettingsManager.ShowGuiInScreenshots ? LanguageControl.Yes : LanguageControl.No);
			this.m_showLogoInScreenshotsButton.Text = (SettingsManager.ShowLogoInScreenshots ? LanguageControl.Yes : LanguageControl.No);
			this.m_screenshotSizeButton.Text = LanguageControl.Get(new string[]
			{
				"ScreenshotSize",
				SettingsManager.ScreenshotSize.ToString()
			});
			this.m_communityContentModeButton.Text = LanguageControl.Get(new string[]
			{
				"CommunityContentMode",
				SettingsManager.CommunityContentMode.ToString()
			});
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x0400042C RID: 1068
		public ContainerWidget m_windowModeContainer;

		// Token: 0x0400042D RID: 1069
		public ButtonWidget m_windowModeButton;

		// Token: 0x0400042E RID: 1070
		public ButtonWidget m_languageButton;

		// Token: 0x0400042F RID: 1071
		public ButtonWidget m_displayLogButton;

		// Token: 0x04000430 RID: 1072
		public ButtonWidget m_uiSizeButton;

		// Token: 0x04000431 RID: 1073
		public ButtonWidget m_upsideDownButton;

		// Token: 0x04000432 RID: 1074
		public ButtonWidget m_hideMoveLookPadsButton;

		// Token: 0x04000433 RID: 1075
		public ButtonWidget m_showGuiInScreenshotsButton;

		// Token: 0x04000434 RID: 1076
		public ButtonWidget m_showLogoInScreenshotsButton;

		// Token: 0x04000435 RID: 1077
		public ButtonWidget m_screenshotSizeButton;

		// Token: 0x04000436 RID: 1078
		public ButtonWidget m_communityContentModeButton;

		// Token: 0x04000437 RID: 1079
		public static string fName = "SettingsUiScreen";
	}
}
