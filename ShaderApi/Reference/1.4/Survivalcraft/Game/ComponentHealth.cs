using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020F RID: 527
	public class ComponentHealth : Component, IUpdateable
	{
		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000FBD RID: 4029 RVA: 0x00074AE2 File Offset: 0x00072CE2
		// (set) Token: 0x06000FBE RID: 4030 RVA: 0x00074AEA File Offset: 0x00072CEA
		public string CauseOfDeath { get; set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000FBF RID: 4031 RVA: 0x00074AF3 File Offset: 0x00072CF3
		// (set) Token: 0x06000FC0 RID: 4032 RVA: 0x00074AFB File Offset: 0x00072CFB
		public bool IsInvulnerable { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x00074B04 File Offset: 0x00072D04
		// (set) Token: 0x06000FC2 RID: 4034 RVA: 0x00074B0C File Offset: 0x00072D0C
		public float Health { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x00074B15 File Offset: 0x00072D15
		// (set) Token: 0x06000FC4 RID: 4036 RVA: 0x00074B1D File Offset: 0x00072D1D
		public float HealthChange { get; set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x00074B26 File Offset: 0x00072D26
		// (set) Token: 0x06000FC6 RID: 4038 RVA: 0x00074B2E File Offset: 0x00072D2E
		public BreathingMode BreathingMode { get; set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x00074B37 File Offset: 0x00072D37
		// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x00074B3F File Offset: 0x00072D3F
		public float Air { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x00074B48 File Offset: 0x00072D48
		// (set) Token: 0x06000FCA RID: 4042 RVA: 0x00074B50 File Offset: 0x00072D50
		public float AirCapacity { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x00074B59 File Offset: 0x00072D59
		// (set) Token: 0x06000FCC RID: 4044 RVA: 0x00074B61 File Offset: 0x00072D61
		public bool CanStrand { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000FCD RID: 4045 RVA: 0x00074B6A File Offset: 0x00072D6A
		// (set) Token: 0x06000FCE RID: 4046 RVA: 0x00074B72 File Offset: 0x00072D72
		public float AttackResilience { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000FCF RID: 4047 RVA: 0x00074B7B File Offset: 0x00072D7B
		// (set) Token: 0x06000FD0 RID: 4048 RVA: 0x00074B83 File Offset: 0x00072D83
		public float FallResilience { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x00074B8C File Offset: 0x00072D8C
		// (set) Token: 0x06000FD2 RID: 4050 RVA: 0x00074B94 File Offset: 0x00072D94
		public float FireResilience { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x00074B9D File Offset: 0x00072D9D
		// (set) Token: 0x06000FD4 RID: 4052 RVA: 0x00074BA5 File Offset: 0x00072DA5
		public double? DeathTime { get; set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00074BAE File Offset: 0x00072DAE
		// (set) Token: 0x06000FD6 RID: 4054 RVA: 0x00074BB6 File Offset: 0x00072DB6
		public float CorpseDuration { get; set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x00074BBF File Offset: 0x00072DBF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x00074BC2 File Offset: 0x00072DC2
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x00074BCA File Offset: 0x00072DCA
		public virtual Action<ComponentCreature> Attacked { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x00074BD3 File Offset: 0x00072DD3
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x00074BDB File Offset: 0x00072DDB
		public virtual Action<ComponentCreature> Injured { get; set; }

		// Token: 0x06000FDC RID: 4060 RVA: 0x00074BE4 File Offset: 0x00072DE4
		public virtual void Heal(float amount)
		{
			if (amount > 0f)
			{
				this.Health = MathUtils.Min(this.Health + amount, 1f);
			}
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00074C08 File Offset: 0x00072E08
		public virtual void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability, string cause)
		{
			if (amount <= 0f || (!ignoreInvulnerability && this.IsInvulnerable))
			{
				return;
			}
			if (this.Health > 0f)
			{
				if (this.m_componentCreature.PlayerStats != null)
				{
					if (attacker != null)
					{
						this.m_componentCreature.PlayerStats.HitsReceived += 1L;
					}
					this.m_componentCreature.PlayerStats.TotalHealthLost += (double)MathUtils.Min(amount, this.Health);
				}
				this.Health = MathUtils.Max(this.Health - amount, 0f);
				if (this.Health <= 0f)
				{
					this.CauseOfDeath = cause;
					if (this.m_componentCreature.PlayerStats != null)
					{
						this.m_componentCreature.PlayerStats.AddDeathRecord(new PlayerStats.DeathRecord
						{
							Day = this.m_subsystemTimeOfDay.Day,
							Location = this.m_componentCreature.ComponentBody.Position,
							Cause = cause
						});
					}
					if (attacker != null)
					{
						ComponentPlayer componentPlayer = attacker.Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer != null)
						{
							if (this.m_componentPlayer != null)
							{
								componentPlayer.PlayerStats.PlayerKills += 1L;
							}
							else if (this.m_componentCreature.Category == CreatureCategory.LandPredator || this.m_componentCreature.Category == CreatureCategory.LandOther)
							{
								componentPlayer.PlayerStats.LandCreatureKills += 1L;
							}
							else if (this.m_componentCreature.Category == CreatureCategory.WaterPredator || this.m_componentCreature.Category == CreatureCategory.WaterOther)
							{
								componentPlayer.PlayerStats.WaterCreatureKills += 1L;
							}
							else
							{
								componentPlayer.PlayerStats.AirCreatureKills += 1L;
							}
							int num = (int)MathUtils.Ceiling(this.m_componentCreature.ComponentHealth.AttackResilience / 12f);
							for (int i = 0; i < num; i++)
							{
								Vector2 vector = this.m_random.Vector2(2.5f, 3.5f);
								this.m_subsystemPickables.AddPickable(248, 1, this.m_componentCreature.ComponentBody.Position, new Vector3?(new Vector3(vector.X, 6f, vector.Y)), null);
							}
						}
					}
				}
			}
			if (attacker != null)
			{
				Action<ComponentCreature> attacked = this.Attacked;
				if (attacked != null)
				{
					attacked(attacker);
				}
			}
			Action<ComponentCreature> injured = this.Injured;
			if (injured == null)
			{
				return;
			}
			injured(attacker);
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00074E6C File Offset: 0x0007306C
		public void Update(float dt)
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			if (this.Health > 0f && this.Health < 1f)
			{
				float num = 0f;
				if (this.m_componentPlayer != null)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
					{
						num = 0.0166666675f;
					}
					else if (this.m_componentPlayer.ComponentSleep.SleepFactor == 1f && this.m_componentPlayer.ComponentVitalStats.Food > 0f)
					{
						num = 0.00166666671f;
					}
					else if (this.m_componentPlayer.ComponentVitalStats.Food > 0.5f)
					{
						num = 0.00111111114f;
					}
				}
				else
				{
					num = 0.00111111114f;
				}
				this.Heal(this.m_subsystemGameInfo.TotalElapsedGameTimeDelta * num);
			}
			if (this.BreathingMode == BreathingMode.Air)
			{
				int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(position.X), Terrain.ToCell(this.m_componentCreature.ComponentCreatureModel.EyePosition.Y), Terrain.ToCell(position.Z));
				this.Air = ((BlocksManager.Blocks[cellContents] is FluidBlock || position.Y > 259f) ? MathUtils.Saturate(this.Air - dt / this.AirCapacity) : 1f);
			}
			else if (this.BreathingMode == BreathingMode.Water)
			{
				this.Air = ((this.m_componentCreature.ComponentBody.ImmersionFactor > 0.25f) ? 1f : MathUtils.Saturate(this.Air - dt / this.AirCapacity));
			}
			if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0f && this.m_componentCreature.ComponentBody.ImmersionFluidBlock is MagmaBlock)
			{
				this.Injure(2f * this.m_componentCreature.ComponentBody.ImmersionFactor * dt, null, false, LanguageControl.Get(base.GetType().Name, 1));
				float num2 = 1.1f + 0.1f * (float)MathUtils.Sin(12.0 * this.m_subsystemTime.GameTime);
				this.m_redScreenFactor = MathUtils.Max(this.m_redScreenFactor, num2 * 1.5f * this.m_componentCreature.ComponentBody.ImmersionFactor);
			}
			float num3 = MathUtils.Abs(this.m_componentCreature.ComponentBody.CollisionVelocityChange.Y);
			if (!this.m_wasStanding && num3 > this.FallResilience)
			{
				float num4 = MathUtils.Sqr(MathUtils.Max(num3 - this.FallResilience, 0f)) / 15f;
				if (this.m_componentPlayer != null)
				{
					num4 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num4, null, false, LanguageControl.Get(base.GetType().Name, 2));
			}
			this.m_wasStanding = (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.StandingOnBody != null);
			if ((position.Y < 0f || position.Y > 296f) && this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
			{
				this.Injure(0.1f, null, true, LanguageControl.Get(base.GetType().Name, 3));
				ComponentPlayer componentPlayer = this.m_componentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(base.GetType().Name, 4), Color.White, true, false);
				}
			}
			bool flag = this.m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0);
			if (flag && this.Air == 0f)
			{
				float num5 = 0.12f;
				if (this.m_componentPlayer != null)
				{
					num5 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num5, null, false, LanguageControl.Get(base.GetType().Name, 7));
			}
			if (flag && (this.m_componentOnFire.IsOnFire || this.m_componentOnFire.TouchesFire))
			{
				float num6 = 1f / this.FireResilience;
				if (this.m_componentPlayer != null)
				{
					num6 /= this.m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				this.Injure(num6, this.m_componentOnFire.Attacker, false, LanguageControl.Get(base.GetType().Name, 5));
			}
			if (flag && this.CanStrand && this.m_componentCreature.ComponentBody.ImmersionFactor < 0.25f)
			{
				int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
				int num7 = 0;
				if (!(standingOnValue.GetValueOrDefault() == num7 & standingOnValue != null) || this.m_componentCreature.ComponentBody.StandingOnBody != null)
				{
					this.Injure(0.05f, null, false, LanguageControl.Get(base.GetType().Name, 6));
				}
			}
			this.HealthChange = this.Health - this.m_lastHealth;
			this.m_lastHealth = this.Health;
			if (this.m_redScreenFactor > 0.01f)
			{
				this.m_redScreenFactor *= MathUtils.Pow(0.2f, dt);
			}
			else
			{
				this.m_redScreenFactor = 0f;
			}
			if (this.HealthChange < 0f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_redScreenFactor += -4f * this.HealthChange;
				ComponentPlayer componentPlayer2 = this.m_componentPlayer;
				if (componentPlayer2 != null)
				{
					componentPlayer2.ComponentGui.HealthBarWidget.Flash(MathUtils.Clamp((int)((0f - this.HealthChange) * 30f), 0, 10));
				}
			}
			if (this.m_componentPlayer != null)
			{
				this.m_componentPlayer.ComponentScreenOverlays.RedoutFactor = MathUtils.Max(this.m_componentPlayer.ComponentScreenOverlays.RedoutFactor, this.m_redScreenFactor);
			}
			if (this.m_componentPlayer != null)
			{
				this.m_componentPlayer.ComponentGui.HealthBarWidget.Value = this.Health;
			}
			if (this.Health == 0f && this.HealthChange < 0f)
			{
				Vector3 position2 = this.m_componentCreature.ComponentBody.Position + new Vector3(0f, this.m_componentCreature.ComponentBody.BoxSize.Y / 2f, 0f);
				float x = this.m_componentCreature.ComponentBody.BoxSize.X;
				this.m_subsystemParticles.AddParticleSystem(new KillParticleSystem(this.m_subsystemTerrain, position2, x));
				Vector3 position3 = (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max) / 2f;
				foreach (IInventory inventory in base.Entity.FindComponents<IInventory>())
				{
					inventory.DropAllItems(position3);
				}
				this.DeathTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
			if (this.Health <= 0f && this.CorpseDuration > 0f)
			{
				double? num8 = this.m_subsystemGameInfo.TotalElapsedGameTime - this.DeathTime;
				double num9 = (double)this.CorpseDuration;
				if (num8.GetValueOrDefault() > num9 & num8 != null)
				{
					this.m_componentCreature.ComponentSpawn.Despawn();
				}
			}
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0007561C File Offset: 0x0007381C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			this.m_componentOnFire = base.Entity.FindComponent<ComponentOnFire>(true);
			this.AttackResilience = valuesDictionary.GetValue<float>("AttackResilience");
			this.FallResilience = valuesDictionary.GetValue<float>("FallResilience");
			this.FireResilience = valuesDictionary.GetValue<float>("FireResilience");
			this.CorpseDuration = valuesDictionary.GetValue<float>("CorpseDuration");
			this.BreathingMode = valuesDictionary.GetValue<BreathingMode>("BreathingMode");
			this.CanStrand = valuesDictionary.GetValue<bool>("CanStrand");
			this.Health = valuesDictionary.GetValue<float>("Health");
			this.Air = valuesDictionary.GetValue<float>("Air");
			this.AirCapacity = valuesDictionary.GetValue<float>("AirCapacity");
			double value = valuesDictionary.GetValue<double>("DeathTime");
			this.DeathTime = ((value >= 0.0) ? new double?(value) : null);
			this.CauseOfDeath = valuesDictionary.GetValue<string>("CauseOfDeath");
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && base.Entity.FindComponent<ComponentPlayer>() != null)
			{
				this.IsInvulnerable = true;
			}
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x000757CC File Offset: 0x000739CC
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Health", this.Health);
			valuesDictionary.SetValue<float>("Air", this.Air);
			if (this.DeathTime != null)
			{
				valuesDictionary.SetValue<double?>("DeathTime", this.DeathTime);
			}
			if (!string.IsNullOrEmpty(this.CauseOfDeath))
			{
				valuesDictionary.SetValue<string>("CauseOfDeath", this.CauseOfDeath);
			}
		}

		// Token: 0x04000924 RID: 2340
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000925 RID: 2341
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000926 RID: 2342
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000927 RID: 2343
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000928 RID: 2344
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000929 RID: 2345
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400092A RID: 2346
		public ComponentCreature m_componentCreature;

		// Token: 0x0400092B RID: 2347
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400092C RID: 2348
		public ComponentOnFire m_componentOnFire;

		// Token: 0x0400092D RID: 2349
		public float m_lastHealth;

		// Token: 0x0400092E RID: 2350
		public bool m_wasStanding;

		// Token: 0x0400092F RID: 2351
		public float m_redScreenFactor;

		// Token: 0x04000930 RID: 2352
		public Game.Random m_random = new Game.Random();
	}
}
