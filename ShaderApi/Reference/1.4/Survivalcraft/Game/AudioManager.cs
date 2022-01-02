using System;
using Engine;
using Engine.Audio;

namespace Game
{
	// Token: 0x02000139 RID: 313
	public static class AudioManager
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00021A79 File Offset: 0x0001FC79
		public static float MinAudibleVolume
		{
			get
			{
				return 0.05f * SettingsManager.SoundsVolume;
			}
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00021A88 File Offset: 0x0001FC88
		public static void PlaySound(string name, float volume, float pitch, float pan)
		{
			if (SettingsManager.SoundsVolume > 0f)
			{
				float num = volume * SettingsManager.SoundsVolume;
				if (num > AudioManager.MinAudibleVolume)
				{
					try
					{
						SoundBuffer soundBuffer = ContentManager.Get<SoundBuffer>(name, ".wav");
						if (soundBuffer == null)
						{
							soundBuffer = ContentManager.Get<SoundBuffer>(name, ".ogg");
						}
						new Sound(soundBuffer, num, AudioManager.ToEnginePitch(pitch), pan, false, true).Play();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00021AF8 File Offset: 0x0001FCF8
		public static float ToEnginePitch(float pitch)
		{
			return MathUtils.Pow(2f, pitch);
		}
	}
}
