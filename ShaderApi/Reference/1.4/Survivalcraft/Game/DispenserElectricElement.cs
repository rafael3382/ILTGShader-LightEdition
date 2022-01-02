using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000271 RID: 625
	public class DispenserElectricElement : ElectricElement
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x00095E38 File Offset: 0x00094038
		public DispenserElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
			this.m_subsystemBlockEntities = base.SubsystemElectricity.Project.FindSubsystem<SubsystemBlockEntities>(true);
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x00095F24 File Offset: 0x00094124
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				if (this.m_isDispenseAllowed)
				{
					if (this.m_lastDispenseTime != null)
					{
						double? num = base.SubsystemElectricity.SubsystemTime.GameTime - this.m_lastDispenseTime;
						double num2 = 0.1;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							return false;
						}
					}
					this.m_isDispenseAllowed = false;
					this.m_lastDispenseTime = new double?(base.SubsystemElectricity.SubsystemTime.GameTime);
					ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(base.CellFaces[0].Point.X, base.CellFaces[0].Point.Y, base.CellFaces[0].Point.Z);
					if (blockEntity != null)
					{
						ComponentDispenser componentDispenser = blockEntity.Entity.FindComponent<ComponentDispenser>();
						if (componentDispenser != null)
						{
							componentDispenser.Dispense();
						}
					}
				}
			}
			else
			{
				this.m_isDispenseAllowed = true;
			}
			return false;
		}

		// Token: 0x04000CB1 RID: 3249
		public bool m_isDispenseAllowed = true;

		// Token: 0x04000CB2 RID: 3250
		public double? m_lastDispenseTime;

		// Token: 0x04000CB3 RID: 3251
		public SubsystemBlockEntities m_subsystemBlockEntities;
	}
}
