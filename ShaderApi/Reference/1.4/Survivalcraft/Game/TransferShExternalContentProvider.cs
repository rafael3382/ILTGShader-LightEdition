using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game
{
	// Token: 0x02000331 RID: 817
	public class TransferShExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06001848 RID: 6216 RVA: 0x000BFF66 File Offset: 0x000BE166
		public string DisplayName
		{
			get
			{
				return "transfer.sh";
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06001849 RID: 6217 RVA: 0x000BFF6D File Offset: 0x000BE16D
		public string Description
		{
			get
			{
				return "No login required";
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600184A RID: 6218 RVA: 0x000BFF74 File Offset: 0x000BE174
		public bool SupportsListing
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600184B RID: 6219 RVA: 0x000BFF77 File Offset: 0x000BE177
		public bool SupportsLinks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600184C RID: 6220 RVA: 0x000BFF7A File Offset: 0x000BE17A
		public bool RequiresLogin
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x0600184D RID: 6221 RVA: 0x000BFF7D File Offset: 0x000BE17D
		public bool IsLoggedIn
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x000BFF80 File Offset: 0x000BE180
		public void Dispose()
		{
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x000BFF82 File Offset: 0x000BE182
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x000BFF8F File Offset: 0x000BE18F
		public void Logout()
		{
			throw new NotSupportedException();
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x000BFF96 File Offset: 0x000BE196
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x000BFFA4 File Offset: 0x000BE1A4
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x000BFFB4 File Offset: 0x000BE1B4
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/octet-stream");
				WebManager.Put("https://transfer.sh/" + path, null, dictionary, stream, progress, delegate(byte[] result)
				{
					string obj2 = Encoding.UTF8.GetString(result, 0, result.Length).Trim();
					success(obj2);
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x000C003C File Offset: 0x000BE23C
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}
	}
}
