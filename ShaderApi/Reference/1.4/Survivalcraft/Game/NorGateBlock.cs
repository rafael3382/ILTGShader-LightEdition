using System;

namespace Game
{
	// Token: 0x020000BA RID: 186
	public class NorGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x00015C8A File Offset: 0x00013E8A
		public NorGateBlock() : base("Models/Gates", "NorGate", 0.375f)
		{
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00015CA1 File Offset: 0x00013EA1
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new NorGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00015CBC File Offset: 0x00013EBC
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

		// Token: 0x040001AC RID: 428
		public const int Index = 135;
	}
}
