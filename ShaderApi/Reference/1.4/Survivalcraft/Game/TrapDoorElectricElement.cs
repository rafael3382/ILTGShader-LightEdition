using System;

namespace Game
{
	// Token: 0x02000332 RID: 818
	public class TrapDoorElectricElement : ElectricElement
	{
		// Token: 0x06001856 RID: 6230 RVA: 0x000C0052 File Offset: 0x000BE252
		public TrapDoorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x000C0074 File Offset: 0x000BE274
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemTrapdoorBlockBehavior>(true).OpenCloseTrapdoor(cellFace.X, cellFace.Y, cellFace.Z, !TrapdoorBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04001102 RID: 4354
		public int m_lastChangeCircuitStep;

		// Token: 0x04001103 RID: 4355
		public bool m_needsReset;

		// Token: 0x04001104 RID: 4356
		public float m_voltage;
	}
}
