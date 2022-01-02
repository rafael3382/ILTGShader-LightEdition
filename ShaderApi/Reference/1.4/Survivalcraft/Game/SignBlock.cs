using System;
using Engine;

namespace Game
{
	// Token: 0x020000EF RID: 239
	public abstract class SignBlock : Block
	{
		// Token: 0x060004AD RID: 1197
		public abstract BlockMesh GetSignSurfaceBlockMesh(int data);

		// Token: 0x060004AE RID: 1198
		public abstract Vector3 GetSignSurfaceNormal(int data);
	}
}
