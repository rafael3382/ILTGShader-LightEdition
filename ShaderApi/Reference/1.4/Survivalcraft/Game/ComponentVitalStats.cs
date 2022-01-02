using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000238 RID: 568
	public class ComponentVitalStats : Component, IUpdateable
	{
		// Token: 0x170002BA RID: 698
		// (get) Token: 0x0600126B RID: 4715 RVA: 0x000880FA File Offset: 0x000862FA
		// (set) Token: 0x0600126C RID: 4716 RVA: 0x00088102 File Offset: 0x00086302
		public float Food
		{
			get
			{
				return this.m_food;
			}
			set
			{
				this.m_food = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x0600126D RID: 4717 RVA: 0x00088110 File Offset: 0x00086310
		// (set) Token: 0x0600126E RID: 4718 RVA: 0x00088118 File Offset: 0x00086318
		public float Stamina
		{
			get
			{
				return this.m_stamina;
			}
			set
			{
				this.m_stamina = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600126F RID: 4719 RVA: 0x00088126 File Offset: 0x00086326
		// (set) Token: 0x06001270 RID: 4720 RVA: 0x0008812E File Offset: 0x0008632E
		public float Sleep
		{
			get
			{
				return this.m_sleep;
			}
			set
			{
				this.m_sleep = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001271 RID: 4721 RVA: 0x0008813C File Offset: 0x0008633C
		// (set) Token: 0x06001272 RID: 4722 RVA: 0x00088144 File Offset: 0x00086344
		public float Temperature
		{
			get
			{
				return this.m_temperature;
			}
			set
			{
				this.m_temperature = MathUtils.Clamp(value, 0f, 24f);
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001273 RID: 4723 RVA: 0x0008815C File Offset: 0x0008635C
		// (set) Token: 0x06001274 RID: 4724 RVA: 0x00088164 File Offset: 0x00086364
		public float Wetness
		{
			get
			{
				return this.m_wetness;
			}
			set
			{
				this.m_wetness = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001275 RID: 4725 RVA: 0x00088172 File Offset: 0x00086372
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x00088178 File Offset: 0x00086378
		public virtual bool Eat(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			float num2 = block.GetNutritionalValue(value);
			float sicknessProbability = block.GetSicknessProbability(value);
			if (num2 <= 0f)
			{
				return false;
			}
			if (this.m_componentPlayer.ComponentSickness.IsSick && sicknessProbability > 0f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 1), Color.White, true, true);
				return false;
			}
			if (this.Food >= 0.98f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 2), Color.White, true, true);
				return false;
			}
			this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/HumanEat", 1f, this.m_random.Float(-0.2f, 0.2f), this.m_componentPlayer.ComponentBody.Position, 2f, 0f);
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				num2 *= 0.75f;
			}
			this.Food += num2;
			float num3;
			this.m_satiation.TryGetValue(num, out num3);
			num3 += MathUtils.Max(num2, 0.5f);
			this.m_satiation[num] = num3;
			if (this.m_componentPlayer.ComponentSickness.IsSick)
			{
				this.m_componentPlayer.ComponentSickness.NauseaEffect();
			}
			else if (sicknessProbability >= 0.5f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 3), Color.White, true, true);
			}
			else if (sicknessProbability > 0f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 4), Color.White, true, true);
			}
			else if (num3 > 2.5f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 5), Color.White, true, true);
			}
			else if (num3 > 2f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 6), Color.White, true, true);
			}
			else if (this.Food > 0.85f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 7), Color.White, true, true);
			}
			else
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 8), Color.White, true, false);
			}
			if (this.m_random.Bool(sicknessProbability) || num3 > 3.5f)
			{
				this.m_componentPlayer.ComponentSickness.StartSickness();
			}
			this.m_componentPlayer.PlayerStats.FoodItemsEaten += 1L;
			return true;
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x00088424 File Offset: 0x00086624
		public virtual void MakeSleepy(float sleepValue)
		{
			this.Sleep = MathUtils.Min(this.Sleep, sleepValue);
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x00088438 File Offset: 0x00086638
		public void Update(float dt)
		{
			if (this.m_componentPlayer.ComponentHealth.Health > 0f)
			{
				this.UpdateFood();
				this.UpdateStamina();
				this.UpdateSleep();
				this.UpdateTemperature();
				this.UpdateWetness();
				return;
			}
			this.m_pantingSound.Stop();
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x00088488 File Offset: 0x00086688
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemMetersBlockBehavior = base.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_pantingSound = this.m_subsystemAudio.CreateSound("Audio/HumanPanting");
			this.m_pantingSound.IsLooped = true;
			this.Food = valuesDictionary.GetValue<float>("Food");
			this.Stamina = valuesDictionary.GetValue<float>("Stamina");
			this.Sleep = valuesDictionary.GetValue<float>("Sleep");
			this.Temperature = valuesDictionary.GetValue<float>("Temperature");
			this.Wetness = valuesDictionary.GetValue<float>("Wetness");
			this.m_lastFood = this.Food;
			this.m_lastStamina = this.Stamina;
			this.m_lastSleep = this.Sleep;
			this.m_lastTemperature = this.Temperature;
			this.m_lastWetness = this.Wetness;
			this.m_environmentTemperature = this.Temperature;
			ComponentHealth componentHealth = this.m_componentPlayer.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature componentCreature)
			{
				this.m_lastAttackedTime = new double?(this.m_subsystemTime.GameTime);
			}));
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Satiation"))
			{
				this.m_satiation[int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture)] = (float)keyValuePair.Value;
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x00088654 File Offset: 0x00086854
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Food", this.Food);
			valuesDictionary.SetValue<float>("Stamina", this.Stamina);
			valuesDictionary.SetValue<float>("Sleep", this.Sleep);
			valuesDictionary.SetValue<float>("Temperature", this.Temperature);
			valuesDictionary.SetValue<float>("Wetness", this.Wetness);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Satiation", valuesDictionary2);
			foreach (KeyValuePair<int, float> keyValuePair in this.m_satiation)
			{
				if (keyValuePair.Value > 0f)
				{
					valuesDictionary2.SetValue<float>(keyValuePair.Key.ToString(CultureInfo.InvariantCulture), keyValuePair.Value);
				}
			}
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x00088738 File Offset: 0x00086938
		public override void OnEntityRemoved()
		{
			this.m_pantingSound.Stop();
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x00088748 File Offset: 0x00086948
		public virtual void UpdateFood()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			float num = (this.m_componentPlayer.ComponentLocomotion.LastWalkOrder != null) ? this.m_componentPlayer.ComponentLocomotion.LastWalkOrder.Value.Length() : 0f;
			float lastJumpOrder = this.m_componentPlayer.ComponentLocomotion.LastJumpOrder;
			float num2 = this.m_componentPlayer.ComponentCreatureModel.EyePosition.Y - this.m_componentPlayer.ComponentBody.Position.Y;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionDepth > num2;
			bool flag2 = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.33f && this.m_componentPlayer.ComponentBody.StandingOnValue == null;
			bool flag3 = this.m_subsystemTime.PeriodicGameTimeEvent(240.0, 13.0) && !this.m_componentPlayer.ComponentSickness.IsSick;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				float hungerFactor = this.m_componentPlayer.ComponentLevel.HungerFactor;
				this.Food -= hungerFactor * gameTimeDelta / 2880f;
				if (flag2 || flag)
				{
					this.Food -= hungerFactor * gameTimeDelta * num / 1440f;
				}
				else
				{
					this.Food -= hungerFactor * gameTimeDelta * num / 2880f;
				}
				this.Food -= hungerFactor * lastJumpOrder / 1200f;
				if (this.m_componentPlayer.ComponentMiner.DigCellFace != null)
				{
					this.Food -= hungerFactor * gameTimeDelta / 2880f;
				}
				if (!this.m_componentPlayer.ComponentSleep.IsSleeping)
				{
					if (this.Food <= 0f)
					{
						if (this.m_subsystemTime.PeriodicGameTimeEvent(50.0, 0.0))
						{
							this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 9));
							this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 10), Color.White, true, false);
							this.m_componentPlayer.ComponentGui.FoodBarWidget.Flash(10);
						}
					}
					else if (this.Food < 0.1f && (this.m_lastFood >= 0.1f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 11), Color.White, true, true);
					}
					else if (this.Food < 0.25f && (this.m_lastFood >= 0.25f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 12), Color.White, true, true);
					}
					else if (this.Food < 0.5f && (this.m_lastFood >= 0.5f || flag3))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 13), Color.White, true, false);
					}
				}
			}
			else
			{
				this.Food = 0.9f;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, -0.01))
			{
				this.m_satiationList.Clear();
				this.m_satiationList.AddRange(this.m_satiation);
				this.m_satiation.Clear();
				foreach (KeyValuePair<int, float> keyValuePair in this.m_satiationList)
				{
					float num3 = MathUtils.Max(keyValuePair.Value - 0.000416666677f, 0f);
					if (num3 > 0f)
					{
						this.m_satiation.Add(keyValuePair.Key, num3);
					}
				}
			}
			this.m_lastFood = this.Food;
			this.m_componentPlayer.ComponentGui.FoodBarWidget.Value = this.Food;
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00088BB4 File Offset: 0x00086DB4
		public virtual void UpdateStamina()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			float num = (this.m_componentPlayer.ComponentLocomotion.LastWalkOrder != null) ? this.m_componentPlayer.ComponentLocomotion.LastWalkOrder.Value.Length() : 0f;
			float lastJumpOrder = this.m_componentPlayer.ComponentLocomotion.LastJumpOrder;
			float num2 = this.m_componentPlayer.ComponentCreatureModel.EyePosition.Y - this.m_componentPlayer.ComponentBody.Position.Y;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionDepth > num2;
			bool flag2 = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.33f && this.m_componentPlayer.ComponentBody.StandingOnValue == null;
			bool isPuking = this.m_componentPlayer.ComponentSickness.IsPuking;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless || !this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				this.Stamina = 1f;
				this.ApplyDensityModifier(0f);
				return;
			}
			float num3 = 1f / MathUtils.Max(this.m_componentPlayer.ComponentLevel.SpeedFactor, 0.75f);
			if (this.m_componentPlayer.ComponentSickness.IsSick || this.m_componentPlayer.ComponentFlu.HasFlu)
			{
				num3 *= 5f;
			}
			this.Stamina += gameTimeDelta * 0.07f;
			this.Stamina -= 0.025f * lastJumpOrder * num3;
			if (flag2 || flag)
			{
				this.Stamina -= gameTimeDelta * (0.07f + 0.006f * num3 + 0.008f * num);
			}
			else
			{
				this.Stamina -= gameTimeDelta * (0.07f + 0.006f * num3) * num;
			}
			if (!flag2 && !flag && this.Stamina < 0.33f && this.m_lastStamina >= 0.33f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 14), Color.White, true, false);
			}
			if ((flag2 || flag) && this.Stamina < 0.4f && this.m_lastStamina >= 0.4f)
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 15), Color.White, true, true);
			}
			if (this.Stamina < 0.1f)
			{
				if (flag2 || flag)
				{
					if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
					{
						this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 16));
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 17), Color.White, true, false);
					}
					if (this.m_random.Float(0f, 1f) < 1f * gameTimeDelta)
					{
						this.m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
					}
				}
				else if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
				{
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 18), Color.White, true, true);
				}
			}
			this.m_lastStamina = this.Stamina;
			float num4 = MathUtils.Saturate(2f * (0.5f - this.Stamina));
			if (!flag && num4 > 0f)
			{
				float num5 = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.2f : 0f;
				this.m_pantingSound.Volume = 1f * SettingsManager.SoundsVolume * MathUtils.Saturate(1f * num4) * MathUtils.Lerp(0.8f, 1f, SimplexNoise.Noise((float)MathUtils.Remainder(3.0 * Time.RealTime + 100.0, 1000.0)));
				this.m_pantingSound.Pitch = AudioManager.ToEnginePitch(num5 + MathUtils.Lerp(-0.15f, 0.05f, num4) * MathUtils.Lerp(0.8f, 1.2f, SimplexNoise.Noise((float)MathUtils.Remainder(3.0 * Time.RealTime + 200.0, 1000.0))));
				this.m_pantingSound.Play();
			}
			else
			{
				this.m_pantingSound.Stop();
			}
			float num6 = MathUtils.Saturate(3f * (0.33f - this.Stamina));
			if (num6 > 0f && SimplexNoise.Noise((float)MathUtils.Remainder(Time.RealTime, 1000.0)) < num6)
			{
				this.ApplyDensityModifier(0.6f);
				return;
			}
			this.ApplyDensityModifier(0f);
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x000890D4 File Offset: 0x000872D4
		public virtual void UpdateSleep()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			bool flag = this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.05f;
			bool flag2 = this.m_subsystemTime.PeriodicGameTimeEvent(240.0, 9.0);
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				if (this.m_componentPlayer.ComponentSleep.SleepFactor == 1f)
				{
					this.Sleep += 0.05f * gameTimeDelta;
				}
				else if (!flag)
				{
					if (this.m_lastAttackedTime != null)
					{
						double? num = this.m_subsystemTime.GameTime - this.m_lastAttackedTime;
						double num2 = 10.0;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							goto IL_379;
						}
					}
					this.Sleep -= gameTimeDelta / 1800f;
					if (this.Sleep < 0.075f && (this.m_lastSleep >= 0.075f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 19), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
					else if (this.Sleep < 0.2f && (this.m_lastSleep >= 0.2f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 20), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
					else if (this.Sleep < 0.33f && (this.m_lastSleep >= 0.33f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 21), Color.White, true, false);
					}
					else if (this.Sleep < 0.5f && (this.m_lastSleep >= 0.5f || flag2))
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 22), Color.White, true, false);
					}
					if (this.Sleep < 0.075f)
					{
						float num3 = MathUtils.Lerp(0.05f, 0.2f, (0.075f - this.Sleep) / 0.075f);
						float x = (this.Sleep < 0.0375f) ? this.m_random.Float(3f, 6f) : this.m_random.Float(2f, 4f);
						if (this.m_random.Float(0f, 1f) < num3 * gameTimeDelta)
						{
							this.m_sleepBlackoutDuration = MathUtils.Max(this.m_sleepBlackoutDuration, x);
							this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
						}
					}
					if (this.Sleep <= 0f && !this.m_componentPlayer.ComponentSleep.IsSleeping)
					{
						this.m_componentPlayer.ComponentSleep.Sleep(false);
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 23), Color.White, true, true);
						this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
					}
				}
			}
			else
			{
				this.Sleep = 0.9f;
			}
			IL_379:
			this.m_lastSleep = this.Sleep;
			this.m_sleepBlackoutDuration -= gameTimeDelta;
			float num4 = MathUtils.Saturate(0.5f * this.m_sleepBlackoutDuration);
			this.m_sleepBlackoutFactor = MathUtils.Saturate(this.m_sleepBlackoutFactor + 2f * gameTimeDelta * (num4 - this.m_sleepBlackoutFactor));
			if (!this.m_componentPlayer.ComponentSleep.IsSleeping)
			{
				this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_sleepBlackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
				if ((double)this.m_sleepBlackoutFactor > 0.01)
				{
					this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentVitalStats.fName, 24);
					this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_sleepBlackoutFactor - 0.9f));
				}
			}
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x00089540 File Offset: 0x00087740
		public virtual void UpdateTemperature()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			bool flag = this.m_subsystemTime.PeriodicGameTimeEvent(300.0, 17.0);
			float num = this.m_componentPlayer.ComponentClothing.Insulation * MathUtils.Lerp(1f, 0.05f, MathUtils.Saturate(4f * this.Wetness));
			string arg;
			switch (this.m_componentPlayer.ComponentClothing.LeastInsulatedSlot)
			{
			case ClothingSlot.Head:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 40);
				break;
			case ClothingSlot.Torso:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 41);
				break;
			case ClothingSlot.Legs:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 42);
				break;
			default:
				arg = LanguageControl.Get(ComponentVitalStats.fName, 43);
				break;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 2.0 * (double)this.GetHashCode() % 1000.0 / 1000.0))
			{
				int x = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.X);
				int y = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Y + 0.1f);
				int z = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Z);
				this.m_subsystemMetersBlockBehavior.CalculateTemperature(x, y, z, 12f, num, out this.m_environmentTemperature, out this.m_environmentTemperatureFlux);
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				float num2 = this.m_environmentTemperature - this.Temperature;
				float num3 = 0.01f + 0.005f * this.m_environmentTemperatureFlux;
				this.Temperature += MathUtils.Saturate(num3 * gameTimeDelta) * num2;
			}
			else
			{
				this.Temperature = 12f;
			}
			if (this.Temperature <= 0f)
			{
				this.m_componentPlayer.ComponentHealth.Injure(1f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 25));
			}
			else if (this.Temperature < 3f)
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
				{
					this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 26));
					string text = (this.Wetness > 0f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 27), arg) : ((num >= 1f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 28), arg) : string.Format(LanguageControl.Get(ComponentVitalStats.fName, 29), arg));
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(text, Color.White, true, false);
					this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				}
			}
			else if (this.Temperature < 6f && (this.m_lastTemperature >= 6f || flag))
			{
				string text2 = (this.Wetness > 0f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 30), arg) : ((num >= 1f) ? string.Format(LanguageControl.Get(ComponentVitalStats.fName, 31), arg) : string.Format(LanguageControl.Get(ComponentVitalStats.fName, 32), arg));
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(text2, Color.White, true, true);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
			}
			else if (this.Temperature < 8f && (this.m_lastTemperature >= 8f || flag))
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 33), Color.White, true, false);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
			}
			if (this.Temperature >= 24f)
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
				{
					this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 34), Color.White, true, false);
					this.m_componentPlayer.ComponentHealth.Injure(0.05f, null, false, LanguageControl.Get(ComponentVitalStats.fName, 35));
					this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(8.0, 0.0))
				{
					this.m_temperatureBlackoutDuration = MathUtils.Max(this.m_temperatureBlackoutDuration, 6f);
					this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
				}
			}
			else if (this.Temperature > 20f && this.m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
			{
				this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 36), Color.White, true, false);
				this.m_temperatureBlackoutDuration = MathUtils.Max(this.m_temperatureBlackoutDuration, 3f);
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.Flash(10);
				this.m_componentPlayer.ComponentCreatureSounds.PlayMoanSound();
			}
			this.m_lastTemperature = this.Temperature;
			this.m_componentPlayer.ComponentScreenOverlays.IceFactor = MathUtils.Saturate(1f - this.Temperature / 6f);
			this.m_temperatureBlackoutDuration -= gameTimeDelta;
			float num4 = MathUtils.Saturate(0.5f * this.m_temperatureBlackoutDuration);
			this.m_temperatureBlackoutFactor = MathUtils.Saturate(this.m_temperatureBlackoutFactor + 2f * gameTimeDelta * (num4 - this.m_temperatureBlackoutFactor));
			this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor = MathUtils.Max(this.m_temperatureBlackoutFactor, this.m_componentPlayer.ComponentScreenOverlays.BlackoutFactor);
			if ((double)this.m_temperatureBlackoutFactor > 0.01)
			{
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessage = LanguageControl.Get(ComponentVitalStats.fName, 37);
				this.m_componentPlayer.ComponentScreenOverlays.FloatingMessageFactor = MathUtils.Saturate(10f * (this.m_temperatureBlackoutFactor - 0.9f));
			}
			if (this.m_environmentTemperature > 22f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature6", null);
				return;
			}
			if (this.m_environmentTemperature > 18f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature5", null);
				return;
			}
			if (this.m_environmentTemperature > 14f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature4", null);
				return;
			}
			if (this.m_environmentTemperature > 10f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature3", null);
				return;
			}
			if (this.m_environmentTemperature > 6f)
			{
				this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Temperature2", null);
				return;
			}
			this.m_componentPlayer.ComponentGui.TemperatureBarWidget.BarSubtexture = ((this.m_environmentTemperature > 2f) ? ContentManager.Get<Subtexture>("Textures/Atlas/Temperature1", null) : ContentManager.Get<Subtexture>("Textures/Atlas/Temperature0", null));
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x00089CC0 File Offset: 0x00087EC0
		public virtual void UpdateWetness()
		{
			float gameTimeDelta = this.m_subsystemTime.GameTimeDelta;
			if (this.m_componentPlayer.ComponentBody.ImmersionFactor > 0.2f && this.m_componentPlayer.ComponentBody.ImmersionFluidBlock is WaterBlock)
			{
				float num = 2f * this.m_componentPlayer.ComponentBody.ImmersionFactor;
				this.Wetness += MathUtils.Saturate(3f * gameTimeDelta) * (num - this.Wetness);
			}
			int x = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.X);
			int num2 = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Y + 0.1f);
			int z = Terrain.ToCell(this.m_componentPlayer.ComponentBody.Position.Z);
			PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
			if (num2 >= precipitationShaftInfo.YLimit && precipitationShaftInfo.Type == PrecipitationType.Rain)
			{
				this.Wetness += 0.05f * precipitationShaftInfo.Intensity * gameTimeDelta;
			}
			float num3 = 180f;
			if (this.m_environmentTemperature > 8f)
			{
				num3 = 120f;
			}
			if (this.m_environmentTemperature > 16f)
			{
				num3 = 60f;
			}
			if (this.m_environmentTemperature > 24f)
			{
				num3 = 30f;
			}
			this.Wetness -= gameTimeDelta / num3;
			if (this.Wetness > 0.8f && this.m_lastWetness <= 0.8f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness > 0.8f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 38), Color.White, true, true);
					}
				});
			}
			else if (this.Wetness > 0.2f && this.m_lastWetness <= 0.2f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness > 0.2f && this.Wetness <= 0.8f && this.Wetness > this.m_lastWetness)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 39), Color.White, true, true);
					}
				});
			}
			else if (this.Wetness <= 0f && this.m_lastWetness > 0f)
			{
				Time.QueueTimeDelayedExecution(Time.FrameStartTime + 2.0, delegate
				{
					if (this.Wetness <= 0f)
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentVitalStats.fName, 40), Color.White, true, true);
					}
				});
			}
			this.m_lastWetness = this.Wetness;
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x00089EE4 File Offset: 0x000880E4
		public virtual void ApplyDensityModifier(float modifier)
		{
			float num = modifier - this.m_densityModifierApplied;
			if (num != 0f)
			{
				this.m_densityModifierApplied = modifier;
				this.m_componentPlayer.ComponentBody.Density += num;
			}
		}

		// Token: 0x04000B4F RID: 2895
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000B50 RID: 2896
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B51 RID: 2897
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000B52 RID: 2898
		public SubsystemMetersBlockBehavior m_subsystemMetersBlockBehavior;

		// Token: 0x04000B53 RID: 2899
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000B54 RID: 2900
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000B55 RID: 2901
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B56 RID: 2902
		public Sound m_pantingSound;

		// Token: 0x04000B57 RID: 2903
		public float m_food;

		// Token: 0x04000B58 RID: 2904
		public float m_stamina;

		// Token: 0x04000B59 RID: 2905
		public float m_sleep;

		// Token: 0x04000B5A RID: 2906
		public float m_temperature;

		// Token: 0x04000B5B RID: 2907
		public float m_wetness;

		// Token: 0x04000B5C RID: 2908
		public float m_lastFood;

		// Token: 0x04000B5D RID: 2909
		public float m_lastStamina;

		// Token: 0x04000B5E RID: 2910
		public float m_lastSleep;

		// Token: 0x04000B5F RID: 2911
		public float m_lastTemperature;

		// Token: 0x04000B60 RID: 2912
		public float m_lastWetness;

		// Token: 0x04000B61 RID: 2913
		public Dictionary<int, float> m_satiation = new Dictionary<int, float>();

		// Token: 0x04000B62 RID: 2914
		public List<KeyValuePair<int, float>> m_satiationList = new List<KeyValuePair<int, float>>();

		// Token: 0x04000B63 RID: 2915
		public float m_densityModifierApplied;

		// Token: 0x04000B64 RID: 2916
		public double? m_lastAttackedTime;

		// Token: 0x04000B65 RID: 2917
		public float m_sleepBlackoutFactor;

		// Token: 0x04000B66 RID: 2918
		public float m_sleepBlackoutDuration;

		// Token: 0x04000B67 RID: 2919
		public float m_environmentTemperature;

		// Token: 0x04000B68 RID: 2920
		public float m_environmentTemperatureFlux;

		// Token: 0x04000B69 RID: 2921
		public float m_temperatureBlackoutFactor;

		// Token: 0x04000B6A RID: 2922
		public float m_temperatureBlackoutDuration;

		// Token: 0x04000B6B RID: 2923
		public static string fName = "ComponentVitalStats";
	}
}
