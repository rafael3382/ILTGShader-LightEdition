using System;
using Engine;

namespace Game
{
	// Token: 0x020002D0 RID: 720
	public class NotGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060015C6 RID: 5574 RVA: 0x000A3F7C File Offset: 0x000A217C
		public NotGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x000A3F86 File Offset: 0x000A2186
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000A3F90 File Offset: 0x000A2190
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					break;
				}
			}
			this.m_voltage = (float)(~(float)num & 15) / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000E51 RID: 3665
		public float m_voltage;
	}
}
