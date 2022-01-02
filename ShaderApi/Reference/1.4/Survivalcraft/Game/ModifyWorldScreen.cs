using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000167 RID: 359
	public class ModifyWorldScreen : Screen
	{
		// Token: 0x0600081C RID: 2076 RVA: 0x0002F648 File Offset: 0x0002D848
		public ModifyWorldScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/ModifyWorldScreen", null);
			base.LoadContents(this, node);
			this.m_nameTextBox = this.Children.Find<TextBoxWidget>("Name", true);
			this.m_seedLabel = this.Children.Find<LabelWidget>("Seed", true);
			this.m_gameModeButton = this.Children.Find<ButtonWidget>("GameMode", true);
			this.m_worldOptionsButton = this.Children.Find<ButtonWidget>("WorldOptions", true);
			this.m_errorLabel = this.Children.Find<LabelWidget>("Error", true);
			this.m_descriptionLabel = this.Children.Find<LabelWidget>("Description", true);
			this.m_applyButton = this.Children.Find<ButtonWidget>("Apply", true);
			this.m_deleteButton = this.Children.Find<ButtonWidget>("Delete", true);
			this.m_uploadButton = this.Children.Find<ButtonWidget>("Upload", true);
			this.m_nameTextBox.TextChanged += delegate(TextBoxWidget <p0>)
			{
				this.m_worldSettings.Name = this.m_nameTextBox.Text;
			};
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0002F76C File Offset: 0x0002D96C
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen.GetType() != typeof(WorldOptionsScreen))
			{
				this.m_directoryName = (string)parameters[0];
				this.m_worldSettings = (WorldSettings)parameters[1];
				this.m_originalWorldSettingsData.Clear();
				this.m_worldSettings.Save(this.m_originalWorldSettingsData, true);
			}
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0002F7D0 File Offset: 0x0002D9D0
		public override void Update()
		{
			if (this.m_gameModeButton.IsClicked && this.m_worldSettings.GameMode != GameMode.Cruel)
			{
				IList<int> enumValues = EnumUtils.GetEnumValues(typeof(GameMode));
				do
				{
					this.m_worldSettings.GameMode = (GameMode)((enumValues.IndexOf((int)this.m_worldSettings.GameMode) + 1) % enumValues.Count);
				}
				while (this.m_worldSettings.GameMode == GameMode.Cruel);
				this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			}
			this.m_currentWorldSettingsData.Clear();
			this.m_worldSettings.Save(this.m_currentWorldSettingsData, true);
			bool flag = !ModifyWorldScreen.CompareValueDictionaries(this.m_originalWorldSettingsData, this.m_currentWorldSettingsData);
			bool flag2 = WorldsManager.ValidateWorldName(this.m_worldSettings.Name);
			this.m_nameTextBox.Text = this.m_worldSettings.Name;
			this.m_seedLabel.Text = this.m_worldSettings.Seed;
			this.m_gameModeButton.Text = LanguageControl.Get(new string[]
			{
				"GameMode",
				this.m_worldSettings.GameMode.ToString()
			});
			this.m_gameModeButton.IsEnabled = (this.m_worldSettings.GameMode != GameMode.Cruel);
			this.m_errorLabel.IsVisible = !flag2;
			this.m_descriptionLabel.IsVisible = flag2;
			this.m_uploadButton.IsEnabled = (flag2 && !flag);
			this.m_applyButton.IsEnabled = (flag2 && flag);
			this.m_descriptionLabel.Text = StringsManager.GetString("GameMode." + this.m_worldSettings.GameMode.ToString() + ".Description");
			if (this.m_worldOptionsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("WorldOptions", new object[]
				{
					this.m_worldSettings,
					true
				});
			}
			if (this.m_deleteButton.IsClicked)
			{
				MessageDialog dialog = null;
				dialog = new MessageDialog(LanguageControl.Get(base.GetType().Name, 1), LanguageControl.Get(base.GetType().Name, 2), LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton button)
				{
					if (button == MessageDialogButton.Button1)
					{
						WorldsManager.DeleteWorld(this.m_directoryName);
						ScreensManager.SwitchScreen("Play", Array.Empty<object>());
						DialogsManager.HideDialog(dialog);
						return;
					}
					DialogsManager.HideDialog(dialog);
				});
				dialog.AutoHide = false;
				DialogsManager.ShowDialog(null, dialog);
			}
			if (this.m_uploadButton.IsClicked && flag2 && !flag)
			{
				ExternalContentManager.ShowUploadUi(ExternalContentType.World, this.m_directoryName);
			}
			if (this.m_applyButton.IsClicked && flag2 && flag)
			{
				if (this.m_worldSettings.GameMode != GameMode.Creative && this.m_worldSettings.GameMode != GameMode.Adventure)
				{
					this.m_worldSettings.ResetOptionsForNonCreativeMode();
				}
				WorldsManager.ChangeWorld(this.m_directoryName, this.m_worldSettings);
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				if (flag)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(base.GetType().Name, 3), LanguageControl.Get(base.GetType().Name, 4), LanguageControl.Yes, LanguageControl.No, delegate(MessageDialogButton button)
					{
						if (button == MessageDialogButton.Button1)
						{
							ScreensManager.SwitchScreen("Play", Array.Empty<object>());
						}
					}));
					return;
				}
				ScreensManager.SwitchScreen("Play", Array.Empty<object>());
			}
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0002FB64 File Offset: 0x0002DD64
		public static bool CompareValueDictionaries(ValuesDictionary d1, ValuesDictionary d2)
		{
			if (d1.Count != d2.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, object> keyValuePair in d1)
			{
				object value = d2.GetValue<object>(keyValuePair.Key, null);
				ValuesDictionary valuesDictionary = value as ValuesDictionary;
				if (valuesDictionary != null)
				{
					ValuesDictionary valuesDictionary2 = keyValuePair.Value as ValuesDictionary;
					if (valuesDictionary2 == null || !ModifyWorldScreen.CompareValueDictionaries(valuesDictionary, valuesDictionary2))
					{
						return false;
					}
				}
				else if (!object.Equals(value, keyValuePair.Value))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400037F RID: 895
		public TextBoxWidget m_nameTextBox;

		// Token: 0x04000380 RID: 896
		public LabelWidget m_seedLabel;

		// Token: 0x04000381 RID: 897
		public ButtonWidget m_gameModeButton;

		// Token: 0x04000382 RID: 898
		public ButtonWidget m_worldOptionsButton;

		// Token: 0x04000383 RID: 899
		public LabelWidget m_errorLabel;

		// Token: 0x04000384 RID: 900
		public LabelWidget m_descriptionLabel;

		// Token: 0x04000385 RID: 901
		public ButtonWidget m_applyButton;

		// Token: 0x04000386 RID: 902
		public ButtonWidget m_deleteButton;

		// Token: 0x04000387 RID: 903
		public ButtonWidget m_uploadButton;

		// Token: 0x04000388 RID: 904
		public string m_directoryName;

		// Token: 0x04000389 RID: 905
		public WorldSettings m_worldSettings;

		// Token: 0x0400038A RID: 906
		public ValuesDictionary m_currentWorldSettingsData = new ValuesDictionary();

		// Token: 0x0400038B RID: 907
		public ValuesDictionary m_originalWorldSettingsData = new ValuesDictionary();
	}
}
