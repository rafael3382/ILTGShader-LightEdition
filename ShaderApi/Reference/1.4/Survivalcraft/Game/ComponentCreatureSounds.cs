using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FA RID: 506
	public class ComponentCreatureSounds : Component
	{
		// Token: 0x06000EAB RID: 3755 RVA: 0x0006AF6C File Offset: 0x0006916C
		public virtual void PlayIdleSound(bool skipIfRecentlyPlayed)
		{
			if (!string.IsNullOrEmpty(this.m_idleSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + (double)(skipIfRecentlyPlayed ? 12f : 1f))
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_idleSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_idleSoundMinDistance, false);
			}
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x0006B000 File Offset: 0x00069200
		public virtual void PlayPainSound()
		{
			if (!string.IsNullOrEmpty(this.m_painSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_painSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_painSoundMinDistance, false);
			}
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0006B08C File Offset: 0x0006928C
		public virtual void PlayMoanSound()
		{
			if (!string.IsNullOrEmpty(this.m_moanSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_moanSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_moanSoundMinDistance, false);
			}
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0006B118 File Offset: 0x00069318
		public virtual void PlaySneezeSound()
		{
			if (!string.IsNullOrEmpty(this.m_sneezeSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_sneezeSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_sneezeSoundMinDistance, false);
			}
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0006B1A4 File Offset: 0x000693A4
		public void PlayCoughSound()
		{
			if (!string.IsNullOrEmpty(this.m_coughSound) && this.m_subsystemTime.GameTime > this.m_lastCoughingSoundTime + 1.0)
			{
				this.m_lastCoughingSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_coughSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_coughSoundMinDistance, false);
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0006B230 File Offset: 0x00069430
		public virtual void PlayPukeSound()
		{
			if (!string.IsNullOrEmpty(this.m_pukeSound) && this.m_subsystemTime.GameTime > this.m_lastPukeSoundTime + 1.0)
			{
				this.m_lastPukeSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_pukeSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_pukeSoundMinDistance, false);
			}
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0006B2BC File Offset: 0x000694BC
		public virtual void PlayAttackSound()
		{
			if (!string.IsNullOrEmpty(this.m_attackSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_attackSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_attackSoundMinDistance, false);
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0006B346 File Offset: 0x00069546
		public virtual bool PlayFootstepSound(float loudnessMultiplier)
		{
			return this.m_subsystemSoundMaterials.PlayFootstepSound(this.m_componentCreature, loudnessMultiplier);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0006B35C File Offset: 0x0006955C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_idleSound = valuesDictionary.GetValue<string>("IdleSound");
			this.m_painSound = valuesDictionary.GetValue<string>("PainSound");
			this.m_moanSound = valuesDictionary.GetValue<string>("MoanSound");
			this.m_sneezeSound = valuesDictionary.GetValue<string>("SneezeSound");
			this.m_coughSound = valuesDictionary.GetValue<string>("CoughSound");
			this.m_pukeSound = valuesDictionary.GetValue<string>("PukeSound");
			this.m_attackSound = valuesDictionary.GetValue<string>("AttackSound");
			this.m_idleSoundMinDistance = valuesDictionary.GetValue<float>("IdleSoundMinDistance");
			this.m_painSoundMinDistance = valuesDictionary.GetValue<float>("PainSoundMinDistance");
			this.m_moanSoundMinDistance = valuesDictionary.GetValue<float>("MoanSoundMinDistance");
			this.m_sneezeSoundMinDistance = valuesDictionary.GetValue<float>("SneezeSoundMinDistance");
			this.m_coughSoundMinDistance = valuesDictionary.GetValue<float>("CoughSoundMinDistance");
			this.m_pukeSoundMinDistance = valuesDictionary.GetValue<float>("PukeSoundMinDistance");
			this.m_attackSoundMinDistance = valuesDictionary.GetValue<float>("AttackSoundMinDistance");
		}

		// Token: 0x040007E0 RID: 2016
		public SubsystemTime m_subsystemTime;

		// Token: 0x040007E1 RID: 2017
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040007E2 RID: 2018
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x040007E3 RID: 2019
		public ComponentCreature m_componentCreature;

		// Token: 0x040007E4 RID: 2020
		public Random m_random = new Random();

		// Token: 0x040007E5 RID: 2021
		public string m_idleSound;

		// Token: 0x040007E6 RID: 2022
		public string m_painSound;

		// Token: 0x040007E7 RID: 2023
		public string m_moanSound;

		// Token: 0x040007E8 RID: 2024
		public string m_sneezeSound;

		// Token: 0x040007E9 RID: 2025
		public string m_coughSound;

		// Token: 0x040007EA RID: 2026
		public string m_pukeSound;

		// Token: 0x040007EB RID: 2027
		public string m_attackSound;

		// Token: 0x040007EC RID: 2028
		public float m_idleSoundMinDistance;

		// Token: 0x040007ED RID: 2029
		public float m_painSoundMinDistance;

		// Token: 0x040007EE RID: 2030
		public float m_moanSoundMinDistance;

		// Token: 0x040007EF RID: 2031
		public float m_sneezeSoundMinDistance;

		// Token: 0x040007F0 RID: 2032
		public float m_coughSoundMinDistance;

		// Token: 0x040007F1 RID: 2033
		public float m_pukeSoundMinDistance;

		// Token: 0x040007F2 RID: 2034
		public float m_attackSoundMinDistance;

		// Token: 0x040007F3 RID: 2035
		public double m_lastSoundTime = -1000.0;

		// Token: 0x040007F4 RID: 2036
		public double m_lastCoughingSoundTime = -1000.0;

		// Token: 0x040007F5 RID: 2037
		public double m_lastPukeSoundTime = -1000.0;
	}
}
