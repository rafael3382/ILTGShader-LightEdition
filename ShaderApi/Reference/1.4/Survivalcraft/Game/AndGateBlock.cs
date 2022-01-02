using System;

namespace Game
{
	// Token: 0x02000010 RID: 16
	public class AndGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00007936 File Offset: 0x00005B36
		public AndGateBlock() : base("Models/Gates", "AndGate", 0.5f)
		{
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000794D File Offset: 0x00005B4D
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new AndGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007968 File Offset: 0x00005B68
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Right;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.Left;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						electricConnectorDirection = connectorDirection;
						electricConnectorDirection2 = ElectricConnectorDirection.Top;
						if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.In;
							if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
							{
								goto IL_97;
							}
						}
						return new ElectricConnectorType?(ElectricConnectorType.Output);
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_97:
			return null;
		}

		// Token: 0x0400005A RID: 90
		public const int Index = 137;
	}
}
