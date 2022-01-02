using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200031A RID: 794
	public class TerrainBrush
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06001768 RID: 5992 RVA: 0x000AEBF0 File Offset: 0x000ACDF0
		public TerrainBrush.Cell[] Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x000AEBF8 File Offset: 0x000ACDF8
		public static int Key(int x, int y, int z)
		{
			return y + 128 + (x + 128 << 8) + (z + 128 << 16);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x000AEC18 File Offset: 0x000ACE18
		public void Compile()
		{
			this.m_cells = new TerrainBrush.Cell[this.m_cellsDictionary.Values.Count];
			int num = 0;
			foreach (TerrainBrush.Cell cell in this.m_cellsDictionary.Values)
			{
				this.m_cells[num++] = cell;
			}
			Array.Sort<TerrainBrush.Cell>(this.m_cells);
			this.m_cellsDictionary = null;
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x000AECAC File Offset: 0x000ACEAC
		public int CountNonDiagonalNeighbors(int x, int y, int z, TerrainBrush.Counter counter)
		{
			return counter.Count(this, new Point3(x - 1, y, z)) + counter.Count(this, new Point3(x + 1, y, z)) + counter.Count(this, new Point3(x, y - 1, z)) + counter.Count(this, new Point3(x, y + 1, z)) + counter.Count(this, new Point3(x, y, z - 1)) + counter.Count(this, new Point3(x, y, z + 1));
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x000AED2C File Offset: 0x000ACF2C
		public int CountBox(int x, int y, int z, int sizeX, int sizeY, int sizeZ, TerrainBrush.Counter counter)
		{
			int num = 0;
			for (int i = x; i < x + sizeX; i++)
			{
				for (int j = y; j < y + sizeY; j++)
				{
					for (int k = z; k < z + sizeZ; k++)
					{
						num += counter.Count(this, new Point3(i, j, k));
					}
				}
			}
			return num;
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x000AED7C File Offset: 0x000ACF7C
		public void Replace(int oldValue, int newValue)
		{
			Dictionary<int, TerrainBrush.Cell> dictionary = new Dictionary<int, TerrainBrush.Cell>();
			foreach (KeyValuePair<int, TerrainBrush.Cell> keyValuePair in this.m_cellsDictionary)
			{
				TerrainBrush.Cell value = keyValuePair.Value;
				if (value.Value == oldValue)
				{
					value.Value = newValue;
				}
				dictionary[keyValuePair.Key] = value;
			}
			this.m_cellsDictionary = dictionary;
			this.m_cells = null;
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x000AEE04 File Offset: 0x000AD004
		public void CalculateBounds(out Point3 min, out Point3 max)
		{
			min = Point3.Zero;
			max = Point3.Zero;
			bool flag = true;
			foreach (TerrainBrush.Cell cell in this.m_cellsDictionary.Values)
			{
				if (flag)
				{
					flag = false;
					min.X = (max.X = (int)cell.X);
					min.Y = (max.Y = (int)cell.Y);
					min.Z = (max.Z = (int)cell.Z);
				}
				else
				{
					min.X = MathUtils.Min(min.X, (int)cell.X);
					min.Y = MathUtils.Min(min.Y, (int)cell.Y);
					min.Z = MathUtils.Min(min.Z, (int)cell.Z);
					max.X = MathUtils.Max(max.X, (int)cell.X);
					max.Y = MathUtils.Max(max.Y, (int)cell.Y);
					max.Z = MathUtils.Max(max.Z, (int)cell.Z);
				}
			}
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x000AEF48 File Offset: 0x000AD148
		public int? GetValue(Point3 p)
		{
			return this.GetValue(p.X, p.Y, p.Z);
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x000AEF64 File Offset: 0x000AD164
		public int? GetValue(int x, int y, int z)
		{
			int key = TerrainBrush.Key(x, y, z);
			TerrainBrush.Cell cell;
			if (this.m_cellsDictionary.TryGetValue(key, out cell))
			{
				return new int?(cell.Value);
			}
			return null;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x000AEFA0 File Offset: 0x000AD1A0
		public void AddCell(int x, int y, int z, TerrainBrush.Brush brush)
		{
			int? num = brush.Paint(this, new Point3(x, y, z));
			if (num != null)
			{
				int key = TerrainBrush.Key(x, y, z);
				this.m_cellsDictionary[key] = new TerrainBrush.Cell
				{
					X = (sbyte)x,
					Y = (sbyte)y,
					Z = (sbyte)z,
					Value = num.Value
				};
				this.m_cells = null;
			}
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000AF018 File Offset: 0x000AD218
		public void AddBox(int x, int y, int z, int sizeX, int sizeY, int sizeZ, TerrainBrush.Brush brush)
		{
			for (int i = x; i < x + sizeX; i++)
			{
				for (int j = y; j < y + sizeY; j++)
				{
					for (int k = z; k < z + sizeZ; k++)
					{
						this.AddCell(i, j, k, brush);
					}
				}
			}
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x000AF060 File Offset: 0x000AD260
		public void AddRay(int x1, int y1, int z1, int x2, int y2, int z2, int sizeX, int sizeY, int sizeZ, TerrainBrush.Brush brush)
		{
			Vector3 vector = new Vector3((float)x1, (float)y1, (float)z1) + new Vector3(0.5f);
			Vector3 vector2 = new Vector3((float)x2, (float)y2, (float)z2) + new Vector3(0.5f);
			Vector3 v = 0.33f * Vector3.Normalize(vector2 - vector);
			int num = (int)MathUtils.Round(3f * Vector3.Distance(vector, vector2));
			Vector3 vector3 = vector;
			for (int i = 0; i < num; i++)
			{
				int x3 = Terrain.ToCell(vector3.X);
				int y3 = Terrain.ToCell(vector3.Y);
				int z3 = Terrain.ToCell(vector3.Z);
				this.AddBox(x3, y3, z3, sizeX, sizeY, sizeZ, brush);
				vector3 += v;
			}
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x000AF12C File Offset: 0x000AD32C
		public void PaintFastSelective(TerrainChunk chunk, int x, int y, int z, int onlyInValue)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 256 && num3 >= 0 && num3 < 16)
				{
					int index = TerrainChunk.CalculateCellIndex(num, num2, num3);
					int cellValueFast = chunk.GetCellValueFast(index);
					if (onlyInValue == cellValueFast)
					{
						chunk.SetCellValueFast(index, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x000AF1E0 File Offset: 0x000AD3E0
		public void PaintFastSelective(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int onlyInValue)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ)
				{
					int cellValueFast = terrain.GetCellValueFast(num, num2, num3);
					if (onlyInValue == cellValueFast)
					{
						terrain.SetCellValueFast(num, num2, num3, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x000AF270 File Offset: 0x000AD470
		public void PaintFastAvoidWater(TerrainChunk chunk, int x, int y, int z)
		{
			Terrain terrain = chunk.Terrain;
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 255 && num3 >= 0 && num3 < 16)
				{
					int num4 = num + chunk.Origin.X;
					int y2 = num2;
					int num5 = num3 + chunk.Origin.Y;
					if (chunk.GetCellContentsFast(num, num2, num3) != 18 && terrain.GetCellContents(num4 - 1, y2, num5) != 18 && terrain.GetCellContents(num4 + 1, y2, num5) != 18 && terrain.GetCellContents(num4, y2, num5 - 1) != 18 && terrain.GetCellContents(num4, y2, num5 + 1) != 18 && chunk.GetCellContentsFast(num, num2 + 1, num3) != 18)
					{
						chunk.SetCellValueFast(num, num2, num3, cell.Value);
					}
				}
			}
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x000AF3BC File Offset: 0x000AD5BC
		public void PaintFastAvoidWater(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ && terrain.GetCellContentsFast(num, num2, num3) != 18 && terrain.GetCellContents(num - 1, num2, num3) != 18 && terrain.GetCellContents(num + 1, num2, num3) != 18 && terrain.GetCellContents(num, num2, num3 - 1) != 18 && terrain.GetCellContents(num, num2, num3 + 1) != 18 && terrain.GetCellContentsFast(num, num2 + 1, num3) != 18)
				{
					terrain.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x000AF4B0 File Offset: 0x000AD6B0
		public void PaintFast(TerrainChunk chunk, int x, int y, int z)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= 0 && num < 16 && num2 >= 0 && num2 < 256 && num3 >= 0 && num3 < 16)
				{
					chunk.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x000AF54C File Offset: 0x000AD74C
		public void PaintFast(Terrain terrain, int x, int y, int z, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int num = (int)cell.X + x;
				int num2 = (int)cell.Y + y;
				int num3 = (int)cell.Z + z;
				if (num >= minX && num < maxX && num2 >= minY && num2 < maxY && num3 >= minZ && num3 < maxZ)
				{
					terrain.SetCellValueFast(num, num2, num3, cell.Value);
				}
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x000AF5C8 File Offset: 0x000AD7C8
		public void Paint(SubsystemTerrain terrain, int x, int y, int z)
		{
			foreach (TerrainBrush.Cell cell in this.Cells)
			{
				int x2 = (int)cell.X + x;
				int y2 = (int)cell.Y + y;
				int z2 = (int)cell.Z + z;
				terrain.ChangeCell(x2, y2, z2, cell.Value, true);
			}
		}

		// Token: 0x04000FF1 RID: 4081
		public Dictionary<int, TerrainBrush.Cell> m_cellsDictionary = new Dictionary<int, TerrainBrush.Cell>();

		// Token: 0x04000FF2 RID: 4082
		public TerrainBrush.Cell[] m_cells;

		// Token: 0x02000542 RID: 1346
		public struct Cell : IComparable<TerrainBrush.Cell>
		{
			// Token: 0x06002243 RID: 8771 RVA: 0x000EC227 File Offset: 0x000EA427
			public int CompareTo(TerrainBrush.Cell other)
			{
				return TerrainBrush.Key((int)this.X, (int)this.Y, (int)this.Z) - TerrainBrush.Key((int)other.X, (int)other.Y, (int)other.Z);
			}

			// Token: 0x040018F4 RID: 6388
			public sbyte X;

			// Token: 0x040018F5 RID: 6389
			public sbyte Y;

			// Token: 0x040018F6 RID: 6390
			public sbyte Z;

			// Token: 0x040018F7 RID: 6391
			public int Value;
		}

		// Token: 0x02000543 RID: 1347
		public class Brush
		{
			// Token: 0x06002245 RID: 8773 RVA: 0x000EC260 File Offset: 0x000EA460
			public static implicit operator TerrainBrush.Brush(int value)
			{
				return new TerrainBrush.Brush
				{
					m_value = value
				};
			}

			// Token: 0x06002246 RID: 8774 RVA: 0x000EC26E File Offset: 0x000EA46E
			public static implicit operator TerrainBrush.Brush(Func<int?, int?> handler)
			{
				return new TerrainBrush.Brush
				{
					m_handler1 = handler
				};
			}

			// Token: 0x06002247 RID: 8775 RVA: 0x000EC27C File Offset: 0x000EA47C
			public static implicit operator TerrainBrush.Brush(Func<Point3, int?> handler)
			{
				return new TerrainBrush.Brush
				{
					m_handler2 = handler
				};
			}

			// Token: 0x06002248 RID: 8776 RVA: 0x000EC28C File Offset: 0x000EA48C
			public int? Paint(TerrainBrush terrainBrush, Point3 p)
			{
				if (this.m_handler1 != null)
				{
					return this.m_handler1(terrainBrush.GetValue(p.X, p.Y, p.Z));
				}
				if (this.m_handler2 != null)
				{
					return this.m_handler2(p);
				}
				return new int?(this.m_value);
			}

			// Token: 0x040018F8 RID: 6392
			public int m_value;

			// Token: 0x040018F9 RID: 6393
			public Func<int?, int?> m_handler1;

			// Token: 0x040018FA RID: 6394
			public Func<Point3, int?> m_handler2;
		}

		// Token: 0x02000544 RID: 1348
		public class Counter
		{
			// Token: 0x0600224A RID: 8778 RVA: 0x000EC2ED File Offset: 0x000EA4ED
			public static implicit operator TerrainBrush.Counter(int value)
			{
				return new TerrainBrush.Counter
				{
					m_value = value
				};
			}

			// Token: 0x0600224B RID: 8779 RVA: 0x000EC2FB File Offset: 0x000EA4FB
			public static implicit operator TerrainBrush.Counter(Func<int?, int> handler)
			{
				return new TerrainBrush.Counter
				{
					m_handler1 = handler
				};
			}

			// Token: 0x0600224C RID: 8780 RVA: 0x000EC309 File Offset: 0x000EA509
			public static implicit operator TerrainBrush.Counter(Func<Point3, int> handler)
			{
				return new TerrainBrush.Counter
				{
					m_handler2 = handler
				};
			}

			// Token: 0x0600224D RID: 8781 RVA: 0x000EC318 File Offset: 0x000EA518
			public int Count(TerrainBrush terrainBrush, Point3 p)
			{
				if (this.m_handler1 != null)
				{
					return this.m_handler1(terrainBrush.GetValue(p));
				}
				if (this.m_handler2 != null)
				{
					return this.m_handler2(p);
				}
				int? value = terrainBrush.GetValue(p);
				int value2 = this.m_value;
				if (!(value.GetValueOrDefault() == value2 & value != null))
				{
					return 0;
				}
				return 1;
			}

			// Token: 0x040018FB RID: 6395
			public int m_value;

			// Token: 0x040018FC RID: 6396
			public Func<int?, int> m_handler1;

			// Token: 0x040018FD RID: 6397
			public Func<Point3, int> m_handler2;
		}
	}
}
