using System;
using Engine;

namespace Game
{
	// Token: 0x02000030 RID: 48
	public class ChestBlock : CubeBlock
	{
		// Token: 0x06000151 RID: 337 RVA: 0x0000A7BC File Offset: 0x000089BC
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 42;
			}
			if (face == 5)
			{
				return 42;
			}
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 27;
				}
				if (face != 2)
				{
					return 25;
				}
				return 26;
			case 1:
				if (face == 1)
				{
					return 27;
				}
				if (face != 3)
				{
					return 25;
				}
				return 26;
			case 2:
				if (face == 0)
				{
					return 26;
				}
				if (face == 2)
				{
					return 27;
				}
				return 25;
			default:
				if (face == 1)
				{
					return 26;
				}
				if (face == 3)
				{
					return 27;
				}
				return 25;
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000A83C File Offset: 0x00008A3C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 45), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x040000A8 RID: 168
		public const int Index = 45;
	}
}
