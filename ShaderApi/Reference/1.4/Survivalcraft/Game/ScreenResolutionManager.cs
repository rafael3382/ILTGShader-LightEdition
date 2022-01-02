using System;
using Engine;

namespace Game
{
	// Token: 0x02000173 RID: 371
	public static class ScreenResolutionManager
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600085B RID: 2139 RVA: 0x00032CC4 File Offset: 0x00030EC4
		// (set) Token: 0x0600085C RID: 2140 RVA: 0x00032CCB File Offset: 0x00030ECB
		public static float ApproximateScreenDpi { get; set; } = 0.5f * (float)(Window.ScreenSize.X / Window.Size.X + Window.ScreenSize.Y / Window.Size.Y);

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x00032CD3 File Offset: 0x00030ED3
		public static float ApproximateScreenInches
		{
			get
			{
				return MathUtils.Sqrt((float)(Window.ScreenSize.X * Window.ScreenSize.X + Window.ScreenSize.Y * Window.ScreenSize.Y)) / ScreenResolutionManager.ApproximateScreenDpi;
			}
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x00032D0C File Offset: 0x00030F0C
		static ScreenResolutionManager()
		{
			ScreenResolutionManager.ApproximateScreenDpi = MathUtils.Clamp(ScreenResolutionManager.ApproximateScreenDpi, 96f, 800f);
		}
	}
}
