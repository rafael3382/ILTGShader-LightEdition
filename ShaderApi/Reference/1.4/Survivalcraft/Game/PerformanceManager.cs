using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000149 RID: 329
	public static class PerformanceManager
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x00024F16 File Offset: 0x00023116
		public static float? LongTermAverageFrameTime
		{
			get
			{
				return PerformanceManager.m_longTermAverageFrameTime;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000679 RID: 1657 RVA: 0x00024F1D File Offset: 0x0002311D
		public static float AverageFrameTime
		{
			get
			{
				return PerformanceManager.m_averageFrameTime.Value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x00024F29 File Offset: 0x00023129
		public static float AverageCpuFrameTime
		{
			get
			{
				return PerformanceManager.m_averageCpuFrameTime.Value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x00024F35 File Offset: 0x00023135
		public static long TotalMemoryUsed
		{
			get
			{
				return PerformanceManager.m_totalMemoryUsed;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x00024F3C File Offset: 0x0002313C
		public static long TotalGpuMemoryUsed
		{
			get
			{
				return PerformanceManager.m_totalGpuMemoryUsed;
			}
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00024F44 File Offset: 0x00023144
		static PerformanceManager()
		{
			PerformanceManager.m_stateMachine.AddState("PreMeasure", delegate
			{
				PerformanceManager.m_totalGameTime = 0.0;
			}, delegate
			{
				if (GameManager.Project != null)
				{
					PerformanceManager.m_totalGameTime += (double)Time.FrameDuration;
					if (PerformanceManager.m_totalGameTime > 60.0)
					{
						PerformanceManager.m_stateMachine.TransitionTo("Measuring");
					}
				}
			}, null);
			PerformanceManager.m_stateMachine.AddState("Measuring", delegate
			{
				PerformanceManager.m_totalFrameTime = 0.0;
				PerformanceManager.m_totalCpuFrameTime = 0.0;
				PerformanceManager.m_frameCount = 0;
			}, delegate
			{
				if (GameManager.Project != null)
				{
					if (ScreensManager.CurrentScreen != null && ScreensManager.CurrentScreen.GetType() == typeof(GameScreen))
					{
						float lastFrameTime = Program.LastFrameTime;
						float lastCpuFrameTime = Program.LastCpuFrameTime;
						if (lastFrameTime > 0f && lastFrameTime < 1f && lastCpuFrameTime > 0f && lastCpuFrameTime < 1f)
						{
							PerformanceManager.m_totalFrameTime += (double)lastFrameTime;
							PerformanceManager.m_totalCpuFrameTime += (double)lastCpuFrameTime;
							PerformanceManager.m_frameCount++;
						}
						if (PerformanceManager.m_totalFrameTime > 180.0)
						{
							PerformanceManager.m_stateMachine.TransitionTo("PostMeasure");
							return;
						}
					}
				}
				else
				{
					PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
				}
			}, null);
			PerformanceManager.m_stateMachine.AddState("PostMeasure", delegate
			{
				if (PerformanceManager.m_frameCount > 0)
				{
					PerformanceManager.m_longTermAverageFrameTime = new float?((float)(PerformanceManager.m_totalFrameTime / (double)PerformanceManager.m_frameCount));
					float num = (float)((int)MathUtils.Round(MathUtils.Round(PerformanceManager.m_totalFrameTime / (double)PerformanceManager.m_frameCount / 0.004999999888241291) * 0.004999999888241291 * 1000.0));
					float num2 = (float)((int)MathUtils.Round(MathUtils.Round(PerformanceManager.m_totalCpuFrameTime / (double)PerformanceManager.m_frameCount / 0.004999999888241291) * 0.004999999888241291 * 1000.0));
					Log.Information(string.Concat(new string[]
					{
						"PerformanceManager Measurement: frames=",
						PerformanceManager.m_frameCount.ToString(),
						", avgFrameTime=",
						num.ToString(),
						"ms, avgFrameCpuTime=",
						num2.ToString(),
						"ms"
					}));
				}
			}, delegate
			{
				if (GameManager.Project == null)
				{
					PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
				}
			}, null);
			PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0002502C File Offset: 0x0002322C
		public static void Update()
		{
			PerformanceManager.m_averageFrameTime.AddSample(Program.LastFrameTime);
			PerformanceManager.m_averageCpuFrameTime.AddSample(Program.LastCpuFrameTime);
			if (Time.PeriodicEvent(1.0, 0.0))
			{
				PerformanceManager.m_totalMemoryUsed = GC.GetTotalMemory(false);
				PerformanceManager.m_totalGpuMemoryUsed = Display.GetGpuMemoryUsage();
			}
			PerformanceManager.m_stateMachine.Update();
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x00025090 File Offset: 0x00023290
		public static void Draw()
		{
			Vector2 vector = new Vector2(MathUtils.Round(MathUtils.Clamp(ScreensManager.RootWidget.GlobalScale, 1f, 4f)));
			Viewport viewport = Display.Viewport;
			if (SettingsManager.DisplayFpsCounter)
			{
				if (Time.PeriodicEvent(1.0, 0.0) && ScreensManager.CurrentScreen != null)
				{
					PerformanceManager.m_statsString = string.Format("CPUMEM {0:0}MB, GPUMEM {1:0}MB, CPU {2:0}%, FPS {3:0.0}", new object[]
					{
						(float)PerformanceManager.TotalMemoryUsed / 1024f / 1024f,
						(float)PerformanceManager.TotalGpuMemoryUsed / 1024f / 1024f,
						PerformanceManager.AverageCpuFrameTime / PerformanceManager.AverageFrameTime * 100f,
						1f / PerformanceManager.AverageFrameTime
					});
				}
				PerformanceManager.m_primitivesRenderer.FontBatch(BitmapFont.DebugFont, 0, null, null, null, SamplerState.PointClamp).QueueText(PerformanceManager.m_statsString, Vector2.Transform(Vector2.Zero, ScreensManager.RootWidget.GlobalTransform), 0f, Color.White, TextAnchor.Default, vector, Vector2.Zero, 0f);
			}
			if (SettingsManager.DisplayFpsRibbon)
			{
				float num = ((float)viewport.Width / vector.X > 480f) ? (vector.X * 2f) : vector.X;
				float num2 = (float)viewport.Height / -0.1f;
				float num3 = (float)(viewport.Height - 1);
				float s = 0.5f;
				int num4 = MathUtils.Max((int)((float)viewport.Width / num), 1);
				if (PerformanceManager.m_frameData == null || PerformanceManager.m_frameData.Length != num4)
				{
					PerformanceManager.m_frameData = new PerformanceManager.FrameData[num4];
					PerformanceManager.m_frameDataIndex = 0;
				}
				PerformanceManager.m_frameData[PerformanceManager.m_frameDataIndex] = new PerformanceManager.FrameData
				{
					CpuTime = Program.LastCpuFrameTime,
					TotalTime = Program.LastFrameTime
				};
				PerformanceManager.m_frameDataIndex = (PerformanceManager.m_frameDataIndex + 1) % PerformanceManager.m_frameData.Length;
				FlatBatch2D flatBatch2D = PerformanceManager.m_primitivesRenderer.FlatBatch(0, null, null, null);
				Color color = Color.Orange * s;
				Color color2 = Color.Red * s;
				for (int i = PerformanceManager.m_frameData.Length - 1; i >= 0; i--)
				{
					int num5 = (i - PerformanceManager.m_frameData.Length + 1 + PerformanceManager.m_frameDataIndex + PerformanceManager.m_frameData.Length) % PerformanceManager.m_frameData.Length;
					PerformanceManager.FrameData frameData = PerformanceManager.m_frameData[num5];
					float x = (float)i * num;
					float x2 = (float)(i + 1) * num;
					flatBatch2D.QueueQuad(new Vector2(x, num3), new Vector2(x2, num3 + frameData.CpuTime * num2), 0f, color);
					flatBatch2D.QueueQuad(new Vector2(x, num3 + frameData.CpuTime * num2), new Vector2(x2, num3 + frameData.TotalTime * num2), 0f, color2);
				}
				flatBatch2D.QueueLine(new Vector2(0f, num3 + 0.0166666675f * num2), new Vector2((float)viewport.Width, num3 + 0.0166666675f * num2), 0f, Color.Green);
			}
			else
			{
				PerformanceManager.m_frameData = null;
			}
			PerformanceManager.m_primitivesRenderer.Flush(true, int.MaxValue);
		}

		// Token: 0x040002D1 RID: 721
		public static PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x040002D2 RID: 722
		public static RunningAverage m_averageFrameTime = new RunningAverage(1f);

		// Token: 0x040002D3 RID: 723
		public static RunningAverage m_averageCpuFrameTime = new RunningAverage(1f);

		// Token: 0x040002D4 RID: 724
		public static float? m_longTermAverageFrameTime;

		// Token: 0x040002D5 RID: 725
		public static long m_totalMemoryUsed;

		// Token: 0x040002D6 RID: 726
		public static long m_totalGpuMemoryUsed;

		// Token: 0x040002D7 RID: 727
		public static StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040002D8 RID: 728
		public static double m_totalGameTime;

		// Token: 0x040002D9 RID: 729
		public static double m_totalFrameTime;

		// Token: 0x040002DA RID: 730
		public static double m_totalCpuFrameTime;

		// Token: 0x040002DB RID: 731
		public static int m_frameCount;

		// Token: 0x040002DC RID: 732
		public static string m_statsString = string.Empty;

		// Token: 0x040002DD RID: 733
		public static PerformanceManager.FrameData[] m_frameData;

		// Token: 0x040002DE RID: 734
		public static int m_frameDataIndex;

		// Token: 0x02000438 RID: 1080
		public struct FrameData
		{
			// Token: 0x0400159E RID: 5534
			public float CpuTime;

			// Token: 0x0400159F RID: 5535
			public float TotalTime;
		}
	}
}
