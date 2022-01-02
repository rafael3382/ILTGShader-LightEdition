using System;
using Engine;

namespace Game
{
	// Token: 0x0200005B RID: 91
	public class DispenserBlock : CubeBlock, IElectricElementBlock
	{
		// Token: 0x060001BE RID: 446 RVA: 0x0000C22C File Offset: 0x0000A42C
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = DispenserBlock.GetDirection(Terrain.ExtractData(value));
			if (face != direction)
			{
				return 42;
			}
			return 59;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000C250 File Offset: 0x0000A450
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float num5 = Vector3.Dot(forward, Vector3.UnitY);
			float num6 = Vector3.Dot(forward, -Vector3.UnitY);
			float num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			int direction = 0;
			if (num == num7)
			{
				direction = 0;
			}
			else if (num2 == num7)
			{
				direction = 1;
			}
			else if (num3 == num7)
			{
				direction = 2;
			}
			else if (num4 == num7)
			{
				direction = 3;
			}
			else if (num5 == num7)
			{
				direction = 4;
			}
			else if (num6 == num7)
			{
				direction = 5;
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(216, 0, DispenserBlock.SetDirection(0, direction)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000C35D File Offset: 0x0000A55D
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000C362 File Offset: 0x0000A562
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000C36C File Offset: 0x0000A56C
		public static DispenserBlock.Mode GetMode(int data)
		{
			if ((data & 8) != 0)
			{
				return DispenserBlock.Mode.Shoot;
			}
			return DispenserBlock.Mode.Dispense;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000C376 File Offset: 0x0000A576
		public static int SetMode(int data, DispenserBlock.Mode mode)
		{
			return (data & -9) | ((mode != DispenserBlock.Mode.Dispense) ? 8 : 0);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000C384 File Offset: 0x0000A584
		public static bool GetAcceptsDrops(int data)
		{
			return (data & 16) != 0;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000C38D File Offset: 0x0000A58D
		public static int SetAcceptsDrops(int data, bool acceptsDrops)
		{
			return (data & -17) | (acceptsDrops ? 16 : 0);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000C39C File Offset: 0x0000A59C
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DispenserElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000C3AE File Offset: 0x0000A5AE
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000C3B6 File Offset: 0x0000A5B6
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x040000E3 RID: 227
		public const int Index = 216;

		// Token: 0x020003EA RID: 1002
		public enum Mode
		{
			// Token: 0x04001462 RID: 5218
			Dispense,
			// Token: 0x04001463 RID: 5219
			Shoot
		}
	}
}
