using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000277 RID: 631
	public class EditAdjustableDelayGateDialog : Dialog
	{
		// Token: 0x0600143C RID: 5180 RVA: 0x00096E38 File Offset: 0x00095038
		public EditAdjustableDelayGateDialog(int delay, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditAdjustableDelayGateDialog", null);
			base.LoadContents(this, node);
			this.m_delaySlider = this.Children.Find<SliderWidget>("EditAdjustableDelayGateDialog.DelaySlider", true);
			this.m_plusButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.PlusButton", true);
			this.m_minusButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.MinusButton", true);
			this.m_delayLabel = this.Children.Find<LabelWidget>("EditAdjustableDelayGateDialog.Label", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel", true);
			this.m_handler = handler;
			this.m_delay = delay;
			this.UpdateControls();
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x00096F00 File Offset: 0x00095100
		public override void Update()
		{
			if (this.m_delaySlider.IsSliding)
			{
				this.m_delay = (int)this.m_delaySlider.Value;
			}
			if (this.m_minusButton.IsClicked)
			{
				this.m_delay = MathUtils.Max(this.m_delay - 1, (int)this.m_delaySlider.MinValue);
			}
			if (this.m_plusButton.IsClicked)
			{
				this.m_delay = MathUtils.Min(this.m_delay + 1, (int)this.m_delaySlider.MaxValue);
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new int?(this.m_delay));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00096FD4 File Offset: 0x000951D4
		public void UpdateControls()
		{
			this.m_delaySlider.Value = (float)this.m_delay;
			this.m_minusButton.IsEnabled = ((float)this.m_delay > this.m_delaySlider.MinValue);
			this.m_plusButton.IsEnabled = ((float)this.m_delay < this.m_delaySlider.MaxValue);
			this.m_delayLabel.Text = string.Format(LanguageControl.Get(base.GetType().Name, 1), Math.Round((double)((float)(this.m_delay + 1) * 0.01f), 2));
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0009706D File Offset: 0x0009526D
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000CCF RID: 3279
		public Action<int> m_handler;

		// Token: 0x04000CD0 RID: 3280
		public SliderWidget m_delaySlider;

		// Token: 0x04000CD1 RID: 3281
		public ButtonWidget m_plusButton;

		// Token: 0x04000CD2 RID: 3282
		public ButtonWidget m_minusButton;

		// Token: 0x04000CD3 RID: 3283
		public LabelWidget m_delayLabel;

		// Token: 0x04000CD4 RID: 3284
		public ButtonWidget m_okButton;

		// Token: 0x04000CD5 RID: 3285
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CD6 RID: 3286
		public int m_delay;
	}
}
