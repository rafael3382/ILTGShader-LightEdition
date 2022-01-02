using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022D RID: 557
	public class ComponentSickness : Component, IUpdateable
	{
		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x000851A2 File Offset: 0x000833A2
		public bool IsSick
		{
			get
			{
				return this.m_sicknessDuration > 0f;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x000851B1 File Offset: 0x000833B1
		public bool IsPuking
		{
			get
			{
				return this.m_pukeParticleSystem != null;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x000851BC File Offset: 0x000833BC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x000851BF File Offset: 0x000833BF
		public virtual void StartSickness()
		{
			if (this.m_sicknessDuration == 0f)
			{
				this.m_componentPlayer.PlayerStats.TimesWasSick += 1L;
			}
			this.m_sicknessDuration = 900f;
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x000851F4 File Offset: 0x000833F4
		public virtual void NauseaEffect()
		{
			this.m_lastNauseaTime = new double?(this.m_subsystemTime.GameTime);
			this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
			float injury = MathUtils.Min(0.1f, this.m_componentPlayer.ComponentHealth.Health - 0.075f);
			if (injury > 0f)
			{
				this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.75, delegate
				{
					this.m_componentPlayer.ComponentHealth.Injure(injury, null, false, LanguageControl.Get(this.GetType().Name, 1));
				});
			}
			if (this.m_pukeParticleSystem == null)
			{
				if (this.m_lastPukeTime != null)
				{
					double? num = this.m_subsystemTime.GameTime - this.m_lastPukeTime;
					double num2 = 50.0;
					if (!(num.GetValueOrDefault() > num2 & num != null))
					{
						goto IL_19B;
					}
				}
				this.m_lastPukeTime = new double?(this.m_subsystemTime.GameTime);
				this.m_pukeParticleSystem = new PukeParticleSystem(this.m_subsystemTerrain);
				this.m_subsystemParticles.AddParticleSystem(this.m_pukeParticleSystem);
				this.m_componentPlayer.ComponentCreatureSounds.PlayPukeSound();
				base.Project.FindSubsystem<SubsystemNoise>(true).MakeNoise(this.m_componentPlayer.ComponentBody.Position, 0.25f, 10f);
				this.m_greenoutDuration = 0.8f;
				this.m_componentPlayer.PlayerStats.TimesPuked += 1L;
				return;
			}
			IL_19B:
			this.m_greenoutDuration = MathUtils.Lerp(4f, 2f, this.m_componentPlayer.ComponentHealth.Health);
			if (this.m_lastMessageTime != null)
			{
				double? num = Time.FrameStartTime - this.m_lastMessageTime;
				double num2 = 60.0;
				if (!(num.GetValueOrDefault() > num2 & num != null))
				{
					return;
				}
			}
			this.m_lastMessageTime = new double?(Time.FrameStartTime);
			this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 1.5, delegate
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(this.GetType().Name, 2), Color.White, true, true);
			});
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0008545C File Offset: 0x0008365C
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.m_sicknessDuration = 0f;
				return;
			}
			if (this.m_sicknessDuration > 0f)
			{
				this.m_sicknessDuration = MathUtils.Max(this.m_sicknessDuration - dt, 0f);
				if (this.m_componentPlayer.ComponentHealth.Health > 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping && this.m_subsystemTime.PeriodicGameTimeEvent(3.0, -0.0099999997764825821))
				{
					if (this.m_lastNauseaTime != null)
					{
						double? num = this.m_subsystemTime.GameTime - this.m_lastNauseaTime;
						double num2 = 15.0;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							goto IL_10E;
						}
					}
					this.NauseaEffect();
				}
			}
			IL_10E:
			if (this.m_pukeParticleSystem != null)
			{
				float num3 = MathUtils.DegToRad(MathUtils.Lerp(-35f, -60f, SimplexNoise.Noise(2f * (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 10000.0))));
				this.m_componentPlayer.ComponentLocomotion.LookOrder = new Vector2(this.m_componentPlayer.ComponentLocomotion.LookOrder.X, MathUtils.Clamp(num3 - this.m_componentPlayer.ComponentLocomotion.LookAngles.Y, -2f, 2f));
				Vector3 upVector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetUpVector();
				Vector3 forwardVector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector();
				this.m_pukeParticleSystem.Position = this.m_componentPlayer.ComponentCreatureModel.EyePosition - 0.08f * upVector + 0.3f * forwardVector;
				this.m_pukeParticleSystem.Direction = Vector3.Normalize(forwardVector + 0.5f * upVector);
				if (this.m_pukeParticleSystem.IsStopped)
				{
					this.m_pukeParticleSystem = null;
				}
			}
			if (this.m_greenoutDuration > 0f)
			{
				this.m_greenoutDuration = MathUtils.Max(this.m_greenoutDuration - dt, 0f);
				this.m_greenoutFactor = MathUtils.Min(this.m_greenoutFactor + 0.5f * dt, 0.95f);
			}
			else if (this.m_greenoutFactor > 0f)
			{
				this.m_greenoutFactor = MathUtils.Max(this.m_greenoutFactor - 0.5f * dt, 0f);
			}
			this.m_componentPlayer.ComponentScreenOverlays.GreenoutFactor = MathUtils.Max(this.m_greenoutFactor, this.m_componentPlayer.ComponentScreenOverlays.GreenoutFactor);
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x00085754 File Offset: 0x00083954
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_sicknessDuration = valuesDictionary.GetValue<float>("SicknessDuration");
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x000857CC File Offset: 0x000839CC
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("SicknessDuration", this.m_sicknessDuration);
		}

		// Token: 0x04000AE0 RID: 2784
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000AE1 RID: 2785
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AE2 RID: 2786
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000AE3 RID: 2787
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000AE4 RID: 2788
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000AE5 RID: 2789
		public PukeParticleSystem m_pukeParticleSystem;

		// Token: 0x04000AE6 RID: 2790
		public float m_sicknessDuration;

		// Token: 0x04000AE7 RID: 2791
		public float m_greenoutDuration;

		// Token: 0x04000AE8 RID: 2792
		public float m_greenoutFactor;

		// Token: 0x04000AE9 RID: 2793
		public double? m_lastNauseaTime;

		// Token: 0x04000AEA RID: 2794
		public double? m_lastMessageTime;

		// Token: 0x04000AEB RID: 2795
		public double? m_lastPukeTime;
	}
}
