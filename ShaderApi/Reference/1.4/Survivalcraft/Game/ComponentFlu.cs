using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000206 RID: 518
	public class ComponentFlu : Component, IUpdateable
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0006FE7F File Offset: 0x0006E07F
		public bool HasFlu
		{
			get
			{
				return this.m_fluDuration > 0f;
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0006FE8E File Offset: 0x0006E08E
		public bool IsCoughing
		{
			get
			{
				return this.m_coughDuration > 0f;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0006FE9D File Offset: 0x0006E09D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0006FEA0 File Offset: 0x0006E0A0
		public virtual void StartFlu()
		{
			if (this.m_fluDuration == 0f)
			{
				this.m_componentPlayer.PlayerStats.TimesHadFlu += 1L;
			}
			this.m_fluDuration = 900f;
			this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 10.0, delegate
			{
				this.m_componentPlayer.ComponentVitalStats.MakeSleepy(0.2f);
			});
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0006FF0C File Offset: 0x0006E10C
		public virtual void Sneeze()
		{
			this.m_sneezeDuration = 1f;
			this.m_componentPlayer.ComponentCreatureSounds.PlaySneezeSound();
			base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0006FF60 File Offset: 0x0006E160
		public virtual void Cough()
		{
			this.m_lastCoughTime = this.m_subsystemTime.GameTime;
			this.m_coughDuration = 4f;
			this.m_componentPlayer.ComponentCreatureSounds.PlayCoughSound();
			base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0006FFC4 File Offset: 0x0006E1C4
		public virtual void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.m_fluDuration = 0f;
				this.m_fluOnset = 0f;
				return;
			}
			if (this.m_fluDuration > 0f)
			{
				this.m_fluOnset = 0f;
				float num = 1f;
				if (this.m_componentPlayer.ComponentVitalStats.Temperature > 16f)
				{
					num = 2f;
				}
				else if (this.m_componentPlayer.ComponentVitalStats.Temperature > 12f)
				{
					num = 1.5f;
				}
				else if (this.m_componentPlayer.ComponentVitalStats.Temperature < 8f)
				{
					num = 0.5f;
				}
				this.m_fluDuration = MathUtils.Max(this.m_fluDuration - num * dt, 0f);
				if (this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping && this.m_subsystemTime.PeriodicGameTimeEvent(5.0, -0.0099999997764825821) && this.m_subsystemTime.GameTime - this.m_lastEffectTime > 13.0)
				{
					this.FluEffect();
				}
			}
			else if (this.m_componentPlayer.ComponentVitalStats.Temperature < 6f)
			{
				float num2 = 13f;
				this.m_fluOnset += dt;
				if (this.m_fluOnset > 120f)
				{
					num2 = 9f;
					if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0) && this.m_random.Bool(0.025f))
					{
						this.StartFlu();
					}
					if (this.m_subsystemTime.GameTime - this.m_lastMessageTime > 60.0)
					{
						this.m_lastMessageTime = this.m_subsystemTime.GameTime;
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 1), Color.White, true, true);
					}
				}
				if (this.m_fluOnset > 60f && this.m_subsystemTime.PeriodicGameTimeEvent((double)num2, -0.0099999997764825821) && this.m_random.Bool(0.75f))
				{
					this.Sneeze();
				}
			}
			else
			{
				this.m_fluOnset = 0f;
			}
			if ((this.m_coughDuration > 0f || this.m_sneezeDuration > 0f) && this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping)
			{
				this.m_coughDuration = MathUtils.Max(this.m_coughDuration - dt, 0f);
				this.m_sneezeDuration = MathUtils.Max(this.m_sneezeDuration - dt, 0f);
				float num3 = MathUtils.DegToRad(MathUtils.Lerp(-35f, -65f, SimplexNoise.Noise(4f * (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 10000.0))));
				this.m_componentPlayer.ComponentLocomotion.LookOrder = new Vector2(this.m_componentPlayer.ComponentLocomotion.LookOrder.X, MathUtils.Clamp(num3 - this.m_componentPlayer.ComponentLocomotion.LookAngles.Y, -3f, 3f));
				if (this.m_random.Bool(2f * dt))
				{
					this.m_componentPlayer.ComponentBody.ApplyImpulse(-1.2f * this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector());
				}
			}
			if (this.m_blackoutDuration > 0f)
			{
				this.m_blackoutDuration = MathUtils.Max(this.m_blackoutDuration - dt, 0f);
				this.m_blackoutFactor = MathUtils.Min(this.m_blackoutFactor + 0.5f * dt, 0.95f);
			}
			else if (this.m_blackoutFactor > 0f)
			{
				this.m_blackoutFactor = MathUtils.Max(this.m_blackoutFactor - 0.5f * dt, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_blackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0007041C File Offset: 0x0006E61C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_fluDuration = valuesDictionary.GetValue<float>("FluDuration");
			this.m_fluOnset = valuesDictionary.GetValue<float>("FluOnset");
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x000704B7 File Offset: 0x0006E6B7
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("FluDuration", this.m_fluDuration);
			valuesDictionary.SetValue<float>("FluOnset", this.m_fluOnset);
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x000704DC File Offset: 0x0006E6DC
		public virtual void FluEffect()
		{
			this.m_lastEffectTime = this.m_subsystemTime.GameTime;
			this.m_blackoutDuration = MathUtils.Lerp(4f, 2f, this.m_componentPlayer.ComponentHealth.Health);
			float injury = MathUtils.Min(0.1f, this.m_componentPlayer.ComponentHealth.Health - 0.175f);
			if (injury > 0f)
			{
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.75, delegate
				{
					this.m_componentPlayer.ComponentHealth.Injure(injury, null, false, LanguageControl.Get(ComponentFlu.fName, 4));
				});
			}
			if (Time.FrameStartTime - this.m_lastMessageTime > 60.0)
			{
				this.m_lastMessageTime = Time.FrameStartTime;
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.5, delegate
				{
					if (this.m_componentPlayer.ComponentVitalStats.Temperature < 8f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 2), Color.White, true, true);
						return;
					}
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentFlu.fName, 3), Color.White, true, true);
				});
			}
			if (this.m_coughDuration == 0f && (this.m_subsystemTime.GameTime - this.m_lastCoughTime > 40.0 || this.m_random.Bool(0.5f)))
			{
				this.Cough();
				return;
			}
			if (this.m_sneezeDuration == 0f)
			{
				this.Sneeze();
			}
		}

		// Token: 0x04000892 RID: 2194
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000893 RID: 2195
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000894 RID: 2196
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000895 RID: 2197
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000896 RID: 2198
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000897 RID: 2199
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000898 RID: 2200
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000899 RID: 2201
		public float m_fluOnset;

		// Token: 0x0400089A RID: 2202
		public static string fName = "ComponentFlu";

		// Token: 0x0400089B RID: 2203
		public float m_fluDuration;

		// Token: 0x0400089C RID: 2204
		public float m_coughDuration;

		// Token: 0x0400089D RID: 2205
		public float m_sneezeDuration;

		// Token: 0x0400089E RID: 2206
		public float m_blackoutDuration;

		// Token: 0x0400089F RID: 2207
		public float m_blackoutFactor;

		// Token: 0x040008A0 RID: 2208
		public double m_lastEffectTime = -1000.0;

		// Token: 0x040008A1 RID: 2209
		public double m_lastCoughTime = -1000.0;

		// Token: 0x040008A2 RID: 2210
		public double m_lastMessageTime = -1000.0;
	}
}
