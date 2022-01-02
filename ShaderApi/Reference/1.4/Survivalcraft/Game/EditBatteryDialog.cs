using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000278 RID: 632
	public class EditBatteryDialog : Dialog
	{
		// Token: 0x06001440 RID: 5184 RVA: 0x00097098 File Offset: 0x00095298
		public EditBatteryDialog(int voltageLevel, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditBatteryDialog", null);
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditBatteryDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditBatteryDialog.Cancel", true);
			this.m_voltageSlider = this.Children.Find<SliderWidget>("EditBatteryDialog.VoltageSlider", true);
			this.m_handler = handler;
			this.m_voltageLevel = voltageLevel;
			this.UpdateControls();
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x00097118 File Offset: 0x00095318
		public override void Update()
		{
			if (this.m_voltageSlider.IsSliding)
			{
				this.m_voltageLevel = (int)this.m_voltageSlider.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new int?(this.m_voltageLevel));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x00097194 File Offset: 0x00095394
		public void UpdateControls()
		{
			this.m_voltageSlider.Text = string.Format("{0:0.0}V ({1})", 1.5f * (float)this.m_voltageLevel / 15f, (this.m_voltageLevel < 8) ? LanguageControl.Get(base.GetType().Name, 1) : LanguageControl.Get(base.GetType().Name, 2));
			this.m_voltageSlider.Value = (float)this.m_voltageLevel;
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0009720D File Offset: 0x0009540D
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000CD7 RID: 3287
		public Action<int> m_handler;

		// Token: 0x04000CD8 RID: 3288
		public ButtonWidget m_okButton;

		// Token: 0x04000CD9 RID: 3289
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CDA RID: 3290
		public SliderWidget m_voltageSlider;

		// Token: 0x04000CDB RID: 3291
		public int m_voltageLevel;
	}
}
