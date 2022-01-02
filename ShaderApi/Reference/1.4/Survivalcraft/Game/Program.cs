using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x020002EA RID: 746
	public static class Program
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001655 RID: 5717 RVA: 0x000A85D4 File Offset: 0x000A67D4
		// (set) Token: 0x06001656 RID: 5718 RVA: 0x000A85DB File Offset: 0x000A67DB
		public static float LastFrameTime { get; set; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001657 RID: 5719 RVA: 0x000A85E3 File Offset: 0x000A67E3
		// (set) Token: 0x06001658 RID: 5720 RVA: 0x000A85EA File Offset: 0x000A67EA
		public static float LastCpuFrameTime { get; set; }

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06001659 RID: 5721 RVA: 0x000A85F4 File Offset: 0x000A67F4
		// (remove) Token: 0x0600165A RID: 5722 RVA: 0x000A8628 File Offset: 0x000A6828
		public static event Action<Uri> HandleUri;

		// Token: 0x0600165B RID: 5723 RVA: 0x000A865C File Offset: 0x000A685C
		[STAThread]
		public static void Main()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			Log.RemoveAllLogSinks();
			Log.AddLogSink(new GameLogSink());
			Window.HandleUri += Program.HandleUriHandler;
			Window.Deactivated += Program.DeactivatedHandler;
			Window.Frame += Program.FrameHandler;
			Display.DeviceReset += ContentManager.Display_DeviceReset;
			Window.UnhandledException += delegate(UnhandledExceptionInfo e)
			{
				ExceptionManager.ReportExceptionToUser("Unhandled exception.", e.Exception);
				e.IsHandled = true;
			};
			Window.Run(1024, 768, WindowMode.Fullscreen, "生存战争2.2插件版V1.40");
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x000A8713 File Offset: 0x000A6913
		public static void HandleUriHandler(Uri uri)
		{
			Program.m_urisToHandle.Add(uri);
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x000A8720 File Offset: 0x000A6920
		public static void DeactivatedHandler()
		{
			GC.Collect();
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x000A8728 File Offset: 0x000A6928
		public static void FrameHandler()
		{
			if (Time.FrameIndex < 0)
			{
				Display.Clear(new Vector4?(Vector4.Zero), new float?(1f), null);
				return;
			}
			if (Time.FrameIndex == 0)
			{
				Program.Initialize();
				return;
			}
			Program.Run();
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x000A8774 File Offset: 0x000A6974
		public static void Initialize()
		{
			Log.Information(string.Format("Survivalcraft starting up at {0}, Version={1}, BuildConfiguration={2}, Platform={3}, Storage.AvailableFreeSpace={4}MB, ApproximateScreenDpi={5:0.0}, ApproxScreenInches={6:0.0}, ScreenResolution={7}, ProcessorsCount={8}, RAM={9}MB, 64bit={10}", new object[]
			{
				DateTime.Now,
				VersionsManager.Version,
				VersionsManager.BuildConfiguration,
				VersionsManager.Platform,
				Storage.FreeSpace / 1024L / 1024L,
				ScreenResolutionManager.ApproximateScreenDpi,
				ScreenResolutionManager.ApproximateScreenInches,
				Window.Size,
				Environment.ProcessorCount,
				Utilities.GetTotalAvailableMemory() / 1024 / 1024,
				Marshal.SizeOf<IntPtr>() == 8
			}));
			try
			{
				SettingsManager.Initialize();
				VersionsManager.Initialize();
				ExternalContentManager.Initialize();
				ScreensManager.Initialize();
				Log.Information("Program Initialize Success");
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
			}
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x000A887C File Offset: 0x000A6A7C
		public static void Run()
		{
			Program.LastFrameTime = (float)(Time.RealTime - Program.m_frameBeginTime);
			Program.LastCpuFrameTime = (float)(Program.m_cpuEndTime - Program.m_frameBeginTime);
			Program.m_frameBeginTime = Time.RealTime;
			if (Keyboard.IsKeyDown(Key.F11))
			{
				SettingsManager.WindowMode = ((SettingsManager.WindowMode == WindowMode.Fullscreen) ? WindowMode.Resizable : WindowMode.Fullscreen);
			}
			try
			{
				if (ExceptionManager.Error == null)
				{
					while (Program.m_urisToHandle.Count > 0)
					{
						Uri obj = Program.m_urisToHandle[0];
						Program.m_urisToHandle.RemoveAt(0);
						Action<Uri> handleUri = Program.HandleUri;
						if (handleUri != null)
						{
							handleUri(obj);
						}
					}
					PerformanceManager.Update();
					MotdManager.Update();
					MusicManager.Update();
					ScreensManager.Update();
					DialogsManager.Update();
				}
				else
				{
					ExceptionManager.UpdateExceptionScreen();
				}
			}
			catch (Exception e)
			{
				ModsManager.AddException(e, false);
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			try
			{
				Display.RenderTarget = null;
				if (ExceptionManager.Error == null)
				{
					ScreensManager.Draw();
					PerformanceManager.Draw();
					ScreenCaptureManager.Run();
				}
				else
				{
					ExceptionManager.DrawExceptionScreen();
				}
				Program.m_cpuEndTime = Time.RealTime;
			}
			catch (Exception e2)
			{
				if (GameManager.Project != null)
				{
					GameManager.DisposeProject();
				}
				ExceptionManager.ReportExceptionToUser(null, e2);
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x04000F16 RID: 3862
		public static double m_frameBeginTime;

		// Token: 0x04000F17 RID: 3863
		public static double m_cpuEndTime;

		// Token: 0x04000F18 RID: 3864
		public static List<Uri> m_urisToHandle = new List<Uri>();
	}
}
