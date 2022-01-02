using System;
using Engine;

namespace Game
{
	// Token: 0x020000A4 RID: 164
	public class MagmaBlock : FluidBlock
	{
		// Token: 0x06000358 RID: 856 RVA: 0x00014AAC File Offset: 0x00012CAC
		public MagmaBlock() : base(MagmaBlock.MaxLevel)
		{
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00014AB9 File Offset: 0x00012CB9
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return FluidBlock.GetIsTop(Terrain.ExtractData(value)) && face != 5;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00014AD4 File Offset: 0x00012CD4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			base.GenerateFluidTerrainVertices(generator, value, x, y, z, Color.White, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00014AFF File Offset: 0x00012CFF
		public override bool ShouldAvoid(int value)
		{
			return true;
		}

		// Token: 0x04000186 RID: 390
		public const int Index = 92;

		// Token: 0x04000187 RID: 391
		public new static int MaxLevel = 4;
	}
}
