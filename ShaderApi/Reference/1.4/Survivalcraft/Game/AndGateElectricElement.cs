using System;
using Engine;

namespace Game
{
	// Token: 0x0200023F RID: 575
	public class AndGateElectricElement : RotateableElectricElement
	{
		// Token: 0x060012AF RID: 4783 RVA: 0x0008AE98 File Offset: 0x00089098
		public AndGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x0008AEA2 File Offset: 0x000890A2
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x0008AEAC File Offset: 0x000890AC
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
			this.m_voltage = ((num == 2) ? ((float)num2 / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x04000B80 RID: 2944
		public float m_voltage;
	}
}
