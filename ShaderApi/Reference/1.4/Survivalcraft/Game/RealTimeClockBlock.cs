using System;

namespace Game
{
	// Token: 0x020000D3 RID: 211
	public class RealTimeClockBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x0600044C RID: 1100 RVA: 0x00018BDA File Offset: 0x00016DDA
		public RealTimeClockBlock() : base("Models/Gates", "RealTimeClock", 0.5f)
		{
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00018BF1 File Offset: 0x00016DF1
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new RealTimeClockElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00018C0C File Offset: 0x00016E0C
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Top;
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
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
							if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.In;
								if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
								{
									goto IL_A8;
								}
							}
						}
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			IL_A8:
			return null;
		}

		// Token: 0x040001E6 RID: 486
		public const int Index = 187;
	}
}
