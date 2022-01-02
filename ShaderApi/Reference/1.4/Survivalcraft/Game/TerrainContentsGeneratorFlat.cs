using System;
using Engine;

namespace Game
{
	// Token: 0x02000320 RID: 800
	public class TerrainContentsGeneratorFlat : ITerrainContentsGenerator
	{
		// Token: 0x17000386 RID: 902
		// (get) Token: 0x060017E0 RID: 6112 RVA: 0x000BAD8E File Offset: 0x000B8F8E
		public int OceanLevel
		{
			get
			{
				return this.m_worldSettings.TerrainLevel + this.m_worldSettings.SeaLevelOffset;
			}
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x000BADA8 File Offset: 0x000B8FA8
		public TerrainContentsGeneratorFlat(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			SubsystemGameInfo subsystemGameInfo = subsystemTerrain.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_worldSettings = subsystemGameInfo.WorldSettings;
			this.m_oceanCorner = ((string.CompareOrdinal(subsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") < 0) ? (this.m_oceanCorner = new Vector2(2001f, 2001f)) : (this.m_oceanCorner = new Vector2(-199f, -199f)));
			this.m_islandSize = ((this.m_worldSettings.TerrainGenerationMode == TerrainGenerationMode.FlatIsland) ? new Vector2?(this.m_worldSettings.IslandSize) : null);
			this.m_shoreRoughnessAmplitude.X = MathUtils.Pow(this.m_worldSettings.ShoreRoughness, 2f) * ((this.m_islandSize != null) ? MathUtils.Min(4f * this.m_islandSize.Value.X, 400f) : 400f);
			this.m_shoreRoughnessAmplitude.Y = MathUtils.Pow(this.m_worldSettings.ShoreRoughness, 2f) * ((this.m_islandSize != null) ? MathUtils.Min(4f * this.m_islandSize.Value.Y, 400f) : 400f);
			this.m_shoreRoughnessFrequency = MathUtils.Lerp(0.5f, 1f, this.m_worldSettings.ShoreRoughness) * new Vector2(1f) / this.m_shoreRoughnessAmplitude;
			this.m_shoreRoughnessOctaves.X = (float)((int)MathUtils.Clamp(MathUtils.Log(1f / this.m_shoreRoughnessFrequency.X) / MathUtils.Log(2f) - 1f, 1f, 7f));
			this.m_shoreRoughnessOctaves.Y = (float)((int)MathUtils.Clamp(MathUtils.Log(1f / this.m_shoreRoughnessFrequency.Y) / MathUtils.Log(2f) - 1f, 1f, 7f));
			Game.Random random = new Game.Random(subsystemGameInfo.WorldSeed);
			this.m_shoreRoughnessOffset[0] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[1] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[2] = random.Float(-2000f, 2000f);
			this.m_shoreRoughnessOffset[3] = random.Float(-2000f, 2000f);
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x000BB03C File Offset: 0x000B923C
		public Vector3 FindCoarseSpawnPosition()
		{
			for (int i = -400; i <= 400; i += 10)
			{
				for (int j = -400; j <= 400; j += 10)
				{
					Vector2 vector = this.m_oceanCorner + new Vector2((float)i, (float)j);
					float num = this.CalculateOceanShoreDistance(vector.X, vector.Y);
					if (num >= 1f && num <= 20f)
					{
						return new Vector3(vector.X, this.CalculateHeight(vector.X, vector.Y), vector.Y);
					}
				}
			}
			return new Vector3(this.m_oceanCorner.X, this.CalculateHeight(this.m_oceanCorner.X, this.m_oceanCorner.Y), this.m_oceanCorner.Y);
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x000BB108 File Offset: 0x000B9308
		public void GenerateChunkContentsPass1(TerrainChunk chunk)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					chunk.SetTemperatureFast(i, j, this.CalculateTemperature((float)num, (float)num2));
					chunk.SetHumidityFast(i, j, this.CalculateHumidity((float)num, (float)num2));
					bool flag = this.CalculateOceanShoreDistance((float)num, (float)num2) >= 0f;
					int num3 = TerrainChunk.CalculateCellIndex(i, 0, j);
					for (int k = 0; k < 256; k++)
					{
						int value = Terrain.MakeBlockValue(0);
						if (flag)
						{
							if (k < 2)
							{
								value = Terrain.MakeBlockValue(1);
							}
							else if (k < this.m_worldSettings.TerrainLevel)
							{
								value = Terrain.MakeBlockValue((this.m_worldSettings.TerrainBlockIndex == 8) ? 2 : this.m_worldSettings.TerrainBlockIndex);
							}
							else if (k == this.m_worldSettings.TerrainLevel)
							{
								value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainBlockIndex);
							}
							else if (k <= this.OceanLevel)
							{
								value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainOceanBlockIndex);
							}
						}
						else if (k < 2)
						{
							value = Terrain.MakeBlockValue(1);
						}
						else if (k <= this.OceanLevel)
						{
							value = Terrain.MakeBlockValue(this.m_worldSettings.TerrainOceanBlockIndex);
						}
						chunk.SetCellValueFast(num3 + k, value);
					}
				}
			}
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000BB283 File Offset: 0x000B9483
		public void GenerateChunkContentsPass2(TerrainChunk chunk)
		{
			this.UpdateFluidIsTop(chunk);
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000BB28C File Offset: 0x000B948C
		public void GenerateChunkContentsPass3(TerrainChunk chunk)
		{
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000BB28E File Offset: 0x000B948E
		public void GenerateChunkContentsPass4(TerrainChunk chunk)
		{
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000BB290 File Offset: 0x000B9490
		public float CalculateOceanShoreDistance(float x, float z)
		{
			float x2 = 0f;
			float x3 = 0f;
			float y = 0f;
			float y2 = 0f;
			if (this.m_shoreRoughnessAmplitude.X > 0f)
			{
				x2 = this.m_shoreRoughnessAmplitude.X * SimplexNoise.OctavedNoise(z + this.m_shoreRoughnessOffset[0], this.m_shoreRoughnessFrequency.X, (int)this.m_shoreRoughnessOctaves.X, 2f, 0.6f, false);
				x3 = this.m_shoreRoughnessAmplitude.X * SimplexNoise.OctavedNoise(z + this.m_shoreRoughnessOffset[1], this.m_shoreRoughnessFrequency.X, (int)this.m_shoreRoughnessOctaves.X, 2f, 0.6f, false);
			}
			if (this.m_shoreRoughnessAmplitude.Y > 0f)
			{
				y = this.m_shoreRoughnessAmplitude.Y * SimplexNoise.OctavedNoise(x + this.m_shoreRoughnessOffset[2], this.m_shoreRoughnessFrequency.Y, (int)this.m_shoreRoughnessOctaves.Y, 2f, 0.6f, false);
				y2 = this.m_shoreRoughnessAmplitude.Y * SimplexNoise.OctavedNoise(x + this.m_shoreRoughnessOffset[3], this.m_shoreRoughnessFrequency.Y, (int)this.m_shoreRoughnessOctaves.Y, 2f, 0.6f, false);
			}
			Vector2 vector = this.m_oceanCorner + new Vector2(x2, y);
			Vector2 vector2 = this.m_oceanCorner + ((this.m_islandSize != null) ? this.m_islandSize.Value : new Vector2(float.MaxValue)) + new Vector2(x3, y2);
			return MathUtils.Min(x - vector.X, vector2.X - x, z - vector.Y, vector2.Y - z);
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x000BB44B File Offset: 0x000B964B
		public float CalculateHeight(float x, float z)
		{
			return (float)this.m_worldSettings.TerrainLevel;
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x000BB459 File Offset: 0x000B9659
		public int CalculateTemperature(float x, float z)
		{
			return MathUtils.Clamp(12 + (int)this.m_worldSettings.TemperatureOffset, 0, 15);
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x000BB472 File Offset: 0x000B9672
		public int CalculateHumidity(float x, float z)
		{
			return MathUtils.Clamp(12 + (int)this.m_worldSettings.HumidityOffset, 0, 15);
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x000BB48B File Offset: 0x000B968B
		public float CalculateMountainRangeFactor(float x, float z)
		{
			return 0f;
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x000BB494 File Offset: 0x000B9694
		public void UpdateFluidIsTop(TerrainChunk chunk)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					int num = TerrainChunk.CalculateCellIndex(i, 255, j);
					int num2 = 0;
					int k = 255;
					while (k >= 0)
					{
						int cellValueFast = chunk.GetCellValueFast(num);
						int num3 = Terrain.ExtractContents(cellValueFast);
						if (num3 != 0 && num3 != num2 && BlocksManager.Blocks[num3] is FluidBlock)
						{
							int data = Terrain.ExtractData(cellValueFast);
							chunk.SetCellValueFast(num, Terrain.MakeBlockValue(num3, 0, FluidBlock.SetIsTop(data, true)));
						}
						num2 = num3;
						k--;
						num--;
					}
				}
			}
		}

		// Token: 0x04001088 RID: 4232
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001089 RID: 4233
		public WorldSettings m_worldSettings;

		// Token: 0x0400108A RID: 4234
		public Vector2 m_oceanCorner;

		// Token: 0x0400108B RID: 4235
		public Vector2? m_islandSize;

		// Token: 0x0400108C RID: 4236
		public Vector2 m_shoreRoughnessFrequency;

		// Token: 0x0400108D RID: 4237
		public Vector2 m_shoreRoughnessAmplitude;

		// Token: 0x0400108E RID: 4238
		public Vector2 m_shoreRoughnessOctaves;

		// Token: 0x0400108F RID: 4239
		public float[] m_shoreRoughnessOffset = new float[4];
	}
}
