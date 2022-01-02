using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004B RID: 75
	public abstract class CrossBlock : Block
	{
		// Token: 0x06000191 RID: 401 RVA: 0x0000B754 File Offset: 0x00009954
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000B782 File Offset: 0x00009982
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
