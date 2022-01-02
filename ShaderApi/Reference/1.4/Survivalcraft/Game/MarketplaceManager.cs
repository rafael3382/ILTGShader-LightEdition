using System;

namespace Game
{
	// Token: 0x02000146 RID: 326
	public static class MarketplaceManager
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0002483B File Offset: 0x00022A3B
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x00024842 File Offset: 0x00022A42
		public static bool IsTrialMode
		{
			get
			{
				return MarketplaceManager.m_isTrialMode;
			}
			set
			{
				MarketplaceManager.m_isTrialMode = value;
			}
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0002484A File Offset: 0x00022A4A
		public static void Initialize()
		{
			MarketplaceManager.m_isInitialized = true;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00024852 File Offset: 0x00022A52
		public static void ShowMarketplace()
		{
			WebBrowserManager.LaunchBrowser("http://play.google.com/store/apps/details?id=com.candyrufusgames.survivalcraft2");
		}

		// Token: 0x040002C4 RID: 708
		public static bool m_isInitialized;

		// Token: 0x040002C5 RID: 709
		public static bool m_isTrialMode;
	}
}
