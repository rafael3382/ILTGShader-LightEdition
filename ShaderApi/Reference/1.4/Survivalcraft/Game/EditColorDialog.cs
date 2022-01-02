using System;
using Engine;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000279 RID: 633
	public class EditColorDialog : Dialog
	{
		// Token: 0x06001444 RID: 5188 RVA: 0x00097238 File Offset: 0x00095438
		public EditColorDialog(Color color, Action<Color?> handler)
		{
			WidgetsList children = this.Children;
			CanvasWidget canvasWidget = new CanvasWidget
			{
				Size = new Vector2(660f, 420f),
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center,
				Children = 
				{
					new RectangleWidget
					{
						FillColor = new Color(0, 0, 0, 255),
						OutlineColor = new Color(128, 128, 128, 128),
						OutlineThickness = 2f
					}
				}
			};
			WidgetsList children2 = canvasWidget.Children;
			StackPanelWidget stackPanelWidget = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				Margin = new Vector2(15f),
				HorizontalAlignment = WidgetAlignment.Center,
				Children = 
				{
					new LabelWidget
					{
						Text = LanguageControl.Get(base.GetType().Name, 1),
						HorizontalAlignment = WidgetAlignment.Center
					},
					new CanvasWidget
					{
						Size = new Vector2(0f, float.PositiveInfinity)
					}
				}
			};
			WidgetsList children3 = stackPanelWidget.Children;
			StackPanelWidget stackPanelWidget2 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal
			};
			WidgetsList children4 = stackPanelWidget2.Children;
			StackPanelWidget stackPanelWidget3 = new StackPanelWidget
			{
				Direction = LayoutDirection.Vertical,
				VerticalAlignment = WidgetAlignment.Center
			};
			WidgetsList children5 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget4 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = LanguageControl.Get(base.GetType().Name, 2),
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children6 = stackPanelWidget4.Children;
			SliderWidget sliderWidget = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			SliderWidget widget = sliderWidget;
			this.m_sliderR = sliderWidget;
			children6.Add(widget);
			children5.Add(stackPanelWidget4);
			WidgetsList children7 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget5 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = LanguageControl.Get(base.GetType().Name, 3),
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children8 = stackPanelWidget5.Children;
			SliderWidget sliderWidget2 = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			widget = sliderWidget2;
			this.m_sliderG = sliderWidget2;
			children8.Add(widget);
			children7.Add(stackPanelWidget5);
			WidgetsList children9 = stackPanelWidget3.Children;
			StackPanelWidget stackPanelWidget6 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Far,
				Margin = new Vector2(0f, 10f),
				Children = 
				{
					new LabelWidget
					{
						Text = LanguageControl.Get(base.GetType().Name, 4),
						Color = Color.Gray,
						VerticalAlignment = WidgetAlignment.Center
					},
					new CanvasWidget
					{
						Size = new Vector2(10f, 0f)
					}
				}
			};
			WidgetsList children10 = stackPanelWidget6.Children;
			SliderWidget sliderWidget3 = new SliderWidget
			{
				Size = new Vector2(300f, 50f),
				IsLabelVisible = false,
				MinValue = 0f,
				MaxValue = 255f,
				Granularity = 1f,
				SoundName = ""
			};
			widget = sliderWidget3;
			this.m_sliderB = sliderWidget3;
			children10.Add(widget);
			children9.Add(stackPanelWidget6);
			children4.Add(stackPanelWidget3);
			stackPanelWidget2.Children.Add(new CanvasWidget
			{
				Size = new Vector2(20f, 0f)
			});
			WidgetsList children11 = stackPanelWidget2.Children;
			CanvasWidget canvasWidget2 = new CanvasWidget();
			WidgetsList children12 = canvasWidget2.Children;
			BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget
			{
				Size = new Vector2(200f, 240f),
				AmbientLight = 1f,
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			BevelledButtonWidget widget2 = bevelledButtonWidget;
			this.m_rectangle = bevelledButtonWidget;
			children12.Add(widget2);
			WidgetsList children13 = canvasWidget2.Children;
			LabelWidget labelWidget = new LabelWidget
			{
				HorizontalAlignment = WidgetAlignment.Center,
				VerticalAlignment = WidgetAlignment.Center
			};
			LabelWidget widget3 = labelWidget;
			this.m_label = labelWidget;
			children13.Add(widget3);
			children11.Add(canvasWidget2);
			children3.Add(stackPanelWidget2);
			stackPanelWidget.Children.Add(new CanvasWidget
			{
				Size = new Vector2(0f, float.PositiveInfinity)
			});
			WidgetsList children14 = stackPanelWidget.Children;
			StackPanelWidget stackPanelWidget7 = new StackPanelWidget
			{
				Direction = LayoutDirection.Horizontal,
				HorizontalAlignment = WidgetAlignment.Center
			};
			WidgetsList children15 = stackPanelWidget7.Children;
			BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Text = LanguageControl.Ok
			};
			ButtonWidget widget4 = bevelledButtonWidget2;
			this.m_okButton = bevelledButtonWidget2;
			children15.Add(widget4);
			stackPanelWidget7.Children.Add(new CanvasWidget
			{
				Size = new Vector2(50f, 0f)
			});
			WidgetsList children16 = stackPanelWidget7.Children;
			BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget
			{
				Size = new Vector2(160f, 60f),
				Text = LanguageControl.Cancel
			};
			widget4 = bevelledButtonWidget3;
			this.m_cancelButton = bevelledButtonWidget3;
			children16.Add(widget4);
			children14.Add(stackPanelWidget7);
			children2.Add(stackPanelWidget);
			children.Add(canvasWidget);
			this.m_handler = handler;
			this.m_color = color;
			this.UpdateControls();
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x00097840 File Offset: 0x00095A40
		public override void Update()
		{
			if (this.m_rectangle.IsClicked)
			{
				DialogsManager.ShowDialog(this, new TextBoxDialog(LanguageControl.Get(base.GetType().Name, 5), this.GetColorString(), 20, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							this.m_color.RGB = HumanReadableConverter.ConvertFromString<Color>(s);
						}
						catch
						{
							DialogsManager.ShowDialog(this, new MessageDialog(LanguageControl.Get(base.GetType().Name, 6), LanguageControl.Get(base.GetType().Name, 7), LanguageControl.Ok, null, null));
						}
					}
				}));
			}
			if (this.m_sliderR.IsSliding)
			{
				this.m_color.R = (byte)this.m_sliderR.Value;
			}
			if (this.m_sliderG.IsSliding)
			{
				this.m_color.G = (byte)this.m_sliderG.Value;
			}
			if (this.m_sliderB.IsSliding)
			{
				this.m_color.B = (byte)this.m_sliderB.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new Color?(this.m_color));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x00097944 File Offset: 0x00095B44
		public void UpdateControls()
		{
			this.m_rectangle.CenterColor = this.m_color;
			this.m_sliderR.Value = (float)this.m_color.R;
			this.m_sliderG.Value = (float)this.m_color.G;
			this.m_sliderB.Value = (float)this.m_color.B;
			this.m_label.Text = this.GetColorString();
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x000979B8 File Offset: 0x00095BB8
		public string GetColorString()
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", this.m_color.R, this.m_color.G, this.m_color.B);
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x000979F4 File Offset: 0x00095BF4
		public void Dismiss(Color? result)
		{
			DialogsManager.HideDialog(this);
			Action<Color?> handler = this.m_handler;
			if (handler == null)
			{
				return;
			}
			handler(result);
		}

		// Token: 0x04000CDC RID: 3292
		public BevelledButtonWidget m_rectangle;

		// Token: 0x04000CDD RID: 3293
		public SliderWidget m_sliderR;

		// Token: 0x04000CDE RID: 3294
		public SliderWidget m_sliderG;

		// Token: 0x04000CDF RID: 3295
		public SliderWidget m_sliderB;

		// Token: 0x04000CE0 RID: 3296
		public LabelWidget m_label;

		// Token: 0x04000CE1 RID: 3297
		public ButtonWidget m_okButton;

		// Token: 0x04000CE2 RID: 3298
		public ButtonWidget m_cancelButton;

		// Token: 0x04000CE3 RID: 3299
		public Action<Color?> m_handler;

		// Token: 0x04000CE4 RID: 3300
		public Color m_color;
	}
}
