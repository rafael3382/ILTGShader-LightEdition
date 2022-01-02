using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020003A2 RID: 930
	public class SubsystemGameWidgets : Subsystem, IUpdateable
	{
		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x000DB1FF File Offset: 0x000D93FF
		public int MaxGameWidgets
		{
			get
			{
				return SubsystemPlayers.MaxPlayers;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001C2B RID: 7211 RVA: 0x000DB206 File Offset: 0x000D9406
		// (set) Token: 0x06001C2C RID: 7212 RVA: 0x000DB20E File Offset: 0x000D940E
		public GamesWidget GamesWidget { get; set; }

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06001C2D RID: 7213 RVA: 0x000DB217 File Offset: 0x000D9417
		public ReadOnlyList<GameWidget> GameWidgets
		{
			get
			{
				return new ReadOnlyList<GameWidget>(this.m_gameWidgets);
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06001C2E RID: 7214 RVA: 0x000DB224 File Offset: 0x000D9424
		// (set) Token: 0x06001C2F RID: 7215 RVA: 0x000DB22C File Offset: 0x000D942C
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001C30 RID: 7216 RVA: 0x000DB235 File Offset: 0x000D9435
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Views;
			}
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x000DB23C File Offset: 0x000D943C
		public float CalculateSquaredDistanceFromNearestView(Vector3 p)
		{
			float num = float.MaxValue;
			foreach (GameWidget gameWidget in this.m_gameWidgets)
			{
				float num2 = Vector3.DistanceSquared(p, gameWidget.ActiveCamera.ViewPosition);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x000DB2A8 File Offset: 0x000D94A8
		public float CalculateDistanceFromNearestView(Vector3 p)
		{
			return MathUtils.Sqrt(this.CalculateSquaredDistanceFromNearestView(p));
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x000DB2B8 File Offset: 0x000D94B8
		public void Update(float dt)
		{
			foreach (GameWidget gameWidget in this.GameWidgets)
			{
				gameWidget.ActiveCamera.Update(Time.FrameDuration);
			}
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000DB318 File Offset: 0x000D9518
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			SubsystemPlayers subsystemPlayers = this.m_subsystemPlayers;
			subsystemPlayers.PlayerAdded = (Action<PlayerData>)Delegate.Combine(subsystemPlayers.PlayerAdded, new Action<PlayerData>(delegate(PlayerData playerData)
			{
				this.AddGameWidgetForPlayer(playerData);
			}));
			SubsystemPlayers subsystemPlayers2 = this.m_subsystemPlayers;
			subsystemPlayers2.PlayerRemoved = (Action<PlayerData>)Delegate.Combine(subsystemPlayers2.PlayerRemoved, new Action<PlayerData>(delegate(PlayerData playerData)
			{
				if (playerData.GameWidget != null)
				{
					this.RemoveGameWidget(playerData.GameWidget);
				}
			}));
			this.GamesWidget = valuesDictionary.GetValue<GamesWidget>("GamesWidget");
			foreach (PlayerData playerData2 in this.m_subsystemPlayers.PlayersData)
			{
				this.AddGameWidgetForPlayer(playerData2);
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000DB3F8 File Offset: 0x000D95F8
		public override void Dispose()
		{
			foreach (GameWidget gameWidget in this.GameWidgets.ToArray<GameWidget>())
			{
				this.RemoveGameWidget(gameWidget);
				gameWidget.Dispose();
			}
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x000DB438 File Offset: 0x000D9638
		public void AddGameWidgetForPlayer(PlayerData playerData)
		{
			int index = 0;
			while (index < this.MaxGameWidgets && this.m_gameWidgets.FirstOrDefault((GameWidget v) => v.GameWidgetIndex == index) != null)
			{
				int index2 = index;
				index = index2 + 1;
			}
			if (index >= this.MaxGameWidgets)
			{
				throw new InvalidOperationException("Too many GameWidgets.");
			}
			GameWidget gameWidget = new GameWidget(playerData, playerData.PlayerIndex);
			this.m_gameWidgets.Add(gameWidget);
			this.GamesWidget.Children.Add(gameWidget);
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000DB4CE File Offset: 0x000D96CE
		public void RemoveGameWidget(GameWidget gameWidget)
		{
			this.m_gameWidgets.Remove(gameWidget);
			this.GamesWidget.Children.Remove(gameWidget);
		}

		// Token: 0x0400130E RID: 4878
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x0400130F RID: 4879
		public List<GameWidget> m_gameWidgets = new List<GameWidget>();
	}
}
