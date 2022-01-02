using System;
using Engine;

namespace Game
{
	// Token: 0x02000262 RID: 610
	public class CounterElectricElement : RotateableElectricElement
	{
		// Token: 0x060013E7 RID: 5095 RVA: 0x000949D0 File Offset: 0x00092BD0
		public CounterElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			float? num = subsystemElectricity.ReadPersistentVoltage(cellFace.Point);
			if (num != null)
			{
				this.m_counter = (int)MathUtils.Round(MathUtils.Abs(num.Value) * 15f);
				this.m_overflow = (num.Value < 0f);
			}
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x00094A44 File Offset: 0x00092C44
		public override float GetOutputVoltage(int face)
		{
			ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, base.Rotation, face);
			if (connectorDirection != null)
			{
				if (connectorDirection.Value == ElectricConnectorDirection.Top)
				{
					return (float)this.m_counter / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
				{
					return (float)(this.m_overflow ? 1 : 0);
				}
			}
			return 0f;
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x00094AB0 File Offset: 0x00092CB0
		public override bool Simulate()
		{
			int counter = this.m_counter;
			bool overflow = this.m_overflow;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
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
							flag = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
							{
								flag2 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
							}
							else
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.In;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									flag3 = ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
								}
							}
						}
					}
				}
			}
			if (flag && this.m_plusAllowed)
			{
				this.m_plusAllowed = false;
				if (this.m_counter < 15)
				{
					this.m_counter++;
					this.m_overflow = false;
				}
				else
				{
					this.m_counter = 0;
					this.m_overflow = true;
				}
			}
			else if (flag2 && this.m_minusAllowed)
			{
				this.m_minusAllowed = false;
				if (this.m_counter > 0)
				{
					this.m_counter--;
					this.m_overflow = false;
				}
				else
				{
					this.m_counter = 15;
					this.m_overflow = true;
				}
			}
			else if (flag3 && this.m_resetAllowed)
			{
				this.m_counter = 0;
				this.m_overflow = false;
			}
			if (!flag)
			{
				this.m_plusAllowed = true;
			}
			if (!flag2)
			{
				this.m_minusAllowed = true;
			}
			if (!flag3)
			{
				this.m_resetAllowed = true;
			}
			if (this.m_counter != counter || this.m_overflow != overflow)
			{
				base.SubsystemElectricity.WritePersistentVoltage(base.CellFaces[0].Point, (float)this.m_counter / 15f * (float)((!this.m_overflow) ? 1 : -1));
				return true;
			}
			return false;
		}

		// Token: 0x04000C84 RID: 3204
		public bool m_plusAllowed = true;

		// Token: 0x04000C85 RID: 3205
		public bool m_minusAllowed = true;

		// Token: 0x04000C86 RID: 3206
		public bool m_resetAllowed = true;

		// Token: 0x04000C87 RID: 3207
		public int m_counter;

		// Token: 0x04000C88 RID: 3208
		public bool m_overflow;
	}
}
