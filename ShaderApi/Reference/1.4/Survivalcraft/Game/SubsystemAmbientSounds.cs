using System;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000181 RID: 385
	public class SubsystemAmbientSounds : Subsystem, IUpdateable
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00036DB3 File Offset: 0x00034FB3
		// (set) Token: 0x060008B7 RID: 2231 RVA: 0x00036DBB File Offset: 0x00034FBB
		public SubsystemAudio SubsystemAudio { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x00036DC4 File Offset: 0x00034FC4
		// (set) Token: 0x060008B9 RID: 2233 RVA: 0x00036DCC File Offset: 0x00034FCC
		public float FireSoundVolume { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x00036DD5 File Offset: 0x00034FD5
		// (set) Token: 0x060008BB RID: 2235 RVA: 0x00036DDD File Offset: 0x00034FDD
		public float WaterSoundVolume { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x00036DE6 File Offset: 0x00034FE6
		// (set) Token: 0x060008BD RID: 2237 RVA: 0x00036DEE File Offset: 0x00034FEE
		public float MagmaSoundVolume { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x00036DF7 File Offset: 0x00034FF7
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00036DFC File Offset: 0x00034FFC
		public void Update(float dt)
		{
			this.m_fireSound.Volume = MathUtils.Lerp(this.m_fireSound.Volume, SettingsManager.SoundsVolume * this.FireSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_fireSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_fireSound.Play();
			}
			else
			{
				this.m_fireSound.Pause();
			}
			this.m_waterSound.Volume = MathUtils.Lerp(this.m_waterSound.Volume, SettingsManager.SoundsVolume * this.WaterSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_waterSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_waterSound.Play();
			}
			else
			{
				this.m_waterSound.Pause();
			}
			this.m_magmaSound.Volume = MathUtils.Lerp(this.m_magmaSound.Volume, SettingsManager.SoundsVolume * this.MagmaSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_magmaSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_magmaSound.Play();
			}
			else
			{
				this.m_magmaSound.Pause();
			}
			if (this.m_magmaSound.State == SoundState.Playing && this.m_random.Bool(0.2f * dt))
			{
				this.SubsystemAudio.PlayRandomSound("Audio/Sizzles", this.m_magmaSound.Volume, this.m_random.Float(-0.2f, 0.2f), 0f, 0f);
			}
			this.FireSoundVolume = 0f;
			this.WaterSoundVolume = 0f;
			this.MagmaSoundVolume = 0f;
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00036FBC File Offset: 0x000351BC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_fireSound = this.SubsystemAudio.CreateSound("Audio/Fire");
			this.m_fireSound.IsLooped = true;
			this.m_fireSound.Volume = 0f;
			this.m_waterSound = this.SubsystemAudio.CreateSound("Audio/Water");
			this.m_waterSound.IsLooped = true;
			this.m_waterSound.Volume = 0f;
			this.m_magmaSound = this.SubsystemAudio.CreateSound("Audio/Magma");
			this.m_magmaSound.IsLooped = true;
			this.m_magmaSound.Volume = 0f;
		}

		// Token: 0x04000460 RID: 1120
		public Sound m_fireSound;

		// Token: 0x04000461 RID: 1121
		public Sound m_waterSound;

		// Token: 0x04000462 RID: 1122
		public Sound m_magmaSound;

		// Token: 0x04000463 RID: 1123
		public Game.Random m_random = new Game.Random();
	}
}
