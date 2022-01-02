using System;
using Engine;

namespace Game
{
	// Token: 0x0200026E RID: 622
	public class Dialog : CanvasWidget
	{
		// Token: 0x06001407 RID: 5127 RVA: 0x00095A3C File Offset: 0x00093C3C
		public Dialog()
		{
			this.IsHitTestVisible = true;
			base.Size = new Vector2(float.PositiveInfinity);
		}
	}
}
