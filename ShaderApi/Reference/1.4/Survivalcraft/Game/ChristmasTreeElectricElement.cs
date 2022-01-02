using System;

namespace Game
{
	// Token: 0x0200025C RID: 604
	public class ChristmasTreeElectricElement : ElectricElement
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0009451F File Offset: 0x0009271F
		public ChristmasTreeElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_voltage = (float)(ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)) ? 1 : 0);
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x00094554 File Offset: 0x00092754
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float voltage = (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
			if (ElectricElement.IsSignalHigh(voltage) != ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			this.m_voltage = voltage;
			if (num >= 10)
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int data = ChristmasTreeBlock.SetLightState(Terrain.ExtractData(cellValue), ElectricElement.IsSignalHigh(this.m_voltage));
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			}
			else
			{
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
			}
			return false;
		}

		// Token: 0x04000C5D RID: 3165
		public int m_lastChangeCircuitStep;

		// Token: 0x04000C5E RID: 3166
		public float m_voltage;
	}
}
