using System;

namespace Game
{
	// Token: 0x0200004E RID: 78
	public class DelayGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060001A2 RID: 418 RVA: 0x0000BC34 File Offset: 0x00009E34
		public DelayGateBlock() : base("Models/Gates", "DelayGate", 0.375f)
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000BC4B File Offset: 0x00009E4B
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DelayGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000BC64 File Offset: 0x00009E64
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
				if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
				{
					return new ElectricConnectorType?(ElectricConnectorType.Input);
				}
				electricConnectorDirection = connectorDirection;
				electricConnectorDirection2 = ElectricConnectorDirection.Top;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						goto IL_7C;
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			IL_7C:
			return null;
		}

		// Token: 0x040000D2 RID: 210
		public const int Index = 145;
	}
}
