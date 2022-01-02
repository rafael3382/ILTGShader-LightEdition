using System;

namespace Game
{
	// Token: 0x020000CF RID: 207
	public class RandomGeneratorBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x06000446 RID: 1094 RVA: 0x00018AB4 File Offset: 0x00016CB4
		public RandomGeneratorBlock() : base("Models/Gates", "RandomGenerator", 0.375f)
		{
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00018ACB File Offset: 0x00016CCB
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new RandomGeneratorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00018AE4 File Offset: 0x00016CE4
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

		// Token: 0x040001E2 RID: 482
		public const int Index = 157;
	}
}
