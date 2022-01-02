using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000123 RID: 291
	public abstract class WoodBlock : CubeBlock
	{
		// Token: 0x060005A4 RID: 1444 RVA: 0x000204BB File Offset: 0x0001E6BB
		public WoodBlock(int cutTextureSlot, int sideTextureSlot)
		{
			this.m_cutTextureSlot = cutTextureSlot;
			this.m_sideTextureSlot = sideTextureSlot;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x000204D4 File Offset: 0x0001E6D4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int cutFace = WoodBlock.GetCutFace(Terrain.ExtractData(value));
			if (cutFace == 0)
			{
				generator.GenerateCubeVertices(this, value, x, y, z, 1, 0, 0, Color.White, geometry.OpaqueSubsetsByFace);
				return;
			}
			if (cutFace == 4)
			{
				generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
				return;
			}
			generator.GenerateCubeVertices(this, value, x, y, z, 0, 1, 1, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00020548 File Offset: 0x0001E748
		public override int GetFaceTextureSlot(int face, int value)
		{
			int cutFace = WoodBlock.GetCutFace(Terrain.ExtractData(value));
			if (cutFace == face || CellFace.OppositeFace(cutFace) == face)
			{
				return this.m_cutTextureSlot;
			}
			return this.m_sideTextureSlot;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0002057C File Offset: 0x0001E77C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.NegativeInfinity;
			int cutFace = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 > num)
				{
					num = num2;
					cutFace = i;
				}
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, WoodBlock.SetCutFace(0, cutFace)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0002060C File Offset: 0x0001E80C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			data = WoodBlock.SetCutFace(data, 4);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00020658 File Offset: 0x0001E858
		public static int GetCutFace(int data)
		{
			data &= 3;
			if (data == 0)
			{
				return 4;
			}
			if (data != 1)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0002066D File Offset: 0x0001E86D
		public static int SetCutFace(int data, int cutFace)
		{
			data &= -4;
			switch (cutFace)
			{
			case 0:
			case 2:
				return data | 1;
			case 1:
			case 3:
				return data | 2;
			default:
				return data;
			}
		}

		// Token: 0x04000279 RID: 633
		public int m_cutTextureSlot;

		// Token: 0x0400027A RID: 634
		public int m_sideTextureSlot;
	}
}
