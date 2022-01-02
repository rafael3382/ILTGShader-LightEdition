using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000169 RID: 361
	public class NewWorldScreen : Screen
	{
		// Token: 0x06000825 RID: 2085 RVA: 0x0002FD0C File Offset: 0x0002DF0C
		public NewWorldScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/NewWorldScreen", null);
			base.LoadContents(this, node);
			this.m_nameTextBox = this.Children.Find<TextBoxWidget>("Name", true);
			this.m_seedTextBox = this.Children.Find<TextBoxWidget>("Seed", true);
			this.m_gameModeButton = this.Children.Find<ButtonWidget>("GameMode", true);
			this.m_startingPositionButton = this.Children.Find<ButtonWidget>("StartingPosition", true);
			this.m_worldOptionsButton = this.Children.Find<ButtonWidget>("WorldOptions", true);
			this.m_blankSeedLabel = this.Children.Find<LabelWidget>("BlankSeed", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_errorLabel = this.Children.Find<LabelWidget>("Error", true);
			this.m_playButton = this.Children.Find<ButtonWidget>("Play", true);
			this.m_nameTextBox.TextChanged += delegate(TextBoxWidget <p0>)
			{
				this.m_worldSettings.Name = this.m_nameTextBox.Text;
			};
			this.m_seedTextBox.TextChanged += delegate(TextBoxWidget <p0>)
			{
				this.m_worldSettings.Seed = this.m_seedTextBox.Text;
			};
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0002FE3C File Offset: 0x0002E03C
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen.GetType() != typeof(WorldOptionsScreen))
			{
				this.m_worldSettings = new WorldSettings
				{
					Name = WorldsManager.NewWorldNames[this.m_random.Int(0, WorldsManager.NewWorldNames.Count - 1)],
					OriginalSerializationVersion = VersionsManager.SerializationVersion
				};
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0002FEA8 File Offset: 0x0002E0A8
		public override void Update()
		{
			if (this.m_gameModeButton.IsClicked)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(GameMode));
				this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				while (this.m_worldSettings.GameMode == GameMode.Adventure)
				{
					this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				}
			}
			if (this.m_startingPositionButton.IsClicked)
			{
				IList<int> enumValues2 = EnumUtils.GetEnumValues(typeof(StartingPositionMode));
				this.m_worldSettings.StartingPositionMode = (StartingPositionMode)((enumValues2.IndexOf((int)this.m_worldSettings.StartingPositionMode) + 1) % enumValues2.Count);
			}
			bool flag = WorldsManager.ValidateWorldName(this.m_worldSettings.Name);
			this.m_nameTextBox.Text = this.m_worldSettings.Name;
			this.m_seedTextBox.Text = this.m_worldSettings.Seed;
			this.m_gameModeButton.Text = LanguageControl.Get(new string[]
			{
				"GameMode",
				this.m_worldSettings.GameMode.ToString()
			});
			this.m_startingPositionButton.Text = LanguageControl.Get(new string[]
			{
				"StartingPositionMode",
				this.m_worldSettings.StartingPositionMode.ToString()
			});
			this.m_playButton.IsVisible = flag;
			this.m_errorLabel.IsVisible = !flag;
			this.m_blankSeedLabel.IsVisible = (this.m_worldSettings.Seed.Length == 0 && !this.m_seedTextBox.HasFocus);
			this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			if (this.m_worldOptionsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("WorldOptions", new object[]
				{
					this.m_worldSettings,
					false
				});
			}
			if (this.m_playButton.IsClicked && WorldsManager.ValidateWorldName(this.m_nameTextBox.Text))
			{
				if (this.m_worldSettings.GameMode != GameMode.Creative)
				{
					this.m_worldSettings.ResetOptionsForNonCreativeMode();
				}
				WorldInfo worldInfo = WorldsManager.CreateWorld(this.m_worldSettings);
				string name = "GameLoading";
				object[] array = new object[2];
				array[0] = worldInfo;
				ScreensManager.SwitchScreen(name, array);
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
		}

		// Token: 0x0400038C RID: 908
		public TextBoxWidget m_nameTextBox;

		// Token: 0x0400038D RID: 909
		public TextBoxWidget m_seedTextBox;

		// Token: 0x0400038E RID: 910
		public ButtonWidget m_gameModeButton;

		// Token: 0x0400038F RID: 911
		public ButtonWidget m_startingPositionButton;

		// Token: 0x04000390 RID: 912
		public ButtonWidget m_worldOptionsButton;

		// Token: 0x04000391 RID: 913
		public LabelWidget m_blankSeedLabel;

		// Token: 0x04000392 RID: 914
		public LabelWidget m_descriptionLabel;

		// Token: 0x04000393 RID: 915
		public LabelWidget m_errorLabel;

		// Token: 0x04000394 RID: 916
		public ButtonWidget m_playButton;

		// Token: 0x04000395 RID: 917
		public Random m_random = new Random();

		// Token: 0x04000396 RID: 918
		public WorldSettings m_worldSettings;
	}
}
