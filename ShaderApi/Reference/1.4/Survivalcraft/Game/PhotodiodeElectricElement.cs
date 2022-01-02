using System;
using Engine;

namespace Game
{
	// Token: 0x020002DB RID: 731
	public class PhotodiodeElectricElement : MountedElectricElement
	{
		// Token: 0x060015EF RID: 5615 RVA: 0x000A53A8 File Offset: 0x000A35A8
		public PhotodiodeElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_voltage = this.CalculateVoltage();
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x000A53BE File Offset: 0x000A35BE
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x000A53C8 File Offset: 0x000A35C8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateVoltage();
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max(50, 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x000A5414 File Offset: 0x000A3614
		public float CalculateVoltage()
		{
			CellFace cellFace = base.CellFaces[0];
			Point3 point = CellFace.FaceToPoint3(cellFace.Face);
			int cellLight = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellLight(cellFace.X, cellFace.Y, cellFace.Z);
			int cellLight2 = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellLight(cellFace.X + point.X, cellFace.Y + point.Y, cellFace.Z + point.Z);
			return (float)MathUtils.Max(cellLight, cellLight2) / 15f;
		}

		// Token: 0x04000E7F RID: 3711
		public float m_voltage;
	}
}
