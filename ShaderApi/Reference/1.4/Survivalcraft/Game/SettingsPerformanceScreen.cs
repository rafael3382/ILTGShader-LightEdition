using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200017B RID: 379
	public class SettingsPerformanceScreen : Screen
	{
		// Token: 0x0600089A RID: 2202 RVA: 0x00034B88 File Offset: 0x00032D88
		public SettingsPerformanceScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsPerformanceScreen", null);
			base.LoadContents(this, node);
			this.m_resolutionButton = this.Children.Find<ButtonWidget>("ResolutionButton", true);
			this.m_visibilityRangeSlider = this.Children.Find<SliderWidget>("VisibilityRangeSlider", true);
			this.m_visibilityRangeWarningLabel = this.Children.Find<LabelWidget>("VisibilityRangeWarningLabel", true);
			this.m_viewAnglesButton = this.Children.Find<ButtonWidget>("ViewAnglesButton", true);
			this.m_terrainMipmapsButton = this.Children.Find<ButtonWidget>("TerrainMipmapsButton", true);
			this.m_skyRenderingModeButton = this.Children.Find<ButtonWidget>("SkyRenderingModeButton", true);
			this.m_objectShadowsButton = this.Children.Find<ButtonWidget>("ObjectShadowsButton", true);
			this.m_framerateLimitSlider = this.Children.Find<SliderWidget>("FramerateLimitSlider", true);
			this.m_displayFpsCounterButton = this.Children.Find<ButtonWidget>("DisplayFpsCounterButton", true);
			this.m_displayFpsRibbonButton = this.Children.Find<ButtonWidget>("DisplayFpsRibbonButton", true);
			this.m_visibilityRangeSlider.MinValue = 0f;
			this.m_visibilityRangeSlider.MaxValue = (float)(SettingsPerformanceScreen.m_visibilityRanges.Count - 1);
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x00034CBD File Offset: 0x00032EBD
		public override void Enter(object[] parameters)
		{
			this.m_enterVisibilityRange = SettingsManager.VisibilityRange;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x00034CCC File Offset: 0x00032ECC
		public override void Update()
		{
			if (this.m_resolutionButton.IsClicked)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(ResolutionMode));
				SettingsManager.ResolutionMode = (ResolutionMode)((enumValues.IndexOf((int)SettingsManager.ResolutionMode) + 1) % enumValues.Count);
			}
			if (this.m_visibilityRangeSlider.IsSliding)
			{
				SettingsManager.VisibilityRange = SettingsPerformanceScreen.m_visibilityRanges[MathUtils.Clamp((int)this.m_visibilityRangeSlider.Value, 0, SettingsPerformanceScreen.m_visibilityRanges.Count - 1)];
			}
			if (this.m_viewAnglesButton.IsClicked)
			{
				IList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(ViewAngleMode));
				SettingsManager.ViewAngleMode = (ViewAngleMode)((enumValues2.IndexOf((int)SettingsManager.ViewAngleMode) + 1) % enumValues2.Count);
			}
			if (this.m_terrainMipmapsButton.IsClicked)
			{
				SettingsManager.TerrainMipmapsEnabled = !SettingsManager.TerrainMipmapsEnabled;
			}
			if (this.m_skyRenderingModeButton.IsClicked)
			{
				IList<int> enumValues3 = EnumUtils.GetEnumValues(typeof(SkyRenderingMode));
				SettingsManager.SkyRenderingMode = (SkyRenderingMode)((enumValues3.IndexOf((int)SettingsManager.SkyRenderingMode) + 1) % enumValues3.Count);
			}
			if (this.m_objectShadowsButton.IsClicked)
			{
				SettingsManager.ObjectsShadowsEnabled = !SettingsManager.ObjectsShadowsEnabled;
			}
			if (this.m_framerateLimitSlider.IsSliding)
			{
				SettingsManager.PresentationInterval = SettingsPerformanceScreen.m_presentationIntervals[MathUtils.Clamp((int)this.m_framerateLimitSlider.Value, 0, SettingsPerformanceScreen.m_presentationIntervals.Count - 1)];
				Window.PresentationInterval = SettingsManager.PresentationInterval;
			}
			if (this.m_displayFpsCounterButton.IsClicked)
			{
				SettingsManager.DisplayFpsCounter = !SettingsManager.DisplayFpsCounter;
			}
			if (this.m_displayFpsRibbonButton.IsClicked)
			{
				SettingsManager.DisplayFpsRibbon = !SettingsManager.DisplayFpsRibbon;
			}
			this.m_resolutionButton.Text = LanguageControl.Get(new string[]
			{
				"ResolutionMode",
				SettingsManager.ResolutionMode.ToString()
			});
			this.m_visibilityRangeSlider.Value = (float)((SettingsPerformanceScreen.m_visibilityRanges.IndexOf(SettingsManager.VisibilityRange) >= 0) ? SettingsPerformanceScreen.m_visibilityRanges.IndexOf(SettingsManager.VisibilityRange) : 64);
			this.m_visibilityRangeSlider.Text = string.Format(LanguageControl.Get(SettingsPerformanceScreen.fName, 1), SettingsManager.VisibilityRange);
			if (SettingsManager.VisibilityRange <= 48)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 2);
			}
			else if (SettingsManager.VisibilityRange <= 64)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = false;
			}
			else if (SettingsManager.VisibilityRange <= 112)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 3);
			}
			else if (SettingsManager.VisibilityRange <= 224)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 4);
			}
			else if (SettingsManager.VisibilityRange <= 384)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 5);
			}
			else if (SettingsManager.VisibilityRange <= 512)
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 6);
			}
			else
			{
				this.m_visibilityRangeWarningLabel.IsVisible = true;
				this.m_visibilityRangeWarningLabel.Text = LanguageControl.Get(SettingsPerformanceScreen.fName, 7);
			}
			this.m_viewAnglesButton.Text = LanguageControl.Get(new string[]
			{
				"ViewAngleMode",
				SettingsManager.ViewAngleMode.ToString()
			});
			this.m_terrainMipmapsButton.Text = (SettingsManager.TerrainMipmapsEnabled ? LanguageControl.Enable : LanguageControl.Disable);
			this.m_skyRenderingModeButton.Text = LanguageControl.Get(new string[]
			{
				"SkyRenderingMode",
				SettingsManager.SkyRenderingMode.ToString()
			});
			this.m_objectShadowsButton.Text = (SettingsManager.ObjectsShadowsEnabled ? LanguageControl.Enable : LanguageControl.Disable);
			this.m_framerateLimitSlider.Value = (float)((SettingsPerformanceScreen.m_presentationIntervals.IndexOf(SettingsManager.PresentationInterval) >= 0) ? SettingsPerformanceScreen.m_presentationIntervals.IndexOf(SettingsManager.PresentationInterval) : (SettingsPerformanceScreen.m_presentationIntervals.Count - 1));
			this.m_framerateLimitSlider.Text = ((SettingsManager.PresentationInterval != 0) ? string.Format(LanguageControl.Get(SettingsPerformanceScreen.fName, 8), SettingsManager.PresentationInterval) : LanguageControl.Get(SettingsPerformanceScreen.fName, 9));
			this.m_displayFpsCounterButton.Text = (SettingsManager.DisplayFpsCounter ? LanguageControl.Yes : LanguageControl.No);
			this.m_displayFpsRibbonButton.Text = (SettingsManager.DisplayFpsRibbon ? LanguageControl.Yes : LanguageControl.No);
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				bool flag = SettingsManager.VisibilityRange > 128;
				if (SettingsManager.VisibilityRange > this.m_enterVisibilityRange && flag)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(SettingsPerformanceScreen.fName, 10), LanguageControl.Get(SettingsPerformanceScreen.fName, 11), LanguageControl.Ok, LanguageControl.Back, delegate(MessageDialogButton button)
					{
						if (button == MessageDialogButton.Button1)
						{
							ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
						}
					}));
					return;
				}
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000417 RID: 1047
		public static List<int> m_presentationIntervals = new List<int>
		{
			2,
			1,
			0
		};

		// Token: 0x04000418 RID: 1048
		public static List<int> m_visibilityRanges = new List<int>
		{
			32,
			48,
			64,
			80,
			96,
			112,
			128,
			160,
			192,
			224,
			256,
			320,
			384,
			448,
			512,
			576,
			640,
			704,
			768,
			832,
			896,
			960,
			1024
		};

		// Token: 0x04000419 RID: 1049
		public ButtonWidget m_resolutionButton;

		// Token: 0x0400041A RID: 1050
		public SliderWidget m_visibilityRangeSlider;

		// Token: 0x0400041B RID: 1051
		public LabelWidget m_visibilityRangeWarningLabel;

		// Token: 0x0400041C RID: 1052
		public ButtonWidget m_viewAnglesButton;

		// Token: 0x0400041D RID: 1053
		public ButtonWidget m_terrainMipmapsButton;

		// Token: 0x0400041E RID: 1054
		public ButtonWidget m_skyRenderingModeButton;

		// Token: 0x0400041F RID: 1055
		public ButtonWidget m_objectShadowsButton;

		// Token: 0x04000420 RID: 1056
		public SliderWidget m_framerateLimitSlider;

		// Token: 0x04000421 RID: 1057
		public ButtonWidget m_displayFpsCounterButton;

		// Token: 0x04000422 RID: 1058
		public ButtonWidget m_displayFpsRibbonButton;

		// Token: 0x04000423 RID: 1059
		public int m_enterVisibilityRange;

		// Token: 0x04000424 RID: 1060
		public static string fName = "SettingsPerformanceScreen";
	}
}
