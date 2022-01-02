using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002BB RID: 699
	public class LevelFactorDialog : Dialog
	{
		// Token: 0x06001579 RID: 5497 RVA: 0x000A1BCC File Offset: 0x0009FDCC
		public LevelFactorDialog(string title, string description, IEnumerable<ComponentLevel.Factor> factors, float total)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/LevelFactorDialog", null);
			base.LoadContents(this, node);
			this.m_titleWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Title", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Description", true);
			this.m_namesWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Names", true);
			this.m_valuesWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Values", true);
			this.m_totalNameWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.TotalName", true);
			this.m_totalValueWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.TotalValue", true);
			this.m_okWidget = this.Children.Find<ButtonWidget>("LevelFactorDialog.OK", true);
			this.m_titleWidget.Text = title;
			this.m_descriptionWidget.Text = description;
			this.m_namesWidget.Text = string.Empty;
			this.m_valuesWidget.Text = string.Empty;
			foreach (ComponentLevel.Factor factor in factors)
			{
				LabelWidget namesWidget = this.m_namesWidget;
				namesWidget.Text += string.Format("{0,24}\n", factor.Description);
				LabelWidget valuesWidget = this.m_valuesWidget;
				valuesWidget.Text += string.Format(CultureInfo.InvariantCulture, "x {0:0.00}\n", factor.Value);
			}
			this.m_namesWidget.Text = this.m_namesWidget.Text.TrimEnd(Array.Empty<char>());
			this.m_valuesWidget.Text = this.m_valuesWidget.Text.TrimEnd(Array.Empty<char>());
			this.m_totalNameWidget.Text = string.Format("{0,24}", "TOTAL");
			this.m_totalValueWidget.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", total);
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x000A1DD4 File Offset: 0x0009FFD4
		public override void Update()
		{
			if (base.Input.Cancel || this.m_okWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000DF9 RID: 3577
		public LabelWidget m_titleWidget;

		// Token: 0x04000DFA RID: 3578
		public LabelWidget m_descriptionWidget;

		// Token: 0x04000DFB RID: 3579
		public LabelWidget m_namesWidget;

		// Token: 0x04000DFC RID: 3580
		public LabelWidget m_valuesWidget;

		// Token: 0x04000DFD RID: 3581
		public LabelWidget m_totalNameWidget;

		// Token: 0x04000DFE RID: 3582
		public LabelWidget m_totalValueWidget;

		// Token: 0x04000DFF RID: 3583
		public ButtonWidget m_okWidget;
	}
}
