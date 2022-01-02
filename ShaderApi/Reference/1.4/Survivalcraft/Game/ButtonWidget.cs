using System;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000372 RID: 882
	public abstract class ButtonWidget : CanvasWidget
	{
		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001A20 RID: 6688
		public abstract bool IsClicked { get; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06001A21 RID: 6689
		// (set) Token: 0x06001A22 RID: 6690
		public abstract bool IsChecked { get; set; }

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06001A23 RID: 6691
		// (set) Token: 0x06001A24 RID: 6692
		public abstract bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06001A25 RID: 6693
		// (set) Token: 0x06001A26 RID: 6694
		public abstract string Text { get; set; }

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001A27 RID: 6695
		// (set) Token: 0x06001A28 RID: 6696
		public abstract BitmapFont Font { get; set; }

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06001A29 RID: 6697
		// (set) Token: 0x06001A2A RID: 6698
		public abstract Color Color { get; set; }
	}
}
