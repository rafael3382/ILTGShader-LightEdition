using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000376 RID: 886
	public class ClearWidget : Widget
	{
		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06001A4B RID: 6731 RVA: 0x000CD1DF File Offset: 0x000CB3DF
		// (set) Token: 0x06001A4C RID: 6732 RVA: 0x000CD1E7 File Offset: 0x000CB3E7
		public Color Color { get; set; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001A4D RID: 6733 RVA: 0x000CD1F0 File Offset: 0x000CB3F0
		// (set) Token: 0x06001A4E RID: 6734 RVA: 0x000CD1F8 File Offset: 0x000CB3F8
		public float Depth { get; set; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06001A4F RID: 6735 RVA: 0x000CD201 File Offset: 0x000CB401
		// (set) Token: 0x06001A50 RID: 6736 RVA: 0x000CD209 File Offset: 0x000CB409
		public int Stencil { get; set; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06001A51 RID: 6737 RVA: 0x000CD212 File Offset: 0x000CB412
		// (set) Token: 0x06001A52 RID: 6738 RVA: 0x000CD21A File Offset: 0x000CB41A
		public bool ClearColor { get; set; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06001A53 RID: 6739 RVA: 0x000CD223 File Offset: 0x000CB423
		// (set) Token: 0x06001A54 RID: 6740 RVA: 0x000CD22B File Offset: 0x000CB42B
		public bool ClearDepth { get; set; }

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06001A55 RID: 6741 RVA: 0x000CD234 File Offset: 0x000CB434
		// (set) Token: 0x06001A56 RID: 6742 RVA: 0x000CD23C File Offset: 0x000CB43C
		public bool ClearStencil { get; set; }

		// Token: 0x06001A57 RID: 6743 RVA: 0x000CD248 File Offset: 0x000CB448
		public ClearWidget()
		{
			this.ClearColor = true;
			this.ClearDepth = true;
			this.ClearStencil = true;
			this.Color = Color.Black;
			this.Depth = 1f;
			this.Stencil = 0;
			this.IsHitTestVisible = false;
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x000CD294 File Offset: 0x000CB494
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x000CD2A0 File Offset: 0x000CB4A0
		public override void Draw(Widget.DrawContext dc)
		{
			Display.Clear(this.ClearColor ? new Vector4?(new Vector4(this.Color)) : null, this.ClearDepth ? new float?(this.Depth) : null, this.ClearStencil ? new int?(this.Stencil) : null);
		}
	}
}
