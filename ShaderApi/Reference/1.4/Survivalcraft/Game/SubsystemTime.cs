using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DA RID: 474
	public class SubsystemTime : Subsystem
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x0005D12A File Offset: 0x0005B32A
		public double GameTime
		{
			get
			{
				return this.m_gameTime;
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000CE4 RID: 3300 RVA: 0x0005D132 File Offset: 0x0005B332
		public float GameTimeDelta
		{
			get
			{
				return this.m_gameTimeDelta;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0005D13A File Offset: 0x0005B33A
		public float PreviousGameTimeDelta
		{
			get
			{
				return this.m_prevGameTimeDelta;
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x0005D142 File Offset: 0x0005B342
		// (set) Token: 0x06000CE7 RID: 3303 RVA: 0x0005D14A File Offset: 0x0005B34A
		public float GameTimeFactor
		{
			get
			{
				return this.m_gameTimeFactor;
			}
			set
			{
				this.m_gameTimeFactor = MathUtils.Clamp(value, 0f, 256f);
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x0005D162 File Offset: 0x0005B362
		// (set) Token: 0x06000CE9 RID: 3305 RVA: 0x0005D16A File Offset: 0x0005B36A
		public float? FixedTimeStep { get; set; }

		// Token: 0x06000CEA RID: 3306 RVA: 0x0005D174 File Offset: 0x0005B374
		public void NextFrame()
		{
			this.m_prevGameTimeDelta = this.m_gameTimeDelta;
			this.m_gameTimeDelta = ((this.FixedTimeStep == null) ? MathUtils.Min(Time.FrameDuration * this.m_gameTimeFactor, 0.1f) : MathUtils.Min(this.FixedTimeStep.Value * this.m_gameTimeFactor, 0.1f));
			this.m_gameTime += (double)this.m_gameTimeDelta;
			int i = 0;
			while (i < this.m_delayedExecutionsRequests.Count)
			{
				SubsystemTime.DelayedExecutionRequest delayedExecutionRequest = this.m_delayedExecutionsRequests[i];
				if (delayedExecutionRequest.GameTime >= 0.0 && this.GameTime >= delayedExecutionRequest.GameTime)
				{
					this.m_delayedExecutionsRequests.RemoveAt(i);
					delayedExecutionRequest.Action();
				}
				else
				{
					i++;
				}
			}
			int num = 0;
			int num2 = 0;
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				if (componentPlayer.ComponentHealth.Health == 0f)
				{
					num2++;
				}
				else if (componentPlayer.ComponentSleep.SleepFactor == 1f)
				{
					num++;
				}
			}
			if (num + num2 == this.m_subsystemPlayers.ComponentPlayers.Count && num >= 1)
			{
				this.FixedTimeStep = new float?(0.05f);
				this.m_subsystemUpdate.UpdatesPerFrame = 20;
			}
			else
			{
				this.FixedTimeStep = null;
				this.m_subsystemUpdate.UpdatesPerFrame = 1;
			}
			bool flag = true;
			using (ReadOnlyList<ComponentPlayer>.Enumerator enumerator = this.m_subsystemPlayers.ComponentPlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ComponentGui.IsGameMenuDialogVisible())
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.GameTimeFactor = 0f;
				return;
			}
			if (this.GameTimeFactor == 0f)
			{
				this.GameTimeFactor = 1f;
			}
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0005D3A4 File Offset: 0x0005B5A4
		public void QueueGameTimeDelayedExecution(double gameTime, Action action)
		{
			this.m_delayedExecutionsRequests.Add(new SubsystemTime.DelayedExecutionRequest
			{
				GameTime = gameTime,
				Action = action
			});
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0005D3D8 File Offset: 0x0005B5D8
		public bool PeriodicGameTimeEvent(double period, double offset)
		{
			double num = this.GameTime - offset;
			double num2 = MathUtils.Floor(num / period) * period;
			return num >= num2 && num - (double)this.GameTimeDelta < num2;
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0005D40B File Offset: 0x0005B60B
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemUpdate = base.Project.FindSubsystem<SubsystemUpdate>(true);
		}

		// Token: 0x040006A3 RID: 1699
		public const float MaxGameTimeDelta = 0.1f;

		// Token: 0x040006A4 RID: 1700
		public double m_gameTime;

		// Token: 0x040006A5 RID: 1701
		public float m_gameTimeDelta;

		// Token: 0x040006A6 RID: 1702
		public float m_prevGameTimeDelta;

		// Token: 0x040006A7 RID: 1703
		public float m_gameTimeFactor = 1f;

		// Token: 0x040006A8 RID: 1704
		public List<SubsystemTime.DelayedExecutionRequest> m_delayedExecutionsRequests = new List<SubsystemTime.DelayedExecutionRequest>();

		// Token: 0x040006A9 RID: 1705
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040006AA RID: 1706
		public SubsystemUpdate m_subsystemUpdate;

		// Token: 0x020004B1 RID: 1201
		public struct DelayedExecutionRequest
		{
			// Token: 0x0400174E RID: 5966
			public double GameTime;

			// Token: 0x0400174F RID: 5967
			public Action Action;
		}
	}
}
