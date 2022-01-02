using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000E9 RID: 233
	public class SeagrassBlock : WaterPlantBlock
	{
		// Token: 0x06000484 RID: 1156 RVA: 0x00019A4A File Offset: 0x00017C4A
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face < 0)
			{
				return 105;
			}
			return base.GetFaceTextureSlot(face, value);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00019A5C File Offset: 0x00017C5C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.SeagrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00019A98 File Offset: 0x00017C98
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.SeagrassColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(-1, value), geometry.SubsetAlphaTest);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00019AE8 File Offset: 0x00017CE8
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, BlockColorsMap.SeagrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z)), 104);
		}

		// Token: 0x04000205 RID: 517
		public new const int Index = 233;
	}
}
