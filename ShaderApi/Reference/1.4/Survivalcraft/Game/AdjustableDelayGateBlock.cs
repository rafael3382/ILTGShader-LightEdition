using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200000C RID: 12
	public class AdjustableDelayGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x000076D2 File Offset: 0x000058D2
		public AdjustableDelayGateBlock() : base("Models/Gates", "AdjustableDelayGate", 0.375f)
		{
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000076EC File Offset: 0x000058EC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int delay = AdjustableDelayGateBlock.GetDelay(Terrain.ExtractData(oldValue));
				int data = AdjustableDelayGateBlock.SetDelay(0, delay);
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(224, 0, data),
					Count = 1
				});
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007746 File Offset: 0x00005946
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new AdjustableDelayGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007760 File Offset: 0x00005960
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

		// Token: 0x060000A5 RID: 165 RVA: 0x000077F3 File Offset: 0x000059F3
		public static int GetDelay(int data)
		{
			return data >> 5 & 255;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000077FE File Offset: 0x000059FE
		public static int SetDelay(int data, int delay)
		{
			return (data & -8161) | (delay & 255) << 5;
		}

		// Token: 0x04000057 RID: 87
		public const int Index = 224;
	}
}
