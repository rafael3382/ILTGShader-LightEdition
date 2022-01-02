using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A8 RID: 424
	public abstract class SubsystemFluidBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000AB1 RID: 2737 RVA: 0x00047573 File Offset: 0x00045773
		// (set) Token: 0x06000AB2 RID: 2738 RVA: 0x0004757B File Offset: 0x0004577B
		public SubsystemTime SubsystemTime { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000AB3 RID: 2739 RVA: 0x00047584 File Offset: 0x00045784
		// (set) Token: 0x06000AB4 RID: 2740 RVA: 0x0004758C File Offset: 0x0004578C
		public SubsystemAudio SubsystemAudio { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000AB5 RID: 2741 RVA: 0x00047595 File Offset: 0x00045795
		// (set) Token: 0x06000AB6 RID: 2742 RVA: 0x0004759D File Offset: 0x0004579D
		public SubsystemAmbientSounds SubsystemAmbientSounds { get; set; }

		// Token: 0x06000AB7 RID: 2743 RVA: 0x000475A8 File Offset: 0x000457A8
		public SubsystemFluidBlockBehavior(FluidBlock fluidBlock, bool generateSources)
		{
			this.m_fluidBlock = fluidBlock;
			this.m_generateSources = generateSources;
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x000475F8 File Offset: 0x000457F8
		public void UpdateIsTop(int value, int x, int y, int z)
		{
			Terrain terrain = base.SubsystemTerrain.Terrain;
			if (y < 255)
			{
				TerrainChunk chunkAtCell = terrain.GetChunkAtCell(x, z);
				if (chunkAtCell != null)
				{
					int num = TerrainChunk.CalculateCellIndex(x & 15, y, z & 15);
					int contents = Terrain.ExtractContents(chunkAtCell.GetCellValueFast(num + 1));
					int data = Terrain.ExtractData(value);
					bool isTop = !this.m_fluidBlock.IsTheSameFluid(contents);
					chunkAtCell.SetCellValueFast(num, Terrain.ReplaceData(value, FluidBlock.SetIsTop(data, isTop)));
				}
			}
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x00047674 File Offset: 0x00045874
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateIsTop(value, x, y, z);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x00047682 File Offset: 0x00045882
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateIsTop(value, x, y, z);
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x00047690 File Offset: 0x00045890
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			this.m_toUpdate[new Point3
			{
				X = x,
				Y = y,
				Z = z
			}] = true;
			if (neighborY == y + 1)
			{
				this.UpdateIsTop(base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z), x, y, z);
			}
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x000476ED File Offset: 0x000458ED
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			newBlockValue = Terrain.MakeBlockValue(this.m_fluidBlock.BlockIndex);
			dropValue.Value = 0;
			dropValue.Count = 0;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x00047714 File Offset: 0x00045914
		public float? GetSurfaceHeight(int x, int y, int z, out FluidBlock surfaceFluidBlock)
		{
			if (y >= 0 && y < 255)
			{
				TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(x, z);
				if (chunkAtCell != null)
				{
					int num = TerrainChunk.CalculateCellIndex(x & 15, 0, z & 15);
					while (y < 255)
					{
						int num2 = Terrain.ExtractContents(chunkAtCell.GetCellValueFast(num + y + 1));
						if (BlocksManager.FluidBlocks[num2] == null)
						{
							int cellValueFast = chunkAtCell.GetCellValueFast(num + y);
							int num3 = Terrain.ExtractContents(cellValueFast);
							FluidBlock fluidBlock = BlocksManager.FluidBlocks[num3];
							if (fluidBlock != null)
							{
								surfaceFluidBlock = fluidBlock;
								int level = FluidBlock.GetLevel(Terrain.ExtractData(cellValueFast));
								return new float?((float)y + surfaceFluidBlock.GetLevelHeight(level));
							}
							surfaceFluidBlock = null;
							return null;
						}
						else
						{
							y++;
						}
					}
				}
			}
			surfaceFluidBlock = null;
			return null;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000477E8 File Offset: 0x000459E8
		public float? GetSurfaceHeight(int x, int y, int z)
		{
			FluidBlock fluidBlock;
			return this.GetSurfaceHeight(x, y, z, out fluidBlock);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x00047800 File Offset: 0x00045A00
		public Vector2? CalculateFlowSpeed(int x, int y, int z, out FluidBlock surfaceBlock, out float? surfaceHeight)
		{
			surfaceHeight = this.GetSurfaceHeight(x, y, z, out surfaceBlock);
			if (surfaceHeight != null)
			{
				y = (int)surfaceHeight.Value;
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
				int num = Terrain.ExtractContents(cellValue);
				if (BlocksManager.Blocks[num] is FluidBlock)
				{
					int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x - 1, y, z);
					int cellValue3 = base.SubsystemTerrain.Terrain.GetCellValue(x + 1, y, z);
					int cellValue4 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z - 1);
					int cellValue5 = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z + 1);
					int num2 = Terrain.ExtractContents(cellValue2);
					int num3 = Terrain.ExtractContents(cellValue3);
					int num4 = Terrain.ExtractContents(cellValue4);
					int num5 = Terrain.ExtractContents(cellValue5);
					int level = FluidBlock.GetLevel(Terrain.ExtractData(cellValue));
					int num6 = (num2 == num) ? FluidBlock.GetLevel(Terrain.ExtractData(cellValue2)) : level;
					int num7 = (num3 == num) ? FluidBlock.GetLevel(Terrain.ExtractData(cellValue3)) : level;
					int num8 = (num4 == num) ? FluidBlock.GetLevel(Terrain.ExtractData(cellValue4)) : level;
					int num9 = (num5 == num) ? FluidBlock.GetLevel(Terrain.ExtractData(cellValue5)) : level;
					Vector2 vector = new Vector2
					{
						X = (float)(MathUtils.Sign(level - num6) - MathUtils.Sign(level - num7)),
						Y = (float)(MathUtils.Sign(level - num8) - MathUtils.Sign(level - num9))
					};
					if (vector.LengthSquared() > 1f)
					{
						vector = Vector2.Normalize(vector);
					}
					Vector2 vector2;
					if (!this.m_fluidRandomFlowDirections.TryGetValue(new Point3(x, y, z), out vector2))
					{
						vector2.X = 0.05f * (2f * SimplexNoise.OctavedNoise((float)x + 0.2f * (float)this.SubsystemTime.GameTime, (float)z, 0.1f, 1, 1f, 1f, false) - 1f);
						vector2.Y = 0.05f * (2f * SimplexNoise.OctavedNoise((float)x + 0.2f * (float)this.SubsystemTime.GameTime + 100f, (float)z, 0.1f, 1, 1f, 1f, false) - 1f);
						if (this.m_fluidRandomFlowDirections.Count < 1000)
						{
							this.m_fluidRandomFlowDirections[new Point3(x, y, z)] = vector2;
						}
						else
						{
							this.m_fluidRandomFlowDirections.Clear();
						}
					}
					vector += vector2;
					return new Vector2?(vector * 2f);
				}
			}
			return null;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x00047AA4 File Offset: 0x00045CA4
		public Vector2? CalculateFlowSpeed(int x, int y, int z)
		{
			FluidBlock fluidBlock;
			float? num;
			return this.CalculateFlowSpeed(x, y, z, out fluidBlock, out num);
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x00047ABE File Offset: 0x00045CBE
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.SubsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.SubsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.SubsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x00047B00 File Offset: 0x00045D00
		public void SpreadFluid()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (Point3 point in this.m_toUpdate.Keys)
				{
					int x = point.X;
					int y = point.Y;
					int z = point.Z;
					int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
					int contents = Terrain.ExtractContents(cellValue);
					int data = Terrain.ExtractData(cellValue);
					int level = FluidBlock.GetLevel(data);
					if (this.m_fluidBlock.IsTheSameFluid(contents))
					{
						int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
						int contents2 = Terrain.ExtractContents(cellValue2);
						int data2 = Terrain.ExtractData(cellValue2);
						int level2 = FluidBlock.GetLevel(data2);
						int num = this.m_fluidBlock.MaxLevel + 1;
						int num2 = 0;
						for (int j = 0; j < 4; j++)
						{
							int cellValue3 = base.SubsystemTerrain.Terrain.GetCellValue(x + SubsystemFluidBlockBehavior.m_sideNeighbors[j].X, y, z + SubsystemFluidBlockBehavior.m_sideNeighbors[j].Y);
							int contents3 = Terrain.ExtractContents(cellValue3);
							if (this.m_fluidBlock.IsTheSameFluid(contents3))
							{
								int level3 = FluidBlock.GetLevel(Terrain.ExtractData(cellValue3));
								num = MathUtils.Min(num, level3);
								if (level3 == 0)
								{
									num2++;
								}
							}
						}
						if (level != 0 && level <= num)
						{
							int contents4 = Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z));
							if (!this.m_fluidBlock.IsTheSameFluid(contents4))
							{
								if (num + 1 > this.m_fluidBlock.MaxLevel)
								{
									this.Set(x, y, z, 0);
									continue;
								}
								this.Set(x, y, z, Terrain.MakeBlockValue(contents, 0, FluidBlock.SetLevel(data, num + 1)));
								continue;
							}
						}
						if (this.m_generateSources && level != 0 && num2 >= 2)
						{
							this.Set(x, y, z, Terrain.MakeBlockValue(contents, 0, FluidBlock.SetLevel(data, 0)));
						}
						else if (this.m_fluidBlock.IsTheSameFluid(contents2))
						{
							if (level2 > 1)
							{
								this.Set(x, y - 1, z, Terrain.MakeBlockValue(contents2, 0, FluidBlock.SetLevel(data2, 1)));
							}
						}
						else if (!this.OnFluidInteract(cellValue2, x, y - 1, z, Terrain.MakeBlockValue(this.m_fluidBlock.BlockIndex, 0, FluidBlock.SetLevel(0, 1))) && level < this.m_fluidBlock.MaxLevel)
						{
							this.m_visited.Clear();
							int num3 = this.LevelAtNearestFall(x + 1, y, z, level + 1, this.m_visited);
							int num4 = this.LevelAtNearestFall(x - 1, y, z, level + 1, this.m_visited);
							int num5 = this.LevelAtNearestFall(x, y, z + 1, level + 1, this.m_visited);
							int num6 = this.LevelAtNearestFall(x, y, z - 1, level + 1, this.m_visited);
							int num7 = MathUtils.Min(num3, num4, num5, num6);
							if (num3 == num7)
							{
								this.FlowTo(x + 1, y, z, level + 1);
								this.FlowTo(x, y, z - 1, this.m_fluidBlock.MaxLevel);
								this.FlowTo(x, y, z + 1, this.m_fluidBlock.MaxLevel);
							}
							if (num4 == num7)
							{
								this.FlowTo(x - 1, y, z, level + 1);
								this.FlowTo(x, y, z - 1, this.m_fluidBlock.MaxLevel);
								this.FlowTo(x, y, z + 1, this.m_fluidBlock.MaxLevel);
							}
							if (num5 == num7)
							{
								this.FlowTo(x, y, z + 1, level + 1);
								this.FlowTo(x - 1, y, z, this.m_fluidBlock.MaxLevel);
								this.FlowTo(x + 1, y, z, this.m_fluidBlock.MaxLevel);
							}
							if (num6 == num7)
							{
								this.FlowTo(x, y, z - 1, level + 1);
								this.FlowTo(x - 1, y, z, this.m_fluidBlock.MaxLevel);
								this.FlowTo(x + 1, y, z, this.m_fluidBlock.MaxLevel);
							}
						}
					}
				}
				this.m_toUpdate.Clear();
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toSet)
				{
					int x2 = keyValuePair.Key.X;
					int y2 = keyValuePair.Key.Y;
					int z2 = keyValuePair.Key.Z;
					int value = keyValuePair.Value;
					int contents5 = Terrain.ExtractContents(keyValuePair.Value);
					int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x2, y2, z2);
					FluidBlock fluidBlock = BlocksManager.FluidBlocks[cellContents];
					if (fluidBlock != null && !fluidBlock.IsTheSameFluid(contents5))
					{
						base.SubsystemTerrain.DestroyCell(0, x2, y2, z2, value, false, false);
					}
					else
					{
						base.SubsystemTerrain.ChangeCell(x2, y2, z2, value, true);
					}
				}
				this.m_toSet.Clear();
				base.SubsystemTerrain.ProcessModifiedCells();
			}
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x00048044 File Offset: 0x00046244
		public virtual bool OnFluidInteract(int interactValue, int x, int y, int z, int fluidValue)
		{
			if (!BlocksManager.Blocks[Terrain.ExtractContents(interactValue)].IsFluidBlocker_(interactValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				this.Set(x, y, z, fluidValue);
				return true;
			}
			return false;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0004807C File Offset: 0x0004627C
		public float? CalculateDistanceToFluid(Vector3 p, int radius, bool flowingFluidOnly)
		{
			float num = float.MaxValue;
			Terrain terrain = base.SubsystemTerrain.Terrain;
			int num2 = Terrain.ToCell(p.X) - radius;
			int num3 = Terrain.ToCell(p.X) + radius;
			int num4 = MathUtils.Clamp(Terrain.ToCell(p.Y) - radius, 0, 254);
			int num5 = MathUtils.Clamp(Terrain.ToCell(p.Y) + radius, 0, 254);
			int num6 = Terrain.ToCell(p.Z) - radius;
			int num7 = Terrain.ToCell(p.Z) + radius;
			for (int i = num6; i <= num7; i++)
			{
				for (int j = num2; j <= num3; j++)
				{
					TerrainChunk chunkAtCell = terrain.GetChunkAtCell(j, i);
					if (chunkAtCell != null)
					{
						int num8 = TerrainChunk.CalculateCellIndex(j & 15, num4, i & 15);
						int k = num4;
						while (k <= num5)
						{
							int cellValueFast = chunkAtCell.GetCellValueFast(num8);
							int contents = Terrain.ExtractContents(cellValueFast);
							if (this.m_fluidBlock.IsTheSameFluid(contents))
							{
								if (flowingFluidOnly)
								{
									if (FluidBlock.GetLevel(Terrain.ExtractData(cellValueFast)) == 0)
									{
										goto IL_15D;
									}
									int contents2 = Terrain.ExtractContents(chunkAtCell.GetCellValueFast(num8 + 1));
									if (this.m_fluidBlock.IsTheSameFluid(contents2))
									{
										goto IL_15D;
									}
								}
								float num9 = p.X - ((float)j + 0.5f);
								float num10 = p.Y - ((float)k + 1f);
								float num11 = p.Z - ((float)i + 0.5f);
								float num12 = num9 * num9 + num10 * num10 + num11 * num11;
								if (num12 < num)
								{
									num = num12;
								}
							}
							IL_15D:
							k++;
							num8++;
						}
					}
				}
			}
			if (num == 3.40282347E+38f)
			{
				return null;
			}
			return new float?(MathUtils.Sqrt(num));
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x00048238 File Offset: 0x00046438
		public void Set(int x, int y, int z, int value)
		{
			Point3 key = new Point3(x, y, z);
			if (!this.m_toSet.ContainsKey(key))
			{
				this.m_toSet[key] = value;
			}
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0004826C File Offset: 0x0004646C
		public void FlowTo(int x, int y, int z, int level)
		{
			if (level > this.m_fluidBlock.MaxLevel)
			{
				return;
			}
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int contents = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (this.m_fluidBlock.IsTheSameFluid(contents))
			{
				int level2 = FluidBlock.GetLevel(data);
				if (level < level2)
				{
					this.Set(x, y, z, Terrain.MakeBlockValue(contents, 0, FluidBlock.SetLevel(data, level)));
					return;
				}
			}
			else
			{
				this.OnFluidInteract(cellValue, x, y, z, Terrain.MakeBlockValue(this.m_fluidBlock.BlockIndex, 0, FluidBlock.SetLevel(0, level)));
			}
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x00048304 File Offset: 0x00046504
		public int LevelAtNearestFall(int x, int y, int z, int level, Dictionary<Point3, int> levels)
		{
			if (level > this.m_fluidBlock.MaxLevel)
			{
				return int.MaxValue;
			}
			int maxValue;
			if (!levels.TryGetValue(new Point3(x, y, z), out maxValue))
			{
				maxValue = int.MaxValue;
			}
			if (level >= maxValue)
			{
				return int.MaxValue;
			}
			levels[new Point3(x, y, z)] = level;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (this.m_fluidBlock.IsTheSameFluid(num))
			{
				if (FluidBlock.GetLevel(Terrain.ExtractData(cellValue)) < level)
				{
					return int.MaxValue;
				}
			}
			else if (BlocksManager.Blocks[num].IsFluidBlocker_(cellValue))
			{
				return int.MaxValue;
			}
			int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			int num2 = Terrain.ExtractContents(cellValue2);
			Block block = BlocksManager.Blocks[num2];
			if (this.m_fluidBlock.IsTheSameFluid(num2) || !block.IsFluidBlocker_(cellValue2))
			{
				return level;
			}
			int x2 = this.LevelAtNearestFall(x - 1, y, z, level + 1, levels);
			int x3 = this.LevelAtNearestFall(x + 1, y, z, level + 1, levels);
			int x4 = this.LevelAtNearestFall(x, y, z - 1, level + 1, levels);
			int x5 = this.LevelAtNearestFall(x, y, z + 1, level + 1, levels);
			return MathUtils.Min(x2, x3, x4, x5);
		}

		// Token: 0x04000541 RID: 1345
		public static Point2[] m_sideNeighbors = new Point2[]
		{
			new Point2(-1, 0),
			new Point2(1, 0),
			new Point2(0, -1),
			new Point2(0, 1)
		};

		// Token: 0x04000542 RID: 1346
		public FluidBlock m_fluidBlock;

		// Token: 0x04000543 RID: 1347
		public Dictionary<Point3, bool> m_toUpdate = new Dictionary<Point3, bool>();

		// Token: 0x04000544 RID: 1348
		public Dictionary<Point3, int> m_toSet = new Dictionary<Point3, int>();

		// Token: 0x04000545 RID: 1349
		public Dictionary<Point3, int> m_visited = new Dictionary<Point3, int>();

		// Token: 0x04000546 RID: 1350
		public Dictionary<Point3, Vector2> m_fluidRandomFlowDirections = new Dictionary<Point3, Vector2>();

		// Token: 0x04000547 RID: 1351
		public bool m_generateSources;
	}
}
