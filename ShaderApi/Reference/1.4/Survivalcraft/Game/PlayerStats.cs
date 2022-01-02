using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020002E4 RID: 740
	public class PlayerStats
	{
		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001635 RID: 5685 RVA: 0x000A6EFA File Offset: 0x000A50FA
		public IEnumerable<FieldInfo> Stats
		{
			get
			{
				foreach (FieldInfo fieldInfo in from f in typeof(PlayerStats).GetRuntimeFields()
				where f.GetCustomAttribute<PlayerStats.StatAttribute>() != null
				select f)
				{
					yield return fieldInfo;
				}
				IEnumerator<FieldInfo> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001636 RID: 5686 RVA: 0x000A6F03 File Offset: 0x000A5103
		public ReadOnlyList<PlayerStats.DeathRecord> DeathRecords
		{
			get
			{
				return new ReadOnlyList<PlayerStats.DeathRecord>(this.m_deathRecords);
			}
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x000A6F10 File Offset: 0x000A5110
		public void AddDeathRecord(PlayerStats.DeathRecord deathRecord)
		{
			this.m_deathRecords.Add(deathRecord);
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x000A6F20 File Offset: 0x000A5120
		public void Load(ValuesDictionary valuesDictionary)
		{
			foreach (FieldInfo fieldInfo in this.Stats)
			{
				if (valuesDictionary.ContainsKey(fieldInfo.Name))
				{
					object value = valuesDictionary.GetValue<object>(fieldInfo.Name);
					fieldInfo.SetValue(this, value);
				}
			}
			if (!string.IsNullOrEmpty(this.DeathRecordsString))
			{
				foreach (string s in this.DeathRecordsString.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					PlayerStats.DeathRecord item = default(PlayerStats.DeathRecord);
					item.Load(s);
					this.m_deathRecords.Add(item);
				}
			}
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x000A6FE4 File Offset: 0x000A51E4
		public void Save(ValuesDictionary valuesDictionary)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PlayerStats.DeathRecord deathRecord in this.m_deathRecords)
			{
				stringBuilder.Append(deathRecord.Save());
				stringBuilder.Append(';');
			}
			this.DeathRecordsString = stringBuilder.ToString();
			foreach (FieldInfo fieldInfo in this.Stats)
			{
				object value = fieldInfo.GetValue(this);
				valuesDictionary.SetValue<object>(fieldInfo.Name, value);
			}
		}

		// Token: 0x04000ECA RID: 3786
		public List<PlayerStats.DeathRecord> m_deathRecords = new List<PlayerStats.DeathRecord>();

		// Token: 0x04000ECB RID: 3787
		[PlayerStats.StatAttribute]
		public double DistanceTravelled;

		// Token: 0x04000ECC RID: 3788
		[PlayerStats.StatAttribute]
		public double DistanceWalked;

		// Token: 0x04000ECD RID: 3789
		[PlayerStats.StatAttribute]
		public double DistanceFallen;

		// Token: 0x04000ECE RID: 3790
		[PlayerStats.StatAttribute]
		public double DistanceClimbed;

		// Token: 0x04000ECF RID: 3791
		[PlayerStats.StatAttribute]
		public double DistanceFlown;

		// Token: 0x04000ED0 RID: 3792
		[PlayerStats.StatAttribute]
		public double DistanceSwam;

		// Token: 0x04000ED1 RID: 3793
		[PlayerStats.StatAttribute]
		public double DistanceRidden;

		// Token: 0x04000ED2 RID: 3794
		[PlayerStats.StatAttribute]
		public double LowestAltitude = double.PositiveInfinity;

		// Token: 0x04000ED3 RID: 3795
		[PlayerStats.StatAttribute]
		public double HighestAltitude = double.NegativeInfinity;

		// Token: 0x04000ED4 RID: 3796
		[PlayerStats.StatAttribute]
		public double DeepestDive;

		// Token: 0x04000ED5 RID: 3797
		[PlayerStats.StatAttribute]
		public long Jumps;

		// Token: 0x04000ED6 RID: 3798
		[PlayerStats.StatAttribute]
		public long BlocksDug;

		// Token: 0x04000ED7 RID: 3799
		[PlayerStats.StatAttribute]
		public long BlocksPlaced;

		// Token: 0x04000ED8 RID: 3800
		[PlayerStats.StatAttribute]
		public long BlocksInteracted;

		// Token: 0x04000ED9 RID: 3801
		[PlayerStats.StatAttribute]
		public long PlayerKills;

		// Token: 0x04000EDA RID: 3802
		[PlayerStats.StatAttribute]
		public long LandCreatureKills;

		// Token: 0x04000EDB RID: 3803
		[PlayerStats.StatAttribute]
		public long WaterCreatureKills;

		// Token: 0x04000EDC RID: 3804
		[PlayerStats.StatAttribute]
		public long AirCreatureKills;

		// Token: 0x04000EDD RID: 3805
		[PlayerStats.StatAttribute]
		public long MeleeAttacks;

		// Token: 0x04000EDE RID: 3806
		[PlayerStats.StatAttribute]
		public long MeleeHits;

		// Token: 0x04000EDF RID: 3807
		[PlayerStats.StatAttribute]
		public long RangedAttacks;

		// Token: 0x04000EE0 RID: 3808
		[PlayerStats.StatAttribute]
		public long RangedHits;

		// Token: 0x04000EE1 RID: 3809
		[PlayerStats.StatAttribute]
		public long HitsReceived;

		// Token: 0x04000EE2 RID: 3810
		[PlayerStats.StatAttribute]
		public long StruckByLightning;

		// Token: 0x04000EE3 RID: 3811
		[PlayerStats.StatAttribute]
		public double TotalHealthLost;

		// Token: 0x04000EE4 RID: 3812
		[PlayerStats.StatAttribute]
		public long FoodItemsEaten;

		// Token: 0x04000EE5 RID: 3813
		[PlayerStats.StatAttribute]
		public long TimesWasSick;

		// Token: 0x04000EE6 RID: 3814
		[PlayerStats.StatAttribute]
		public long TimesHadFlu;

		// Token: 0x04000EE7 RID: 3815
		[PlayerStats.StatAttribute]
		public long TimesPuked;

		// Token: 0x04000EE8 RID: 3816
		[PlayerStats.StatAttribute]
		public long TimesWentToSleep;

		// Token: 0x04000EE9 RID: 3817
		[PlayerStats.StatAttribute]
		public double TimeSlept;

		// Token: 0x04000EEA RID: 3818
		[PlayerStats.StatAttribute]
		public long ItemsCrafted;

		// Token: 0x04000EEB RID: 3819
		[PlayerStats.StatAttribute]
		public long FurnitureItemsMade;

		// Token: 0x04000EEC RID: 3820
		[PlayerStats.StatAttribute]
		public GameMode EasiestModeUsed = (GameMode)2147483647;

		// Token: 0x04000EED RID: 3821
		[PlayerStats.StatAttribute]
		public float HighestLevel;

		// Token: 0x04000EEE RID: 3822
		[PlayerStats.StatAttribute]
		public string DeathRecordsString;

		// Token: 0x02000527 RID: 1319
		public class StatAttribute : Attribute
		{
		}

		// Token: 0x02000528 RID: 1320
		public struct DeathRecord
		{
			// Token: 0x060021FF RID: 8703 RVA: 0x000EBA08 File Offset: 0x000E9C08
			public void Load(string s)
			{
				string[] array = s.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 5)
				{
					throw new InvalidOperationException("Invalid death record.");
				}
				this.Day = double.Parse(array[0], CultureInfo.InvariantCulture);
				this.Location.X = float.Parse(array[1], CultureInfo.InvariantCulture);
				this.Location.Y = float.Parse(array[2], CultureInfo.InvariantCulture);
				this.Location.Z = float.Parse(array[3], CultureInfo.InvariantCulture);
				this.Cause = array[4];
			}

			// Token: 0x06002200 RID: 8704 RVA: 0x000EBAA0 File Offset: 0x000E9CA0
			public string Save()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.Day.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.X.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.Y.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Location.Z.ToString("R", CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(this.Cause);
				return stringBuilder.ToString();
			}

			// Token: 0x040018A4 RID: 6308
			public double Day;

			// Token: 0x040018A5 RID: 6309
			public Vector3 Location;

			// Token: 0x040018A6 RID: 6310
			public string Cause;
		}
	}
}
