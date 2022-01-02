using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200027D RID: 637
	public class EditSignDialog : Dialog
	{
		// Token: 0x06001456 RID: 5206 RVA: 0x00098654 File Offset: 0x00096854
		public EditSignDialog(SubsystemSignBlockBehavior subsystemSignBlockBehavior, Point3 signPoint)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditSignDialog", null);
			base.LoadContents(this, node);
			this.m_linesPage = this.Children.Find<ContainerWidget>("EditSignDialog.LinesPage", true);
			this.m_urlPage = this.Children.Find<ContainerWidget>("EditSignDialog.UrlPage", true);
			this.m_textBox1 = this.Children.Find<TextBoxWidget>("EditSignDialog.TextBox1", true);
			this.m_textBox2 = this.Children.Find<TextBoxWidget>("EditSignDialog.TextBox2", true);
			this.m_textBox3 = this.Children.Find<TextBoxWidget>("EditSignDialog.TextBox3", true);
			this.m_textBox4 = this.Children.Find<TextBoxWidget>("EditSignDialog.TextBox4", true);
			this.m_colorButton1 = this.Children.Find<ButtonWidget>("EditSignDialog.ColorButton1", true);
			this.m_colorButton2 = this.Children.Find<ButtonWidget>("EditSignDialog.ColorButton2", true);
			this.m_colorButton3 = this.Children.Find<ButtonWidget>("EditSignDialog.ColorButton3", true);
			this.m_colorButton4 = this.Children.Find<ButtonWidget>("EditSignDialog.ColorButton4", true);
			this.m_urlTextBox = this.Children.Find<TextBoxWidget>("EditSignDialog.UrlTextBox", true);
			this.m_urlTestButton = this.Children.Find<ButtonWidget>("EditSignDialog.UrlTestButton", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditSignDialog.OkButton", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditSignDialog.CancelButton", true);
			this.m_urlButton = this.Children.Find<ButtonWidget>("EditSignDialog.UrlButton", true);
			this.m_linesButton = this.Children.Find<ButtonWidget>("EditSignDialog.LinesButton", true);
			this.m_subsystemSignBlockBehavior = subsystemSignBlockBehavior;
			this.m_signPoint = signPoint;
			SignData signData = this.m_subsystemSignBlockBehavior.GetSignData(this.m_signPoint);
			if (signData != null)
			{
				this.m_textBox1.Text = signData.Lines[0];
				this.m_textBox2.Text = signData.Lines[1];
				this.m_textBox3.Text = signData.Lines[2];
				this.m_textBox4.Text = signData.Lines[3];
				this.m_colorButton1.Color = signData.Colors[0];
				this.m_colorButton2.Color = signData.Colors[1];
				this.m_colorButton3.Color = signData.Colors[2];
				this.m_colorButton4.Color = signData.Colors[3];
				this.m_urlTextBox.Text = signData.Url;
			}
			else
			{
				this.m_textBox1.Text = string.Empty;
				this.m_textBox2.Text = string.Empty;
				this.m_textBox3.Text = string.Empty;
				this.m_textBox4.Text = string.Empty;
				this.m_colorButton1.Color = Color.Black;
				this.m_colorButton2.Color = Color.Black;
				this.m_colorButton3.Color = Color.Black;
				this.m_colorButton4.Color = Color.Black;
				this.m_urlTextBox.Text = string.Empty;
			}
			this.m_linesPage.IsVisible = true;
			this.m_urlPage.IsVisible = false;
			this.UpdateControls();
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00098A24 File Offset: 0x00096C24
		public override void Update()
		{
			this.UpdateControls();
			if (this.m_okButton.IsClicked)
			{
				string[] lines = new string[]
				{
					this.m_textBox1.Text,
					this.m_textBox2.Text,
					this.m_textBox3.Text,
					this.m_textBox4.Text
				};
				Color[] colors = new Color[]
				{
					this.m_colorButton1.Color,
					this.m_colorButton2.Color,
					this.m_colorButton3.Color,
					this.m_colorButton4.Color
				};
				this.m_subsystemSignBlockBehavior.SetSignData(this.m_signPoint, lines, colors, this.m_urlTextBox.Text);
				this.Dismiss();
			}
			if (this.m_urlButton.IsClicked)
			{
				this.m_urlPage.IsVisible = true;
				this.m_linesPage.IsVisible = false;
			}
			if (this.m_linesButton.IsClicked)
			{
				this.m_urlPage.IsVisible = false;
				this.m_linesPage.IsVisible = true;
			}
			if (this.m_urlTestButton.IsClicked)
			{
				WebBrowserManager.LaunchBrowser(this.m_urlTextBox.Text);
			}
			if (this.m_colorButton1.IsClicked)
			{
				this.m_colorButton1.Color = this.m_colors[(this.m_colors.FirstIndex(this.m_colorButton1.Color) + 1) % this.m_colors.Length];
			}
			if (this.m_colorButton2.IsClicked)
			{
				this.m_colorButton2.Color = this.m_colors[(this.m_colors.FirstIndex(this.m_colorButton2.Color) + 1) % this.m_colors.Length];
			}
			if (this.m_colorButton3.IsClicked)
			{
				this.m_colorButton3.Color = this.m_colors[(this.m_colors.FirstIndex(this.m_colorButton3.Color) + 1) % this.m_colors.Length];
			}
			if (this.m_colorButton4.IsClicked)
			{
				this.m_colorButton4.Color = this.m_colors[(this.m_colors.FirstIndex(this.m_colorButton4.Color) + 1) % this.m_colors.Length];
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss();
			}
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x00098C90 File Offset: 0x00096E90
		public void UpdateControls()
		{
			bool flag = !string.IsNullOrEmpty(this.m_urlTextBox.Text);
			this.m_urlButton.IsVisible = this.m_linesPage.IsVisible;
			this.m_linesButton.IsVisible = !this.m_linesPage.IsVisible;
			this.m_colorButton1.IsEnabled = !flag;
			this.m_colorButton2.IsEnabled = !flag;
			this.m_colorButton3.IsEnabled = !flag;
			this.m_colorButton4.IsEnabled = !flag;
			this.m_urlTestButton.IsEnabled = flag;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00098D28 File Offset: 0x00096F28
		public void Dismiss()
		{
			DialogsManager.HideDialog(this);
		}

		// Token: 0x04000D06 RID: 3334
		public SubsystemSignBlockBehavior m_subsystemSignBlockBehavior;

		// Token: 0x04000D07 RID: 3335
		public Point3 m_signPoint;

		// Token: 0x04000D08 RID: 3336
		public ContainerWidget m_linesPage;

		// Token: 0x04000D09 RID: 3337
		public ContainerWidget m_urlPage;

		// Token: 0x04000D0A RID: 3338
		public TextBoxWidget m_textBox1;

		// Token: 0x04000D0B RID: 3339
		public TextBoxWidget m_textBox2;

		// Token: 0x04000D0C RID: 3340
		public TextBoxWidget m_textBox3;

		// Token: 0x04000D0D RID: 3341
		public TextBoxWidget m_textBox4;

		// Token: 0x04000D0E RID: 3342
		public ButtonWidget m_colorButton1;

		// Token: 0x04000D0F RID: 3343
		public ButtonWidget m_colorButton2;

		// Token: 0x04000D10 RID: 3344
		public ButtonWidget m_colorButton3;

		// Token: 0x04000D11 RID: 3345
		public ButtonWidget m_colorButton4;

		// Token: 0x04000D12 RID: 3346
		public TextBoxWidget m_urlTextBox;

		// Token: 0x04000D13 RID: 3347
		public ButtonWidget m_urlTestButton;

		// Token: 0x04000D14 RID: 3348
		public ButtonWidget m_okButton;

		// Token: 0x04000D15 RID: 3349
		public ButtonWidget m_cancelButton;

		// Token: 0x04000D16 RID: 3350
		public ButtonWidget m_urlButton;

		// Token: 0x04000D17 RID: 3351
		public ButtonWidget m_linesButton;

		// Token: 0x04000D18 RID: 3352
		public Color[] m_colors = new Color[]
		{
			new Color(0, 0, 0),
			new Color(140, 0, 0),
			new Color(0, 112, 0),
			new Color(0, 0, 96),
			new Color(160, 0, 128),
			new Color(0, 112, 112),
			new Color(160, 112, 0),
			new Color(180, 180, 180)
		};
	}
}
