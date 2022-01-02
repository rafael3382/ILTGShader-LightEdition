using System;
using Engine;
using Engine.Audio;
using Engine.Media;

namespace Game
{
	// Token: 0x02000148 RID: 328
	public static class MusicManager
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x00024C20 File Offset: 0x00022E20
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x00024C27 File Offset: 0x00022E27
		public static MusicManager.Mix CurrentMix
		{
			get
			{
				return MusicManager.m_currentMix;
			}
			set
			{
				if (value != MusicManager.m_currentMix)
				{
					MusicManager.m_currentMix = value;
					MusicManager.m_nextSongTime = 0.0;
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000672 RID: 1650 RVA: 0x00024C45 File Offset: 0x00022E45
		public static bool IsPlaying
		{
			get
			{
				return MusicManager.m_sound != null && MusicManager.m_sound.State > SoundState.Stopped;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x00024C5D File Offset: 0x00022E5D
		public static float Volume
		{
			get
			{
				return SettingsManager.MusicVolume * 0.6f;
			}
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00024C6C File Offset: 0x00022E6C
		public static void Update()
		{
			if (MusicManager.m_fadeSound != null)
			{
				MusicManager.m_fadeSound.Volume = MathUtils.Min(MusicManager.m_fadeSound.Volume - 0.33f * MusicManager.Volume * Time.FrameDuration, MusicManager.Volume);
				if (MusicManager.m_fadeSound.Volume <= 0f)
				{
					MusicManager.m_fadeSound.Dispose();
					MusicManager.m_fadeSound = null;
				}
			}
			if (MusicManager.m_sound != null && Time.FrameStartTime >= MusicManager.m_fadeStartTime)
			{
				MusicManager.m_sound.Volume = MathUtils.Min(MusicManager.m_sound.Volume + 0.33f * MusicManager.Volume * Time.FrameDuration, MusicManager.Volume);
			}
			if (MusicManager.m_currentMix == MusicManager.Mix.None || MusicManager.Volume == 0f)
			{
				MusicManager.StopMusic();
				return;
			}
			if (MusicManager.m_currentMix == MusicManager.Mix.Menu && (Time.FrameStartTime >= MusicManager.m_nextSongTime || !MusicManager.IsPlaying))
			{
				float startPercentage = MusicManager.IsPlaying ? MusicManager.m_random.Float(0f, 0.75f) : 0f;
				switch (MusicManager.m_random.Int(0, 5))
				{
				case 0:
					MusicManager.PlayMusic("Music/NativeAmericanFluteSpirit", startPercentage);
					break;
				case 1:
					MusicManager.PlayMusic("Music/AloneForever", startPercentage);
					break;
				case 2:
					MusicManager.PlayMusic("Music/NativeAmerican", startPercentage);
					break;
				case 3:
					MusicManager.PlayMusic("Music/NativeAmericanHeart", startPercentage);
					break;
				case 4:
					MusicManager.PlayMusic("Music/NativeAmericanPeaceFlute", startPercentage);
					break;
				case 5:
					MusicManager.PlayMusic("Music/NativeIndianChant", startPercentage);
					break;
				}
				MusicManager.m_nextSongTime = Time.FrameStartTime + (double)MusicManager.m_random.Float(40f, 60f);
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00024E0C File Offset: 0x0002300C
		public static void PlayMusic(string name, float startPercentage)
		{
			if (string.IsNullOrEmpty(name))
			{
				MusicManager.StopMusic();
				return;
			}
			try
			{
				MusicManager.StopMusic();
				MusicManager.m_fadeStartTime = Time.FrameStartTime + 2.0;
				float volume = (MusicManager.m_fadeSound != null) ? 0f : MusicManager.Volume;
				StreamingSource streamingSource = ContentManager.Get<StreamingSource>(name, ".ogg").Duplicate();
				streamingSource.Position = (long)(MathUtils.Saturate(startPercentage) * (float)(streamingSource.BytesCount / (long)streamingSource.ChannelsCount / 2L)) / 16L * 16L;
				MusicManager.m_sound = new StreamingSound(streamingSource, volume, 1f, 0f, false, true, 1f);
				MusicManager.m_sound.Play();
			}
			catch
			{
				Log.Warning("Error playing music \"{0}\".", new object[]
				{
					name
				});
			}
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00024EE0 File Offset: 0x000230E0
		public static void StopMusic()
		{
			if (MusicManager.m_sound != null)
			{
				if (MusicManager.m_fadeSound != null)
				{
					MusicManager.m_fadeSound.Dispose();
				}
				MusicManager.m_fadeSound = MusicManager.m_sound;
				MusicManager.m_sound = null;
			}
		}

		// Token: 0x040002C9 RID: 713
		public const float m_fadeSpeed = 0.33f;

		// Token: 0x040002CA RID: 714
		public const float m_fadeWait = 2f;

		// Token: 0x040002CB RID: 715
		public static StreamingSound m_fadeSound;

		// Token: 0x040002CC RID: 716
		public static StreamingSound m_sound;

		// Token: 0x040002CD RID: 717
		public static double m_fadeStartTime;

		// Token: 0x040002CE RID: 718
		public static MusicManager.Mix m_currentMix;

		// Token: 0x040002CF RID: 719
		public static double m_nextSongTime;

		// Token: 0x040002D0 RID: 720
		public static Game.Random m_random = new Game.Random();

		// Token: 0x02000437 RID: 1079
		public enum Mix
		{
			// Token: 0x0400159C RID: 5532
			None,
			// Token: 0x0400159D RID: 5533
			Menu
		}
	}
}
