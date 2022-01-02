using System;
using Engine;

namespace Game
{
	// Token: 0x0200001F RID: 31
	public abstract class BottomSuckerBlock : WaterBlock
	{
		// Token: 0x06000103 RID: 259 RVA: 0x000092AC File Offset: 0x000074AC
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Point3 point = raycastResult.CellFace.Point + CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int cellValue = subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			Block block = BlocksManager.Blocks[num];
			int face = Time.FrameIndex % 4;
			if (block is WaterBlock)
			{
				return new BlockPlacementData
				{
					CellFace = raycastResult.CellFace,
					Value = Terrain.MakeBlockValue(this.BlockIndex, 0, BottomSuckerBlock.SetSubvariant(BottomSuckerBlock.SetFace(data, raycastResult.CellFace.Face), face))
				};
			}
			return default(BlockPlacementData);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000936A File Offset: 0x0000756A
		public static int GetFace(int data)
		{
			return data >> 8 & 7;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00009371 File Offset: 0x00007571
		public static int SetFace(int data, int face)
		{
			return (data & -1793) | (face & 7) << 8;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00009380 File Offset: 0x00007580
		public static int GetSubvariant(int data)
		{
			return data >> 11 & 3;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00009388 File Offset: 0x00007588
		public static int SetSubvariant(int data, int face)
		{
			return (data & -6145) | (face & 3) << 11;
		}
	}
}
