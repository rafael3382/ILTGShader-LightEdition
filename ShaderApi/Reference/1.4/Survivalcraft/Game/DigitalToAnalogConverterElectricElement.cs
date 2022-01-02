using System;

namespace Game
{
	// Token: 0x0200026F RID: 623
	public class DigitalToAnalogConverterElectricElement : RotateableElectricElement
	{
		// Token: 0x06001408 RID: 5128 RVA: 0x00095A5B File Offset: 0x00093C5B
		public DigitalToAnalogConverterElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00095A65 File Offset: 0x00093C65
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00095A70 File Offset: 0x00093C70
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = 0f;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input && ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						if (connectorDirection.Value == ElectricConnectorDirection.Top)
						{
							this.m_voltage += 0.06666667f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Right)
						{
							this.m_voltage += 0.13333334f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
						{
							this.m_voltage += 0.266666681f;
						}
						if (connectorDirection.Value == ElectricConnectorDirection.Left)
						{
							this.m_voltage += 0.533333361f;
						}
					}
				}
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000CAE RID: 3246
		public float m_voltage;
	}
}
