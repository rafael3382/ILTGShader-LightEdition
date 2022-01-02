using System;
using System.Diagnostics;
using Engine;

namespace Game
{
	// Token: 0x02000150 RID: 336
	public static class WebBrowserManager
	{
		// Token: 0x06000720 RID: 1824 RVA: 0x00027518 File Offset: 0x00025718
		public static void LaunchBrowser(string url)
		{
			if (!url.Contains("://"))
			{
				url = "http://" + url;
			}
			try
			{
				Process.Start(url);
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error launching web browser with URL \"{0}\". Reason: {1}", new object[]
				{
					url,
					ex.Message
				}));
			}
		}
	}
}
