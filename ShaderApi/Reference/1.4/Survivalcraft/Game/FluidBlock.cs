using System;
using System.Reflection;
using Engine;

namespace Game
{
	// Token: 0x0200006A RID: 106
	public abstract class FluidBlock : CubeBlock
	{
		// Token: 0x0600024D RID: 589 RVA: 0x0000F748 File Offset: 0x0000D948
		public FluidBlock(int maxLevel)
		{
			this.MaxLevel = maxLevel;
			for (int i = 0; i < 16; i++)
			{
				float num = 0.875f * MathUtils.Saturate(1f - (float)i / (float)this.MaxLevel);
				this.m_heightByLevel[i] = num;
				this.m_boundingBoxesByLevel[i] = new BoundingBox[]
				{
					new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, num, 1f))
				};
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000F7EC File Offset: 0x0000D9EC
		public override void Initialize()
		{
			base.Initialize();
			TypeInfo typeInfo = null;
			TypeInfo typeInfo2 = base.GetType().GetTypeInfo();
			while (typeInfo2 != null)
			{
				if (typeInfo2.BaseType == typeof(FluidBlock))
				{
					typeInfo = typeInfo2;
					break;
				}
				typeInfo2 = typeInfo2.BaseType.GetTypeInfo();
			}
			if (typeInfo == null)
			{
				throw new InvalidOperationException("Fluid type not found.");
			}
			this.m_theSameFluidsByIndex = new bool[BlocksManager.Blocks.Length];
			for (int i = 0; i < BlocksManager.Blocks.Length; i++)
			{
				Block block = BlocksManager.Blocks[i];
				this.m_theSameFluidsByIndex[i] = (block.GetType().GetTypeInfo() == typeInfo || block.GetType().GetTypeInfo().IsSubclassOf(typeInfo.AsType()));
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000F8B3 File Offset: 0x0000DAB3
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_boundingBoxesByLevel[FluidBlock.GetLevel(Terrain.ExtractData(value))];
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000F8C7 File Offset: 0x0000DAC7
		public bool IsTheSameFluid(int contents)
		{
			return this.m_theSameFluidsByIndex[contents];
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000F8D1 File Offset: 0x0000DAD1
		public float GetLevelHeight(int level)
		{
			return this.m_heightByLevel[level];
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000F8DC File Offset: 0x0000DADC
		public void GenerateFluidTerrainVertices(BlockGeometryGenerator generator, int value, int x, int y, int z, Color sideColor, Color topColor, TerrainGeometrySubset[] subset)
		{
			int data = Terrain.ExtractData(value);
			if (FluidBlock.GetIsTop(data))
			{
				Terrain terrain = generator.Terrain;
				int cellValueFast = terrain.GetCellValueFast(x - 1, y, z - 1);
				int cellValueFast2 = terrain.GetCellValueFast(x, y, z - 1);
				int cellValueFast3 = terrain.GetCellValueFast(x + 1, y, z - 1);
				int cellValueFast4 = terrain.GetCellValueFast(x - 1, y, z);
				int cellValueFast5 = terrain.GetCellValueFast(x + 1, y, z);
				int cellValueFast6 = terrain.GetCellValueFast(x - 1, y, z + 1);
				int cellValueFast7 = terrain.GetCellValueFast(x, y, z + 1);
				int cellValueFast8 = terrain.GetCellValueFast(x + 1, y, z + 1);
				float h = this.CalculateNeighborHeight(cellValueFast);
				float num = this.CalculateNeighborHeight(cellValueFast2);
				float h2 = this.CalculateNeighborHeight(cellValueFast3);
				float num2 = this.CalculateNeighborHeight(cellValueFast4);
				float num3 = this.CalculateNeighborHeight(cellValueFast5);
				float h3 = this.CalculateNeighborHeight(cellValueFast6);
				float num4 = this.CalculateNeighborHeight(cellValueFast7);
				float h4 = this.CalculateNeighborHeight(cellValueFast8);
				float levelHeight = this.GetLevelHeight(FluidBlock.GetLevel(data));
				float height = FluidBlock.CalculateFluidVertexHeight(h, num, num2, levelHeight);
				float height2 = FluidBlock.CalculateFluidVertexHeight(num, h2, levelHeight, num3);
				float height3 = FluidBlock.CalculateFluidVertexHeight(levelHeight, num3, num4, h4);
				float height4 = FluidBlock.CalculateFluidVertexHeight(num2, levelHeight, h3, num4);
				float x2 = FluidBlock.ZeroSubst(num3, levelHeight) - FluidBlock.ZeroSubst(num2, levelHeight);
				float x3 = FluidBlock.ZeroSubst(num4, levelHeight) - FluidBlock.ZeroSubst(num, levelHeight);
				int overrideTopTextureSlot = this.DefaultTextureSlot - (int)MathUtils.Sign(x2) - 16 * (int)MathUtils.Sign(x3);
				generator.GenerateCubeVertices(this, value, x, y, z, height, height2, height3, height4, sideColor, topColor, topColor, topColor, topColor, overrideTopTextureSlot, subset);
				return;
			}
			generator.GenerateCubeVertices(this, value, x, y, z, sideColor, subset);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000FA8C File Offset: 0x0000DC8C
		public static float ZeroSubst(float v, float subst)
		{
			if (v != 0f)
			{
				return v;
			}
			return subst;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000FA9C File Offset: 0x0000DC9C
		public static float CalculateFluidVertexHeight(float h1, float h2, float h3, float h4)
		{
			float num = MathUtils.Max(h1, h2, h3, h4);
			if (num >= 1f)
			{
				return 1f;
			}
			if (h1 == 0.01f || h2 == 0.01f || h3 == 0.01f || h4 == 0.01f)
			{
				return 0f;
			}
			return num;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000FAE8 File Offset: 0x0000DCE8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), 0),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000FB25 File Offset: 0x0000DD25
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face >= 4)
			{
				return this.DefaultTextureSlot;
			}
			return this.DefaultTextureSlot + 16;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000FB3C File Offset: 0x0000DD3C
		public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			int contents = Terrain.ExtractContents(neighborValue);
			return !this.IsTheSameFluid(contents) && base.ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000FB68 File Offset: 0x0000DD68
		public float CalculateNeighborHeight(int value)
		{
			int num = Terrain.ExtractContents(value);
			if (this.IsTheSameFluid(num))
			{
				int data = Terrain.ExtractData(value);
				if (FluidBlock.GetIsTop(data))
				{
					return this.GetLevelHeight(FluidBlock.GetLevel(data));
				}
				return 1f;
			}
			else
			{
				if (num == 0)
				{
					return 0.01f;
				}
				return 0f;
			}
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000FBB5 File Offset: 0x0000DDB5
		public override bool IsHeatBlocker(int value)
		{
			return true;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000FBB8 File Offset: 0x0000DDB8
		public static int GetLevel(int data)
		{
			return data & 15;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000FBBE File Offset: 0x0000DDBE
		public static int SetLevel(int data, int level)
		{
			return (data & -16) | (level & 15);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000FBC9 File Offset: 0x0000DDC9
		public static bool GetIsTop(int data)
		{
			return (data & 16) != 0;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000FBD2 File Offset: 0x0000DDD2
		public static int SetIsTop(int data, bool isTop)
		{
			if (!isTop)
			{
				return data & -17;
			}
			return data | 16;
		}

		// Token: 0x04000112 RID: 274
		public float[] m_heightByLevel = new float[16];

		// Token: 0x04000113 RID: 275
		public BoundingBox[][] m_boundingBoxesByLevel = new BoundingBox[16][];

		// Token: 0x04000114 RID: 276
		public bool[] m_theSameFluidsByIndex;

		// Token: 0x04000115 RID: 277
		public readonly int MaxLevel;
	}
}
