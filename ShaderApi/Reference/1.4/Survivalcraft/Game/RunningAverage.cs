using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x020002FA RID: 762
	public class RunningAverage
	{
		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060016A4 RID: 5796 RVA: 0x000AA5A6 File Offset: 0x000A87A6
		public float Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x000AA5AE File Offset: 0x000A87AE
		public RunningAverage(float period)
		{
			this.m_period = (long)(period * (float)Stopwatch.Frequency);
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x000AA5C8 File Offset: 0x000A87C8
		public void AddSample(float sample)
		{
			this.m_sumValues += sample;
			this.m_countValues++;
			long timestamp = Stopwatch.GetTimestamp();
			if (timestamp >= this.m_startTicks + this.m_period)
			{
				this.m_value = this.m_sumValues / (float)this.m_countValues;
				this.m_sumValues = 0f;
				this.m_countValues = 0;
				this.m_startTicks = timestamp;
			}
		}

		// Token: 0x04000F61 RID: 3937
		public long m_startTicks;

		// Token: 0x04000F62 RID: 3938
		public long m_period;

		// Token: 0x04000F63 RID: 3939
		public float m_sumValues;

		// Token: 0x04000F64 RID: 3940
		public int m_countValues;

		// Token: 0x04000F65 RID: 3941
		public float m_value;
	}
}
