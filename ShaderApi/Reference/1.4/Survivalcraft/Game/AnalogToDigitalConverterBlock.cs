using System;

namespace Game
{
	// Token: 0x0200000F RID: 15
	public class AnalogToDigitalConverterBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00007840 File Offset: 0x00005A40
		public AnalogToDigitalConverterBlock() : base("Models/Gates", "AnalogToDigitalConverter", 0.375f)
		{
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00007857 File Offset: 0x00005A57
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new AnalogToDigitalConverterElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00007870 File Offset: 0x00005A70
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
					return new ElectricConnectorType?(ElectricConnectorType.Input);
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
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			IL_AF:
			return null;
		}

		// Token: 0x04000059 RID: 89
		public const int Index = 181;
	}
}
