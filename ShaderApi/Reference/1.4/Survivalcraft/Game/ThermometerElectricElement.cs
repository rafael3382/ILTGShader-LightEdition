using System;
using Engine;

namespace Game
{
	// Token: 0x0200032C RID: 812
	public class ThermometerElectricElement : ElectricElement
	{
		// Token: 0x06001840 RID: 6208 RVA: 0x000BFBC0 File Offset: 0x000BDDC0
		public ThermometerElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemMetersBlockBehavior = base.SubsystemElectricity.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true);
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x000BFBE1 File Offset: 0x000BDDE1
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x000BFBEC File Offset: 0x000BDDEC
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			CellFace cellFace = base.CellFaces[0];
			this.m_voltage = MathUtils.Saturate((float)this.m_subsystemMetersBlockBehavior.GetThermometerReading(cellFace.X, cellFace.Y, cellFace.Z) / 15f);
			float num = 0.5f * (0.9f + 0.000200000009f * (float)(this.GetHashCode() % 1000));
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(num / 0.01f), 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x040010ED RID: 4333
		public SubsystemMetersBlockBehavior m_subsystemMetersBlockBehavior;

		// Token: 0x040010EE RID: 4334
		public float m_voltage;

		// Token: 0x040010EF RID: 4335
		public const float m_pollingPeriod = 0.5f;
	}
}
