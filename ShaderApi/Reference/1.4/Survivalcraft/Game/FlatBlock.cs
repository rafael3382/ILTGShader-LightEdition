using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000067 RID: 103
	public abstract class FlatBlock : Block
	{
		// Token: 0x06000242 RID: 578 RVA: 0x0000F64C File Offset: 0x0000D84C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000F64E File Offset: 0x0000D84E
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
