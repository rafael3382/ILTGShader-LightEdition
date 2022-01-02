using System;
using Engine;

namespace Game
{
	// Token: 0x02000243 RID: 579
	public class BatteryElectricElement : ElectricElement
	{
		// Token: 0x060012DA RID: 4826 RVA: 0x0008BCC5 File Offset: 0x00089EC5
		public BatteryElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0008BCD0 File Offset: 0x00089ED0
		public override float GetOutputVoltage(int face)
		{
			Point3 point = base.CellFaces[0].Point;
			return (float)BatteryBlock.GetVoltageLevel(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z))) / 15f;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0008BD30 File Offset: 0x00089F30
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y - 1, cellFace.Z);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			if (!block.IsCollidable_(cellValue) || block.IsTransparent_(cellValue))
			{
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
			}
		}
	}
}
