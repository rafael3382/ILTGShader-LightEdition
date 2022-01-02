using System;
using System.IO;

namespace Game
{
	// Token: 0x02000364 RID: 868
	public class ContentInfo
	{
		// Token: 0x06001959 RID: 6489 RVA: 0x000C84D8 File Offset: 0x000C66D8
		public ContentInfo(string AbsolutePath_)
		{
			this.AbsolutePath = AbsolutePath_;
			int num = AbsolutePath_.LastIndexOf('.');
			this.ContentPath = ((num > -1) ? AbsolutePath_.Substring(0, num) : AbsolutePath_);
			this.Filename = Path.GetFileName(this.AbsolutePath);
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x000C8521 File Offset: 0x000C6721
		public void SetContentStream(Stream stream)
		{
			if (stream is MemoryStream)
			{
				this.ContentStream = (stream as MemoryStream);
				this.ContentStream.Position = 0L;
				return;
			}
			throw new Exception("Can't set ContentStream width type " + stream.GetType().Name);
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x000C8560 File Offset: 0x000C6760
		public Stream Duplicate()
		{
			if (this.ContentStream == null || !this.ContentStream.CanRead || !this.ContentStream.CanWrite)
			{
				throw new Exception("ContentStream has been disposed");
			}
			MemoryStream memoryStream = new MemoryStream();
			this.ContentStream.CopyTo(memoryStream);
			this.ContentStream.Position = 0L;
			memoryStream.Position = 0L;
			return memoryStream;
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x000C85C2 File Offset: 0x000C67C2
		public void Dispose()
		{
			MemoryStream contentStream = this.ContentStream;
			if (contentStream == null)
			{
				return;
			}
			contentStream.Dispose();
		}

		// Token: 0x04001179 RID: 4473
		public MemoryStream ContentStream;

		// Token: 0x0400117A RID: 4474
		public string AbsolutePath;

		// Token: 0x0400117B RID: 4475
		public string ContentPath;

		// Token: 0x0400117C RID: 4476
		public string Filename;

		// Token: 0x0400117D RID: 4477
		public object Instance;
	}
}
