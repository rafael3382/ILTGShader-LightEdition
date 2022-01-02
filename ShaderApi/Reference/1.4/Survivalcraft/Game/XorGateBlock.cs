using System;

namespace Game
{
	// Token: 0x02000130 RID: 304
	public class XorGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060005BA RID: 1466 RVA: 0x00020837 File Offset: 0x0001EA37
		public XorGateBlock() : base("Models/Gates", "XorGate", 0.375f)
		{
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x0002084E File Offset: 0x0001EA4E
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new XorGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00020868 File Offset: 0x0001EA68
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

		// Token: 0x04000288 RID: 648
		public const int Index = 156;
	}
}
