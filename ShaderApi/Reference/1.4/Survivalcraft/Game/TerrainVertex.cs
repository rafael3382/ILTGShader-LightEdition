using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000329 RID: 809
	public struct TerrainVertex
	{
		// Token: 0x040010DD RID: 4317
		public float X;

		// Token: 0x040010DE RID: 4318
		public float Y;

		// Token: 0x040010DF RID: 4319
		public float Z;

		// Token: 0x040010E0 RID: 4320
		public short Tx;

		// Token: 0x040010E1 RID: 4321
		public short Ty;

		// Token: 0x040010E2 RID: 4322
		public Color Color;

		// Token: 0x040010E3 RID: 4323
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.NormalizedShort2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(16, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});
	}
}
