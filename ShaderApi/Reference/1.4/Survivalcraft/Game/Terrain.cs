using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Engine;

namespace Game
{
	// Token: 0x02000319 RID: 793
	public class Terrain : IDisposable
	{
		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600172C RID: 5932 RVA: 0x000AE616 File Offset: 0x000AC816
		public TerrainChunk[] AllocatedChunks
		{
			get
			{
				if (this.m_allocatedChunksArray == null)
				{
					this.m_allocatedChunksArray = this.m_allocatedChunks.ToArray<TerrainChunk>();
				}
				return this.m_allocatedChunksArray;
			}
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x000AE637 File Offset: 0x000AC837
		public Terrain()
		{
			this.m_allChunks = new Terrain.ChunksStorage();
			this.m_allocatedChunks = new HashSet<TerrainChunk>();
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x000AE658 File Offset: 0x000AC858
		public void Dispose()
		{
			foreach (TerrainChunk terrainChunk in this.m_allocatedChunks)
			{
				terrainChunk.Dispose();
			}
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x000AE6A8 File Offset: 0x000AC8A8
		public TerrainChunk GetNextChunk(int chunkX, int chunkZ)
		{
			TerrainChunk terrainChunk = this.GetChunkAtCoords(chunkX, chunkZ);
			if (terrainChunk != null)
			{
				return terrainChunk;
			}
			TerrainChunk[] allocatedChunks = this.AllocatedChunks;
			for (int i = 0; i < allocatedChunks.Length; i++)
			{
				if (Terrain.ComparePoints(allocatedChunks[i].Coords, new Point2(chunkX, chunkZ)) >= 0 && (terrainChunk == null || Terrain.ComparePoints(allocatedChunks[i].Coords, terrainChunk.Coords) < 0))
				{
					terrainChunk = allocatedChunks[i];
				}
			}
			if (terrainChunk == null)
			{
				for (int j = 0; j < allocatedChunks.Length; j++)
				{
					if (terrainChunk == null || Terrain.ComparePoints(allocatedChunks[j].Coords, terrainChunk.Coords) < 0)
					{
						terrainChunk = allocatedChunks[j];
					}
				}
			}
			return terrainChunk;
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x000AE73B File Offset: 0x000AC93B
		public TerrainChunk GetChunkAtCoords(int chunkX, int chunkZ)
		{
			return this.m_allChunks.Get(chunkX, chunkZ);
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x000AE74A File Offset: 0x000AC94A
		public TerrainChunk GetChunkAtCell(int x, int z)
		{
			return this.GetChunkAtCoords(x >> 4, z >> 4);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x000AE758 File Offset: 0x000AC958
		public TerrainChunk AllocateChunk(int chunkX, int chunkZ)
		{
			if (this.GetChunkAtCoords(chunkX, chunkZ) != null)
			{
				throw new InvalidOperationException("Chunk already allocated.");
			}
			TerrainChunk terrainChunk = new TerrainChunk(this, chunkX, chunkZ);
			this.m_allocatedChunks.Add(terrainChunk);
			this.m_allChunks.Add(chunkX, chunkZ, terrainChunk);
			this.m_allocatedChunksArray = null;
			return terrainChunk;
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000AE7A8 File Offset: 0x000AC9A8
		public void FreeChunk(TerrainChunk chunk)
		{
			if (!this.m_allocatedChunks.Remove(chunk))
			{
				throw new InvalidOperationException("Chunk not allocated.");
			}
			this.m_allChunks.Remove(chunk.Coords.X, chunk.Coords.Y);
			this.m_allocatedChunksArray = null;
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x000AE7F6 File Offset: 0x000AC9F6
		public static int ComparePoints(Point2 c1, Point2 c2)
		{
			if (c1.Y == c2.Y)
			{
				return c1.X - c2.X;
			}
			return c1.Y - c2.Y;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x000AE821 File Offset: 0x000ACA21
		public static Point2 ToChunk(Vector2 p)
		{
			return Terrain.ToChunk(Terrain.ToCell(p.X), Terrain.ToCell(p.Y));
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000AE83E File Offset: 0x000ACA3E
		public static Point2 ToChunk(int x, int z)
		{
			return new Point2(x >> 4, z >> 4);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x000AE84B File Offset: 0x000ACA4B
		public static int ToCell(float x)
		{
			return (int)MathUtils.Floor(x);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x000AE854 File Offset: 0x000ACA54
		public static Point2 ToCell(float x, float y)
		{
			return new Point2((int)MathUtils.Floor(x), (int)MathUtils.Floor(y));
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x000AE869 File Offset: 0x000ACA69
		public static Point2 ToCell(Vector2 p)
		{
			return new Point2((int)MathUtils.Floor(p.X), (int)MathUtils.Floor(p.Y));
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000AE888 File Offset: 0x000ACA88
		public static Point3 ToCell(float x, float y, float z)
		{
			return new Point3((int)MathUtils.Floor(x), (int)MathUtils.Floor(y), (int)MathUtils.Floor(z));
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x000AE8A4 File Offset: 0x000ACAA4
		public static Point3 ToCell(Vector3 p)
		{
			return new Point3((int)MathUtils.Floor(p.X), (int)MathUtils.Floor(p.Y), (int)MathUtils.Floor(p.Z));
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x000AE8CF File Offset: 0x000ACACF
		public bool IsCellValid(int x, int y, int z)
		{
			return y >= 0 && y < 256;
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x000AE8DF File Offset: 0x000ACADF
		public int GetCellValue(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellValueFast(x, y, z);
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x000AE8F7 File Offset: 0x000ACAF7
		public int GetCellContents(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellContentsFast(x, y, z);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x000AE90F File Offset: 0x000ACB0F
		public int GetCellLight(int x, int y, int z)
		{
			if (!this.IsCellValid(x, y, z))
			{
				return 0;
			}
			return this.GetCellLightFast(x, y, z);
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x000AE927 File Offset: 0x000ACB27
		public int GetCellValueFast(int x, int y, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.GetCellValueFast(x & 15, y, z & 15);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x000AE945 File Offset: 0x000ACB45
		public int GetCellValueFastChunkExists(int x, int y, int z)
		{
			return this.GetChunkAtCell(x, z).GetCellValueFast(x & 15, y, z & 15);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000AE95D File Offset: 0x000ACB5D
		public int GetCellContentsFast(int x, int y, int z)
		{
			return Terrain.ExtractContents(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x000AE96D File Offset: 0x000ACB6D
		public int GetCellLightFast(int x, int y, int z)
		{
			return Terrain.ExtractLight(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x000AE97D File Offset: 0x000ACB7D
		public void SetCellValueFast(int x, int y, int z, int value)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return;
			}
			chunkAtCell.SetCellValueFast(x & 15, y, z & 15, value);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x000AE99C File Offset: 0x000ACB9C
		public int CalculateTopmostCellHeight(int x, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.CalculateTopmostCellHeight(x & 15, z & 15);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x000AE9B9 File Offset: 0x000ACBB9
		public int GetShaftValue(int x, int z)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return 0;
			}
			return chunkAtCell.GetShaftValueFast(x & 15, z & 15);
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x000AE9D6 File Offset: 0x000ACBD6
		public void SetShaftValue(int x, int z, int value)
		{
			TerrainChunk chunkAtCell = this.GetChunkAtCell(x, z);
			if (chunkAtCell == null)
			{
				return;
			}
			chunkAtCell.SetShaftValueFast(x & 15, z & 15, value);
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x000AE9F3 File Offset: 0x000ACBF3
		public int GetTemperature(int x, int z)
		{
			return Terrain.ExtractTemperature(this.GetShaftValue(x, z));
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x000AEA02 File Offset: 0x000ACC02
		public void SetTemperature(int x, int z, int temperature)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceTemperature(this.GetShaftValue(x, z), temperature));
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x000AEA1A File Offset: 0x000ACC1A
		public int GetHumidity(int x, int z)
		{
			return Terrain.ExtractHumidity(this.GetShaftValue(x, z));
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x000AEA29 File Offset: 0x000ACC29
		public void SetHumidity(int x, int z, int humidity)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceHumidity(this.GetShaftValue(x, z), humidity));
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x000AEA41 File Offset: 0x000ACC41
		public int GetTopHeight(int x, int z)
		{
			return Terrain.ExtractTopHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x000AEA50 File Offset: 0x000ACC50
		public void SetTopHeight(int x, int z, int topHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceTopHeight(this.GetShaftValue(x, z), topHeight));
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x000AEA68 File Offset: 0x000ACC68
		public int GetBottomHeight(int x, int z)
		{
			return Terrain.ExtractBottomHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x000AEA77 File Offset: 0x000ACC77
		public void SetBottomHeight(int x, int z, int bottomHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceBottomHeight(this.GetShaftValue(x, z), bottomHeight));
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x000AEA8F File Offset: 0x000ACC8F
		public int GetSunlightHeight(int x, int z)
		{
			return Terrain.ExtractSunlightHeight(this.GetShaftValue(x, z));
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x000AEA9E File Offset: 0x000ACC9E
		public void SetSunlightHeight(int x, int z, int sunlightHeight)
		{
			this.SetShaftValue(x, z, Terrain.ReplaceSunlightHeight(this.GetShaftValue(x, z), sunlightHeight));
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x000AEAB6 File Offset: 0x000ACCB6
		public static int MakeBlockValue(int contents)
		{
			return contents & 1023;
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x000AEABF File Offset: 0x000ACCBF
		public static int MakeBlockValue(int contents, int light, int data)
		{
			return (contents & 1023) | (light << 10 & 15360) | (data << 14 & -16384);
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x000AEADE File Offset: 0x000ACCDE
		public static int ExtractContents(int value)
		{
			return value & 1023;
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x000AEAE7 File Offset: 0x000ACCE7
		public static int ExtractLight(int value)
		{
			return (value & 15360) >> 10;
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x000AEAF3 File Offset: 0x000ACCF3
		public static int ExtractData(int value)
		{
			return (value & -16384) >> 14;
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000AEAFF File Offset: 0x000ACCFF
		public static int ExtractTopHeight(int value)
		{
			return value & 255;
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x000AEB08 File Offset: 0x000ACD08
		public static int ExtractBottomHeight(int value)
		{
			return (value & 16711680) >> 16;
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x000AEB14 File Offset: 0x000ACD14
		public static int ExtractSunlightHeight(int value)
		{
			return (value & -16777216) >> 24;
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x000AEB20 File Offset: 0x000ACD20
		public static int ExtractHumidity(int value)
		{
			return (value & 61440) >> 12;
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x000AEB2C File Offset: 0x000ACD2C
		public static int ExtractTemperature(int value)
		{
			return (value & 3840) >> 8;
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x000AEB37 File Offset: 0x000ACD37
		public static int ReplaceContents(int value, int contents)
		{
			return value ^ ((value ^ contents) & 1023);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x000AEB44 File Offset: 0x000ACD44
		public static int ReplaceLight(int value, int light)
		{
			return value ^ ((value ^ light << 10) & 15360);
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x000AEB54 File Offset: 0x000ACD54
		public static int ReplaceData(int value, int data)
		{
			return value ^ ((value ^ data << 14) & -16384);
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x000AEB64 File Offset: 0x000ACD64
		public static int ReplaceTopHeight(int value, int topHeight)
		{
			return value ^ ((value ^ topHeight) & 255);
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x000AEB71 File Offset: 0x000ACD71
		public static int ReplaceBottomHeight(int value, int bottomHeight)
		{
			return value ^ ((value ^ bottomHeight << 16) & 16711680);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x000AEB81 File Offset: 0x000ACD81
		public static int ReplaceSunlightHeight(int value, int sunlightHeight)
		{
			return value ^ ((value ^ sunlightHeight << 24) & -16777216);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x000AEB91 File Offset: 0x000ACD91
		public static int ReplaceHumidity(int value, int humidity)
		{
			return value ^ ((value ^ humidity << 12) & 61440);
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x000AEBA1 File Offset: 0x000ACDA1
		public static int ReplaceTemperature(int value, int temperature)
		{
			return value ^ ((value ^ temperature << 8) & 3840);
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x000AEBB0 File Offset: 0x000ACDB0
		public int GetSeasonalTemperature(int x, int z)
		{
			return this.GetTemperature(x, z) + this.SeasonTemperature;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x000AEBC1 File Offset: 0x000ACDC1
		public int GetSeasonalTemperature(int shaftValue)
		{
			return Terrain.ExtractTemperature(shaftValue) + this.SeasonTemperature;
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x000AEBD0 File Offset: 0x000ACDD0
		public int GetSeasonalHumidity(int x, int z)
		{
			return this.GetHumidity(x, z) + this.SeasonHumidity;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x000AEBE1 File Offset: 0x000ACDE1
		public int GetSeasonalHumidity(int shaftValue)
		{
			return Terrain.ExtractHumidity(shaftValue) + this.SeasonHumidity;
		}

		// Token: 0x04000FDD RID: 4061
		public const int ContentsMask = 1023;

		// Token: 0x04000FDE RID: 4062
		public const int LightMask = 15360;

		// Token: 0x04000FDF RID: 4063
		public const int LightShift = 10;

		// Token: 0x04000FE0 RID: 4064
		public const int DataMask = -16384;

		// Token: 0x04000FE1 RID: 4065
		public const int DataShift = 14;

		// Token: 0x04000FE2 RID: 4066
		public const int TopHeightMask = 255;

		// Token: 0x04000FE3 RID: 4067
		public const int TopHeightShift = 0;

		// Token: 0x04000FE4 RID: 4068
		public const int TemperatureMask = 3840;

		// Token: 0x04000FE5 RID: 4069
		public const int TemperatureShift = 8;

		// Token: 0x04000FE6 RID: 4070
		public const int HumidityMask = 61440;

		// Token: 0x04000FE7 RID: 4071
		public const int HumidityShift = 12;

		// Token: 0x04000FE8 RID: 4072
		public const int BottomHeightMask = 16711680;

		// Token: 0x04000FE9 RID: 4073
		public const int BottomHeightShift = 16;

		// Token: 0x04000FEA RID: 4074
		public const int SunlightHeightMask = -16777216;

		// Token: 0x04000FEB RID: 4075
		public const int SunlightHeightShift = 24;

		// Token: 0x04000FEC RID: 4076
		public Terrain.ChunksStorage m_allChunks;

		// Token: 0x04000FED RID: 4077
		public HashSet<TerrainChunk> m_allocatedChunks;

		// Token: 0x04000FEE RID: 4078
		public TerrainChunk[] m_allocatedChunksArray;

		// Token: 0x04000FEF RID: 4079
		public int SeasonTemperature;

		// Token: 0x04000FF0 RID: 4080
		public int SeasonHumidity;

		// Token: 0x02000541 RID: 1345
		public class ChunksStorage
		{
			// Token: 0x0600223F RID: 8767 RVA: 0x000EC130 File Offset: 0x000EA330
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TerrainChunk Get(int x, int y)
			{
				int num = x + (y << 8) & 65535;
				for (;;)
				{
					TerrainChunk terrainChunk = this.m_array[num];
					if (terrainChunk == null)
					{
						break;
					}
					if (terrainChunk.Coords.X == x && terrainChunk.Coords.Y == y)
					{
						return terrainChunk;
					}
					num = (num + 1 & 65535);
				}
				return null;
			}

			// Token: 0x06002240 RID: 8768 RVA: 0x000EC180 File Offset: 0x000EA380
			public void Add(int x, int y, TerrainChunk chunk)
			{
				int num = x + (y << 8) & 65535;
				while (this.m_array[num] != null)
				{
					num = (num + 1 & 65535);
				}
				this.m_array[num] = chunk;
			}

			// Token: 0x06002241 RID: 8769 RVA: 0x000EC1B8 File Offset: 0x000EA3B8
			public void Remove(int x, int y)
			{
				int num = x + (y << 8) & 65535;
				for (;;)
				{
					TerrainChunk terrainChunk = this.m_array[num];
					if (terrainChunk == null)
					{
						break;
					}
					if (terrainChunk.Coords.X == x && terrainChunk.Coords.Y == y)
					{
						goto IL_41;
					}
					num = (num + 1 & 65535);
				}
				return;
				IL_41:
				this.m_array[num] = null;
			}

			// Token: 0x040018F0 RID: 6384
			public const int Shift = 8;

			// Token: 0x040018F1 RID: 6385
			public const int Capacity = 65536;

			// Token: 0x040018F2 RID: 6386
			public const int CapacityMinusOne = 65535;

			// Token: 0x040018F3 RID: 6387
			public TerrainChunk[] m_array = new TerrainChunk[65536];
		}
	}
}
