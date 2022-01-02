using System;

namespace Game
{
	// Token: 0x02000272 RID: 626
	public class DoorElectricElement : ElectricElement
	{
		// Token: 0x0600141D RID: 5149 RVA: 0x0009605E File Offset: 0x0009425E
		public DoorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_lastChangeCircuitStep = base.SubsystemElectricity.CircuitStep;
			this.m_needsReset = true;
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x00096080 File Offset: 0x00094280
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
						base.SubsystemElectricity.Project.FindSubsystem<SubsystemDoorBlockBehavior>(true).OpenCloseDoor(cellFace.X, cellFace.Y, cellFace.Z, !DoorBlock.GetOpen(data));
					}
				}
				else
				{
					base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10 - num);
				}
			}
			return false;
		}

		// Token: 0x04000CB4 RID: 3252
		public int m_lastChangeCircuitStep;

		// Token: 0x04000CB5 RID: 3253
		public bool m_needsReset;

		// Token: 0x04000CB6 RID: 3254
		public float m_voltage;
	}
}
