using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200027A RID: 634
	public class EditMemoryBankDialog : Dialog
	{
		// Token: 0x0600144A RID: 5194 RVA: 0x00097A7C File Offset: 0x00095C7C
		public EditMemoryBankDialog(MemoryBankData memoryBankData, Action handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditMemoryBankDialog", null);
			base.LoadContents(this, node);
			this.m_linearPanel = this.Children.Find<Widget>("EditMemoryBankDialog.LinearPanel", true);
			this.m_gridPanel = this.Children.Find<Widget>("EditMemoryBankDialog.GridPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.Cancel", true);
			this.m_switchViewButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.SwitchViewButton", true);
			this.m_linearTextBox = this.Children.Find<TextBoxWidget>("EditMemoryBankDialog.LinearText", true);
			for (int i = 0; i < 16; i++)
			{
				this.m_lineTextBoxes[i] = this.Children.Find<TextBoxWidget>("EditMemoryBankDialog.Line" + i.ToString(), true);
			}
			this.m_handler = handler;
			this.m_memoryBankData = memoryBankData;
			this.m_tmpMemoryBankData = (MemoryBankData)this.m_memoryBankData.Copy();
			this.m_linearPanel.IsVisible = false;
			for (int j = 0; j < 16; j++)
			{
				this.m_lineTextBoxes[j].TextChanged += this.TextBox_TextChanged;
			}
			this.m_linearTextBox.TextChanged += this.TextBox_TextChanged;
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00097BDC File Offset: 0x00095DDC
		public void TextBox_TextChanged(TextBoxWidget textBox)
		{
			if (this.m_ignoreTextChanges)
			{
				return;
			}
			if (textBox == this.m_linearTextBox)
			{
				this.m_tmpMemoryBankData = new MemoryBankData();
				this.m_tmpMemoryBankData.LoadString(this.m_linearTextBox.Text);
				return;
			}
			string text = string.Empty;
			for (int i = 0; i < 16; i++)
			{
				text += this.m_lineTextBoxes[i].Text;
			}
			this.m_tmpMemoryBankData = new MemoryBankData();
			this.m_tmpMemoryBankData.LoadString(text);
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x00097C5C File Offset: 0x00095E5C
		public override void Update()
		{
			this.m_ignoreTextChanges = true;
			try
			{
				string text = this.m_tmpMemoryBankData.SaveString(false);
				if (text.Length < 256)
				{
					text += new string('0', 256 - text.Length);
				}
				for (int i = 0; i < 16; i++)
				{
					this.m_lineTextBoxes[i].Text = text.Substring(i * 16, 16);
				}
				this.m_linearTextBox.Text = this.m_tmpMemoryBankData.SaveString(false);
			}
			finally
			{
				this.m_ignoreTextChanges = false;
			}
			if (this.m_linearPanel.IsVisible)
			{
				this.m_switchViewButton.Text = "Grid";
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = false;
					this.m_gridPanel.IsVisible = true;
				}
			}
			else
			{
				this.m_switchViewButton.Text = "Linear";
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = true;
					this.m_gridPanel.IsVisible = false;
				}
			}
			if (this.m_okButton.IsClicked)
			{
				this.m_memoryBankData.Data = this.m_tmpMemoryBankData.Data;
				this.Dismiss(true);
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x00097DC0 File Offset: 0x00095FC0
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result)
			{
				this.m_handler();
			}
		}

		// Token: 0x04000CE5 RID: 3301
		public Action m_handler;

		// Token: 0x04000CE6 RID: 3302
		public Widget m_linearPanel;

		// Token: 0x04000CE7 RID: 3303
		public Widget m_gridPanel;

		// Token: 0x04000CE8 RID: 3304
		public ButtonWidget m_okButton;

		// Token: 0x04000CE9 RID: 3305
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CEA RID: 3306
		public ButtonWidget m_switchViewButton;

		// Token: 0x04000CEB RID: 3307
		public TextBoxWidget[] m_lineTextBoxes = new TextBoxWidget[16];

		// Token: 0x04000CEC RID: 3308
		public TextBoxWidget m_linearTextBox;

		// Token: 0x04000CED RID: 3309
		public MemoryBankData m_memoryBankData;

		// Token: 0x04000CEE RID: 3310
		public MemoryBankData m_tmpMemoryBankData;

		// Token: 0x04000CEF RID: 3311
		public bool m_ignoreTextChanges;
	}
}
