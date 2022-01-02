using System;

namespace Game
{
	// Token: 0x020002A9 RID: 681
	public class HygrometerElectricElement : ElectricElement
	{
		// Token: 0x06001525 RID: 5413 RVA: 0x000A1400 File Offset: 0x0009F600
		public HygrometerElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x000A140A File Offset: 0x0009F60A
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x000A1414 File Offset: 0x0009F614
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			CellFace cellFace = base.CellFaces[0];
			int humidity = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetHumidity(cellFace.X, cellFace.Z);
			this.m_voltage = (float)humidity / 15f;
			return this.m_voltage != voltage;
		}

		// Token: 0x04000DE4 RID: 3556
		public float m_voltage;
	}
}
