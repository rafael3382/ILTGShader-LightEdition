using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000048 RID: 72
	public class CottonWadBlock : FlatBlock
	{
		// Token: 0x06000188 RID: 392 RVA: 0x0000B50F File Offset: 0x0000970F
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x040000CB RID: 203
		public const int Index = 205;
	}
}
