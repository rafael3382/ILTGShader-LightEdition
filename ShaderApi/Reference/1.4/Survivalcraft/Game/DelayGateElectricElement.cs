using System;

namespace Game
{
	// Token: 0x0200026C RID: 620
	public class DelayGateElectricElement : BaseDelayGateElectricElement
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x000958B4 File Offset: 0x00093AB4
		public override int DelaySteps
		{
			get
			{
				if (base.SubsystemElectricity.CircuitStep - this.m_lastDelayCalculationStep > 50)
				{
					this.m_delaySteps = null;
				}
				if (this.m_delaySteps == null)
				{
					int num = 0;
					DelayGateElectricElement.CountDelayPredecessors(this, ref num);
					this.m_delaySteps = new int?(DelayGateElectricElement.m_delaysByPredecessorsCount[num]);
					this.m_lastDelayCalculationStep = base.SubsystemElectricity.CircuitStep;
				}
				return this.m_delaySteps.Value;
			}
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x00095928 File Offset: 0x00093B28
		public DelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x00095934 File Offset: 0x00093B34
		public static void CountDelayPredecessors(DelayGateElectricElement delayGate, ref int count)
		{
			if (count < 2)
			{
				foreach (ElectricConnection electricConnection in delayGate.Connections)
				{
					if (electricConnection.ConnectorType == ElectricConnectorType.Input)
					{
						DelayGateElectricElement delayGateElectricElement = electricConnection.NeighborElectricElement as DelayGateElectricElement;
						if (delayGateElectricElement != null)
						{
							count++;
							DelayGateElectricElement.CountDelayPredecessors(delayGateElectricElement, ref count);
							break;
						}
					}
				}
			}
		}

		// Token: 0x04000CAB RID: 3243
		public int? m_delaySteps;

		// Token: 0x04000CAC RID: 3244
		public int m_lastDelayCalculationStep;

		// Token: 0x04000CAD RID: 3245
		public static int[] m_delaysByPredecessorsCount = new int[]
		{
			20,
			80,
			400
		};
	}
}
