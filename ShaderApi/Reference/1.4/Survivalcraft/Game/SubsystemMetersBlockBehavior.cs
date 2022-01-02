using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BA RID: 442
	public class SubsystemMetersBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000B58 RID: 2904 RVA: 0x0004C087 File Offset: 0x0004A287
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					120,
					121
				};
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000B59 RID: 2905 RVA: 0x0004C099 File Offset: 0x0004A299
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0004C09C File Offset: 0x0004A29C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			Point3 point = CellFace.FaceToPoint3(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0004C116 File Offset: 0x0004A316
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0004C124 File Offset: 0x0004A324
		public override void OnBlockRemoved(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveMeter(oldValue, x, y, z);
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0004C132 File Offset: 0x0004A332
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveMeter(oldValue, x, y, z);
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0004C14C File Offset: 0x0004A34C
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0004C15C File Offset: 0x0004A35C
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_thermometersByPoint.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 key in list)
			{
				this.m_thermometersByPoint.Remove(key);
			}
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0004C254 File Offset: 0x0004A454
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0004C294 File Offset: 0x0004A494
		public void Update(float dt)
		{
			if (this.m_thermometersToSimulateIndex < this.m_thermometersToSimulate.Count)
			{
				double period = MathUtils.Max(5.0 / (double)this.m_thermometersToSimulate.Count, 1.0);
				if (this.m_subsystemTime.PeriodicGameTimeEvent(period, 0.0))
				{
					Point3 point = this.m_thermometersToSimulate.Array[this.m_thermometersToSimulateIndex];
					this.SimulateThermometer(point.X, point.Y, point.Z, true);
					this.m_thermometersToSimulateIndex++;
					return;
				}
			}
			else if (this.m_thermometersByPoint.Count > 0)
			{
				this.m_thermometersToSimulateIndex = 0;
				this.m_thermometersToSimulate.Clear();
				this.m_thermometersToSimulate.AddRange(this.m_thermometersByPoint.Keys);
			}
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0004C368 File Offset: 0x0004A568
		public int GetThermometerReading(int x, int y, int z)
		{
			int result;
			this.m_thermometersByPoint.TryGetValue(new Point3(x, y, z), out result);
			return result;
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0004C38C File Offset: 0x0004A58C
		public void CalculateTemperature(int x, int y, int z, float meterTemperature, float meterInsulation, out float temperature, out float temperatureFlux)
		{
			this.m_toVisit.Clear();
			for (int i = 0; i < this.m_visited.Length; i++)
			{
				this.m_visited[i] = 0;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			this.m_toVisit.Add(133152);
			for (int j = 0; j < this.m_toVisit.Count; j++)
			{
				int num7 = this.m_toVisit.Array[j];
				if ((this.m_visited[num7 / 32] & 1 << num7) == 0)
				{
					this.m_visited[num7 / 32] |= 1 << num7;
					int num8 = (num7 & 63) - 32;
					int num9 = (num7 >> 6 & 63) - 32;
					int num10 = (num7 >> 12 & 63) - 32;
					int num11 = num8 + x;
					int num12 = num9 + y;
					int num13 = num10 + z;
					Terrain terrain = base.SubsystemTerrain.Terrain;
					TerrainChunk chunkAtCell = terrain.GetChunkAtCell(num11, num13);
					if (chunkAtCell != null && num12 >= 0 && num12 < 256)
					{
						int x2 = num11 & 15;
						int y2 = num12;
						int z2 = num13 & 15;
						int cellValueFast = chunkAtCell.GetCellValueFast(x2, y2, z2);
						int num14 = Terrain.ExtractContents(cellValueFast);
						Block block = BlocksManager.Blocks[num14];
						float heat = SubsystemMetersBlockBehavior.GetHeat(cellValueFast);
						if (heat > 0f)
						{
							int num15 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num16 = (num15 <= 0) ? 1 : (4 * num15 * num15 + 2);
							float num17 = 1f / (float)num16;
							num5 += num17 * 36f * heat;
							num6 += num17;
						}
						else if (block.IsHeatBlocker(cellValueFast))
						{
							int num18 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num19 = (num18 <= 0) ? 1 : (4 * num18 * num18 + 2);
							float num20 = 1f / (float)num19;
							float num21 = (float)terrain.SeasonTemperature;
							float num22 = (float)SubsystemWeather.GetTemperatureAdjustmentAtHeight(y2);
							float num23 = (block is WaterBlock) ? (MathUtils.Max((float)chunkAtCell.GetTemperatureFast(x2, z2) + num21 - 6f, 0f) + num22) : ((!(block is IceBlock)) ? ((float)chunkAtCell.GetTemperatureFast(x2, z2) + num21 + num22) : (0f + num21 + num22));
							num += num20 * num23;
							num2 += num20;
						}
						else if (y >= chunkAtCell.GetTopHeightFast(x2, z2))
						{
							int num24 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num25 = (num24 <= 0) ? 1 : (4 * num24 * num24 + 2);
							float num26 = 1f / (float)num25;
							PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
							float num27 = (float)terrain.SeasonTemperature;
							float num28 = (y >= precipitationShaftInfo.YLimit) ? MathUtils.Lerp(0f, -2f, precipitationShaftInfo.Intensity) : 0f;
							float num29 = MathUtils.Lerp(-6f, 0f, this.m_subsystemSky.SkyLightIntensity);
							float num30 = (float)SubsystemWeather.GetTemperatureAdjustmentAtHeight(y2);
							num3 += num26 * ((float)chunkAtCell.GetTemperatureFast(x2, z2) + num27 + num28 + num29 + num30);
							num4 += num26;
						}
						else if (this.m_toVisit.Count < 4090)
						{
							if (num8 > -30)
							{
								this.m_toVisit.Add(num7 - 1);
							}
							if (num8 < 30)
							{
								this.m_toVisit.Add(num7 + 1);
							}
							if (num9 > -30)
							{
								this.m_toVisit.Add(num7 - 64);
							}
							if (num9 < 30)
							{
								this.m_toVisit.Add(num7 + 64);
							}
							if (num10 > -30)
							{
								this.m_toVisit.Add(num7 - 4096);
							}
							if (num10 < 30)
							{
								this.m_toVisit.Add(num7 + 4096);
							}
						}
					}
				}
			}
			float num31 = 0f;
			for (int k = -7; k <= 7; k++)
			{
				for (int l = -7; l <= 7; l++)
				{
					TerrainChunk chunkAtCell2 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + k, z + l);
					if (chunkAtCell2 != null && chunkAtCell2.State >= TerrainChunkState.InvalidVertices1)
					{
						for (int m = -7; m <= 7; m++)
						{
							int num32 = k * k + m * m + l * l;
							if (num32 <= 49 && num32 > 0)
							{
								int x3 = x + k & 15;
								int num33 = y + m;
								int z3 = z + l & 15;
								if (num33 >= 0 && num33 < 256)
								{
									float heat2 = SubsystemMetersBlockBehavior.GetHeat(chunkAtCell2.GetCellValueFast(x3, num33, z3));
									if (heat2 > 0f)
									{
										if (base.SubsystemTerrain.Raycast(new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f, 0.75f, 0.5f), new Vector3((float)(x + k), (float)(y + m), (float)(z + l)) + new Vector3(0.5f, 0.75f, 0.5f), false, true, delegate(int raycastValue, float d)
										{
											Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(raycastValue)];
											return block2.IsCollidable_(raycastValue) && !block2.IsTransparent_(raycastValue);
										}) == null)
										{
											num31 += heat2 * 3f / (float)(num32 + 2);
										}
									}
								}
							}
						}
					}
				}
			}
			float num34 = 0f;
			float num35 = 0f;
			if (num31 > 0f)
			{
				float num36 = 3f * num31;
				num34 += 35f * num36;
				num35 += num36;
			}
			if (num2 > 0f)
			{
				float num37 = 1f;
				num34 += num / num2 * num37;
				num35 += num37;
			}
			if (num4 > 0f)
			{
				float num38 = 4f * MathUtils.Pow(num4, 0.25f);
				num34 += num3 / num4 * num38;
				num35 += num38;
			}
			if (num6 > 0f)
			{
				float num39 = 1.5f * MathUtils.Pow(num6, 0.25f);
				num34 += num5 / num6 * num39;
				num35 += num39;
			}
			if (meterInsulation > 0f)
			{
				num34 += meterTemperature * meterInsulation;
				num35 += meterInsulation;
			}
			temperature = ((num35 > 0f) ? (num34 / num35) : meterTemperature);
			temperatureFlux = num35 - meterInsulation;
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0004CA0C File Offset: 0x0004AC0C
		public static float GetHeat(int value)
		{
			int num = Terrain.ExtractContents(value);
			return BlocksManager.Blocks[num].GetHeat(value);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0004CA30 File Offset: 0x0004AC30
		public void SimulateThermometer(int x, int y, int z, bool invalidateTerrainOnChange)
		{
			Point3 key = new Point3(x, y, z);
			if (!this.m_thermometersByPoint.ContainsKey(key))
			{
				return;
			}
			int num = this.m_thermometersByPoint[key];
			float x2;
			float num2;
			this.CalculateTemperature(x, y, z, 0f, 0f, out x2, out num2);
			int num3 = MathUtils.Clamp((int)MathUtils.Round(x2), 0, 15);
			if (num3 == num)
			{
				return;
			}
			this.m_thermometersByPoint[new Point3(x, y, z)] = num3;
			if (invalidateTerrainOnChange)
			{
				TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(x, z);
				if (chunkAtCell != null)
				{
					base.SubsystemTerrain.TerrainUpdater.DowngradeChunkNeighborhoodState(chunkAtCell.Coords, 0, TerrainChunkState.InvalidVertices1, true);
				}
			}
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0004CAD9 File Offset: 0x0004ACD9
		public void AddMeter(int value, int x, int y, int z)
		{
			if (Terrain.ExtractContents(value) == 120)
			{
				this.m_thermometersByPoint.Add(new Point3(x, y, z), 0);
				this.SimulateThermometer(x, y, z, false);
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0004CB05 File Offset: 0x0004AD05
		public void RemoveMeter(int value, int x, int y, int z)
		{
			this.m_thermometersByPoint.Remove(new Point3(x, y, z));
		}

		// Token: 0x04000580 RID: 1408
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000581 RID: 1409
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x04000582 RID: 1410
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000583 RID: 1411
		public Dictionary<Point3, int> m_thermometersByPoint = new Dictionary<Point3, int>();

		// Token: 0x04000584 RID: 1412
		public DynamicArray<Point3> m_thermometersToSimulate = new DynamicArray<Point3>();

		// Token: 0x04000585 RID: 1413
		public int m_thermometersToSimulateIndex;

		// Token: 0x04000586 RID: 1414
		public const int m_diameterBits = 6;

		// Token: 0x04000587 RID: 1415
		public const int m_diameter = 64;

		// Token: 0x04000588 RID: 1416
		public const int m_diameterMask = 63;

		// Token: 0x04000589 RID: 1417
		public const int m_radius = 32;

		// Token: 0x0400058A RID: 1418
		public DynamicArray<int> m_toVisit = new DynamicArray<int>();

		// Token: 0x0400058B RID: 1419
		public int[] m_visited = new int[8192];
	}
}
