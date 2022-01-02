using System;
using Engine;

namespace Game
{
	// Token: 0x020002CA RID: 714
	public abstract class MountedElectricElement : ElectricElement
	{
		// Token: 0x060015B8 RID: 5560 RVA: 0x000A3992 File Offset: 0x000A1B92
		public MountedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x000A399C File Offset: 0x000A1B9C
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			Point3 point = CellFace.FaceToPoint3(cellFace.Face);
			int x = cellFace.X - point.X;
			int y = cellFace.Y - point.Y;
			int z = cellFace.Z - point.Z;
			if (base.SubsystemElectricity.SubsystemTerrain.Terrain.IsCellValid(x, y, z))
			{
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
				if ((!block.IsCollidable_(cellValue) || block.IsFaceTransparent(base.SubsystemElectricity.SubsystemTerrain, cellFace.Face, cellValue)) && (cellFace.Face != 4 || !(block is FenceBlock)))
				{
					base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
				}
			}
		}
	}
}
