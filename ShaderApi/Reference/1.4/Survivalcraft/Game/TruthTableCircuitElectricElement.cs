using System;

namespace Game
{
	// Token: 0x02000334 RID: 820
	public class TruthTableCircuitElectricElement : RotateableElectricElement
	{
		// Token: 0x06001858 RID: 6232 RVA: 0x000C0194 File Offset: 0x000BE394
		public TruthTableCircuitElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemTruthTableCircuitBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemTruthTableCircuitBlockBehavior>(true);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x000C01B0 File Offset: 0x000BE3B0
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x000C01B8 File Offset: 0x000BE3B8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			int num = 0;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
						ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Top;
						if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
						{
							if (ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
							{
								num |= 1;
							}
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Right;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								if (ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
								{
									num |= 2;
								}
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									if (ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
									{
										num |= 4;
									}
								}
								else
								{
									electricConnectorDirection = connectorDirection;
									electricConnectorDirection2 = ElectricConnectorDirection.Left;
									if ((electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null) && ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
									{
										num |= 8;
									}
								}
							}
						}
					}
				}
			}
			TruthTableData blockData = this.m_subsystemTruthTableCircuitBlockBehavior.GetBlockData(base.CellFaces[0].Point);
			this.m_voltage = ((blockData != null) ? ((float)blockData.Data[num] / 15f) : 0f);
			return this.m_voltage != voltage;
		}

		// Token: 0x0400110B RID: 4363
		public SubsystemTruthTableCircuitBlockBehavior m_subsystemTruthTableCircuitBlockBehavior;

		// Token: 0x0400110C RID: 4364
		public float m_voltage;
	}
}
