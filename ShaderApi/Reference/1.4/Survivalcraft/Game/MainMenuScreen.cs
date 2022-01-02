using System;
using System.Xml.Linq;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x02000166 RID: 358
	public class MainMenuScreen : Screen
	{
		// Token: 0x06000818 RID: 2072 RVA: 0x0002F388 File Offset: 0x0002D588
		public MainMenuScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/MainMenuScreen", null);
			base.LoadContents(this, node);
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0002F3BA File Offset: 0x0002D5BA
		public override void Enter(object[] parameters)
		{
			MusicManager.CurrentMix = MusicManager.Mix.Menu;
			this.Children.Find<MotdWidget>(true).Restart();
			if (SettingsManager.IsolatedStorageMigrationCounter < 3)
			{
				SettingsManager.IsolatedStorageMigrationCounter++;
				VersionConverter126To127.MigrateDataFromIsolatedStorageWithDialog();
			}
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0002F3EC File Offset: 0x0002D5EC
		public override void Leave()
		{
			Keyboard.BackButtonQuitsApp = false;
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x0002F3F4 File Offset: 0x0002D5F4
		public override void Update()
		{
			Keyboard.BackButtonQuitsApp = !MarketplaceManager.IsTrialMode;
			if (string.IsNullOrEmpty(this.m_versionString) || MarketplaceManager.IsTrialMode != this.m_versionStringTrial)
			{
				this.m_versionString = string.Format("Version {0}{1}", VersionsManager.Version, MarketplaceManager.IsTrialMode ? " (Day One)" : string.Empty);
				this.m_versionStringTrial = MarketplaceManager.IsTrialMode;
			}
			this.Children.Find("Buy", true).IsVisible = MarketplaceManager.IsTrialMode;
			this.Children.Find<LabelWidget>("Version", true).Text = this.m_versionString;
			RectangleWidget rectangleWidget = this.Children.Find<RectangleWidget>("Logo", true);
			float num = 1f + 0.02f * MathUtils.Sin(1.5f * (float)MathUtils.Remainder(Time.FrameStartTime, 10000.0));
			rectangleWidget.RenderTransform = Matrix.CreateTranslation((0f - rectangleWidget.ActualSize.X) / 2f, (0f - rectangleWidget.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(rectangleWidget.ActualSize.X / 2f, rectangleWidget.ActualSize.Y / 2f, 0f);
			if (this.Children.Find<ButtonWidget>("Play", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Help", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Help", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Content", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Content", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Settings", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Settings", Array.Empty<object>());
			}
			if (this.Children.Find<ButtonWidget>("Buy", true).IsClicked)
			{
				MarketplaceManager.ShowMarketplace();
			}
			if ((base.Input.Back && !Keyboard.BackButtonQuitsApp) || base.Input.IsKeyDownOnce(Key.Escape))
			{
				if (MarketplaceManager.IsTrialMode)
				{
					ScreensManager.SwitchScreen("Nag", Array.Empty<object>());
					return;
				}
				Window.Close();
			}
		}

		// Token: 0x0400037D RID: 893
		public string m_versionString = string.Empty;

		// Token: 0x0400037E RID: 894
		public bool m_versionStringTrial;
	}
}
