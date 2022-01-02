using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CE RID: 462
	public class SubsystemSaplingBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x000562CD File Offset: 0x000544CD
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					119
				};
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000C33 RID: 3123 RVA: 0x000562DA File Offset: 0x000544DA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000562E0 File Offset: 0x000544E0
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x0005632C File Offset: 0x0005452C
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			float num = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? this.m_random.Float(8f, 12f) : this.m_random.Float(480f, 600f);
			this.AddSapling(new SubsystemSaplingBlockBehavior.SaplingData
			{
				Point = new Point3(x, y, z),
				Type = (TreeType)Terrain.ExtractData(value),
				MatureTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)num
			});
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x000563B3 File Offset: 0x000545B3
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveSapling(new Point3(x, y, z));
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000563C8 File Offset: 0x000545C8
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Saplings").Values)
			{
				string data = (string)obj;
				this.AddSapling(this.LoadSaplingData(data));
			}
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x0005645C File Offset: 0x0005465C
		public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Saplings", valuesDictionary2);
			int num = 0;
			foreach (SubsystemSaplingBlockBehavior.SaplingData saplingData in this.m_saplings.Values)
			{
				valuesDictionary2.SetValue<string>(num++.ToString(CultureInfo.InvariantCulture), this.SaveSaplingData(saplingData));
			}
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x000564E4 File Offset: 0x000546E4
		public void Update(float dt)
		{
			for (int i = 0; i < 10; i++)
			{
				if (!this.m_enumerator.MoveNext())
				{
					this.m_enumerator = this.m_saplings.Values.GetEnumerator();
					return;
				}
				this.MatureSapling(this.m_enumerator.Current);
			}
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x00056534 File Offset: 0x00054734
		public SubsystemSaplingBlockBehavior.SaplingData LoadSaplingData(string data)
		{
			string[] array = data.Split(new string[]
			{
				";"
			}, StringSplitOptions.None);
			if (array.Length != 3)
			{
				throw new InvalidOperationException("Invalid sapling data string.");
			}
			return new SubsystemSaplingBlockBehavior.SaplingData
			{
				Point = HumanReadableConverter.ConvertFromString<Point3>(array[0]),
				Type = HumanReadableConverter.ConvertFromString<TreeType>(array[1]),
				MatureTime = HumanReadableConverter.ConvertFromString<double>(array[2])
			};
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x00056598 File Offset: 0x00054798
		public string SaveSaplingData(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			this.m_stringBuilder.Length = 0;
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.Point));
			this.m_stringBuilder.Append(';');
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.Type));
			this.m_stringBuilder.Append(';');
			this.m_stringBuilder.Append(HumanReadableConverter.ConvertToString(saplingData.MatureTime));
			return this.m_stringBuilder.ToString();
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x0005662C File Offset: 0x0005482C
		public void MatureSapling(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			if (this.m_subsystemGameInfo.TotalElapsedGameTime < saplingData.MatureTime)
			{
				return;
			}
			int x = saplingData.Point.X;
			int y = saplingData.Point.Y;
			int z = saplingData.Point.Z;
			TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(x - 6, z - 6);
			TerrainChunk chunkAtCell2 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x - 6, z + 6);
			TerrainChunk chunkAtCell3 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + 6, z - 6);
			TerrainChunk chunkAtCell4 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + 6, z + 6);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid && chunkAtCell2 != null && chunkAtCell2.State == TerrainChunkState.Valid && chunkAtCell3 != null && chunkAtCell3.State == TerrainChunkState.Valid && chunkAtCell4 != null && chunkAtCell4.State == TerrainChunkState.Valid)
			{
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (cellContents != 2 && cellContents != 8)
				{
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
					return;
				}
				if (base.SubsystemTerrain.Terrain.GetCellLight(x, y + 1, z) >= 9)
				{
					bool flag = false;
					for (int i = x - 1; i <= x + 1; i++)
					{
						for (int j = z - 1; j <= z + 1; j++)
						{
							int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(i, y - 1, j);
							if (BlocksManager.Blocks[cellContents2] is WaterBlock)
							{
								flag = true;
								break;
							}
						}
					}
					float probability;
					if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
					{
						probability = 1f;
					}
					else
					{
						int num = base.SubsystemTerrain.Terrain.GetTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
						int num2 = base.SubsystemTerrain.Terrain.GetHumidity(x, z);
						if (flag)
						{
							num = (num + 10) / 2;
							num2 = MathUtils.Max(num2, 12);
						}
						probability = 2f * PlantsManager.CalculateTreeProbability(saplingData.Type, num, num2, y);
					}
					if (!this.m_random.Bool(probability))
					{
						base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
						return;
					}
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(0, 0, 0), true);
					if (!this.GrowTree(x, y, z, saplingData.Type))
					{
						base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
						return;
					}
				}
				else if (this.m_subsystemGameInfo.TotalElapsedGameTime > saplingData.MatureTime + 1200.0)
				{
					base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(28, 0, 0), true);
					return;
				}
			}
			else
			{
				saplingData.MatureTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			}
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x00056908 File Offset: 0x00054B08
		public bool GrowTree(int x, int y, int z, TreeType treeType)
		{
			ReadOnlyList<TerrainBrush> treeBrushes = PlantsManager.GetTreeBrushes(treeType);
			for (int i = 0; i < 20; i++)
			{
				TerrainBrush terrainBrush = treeBrushes[this.m_random.Int(0, treeBrushes.Count - 1)];
				bool flag = true;
				foreach (TerrainBrush.Cell cell in terrainBrush.Cells)
				{
					if (cell.Y >= 0 && (cell.X != 0 || cell.Y != 0 || cell.Z != 0))
					{
						int cellContents = base.SubsystemTerrain.Terrain.GetCellContents((int)cell.X + x, (int)cell.Y + y, (int)cell.Z + z);
						if (cellContents != 0 && !(BlocksManager.Blocks[cellContents] is LeavesBlock))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					terrainBrush.Paint(base.SubsystemTerrain, x, y, z);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000569F6 File Offset: 0x00054BF6
		public void AddSapling(SubsystemSaplingBlockBehavior.SaplingData saplingData)
		{
			this.m_saplings[saplingData.Point] = saplingData;
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x00056A20 File Offset: 0x00054C20
		public void RemoveSapling(Point3 point)
		{
			this.m_saplings.Remove(point);
			this.m_enumerator = this.m_saplings.Values.GetEnumerator();
		}

		// Token: 0x04000617 RID: 1559
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000618 RID: 1560
		public Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData> m_saplings = new Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData>();

		// Token: 0x04000619 RID: 1561
		public Dictionary<Point3, SubsystemSaplingBlockBehavior.SaplingData>.ValueCollection.Enumerator m_enumerator;

		// Token: 0x0400061A RID: 1562
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400061B RID: 1563
		public StringBuilder m_stringBuilder = new StringBuilder();

		// Token: 0x020004A4 RID: 1188
		public class SaplingData
		{
			// Token: 0x0400171A RID: 5914
			public Point3 Point;

			// Token: 0x0400171B RID: 5915
			public TreeType Type;

			// Token: 0x0400171C RID: 5916
			public double MatureTime;
		}
	}
}
