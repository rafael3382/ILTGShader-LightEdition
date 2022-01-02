using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000219 RID: 537
	public class ComponentLevel : Component, IUpdateable
	{
		// Token: 0x17000215 RID: 533
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x0007A2C8 File Offset: 0x000784C8
		// (set) Token: 0x0600104E RID: 4174 RVA: 0x0007A2D0 File Offset: 0x000784D0
		public float StrengthFactor { get; set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x0007A2D9 File Offset: 0x000784D9
		// (set) Token: 0x06001050 RID: 4176 RVA: 0x0007A2E1 File Offset: 0x000784E1
		public float ResilienceFactor { get; set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06001051 RID: 4177 RVA: 0x0007A2EA File Offset: 0x000784EA
		// (set) Token: 0x06001052 RID: 4178 RVA: 0x0007A2F2 File Offset: 0x000784F2
		public float SpeedFactor { get; set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06001053 RID: 4179 RVA: 0x0007A2FB File Offset: 0x000784FB
		// (set) Token: 0x06001054 RID: 4180 RVA: 0x0007A303 File Offset: 0x00078503
		public float HungerFactor { get; set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x0007A30C File Offset: 0x0007850C
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x0007A310 File Offset: 0x00078510
		public virtual void AddExperience(int count, bool playSound)
		{
			if (playSound)
			{
				this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 0.2f, this.m_random.Float(-0.1f, 0.4f), 0f, 0f);
			}
			for (int i = 0; i < count; i++)
			{
				float num = 0.012f / MathUtils.Pow(1.08f, MathUtils.Floor(this.m_componentPlayer.PlayerData.Level - 1f));
				if (MathUtils.Floor(this.m_componentPlayer.PlayerData.Level + num) > MathUtils.Floor(this.m_componentPlayer.PlayerData.Level))
				{
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentLevel.fName, 1), Color.White, true, false);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.2f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.15000000596046448, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.30000001192092896, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.13333334f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.45000001788139343, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.383333325f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.75, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.90000003576278687, delegate
					{
						this.m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 0.383333325f, 0f, 0f);
					});
				}
				this.m_componentPlayer.PlayerData.Level += num;
			}
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0007A504 File Offset: 0x00078704
		public virtual float CalculateStrengthFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float stamina = this.m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			float num6 = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 3), string.Format("{0:0}", stamina * 100f))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = (float)((!this.m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0);
			float num10 = num8 * num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = (this.m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(ComponentLevel.fName, 6) : LanguageControl.Get(ComponentLevel.fName, 7))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num11 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num12 = num10 * num11;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num11,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num13 = (float)((!this.m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0);
			float num14 = num12 * num13;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num13,
					Description = (this.m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(ComponentLevel.fName, 10) : LanguageControl.Get(ComponentLevel.fName, 11))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num15 = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless) ? 1.25f : 1f;
			float result = num14 * num15;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num15,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0007A8C4 File Offset: 0x00078AC4
		public virtual float CalculateResilienceFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.05f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num5 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			float num6 = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			float num8 = num6 * num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = 1f;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num9 = 1.5f;
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num9 = float.PositiveInfinity;
			}
			float result = num8 * num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0007AB2C File Offset: 0x00078D2C
		public virtual float CalculateSpeedFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = 1f;
			float num2 = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 1.03f : 1f;
			num *= num2;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num2,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f + 0.02f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			num *= num3;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num4 = 1f;
			foreach (int clothingValue in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Head))
			{
				ComponentLevel.AddClothingFactor(clothingValue, ref num4, factors);
			}
			foreach (int clothingValue2 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Torso))
			{
				ComponentLevel.AddClothingFactor(clothingValue2, ref num4, factors);
			}
			foreach (int clothingValue3 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Legs))
			{
				ComponentLevel.AddClothingFactor(clothingValue3, ref num4, factors);
			}
			foreach (int clothingValue4 in this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Feet))
			{
				ComponentLevel.AddClothingFactor(clothingValue4, ref num4, factors);
			}
			num *= num4;
			float stamina = this.m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			num *= num5;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 3), string.Format("{0:0}", stamina * 100f))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num6 = this.m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f;
			num *= num6;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num6,
					Description = (this.m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(ComponentLevel.fName, 4) : LanguageControl.Get(ComponentLevel.fName, 5))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num7 = (float)((!this.m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0);
			num *= num7;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num7,
					Description = (this.m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(ComponentLevel.fName, 6) : LanguageControl.Get(ComponentLevel.fName, 7))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num8 = this.m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f;
			num *= num8;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num8,
					Description = (this.m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(ComponentLevel.fName, 8) : LanguageControl.Get(ComponentLevel.fName, 9))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			float num9 = (float)((!this.m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0);
			num *= num9;
			if (factors != null)
			{
				ComponentLevel.Factor factor = new ComponentLevel.Factor
				{
					Value = num9,
					Description = (this.m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(ComponentLevel.fName, 10) : LanguageControl.Get(ComponentLevel.fName, 11))
				};
				ComponentLevel.Factor item = factor;
				factors.Add(item);
			}
			return num;
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0007AFE0 File Offset: 0x000791E0
		public virtual float CalculateHungerFactor(ICollection<ComponentLevel.Factor> factors)
		{
			float num = (this.m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.7f : 1f;
			float num2 = 1f * num;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num,
					Description = this.m_componentPlayer.PlayerData.PlayerClass.ToString()
				};
				factors.Add(item);
			}
			float level = this.m_componentPlayer.PlayerData.Level;
			float num3 = 1f - 0.01f * MathUtils.Floor(MathUtils.Clamp(level, 1f, 21f) - 1f);
			float num4 = num2 * num3;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num3,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(level).ToString())
				};
				factors.Add(item);
			}
			float num5 = 1f;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num5 = 0.66f;
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num5 = 0f;
			}
			float result = num4 * num5;
			if (factors != null)
			{
				ComponentLevel.Factor item = new ComponentLevel.Factor
				{
					Value = num5,
					Description = string.Format(LanguageControl.Get(ComponentLevel.fName, 12), this.m_subsystemGameInfo.WorldSettings.GameMode.ToString())
				};
				factors.Add(item);
			}
			return result;
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0007B164 File Offset: 0x00079364
		public virtual void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(180.0, 179.0))
			{
				this.AddExperience(1, false);
			}
			if (this.m_lastLevelTextValue == null || this.m_lastLevelTextValue.Value != MathUtils.Floor(this.m_componentPlayer.PlayerData.Level))
			{
				this.m_componentPlayer.ComponentGui.LevelLabelWidget.Text = string.Format(LanguageControl.Get(ComponentLevel.fName, 2), MathUtils.Floor(this.m_componentPlayer.PlayerData.Level).ToString());
				this.m_lastLevelTextValue = new float?(MathUtils.Floor(this.m_componentPlayer.PlayerData.Level));
			}
			this.m_componentPlayer.PlayerStats.HighestLevel = MathUtils.Max(this.m_componentPlayer.PlayerStats.HighestLevel, this.m_componentPlayer.PlayerData.Level);
			this.StrengthFactor = this.CalculateStrengthFactor(null);
			this.SpeedFactor = this.CalculateSpeedFactor(null);
			this.HungerFactor = this.CalculateHungerFactor(null);
			this.ResilienceFactor = this.CalculateResilienceFactor(null);
			ModsManager.HookAction("OnLevelUpdate", delegate(ModLoader modLoader)
			{
				modLoader.OnLevelUpdate(this);
				return false;
			});
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x0007B2A8 File Offset: 0x000794A8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.StrengthFactor = 1f;
			this.SpeedFactor = 1f;
			this.HungerFactor = 1f;
			this.ResilienceFactor = 1f;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0007B32C File Offset: 0x0007952C
		public static void AddClothingFactor(int clothingValue, ref float clothingFactor, ICollection<ComponentLevel.Factor> factors)
		{
			ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(clothingValue)].GetClothingData(clothingValue);
			if (clothingData.MovementSpeedFactor != 1f)
			{
				clothingFactor *= clothingData.MovementSpeedFactor;
				if (factors != null)
				{
					factors.Add(new ComponentLevel.Factor
					{
						Value = clothingData.MovementSpeedFactor,
						Description = clothingData.DisplayName
					});
				}
			}
		}

		// Token: 0x040009A9 RID: 2473
		public Game.Random m_random = new Game.Random();

		// Token: 0x040009AA RID: 2474
		public static string fName = "ComponentLevel";

		// Token: 0x040009AB RID: 2475
		public List<ComponentLevel.Factor> m_factors = new List<ComponentLevel.Factor>();

		// Token: 0x040009AC RID: 2476
		public float? m_lastLevelTextValue;

		// Token: 0x040009AD RID: 2477
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040009AE RID: 2478
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040009AF RID: 2479
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009B0 RID: 2480
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040009B1 RID: 2481
		public const float FemaleStrengthFactor = 0.8f;

		// Token: 0x040009B2 RID: 2482
		public const float FemaleResilienceFactor = 0.8f;

		// Token: 0x040009B3 RID: 2483
		public const float FemaleSpeedFactor = 1.03f;

		// Token: 0x040009B4 RID: 2484
		public const float FemaleHungerFactor = 0.7f;

		// Token: 0x020004D7 RID: 1239
		public struct Factor
		{
			// Token: 0x040017B6 RID: 6070
			public string Description;

			// Token: 0x040017B7 RID: 6071
			public float Value;
		}
	}
}
