using System;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002B1 RID: 689
	public class InstancedModelData
	{
		// Token: 0x04000DE5 RID: 3557
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementSemantic.Normal),
			new VertexElement(24, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(32, VertexElementFormat.Single, VertexElementSemantic.Instance)
		});

		// Token: 0x04000DE6 RID: 3558
		public VertexBuffer VertexBuffer;

		// Token: 0x04000DE7 RID: 3559
		public IndexBuffer IndexBuffer;
	}
}
