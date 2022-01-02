using System;

namespace Game
{
	// Token: 0x02000059 RID: 89
	public class DigitalToAnalogConverterBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060001BA RID: 442 RVA: 0x0000C129 File Offset: 0x0000A329
		public DigitalToAnalogConverterBlock() : base("Models/Gates", "DigitalToAnalogConverter", 0.375f)
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000C140 File Offset: 0x0000A340
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DigitalToAnalogConverterElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000C15C File Offset: 0x0000A35C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.In;
				if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
				{
					return new ElectricConnectorType?(ElectricConnectorType.Output);
				}
				electricConnectorDirection = connectorDirection;
				electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.Top;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						electricConnectorDirection = connectorDirection;
						electricConnectorDirection2 = ElectricConnectorDirection.Right;
						if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Left;
							if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
							{
								goto IL_AF;
							}
						}
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_AF:
			return null;
		}

		// Token: 0x040000E1 RID: 225
		public const int Index = 180;
	}
}
