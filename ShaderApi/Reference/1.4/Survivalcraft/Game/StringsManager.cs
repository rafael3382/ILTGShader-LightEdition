using System;

namespace Game
{
	// Token: 0x0200014D RID: 333
	public static class StringsManager
	{
		// Token: 0x0600070B RID: 1803 RVA: 0x00026E44 File Offset: 0x00025044
		public static string GetString(string name)
		{
			return LanguageControl.Get(new string[]
			{
				"Strings",
				name
			});
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00026E5D File Offset: 0x0002505D
		public static void LoadStrings()
		{
		}
	}
}
