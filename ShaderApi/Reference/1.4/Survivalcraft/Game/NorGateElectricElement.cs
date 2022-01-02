using System;
using Engine;

namespace Game
{
	// Token: 0x020002CF RID: 719
	public class NorGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060015C3 RID: 5571 RVA: 0x000A3EC4 File Offset: 0x000A20C4
		public NorGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x000A3ECE File Offset: 0x000A20CE
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x000A3ED8 File Offset: 0x000A20D8
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
			this.m_voltage = (float)(~(float)num & 15) / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000E50 RID: 3664
		public float m_voltage;
	}
}
