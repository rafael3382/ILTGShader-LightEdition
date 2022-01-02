using System;
using Engine;

namespace Game
{
	// Token: 0x020002CE RID: 718
	public class NandGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060015C0 RID: 5568 RVA: 0x000A3DF4 File Offset: 0x000A1FF4
		public NandGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x000A3DFE File Offset: 0x000A1FFE
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x000A3E08 File Offset: 0x000A2008
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			int num2 = 15;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num2 &= (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					num++;
				}
			}
			this.m_voltage = ((num == 2) ? ((float)(~(float)num2 & 15) / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x04000E4F RID: 3663
		public float m_voltage;
	}
}
