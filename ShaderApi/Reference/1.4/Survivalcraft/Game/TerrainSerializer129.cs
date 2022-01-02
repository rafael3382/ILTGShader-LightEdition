using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000326 RID: 806
	public class TerrainSerializer129 : IDisposable
	{
		// Token: 0x06001802 RID: 6146 RVA: 0x000BC9BC File Offset: 0x000BABBC
		public TerrainSerializer129(Terrain terrain, string directoryName)
		{
			this.m_terrain = terrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer129.WriteTOCEntry(stream, 0, 0, -1);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer129.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num < 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = 786444L + 132112L * (long)num;
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x000BCA98 File Offset: 0x000BAC98
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x000BCAA1 File Offset: 0x000BACA1
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x000BCAC3 File Offset: 0x000BACC3
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x000BCAD0 File Offset: 0x000BACD0
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer129.ReadInt(stream);
			int num2 = TerrainSerializer129.ReadInt(stream);
			TerrainSerializer129.ReadInt(stream);
			TerrainSerializer129.ReadInt(stream);
			if (num != -559038737 || num2 != -2)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x000BCB0F File Offset: 0x000BAD0F
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer129.WriteInt(stream, -559038737);
			TerrainSerializer129.WriteInt(stream, -2);
			TerrainSerializer129.WriteInt(stream, cx);
			TerrainSerializer129.WriteInt(stream, cz);
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x000BCB32 File Offset: 0x000BAD32
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int index)
		{
			cx = TerrainSerializer129.ReadInt(stream);
			cz = TerrainSerializer129.ReadInt(stream);
			index = TerrainSerializer129.ReadInt(stream);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x000BCB4C File Offset: 0x000BAD4C
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int index)
		{
			TerrainSerializer129.WriteInt(stream, cx);
			TerrainSerializer129.WriteInt(stream, cz);
			TerrainSerializer129.WriteInt(stream, index);
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x000BCB64 File Offset: 0x000BAD64
		public unsafe bool LoadChunkBlocks(TerrainChunk chunk)
		{
			bool flag = false;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			bool result;
			try
			{
				long offset;
				if (!this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out offset))
				{
					result = flag;
				}
				else
				{
					double realTime = Time.RealTime;
					this.m_stream.Seek(offset, SeekOrigin.Begin);
					TerrainSerializer129.ReadChunkHeader(this.m_stream);
					this.m_stream.Read(this.m_buffer, 0, 131072);
					try
					{
						fixed (byte* ptr = &this.m_buffer[0])
						{
							int* ptr2 = (int*)ptr;
							for (int i = 0; i < 16; i++)
							{
								for (int j = 0; j < 16; j++)
								{
									int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
									int k = 0;
									while (k < 128)
									{
										chunk.SetCellValueFast(num3, *ptr2);
										k++;
										num3++;
										ptr2++;
									}
								}
							}
						}
					}
					finally
					{
						byte* ptr = null;
					}
					this.m_stream.Read(this.m_buffer, 0, 1024);
					try
					{
						fixed (byte* ptr = &this.m_buffer[0])
						{
							int* ptr3 = (int*)ptr;
							for (int l = 0; l < 16; l++)
							{
								for (int m = 0; m < 16; m++)
								{
									this.m_terrain.SetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y, *ptr3);
									ptr3++;
								}
							}
						}
					}
					finally
					{
						byte* ptr = null;
					}
					flag = true;
					double realTime2 = Time.RealTime;
					result = flag;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error loading data for chunk ({0},{1}).", num, num2), e));
				result = flag;
			}
			return result;
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x000BCD60 File Offset: 0x000BAF60
		public unsafe void SaveChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				bool flag = false;
				long length;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out length))
				{
					this.m_stream.Seek(length, SeekOrigin.Begin);
				}
				else
				{
					flag = true;
					length = this.m_stream.Length;
					this.m_stream.Seek(length, SeekOrigin.Begin);
				}
				TerrainSerializer129.WriteChunkHeader(this.m_stream, num, num2);
				try
				{
					fixed (byte* ptr = &this.m_buffer[0])
					{
						int* ptr2 = (int*)ptr;
						for (int i = 0; i < 16; i++)
						{
							for (int j = 0; j < 16; j++)
							{
								int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
								int k = 0;
								while (k < 128)
								{
									*ptr2 = chunk.GetCellValueFast(num3);
									k++;
									num3++;
									ptr2++;
								}
							}
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				this.m_stream.Write(this.m_buffer, 0, 131072);
				try
				{
					fixed (byte* ptr = &this.m_buffer[0])
					{
						int* ptr3 = (int*)ptr;
						for (int l = 0; l < 16; l++)
						{
							for (int m = 0; m < 16; m++)
							{
								*ptr3 = this.m_terrain.GetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y);
								ptr3++;
							}
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				this.m_stream.Write(this.m_buffer, 0, 1024);
				if (flag)
				{
					this.m_stream.Flush();
					int num4 = this.m_chunkOffsets.Count % 65536 * 3 * 4;
					this.m_stream.Seek((long)num4, SeekOrigin.Begin);
					TerrainSerializer129.WriteInt(this.m_stream, num);
					TerrainSerializer129.WriteInt(this.m_stream, num2);
					TerrainSerializer129.WriteInt(this.m_stream, this.m_chunkOffsets.Count);
					this.m_chunkOffsets[new Point2(num, num2)] = length;
				}
				this.m_stream.Flush();
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error writing data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x000BCFE8 File Offset: 0x000BB1E8
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x000BD00D File Offset: 0x000BB20D
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}

		// Token: 0x040010B2 RID: 4274
		public const int MaxChunks = 65536;

		// Token: 0x040010B3 RID: 4275
		public const int TocEntryBytesCount = 12;

		// Token: 0x040010B4 RID: 4276
		public const int TocBytesCount = 786444;

		// Token: 0x040010B5 RID: 4277
		public const int ChunkSizeX = 16;

		// Token: 0x040010B6 RID: 4278
		public const int ChunkSizeY = 128;

		// Token: 0x040010B7 RID: 4279
		public const int ChunkSizeZ = 16;

		// Token: 0x040010B8 RID: 4280
		public const int ChunkBitsX = 4;

		// Token: 0x040010B9 RID: 4281
		public const int ChunkBitsZ = 4;

		// Token: 0x040010BA RID: 4282
		public const int ChunkBytesCount = 132112;

		// Token: 0x040010BB RID: 4283
		public const string ChunksFileName = "Chunks32.dat";

		// Token: 0x040010BC RID: 4284
		public Terrain m_terrain;

		// Token: 0x040010BD RID: 4285
		public byte[] m_buffer = new byte[131072];

		// Token: 0x040010BE RID: 4286
		public Dictionary<Point2, long> m_chunkOffsets = new Dictionary<Point2, long>();

		// Token: 0x040010BF RID: 4287
		public Stream m_stream;
	}
}
