using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000136 RID: 310
	public class TerrainSerializer22 : IDisposable
	{
		// Token: 0x060005CF RID: 1487 RVA: 0x00021074 File Offset: 0x0001F274
		public TerrainSerializer22(Terrain terrain, string directoryName)
		{
			this.m_terrain = terrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32h.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer22.WriteTOCEntry(stream, 0, 0, -1);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer22.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num < 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = 786444L + 263184L * (long)num;
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00021150 File Offset: 0x0001F350
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00021159 File Offset: 0x0001F359
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x0002117B File Offset: 0x0001F37B
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00021188 File Offset: 0x0001F388
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer22.ReadInt(stream);
			int num2 = TerrainSerializer22.ReadInt(stream);
			TerrainSerializer22.ReadInt(stream);
			TerrainSerializer22.ReadInt(stream);
			if (num != -559038737 || num2 != -2)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x000211C7 File Offset: 0x0001F3C7
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer22.WriteInt(stream, -559038737);
			TerrainSerializer22.WriteInt(stream, -2);
			TerrainSerializer22.WriteInt(stream, cx);
			TerrainSerializer22.WriteInt(stream, cz);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x000211EA File Offset: 0x0001F3EA
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int index)
		{
			cx = TerrainSerializer22.ReadInt(stream);
			cz = TerrainSerializer22.ReadInt(stream);
			index = TerrainSerializer22.ReadInt(stream);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00021204 File Offset: 0x0001F404
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int index)
		{
			TerrainSerializer22.WriteInt(stream, cx);
			TerrainSerializer22.WriteInt(stream, cz);
			TerrainSerializer22.WriteInt(stream, index);
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0002121C File Offset: 0x0001F41C
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
					TerrainSerializer22.ReadChunkHeader(this.m_stream);
					this.m_stream.Read(this.m_buffer, 0, 262144);
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
									while (k < 256)
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

		// Token: 0x060005D8 RID: 1496 RVA: 0x00021418 File Offset: 0x0001F618
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
				TerrainSerializer22.WriteChunkHeader(this.m_stream, num, num2);
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
								while (k < 256)
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
				this.m_stream.Write(this.m_buffer, 0, 262144);
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
					TerrainSerializer22.WriteInt(this.m_stream, num);
					TerrainSerializer22.WriteInt(this.m_stream, num2);
					TerrainSerializer22.WriteInt(this.m_stream, this.m_chunkOffsets.Count);
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

		// Token: 0x060005D9 RID: 1497 RVA: 0x000216A0 File Offset: 0x0001F8A0
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x000216C5 File Offset: 0x0001F8C5
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}

		// Token: 0x04000294 RID: 660
		public const int MaxChunks = 65536;

		// Token: 0x04000295 RID: 661
		public const int TocEntryBytesCount = 12;

		// Token: 0x04000296 RID: 662
		public const int TocBytesCount = 786444;

		// Token: 0x04000297 RID: 663
		public const int ChunkSizeX = 16;

		// Token: 0x04000298 RID: 664
		public const int ChunkSizeY = 256;

		// Token: 0x04000299 RID: 665
		public const int ChunkSizeZ = 16;

		// Token: 0x0400029A RID: 666
		public const int ChunkBitsX = 4;

		// Token: 0x0400029B RID: 667
		public const int ChunkBitsZ = 4;

		// Token: 0x0400029C RID: 668
		public const int ChunkBytesCount = 263184;

		// Token: 0x0400029D RID: 669
		public const string ChunksFileName = "Chunks32h.dat";

		// Token: 0x0400029E RID: 670
		public Terrain m_terrain;

		// Token: 0x0400029F RID: 671
		public byte[] m_buffer = new byte[262144];

		// Token: 0x040002A0 RID: 672
		public Dictionary<Point2, long> m_chunkOffsets = new Dictionary<Point2, long>();

		// Token: 0x040002A1 RID: 673
		public Stream m_stream;
	}
}
