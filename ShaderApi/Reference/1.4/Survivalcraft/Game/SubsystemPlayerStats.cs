using System;
using System.Collections.Generic;
using System.Globalization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C7 RID: 455
	public class SubsystemPlayerStats : Subsystem
	{
		// Token: 0x06000C08 RID: 3080 RVA: 0x0005428C File Offset: 0x0005248C
		public PlayerStats GetPlayerStats(int playerIndex)
		{
			PlayerStats playerStats;
			if (!this.m_playerStats.TryGetValue(playerIndex, out playerStats))
			{
				playerStats = new PlayerStats();
				this.m_playerStats.Add(playerIndex, playerStats);
			}
			return playerStats;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000542C0 File Offset: 0x000524C0
		public override void Load(ValuesDictionary valuesDictionary)
		{
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Stats"))
			{
				PlayerStats playerStats = new PlayerStats();
				playerStats.Load((ValuesDictionary)keyValuePair.Value);
				this.m_playerStats.Add(int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture), playerStats);
			}
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x00054340 File Offset: 0x00052540
		public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Stats", valuesDictionary2);
			foreach (KeyValuePair<int, PlayerStats> keyValuePair in this.m_playerStats)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(keyValuePair.Key.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				keyValuePair.Value.Save(valuesDictionary3);
			}
		}

		// Token: 0x040005F3 RID: 1523
		public Dictionary<int, PlayerStats> m_playerStats = new Dictionary<int, PlayerStats>();
	}
}
