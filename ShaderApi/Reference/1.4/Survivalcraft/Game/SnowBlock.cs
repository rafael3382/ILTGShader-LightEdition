using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020000F4 RID: 244
	public class SnowBlock : CubeBlock
	{
		// Token: 0x060004C8 RID: 1224 RVA: 0x0001B0B6 File Offset: 0x000192B6
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != 5;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001B0C0 File Offset: 0x000192C0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, 0.125f, 0.125f, 0.125f, 0.125f, Color.White, Color.White, Color.White, Color.White, Color.White, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001B110 File Offset: 0x00019310
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int num = this.Random.Int(1, 3);
				for (int i = 0; i < num; i++)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(85),
						Count = 1
					});
				}
			}
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0001B16B File Offset: 0x0001936B
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x04000220 RID: 544
		public const int Index = 61;

		// Token: 0x04000221 RID: 545
		public BoundingBox[] m_collisionBoxes = new BoundingBox[]
		{
			new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.125f, 1f))
		};
	}
}
