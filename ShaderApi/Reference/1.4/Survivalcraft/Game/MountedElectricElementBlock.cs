using System;

namespace Game
{
	// Token: 0x020000B4 RID: 180
	public abstract class MountedElectricElementBlock : Block, IElectricElementBlock
	{
		// Token: 0x06000380 RID: 896
		public abstract int GetFace(int value);

		// Token: 0x06000381 RID: 897
		public abstract ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z);

		// Token: 0x06000382 RID: 898
		public abstract ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

		// Token: 0x06000383 RID: 899 RVA: 0x00015517 File Offset: 0x00013717
		public virtual int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}
	}
}
