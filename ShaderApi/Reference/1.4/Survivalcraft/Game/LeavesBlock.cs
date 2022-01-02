using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009E RID: 158
	public abstract class LeavesBlock : AlphaTestCubeBlock
	{
		// Token: 0x06000323 RID: 803 RVA: 0x00013A75 File Offset: 0x00011C75
		public LeavesBlock(BlockColorsMap blockColorsMap)
		{
			this.m_blockColorsMap = blockColorsMap;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00013A90 File Offset: 0x00011C90
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = this.m_blockColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCubeVertices(this, value, x, y, z, color, geometry.AlphaTestSubsetsByFace);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00013ACA File Offset: 0x00011CCA
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= this.m_blockColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00013B04 File Offset: 0x00011D04
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = this.m_blockColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00013B5C File Offset: 0x00011D5C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			if (this.m_random.Bool(0.15f))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = 23,
					Count = 1
				});
				showDebris = true;
				return;
			}
			base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
		}

		// Token: 0x04000171 RID: 369
		public BlockColorsMap m_blockColorsMap;

		// Token: 0x04000172 RID: 370
		public Game.Random m_random = new Game.Random();
	}
}
