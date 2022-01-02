using System;

namespace Game
{
	// Token: 0x02000049 RID: 73
	public class CounterBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x0600018A RID: 394 RVA: 0x0000B529 File Offset: 0x00009729
		public CounterBlock() : base("Models/Gates", "Counter", 0.5f)
		{
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000B540 File Offset: 0x00009740
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CounterElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000B55C File Offset: 0x0000975C
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
						electricConnectorDirection2 = ElectricConnectorDirection.In;
						if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
						{
							electricConnectorDirection = connectorDirection;
							electricConnectorDirection2 = ElectricConnectorDirection.Top;
							if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
							{
								electricConnectorDirection = connectorDirection;
								electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
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

		// Token: 0x040000CC RID: 204
		public const int Index = 184;
	}
}
