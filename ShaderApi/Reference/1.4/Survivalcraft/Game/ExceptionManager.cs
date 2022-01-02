using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x0200013F RID: 319
	public static class ExceptionManager
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x0002333B File Offset: 0x0002153B
		public static Exception Error
		{
			get
			{
				return ExceptionManager.m_error;
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00023342 File Offset: 0x00021542
		public static void ReportExceptionToUser(string additionalMessage, Exception e)
		{
			Log.Error(ExceptionManager.MakeFullErrorMessage(additionalMessage, e) + "\n" + e.StackTrace);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00023360 File Offset: 0x00021560
		public static void DrawExceptionScreen()
		{
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00023362 File Offset: 0x00021562
		public static void UpdateExceptionScreen()
		{
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00023364 File Offset: 0x00021564
		public static string MakeFullErrorMessage(Exception e)
		{
			return ExceptionManager.MakeFullErrorMessage(null, e);
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00023370 File Offset: 0x00021570
		public static string MakeFullErrorMessage(string additionalMessage, Exception e)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(additionalMessage))
			{
				text = additionalMessage;
			}
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				text = text + ((text.Length > 0) ? Environment.NewLine : string.Empty) + ex.Message;
			}
			return text;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000233BE File Offset: 0x000215BE
		public static bool CheckContinueKey()
		{
			return Keyboard.IsKeyDown(Key.F12) || Keyboard.IsKeyDown(Key.Back);
		}

		// Token: 0x040002B3 RID: 691
		public static Exception m_error;
	}
}
