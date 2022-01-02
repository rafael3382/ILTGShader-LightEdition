using System;
using Engine;

namespace Game
{
	// Token: 0x020002D4 RID: 724
	public class OrGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060015DB RID: 5595 RVA: 0x000A4918 File Offset: 0x000A2B18
		public OrGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x000A4922 File Offset: 0x000A2B22
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x000A492C File Offset: 0x000A2B2C
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

		// Token: 0x04000E5F RID: 3679
		public float m_voltage;
	}
}
