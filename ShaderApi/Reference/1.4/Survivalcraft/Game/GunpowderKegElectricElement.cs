using System;

namespace Game
{
	// Token: 0x020002A5 RID: 677
	public class GunpowderKegElectricElement : ElectricElement
	{
		// Token: 0x0600151D RID: 5405 RVA: 0x000A0CF4 File Offset: 0x0009EEF4
		public GunpowderKegElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x000A0D00 File Offset: 0x0009EF00
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.Project.FindSubsystem<SubsystemExplosivesBlockBehavior>(true).IgniteFuse(cellFace.X, cellFace.Y, cellFace.Z);
			}
			return false;
		}
	}
}
