using System;

namespace Game
{
	// Token: 0x02000377 RID: 887
	public class ClickableWidget : Widget
	{
		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06001A5A RID: 6746 RVA: 0x000CD311 File Offset: 0x000CB511
		// (set) Token: 0x06001A5B RID: 6747 RVA: 0x000CD319 File Offset: 0x000CB519
		public string SoundName { get; set; }

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06001A5C RID: 6748 RVA: 0x000CD322 File Offset: 0x000CB522
		// (set) Token: 0x06001A5D RID: 6749 RVA: 0x000CD32A File Offset: 0x000CB52A
		public bool IsPressed { get; set; }

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001A5E RID: 6750 RVA: 0x000CD333 File Offset: 0x000CB533
		// (set) Token: 0x06001A5F RID: 6751 RVA: 0x000CD33B File Offset: 0x000CB53B
		public bool IsClicked { get; set; }

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06001A60 RID: 6752 RVA: 0x000CD344 File Offset: 0x000CB544
		// (set) Token: 0x06001A61 RID: 6753 RVA: 0x000CD34C File Offset: 0x000CB54C
		public bool IsTapped { get; set; }

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06001A62 RID: 6754 RVA: 0x000CD355 File Offset: 0x000CB555
		// (set) Token: 0x06001A63 RID: 6755 RVA: 0x000CD35D File Offset: 0x000CB55D
		public bool IsChecked { get; set; }

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06001A64 RID: 6756 RVA: 0x000CD366 File Offset: 0x000CB566
		// (set) Token: 0x06001A65 RID: 6757 RVA: 0x000CD36E File Offset: 0x000CB56E
		public bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x06001A66 RID: 6758 RVA: 0x000CD377 File Offset: 0x000CB577
		public override void UpdateCeases()
		{
			base.UpdateCeases();
			this.IsPressed = false;
			this.IsClicked = false;
			this.IsTapped = false;
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x000CD394 File Offset: 0x000CB594
		public override void Update()
		{
			WidgetInput input = base.Input;
			this.IsPressed = false;
			this.IsTapped = false;
			this.IsClicked = false;
			if (input.Press != null && base.HitTestGlobal(input.Press.Value, null) == this)
			{
				this.IsPressed = true;
			}
			if (input.Tap != null && base.HitTestGlobal(input.Tap.Value, null) == this)
			{
				this.IsTapped = true;
			}
			if (input.Click != null && base.HitTestGlobal(input.Click.Value.Start, null) == this && base.HitTestGlobal(input.Click.Value.End, null) == this)
			{
				this.IsClicked = true;
				if (this.IsAutoCheckingEnabled)
				{
					this.IsChecked = !this.IsChecked;
				}
				if (!string.IsNullOrEmpty(this.SoundName))
				{
					AudioManager.PlaySound(this.SoundName, 1f, 0f, 0f);
				}
			}
		}
	}
}
