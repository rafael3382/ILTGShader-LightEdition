using System;
using System.Threading;

namespace Game
{
	// Token: 0x02000259 RID: 601
	public class CancellableProgress : Progress
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060013BC RID: 5052 RVA: 0x00093F04 File Offset: 0x00092104
		// (remove) Token: 0x060013BD RID: 5053 RVA: 0x00093F3C File Offset: 0x0009213C
		public event Action Cancelled;

		// Token: 0x060013BE RID: 5054 RVA: 0x00093F71 File Offset: 0x00092171
		public CancellableProgress()
		{
			this.CancellationToken = this.CancellationTokenSource.Token;
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x00093F95 File Offset: 0x00092195
		public void Cancel()
		{
			this.CancellationTokenSource.Cancel();
			Action cancelled = this.Cancelled;
			if (cancelled == null)
			{
				return;
			}
			cancelled();
		}

		// Token: 0x04000C52 RID: 3154
		public readonly CancellationToken CancellationToken;

		// Token: 0x04000C53 RID: 3155
		public readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
	}
}
