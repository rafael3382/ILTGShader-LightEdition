using System;
using Engine;

namespace Game
{
	// Token: 0x020001B5 RID: 437
	public class SubsystemLadderBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000B3A RID: 2874 RVA: 0x0004B6AD File Offset: 0x000498AD
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					59,
					213
				};
			}
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0004B6C4 File Offset: 0x000498C4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int face = LadderBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			Point3 point = CellFace.FaceToPoint3(face);
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num].IsFaceTransparent(base.SubsystemTerrain, face, cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
