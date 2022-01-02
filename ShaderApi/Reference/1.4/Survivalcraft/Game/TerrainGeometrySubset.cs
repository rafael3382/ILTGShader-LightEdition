using System;
using Engine;

namespace Game
{
	// Token: 0x02000323 RID: 803
	public class TerrainGeometrySubset
	{
		// Token: 0x060017F0 RID: 6128 RVA: 0x000BB74C File Offset: 0x000B994C
		public TerrainGeometrySubset()
		{
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x000BB76A File Offset: 0x000B996A
		public TerrainGeometrySubset(DynamicArray<TerrainVertex> vertices, DynamicArray<int> indices)
		{
			this.Vertices = vertices;
			this.Indices = indices;
		}

		// Token: 0x0400109D RID: 4253
		public DynamicArray<TerrainVertex> Vertices = new DynamicArray<TerrainVertex>();

		// Token: 0x0400109E RID: 4254
		public DynamicArray<int> Indices = new DynamicArray<int>();
	}
}
