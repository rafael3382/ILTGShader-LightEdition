using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200039A RID: 922
	public class PlayerWidget : CanvasWidget
	{
		// Token: 0x06001BC6 RID: 7110 RVA: 0x000D8ECC File Offset: 0x000D70CC
		public PlayerWidget(PlayerData playerData, CharacterSkinsCache characterSkinsCache)
		{
			XElement node = ContentManager.Get<XElement>("Widgets/PlayerWidget", null);
			base.LoadContents(this, node);
			this.m_playerModel = this.Children.Find<PlayerModelWidget>("PlayerModel", true);
			this.m_nameLabel = this.Children.Find<LabelWidget>("Name", true);
			this.m_detailsLabel = this.Children.Find<LabelWidget>("Details", true);
			this.m_editButton = this.Children.Find<ButtonWidget>("EditButton", true);
			this.m_playerModel.CharacterSkinsCache = characterSkinsCache;
			this.m_playerData = playerData;
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000D8F64 File Offset: 0x000D7164
		public override void Update()
		{
			SubsystemGameInfo subsystemGameInfo = this.m_playerData.SubsystemPlayers.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_playerModel.PlayerClass = this.m_playerData.PlayerClass;
			this.m_playerModel.CharacterSkinName = this.m_playerData.CharacterSkinName;
			this.m_nameLabel.Text = this.m_playerData.Name;
			this.m_detailsLabel.Text = this.m_playerData.PlayerClass.ToString();
			LabelWidget detailsLabel = this.m_detailsLabel;
			detailsLabel.Text += "\n";
			LabelWidget detailsLabel2 = this.m_detailsLabel;
			detailsLabel2.Text = string.Format(LanguageControl.Get(base.GetType().Name, 0), detailsLabel2.Text, PlayerScreen.GetDeviceDisplayName(this.m_playerData.InputDevice));
			LabelWidget detailsLabel3 = this.m_detailsLabel;
			detailsLabel3.Text += "\n";
			LabelWidget detailsLabel4 = this.m_detailsLabel;
			detailsLabel4.Text += ((this.m_playerData.LastSpawnTime >= 0.0) ? string.Format(LanguageControl.Get(base.GetType().Name, 1), string.Format("{0:N1}", (subsystemGameInfo.TotalElapsedGameTime - this.m_playerData.LastSpawnTime) / 1200.0)) : LanguageControl.Get(base.GetType().Name, 2));
			if (this.m_editButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Player", new object[]
				{
					PlayerScreen.Mode.Edit,
					this.m_playerData
				});
			}
		}

		// Token: 0x040012D6 RID: 4822
		public PlayerData m_playerData;

		// Token: 0x040012D7 RID: 4823
		public PlayerModelWidget m_playerModel;

		// Token: 0x040012D8 RID: 4824
		public LabelWidget m_nameLabel;

		// Token: 0x040012D9 RID: 4825
		public LabelWidget m_detailsLabel;

		// Token: 0x040012DA RID: 4826
		public ButtonWidget m_editButton;
	}
}
