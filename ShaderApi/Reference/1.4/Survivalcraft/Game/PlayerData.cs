using System;
using System.Globalization;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020002E1 RID: 737
	public class PlayerData : IDisposable
	{
		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x000A5664 File Offset: 0x000A3864
		// (set) Token: 0x060015F7 RID: 5623 RVA: 0x000A566C File Offset: 0x000A386C
		public int PlayerIndex { get; set; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060015F8 RID: 5624 RVA: 0x000A5675 File Offset: 0x000A3875
		// (set) Token: 0x060015F9 RID: 5625 RVA: 0x000A567D File Offset: 0x000A387D
		public SubsystemGameWidgets SubsystemGameWidgets { get; set; }

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060015FA RID: 5626 RVA: 0x000A5686 File Offset: 0x000A3886
		// (set) Token: 0x060015FB RID: 5627 RVA: 0x000A568E File Offset: 0x000A388E
		public SubsystemPlayers SubsystemPlayers { get; set; }

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060015FC RID: 5628 RVA: 0x000A5697 File Offset: 0x000A3897
		// (set) Token: 0x060015FD RID: 5629 RVA: 0x000A569F File Offset: 0x000A389F
		public ComponentPlayer ComponentPlayer { get; set; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x000A56A8 File Offset: 0x000A38A8
		public GameWidget GameWidget
		{
			get
			{
				if (this.m_gameWidget == null)
				{
					foreach (GameWidget gameWidget in this.SubsystemGameWidgets.GameWidgets)
					{
						if (gameWidget.PlayerData == this)
						{
							this.m_gameWidget = gameWidget;
							break;
						}
					}
					if (this.m_gameWidget == null)
					{
						throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 11));
					}
				}
				return this.m_gameWidget;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x000A5738 File Offset: 0x000A3938
		// (set) Token: 0x06001600 RID: 5632 RVA: 0x000A5740 File Offset: 0x000A3940
		public Vector3 SpawnPosition { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06001601 RID: 5633 RVA: 0x000A5749 File Offset: 0x000A3949
		// (set) Token: 0x06001602 RID: 5634 RVA: 0x000A5751 File Offset: 0x000A3951
		public double FirstSpawnTime { get; set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06001603 RID: 5635 RVA: 0x000A575A File Offset: 0x000A395A
		// (set) Token: 0x06001604 RID: 5636 RVA: 0x000A5762 File Offset: 0x000A3962
		public double LastSpawnTime { get; set; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001605 RID: 5637 RVA: 0x000A576B File Offset: 0x000A396B
		// (set) Token: 0x06001606 RID: 5638 RVA: 0x000A5773 File Offset: 0x000A3973
		public int SpawnsCount { get; set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001607 RID: 5639 RVA: 0x000A577C File Offset: 0x000A397C
		// (set) Token: 0x06001608 RID: 5640 RVA: 0x000A5784 File Offset: 0x000A3984
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				if (value != this.m_name)
				{
					this.m_name = value;
					this.IsDefaultName = false;
				}
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001609 RID: 5641 RVA: 0x000A57A2 File Offset: 0x000A39A2
		// (set) Token: 0x0600160A RID: 5642 RVA: 0x000A57AA File Offset: 0x000A39AA
		public bool IsDefaultName { get; set; }

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x0600160B RID: 5643 RVA: 0x000A57B3 File Offset: 0x000A39B3
		// (set) Token: 0x0600160C RID: 5644 RVA: 0x000A57BC File Offset: 0x000A39BC
		public PlayerClass PlayerClass
		{
			get
			{
				return this.m_playerClass;
			}
			set
			{
				if (this.SubsystemPlayers.PlayersData.Contains(this))
				{
					throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 1));
				}
				this.m_playerClass = value;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x0600160D RID: 5645 RVA: 0x000A57F7 File Offset: 0x000A39F7
		// (set) Token: 0x0600160E RID: 5646 RVA: 0x000A57FF File Offset: 0x000A39FF
		public float Level { get; set; }

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x0600160F RID: 5647 RVA: 0x000A5808 File Offset: 0x000A3A08
		// (set) Token: 0x06001610 RID: 5648 RVA: 0x000A5810 File Offset: 0x000A3A10
		public string CharacterSkinName { get; set; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001611 RID: 5649 RVA: 0x000A5819 File Offset: 0x000A3A19
		// (set) Token: 0x06001612 RID: 5650 RVA: 0x000A5821 File Offset: 0x000A3A21
		public WidgetInputDevice InputDevice { get; set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001613 RID: 5651 RVA: 0x000A582A File Offset: 0x000A3A2A
		public bool IsReadyForPlaying
		{
			get
			{
				return this.m_stateMachine.CurrentState == "Playing" || this.m_stateMachine.CurrentState == "PlayerDead";
			}
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x000A585C File Offset: 0x000A3A5C
		public PlayerData(Project project)
		{
			this.m_project = project;
			this.SubsystemPlayers = project.FindSubsystem<SubsystemPlayers>(true);
			this.SubsystemGameWidgets = project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = project.FindSubsystem<SubsystemSky>(true);
			this.m_playerClass = PlayerClass.Male;
			this.Level = 1f;
			this.FirstSpawnTime = -1.0;
			this.LastSpawnTime = -1.0;
			this.RandomizeCharacterSkin();
			this.ResetName();
			this.InputDevice = WidgetInputDevice.None;
			this.m_stateMachine.AddState("FirstUpdate", null, delegate
			{
				if (this.ComponentPlayer != null)
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 4), this.Name, MathUtils.Floor(this.Level)), null, 0f, true);
					this.m_stateMachine.TransitionTo("WaitForTerrain");
					return;
				}
				this.m_stateMachine.TransitionTo("PrepareSpawn");
			}, null);
			this.m_stateMachine.AddState("PrepareSpawn", delegate
			{
				if (this.SpawnPosition == Vector3.Zero)
				{
					if (this.SubsystemPlayers.GlobalSpawnPosition == Vector3.Zero)
					{
						PlayerData playerData = this.SubsystemPlayers.PlayersData.FirstOrDefault((PlayerData pd) => pd.SpawnPosition != Vector3.Zero);
						if (playerData != null)
						{
							if (playerData.ComponentPlayer != null)
							{
								this.SpawnPosition = playerData.ComponentPlayer.ComponentBody.Position;
								this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
							}
							else
							{
								this.SpawnPosition = playerData.SpawnPosition;
								this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
							}
						}
						else
						{
							this.SpawnPosition = this.m_subsystemTerrain.TerrainContentsGenerator.FindCoarseSpawnPosition();
							this.m_spawnMode = PlayerData.SpawnMode.InitialIntro;
						}
						this.SubsystemPlayers.GlobalSpawnPosition = this.SpawnPosition;
					}
					else
					{
						this.SpawnPosition = this.SubsystemPlayers.GlobalSpawnPosition;
						this.m_spawnMode = PlayerData.SpawnMode.InitialNoIntro;
					}
				}
				else
				{
					this.m_spawnMode = PlayerData.SpawnMode.Respawn;
				}
				if (this.m_spawnMode == PlayerData.SpawnMode.Respawn)
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 2), this.Name, MathUtils.Floor(this.Level)), LanguageControl.Get(PlayerData.fName, 3), 0f, true);
				}
				else
				{
					this.UpdateSpawnDialog(string.Format(LanguageControl.Get(PlayerData.fName, 4), this.Name, MathUtils.Floor(this.Level)), null, 0f, true);
				}
				this.m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(this.PlayerIndex, this.SpawnPosition.XZ, 0f, 64f);
				this.m_terrainWaitStartTime = Time.FrameStartTime;
			}, delegate
			{
				if (Time.PeriodicEvent(0.1, 0.0))
				{
					float updateProgress = this.m_subsystemTerrain.TerrainUpdater.GetUpdateProgress(this.PlayerIndex, 0f, 64f);
					this.UpdateSpawnDialog(null, null, 0.5f * updateProgress, false);
					if (updateProgress >= 1f || Time.FrameStartTime - this.m_terrainWaitStartTime >= 15.0)
					{
						switch (this.m_spawnMode)
						{
						case PlayerData.SpawnMode.InitialIntro:
							this.SpawnPosition = this.FindIntroSpawnPosition(this.SpawnPosition.XZ);
							break;
						case PlayerData.SpawnMode.InitialNoIntro:
							this.SpawnPosition = this.FindNoIntroSpawnPosition(this.SpawnPosition, false);
							break;
						case PlayerData.SpawnMode.Respawn:
							this.SpawnPosition = this.FindNoIntroSpawnPosition(this.SpawnPosition, true);
							break;
						default:
							throw new InvalidOperationException(LanguageControl.Get(PlayerData.fName, 5));
						}
						this.m_stateMachine.TransitionTo("WaitForTerrain");
					}
				}
			}, null);
			this.m_stateMachine.AddState("WaitForTerrain", delegate
			{
				this.m_terrainWaitStartTime = Time.FrameStartTime;
				Vector2 center = (this.ComponentPlayer != null) ? this.ComponentPlayer.ComponentBody.Position.XZ : this.SpawnPosition.XZ;
				this.m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(this.PlayerIndex, center, MathUtils.Min(this.m_subsystemSky.VisibilityRange, 64f), 0f);
			}, delegate
			{
				if (Time.PeriodicEvent(0.1, 0.0))
				{
					float updateProgress = this.m_subsystemTerrain.TerrainUpdater.GetUpdateProgress(this.PlayerIndex, MathUtils.Min(this.m_subsystemSky.VisibilityRange, 64f), 0f);
					this.UpdateSpawnDialog(null, null, 0.5f + 0.5f * updateProgress, false);
					if ((updateProgress >= 1f && Time.FrameStartTime - this.m_terrainWaitStartTime > 2.0) || Time.FrameStartTime - this.m_terrainWaitStartTime >= 15.0)
					{
						if (this.ComponentPlayer == null)
						{
							this.SpawnPlayer(this.SpawnPosition, this.m_spawnMode);
						}
						this.m_stateMachine.TransitionTo("Playing");
					}
				}
			}, null);
			this.m_stateMachine.AddState("Playing", delegate
			{
				this.HideSpawnDialog();
			}, delegate
			{
				if (this.ComponentPlayer == null)
				{
					this.m_stateMachine.TransitionTo("PrepareSpawn");
					return;
				}
				if (this.m_playerDeathTime != null)
				{
					this.m_stateMachine.TransitionTo("PlayerDead");
					return;
				}
				if (this.ComponentPlayer.ComponentHealth.Health <= 0f)
				{
					this.m_playerDeathTime = new double?(Time.RealTime);
				}
			}, null);
			this.m_stateMachine.AddState("PlayerDead", delegate
			{
				ModsManager.HookAction("OnPlayerDead", delegate(ModLoader modLoader)
				{
					modLoader.OnPlayerDead(this);
					return false;
				});
			}, delegate
			{
				if (this.ComponentPlayer == null)
				{
					this.m_stateMachine.TransitionTo("PrepareSpawn");
					return;
				}
				if (Time.RealTime - this.m_playerDeathTime.Value > 1.5 && !DialogsManager.HasDialogs(this.ComponentPlayer.GuiWidget) && this.ComponentPlayer.GameWidget.Input.Any)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
					{
						DialogsManager.ShowDialog(this.ComponentPlayer.GuiWidget, new GameMenuDialog(this.ComponentPlayer));
						return;
					}
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure && !this.m_subsystemGameInfo.WorldSettings.IsAdventureRespawnAllowed)
					{
						ScreensManager.SwitchScreen("GameLoading", new object[]
						{
							GameManager.WorldInfo,
							"AdventureRestart"
						});
						return;
					}
					this.m_project.RemoveEntity(this.ComponentPlayer.Entity, true);
				}
			}, null);
			this.m_stateMachine.TransitionTo("FirstUpdate");
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x000A59D7 File Offset: 0x000A3BD7
		public void Dispose()
		{
			this.HideSpawnDialog();
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x000A59E0 File Offset: 0x000A3BE0
		public void RandomizeCharacterSkin()
		{
			Game.Random random = new Game.Random();
			CharacterSkinsManager.UpdateCharacterSkinsList();
			string[] array = CharacterSkinsManager.CharacterSkinsNames.Where(delegate(string n)
			{
				if (CharacterSkinsManager.IsBuiltIn(n))
				{
					PlayerClass? playerClass = CharacterSkinsManager.GetPlayerClass(n);
					PlayerClass playerClass2 = this.m_playerClass;
					return playerClass.GetValueOrDefault() == playerClass2 & playerClass != null;
				}
				return false;
			}).ToArray<string>();
			string[] second = (from pd in this.SubsystemPlayers.PlayersData
			select pd.CharacterSkinName).ToArray<string>();
			string[] array2 = array.Except(second).ToArray<string>();
			this.CharacterSkinName = ((array2.Length != 0) ? array2[random.Int(0, array2.Length - 1)] : array[random.Int(0, array.Length - 1)]);
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x000A5A88 File Offset: 0x000A3C88
		public void ResetName()
		{
			this.m_name = CharacterSkinsManager.GetDisplayName(this.CharacterSkinName);
			this.IsDefaultName = true;
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x000A5AA2 File Offset: 0x000A3CA2
		public static bool VerifyName(string name)
		{
			return name.Length >= 2;
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x000A5AB0 File Offset: 0x000A3CB0
		public void Update()
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x000A5AC0 File Offset: 0x000A3CC0
		public void Load(ValuesDictionary valuesDictionary)
		{
			this.SpawnPosition = valuesDictionary.GetValue<Vector3>("SpawnPosition", Vector3.Zero);
			this.FirstSpawnTime = valuesDictionary.GetValue<double>("FirstSpawnTime", 0.0);
			this.LastSpawnTime = valuesDictionary.GetValue<double>("LastSpawnTime", 0.0);
			this.SpawnsCount = valuesDictionary.GetValue<int>("SpawnsCount", 0);
			this.Name = valuesDictionary.GetValue<string>("Name", "Walter");
			this.PlayerClass = valuesDictionary.GetValue<PlayerClass>("PlayerClass", PlayerClass.Male);
			this.Level = valuesDictionary.GetValue<float>("Level", 1f);
			this.CharacterSkinName = valuesDictionary.GetValue<string>("CharacterSkinName", CharacterSkinsManager.CharacterSkinsNames[0]);
			this.InputDevice = valuesDictionary.GetValue<WidgetInputDevice>("InputDevice", this.InputDevice);
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x000A5BA0 File Offset: 0x000A3DA0
		public void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<Vector3>("SpawnPosition", this.SpawnPosition);
			valuesDictionary.SetValue<double>("FirstSpawnTime", this.FirstSpawnTime);
			valuesDictionary.SetValue<double>("LastSpawnTime", this.LastSpawnTime);
			valuesDictionary.SetValue<int>("SpawnsCount", this.SpawnsCount);
			valuesDictionary.SetValue<string>("Name", this.Name);
			valuesDictionary.SetValue<PlayerClass>("PlayerClass", this.PlayerClass);
			valuesDictionary.SetValue<float>("Level", this.Level);
			valuesDictionary.SetValue<string>("CharacterSkinName", this.CharacterSkinName);
			valuesDictionary.SetValue<WidgetInputDevice>("InputDevice", this.InputDevice);
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x000A5C48 File Offset: 0x000A3E48
		public void OnEntityAdded(Entity entity)
		{
			ComponentPlayer componentPlayer = entity.FindComponent<ComponentPlayer>();
			if (componentPlayer != null && componentPlayer.PlayerData == this)
			{
				if (this.ComponentPlayer != null)
				{
					throw new InvalidOperationException(string.Format(LanguageControl.Get(PlayerData.fName, 10), this.PlayerIndex));
				}
				this.ComponentPlayer = componentPlayer;
				this.GameWidget.ActiveCamera = this.GameWidget.FindCamera<FppCamera>(true);
				this.GameWidget.Target = componentPlayer;
				if (this.FirstSpawnTime < 0.0)
				{
					this.FirstSpawnTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
				}
			}
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x000A5CDE File Offset: 0x000A3EDE
		public void OnEntityRemoved(Entity entity)
		{
			if (this.ComponentPlayer != null && entity == this.ComponentPlayer.Entity)
			{
				this.ComponentPlayer = null;
				this.m_playerDeathTime = null;
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000A5D0C File Offset: 0x000A3F0C
		public Vector3 FindIntroSpawnPosition(Vector2 desiredSpawnPosition)
		{
			Vector2 zero = Vector2.Zero;
			float num = float.MinValue;
			for (int i = -30; i <= 30; i += 2)
			{
				for (int j = -30; j <= 30; j += 2)
				{
					int num2 = Terrain.ToCell(desiredSpawnPosition.X) + i;
					int num3 = Terrain.ToCell(desiredSpawnPosition.Y) + j;
					float num4 = this.ScoreIntroSpawnPosition(desiredSpawnPosition, num2, num3);
					if (num4 > num)
					{
						num = num4;
						zero = new Vector2((float)num2, (float)num3);
					}
				}
			}
			float num5 = (float)(this.m_subsystemTerrain.Terrain.CalculateTopmostCellHeight(Terrain.ToCell(zero.X), Terrain.ToCell(zero.Y)) + 1);
			return new Vector3(zero.X + 0.5f, num5 + 0.01f, zero.Y + 0.5f);
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x000A5DD4 File Offset: 0x000A3FD4
		public Vector3 FindNoIntroSpawnPosition(Vector3 desiredSpawnPosition, bool respawn)
		{
			Vector3 zero = Vector3.Zero;
			float num = float.MinValue;
			for (int i = -8; i <= 8; i++)
			{
				for (int j = -8; j <= 8; j++)
				{
					for (int k = -8; k <= 8; k++)
					{
						int num2 = Terrain.ToCell(desiredSpawnPosition.X) + i;
						int num3 = Terrain.ToCell(desiredSpawnPosition.Y) + j;
						int num4 = Terrain.ToCell(desiredSpawnPosition.Z) + k;
						float num5 = this.ScoreNoIntroSpawnPosition(desiredSpawnPosition, num2, num3, num4);
						if (num5 > num)
						{
							num = num5;
							zero = new Vector3((float)num2, (float)num3, (float)num4);
						}
					}
				}
			}
			return new Vector3(zero.X + 0.5f, zero.Y + 0.01f, zero.Z + 0.5f);
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x000A5E98 File Offset: 0x000A4098
		public float ScoreIntroSpawnPosition(Vector2 desiredSpawnPosition, int x, int z)
		{
			float num = -0.01f * Vector2.Distance(new Vector2((float)x, (float)z), desiredSpawnPosition);
			int num2 = this.m_subsystemTerrain.Terrain.CalculateTopmostCellHeight(x, z);
			if (num2 < 64 || num2 > 85)
			{
				num -= 5f;
			}
			if (this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) < 8)
			{
				num -= 5f;
			}
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, num2, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				num -= 5f;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					if (this.m_subsystemTerrain.Terrain.GetCellContents(i, num2 + 2, j) != 0)
					{
						num -= 1f;
					}
				}
			}
			Vector2 vector = ComponentIntro.FindOceanDirection(this.m_subsystemTerrain.TerrainContentsGenerator, new Vector2((float)x, (float)z));
			Vector3 vector2 = new Vector3((float)x, (float)num2 + 1.5f, (float)z);
			for (int k = -1; k <= 1; k++)
			{
				Vector3 end = vector2 + new Vector3(30f * vector.X, 5f * (float)k, 30f * vector.Y);
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector2, end, false, true, (int value, float distance) => Terrain.ExtractContents(value) != 0);
				if (terrainRaycastResult != null)
				{
					CellFace cellFace = terrainRaycastResult.Value.CellFace;
					int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(cellFace.X, cellFace.Y, cellFace.Z);
					if (cellContents != 18 && cellContents != 0)
					{
						num -= 2f;
					}
				}
			}
			return num;
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x000A606C File Offset: 0x000A426C
		public float ScoreNoIntroSpawnPosition(Vector3 desiredSpawnPosition, int x, int y, int z)
		{
			float num = -0.01f * Vector3.Distance(new Vector3((float)x, (float)y, (float)z), desiredSpawnPosition);
			if (y < 1 || y >= 255)
			{
				num -= 100f;
			}
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
			int cellValue3 = this.m_subsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
			Block block3 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue3)];
			if (block.IsTransparent_(cellValue))
			{
				num -= 10f;
			}
			if (!block.IsCollidable_(cellValue))
			{
				num -= 10f;
			}
			if (block2.IsCollidable_(cellValue2))
			{
				num -= 10f;
			}
			if (block3.IsCollidable_(cellValue3))
			{
				num -= 10f;
			}
			foreach (PlayerData playerData in this.SubsystemPlayers.PlayersData)
			{
				if (playerData != this && Vector3.DistanceSquared(playerData.SpawnPosition, new Vector3((float)x, (float)y, (float)z)) < (float)MathUtils.Sqr(2))
				{
					num -= 1f;
				}
			}
			return num;
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000A61D0 File Offset: 0x000A43D0
		public bool CheckIsPointInWater(Point3 p)
		{
			bool result = true;
			for (int i = p.X - 1; i < p.X + 1; i++)
			{
				for (int j = p.Z - 1; j < p.Z + 1; j++)
				{
					for (int k = p.Y; k > 0; k--)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(p.X, k, p.Z);
						Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
						if (block.IsCollidable_(cellValue))
						{
							return false;
						}
						if (block is WaterBlock)
						{
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x000A626C File Offset: 0x000A446C
		public void SpawnPlayer(Vector3 position, PlayerData.SpawnMode spawnMode)
		{
			PlayerData.<>c__DisplayClass88_0 CS$<>8__locals1 = new PlayerData.<>c__DisplayClass88_0();
			CS$<>8__locals1.spawnMode = spawnMode;
			CS$<>8__locals1.position = position;
			ComponentMount componentMount = null;
			if (CS$<>8__locals1.spawnMode != PlayerData.SpawnMode.Respawn && this.CheckIsPointInWater(Terrain.ToCell(CS$<>8__locals1.position)))
			{
				Entity entity = DatabaseManager.CreateEntity(this.m_project, "Boat", true);
				entity.FindComponent<ComponentBody>(true).Position = CS$<>8__locals1.position;
				entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.DegToRad(45f));
				componentMount = entity.FindComponent<ComponentMount>(true);
				this.m_project.AddEntity(entity);
				PlayerData.<>c__DisplayClass88_0 CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals2.position.Y = CS$<>8__locals2.position.Y + entity.FindComponent<ComponentBody>(true).BoxSize.Y;
			}
			string value = "";
			string value2 = "";
			string value3 = "";
			string value4 = "";
			if (CS$<>8__locals1.spawnMode != PlayerData.SpawnMode.Respawn)
			{
				if (this.PlayerClass == PlayerClass.Female)
				{
					if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("2"))
					{
						value = "";
						value2 = PlayerData.MakeClothingValue(37, 2);
						value3 = PlayerData.MakeClothingValue(16, 14);
						value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(27, 0);
					}
					else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("3"))
					{
						value = PlayerData.MakeClothingValue(31, 0);
						value2 = PlayerData.MakeClothingValue(13, 7) + ";" + PlayerData.MakeClothingValue(5, 0);
						value3 = PlayerData.MakeClothingValue(17, 15);
						value4 = PlayerData.MakeClothingValue(29, 0);
					}
					else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("4"))
					{
						value = PlayerData.MakeClothingValue(30, 7);
						value2 = PlayerData.MakeClothingValue(14, 6);
						value3 = PlayerData.MakeClothingValue(25, 7);
						value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
					}
					else
					{
						value = PlayerData.MakeClothingValue(30, 12);
						value2 = PlayerData.MakeClothingValue(37, 3) + ";" + PlayerData.MakeClothingValue(1, 3);
						value3 = PlayerData.MakeClothingValue(0, 12);
						value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
					}
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("2"))
				{
					value = "";
					value2 = PlayerData.MakeClothingValue(13, 0) + ";" + PlayerData.MakeClothingValue(5, 0);
					value3 = PlayerData.MakeClothingValue(25, 8);
					value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("3"))
				{
					value = PlayerData.MakeClothingValue(32, 0);
					value2 = PlayerData.MakeClothingValue(37, 5);
					value3 = PlayerData.MakeClothingValue(0, 15);
					value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
				}
				else if (CharacterSkinsManager.IsBuiltIn(this.CharacterSkinName) && this.CharacterSkinName.Contains("4"))
				{
					value = PlayerData.MakeClothingValue(31, 0);
					value2 = PlayerData.MakeClothingValue(15, 14);
					value3 = PlayerData.MakeClothingValue(0, 0);
					value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(8, 0);
				}
				else
				{
					value = PlayerData.MakeClothingValue(32, 0);
					value2 = PlayerData.MakeClothingValue(37, 0) + ";" + PlayerData.MakeClothingValue(1, 9);
					value3 = PlayerData.MakeClothingValue(0, 12);
					value4 = PlayerData.MakeClothingValue(26, 6) + ";" + PlayerData.MakeClothingValue(29, 0);
				}
			}
			ValuesDictionary overrides = new ValuesDictionary
			{
				{
					"Player",
					new ValuesDictionary
					{
						{
							"PlayerIndex",
							this.PlayerIndex
						}
					}
				},
				{
					"Intro",
					new ValuesDictionary
					{
						{
							"PlayIntro",
							CS$<>8__locals1.spawnMode == PlayerData.SpawnMode.InitialIntro
						}
					}
				},
				{
					"Clothing",
					new ValuesDictionary
					{
						{
							"Clothes",
							new ValuesDictionary
							{
								{
									"Feet",
									value4
								},
								{
									"Legs",
									value3
								},
								{
									"Torso",
									value2
								},
								{
									"Head",
									value
								}
							}
						}
					}
				}
			};
			Vector2 v = ComponentIntro.FindOceanDirection(this.m_subsystemTerrain.TerrainContentsGenerator, CS$<>8__locals1.position.XZ);
			string entityTemplateName = (this.PlayerClass == PlayerClass.Male) ? "MalePlayer" : "FemalePlayer";
			CS$<>8__locals1.entity2 = DatabaseManager.CreateEntity(this.m_project, entityTemplateName, overrides, true);
			CS$<>8__locals1.entity2.FindComponent<ComponentBody>(true).Position = CS$<>8__locals1.position;
			CS$<>8__locals1.entity2.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Vector2.Angle(v, -Vector2.UnitY));
			this.m_project.AddEntity(CS$<>8__locals1.entity2);
			if (componentMount != null)
			{
				CS$<>8__locals1.entity2.FindComponent<ComponentRider>(true).StartMounting(componentMount);
			}
			this.LastSpawnTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			int spawnsCount = this.SpawnsCount + 1;
			this.SpawnsCount = spawnsCount;
			ModsManager.HookAction("OnPlayerSpawned", (ModLoader modLoader) => modLoader.OnPlayerSpawned(CS$<>8__locals1.spawnMode, CS$<>8__locals1.entity2.FindComponent<ComponentPlayer>(), CS$<>8__locals1.position));
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000A67AE File Offset: 0x000A49AE
		public string GetEntityTemplateName()
		{
			if (this.PlayerClass != PlayerClass.Male)
			{
				return "FemalePlayer";
			}
			return "MalePlayer";
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x000A67C4 File Offset: 0x000A49C4
		public void UpdateSpawnDialog(string largeMessage, string smallMessage, float progress, bool resetProgress)
		{
			if (resetProgress)
			{
				this.m_progress = 0f;
			}
			this.m_progress = MathUtils.Max(progress, this.m_progress);
			if (this.m_spawnDialog == null)
			{
				this.m_spawnDialog = new SpawnDialog();
				DialogsManager.ShowDialog(this.GameWidget.GuiWidget, this.m_spawnDialog);
			}
			if (largeMessage != null)
			{
				this.m_spawnDialog.LargeMessage = largeMessage;
			}
			if (smallMessage != null)
			{
				this.m_spawnDialog.SmallMessage = smallMessage;
			}
			this.m_spawnDialog.Progress = this.m_progress;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x000A684A File Offset: 0x000A4A4A
		public void HideSpawnDialog()
		{
			if (this.m_spawnDialog != null)
			{
				DialogsManager.HideDialog(this.m_spawnDialog);
				this.m_spawnDialog = null;
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x000A6868 File Offset: 0x000A4A68
		public static string MakeClothingValue(int index, int color)
		{
			return Terrain.MakeBlockValue(203, 0, ClothingBlock.SetClothingIndex(ClothingBlock.SetClothingColor(0, color), index)).ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x04000E92 RID: 3730
		public static string fName = "PlayerData";

		// Token: 0x04000E93 RID: 3731
		public Project m_project;

		// Token: 0x04000E94 RID: 3732
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000E95 RID: 3733
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000E96 RID: 3734
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000E97 RID: 3735
		public GameWidget m_gameWidget;

		// Token: 0x04000E98 RID: 3736
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000E99 RID: 3737
		public PlayerClass m_playerClass;

		// Token: 0x04000E9A RID: 3738
		public string m_name;

		// Token: 0x04000E9B RID: 3739
		public PlayerData.SpawnMode m_spawnMode;

		// Token: 0x04000E9C RID: 3740
		public double? m_playerDeathTime;

		// Token: 0x04000E9D RID: 3741
		public double m_terrainWaitStartTime;

		// Token: 0x04000E9E RID: 3742
		public SpawnDialog m_spawnDialog;

		// Token: 0x04000E9F RID: 3743
		public float m_progress;

		// Token: 0x02000524 RID: 1316
		public enum SpawnMode
		{
			// Token: 0x0400189A RID: 6298
			InitialIntro,
			// Token: 0x0400189B RID: 6299
			InitialNoIntro,
			// Token: 0x0400189C RID: 6300
			Respawn
		}
	}
}
