using System;
using Engine;

namespace Game
{
	// Token: 0x0200024C RID: 588
	public struct BlockMeshVertex
	{
		// Token: 0x04000C1C RID: 3100
		public Vector3 Position;

		// Token: 0x04000C1D RID: 3101
		public Vector2 TextureCoordinates;

		// Token: 0x04000C1E RID: 3102
		public Color Color;

		// Token: 0x04000C1F RID: 3103
		public byte Face;

		// Token: 0x04000C20 RID: 3104
		public bool IsEmissive;
	}
}
