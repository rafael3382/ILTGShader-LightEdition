using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C5 RID: 197
	public class PigmentBlock : FlatBlock
	{
		// Token: 0x060003E7 RID: 999 RVA: 0x00016DD8 File Offset: 0x00014FD8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int value2 = Terrain.ExtractData(value);
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * SubsystemPalette.GetColor(environmentData, new int?(value2)), false, environmentData);
		}

		// Token: 0x040001C2 RID: 450
		public const int Index = 130;
	}
}
