using System;
using System.Collections.Generic;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000184 RID: 388
	public class SubsystemAudio : Subsystem, IUpdateable
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x00038150 File Offset: 0x00036350
		public ReadOnlyList<Vector3> ListenerPositions
		{
			get
			{
				return new ReadOnlyList<Vector3>(this.m_listenerPositions);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x0003815D File Offset: 0x0003635D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00038160 File Offset: 0x00036360
		public float CalculateListenerDistanceSquared(Vector3 p)
		{
			float num = float.MaxValue;
			for (int i = 0; i < this.m_listenerPositions.Count; i++)
			{
				float num2 = Vector3.DistanceSquared(this.m_listenerPositions[i], p);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x000381A3 File Offset: 0x000363A3
		public float CalculateListenerDistance(Vector3 p)
		{
			return MathUtils.Sqrt(this.CalculateListenerDistanceSquared(p));
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x000381B4 File Offset: 0x000363B4
		public void Mute()
		{
			foreach (Sound sound in this.m_sounds)
			{
				if (sound.State == SoundState.Playing)
				{
					this.m_mutedSounds[sound] = true;
					sound.Pause();
				}
			}
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0003821C File Offset: 0x0003641C
		public void Unmute()
		{
			foreach (Sound sound in this.m_mutedSounds.Keys)
			{
				sound.Play();
			}
			this.m_mutedSounds.Clear();
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0003827C File Offset: 0x0003647C
		public void PlaySound(string name, float volume, float pitch, float pan, float delay)
		{
			double num = this.m_subsystemTime.GameTime + (double)delay;
			this.m_nextSoundTime = MathUtils.Min(this.m_nextSoundTime, num);
			this.m_queuedSounds.Add(new SubsystemAudio.SoundInfo
			{
				Time = num,
				Name = name,
				Volume = volume,
				Pitch = pitch,
				Pan = pan
			});
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x000382E8 File Offset: 0x000364E8
		public void PlaySound(string name, float volume, float pitch, Vector3 position, float minDistance, float delay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlaySound(name, volume * num, pitch, 0f, delay);
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x00038320 File Offset: 0x00036520
		public void PlaySound(string name, float volume, float pitch, Vector3 position, float minDistance, bool autoDelay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlaySound(name, volume * num, pitch, 0f, autoDelay ? this.CalculateDelay(position) : 0f);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x00038368 File Offset: 0x00036568
		public void PlayRandomSound(string directory, float volume, float pitch, float pan, float delay)
		{
			ReadOnlyList<ContentInfo> readOnlyList = ContentManager.List(directory);
			if (readOnlyList.Count > 0)
			{
				int index = this.m_random.Int(0, readOnlyList.Count - 1);
				this.PlaySound(readOnlyList[index].ContentPath, volume, pitch, pan, delay);
				return;
			}
			Log.Warning("Sounds directory \"{0}\" not found or empty.", new object[]
			{
				directory
			});
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x000383CC File Offset: 0x000365CC
		public void PlayRandomSound(string directory, float volume, float pitch, Vector3 position, float minDistance, float delay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlayRandomSound(directory, volume * num, pitch, 0f, delay);
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00038404 File Offset: 0x00036604
		public void PlayRandomSound(string directory, float volume, float pitch, Vector3 position, float minDistance, bool autoDelay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlayRandomSound(directory, volume * num, pitch, 0f, autoDelay ? this.CalculateDelay(position) : 0f);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0003844C File Offset: 0x0003664C
		public Sound CreateSound(string name)
		{
			Sound sound = new Sound(ContentManager.Get<SoundBuffer>(name, null), 1f, 1f, 0f, false, false);
			this.m_sounds.Add(sound);
			return sound;
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x00038484 File Offset: 0x00036684
		public float CalculateVolume(float distance, float minDistance, float rolloffFactor = 2f)
		{
			if (distance > minDistance)
			{
				return minDistance / (minDistance + MathUtils.Max(rolloffFactor * (distance - minDistance), 0f));
			}
			return 1f;
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x000384A3 File Offset: 0x000366A3
		public float CalculateDelay(Vector3 position)
		{
			return this.CalculateDelay(this.CalculateListenerDistance(position));
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x000384B2 File Offset: 0x000366B2
		public float CalculateDelay(float distance)
		{
			return MathUtils.Min(distance / 100f, 5f);
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x000384C8 File Offset: 0x000366C8
		public void Update(float dt)
		{
			this.m_listenerPositions.Clear();
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				this.m_listenerPositions.Add(gameWidget.ActiveCamera.ViewPosition);
			}
			if (this.m_subsystemTime.GameTime < this.m_nextSoundTime)
			{
				return;
			}
			this.m_nextSoundTime = double.MaxValue;
			int i = 0;
			while (i < this.m_queuedSounds.Count)
			{
				SubsystemAudio.SoundInfo soundInfo = this.m_queuedSounds[i];
				if (this.m_subsystemTime.GameTime >= soundInfo.Time)
				{
					if (this.m_subsystemTime.GameTimeFactor == 1f && this.m_subsystemTime.FixedTimeStep == null && soundInfo.Volume * SettingsManager.SoundsVolume > AudioManager.MinAudibleVolume && this.UpdateCongestion(soundInfo.Name, soundInfo.Volume))
					{
						AudioManager.PlaySound(soundInfo.Name, soundInfo.Volume, soundInfo.Pitch, soundInfo.Pan);
					}
					this.m_queuedSounds.RemoveAt(i);
				}
				else
				{
					this.m_nextSoundTime = MathUtils.Min(this.m_nextSoundTime, soundInfo.Time);
					i++;
				}
			}
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0003863C File Offset: 0x0003683C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00038664 File Offset: 0x00036864
		public override void Dispose()
		{
			foreach (Sound sound in this.m_sounds)
			{
				sound.Dispose();
			}
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x000386B4 File Offset: 0x000368B4
		public bool UpdateCongestion(string name, float volume)
		{
			SubsystemAudio.Congestion congestion;
			if (!this.m_congestions.TryGetValue(name, out congestion))
			{
				congestion = new SubsystemAudio.Congestion();
				this.m_congestions.Add(name, congestion);
			}
			double realTime = Time.RealTime;
			double lastUpdateTime = congestion.LastUpdateTime;
			double lastPlayedTime = congestion.LastPlayedTime;
			float num = (lastUpdateTime > 0.0) ? ((float)(realTime - lastUpdateTime)) : 0f;
			congestion.Value = MathUtils.Max(congestion.Value - 10f * num, 0f);
			congestion.LastUpdateTime = realTime;
			if (congestion.Value <= 6f && (lastPlayedTime == 0.0 || volume > congestion.LastPlayedVolume || realTime - lastPlayedTime >= 0.0))
			{
				congestion.LastPlayedTime = realTime;
				congestion.LastPlayedVolume = volume;
				congestion.Value += 1f;
				return true;
			}
			return false;
		}

		// Token: 0x04000479 RID: 1145
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400047A RID: 1146
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x0400047B RID: 1147
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400047C RID: 1148
		public List<Vector3> m_listenerPositions = new List<Vector3>();

		// Token: 0x0400047D RID: 1149
		public Dictionary<string, SubsystemAudio.Congestion> m_congestions = new Dictionary<string, SubsystemAudio.Congestion>();

		// Token: 0x0400047E RID: 1150
		public double m_nextSoundTime;

		// Token: 0x0400047F RID: 1151
		public List<SubsystemAudio.SoundInfo> m_queuedSounds = new List<SubsystemAudio.SoundInfo>();

		// Token: 0x04000480 RID: 1152
		public List<Sound> m_sounds = new List<Sound>();

		// Token: 0x04000481 RID: 1153
		public Dictionary<Sound, bool> m_mutedSounds = new Dictionary<Sound, bool>();

		// Token: 0x0200047A RID: 1146
		public class Congestion
		{
			// Token: 0x04001665 RID: 5733
			public double LastUpdateTime;

			// Token: 0x04001666 RID: 5734
			public double LastPlayedTime;

			// Token: 0x04001667 RID: 5735
			public float LastPlayedVolume;

			// Token: 0x04001668 RID: 5736
			public float Value;
		}

		// Token: 0x0200047B RID: 1147
		public struct SoundInfo
		{
			// Token: 0x04001669 RID: 5737
			public double Time;

			// Token: 0x0400166A RID: 5738
			public string Name;

			// Token: 0x0400166B RID: 5739
			public float Volume;

			// Token: 0x0400166C RID: 5740
			public float Pitch;

			// Token: 0x0400166D RID: 5741
			public float Pan;
		}
	}
}
