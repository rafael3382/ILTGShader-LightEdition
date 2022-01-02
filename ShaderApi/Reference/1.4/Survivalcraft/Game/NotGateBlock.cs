using System;

namespace Game
{
	// Token: 0x020000BB RID: 187
	public class NotGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x00015D6A File Offset: 0x00013F6A
		public NotGateBlock() : base("Models/Gates", "NotGate", 0.375f)
		{
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00015D81 File Offset: 0x00013F81
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new NotGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00015D9C File Offset: 0x00013F9C
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

		// Token: 0x040001AD RID: 429
		public const int Index = 140;
	}
}
