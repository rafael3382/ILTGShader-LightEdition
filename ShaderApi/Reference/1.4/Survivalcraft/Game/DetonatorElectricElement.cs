using System;

namespace Game
{
	// Token: 0x0200026D RID: 621
	public class DetonatorElectricElement : MountedElectricElement
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x000959C4 File Offset: 0x00093BC4
		public DetonatorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x000959D0 File Offset: 0x00093BD0
		public void Detonate()
		{
			CellFace cellFace = base.CellFaces[0];
			int value = Terrain.MakeBlockValue(147);
			base.SubsystemElectricity.Project.FindSubsystem<SubsystemExplosions>(true).TryExplodeBlock(cellFace.X, cellFace.Y, cellFace.Z, value);
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x00095A22 File Offset: 0x00093C22
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				this.Detonate();
			}
			return false;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x00095A34 File Offset: 0x00093C34
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Detonate();
		}
	}
}
