using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000068 RID: 104
	public class FlourBlock : FlatBlock
	{
		// Token: 0x06000245 RID: 581 RVA: 0x0000F668 File Offset: 0x0000D868
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * new Color(248, 255, 232), false, environmentData);
		}

		// Token: 0x04000111 RID: 273
		public const int Index = 175;
	}
}
