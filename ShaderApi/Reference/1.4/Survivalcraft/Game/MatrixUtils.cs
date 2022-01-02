using System;
using Engine;

namespace Game
{
	// Token: 0x020002C1 RID: 705
	internal class MatrixUtils
	{
		// Token: 0x0600158A RID: 5514 RVA: 0x000A27E4 File Offset: 0x000A09E4
		public static Matrix CreateScaleTranslation(float sx, float sy, float tx, float ty)
		{
			return new Matrix(sx, 0f, 0f, 0f, 0f, sy, 0f, 0f, 0f, 0f, 1f, 0f, tx, ty, 0f, 1f);
		}
	}
}
