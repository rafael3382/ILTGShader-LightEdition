using System;
using Engine;

namespace Game
{
	// Token: 0x020002CD RID: 717
	public class MultistateFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x060015BE RID: 5566 RVA: 0x000A3CD8 File Offset: 0x000A1ED8
		public MultistateFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x000A3CE4 File Offset: 0x000A1EE4
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				if (this.m_isActionAllowed)
				{
					if (this.m_lastActionTime != null)
					{
						double? num = base.SubsystemElectricity.SubsystemTime.GameTime - this.m_lastActionTime;
						double num2 = 0.1;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							return false;
						}
					}
					this.m_isActionAllowed = false;
					this.m_lastActionTime = new double?(base.SubsystemElectricity.SubsystemTime.GameTime);
					base.SubsystemElectricity.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true).SwitchToNextState(base.CellFaces[0].X, base.CellFaces[0].Y, base.CellFaces[0].Z, false);
				}
			}
			else
			{
				this.m_isActionAllowed = true;
			}
			return false;
		}

		// Token: 0x04000E4D RID: 3661
		public bool m_isActionAllowed;

		// Token: 0x04000E4E RID: 3662
		public double? m_lastActionTime;
	}
}
