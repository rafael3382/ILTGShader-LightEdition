using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DB RID: 475
	public class SubsystemTimeOfDay : Subsystem
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0005D450 File Offset: 0x0005B650
		public float TimeOfDay
		{
			get
			{
				if (!this.TimeOfDayEnabled)
				{
					return 0.5f;
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Changing)
				{
					return this.CalculateTimeOfDay(this.m_subsystemGameInfo.TotalElapsedGameTime);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Day)
				{
					return MathUtils.Remainder(0.5f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Night)
				{
					return MathUtils.Remainder(1f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Sunrise)
				{
					return MathUtils.Remainder(0.25f, 1f);
				}
				if (this.m_subsystemGameInfo.WorldSettings.TimeOfDayMode == TimeOfDayMode.Sunset)
				{
					return MathUtils.Remainder(0.75f, 1f);
				}
				return 0.5f;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000CF0 RID: 3312 RVA: 0x0005D523 File Offset: 0x0005B723
		public double Day
		{
			get
			{
				return this.CalculateDay(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x0005D536 File Offset: 0x0005B736
		// (set) Token: 0x06000CF2 RID: 3314 RVA: 0x0005D53E File Offset: 0x0005B73E
		public double TimeOfDayOffset { get; set; }

		// Token: 0x06000CF3 RID: 3315 RVA: 0x0005D547 File Offset: 0x0005B747
		public double CalculateDay(double totalElapsedGameTime)
		{
			return (totalElapsedGameTime + (this.TimeOfDayOffset + 0.30000001192092896) * 1200.0) / 1200.0;
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0005D56F File Offset: 0x0005B76F
		public float CalculateTimeOfDay(double totalElapsedGameTime)
		{
			return (float)MathUtils.Remainder(totalElapsedGameTime + (this.TimeOfDayOffset + 0.30000001192092896) * 1200.0, 1200.0) / 1200f;
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0005D5A2 File Offset: 0x0005B7A2
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.TimeOfDayOffset = valuesDictionary.GetValue<double>("TimeOfDayOffset");
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0005D5C7 File Offset: 0x0005B7C7
		public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<double>("TimeOfDayOffset", this.TimeOfDayOffset);
		}

		// Token: 0x040006AC RID: 1708
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040006AD RID: 1709
		public bool TimeOfDayEnabled = true;

		// Token: 0x040006AE RID: 1710
		public const float DayDuration = 1200f;
	}
}
