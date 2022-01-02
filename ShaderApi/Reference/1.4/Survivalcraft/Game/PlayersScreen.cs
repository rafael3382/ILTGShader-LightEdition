using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200016B RID: 363
	public class PlayersScreen : Screen
	{
		// Token: 0x06000837 RID: 2103 RVA: 0x00030CB4 File Offset: 0x0002EEB4
		public PlayersScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/PlayersScreen", null);
			base.LoadContents(this, node);
			this.m_playersPanel = this.Children.Find<StackPanelWidget>("PlayersPanel", true);
			this.m_addPlayerButton = this.Children.Find<ButtonWidget>("AddPlayerButton", true);
			this.m_screenLayoutButton = this.Children.Find<ButtonWidget>("ScreenLayoutButton", true);
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00030D2C File Offset: 0x0002EF2C
		public override void Enter(object[] parameters)
		{
			this.m_subsystemPlayers = (SubsystemPlayers)parameters[0];
			SubsystemPlayers subsystemPlayers = this.m_subsystemPlayers;
			subsystemPlayers.PlayerAdded = (Action<PlayerData>)Delegate.Combine(subsystemPlayers.PlayerAdded, new Action<PlayerData>(this.PlayersChanged));
			SubsystemPlayers subsystemPlayers2 = this.m_subsystemPlayers;
			subsystemPlayers2.PlayerRemoved = (Action<PlayerData>)Delegate.Combine(subsystemPlayers2.PlayerRemoved, new Action<PlayerData>(this.PlayersChanged));
			this.UpdatePlayersPanel();
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00030D9C File Offset: 0x0002EF9C
		public override void Leave()
		{
			SubsystemPlayers subsystemPlayers = this.m_subsystemPlayers;
			subsystemPlayers.PlayerAdded = (Action<PlayerData>)Delegate.Remove(subsystemPlayers.PlayerAdded, new Action<PlayerData>(this.PlayersChanged));
			SubsystemPlayers subsystemPlayers2 = this.m_subsystemPlayers;
			subsystemPlayers2.PlayerRemoved = (Action<PlayerData>)Delegate.Remove(subsystemPlayers2.PlayerRemoved, new Action<PlayerData>(this.PlayersChanged));
			this.m_subsystemPlayers = null;
			this.m_characterSkinsCache.Clear();
			this.m_playersPanel.Children.Clear();
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00030E1C File Offset: 0x0002F01C
		public override void Update()
		{
			if (this.m_addPlayerButton.IsClicked)
			{
				SubsystemGameInfo subsystemGameInfo = this.m_subsystemPlayers.Project.FindSubsystem<SubsystemGameInfo>(true);
				if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Unavailable, LanguageControl.GetContentWidgets(base.GetType().Name, 3), LanguageControl.Ok, null, null));
				}
				else if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Unavailable, LanguageControl.GetContentWidgets(base.GetType().Name, 4), LanguageControl.Ok, null, null));
				}
				else if (this.m_subsystemPlayers.PlayersData.Count >= 4)
				{
					DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Unavailable, string.Format(LanguageControl.GetContentWidgets(base.GetType().Name, 5), SubsystemPlayers.MaxPlayers), LanguageControl.Ok, null, null));
				}
				else
				{
					ScreensManager.SwitchScreen("Player", new object[]
					{
						PlayerScreen.Mode.Add,
						this.m_subsystemPlayers.Project
					});
				}
			}
			if (this.m_screenLayoutButton.IsClicked)
			{
				ScreenLayout[] array = null;
				if (this.m_subsystemPlayers.PlayersData.Count == 1)
				{
					array = new ScreenLayout[1];
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 2)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.DoubleVertical,
						ScreenLayout.DoubleHorizontal,
						ScreenLayout.DoubleOpposite
					};
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 3)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.TripleVertical,
						ScreenLayout.TripleHorizontal,
						ScreenLayout.TripleEven,
						ScreenLayout.TripleOpposite
					};
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 4)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.Quadruple,
						ScreenLayout.QuadrupleOpposite
					};
				}
				if (array != null)
				{
					DialogsManager.ShowDialog(null, new ListSelectionDialog(LanguageControl.GetContentWidgets(base.GetType().Name, 6), array, 80f, delegate(object o)
					{
						string str = o.ToString();
						string name = "Textures/Atlas/ScreenLayout" + str;
						return new StackPanelWidget
						{
							Direction = LayoutDirection.Horizontal,
							VerticalAlignment = WidgetAlignment.Center,
							Children = 
							{
								new RectangleWidget
								{
									Size = new Vector2(98f, 56f),
									Subtexture = ContentManager.Get<Subtexture>(name, null),
									FillColor = Color.White,
									OutlineColor = Color.Transparent,
									Margin = new Vector2(10f, 0f)
								},
								new StackPanelWidget
								{
									Direction = LayoutDirection.Vertical,
									VerticalAlignment = WidgetAlignment.Center,
									Margin = new Vector2(10f, 0f),
									Children = 
									{
										new LabelWidget
										{
											Text = StringsManager.GetString("ScreenLayout." + str + ".Name")
										},
										new LabelWidget
										{
											Text = StringsManager.GetString("ScreenLayout." + str + ".Description"),
											Color = Color.Gray
										}
									}
								}
							}
						};
					}, delegate(object o)
					{
						if (o != null)
						{
							if (this.m_subsystemPlayers.PlayersData.Count == 1)
							{
								SettingsManager.ScreenLayout1 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 2)
							{
								SettingsManager.ScreenLayout2 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 3)
							{
								SettingsManager.ScreenLayout3 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 4)
							{
								SettingsManager.ScreenLayout4 = (ScreenLayout)o;
							}
						}
					}));
				}
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Game", Array.Empty<object>());
			}
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00031070 File Offset: 0x0002F270
		public void UpdatePlayersPanel()
		{
			this.m_playersPanel.Children.Clear();
			foreach (PlayerData playerData in this.m_subsystemPlayers.PlayersData)
			{
				this.m_playersPanel.Children.Add(new PlayerWidget(playerData, this.m_characterSkinsCache));
			}
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x000310F0 File Offset: 0x0002F2F0
		public void PlayersChanged(PlayerData playerData)
		{
			this.UpdatePlayersPanel();
		}

		// Token: 0x040003A8 RID: 936
		public StackPanelWidget m_playersPanel;

		// Token: 0x040003A9 RID: 937
		public ButtonWidget m_addPlayerButton;

		// Token: 0x040003AA RID: 938
		public ButtonWidget m_screenLayoutButton;

		// Token: 0x040003AB RID: 939
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040003AC RID: 940
		public CharacterSkinsCache m_characterSkinsCache = new CharacterSkinsCache();
	}
}
