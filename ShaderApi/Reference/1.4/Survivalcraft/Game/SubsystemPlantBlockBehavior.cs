using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C5 RID: 453
	public class SubsystemPlantBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00053303 File Offset: 0x00051503
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					19,
					20,
					24,
					25,
					28,
					99,
					131,
					244,
					132,
					174,
					204
				};
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00053317 File Offset: 0x00051517
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0005331C File Offset: 0x0005151C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int num = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z));
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			int num2 = Terrain.ExtractContents(cellValue);
			if (num != 131)
			{
				if (num != 132)
				{
					if (num != 244)
					{
						if (num2 != 8 && num2 != 2 && num2 != 7 && num2 != 168)
						{
							base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
							return;
						}
						return;
					}
				}
				else
				{
					Block block = BlocksManager.Blocks[num2];
					if (block.IsFaceTransparent(base.SubsystemTerrain, 4, cellValue) && !(block is FenceBlock))
					{
						base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
						return;
					}
					return;
				}
			}
			if (num2 != 8 && num2 != 2 && num2 != 168)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				return;
			}
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x000533F4 File Offset: 0x000515F4
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living || y <= 0 || y >= 255)
			{
				return;
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (num == 19)
			{
				this.GrowTallGrass(value, x, y, z, pollPass);
				return;
			}
			if (block is FlowerBlock)
			{
				this.GrowFlower(value, x, y, z, pollPass);
				return;
			}
			if (num == 131)
			{
				this.GrowPumpkin(value, x, y, z, pollPass);
				return;
			}
			if (num == 174)
			{
				this.GrowRye(value, x, y, z, pollPass);
				return;
			}
			if (num != 204)
			{
				return;
			}
			this.GrowCotton(value, x, y, z, pollPass);
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0005349A File Offset: 0x0005169A
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x000534C8 File Offset: 0x000516C8
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(30.0, 0.0))
			{
				foreach (KeyValuePair<Point3, SubsystemPlantBlockBehavior.Replacement> keyValuePair in this.m_toReplace)
				{
					Point3 key = keyValuePair.Key;
					if (Terrain.ReplaceLight(base.SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z), 0) == Terrain.ReplaceLight(keyValuePair.Value.RequiredValue, 0))
					{
						base.SubsystemTerrain.ChangeCell(key.X, key.Y, key.Z, keyValuePair.Value.Value, true);
					}
				}
				this.m_toReplace.Clear();
			}
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x000535B0 File Offset: 0x000517B0
		public void GrowTallGrass(int value, int x, int y, int z, int pollPass)
		{
			int data = Terrain.ExtractData(value);
			if (TallGrassBlock.GetIsSmall(data) && Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z)) >= 9)
			{
				int data2 = TallGrassBlock.SetIsSmall(data, false);
				int value2 = Terrain.ReplaceData(value, data2);
				this.m_toReplace[new Point3(x, y, z)] = new SubsystemPlantBlockBehavior.Replacement
				{
					Value = value2,
					RequiredValue = value
				};
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x00053628 File Offset: 0x00051828
		public void GrowFlower(int value, int x, int y, int z, int pollPass)
		{
			int data = Terrain.ExtractData(value);
			if (FlowerBlock.GetIsSmall(data) && Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z)) >= 9)
			{
				int data2 = FlowerBlock.SetIsSmall(data, false);
				int value2 = Terrain.ReplaceData(value, data2);
				this.m_toReplace[new Point3(x, y, z)] = new SubsystemPlantBlockBehavior.Replacement
				{
					Value = value2,
					RequiredValue = value
				};
			}
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x000536A0 File Offset: 0x000518A0
		public void GrowRye(int value, int x, int y, int z, int pollPass)
		{
			if (Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z)) < 9)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int size = RyeBlock.GetSize(data);
			if (size == 7)
			{
				return;
			}
			if (RyeBlock.GetIsWild(data))
			{
				if (size < 7)
				{
					int data2 = RyeBlock.SetSize(RyeBlock.SetIsWild(data, true), size + 1);
					int value2 = Terrain.ReplaceData(value, data2);
					Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace = this.m_toReplace;
					Point3 key = new Point3(x, y, z);
					SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
					{
						Value = value2,
						RequiredValue = value
					};
					toReplace[key] = value3;
				}
				return;
			}
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y - 1, z);
			if (Terrain.ExtractContents(cellValueFast) == 168)
			{
				int data3 = Terrain.ExtractData(cellValueFast);
				bool hydration = SoilBlock.GetHydration(data3);
				int nitrogen = SoilBlock.GetNitrogen(data3);
				int num = 3;
				float num2 = 0.8f;
				if (nitrogen > 0)
				{
					num--;
					num2 -= 0.4f;
				}
				if (hydration)
				{
					num--;
					num2 -= 0.4f;
				}
				if (pollPass % MathUtils.Max(num, 1) == 0)
				{
					int data4 = RyeBlock.SetSize(data, MathUtils.Min(size + 1, 7));
					if (this.m_random.Float(0f, 1f) < num2 && size == 3)
					{
						data4 = RyeBlock.SetIsWild(data4, true);
					}
					int value4 = Terrain.ReplaceData(value, data4);
					SubsystemPlantBlockBehavior.Replacement replacement = this.m_toReplace[new Point3(x, y, z)] = new SubsystemPlantBlockBehavior.Replacement
					{
						Value = value4,
						RequiredValue = value
					};
					if (size + 1 == 7)
					{
						int data5 = SoilBlock.SetNitrogen(data3, MathUtils.Max(nitrogen - 1, 0));
						int value5 = Terrain.ReplaceData(cellValueFast, data5);
						Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace2 = this.m_toReplace;
						Point3 key2 = new Point3(x, y - 1, z);
						SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
						{
							Value = value5,
							RequiredValue = cellValueFast
						};
						toReplace2[key2] = value3;
						return;
					}
				}
			}
			else
			{
				int value6 = Terrain.ReplaceData(value, RyeBlock.SetIsWild(data, true));
				Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace3 = this.m_toReplace;
				Point3 key3 = new Point3(x, y, z);
				SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
				{
					Value = value6,
					RequiredValue = value
				};
				toReplace3[key3] = value3;
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x000538DC File Offset: 0x00051ADC
		public void GrowCotton(int value, int x, int y, int z, int pollPass)
		{
			if (Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z)) < 9)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int size = CottonBlock.GetSize(data);
			if (size >= 2)
			{
				return;
			}
			if (CottonBlock.GetIsWild(data))
			{
				if (size < 2)
				{
					int data2 = CottonBlock.SetSize(CottonBlock.SetIsWild(data, true), size + 1);
					int value2 = Terrain.ReplaceData(value, data2);
					Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace = this.m_toReplace;
					Point3 key = new Point3(x, y, z);
					SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
					{
						Value = value2,
						RequiredValue = value
					};
					toReplace[key] = value3;
				}
				return;
			}
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y - 1, z);
			if (Terrain.ExtractContents(cellValueFast) == 168)
			{
				int data3 = Terrain.ExtractData(cellValueFast);
				bool hydration = SoilBlock.GetHydration(data3);
				int nitrogen = SoilBlock.GetNitrogen(data3);
				int num = 6;
				float num2 = 0.8f;
				if (nitrogen > 0)
				{
					num -= 2;
					num2 -= 0.4f;
				}
				if (hydration)
				{
					num -= 2;
					num2 -= 0.4f;
				}
				if (pollPass % MathUtils.Max(num, 1) == 0)
				{
					int data4 = CottonBlock.SetSize(data, MathUtils.Min(size + 1, 2));
					if (this.m_random.Float(0f, 1f) < num2 && size == 1)
					{
						data4 = CottonBlock.SetIsWild(data4, true);
					}
					int value4 = Terrain.ReplaceData(value, data4);
					SubsystemPlantBlockBehavior.Replacement replacement = this.m_toReplace[new Point3(x, y, z)] = new SubsystemPlantBlockBehavior.Replacement
					{
						Value = value4,
						RequiredValue = value
					};
					if (size + 1 == 2)
					{
						int data5 = SoilBlock.SetNitrogen(data3, MathUtils.Max(nitrogen - 1, 0));
						int value5 = Terrain.ReplaceData(cellValueFast, data5);
						Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace2 = this.m_toReplace;
						Point3 key2 = new Point3(x, y - 1, z);
						SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
						{
							Value = value5,
							RequiredValue = cellValueFast
						};
						toReplace2[key2] = value3;
						return;
					}
				}
			}
			else
			{
				int value6 = Terrain.ReplaceData(value, CottonBlock.SetIsWild(data, true));
				Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace3 = this.m_toReplace;
				Point3 key3 = new Point3(x, y, z);
				SubsystemPlantBlockBehavior.Replacement value3 = new SubsystemPlantBlockBehavior.Replacement
				{
					Value = value6,
					RequiredValue = value
				};
				toReplace3[key3] = value3;
			}
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x00053B18 File Offset: 0x00051D18
		public void GrowPumpkin(int value, int x, int y, int z, int pollPass)
		{
			if (Terrain.ExtractLight(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z)) < 9)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int size = BasePumpkinBlock.GetSize(data);
			if (BasePumpkinBlock.GetIsDead(data) || size >= 7)
			{
				return;
			}
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y - 1, z);
			int num = Terrain.ExtractContents(cellValueFast);
			int data2 = Terrain.ExtractData(cellValueFast);
			bool flag = num == 168 && SoilBlock.GetHydration(data2);
			int num2 = (num == 168) ? SoilBlock.GetNitrogen(data2) : 0;
			int num3 = 4;
			float num4 = 0.15f;
			if (num == 168)
			{
				num3--;
				num4 -= 0.05f;
			}
			if (num2 > 0)
			{
				num3--;
				num4 -= 0.05f;
			}
			if (flag)
			{
				num3--;
				num4 -= 0.05f;
			}
			if (pollPass % MathUtils.Max(num3, 1) == 0)
			{
				int data3 = BasePumpkinBlock.SetSize(data, MathUtils.Min(size + 1, 7));
				if (this.m_random.Float(0f, 1f) < num4)
				{
					data3 = BasePumpkinBlock.SetIsDead(data3, true);
				}
				int value2 = Terrain.ReplaceData(value, data3);
				SubsystemPlantBlockBehavior.Replacement replacement = this.m_toReplace[new Point3(x, y, z)] = new SubsystemPlantBlockBehavior.Replacement
				{
					Value = value2,
					RequiredValue = value
				};
				if (num == 168 && size + 1 == 7)
				{
					int data4 = SoilBlock.SetNitrogen(data2, MathUtils.Max(num2 - 3, 0));
					int value3 = Terrain.ReplaceData(cellValueFast, data4);
					Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> toReplace = this.m_toReplace;
					Point3 key = new Point3(x, y - 1, z);
					SubsystemPlantBlockBehavior.Replacement value4 = new SubsystemPlantBlockBehavior.Replacement
					{
						Value = value3,
						RequiredValue = cellValueFast
					};
					toReplace[key] = value4;
				}
			}
		}

		// Token: 0x040005E7 RID: 1511
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005E8 RID: 1512
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040005E9 RID: 1513
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005EA RID: 1514
		public Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement> m_toReplace = new Dictionary<Point3, SubsystemPlantBlockBehavior.Replacement>();

		// Token: 0x020004A2 RID: 1186
		public struct Replacement
		{
			// Token: 0x04001712 RID: 5906
			public int RequiredValue;

			// Token: 0x04001713 RID: 5907
			public int Value;
		}
	}
}
