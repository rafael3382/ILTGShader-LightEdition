using System;
using Engine;

namespace Game
{
	// Token: 0x020002F6 RID: 758
	public class RealTimeClockElectricElement : RotateableElectricElement
	{
		// Token: 0x06001699 RID: 5785 RVA: 0x000A9ECC File Offset: 0x000A80CC
		public RealTimeClockElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemTimeOfDay = base.SubsystemElectricity.Project.FindSubsystem<SubsystemTimeOfDay>(true);
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x000A9EF4 File Offset: 0x000A80F4
		public override float GetOutputVoltage(int face)
		{
			ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, base.Rotation, face);
			if (connectorDirection != null)
			{
				if (connectorDirection.Value == ElectricConnectorDirection.Top)
				{
					return (float)(this.GetClockValue() & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Right)
				{
					return (float)(this.GetClockValue() >> 4 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Bottom)
				{
					return (float)(this.GetClockValue() >> 8 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.Left)
				{
					return (float)(this.GetClockValue() >> 12 & 15) / 15f;
				}
				if (connectorDirection.Value == ElectricConnectorDirection.In)
				{
					return (float)(this.GetClockValue() >> 16 & 15) / 15f;
				}
			}
			return 0f;
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x000A9FC4 File Offset: 0x000A81C4
		public override bool Simulate()
		{
			double day = this.m_subsystemTimeOfDay.Day;
			int num = (int)(((MathUtils.Ceiling(day * 4096.0) + 0.5) / 4096.0 - day) * 1200.0 / 0.0099999997764825821);
			int circuitStep = MathUtils.Max(base.SubsystemElectricity.FrameStartCircuitStep + num, base.SubsystemElectricity.CircuitStep + 1);
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, circuitStep);
			int clockValue = this.GetClockValue();
			if (clockValue != this.m_lastClockValue)
			{
				this.m_lastClockValue = clockValue;
				return true;
			}
			return false;
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x000AA060 File Offset: 0x000A8260
		public int GetClockValue()
		{
			return (int)(this.m_subsystemTimeOfDay.Day * 4096.0);
		}

		// Token: 0x04000F53 RID: 3923
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000F54 RID: 3924
		public int m_lastClockValue = -1;

		// Token: 0x04000F55 RID: 3925
		public const int m_periodsPerDay = 4096;
	}
}
