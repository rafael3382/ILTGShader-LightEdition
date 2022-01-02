using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000327 RID: 807
	public class TerrainSerializer14 : IDisposable
	{
		// Token: 0x0600180E RID: 6158 RVA: 0x000BD038 File Offset: 0x000BB238
		public TerrainSerializer14(SubsystemTerrain subsystemTerrain, string directoryName)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks.dat"
			});
			if (!Storage.FileExists(path))
			{
				using (Stream stream = Storage.OpenFile(path, OpenFileMode.Create))
				{
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer14.WriteTOCEntry(stream, 0, 0, 0);
					}
				}
			}
			this.m_stream = Storage.OpenFile(path, OpenFileMode.ReadWrite);
			for (;;)
			{
				int x;
				int y;
				int num;
				TerrainSerializer14.ReadTOCEntry(this.m_stream, out x, out y, out num);
				if (num == 0)
				{
					break;
				}
				this.m_chunkOffsets[new Point2(x, y)] = num;
			}
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x000BD104 File Offset: 0x000BB304
		public bool LoadChunk(TerrainChunk chunk)
		{
			return this.LoadChunkBlocks(chunk);
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x000BD10D File Offset: 0x000BB30D
		public void SaveChunk(TerrainChunk chunk)
		{
			if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)
			{
				this.SaveChunkBlocks(chunk);
				chunk.ModificationCounter = 0;
			}
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x000BD12F File Offset: 0x000BB32F
		public void Dispose()
		{
			Utilities.Dispose<Stream>(ref this.m_stream);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x000BD13C File Offset: 0x000BB33C
		public static void ReadChunkHeader(Stream stream)
		{
			int num = TerrainSerializer14.ReadInt(stream);
			int num2 = TerrainSerializer14.ReadInt(stream);
			TerrainSerializer14.ReadInt(stream);
			TerrainSerializer14.ReadInt(stream);
			if (num != -559038737 || num2 != -1)
			{
				throw new InvalidOperationException("Invalid chunk header.");
			}
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x000BD17A File Offset: 0x000BB37A
		public static void WriteChunkHeader(Stream stream, int cx, int cz)
		{
			TerrainSerializer14.WriteInt(stream, -559038737);
			TerrainSerializer14.WriteInt(stream, -1);
			TerrainSerializer14.WriteInt(stream, cx);
			TerrainSerializer14.WriteInt(stream, cz);
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x000BD19C File Offset: 0x000BB39C
		public static void ReadTOCEntry(Stream stream, out int cx, out int cz, out int offset)
		{
			cx = TerrainSerializer14.ReadInt(stream);
			cz = TerrainSerializer14.ReadInt(stream);
			offset = TerrainSerializer14.ReadInt(stream);
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x000BD1B6 File Offset: 0x000BB3B6
		public static void WriteTOCEntry(Stream stream, int cx, int cz, int offset)
		{
			TerrainSerializer14.WriteInt(stream, cx);
			TerrainSerializer14.WriteInt(stream, cz);
			TerrainSerializer14.WriteInt(stream, offset);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x000BD1D0 File Offset: 0x000BB3D0
		public bool LoadChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			bool result = false;
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				int num3;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out num3))
				{
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
					TerrainSerializer14.ReadChunkHeader(this.m_stream);
					int num4 = 0;
					this.m_stream.Read(this.m_buffer, 0, 131072);
					for (int i = 0; i < 16; i++)
					{
						for (int j = 0; j < 16; j++)
						{
							int num5 = TerrainChunk.CalculateCellIndex(i, 0, j);
							for (int k = 0; k < 256; k++)
							{
								int num6 = (int)this.m_buffer[num4++];
								num6 |= (int)this.m_buffer[num4++] << 8;
								chunk.SetCellValueFast(num5++, num6);
							}
						}
					}
					num4 = 0;
					this.m_stream.Read(this.m_buffer, 0, 1024);
					for (int l = 0; l < 16; l++)
					{
						for (int m = 0; m < 16; m++)
						{
							int num7 = (int)this.m_buffer[num4++];
							num7 |= (int)this.m_buffer[num4++] << 8;
							num7 |= (int)this.m_buffer[num4++] << 16;
							num7 |= (int)this.m_buffer[num4++] << 24;
							terrain.SetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y, num7);
						}
					}
					result = true;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error loading data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
			return result;
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x000BD3DC File Offset: 0x000BB5DC
		public void SaveChunkBlocks(TerrainChunk chunk)
		{
			double realTime = Time.RealTime;
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			int num = chunk.Origin.X >> 4;
			int num2 = chunk.Origin.Y >> 4;
			try
			{
				bool flag = false;
				int num3;
				if (this.m_chunkOffsets.TryGetValue(new Point2(num, num2), out num3))
				{
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
				}
				else
				{
					flag = true;
					num3 = (int)this.m_stream.Length;
					this.m_stream.Seek((long)num3, SeekOrigin.Begin);
				}
				TerrainSerializer14.WriteChunkHeader(this.m_stream, num, num2);
				int num4 = 0;
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						int num5 = TerrainChunk.CalculateCellIndex(i, 0, j);
						for (int k = 0; k < 256; k++)
						{
							int cellValueFast = chunk.GetCellValueFast(num5++);
							this.m_buffer[num4++] = (byte)cellValueFast;
							this.m_buffer[num4++] = (byte)(cellValueFast >> 8);
						}
					}
				}
				this.m_stream.Write(this.m_buffer, 0, 131072);
				num4 = 0;
				for (int l = 0; l < 16; l++)
				{
					for (int m = 0; m < 16; m++)
					{
						int shaftValue = terrain.GetShaftValue(l + chunk.Origin.X, m + chunk.Origin.Y);
						this.m_buffer[num4++] = (byte)shaftValue;
						this.m_buffer[num4++] = (byte)(shaftValue >> 8);
						this.m_buffer[num4++] = (byte)(shaftValue >> 16);
						this.m_buffer[num4++] = (byte)(shaftValue >> 24);
					}
				}
				this.m_stream.Write(this.m_buffer, 0, 1024);
				if (flag)
				{
					this.m_stream.Flush();
					int num6 = this.m_chunkOffsets.Count % 65536 * 3 * 4;
					this.m_stream.Seek((long)num6, SeekOrigin.Begin);
					TerrainSerializer14.WriteInt(this.m_stream, num);
					TerrainSerializer14.WriteInt(this.m_stream, num2);
					TerrainSerializer14.WriteInt(this.m_stream, num3);
					this.m_chunkOffsets[new Point2(num, num2)] = num3;
				}
			}
			catch (Exception e)
			{
				Log.Error(ExceptionManager.MakeFullErrorMessage(string.Format("Error writing data for chunk ({0},{1}).", num, num2), e));
			}
			double realTime2 = Time.RealTime;
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x000BD668 File Offset: 0x000BB868
		public static int ReadInt(Stream stream)
		{
			return stream.ReadByte() + (stream.ReadByte() << 8) + (stream.ReadByte() << 16) + (stream.ReadByte() << 24);
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x000BD690 File Offset: 0x000BB890
		public static void WriteInt(Stream stream, int value)
		{
			stream.WriteByte((byte)(value & 255));
			stream.WriteByte((byte)(value >> 8 & 255));
			stream.WriteByte((byte)(value >> 16 & 255));
			stream.WriteByte((byte)(value >> 24 & 255));
		}

		// Token: 0x040010C0 RID: 4288
		public const int MaxChunks = 65536;

		// Token: 0x040010C1 RID: 4289
		public const string ChunksFileName = "Chunks.dat";

		// Token: 0x040010C2 RID: 4290
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040010C3 RID: 4291
		public byte[] m_buffer = new byte[131072];

		// Token: 0x040010C4 RID: 4292
		public Dictionary<Point2, int> m_chunkOffsets = new Dictionary<Point2, int>();

		// Token: 0x040010C5 RID: 4293
		public Stream m_stream;
	}
}
