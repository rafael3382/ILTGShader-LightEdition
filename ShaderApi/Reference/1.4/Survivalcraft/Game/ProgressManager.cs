using System;
using Engine;

namespace Game
{
	// Token: 0x0200014B RID: 331
	public static class ProgressManager
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x00026204 File Offset: 0x00024404
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x0002620B File Offset: 0x0002440B
		public static string OperationName { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00026213 File Offset: 0x00024413
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x0002621A File Offset: 0x0002441A
		public static float Progress { get; set; }

		// Token: 0x0600068F RID: 1679 RVA: 0x00026222 File Offset: 0x00024422
		public static void UpdateProgress(string operationName, float progress)
		{
			ProgressManager.OperationName = operationName;
			ProgressManager.Progress = MathUtils.Saturate(progress);
		}
	}
}
