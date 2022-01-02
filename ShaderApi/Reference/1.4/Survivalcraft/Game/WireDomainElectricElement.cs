using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200035B RID: 859
	public class WireDomainElectricElement : ElectricElement
	{
		// Token: 0x0600191B RID: 6427 RVA: 0x000C56E5 File Offset: 0x000C38E5
		public WireDomainElectricElement(SubsystemElectricity subsystemElectricity, IEnumerable<CellFace> cellFaces) : base(subsystemElectricity, cellFaces)
		{
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x000C56EF File Offset: 0x000C38EF
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x000C56F8 File Offset: 0x000C38F8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num |= (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
				}
			}
			this.m_voltage = (float)num / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x000C5798 File Offset: 0x000C3998
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (!(BlocksManager.Blocks[num] is WireBlock))
			{
				return;
			}
			int wireFacesBitmask = WireBlock.GetWireFacesBitmask(cellValue);
			int num2 = wireFacesBitmask;
			if (WireBlock.WireExistsOnFace(cellValue, cellFace.Face))
			{
				Point3 point = CellFace.FaceToPoint3(cellFace.Face);
				int cellValue2 = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X - point.X, cellFace.Y - point.Y, cellFace.Z - point.Z);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
				if (!block.IsCollidable_(cellValue2) || block.IsTransparent_(cellValue2))
				{
					num2 &= ~(1 << cellFace.Face);
				}
			}
			if (num2 == 0)
			{
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
				return;
			}
			if (num2 != wireFacesBitmask)
			{
				int newValue = WireBlock.SetWireFacesBitmask(cellValue, num2);
				base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, newValue, false, false);
			}
		}

		// Token: 0x04001134 RID: 4404
		public float m_voltage;
	}
}
