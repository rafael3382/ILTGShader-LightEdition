using System;
using Engine;

namespace Game
{
	// Token: 0x02000360 RID: 864
	public class XorGateElectricElement : RotateableElectricElement
	{
		// Token: 0x0600192C RID: 6444 RVA: 0x000C626A File Offset: 0x000C446A
		public XorGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x000C6274 File Offset: 0x000C4474
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x000C627C File Offset: 0x000C447C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int? num = null;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					int num2 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
					num = ((num == null) ? new int?(num2) : (num ^= num2));
				}
			}
			this.m_voltage = ((num != null) ? ((float)num.Value / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x0400115E RID: 4446
		public float m_voltage;
	}
}
