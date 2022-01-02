using System;

namespace Game
{
	// Token: 0x0200007F RID: 127
	public interface IElectricElementBlock
	{
		// Token: 0x060002D8 RID: 728
		ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z);

		// Token: 0x060002D9 RID: 729
		ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

		// Token: 0x060002DA RID: 730
		int GetConnectionMask(int value);
	}
}
