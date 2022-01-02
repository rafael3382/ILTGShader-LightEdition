using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AD RID: 429
	public class SubsystemGrassBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x0004A70F File Offset: 0x0004890F
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					8
				};
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000B0F RID: 2831 RVA: 0x0004A71B File Offset: 0x0004891B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0004A720 File Offset: 0x00048920
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (Terrain.ExtractData(value) != 0 || this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			int num = Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z));
			if (num == 0)
			{
				this.m_toUpdate[new Point3(x, y, z)] = Terrain.ReplaceContents(value, 8);
			}
			if (num < 13)
			{
				return;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					for (int k = y - 2; k <= y + 1; k++)
					{
						int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(i, k, j);
						if (Terrain.ExtractContents(cellValue) == 2)
						{
							int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(i, k + 1, j);
							if (!this.KillsGrassIfOnTopOfIt(cellValue2) && Terrain.ExtractLight(cellValue2) >= 13 && this.m_random.Float(0f, 1f) < 0.1f)
							{
								int num2 = Terrain.ReplaceContents(cellValue, 8);
								this.m_toUpdate[new Point3(i, k, j)] = num2;
								if (Terrain.ExtractContents(cellValue2) == 0)
								{
									int temperature = base.SubsystemTerrain.Terrain.GetTemperature(i, j);
									int humidity = base.SubsystemTerrain.Terrain.GetHumidity(i, j);
									int num3 = PlantsManager.GenerateRandomPlantValue(this.m_random, num2, temperature, humidity, k + 1);
									if (num3 != 0)
									{
										this.m_toUpdate[new Point3(i, k + 1, j)] = num3;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x0004A8BC File Offset: 0x00048ABC
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 61)
			{
				int value = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				value = Terrain.ReplaceData(value, 1);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
			}
			else
			{
				int value2 = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				value2 = Terrain.ReplaceData(value2, 0);
				base.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
			}
			if (this.KillsGrassIfOnTopOfIt(cellValue))
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(2, 0, 0), true);
			}
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0004A963 File Offset: 0x00048B63
		public override void OnExplosion(int value, int x, int y, int z, float damage)
		{
			if (damage > BlocksManager.Blocks[8].ExplosionResilience * this.m_random.Float(0f, 1f))
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(2, 0, 0), true);
			}
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0004A9A3 File Offset: 0x00048BA3
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0004A9D0 File Offset: 0x00048BD0
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 0.0))
			{
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toUpdate)
				{
					if (Terrain.ExtractContents(keyValuePair.Value) == 8)
					{
						if (base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) != 2)
						{
							continue;
						}
					}
					else
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y - 1, keyValuePair.Key.Z);
						if ((cellContents != 8 && cellContents != 2) || base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) != 0)
						{
							continue;
						}
					}
					base.SubsystemTerrain.ChangeCell(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z, keyValuePair.Value, true);
				}
				this.m_toUpdate.Clear();
			}
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0004AB54 File Offset: 0x00048D54
		public bool KillsGrassIfOnTopOfIt(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return block is FluidBlock || (!block.IsFaceTransparent(base.SubsystemTerrain, 5, value) && block.IsCollidable_(value));
		}

		// Token: 0x04000564 RID: 1380
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000565 RID: 1381
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000566 RID: 1382
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();

		// Token: 0x04000567 RID: 1383
		public Game.Random m_random = new Game.Random();
	}
}
