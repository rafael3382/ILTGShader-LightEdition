using System;

namespace Game
{
	// Token: 0x0200030F RID: 783
	public class SpikedPlankElectricElement : MountedElectricElement
	{
		// Token: 0x060016F5 RID: 5877 RVA: 0x000AD038 File Offset: 0x000AB238
		public SpikedPlankElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x000AD05C File Offset: 0x000AB25C
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemSpikesBlockBehavior>(true).RetractExtendSpikes(cellFace.X, cellFace.Y, cellFace.Z, !SpikedPlankBlock.GetSpikesState(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000FB2 RID: 4018
		public int m_lastChangeCircuitStep;

		// Token: 0x04000FB3 RID: 4019
		public bool m_needsReset;

		// Token: 0x04000FB4 RID: 4020
		public float m_voltage;
	}
}
