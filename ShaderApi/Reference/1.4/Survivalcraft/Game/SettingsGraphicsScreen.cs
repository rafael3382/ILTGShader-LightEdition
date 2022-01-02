using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200017A RID: 378
	public class SettingsGraphicsScreen : Screen
	{
		// Token: 0x06000898 RID: 2200 RVA: 0x00034A48 File Offset: 0x00032C48
		public SettingsGraphicsScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/SettingsGraphicsScreen", null);
			base.LoadContents(this, node);
			this.m_virtualRealityButton = this.Children.Find<BevelledButtonWidget>("VirtualRealityButton", true);
			this.m_brightnessSlider = this.Children.Find<SliderWidget>("BrightnessSlider", true);
			this.m_vrPanel = this.Children.Find<ContainerWidget>("VrPanel", true);
			this.m_vrPanel.IsVisible = false;
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00034AC0 File Offset: 0x00032CC0
		public override void Update()
		{
			if (this.m_brightnessSlider.IsSliding)
			{
				SettingsManager.Brightness = this.m_brightnessSlider.Value;
			}
			this.m_virtualRealityButton.IsEnabled = false;
			this.m_virtualRealityButton.Text = (SettingsManager.UseVr ? "Enabled" : "Disabled");
			this.m_brightnessSlider.Value = SettingsManager.Brightness;
			this.m_brightnessSlider.Text = MathUtils.Round(SettingsManager.Brightness * 10f).ToString();
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x04000414 RID: 1044
		public BevelledButtonWidget m_virtualRealityButton;

		// Token: 0x04000415 RID: 1045
		public SliderWidget m_brightnessSlider;

		// Token: 0x04000416 RID: 1046
		public ContainerWidget m_vrPanel;
	}
}
