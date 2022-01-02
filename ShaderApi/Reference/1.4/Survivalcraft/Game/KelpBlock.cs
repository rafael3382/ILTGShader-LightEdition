using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000098 RID: 152
	public class KelpBlock : WaterPlantBlock
	{
		// Token: 0x0600030E RID: 782 RVA: 0x0001358A File Offset: 0x0001178A
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face < 0)
			{
				return 104;
			}
			return base.GetFaceTextureSlot(face, value);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001359C File Offset: 0x0001179C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.KelpColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000135D8 File Offset: 0x000117D8
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.KelpColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(-1, value), geometry.SubsetAlphaTest);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00013628 File Offset: 0x00011828
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, BlockColorsMap.KelpColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z)), 104);
		}

		// Token: 0x04000166 RID: 358
		public new const int Index = 232;
	}
}
