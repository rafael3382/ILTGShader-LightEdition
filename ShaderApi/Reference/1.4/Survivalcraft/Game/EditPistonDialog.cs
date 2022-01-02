using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200027C RID: 636
	public class EditPistonDialog : Dialog
	{
		// Token: 0x06001451 RID: 5201 RVA: 0x0009827C File Offset: 0x0009647C
		public EditPistonDialog(int data, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditPistonDialog", null);
			base.LoadContents(this, node);
			this.m_title = this.Children.Find<LabelWidget>("EditPistonDialog.Title", true);
			this.m_slider1 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider1", true);
			this.m_panel2 = this.Children.Find<ContainerWidget>("EditPistonDialog.Panel2", true);
			this.m_slider2 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider2", true);
			this.m_slider3 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider3", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditPistonDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditPistonDialog.Cancel", true);
			this.m_handler = handler;
			this.m_data = data;
			this.m_mode = PistonBlock.GetMode(data);
			this.m_maxExtension = PistonBlock.GetMaxExtension(data);
			this.m_pullCount = PistonBlock.GetPullCount(data);
			this.m_speed = PistonBlock.GetSpeed(data);
			this.m_title.Text = "Edit " + BlocksManager.Blocks[237].GetDisplayName(null, Terrain.MakeBlockValue(237, 0, data));
			this.m_slider1.Granularity = 1f;
			this.m_slider1.MinValue = 1f;
			this.m_slider1.MaxValue = 8f;
			this.m_slider2.Granularity = 1f;
			this.m_slider2.MinValue = 1f;
			this.m_slider2.MaxValue = 8f;
			this.m_slider3.Granularity = 1f;
			this.m_slider3.MinValue = 0f;
			this.m_slider3.MaxValue = 3f;
			this.m_panel2.IsVisible = (this.m_mode > PistonMode.Pushing);
			this.UpdateControls();
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x00098460 File Offset: 0x00096660
		public override void Update()
		{
			if (this.m_slider1.IsSliding)
			{
				this.m_maxExtension = (int)this.m_slider1.Value - 1;
			}
			if (this.m_slider2.IsSliding)
			{
				this.m_pullCount = (int)this.m_slider2.Value - 1;
			}
			if (this.m_slider3.IsSliding)
			{
				this.m_speed = (int)this.m_slider3.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				int value = PistonBlock.SetMaxExtension(PistonBlock.SetPullCount(PistonBlock.SetSpeed(this.m_data, this.m_speed), this.m_pullCount), this.m_maxExtension);
				this.Dismiss(new int?(value));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x00098540 File Offset: 0x00096740
		public void UpdateControls()
		{
			this.m_slider1.Value = (float)(this.m_maxExtension + 1);
			this.m_slider1.Text = string.Format(LanguageControl.Get(base.GetType().Name, 1), this.m_maxExtension + 1);
			this.m_slider2.Value = (float)(this.m_pullCount + 1);
			this.m_slider2.Text = string.Format(LanguageControl.Get(base.GetType().Name, 1), this.m_pullCount + 1);
			this.m_slider3.Value = (float)this.m_speed;
			this.m_slider3.Text = EditPistonDialog.m_speedNames[this.m_speed];
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x000985FA File Offset: 0x000967FA
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000CF8 RID: 3320
		public LabelWidget m_title;

		// Token: 0x04000CF9 RID: 3321
		public SliderWidget m_slider1;

		// Token: 0x04000CFA RID: 3322
		public SliderWidget m_slider2;

		// Token: 0x04000CFB RID: 3323
		public ContainerWidget m_panel2;

		// Token: 0x04000CFC RID: 3324
		public SliderWidget m_slider3;

		// Token: 0x04000CFD RID: 3325
		public ButtonWidget m_okButton;

		// Token: 0x04000CFE RID: 3326
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CFF RID: 3327
		public Action<int> m_handler;

		// Token: 0x04000D00 RID: 3328
		public int m_data;

		// Token: 0x04000D01 RID: 3329
		public PistonMode m_mode;

		// Token: 0x04000D02 RID: 3330
		public int m_maxExtension;

		// Token: 0x04000D03 RID: 3331
		public int m_pullCount;

		// Token: 0x04000D04 RID: 3332
		public int m_speed;

		// Token: 0x04000D05 RID: 3333
		public static string[] m_speedNames = new string[]
		{
			"Fast",
			"Medium",
			"Slow",
			"Very Slow"
		};
	}
}
