using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200027B RID: 635
	public class EditPaletteDialog : Dialog
	{
		// Token: 0x0600144E RID: 5198 RVA: 0x00097DE0 File Offset: 0x00095FE0
		public EditPaletteDialog(WorldPalette palette)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditPaletteDialog", null);
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ContainerWidget>("EditPaletteDialog.ListPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditPaletteDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditPaletteDialog.Cancel", true);
			for (int i = 0; i < 16; i++)
			{
				StackPanelWidget stackPanelWidget = new StackPanelWidget();
				stackPanelWidget.Direction = LayoutDirection.Horizontal;
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(32f, 60f),
					Children = 
					{
						new LabelWidget
						{
							Text = (i + 1).ToString() + ".",
							Color = Color.Gray,
							HorizontalAlignment = WidgetAlignment.Far,
							VerticalAlignment = WidgetAlignment.Center
						}
					}
				});
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children = stackPanelWidget.Children;
				LinkWidget[] labels = this.m_labels;
				int num = i;
				LinkWidget linkWidget = new LinkWidget();
				linkWidget.Size = new Vector2(300f, -1f);
				linkWidget.VerticalAlignment = WidgetAlignment.Center;
				LinkWidget widget = linkWidget;
				labels[num] = linkWidget;
				children.Add(widget);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children2 = stackPanelWidget.Children;
				BevelledButtonWidget[] rectangles = this.m_rectangles;
				int num2 = i;
				BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
				bevelledButtonWidget.Size = new Vector2(float.PositiveInfinity, 60f);
				bevelledButtonWidget.BevelSize = 1f;
				bevelledButtonWidget.AmbientLight = 1f;
				bevelledButtonWidget.DirectionalLight = 0.4f;
				bevelledButtonWidget.VerticalAlignment = WidgetAlignment.Center;
				BevelledButtonWidget widget2 = bevelledButtonWidget;
				rectangles[num2] = bevelledButtonWidget;
				children2.Add(widget2);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				WidgetsList children3 = stackPanelWidget.Children;
				ButtonWidget[] resetButtons = this.m_resetButtons;
				int num3 = i;
				BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
				bevelledButtonWidget2.Size = new Vector2(160f, 60f);
				bevelledButtonWidget2.VerticalAlignment = WidgetAlignment.Center;
				bevelledButtonWidget2.Text = LanguageControl.Get(base.GetType().Name, 1);
				ButtonWidget widget3 = bevelledButtonWidget2;
				resetButtons[num3] = bevelledButtonWidget2;
				children3.Add(widget3);
				stackPanelWidget.Children.Add(new CanvasWidget
				{
					Size = new Vector2(10f, 0f)
				});
				StackPanelWidget widget4 = stackPanelWidget;
				this.m_listPanel.Children.Add(widget4);
			}
			this.m_palette = palette;
			this.m_tmpPalette = new WorldPalette();
			this.m_palette.CopyTo(this.m_tmpPalette);
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x000980A8 File Offset: 0x000962A8
		public override void Update()
		{
			for (int k = 0; k < 16; k++)
			{
				this.m_labels[k].Text = this.m_tmpPalette.Names[k];
				this.m_rectangles[k].CenterColor = this.m_tmpPalette.Colors[k];
				this.m_resetButtons[k].IsEnabled = (this.m_tmpPalette.Colors[k] != WorldPalette.DefaultColors[k] || this.m_tmpPalette.Names[k] != LanguageControl.GetWorldPalette(k));
			}
			for (int j = 0; j < 16; j++)
			{
				int i = j;
				if (this.m_labels[j].IsClicked)
				{
					DialogsManager.ShowDialog(this, new TextBoxDialog(LanguageControl.Get(base.GetType().Name, 2), this.m_labels[j].Text, 16, delegate(string s)
					{
						if (s != null)
						{
							if (WorldPalette.VerifyColorName(s))
							{
								this.m_tmpPalette.Names[i] = s;
								return;
							}
							DialogsManager.ShowDialog(this, new MessageDialog(LanguageControl.Get(this.GetType().Name, 3), null, LanguageControl.Ok, null, null));
						}
					}));
				}
				if (this.m_rectangles[j].IsClicked)
				{
					DialogsManager.ShowDialog(this, new EditColorDialog(this.m_tmpPalette.Colors[j], delegate(Color? color)
					{
						if (color != null)
						{
							this.m_tmpPalette.Colors[i] = color.Value;
						}
					}));
				}
				if (this.m_resetButtons[j].IsClicked)
				{
					this.m_tmpPalette.Colors[j] = WorldPalette.DefaultColors[j];
					this.m_tmpPalette.Names[j] = LanguageControl.GetWorldPalette(j);
				}
			}
			if (this.m_okButton.IsClicked)
			{
				this.m_tmpPalette.CopyTo(this.m_palette);
				this.Dismiss();
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss();
			}
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x00098271 File Offset: 0x00096471
		public void Dismiss()
		{
			DialogsManager.HideDialog(this);
		}

		// Token: 0x04000CF0 RID: 3312
		public ContainerWidget m_listPanel;

		// Token: 0x04000CF1 RID: 3313
		public ButtonWidget m_okButton;

		// Token: 0x04000CF2 RID: 3314
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CF3 RID: 3315
		public LinkWidget[] m_labels = new LinkWidget[16];

		// Token: 0x04000CF4 RID: 3316
		public BevelledButtonWidget[] m_rectangles = new BevelledButtonWidget[16];

		// Token: 0x04000CF5 RID: 3317
		public ButtonWidget[] m_resetButtons = new ButtonWidget[16];

		// Token: 0x04000CF6 RID: 3318
		public WorldPalette m_palette;

		// Token: 0x04000CF7 RID: 3319
		public WorldPalette m_tmpPalette;
	}
}
