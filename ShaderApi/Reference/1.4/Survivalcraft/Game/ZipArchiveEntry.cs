using System;

namespace Game
{
	// Token: 0x02000362 RID: 866
	public class ZipArchiveEntry
	{
		// Token: 0x06001943 RID: 6467 RVA: 0x000C70E8 File Offset: 0x000C52E8
		public override string ToString()
		{
			return this.FilenameInZip;
		}

		// Token: 0x04001168 RID: 4456
		public ZipArchive.Compression Method;

		// Token: 0x04001169 RID: 4457
		public string FilenameInZip;

		// Token: 0x0400116A RID: 4458
		public uint FileSize;

		// Token: 0x0400116B RID: 4459
		public uint CompressedSize;

		// Token: 0x0400116C RID: 4460
		public uint HeaderOffset;

		// Token: 0x0400116D RID: 4461
		public uint FileOffset;

		// Token: 0x0400116E RID: 4462
		public uint HeaderSize;

		// Token: 0x0400116F RID: 4463
		public uint Crc32;

		// Token: 0x04001170 RID: 4464
		public bool IsFilenameUtf8;

		// Token: 0x04001171 RID: 4465
		public DateTime ModifyTime;

		// Token: 0x04001172 RID: 4466
		public string Comment;

		// Token: 0x04001173 RID: 4467
		public bool EncodeUTF8;
	}
}
