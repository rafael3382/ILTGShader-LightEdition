using System;
using Engine;

namespace Game
{
	// Token: 0x020002C3 RID: 707
	public class MemoryBankElectricElement : RotateableElectricElement
	{
		// Token: 0x06001596 RID: 5526 RVA: 0x000A2B20 File Offset: 0x000A0D20
		public MemoryBankElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemMemoryBankBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemMemoryBankBlockBehavior>(true);
			MemoryBankData blockData = this.m_subsystemMemoryBankBlockBehavior.GetBlockData(cellFace.Point);
			if (blockData != null)
			{
				this.m_voltage = (float)blockData.LastOutput / 15f;
			}
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000A2B70 File Offset: 0x000A0D70
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x000A2B78 File Offset: 0x000A0D78
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
						ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Right;
						if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
						{
							num2 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								num3 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									int num4 = (int)MathUtils.Round(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace) * 15f);
									flag = (num4 >= 8);
									flag3 = (num4 > 0 && num4 < 8);
									flag2 = true;
								}
								else
								{
									electricConnectorDirection = connectorDirection;
									electricConnectorDirection2 = ElectricConnectorDirection.In;
									if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
									{
										num = electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace);
									}
								}
							}
						}
					}
				}
			}
			MemoryBankData memoryBankData = this.m_subsystemMemoryBankBlockBehavior.GetBlockData(base.CellFaces[0].Point);
			int address = num2 + (num3 << 4);
			if (flag2)
			{
				if (flag && this.m_clockAllowed)
				{
					this.m_clockAllowed = false;
					this.m_voltage = ((memoryBankData != null) ? ((float)memoryBankData.Read(address) / 15f) : 0f);
				}
				else if (flag3 && this.m_writeAllowed)
				{
					this.m_writeAllowed = false;
					if (memoryBankData == null)
					{
						memoryBankData = new MemoryBankData();
						this.m_subsystemMemoryBankBlockBehavior.SetBlockData(base.CellFaces[0].Point, memoryBankData);
					}
					memoryBankData.Write(address, (byte)MathUtils.Round(num * 15f));
				}
			}
			else
			{
				this.m_voltage = ((memoryBankData != null) ? ((float)memoryBankData.Read(address) / 15f) : 0f);
			}
			if (!flag)
			{
				this.m_clockAllowed = true;
			}
			if (!flag3)
			{
				this.m_writeAllowed = true;
			}
			if (memoryBankData != null)
			{
				memoryBankData.LastOutput = (byte)MathUtils.Round(this.m_voltage * 15f);
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x04000E13 RID: 3603
		public SubsystemMemoryBankBlockBehavior m_subsystemMemoryBankBlockBehavior;

		// Token: 0x04000E14 RID: 3604
		public float m_voltage;

		// Token: 0x04000E15 RID: 3605
		public bool m_writeAllowed;

		// Token: 0x04000E16 RID: 3606
		public bool m_clockAllowed;
	}
}
