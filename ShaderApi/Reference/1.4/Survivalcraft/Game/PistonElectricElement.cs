using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020002DD RID: 733
	public class PistonElectricElement : ElectricElement
	{
		// Token: 0x060015F4 RID: 5620 RVA: 0x000A54C0 File Offset: 0x000A36C0
		public PistonElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x000A5594 File Offset: 0x000A3794
		public override bool Simulate()
		{
			float num = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = MathUtils.Max(num, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			int num2 = MathUtils.Max((int)(num * 15.999f) - 7, 0);
			if (num2 != this.m_lastLength)
			{
				this.m_lastLength = num2;
				base.SubsystemElectricity.Project.FindSubsystem<SubsystemPistonBlockBehavior>(true).AdjustPiston(base.CellFaces[0].Point, num2);
			}
			return false;
		}

		// Token: 0x04000E84 RID: 3716
		public int m_lastLength = -1;
	}
}
