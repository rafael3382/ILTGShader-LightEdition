using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200004D RID: 77
	public abstract class CubeBlock : Block
	{
		// Token: 0x0600019F RID: 415 RVA: 0x0000BBFB File Offset: 0x00009DFB
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000BC16 File Offset: 0x00009E16
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
}
