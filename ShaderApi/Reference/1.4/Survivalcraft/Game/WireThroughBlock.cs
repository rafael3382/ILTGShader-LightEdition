using System;
using Engine;

namespace Game
{
	// Token: 0x0200011D RID: 285
	public abstract class WireThroughBlock : CubeBlock, IElectricWireElementBlock, IElectricElementBlock
	{
		// Token: 0x06000596 RID: 1430 RVA: 0x000202D7 File Offset: 0x0001E4D7
		public WireThroughBlock(int wiredTextureSlot, int unwiredTextureSlot)
		{
			this.m_wiredTextureSlot = wiredTextureSlot;
			this.m_unwiredTextureSlot = unwiredTextureSlot;
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x000202ED File Offset: 0x0001E4ED
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return null;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x000202F0 File Offset: 0x0001E4F0
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if ((face == wiredFace || face == CellFace.OppositeFace(wiredFace)) && connectorFace == CellFace.OppositeFace(face))
			{
				return new ElectricConnectorType?(ElectricConnectorType.InputOutput);
			}
			return null;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x00020330 File Offset: 0x0001E530
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00020338 File Offset: 0x0001E538
		public int GetConnectedWireFacesMask(int value, int face)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
			{
				return 1 << wiredFace | 1 << CellFace.OppositeFace(wiredFace);
			}
			return 0;
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x00020374 File Offset: 0x0001E574
		public override int GetFaceTextureSlot(int face, int value)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
			{
				return this.m_wiredTextureSlot;
			}
			return this.m_unwiredTextureSlot;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x000203A8 File Offset: 0x0001E5A8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.NegativeInfinity;
			int wiredFace = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 > num)
				{
					num = num2;
					wiredFace = i;
				}
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, WireThroughBlock.SetWiredFace(0, wiredFace)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00020437 File Offset: 0x0001E637
		public static int GetWiredFace(int data)
		{
			if ((data & 3) == 0)
			{
				return 0;
			}
			if ((data & 3) == 1)
			{
				return 1;
			}
			return 4;
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00020449 File Offset: 0x0001E649
		public static int SetWiredFace(int data, int wiredFace)
		{
			data &= -4;
			switch (wiredFace)
			{
			case 0:
			case 2:
				return data;
			case 1:
			case 3:
				return data | 1;
			default:
				return data | 2;
			}
		}

		// Token: 0x04000272 RID: 626
		public int m_wiredTextureSlot;

		// Token: 0x04000273 RID: 627
		public int m_unwiredTextureSlot;
	}
}
