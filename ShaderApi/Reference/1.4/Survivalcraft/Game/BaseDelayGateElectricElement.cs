using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000241 RID: 577
	public abstract class BaseDelayGateElectricElement : RotateableElectricElement
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x060012C5 RID: 4805
		public abstract int DelaySteps { get; }

		// Token: 0x060012C6 RID: 4806 RVA: 0x0008B537 File Offset: 0x00089737
		public BaseDelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x0008B54C File Offset: 0x0008974C
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0008B554 File Offset: 0x00089754
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int delaySteps = this.DelaySteps;
			float num = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace);
					break;
				}
			}
			if (delaySteps > 0)
			{
				float voltage2;
				if (this.m_voltagesHistory.TryGetValue(base.SubsystemElectricity.CircuitStep, out voltage2))
				{
					this.m_voltage = voltage2;
					this.m_voltagesHistory.Remove(base.SubsystemElectricity.CircuitStep);
				}
				if (num != this.m_lastStoredVoltage)
				{
					this.m_lastStoredVoltage = num;
					if (this.m_voltagesHistory.Count < 300)
					{
						this.m_voltagesHistory[base.SubsystemElectricity.CircuitStep + this.DelaySteps] = num;
						base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + this.DelaySteps);
					}
				}
			}
			else
			{
				this.m_voltage = num;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000B8A RID: 2954
		public float m_voltage;

		// Token: 0x04000B8B RID: 2955
		public float m_lastStoredVoltage;

		// Token: 0x04000B8C RID: 2956
		public Dictionary<int, float> m_voltagesHistory = new Dictionary<int, float>();
	}
}
