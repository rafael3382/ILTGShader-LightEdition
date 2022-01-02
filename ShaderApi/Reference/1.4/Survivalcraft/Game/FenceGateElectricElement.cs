using System;

namespace Game
{
	// Token: 0x0200028C RID: 652
	public class FenceGateElectricElement : ElectricElement
	{
		// Token: 0x0600148B RID: 5259 RVA: 0x00099A5D File Offset: 0x00097C5D
		public FenceGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00099A80 File Offset: 0x00097C80
		public override bool Simulate()
		{
			int num = base.SubsystemElectricity.CircuitStep - this.m_lastChangeCircuitStep;
			float voltage = (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
			if (ElectricElement.IsSignalHigh(voltage) != ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			}
			this.m_voltage = voltage;
			if (!ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_needsReset = false;
			}
			if (!this.m_needsReset)
			{
				if (num >= 10)
				{
					if (ElectricElement.IsSignalHigh(this.m_voltage))
					{
						CellFace cellFace = base.CellFaces[0];
						int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemFenceGateBlockBehavior>(true).OpenCloseGate(cellFace.X, cellFace.Y, cellFace.Z, !FenceGateBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000D58 RID: 3416
		public int m_lastChangeCircuitStep;

		// Token: 0x04000D59 RID: 3417
		public bool m_needsReset;

		// Token: 0x04000D5A RID: 3418
		public float m_voltage;
	}
}
