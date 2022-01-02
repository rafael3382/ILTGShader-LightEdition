using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C6 RID: 454
	public class SubsystemPlayers : Subsystem, IUpdateable
	{
		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00053CFF File Offset: 0x00051EFF
		public ReadOnlyList<PlayerData> PlayersData
		{
			get
			{
				return new ReadOnlyList<PlayerData>(this.m_playersData);
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x00053D0C File Offset: 0x00051F0C
		public ReadOnlyList<ComponentPlayer> ComponentPlayers
		{
			get
			{
				return new ReadOnlyList<ComponentPlayer>(this.m_componentPlayers);
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00053D19 File Offset: 0x00051F19
		// (set) Token: 0x06000BF5 RID: 3061 RVA: 0x00053D21 File Offset: 0x00051F21
		public Vector3 GlobalSpawnPosition { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x00053D2A File Offset: 0x00051F2A
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.SubsystemPlayers;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x00053D2E File Offset: 0x00051F2E
		// (set) Token: 0x06000BF8 RID: 3064 RVA: 0x00053D36 File Offset: 0x00051F36
		public virtual Action<PlayerData> PlayerAdded { get; set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x00053D3F File Offset: 0x00051F3F
		// (set) Token: 0x06000BFA RID: 3066 RVA: 0x00053D47 File Offset: 0x00051F47
		public virtual Action<PlayerData> PlayerRemoved { get; set; }

		// Token: 0x06000BFB RID: 3067 RVA: 0x00053D50 File Offset: 0x00051F50
		public bool IsPlayer(Entity entity)
		{
			foreach (ComponentPlayer componentPlayer in this.m_componentPlayers)
			{
				if (entity == componentPlayer.Entity)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x00053DAC File Offset: 0x00051FAC
		public ComponentPlayer FindNearestPlayer(Vector3 position)
		{
			ComponentPlayer result = null;
			float num = float.MaxValue;
			foreach (ComponentPlayer componentPlayer in this.ComponentPlayers)
			{
				float num2 = Vector3.DistanceSquared(componentPlayer.ComponentBody.Position, position);
				if (num2 < num)
				{
					num = num2;
					result = componentPlayer;
				}
			}
			return result;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x00053E24 File Offset: 0x00052024
		public void AddPlayerData(PlayerData playerData)
		{
			if (this.m_playersData.Count >= SubsystemPlayers.MaxPlayers)
			{
				throw new InvalidOperationException("Too many players.");
			}
			if (this.m_playersData.Contains(playerData))
			{
				throw new InvalidOperationException("Player already added.");
			}
			this.m_playersData.Add(playerData);
			playerData.PlayerIndex = this.m_nextPlayerIndex - 1;
			this.m_nextPlayerIndex++;
			Action<PlayerData> playerAdded = this.PlayerAdded;
			if (playerAdded == null)
			{
				return;
			}
			playerAdded(playerData);
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x00053EA0 File Offset: 0x000520A0
		public void RemovePlayerData(PlayerData playerData)
		{
			if (!this.m_playersData.Contains(playerData))
			{
				throw new InvalidOperationException("Player does not exist.");
			}
			this.m_playersData.Remove(playerData);
			if (playerData.ComponentPlayer != null)
			{
				base.Project.RemoveEntity(playerData.ComponentPlayer.Entity, true);
			}
			Action<PlayerData> playerRemoved = this.PlayerRemoved;
			if (playerRemoved != null)
			{
				playerRemoved(playerData);
			}
			playerData.Dispose();
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x00053F0C File Offset: 0x0005210C
		public void Update(float dt)
		{
			if (this.m_playersData.Count == 0)
			{
				ScreensManager.SwitchScreen("Player", new object[]
				{
					PlayerScreen.Mode.Initial,
					base.Project
				});
			}
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.Update();
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x00053F8C File Offset: 0x0005218C
		public override void Dispose()
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.Dispose();
			}
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x00053FDC File Offset: 0x000521DC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_nextPlayerIndex = valuesDictionary.GetValue<int>("NextPlayerIndex");
			this.GlobalSpawnPosition = valuesDictionary.GetValue<Vector3>("GlobalSpawnPosition");
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Players"))
			{
				PlayerData playerData = new PlayerData(base.Project);
				playerData.Load((ValuesDictionary)keyValuePair.Value);
				playerData.PlayerIndex = int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture);
				this.m_playersData.Add(playerData);
			}
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0005409C File Offset: 0x0005229C
		public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<int>("NextPlayerIndex", this.m_nextPlayerIndex);
			valuesDictionary.SetValue<Vector3>("GlobalSpawnPosition", this.GlobalSpawnPosition);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Players", valuesDictionary2);
			foreach (PlayerData playerData in this.m_playersData)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(playerData.PlayerIndex.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				playerData.Save(valuesDictionary3);
			}
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x00054144 File Offset: 0x00052344
		public override void OnEntityAdded(Entity entity)
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.OnEntityAdded(entity);
			}
			this.UpdateComponentPlayers();
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0005419C File Offset: 0x0005239C
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.OnEntityRemoved(entity);
			}
			this.UpdateComponentPlayers();
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x000541F4 File Offset: 0x000523F4
		public void UpdateComponentPlayers()
		{
			this.m_componentPlayers.Clear();
			foreach (PlayerData playerData in this.m_playersData)
			{
				if (playerData.ComponentPlayer != null)
				{
					this.m_componentPlayers.Add(playerData.ComponentPlayer);
				}
			}
		}

		// Token: 0x040005EB RID: 1515
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005EC RID: 1516
		public List<PlayerData> m_playersData = new List<PlayerData>();

		// Token: 0x040005ED RID: 1517
		public List<ComponentPlayer> m_componentPlayers = new List<ComponentPlayer>();

		// Token: 0x040005EE RID: 1518
		public int m_nextPlayerIndex;

		// Token: 0x040005EF RID: 1519
		public static int MaxPlayers = 4;
	}
}
