using System;

namespace Game
{
	// Token: 0x020000BF RID: 191
	public class OrGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060003B9 RID: 953 RVA: 0x000161D7 File Offset: 0x000143D7
		public OrGateBlock() : base("Models/Gates", "OrGate", 0.375f)
		{
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000161EE File Offset: 0x000143EE
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new OrGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00016208 File Offset: 0x00014408
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

		// Token: 0x040001B4 RID: 436
		public const int Index = 143;
	}
}
