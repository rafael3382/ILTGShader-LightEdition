using System;

namespace Game
{
	// Token: 0x020000AF RID: 175
	public class MemoryBankBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x0600036E RID: 878 RVA: 0x00014F91 File Offset: 0x00013191
		public MemoryBankBlock() : base("Models/Gates", "MemoryBank", 0.875f)
		{
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00014FA8 File Offset: 0x000131A8
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MemoryBankElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00014FC4 File Offset: 0x000131C4
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
							electricConnectorDirection2 = ElectricConnectorDirection.In;
							if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Top;
								if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
								{
									return new ElectricConnectorType?(ElectricConnectorType.Output);
								}
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

		// Token: 0x04000196 RID: 406
		public const int Index = 186;
	}
}
