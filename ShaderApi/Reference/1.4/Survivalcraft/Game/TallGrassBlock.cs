using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200010D RID: 269
	public class TallGrassBlock : CrossBlock
	{
		// Token: 0x06000537 RID: 1335 RVA: 0x0001DF44 File Offset: 0x0001C144
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (!TallGrassBlock.GetIsSmall(data))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(19, 0, data),
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001DF8C File Offset: 0x0001C18C
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!TallGrassBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return 85;
			}
			return 84;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001DFA0 File Offset: 0x0001C1A0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001DFDC File Offset: 0x0001C1DC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCrossfaceVertices(this, value, x, y, z, BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z), this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001E01B File Offset: 0x0001C21B
		public override int GetShadowStrength(int value)
		{
			if (!TallGrassBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return this.DefaultShadowStrength / 2;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001E03C File Offset: 0x0001C23C
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001E093 File Offset: 0x0001C293
		public static bool GetIsSmall(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001E09B File Offset: 0x0001C29B
		public static int SetIsSmall(int data, bool isSmall)
		{
			if (!isSmall)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x04000251 RID: 593
		public const int Index = 19;
	}
}
