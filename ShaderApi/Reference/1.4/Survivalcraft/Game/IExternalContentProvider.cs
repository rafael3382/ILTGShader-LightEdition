using System;
using System.IO;

namespace Game
{
	// Token: 0x020002AE RID: 686
	public interface IExternalContentProvider : IDisposable
	{
		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001534 RID: 5428
		string DisplayName { get; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001535 RID: 5429
		bool SupportsLinks { get; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001536 RID: 5430
		bool SupportsListing { get; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001537 RID: 5431
		bool RequiresLogin { get; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06001538 RID: 5432
		bool IsLoggedIn { get; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001539 RID: 5433
		string Description { get; }

		// Token: 0x0600153A RID: 5434
		void Login(CancellableProgress progress, Action success, Action<Exception> failure);

		// Token: 0x0600153B RID: 5435
		void Logout();

		// Token: 0x0600153C RID: 5436
		void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure);

		// Token: 0x0600153D RID: 5437
		void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure);

		// Token: 0x0600153E RID: 5438
		void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure);

		// Token: 0x0600153F RID: 5439
		void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure);
	}
}
