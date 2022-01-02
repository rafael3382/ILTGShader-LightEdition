using System;
using Engine;

namespace Game
{
	// Token: 0x020002BC RID: 700
	public class LightBulbElectricElement : MountedElectricElement
	{
		// Token: 0x0600157B RID: 5499 RVA: 0x000A1DF8 File Offset: 0x0009FFF8
		public LightBulbElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			int data = Terrain.ExtractData(value);
			this.m_intensity = LightbulbBlock.GetLightIntensity(data);
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x000A1E34 File Offset: 0x000A0034
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float num2 = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num2 = MathUtils.Max(num2, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			int intensity = this.m_intensity;
			this.m_intensity = MathUtils.Clamp((int)MathUtils.Round((num2 - 0.5f) * 30f), 0, 15);
			if (this.m_intensity != intensity)
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			if (num >= 10)
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int data = LightbulbBlock.SetLightIntensity(Terrain.ExtractData(cellValue), this.m_intensity);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			}
			else
			{
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
			}
			return false;
		}

		// Token: 0x04000E00 RID: 3584
		public int m_intensity;

		// Token: 0x04000E01 RID: 3585
		public int m_lastChangeCircuitStep;
	}
}
