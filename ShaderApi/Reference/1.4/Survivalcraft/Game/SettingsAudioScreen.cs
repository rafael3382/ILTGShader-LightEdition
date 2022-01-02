using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000177 RID: 375
	public class SettingsAudioScreen : Screen
	{
		// Token: 0x06000891 RID: 2193 RVA: 0x00033F7C File Offset: 0x0003217C
		public SettingsAudioScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsAudioScreen", null);
			base.LoadContents(this, node);
			this.m_soundsVolumeSlider = this.Children.Find<SliderWidget>("SoundsVolumeSlider", true);
			this.m_musicVolumeSlider = this.Children.Find<SliderWidget>("MusicVolumeSlider", true);
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x00033FD4 File Offset: 0x000321D4
		public override void Update()
		{
			if (this.m_soundsVolumeSlider.IsSliding)
			{
				SettingsManager.SoundsVolume = this.m_soundsVolumeSlider.Value;
			}
			if (this.m_musicVolumeSlider.IsSliding)
			{
				SettingsManager.MusicVolume = this.m_musicVolumeSlider.Value;
			}
			this.m_soundsVolumeSlider.Value = SettingsManager.SoundsVolume;
			this.m_soundsVolumeSlider.Text = MathUtils.Round(SettingsManager.SoundsVolume * 10f).ToString();
			this.m_musicVolumeSlider.Value = SettingsManager.MusicVolume;
			this.m_musicVolumeSlider.Text = MathUtils.Round(SettingsManager.MusicVolume * 10f).ToString();
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x040003F9 RID: 1017
		public SliderWidget m_soundsVolumeSlider;

		// Token: 0x040003FA RID: 1018
		public SliderWidget m_musicVolumeSlider;
	}
}
