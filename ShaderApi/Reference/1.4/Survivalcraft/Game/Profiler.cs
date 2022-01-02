using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x020002E9 RID: 745
	public struct Profiler : IDisposable
	{
		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001649 RID: 5705 RVA: 0x000A809A File Offset: 0x000A629A
		public static int MaxNameLength
		{
			get
			{
				return Profiler.m_maxNameLength;
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600164A RID: 5706 RVA: 0x000A80A4 File Offset: 0x000A62A4
		public static ReadOnlyList<Profiler.Metric> Metrics
		{
			get
			{
				if (Profiler.m_sortNeeded)
				{
					Profiler.m_sortedMetrics.Sort((Profiler.Metric x, Profiler.Metric y) => string.CompareOrdinal(x.Name, y.Name));
					Profiler.m_sortNeeded = false;
				}
				return new ReadOnlyList<Profiler.Metric>(Profiler.m_sortedMetrics);
			}
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x000A80F4 File Offset: 0x000A62F4
		public Profiler(string name)
		{
			if (Profiler.Enabled)
			{
				if (!Profiler.m_metrics.TryGetValue(name, out this.m_metric))
				{
					this.m_metric = new Profiler.Metric();
					this.m_metric.Name = name;
					Profiler.m_maxNameLength = MathUtils.Max(Profiler.m_maxNameLength, name.Length);
					Profiler.m_metrics.Add(name, this.m_metric);
					Profiler.m_sortedMetrics.Add(this.m_metric);
					Profiler.m_sortNeeded = true;
				}
				this.m_startTicks = Stopwatch.GetTimestamp();
				return;
			}
			this.m_startTicks = 0L;
			this.m_metric = null;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x000A818C File Offset: 0x000A638C
		public void Dispose()
		{
			if (this.m_metric != null)
			{
				long num = Stopwatch.GetTimestamp() - this.m_startTicks;
				this.m_metric.TotalTicks += num;
				this.m_metric.MaxTicks = MathUtils.Max(this.m_metric.MaxTicks, num);
				this.m_metric.HitCount++;
				this.m_metric = null;
				return;
			}
			throw new InvalidOperationException("Profiler.Dispose called without a matching constructor.");
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x000A8204 File Offset: 0x000A6404
		public static void Sample()
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				float sample = (float)metric.TotalTicks / (float)Stopwatch.Frequency;
				metric.AverageHitCount.AddSample((float)metric.HitCount);
				metric.AverageTime.AddSample(sample);
				metric.HitCount = 0;
				metric.TotalTicks = 0L;
				metric.MaxTicks = 0L;
			}
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x000A8298 File Offset: 0x000A6498
		public static void ReportAverage(Profiler.Metric metric, StringBuilder text)
		{
			int num = Profiler.m_maxNameLength + 2;
			int length = text.Length;
			text.Append(metric.Name);
			text.Append('.', Math.Max(1, num - text.Length + length));
			text.AppendNumber(metric.AverageHitCount.Value, 2);
			text.Append("x");
			text.Append('.', Math.Max(1, num + 9 - text.Length + length));
			Profiler.FormatTimeSimple(text, metric.AverageTime.Value);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x000A8328 File Offset: 0x000A6528
		public static void ReportFrame(Profiler.Metric metric, StringBuilder text)
		{
			int num = Profiler.m_maxNameLength + 2;
			int length = text.Length;
			text.Append(metric.Name);
			text.Append('.', Math.Max(1, num - text.Length + length));
			Profiler.FormatTimeSimple(text, (float)metric.TotalTicks / (float)Stopwatch.Frequency);
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x000A8380 File Offset: 0x000A6580
		public static void ReportAverage(StringBuilder text)
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				Profiler.ReportAverage(metric, text);
				text.Append("\n");
			}
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000A83E0 File Offset: 0x000A65E0
		public static void ReportFrame(StringBuilder text)
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				Profiler.ReportFrame(metric, text);
				text.Append("\n");
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x000A8440 File Offset: 0x000A6640
		public static void FormatTimeSimple(StringBuilder text, float time)
		{
			text.AppendNumber(time * 1000f, 3);
			text.Append("ms");
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x000A845C File Offset: 0x000A665C
		public static void FormatTime(StringBuilder text, float time)
		{
			if (time >= 1f)
			{
				text.AppendNumber(time, 2);
				text.Append("s");
				return;
			}
			if (time >= 0.1f)
			{
				text.AppendNumber(time * 1000f, 0);
				text.Append("ms");
				return;
			}
			if (time >= 0.01f)
			{
				text.AppendNumber(time * 1000f, 1);
				text.Append("ms");
				return;
			}
			if (time >= 0.001f)
			{
				text.AppendNumber(time * 1000f, 2);
				text.Append("ms");
				return;
			}
			if (time >= 0.0001f)
			{
				text.AppendNumber(time * 1000000f, 0);
				text.Append("us");
				return;
			}
			if (time >= 1E-05f)
			{
				text.AppendNumber(time * 1000000f, 1);
				text.Append("us");
				return;
			}
			if (time >= 1E-06f)
			{
				text.AppendNumber(time * 1000000f, 2);
				text.Append("us");
				return;
			}
			if (time >= 1E-07f)
			{
				text.AppendNumber(time * 1E+09f, 0);
				text.Append("ns");
				return;
			}
			if (time >= 1E-08f)
			{
				text.AppendNumber(time * 1E+09f, 1);
				text.Append("ns");
				return;
			}
			text.AppendNumber(time * 1E+09f, 2);
			text.Append("ns");
		}

		// Token: 0x04000F0F RID: 3855
		public static Dictionary<string, Profiler.Metric> m_metrics = new Dictionary<string, Profiler.Metric>();

		// Token: 0x04000F10 RID: 3856
		public static List<Profiler.Metric> m_sortedMetrics = new List<Profiler.Metric>();

		// Token: 0x04000F11 RID: 3857
		public static int m_maxNameLength;

		// Token: 0x04000F12 RID: 3858
		public static bool m_sortNeeded;

		// Token: 0x04000F13 RID: 3859
		public long m_startTicks;

		// Token: 0x04000F14 RID: 3860
		public Profiler.Metric m_metric;

		// Token: 0x04000F15 RID: 3861
		public static bool Enabled = true;

		// Token: 0x0200052C RID: 1324
		public class Metric
		{
			// Token: 0x040018B7 RID: 6327
			public string Name;

			// Token: 0x040018B8 RID: 6328
			public int HitCount;

			// Token: 0x040018B9 RID: 6329
			public long TotalTicks;

			// Token: 0x040018BA RID: 6330
			public long MaxTicks;

			// Token: 0x040018BB RID: 6331
			public readonly RunningAverage AverageHitCount = new RunningAverage(5f);

			// Token: 0x040018BC RID: 6332
			public readonly RunningAverage AverageTime = new RunningAverage(5f);
		}
	}
}
