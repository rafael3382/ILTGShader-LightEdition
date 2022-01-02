using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000134 RID: 308
	public class Subtexture
	{
		// Token: 0x060005CA RID: 1482 RVA: 0x00020F52 File Offset: 0x0001F152
		public Subtexture(Texture2D texture, Vector2 topLeft, Vector2 bottomRight)
		{
			this.Texture = texture;
			this.TopLeft = topLeft;
			this.BottomRight = bottomRight;
		}

		// Token: 0x0400028E RID: 654
		public readonly Texture2D Texture;

		// Token: 0x0400028F RID: 655
		public readonly Vector2 TopLeft;

		// Token: 0x04000290 RID: 656
		public readonly Vector2 BottomRight;
	}
}
