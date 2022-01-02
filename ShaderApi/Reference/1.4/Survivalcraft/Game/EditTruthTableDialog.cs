using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200027E RID: 638
	public class EditTruthTableDialog : Dialog
	{
		// Token: 0x0600145A RID: 5210 RVA: 0x00098D30 File Offset: 0x00096F30
		public EditTruthTableDialog(TruthTableData truthTableData, Action<bool> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditTruthTableDialog", null);
			base.LoadContents(this, node);
			this.m_linearPanel = this.Children.Find<Widget>("EditTruthTableDialog.LinearPanel", true);
			this.m_gridPanel = this.Children.Find<Widget>("EditTruthTableDialog.GridPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.Cancel", true);
			this.m_switchViewButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.SwitchViewButton", true);
			this.m_linearTextBox = this.Children.Find<TextBoxWidget>("EditTruthTableDialog.LinearText", true);
			for (int i = 0; i < 16; i++)
			{
				this.m_lineCheckboxes[i] = this.Children.Find<CheckboxWidget>("EditTruthTableDialog.Line" + i.ToString(), true);
			}
			this.m_handler = handler;
			this.m_truthTableData = truthTableData;
			this.m_tmpTruthTableData = (TruthTableData)this.m_truthTableData.Copy();
			this.m_linearPanel.IsVisible = false;
			this.m_linearTextBox.TextChanged += delegate(TextBoxWidget <p0>)
			{
				if (!this.m_ignoreTextChanges)
				{
					this.m_tmpTruthTableData = new TruthTableData();
					this.m_tmpTruthTableData.LoadBinaryString(this.m_linearTextBox.Text);
				}
			};
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00098E68 File Offset: 0x00097068
		public override void Update()
		{
			this.m_ignoreTextChanges = true;
			try
			{
				this.m_linearTextBox.Text = this.m_tmpTruthTableData.SaveBinaryString();
			}
			finally
			{
				this.m_ignoreTextChanges = false;
			}
			for (int i = 0; i < 16; i++)
			{
				if (this.m_lineCheckboxes[i].IsClicked)
				{
					this.m_tmpTruthTableData.Data[i] = ((this.m_tmpTruthTableData.Data[i] == 0) ? 15 : 0);
				}
				this.m_lineCheckboxes[i].IsChecked = (this.m_tmpTruthTableData.Data[i] > 0);
			}
			if (this.m_linearPanel.IsVisible)
			{
				this.m_switchViewButton.Text = LanguageControl.Get(base.GetType().Name, 1);
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = false;
					this.m_gridPanel.IsVisible = true;
				}
			}
			else
			{
				this.m_switchViewButton.Text = LanguageControl.Get(base.GetType().Name, 2);
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = true;
					this.m_gridPanel.IsVisible = false;
				}
			}
			if (this.m_okButton.IsClicked)
			{
				this.m_truthTableData.Data = this.m_tmpTruthTableData.Data;
				this.Dismiss(true);
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00098FE4 File Offset: 0x000971E4
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			Action<bool> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(result);
		}

		// Token: 0x04000D19 RID: 3353
		public Action<bool> m_handler;

		// Token: 0x04000D1A RID: 3354
		public Widget m_linearPanel;

		// Token: 0x04000D1B RID: 3355
		public Widget m_gridPanel;

		// Token: 0x04000D1C RID: 3356
		public ButtonWidget m_okButton;

		// Token: 0x04000D1D RID: 3357
		public ButtonWidget m_cancelButton;

		// Token: 0x04000D1E RID: 3358
		public ButtonWidget m_switchViewButton;

		// Token: 0x04000D1F RID: 3359
		public CheckboxWidget[] m_lineCheckboxes = new CheckboxWidget[16];

		// Token: 0x04000D20 RID: 3360
		public TextBoxWidget m_linearTextBox;

		// Token: 0x04000D21 RID: 3361
		public TruthTableData m_truthTableData;

		// Token: 0x04000D22 RID: 3362
		public TruthTableData m_tmpTruthTableData;

		// Token: 0x04000D23 RID: 3363
		public bool m_ignoreTextChanges;
	}
}
