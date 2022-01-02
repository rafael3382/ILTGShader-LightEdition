using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Game
{
	// Token: 0x02000270 RID: 624
	public class DiskExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x0600140B RID: 5131 RVA: 0x00095BB4 File Offset: 0x00093DB4
		public string DisplayName
		{
			get
			{
				return LanguageControl.Get(new string[]
				{
					DiskExternalContentProvider.fName,
					"DisplayName"
				});
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x00095BD1 File Offset: 0x00093DD1
		public bool SupportsLinks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x0600140D RID: 5133 RVA: 0x00095BD4 File Offset: 0x00093DD4
		public bool SupportsListing
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600140E RID: 5134 RVA: 0x00095BD7 File Offset: 0x00093DD7
		public bool RequiresLogin
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x0600140F RID: 5135 RVA: 0x00095BDA File Offset: 0x00093DDA
		public bool IsLoggedIn
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06001410 RID: 5136 RVA: 0x00095BDD File Offset: 0x00093DDD
		public string Description
		{
			get
			{
				return LanguageControl.Get(new string[]
				{
					DiskExternalContentProvider.fName,
					"Description"
				});
			}
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00095BFA File Offset: 0x00093DFA
		public DiskExternalContentProvider()
		{
			if (!Directory.Exists(DiskExternalContentProvider.LocalPath))
			{
				Directory.CreateDirectory(DiskExternalContentProvider.LocalPath);
			}
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00095C19 File Offset: 0x00093E19
		public void Dispose()
		{
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00095C1C File Offset: 0x00093E1C
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			FileStream fileStream = null;
			if (!File.Exists(path))
			{
				failure(new FileNotFoundException());
				return;
			}
			fileStream = File.OpenRead(path);
			ThreadPool.QueueUserWorkItem(delegate(object <p0>)
			{
				try
				{
					success(fileStream);
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			});
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00095C7C File Offset: 0x00093E7C
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00095C8A File Offset: 0x00093E8A
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			ExternalContentEntry entry = null;
			ThreadPool.QueueUserWorkItem(delegate(object <p0>)
			{
				try
				{
					string path2 = path;
					entry = this.GetDirectoryEntry(path2, true);
					success(entry);
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			});
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x00095CC6 File Offset: 0x00093EC6
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x00095CD3 File Offset: 0x00093ED3
		public void Logout()
		{
			throw new NotSupportedException();
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x00095CDC File Offset: 0x00093EDC
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			new SaveFileDialog();
			try
			{
				string path2 = Path.Combine(DiskExternalContentProvider.LocalPath, path);
				FileStream fileStream = (!File.Exists(path2)) ? File.Create(path2) : File.OpenWrite(path2);
				stream.CopyTo(fileStream);
				fileStream.Close();
				success(null);
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x00095D48 File Offset: 0x00093F48
		private ExternalContentEntry GetDirectoryEntry(string internalPath, bool scanContents)
		{
			ExternalContentEntry externalContentEntry = new ExternalContentEntry();
			externalContentEntry.Type = ExternalContentType.Directory;
			externalContentEntry.Path = internalPath;
			externalContentEntry.Time = new DateTime(1970, 1, 1);
			if (scanContents)
			{
				foreach (string internalPath2 in Directory.GetDirectories(internalPath))
				{
					externalContentEntry.ChildEntries.Add(this.GetDirectoryEntry(internalPath2, false));
				}
				foreach (string text in Directory.GetFiles(internalPath))
				{
					FileInfo fileInfo = new FileInfo(text);
					ExternalContentEntry externalContentEntry2 = new ExternalContentEntry();
					externalContentEntry2.Type = ExternalContentManager.ExtensionToType(Path.GetExtension(text));
					externalContentEntry2.Path = text;
					externalContentEntry2.Size = fileInfo.Length;
					externalContentEntry2.Time = fileInfo.CreationTime;
					externalContentEntry.ChildEntries.Add(externalContentEntry2);
				}
			}
			return externalContentEntry;
		}

		// Token: 0x04000CAF RID: 3247
		public static string fName = "DiskExternalContentProvider";

		// Token: 0x04000CB0 RID: 3248
		public static string LocalPath = AppDomain.CurrentDomain.BaseDirectory;
	}
}
