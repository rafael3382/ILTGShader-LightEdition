using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000178 RID: 376
	public class SettingsCompatibilityScreen : Screen
	{
		// Token: 0x06000893 RID: 2195 RVA: 0x000340C4 File Offset: 0x000322C4
		public SettingsCompatibilityScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsCompatibilityScreen", null);
			base.LoadContents(this, node);
			this.m_singlethreadedTerrainUpdateButton = this.Children.Find<ButtonWidget>("SinglethreadedTerrainUpdateButton", true);
			this.m_useAudioTrackCachingButton = this.Children.Find<ButtonWidget>("UseAudioTrackCachingButton", true);
			this.m_disableAudioTrackCachingContainer = this.Children.Find<ContainerWidget>("DisableAudioTrackCachingContainer", true);
			this.m_useReducedZRangeButton = this.Children.Find<ButtonWidget>("UseReducedZRangeButton", true);
			this.m_useReducedZRangeContainer = this.Children.Find<ContainerWidget>("UseReducedZRangeContainer", true);
			this.m_viewGameLogButton = this.Children.Find<ButtonWidget>("ViewGameLogButton", true);
			this.m_resetDefaultsButton = this.Children.Find<ButtonWidget>("ResetDefaultsButton", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x000341A3 File Offset: 0x000323A3
		public override void Enter(object[] parameters)
		{
			this.m_descriptionLabel.Text = string.Empty;
			this.m_disableAudioTrackCachingContainer.IsVisible = false;
			this.m_useAudioTrackCachingButton.IsVisible = false;
			this.m_useReducedZRangeContainer.IsVisible = false;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x000341DC File Offset: 0x000323DC
		public override void Update()
		{
			if (this.m_singlethreadedTerrainUpdateButton.IsClicked)
			{
				SettingsManager.MultithreadedTerrainUpdate = !SettingsManager.MultithreadedTerrainUpdate;
				this.m_descriptionLabel.Text = StringsManager.GetString("Settings.Compatibility.SinglethreadedTerrainUpdate.Description");
			}
			if (this.m_useReducedZRangeButton.IsClicked)
			{
				SettingsManager.UseReducedZRange = !SettingsManager.UseReducedZRange;
				this.m_descriptionLabel.Text = StringsManager.GetString("Settings.Compatibility.UseReducedZRange.Description");
			}
			if (this.m_useAudioTrackCachingButton.IsClicked)
			{
				SettingsManager.EnableAndroidAudioTrackCaching = !SettingsManager.EnableAndroidAudioTrackCaching;
				this.m_descriptionLabel.Text = StringsManager.GetString("Settings.Compatibility.UseAudioTrackCaching.Description");
			}
			if (this.m_viewGameLogButton.IsClicked)
			{
				DialogsManager.ShowDialog(null, new ViewGameLogDialog());
			}
			if (this.m_resetDefaultsButton.IsClicked)
			{
				SettingsManager.MultithreadedTerrainUpdate = true;
				SettingsManager.UseReducedZRange = false;
			}
			this.m_singlethreadedTerrainUpdateButton.Text = (SettingsManager.MultithreadedTerrainUpdate ? LanguageControl.Off : LanguageControl.On);
			this.m_useAudioTrackCachingButton.Text = (SettingsManager.EnableAndroidAudioTrackCaching ? LanguageControl.On : LanguageControl.Off);
			this.m_useReducedZRangeButton.Text = (SettingsManager.UseReducedZRange ? LanguageControl.On : LanguageControl.Off);
			this.m_resetDefaultsButton.IsEnabled = (!SettingsManager.MultithreadedTerrainUpdate || SettingsManager.UseReducedZRange);
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x040003FB RID: 1019
		public ButtonWidget m_singlethreadedTerrainUpdateButton;

		// Token: 0x040003FC RID: 1020
		public ButtonWidget m_useAudioTrackCachingButton;

		// Token: 0x040003FD RID: 1021
		public ContainerWidget m_disableAudioTrackCachingContainer;

		// Token: 0x040003FE RID: 1022
		public ButtonWidget m_useReducedZRangeButton;

		// Token: 0x040003FF RID: 1023
		public ContainerWidget m_useReducedZRangeContainer;

		// Token: 0x04000400 RID: 1024
		public ButtonWidget m_viewGameLogButton;

		// Token: 0x04000401 RID: 1025
		public ButtonWidget m_resetDefaultsButton;

		// Token: 0x04000402 RID: 1026
		public LabelWidget m_descriptionLabel;
	}
}
