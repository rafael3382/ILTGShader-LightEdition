using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200031B RID: 795
	public class TerrainChunk : IDisposable
	{
		// Token: 0x0600177C RID: 6012 RVA: 0x000AF638 File Offset: 0x000AD838
		public TerrainChunk(Terrain terrain, int x, int z)
		{
			this.Terrain = terrain;
			this.Coords = new Point2(x, z);
			this.Origin = new Point2(x * 16, z * 16);
			this.BoundingBox = new BoundingBox(new Vector3((float)this.Origin.X, 0f, (float)this.Origin.Y), new Vector3((float)(this.Origin.X + 16), 256f, (float)(this.Origin.Y + 16)));
			this.Center = new Vector2((float)this.Origin.X + 8f, (float)this.Origin.Y + 8f);
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000AF750 File Offset: 0x000AD950
		public void Dispose()
		{
			this.Geometry.Dispose();
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x000AF75D File Offset: 0x000AD95D
		public static bool IsCellValid(int x, int y, int z)
		{
			return x >= 0 && x < 16 && y >= 0 && y < 256 && z >= 0 && z < 16;
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x000AF77F File Offset: 0x000AD97F
		public static bool IsShaftValid(int x, int z)
		{
			return x >= 0 && x < 16 && z >= 0 && z < 16;
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x000AF795 File Offset: 0x000AD995
		public static int CalculateCellIndex(int x, int y, int z)
		{
			return y + x * 256 + z * 256 * 16;
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x000AF7AC File Offset: 0x000AD9AC
		public int CalculateTopmostCellHeight(int x, int z)
		{
			int num = TerrainChunk.CalculateCellIndex(x, 255, z);
			int i = 255;
			while (i >= 0)
			{
				if (Terrain.ExtractContents(this.GetCellValueFast(num)) != 0)
				{
					return i;
				}
				i--;
				num--;
			}
			return 0;
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x000AF7EB File Offset: 0x000AD9EB
		public int GetCellValueFast(int index)
		{
			return this.Cells[index];
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x000AF7F5 File Offset: 0x000AD9F5
		public int GetCellValueFast(int x, int y, int z)
		{
			return this.Cells[y + x * 256 + z * 256 * 16];
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000AF812 File Offset: 0x000ADA12
		public void SetCellValueFast(int x, int y, int z, int value)
		{
			this.Cells[y + x * 256 + z * 256 * 16] = value;
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000AF831 File Offset: 0x000ADA31
		public void SetCellValueFast(int index, int value)
		{
			this.Cells[index] = value;
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x000AF83C File Offset: 0x000ADA3C
		public int GetCellContentsFast(int x, int y, int z)
		{
			return Terrain.ExtractContents(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x000AF84C File Offset: 0x000ADA4C
		public int GetCellLightFast(int x, int y, int z)
		{
			return Terrain.ExtractLight(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x000AF85C File Offset: 0x000ADA5C
		public int GetShaftValueFast(int x, int z)
		{
			return this.Shafts[x + z * 16];
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x000AF86B File Offset: 0x000ADA6B
		public void SetShaftValueFast(int x, int z, int value)
		{
			this.Shafts[x + z * 16] = value;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x000AF87B File Offset: 0x000ADA7B
		public int GetTemperatureFast(int x, int z)
		{
			return Terrain.ExtractTemperature(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x000AF88A File Offset: 0x000ADA8A
		public void SetTemperatureFast(int x, int z, int temperature)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceTemperature(this.GetShaftValueFast(x, z), temperature));
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x000AF8A2 File Offset: 0x000ADAA2
		public int GetHumidityFast(int x, int z)
		{
			return Terrain.ExtractHumidity(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x000AF8B1 File Offset: 0x000ADAB1
		public void SetHumidityFast(int x, int z, int humidity)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceHumidity(this.GetShaftValueFast(x, z), humidity));
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000AF8C9 File Offset: 0x000ADAC9
		public int GetTopHeightFast(int x, int z)
		{
			return Terrain.ExtractTopHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x000AF8D8 File Offset: 0x000ADAD8
		public void SetTopHeightFast(int x, int z, int topHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceTopHeight(this.GetShaftValueFast(x, z), topHeight));
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x000AF8F0 File Offset: 0x000ADAF0
		public int GetBottomHeightFast(int x, int z)
		{
			return Terrain.ExtractBottomHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x000AF8FF File Offset: 0x000ADAFF
		public void SetBottomHeightFast(int x, int z, int bottomHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceBottomHeight(this.GetShaftValueFast(x, z), bottomHeight));
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000AF917 File Offset: 0x000ADB17
		public int GetSunlightHeightFast(int x, int z)
		{
			return Terrain.ExtractSunlightHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000AF926 File Offset: 0x000ADB26
		public void SetSunlightHeightFast(int x, int z, int sunlightHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceSunlightHeight(this.GetShaftValueFast(x, z), sunlightHeight));
		}

		// Token: 0x04000FF3 RID: 4083
		public const int SizeBits = 4;

		// Token: 0x04000FF4 RID: 4084
		public const int Size = 16;

		// Token: 0x04000FF5 RID: 4085
		public const int HeightBits = 8;

		// Token: 0x04000FF6 RID: 4086
		public const int Height = 256;

		// Token: 0x04000FF7 RID: 4087
		public const int SizeMinusOne = 15;

		// Token: 0x04000FF8 RID: 4088
		public const int HeightMinusOne = 255;

		// Token: 0x04000FF9 RID: 4089
		public const int SliceHeight = 16;

		// Token: 0x04000FFA RID: 4090
		public const int SlicesCount = 16;

		// Token: 0x04000FFB RID: 4091
		public Terrain Terrain;

		// Token: 0x04000FFC RID: 4092
		public Point2 Coords;

		// Token: 0x04000FFD RID: 4093
		public Point2 Origin;

		// Token: 0x04000FFE RID: 4094
		public BoundingBox BoundingBox;

		// Token: 0x04000FFF RID: 4095
		public Vector2 Center;

		// Token: 0x04001000 RID: 4096
		public TerrainChunkState State;

		// Token: 0x04001001 RID: 4097
		public TerrainChunkState ThreadState;

		// Token: 0x04001002 RID: 4098
		public bool WasDowngraded;

		// Token: 0x04001003 RID: 4099
		public TerrainChunkState? DowngradedState;

		// Token: 0x04001004 RID: 4100
		public bool WasUpgraded;

		// Token: 0x04001005 RID: 4101
		public TerrainChunkState? UpgradedState;

		// Token: 0x04001006 RID: 4102
		public float DrawDistanceSquared;

		// Token: 0x04001007 RID: 4103
		public int LightPropagationMask;

		// Token: 0x04001008 RID: 4104
		public int ModificationCounter;

		// Token: 0x04001009 RID: 4105
		public float[] FogEnds = new float[4];

		// Token: 0x0400100A RID: 4106
		public int[] SliceContentsHashes = new int[16];

		// Token: 0x0400100B RID: 4107
		public bool AreBehaviorsNotified;

		// Token: 0x0400100C RID: 4108
		public object lockobj = new object();

		// Token: 0x0400100D RID: 4109
		public bool IsLoaded;

		// Token: 0x0400100E RID: 4110
		public volatile bool NewGeometryData;

		// Token: 0x0400100F RID: 4111
		public TerrainChunkGeometry Geometry = new TerrainChunkGeometry();

		// Token: 0x04001010 RID: 4112
		public int[] Cells = new int[65536];

		// Token: 0x04001011 RID: 4113
		public int[] Shafts = new int[256];

		// Token: 0x04001012 RID: 4114
		public Dictionary<Texture2D, TerrainGeometrySubset[]> Draws = new Dictionary<Texture2D, TerrainGeometrySubset[]>();
	}
}
