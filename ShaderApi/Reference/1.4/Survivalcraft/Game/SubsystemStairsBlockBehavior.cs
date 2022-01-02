﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x020001D7 RID: 471
	public class SubsystemStairsBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0005BD37 File Offset: 0x00059F37
		public override int[] HandledBlocks
		{
			get
			{
				return this.m_handledBlocks;
			}
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0005BD40 File Offset: 0x00059F40
		public SubsystemStairsBlockBehavior()
		{
			List<int> list = new List<int>();
			list.AddRange(from b in BlocksManager.Blocks
			where b is StairsBlock
			select b.BlockIndex);
			this.m_handledBlocks = list.ToArray();
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0005BDB8 File Offset: 0x00059FB8
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			this.UpdateIsCorner(cellValue, x, y, z, true);
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0005BDE4 File Offset: 0x00059FE4
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.UpdateIsCorner(value, x, y, z, false);
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0005BDF2 File Offset: 0x00059FF2
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.UpdateIsCorner(value, x, y, z, true);
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x0005BE04 File Offset: 0x0005A004
		public void UpdateIsCorner(int value, int x, int y, int z, bool updateModificationCounter)
		{
			int value2 = Terrain.ExtractContents(value);
			if (!this.HandledBlocks.Contains(value2))
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			if (StairsBlock.GetCornerType(data) != StairsBlock.CornerType.None)
			{
				return;
			}
			int rotation = StairsBlock.GetRotation(data);
			bool isUpsideDown = StairsBlock.GetIsUpsideDown(data);
			Point3 point = StairsBlock.RotationToDirection(rotation);
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is StairsBlock)
			{
				int data2 = Terrain.ExtractData(cellValue);
				bool isUpsideDown2 = StairsBlock.GetIsUpsideDown(data2);
				StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(data2);
				int num2 = -1;
				if (isUpsideDown2 == isUpsideDown)
				{
					int rotation2 = StairsBlock.GetRotation(data2);
					if (rotation == 0 && rotation2 == 1 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 1;
					}
					if (rotation == 0 && rotation2 == 3 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 0;
					}
					if (rotation == 1 && rotation2 == 0 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 1;
					}
					if (rotation == 1 && rotation2 == 2 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 2;
					}
					if (rotation == 2 && rotation2 == 1 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 2;
					}
					if (rotation == 2 && rotation2 == 3 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 3;
					}
					if (rotation == 3 && rotation2 == 0 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 0;
					}
					if (rotation == 3 && rotation2 == 2 && cornerType != StairsBlock.CornerType.ThreeQuarters)
					{
						num2 = 3;
					}
				}
				if (num2 >= 0)
				{
					int data3 = StairsBlock.SetRotation(StairsBlock.SetCornerType(data, StairsBlock.CornerType.OneQuarter), num2);
					int value3 = Terrain.ReplaceData(value, data3);
					base.SubsystemTerrain.ChangeCell(x, y, z, value3, updateModificationCounter);
				}
				return;
			}
			cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
			num = Terrain.ExtractContents(cellValue);
			if (!(BlocksManager.Blocks[num] is StairsBlock))
			{
				return;
			}
			int data4 = Terrain.ExtractData(cellValue);
			bool isUpsideDown3 = StairsBlock.GetIsUpsideDown(data4);
			StairsBlock.CornerType cornerType2 = StairsBlock.GetCornerType(data4);
			int num3 = -1;
			if (isUpsideDown3 == isUpsideDown)
			{
				int rotation3 = StairsBlock.GetRotation(data4);
				if (rotation == 0 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 1;
				}
				if (rotation == 0 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 0;
				}
				if (rotation == 0 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 1;
				}
				if (rotation == 0 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 0;
				}
				if (rotation == 1 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 1;
				}
				if (rotation == 1 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 2;
				}
				if (rotation == 1 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 2;
				}
				if (rotation == 1 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 1;
				}
				if (rotation == 2 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 2;
				}
				if (rotation == 2 && rotation3 == 3 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 3;
				}
				if (rotation == 2 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 3;
				}
				if (rotation == 2 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 2;
				}
				if (rotation == 3 && rotation3 == 0 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 0;
				}
				if (rotation == 3 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.None)
				{
					num3 = 3;
				}
				if (rotation == 3 && rotation3 == 2 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 3;
				}
				if (rotation == 3 && rotation3 == 1 && cornerType2 == StairsBlock.CornerType.ThreeQuarters)
				{
					num3 = 0;
				}
			}
			if (num3 >= 0)
			{
				int data5 = StairsBlock.SetRotation(StairsBlock.SetCornerType(data, StairsBlock.CornerType.ThreeQuarters), num3);
				int value4 = Terrain.ReplaceData(value, data5);
				base.SubsystemTerrain.ChangeCell(x, y, z, value4, updateModificationCounter);
			}
		}

		// Token: 0x04000688 RID: 1672
		public int[] m_handledBlocks;
	}
}
