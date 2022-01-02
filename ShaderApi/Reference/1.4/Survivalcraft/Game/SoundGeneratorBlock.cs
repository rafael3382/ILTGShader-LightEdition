using System;

namespace Game
{
	// Token: 0x020000F6 RID: 246
	public class SoundGeneratorBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x0001B3CA File Offset: 0x000195CA
		public SoundGeneratorBlock() : base("Models/Gates", "SoundGenerator", 0.5f)
		{
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001B3E1 File Offset: 0x000195E1
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SoundGeneratorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001B3FC File Offset: 0x000195FC
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
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
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			IL_A8:
			return null;
		}

		// Token: 0x04000225 RID: 549
		public const int Index = 183;
	}
}
