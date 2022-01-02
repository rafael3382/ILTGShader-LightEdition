using System;

namespace Game
{
	// Token: 0x020000FB RID: 251
	public class SRLatchBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060004F2 RID: 1266 RVA: 0x0001BA3E File Offset: 0x00019C3E
		public SRLatchBlock() : base("Models/Gates", "SRLatch", 0.375f)
		{
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001BA55 File Offset: 0x00019C55
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SRLatchElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001BA70 File Offset: 0x00019C70
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
						electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
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
									goto IL_AF;
								}
							}
							return new ElectricConnectorType?(ElectricConnectorType.Output);
						}
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_AF:
			return null;
		}

		// Token: 0x0400022F RID: 559
		public const int Index = 146;
	}
}
