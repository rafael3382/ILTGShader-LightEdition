using System;

namespace Game
{
	// Token: 0x020000B9 RID: 185
	public class NandGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060003A0 RID: 928 RVA: 0x00015BA9 File Offset: 0x00013DA9
		public NandGateBlock() : base("Models/Gates", "NandGate", 0.5f)
		{
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00015BC0 File Offset: 0x00013DC0
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new NandGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00015BDC File Offset: 0x00013DDC
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

		// Token: 0x040001AB RID: 427
		public const int Index = 134;
	}
}
