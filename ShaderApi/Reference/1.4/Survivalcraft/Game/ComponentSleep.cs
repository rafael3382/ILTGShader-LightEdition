using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022F RID: 559
	public class ComponentSleep : Component, IUpdateable
	{
		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x0008591E File Offset: 0x00083B1E
		public bool IsSleeping
		{
			get
			{
				return this.m_sleepStartTime != null;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060011F6 RID: 4598 RVA: 0x0008592B File Offset: 0x00083B2B
		public float SleepFactor
		{
			get
			{
				return this.m_sleepFactor;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x00085933 File Offset: 0x00083B33
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x00085938 File Offset: 0x00083B38
		public virtual bool CanSleep(out string reason)
		{
			Block block = (this.m_componentPlayer.ComponentBody.StandingOnValue != null) ? BlocksManager.Blocks[Terrain.ExtractContents(this.m_componentPlayer.ComponentBody.StandingOnValue.Value)] : null;
			if (block == null || this.m_componentPlayer.ComponentBody.ImmersionDepth > 0f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 1);
				return false;
			}
			if (block != null && block.SleepSuitability == 0f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 2);
				return false;
			}
			if (this.m_componentPlayer.ComponentVitalStats.Sleep > 0.99f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 3);
				return false;
			}
			if (this.m_componentPlayer.ComponentVitalStats.Wetness > 0.95f)
			{
				reason = LanguageControl.Get(ComponentSleep.fName, 4);
				return false;
			}
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					Vector3 vector = this.m_componentPlayer.ComponentBody.Position + new Vector3((float)i, 1f, (float)j);
					Vector3 end = new Vector3(vector.X, 255f, vector.Z);
					if (this.m_subsystemTerrain.Raycast(vector, end, false, true, (int value, float distance) => Terrain.ExtractContents(value) != 0) == null)
					{
						reason = LanguageControl.Get(ComponentSleep.fName, 5);
						return false;
					}
				}
			}
			reason = string.Empty;
			return true;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x00085AD0 File Offset: 0x00083CD0
		public virtual void Sleep(bool allowManualWakeup)
		{
			if (!this.IsSleeping)
			{
				this.m_sleepStartTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
				this.m_allowManualWakeUp = allowManualWakeup;
				this.m_minWetness = float.MaxValue;
				this.m_messageFactor = 0f;
				if (this.m_componentPlayer.PlayerStats != null)
				{
					this.m_componentPlayer.PlayerStats.TimesWentToSleep += 1L;
				}
			}
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00085B40 File Offset: 0x00083D40
		public virtual void WakeUp()
		{
			if (this.m_sleepStartTime != null)
			{
				this.m_sleepStartTime = null;
				this.m_componentPlayer.PlayerData.SpawnPosition = this.m_componentPlayer.ComponentBody.Position + new Vector3(0f, 0.1f, 0f);
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x00085BA0 File Offset: 0x00083DA0
		public void Update(float dt)
		{
			if (this.IsSleeping && this.m_componentPlayer.ComponentHealth.Health > 0f)
			{
				this.m_sleepFactor = MathUtils.Min(this.m_sleepFactor + 0.33f * Time.FrameDuration, 1f);
				this.m_minWetness = MathUtils.Min(this.m_minWetness, this.m_componentPlayer.ComponentVitalStats.Wetness);
				this.m_componentPlayer.PlayerStats.TimeSlept += (double)this.m_subsystemGameInfo.TotalElapsedGameTimeDelta;
				if ((this.m_componentPlayer.ComponentVitalStats.Sleep >= 1f || this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) && this.m_subsystemTimeOfDay.TimeOfDay > 0.3f && this.m_subsystemTimeOfDay.TimeOfDay < 0.599999964f && this.m_sleepStartTime != null)
				{
					double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
					double? num = this.m_sleepStartTime + 180.0;
					if (totalElapsedGameTime > num.GetValueOrDefault() & num != null)
					{
						this.WakeUp();
					}
				}
				if (this.m_componentPlayer.ComponentHealth.HealthChange < 0f && (this.m_componentPlayer.ComponentHealth.Health < 0.5f || this.m_componentPlayer.ComponentVitalStats.Sleep > 0.5f))
				{
					this.WakeUp();
				}
				if (this.m_componentPlayer.ComponentVitalStats.Wetness > this.m_minWetness + 0.05f && this.m_componentPlayer.ComponentVitalStats.Sleep > 0.2f)
				{
					this.WakeUp();
					this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.0, delegate
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentSleep.fName, 6), Color.White, true, true);
					});
				}
				if (this.m_sleepStartTime != null)
				{
					float num2 = (float)(this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_sleepStartTime.Value);
					if (this.m_allowManualWakeUp && num2 > 10f)
					{
						if (this.m_componentPlayer.GameWidget.Input.Any && !DialogsManager.HasDialogs(this.m_componentPlayer.GameWidget))
						{
							this.m_componentPlayer.GameWidget.Input.Clear();
							this.WakeUp();
							this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 2.0, delegate
							{
								this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentSleep.fName, 7), Color.White, true, false);
							});
						}
						this.m_messageFactor = MathUtils.Min(this.m_messageFactor + 0.5f * Time.FrameDuration, 1f);
						this.m_componentPlayer.ComponentScreenOverlays.Message = LanguageControl.Get(ComponentSleep.fName, 8);
						this.m_componentPlayer.ComponentScreenOverlays.MessageFactor = this.m_messageFactor;
					}
					if (!this.m_allowManualWakeUp && num2 > 5f)
					{
						this.m_messageFactor = MathUtils.Min(this.m_messageFactor + 1f * Time.FrameDuration, 1f);
						this.m_componentPlayer.ComponentScreenOverlays.Message = LanguageControl.Get(ComponentSleep.fName, 9);
						this.m_componentPlayer.ComponentScreenOverlays.MessageFactor = this.m_messageFactor;
					}
				}
			}
			else
			{
				this.m_sleepFactor = MathUtils.Max(this.m_sleepFactor - 1f * Time.FrameDuration, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor, this.m_sleepFactor);
			if (this.m_sleepFactor > 0.01f)
			{
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentSleep.fName, 10);
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_sleepFactor - 0.9f));
			}
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00085FAC File Offset: 0x000841AC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemUpdate = base.Project.FindSubsystem<SubsystemUpdate>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_sleepStartTime = new double?(valuesDictionary.GetValue<double>("SleepStartTime"));
			this.m_allowManualWakeUp = valuesDictionary.GetValue<bool>("AllowManualWakeUp");
			ComponentHealth componentHealth = this.m_componentPlayer.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature attacker)
			{
				if (this.IsSleeping && this.m_componentPlayer.ComponentVitalStats.Sleep > 0.25f)
				{
					this.WakeUp();
				}
			}));
			double? sleepStartTime = this.m_sleepStartTime;
			double num = 0.0;
			if (sleepStartTime.GetValueOrDefault() == num & sleepStartTime != null)
			{
				this.m_sleepStartTime = null;
			}
			if (this.m_sleepStartTime != null)
			{
				this.m_sleepFactor = 1f;
				this.m_minWetness = float.MaxValue;
			}
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x000860E0 File Offset: 0x000842E0
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("SleepStartTime", (this.m_sleepStartTime != null) ? this.m_sleepStartTime.Value : 0.0);
			valuesDictionary.SetValue<bool>("AllowManualWakeUp", this.m_allowManualWakeUp);
		}

		// Token: 0x04000AEE RID: 2798
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x04000AEF RID: 2799
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000AF0 RID: 2800
		public SubsystemUpdate m_subsystemUpdate;

		// Token: 0x04000AF1 RID: 2801
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000AF2 RID: 2802
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000AF3 RID: 2803
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AF4 RID: 2804
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000AF5 RID: 2805
		public double? m_sleepStartTime;

		// Token: 0x04000AF6 RID: 2806
		public float m_sleepFactor;

		// Token: 0x04000AF7 RID: 2807
		public bool m_allowManualWakeUp;

		// Token: 0x04000AF8 RID: 2808
		public static string fName = "ComponentSleep";

		// Token: 0x04000AF9 RID: 2809
		public float m_minWetness;

		// Token: 0x04000AFA RID: 2810
		public float m_messageFactor;
	}
}
