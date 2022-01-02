using System;
using System.Windows.Forms;

namespace Game
{
	// Token: 0x0200013C RID: 316
	public static class ClipboardManager
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x0002253E File Offset: 0x0002073E
		// (set) Token: 0x0600060C RID: 1548 RVA: 0x00022545 File Offset: 0x00020745
		public static string ClipboardString
		{
			get
			{
				return Clipboard.GetText();
			}
			set
			{
				Clipboard.SetText(value);
			}
		}
	}
}
