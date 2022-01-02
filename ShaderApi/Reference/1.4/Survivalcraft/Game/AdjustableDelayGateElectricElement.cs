using System;

namespace Game
{
	// Token: 0x0200023C RID: 572
	public class AdjustableDelayGateElectricElement : BaseDelayGateElectricElement
	{
		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x0008ACC2 File Offset: 0x00088EC2
		public override int DelaySteps
		{
			get
			{
				return this.m_delaySteps;
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x0008ACCC File Offset: 0x00088ECC
		public AdjustableDelayGateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			int data = Terrain.ExtractData(subsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			this.m_delaySteps = AdjustableDelayGateBlock.GetDelay(data);
		}

		// Token: 0x04000B7A RID: 2938
		public int m_delaySteps;
	}
}
