using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200015C RID: 348
	public class BestiaryDescriptionScreen : Screen
	{
		// Token: 0x060007AB RID: 1963 RVA: 0x0002B988 File Offset: 0x00029B88
		public BestiaryDescriptionScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/BestiaryDescriptionScreen", null);
			base.LoadContents(this, node);
			this.m_modelWidget = this.Children.Find<ModelWidget>("Model", true);
			this.m_nameWidget = this.Children.Find<LabelWidget>("Name", true);
			this.m_leftButtonWidget = this.Children.Find<ButtonWidget>("Left", true);
			this.m_rightButtonWidget = this.Children.Find<ButtonWidget>("Right", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("Description", true);
			this.m_propertyNames1Widget = this.Children.Find<LabelWidget>("PropertyNames1", true);
			this.m_propertyValues1Widget = this.Children.Find<LabelWidget>("PropertyValues1", true);
			this.m_propertyNames2Widget = this.Children.Find<LabelWidget>("PropertyNames2", true);
			this.m_propertyValues2Widget = this.Children.Find<LabelWidget>("PropertyValues2", true);
			this.m_dropsPanel = this.Children.Find<ContainerWidget>("Drops", true);
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0002BA98 File Offset: 0x00029C98
		public override void Enter(object[] parameters)
		{
			BestiaryCreatureInfo item = (BestiaryCreatureInfo)parameters[0];
			this.m_infoList = (IList<BestiaryCreatureInfo>)parameters[1];
			this.m_index = this.m_infoList.IndexOf(item);
			this.UpdateCreatureProperties();
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0002BAD4 File Offset: 0x00029CD4
		public override void Update()
		{
			this.m_leftButtonWidget.IsEnabled = (this.m_index > 0);
			this.m_rightButtonWidget.IsEnabled = (this.m_index < this.m_infoList.Count - 1);
			if (this.m_leftButtonWidget.IsClicked || base.Input.Left)
			{
				this.m_index = MathUtils.Max(this.m_index - 1, 0);
				this.UpdateCreatureProperties();
			}
			if (this.m_rightButtonWidget.IsClicked || base.Input.Right)
			{
				this.m_index = MathUtils.Min(this.m_index + 1, this.m_infoList.Count - 1);
				this.UpdateCreatureProperties();
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0002BBCC File Offset: 0x00029DCC
		public void UpdateCreatureProperties()
		{
			if (this.m_index >= 0 && this.m_index < this.m_infoList.Count)
			{
				BestiaryCreatureInfo bestiaryCreatureInfo = this.m_infoList[this.m_index];
				this.m_modelWidget.AutoRotationVector = new Vector3(0f, 1f, 0f);
				BestiaryScreen.SetupBestiaryModelWidget(bestiaryCreatureInfo, this.m_modelWidget, new Vector3(-1f, 0f, -1f), true, true);
				this.m_nameWidget.Text = bestiaryCreatureInfo.DisplayName;
				this.m_descriptionWidget.Text = bestiaryCreatureInfo.Description;
				this.m_propertyNames1Widget.Text = string.Empty;
				this.m_propertyValues1Widget.Text = string.Empty;
				LabelWidget propertyNames1Widget = this.m_propertyNames1Widget;
				propertyNames1Widget.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"resilience"
				});
				LabelWidget propertyValues1Widget = this.m_propertyValues1Widget;
				propertyValues1Widget.Text = propertyValues1Widget.Text + bestiaryCreatureInfo.AttackResilience.ToString() + "\n";
				LabelWidget propertyNames1Widget2 = this.m_propertyNames1Widget;
				propertyNames1Widget2.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"attack"
				});
				LabelWidget propertyValues1Widget2 = this.m_propertyValues1Widget;
				propertyValues1Widget2.Text = propertyValues1Widget2.Text + ((bestiaryCreatureInfo.AttackPower > 0f) ? bestiaryCreatureInfo.AttackPower.ToString("0.0") : LanguageControl.None) + "\n";
				LabelWidget propertyNames1Widget3 = this.m_propertyNames1Widget;
				propertyNames1Widget3.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"herding"
				});
				LabelWidget propertyValues1Widget3 = this.m_propertyValues1Widget;
				propertyValues1Widget3.Text = propertyValues1Widget3.Text + (bestiaryCreatureInfo.IsHerding ? LanguageControl.Yes : LanguageControl.No) + "\n";
				LabelWidget propertyNames1Widget4 = this.m_propertyNames1Widget;
				propertyNames1Widget4.Text += LanguageControl.Get(base.GetType().Name, 1);
				LabelWidget propertyValues1Widget4 = this.m_propertyValues1Widget;
				propertyValues1Widget4.Text = propertyValues1Widget4.Text + (bestiaryCreatureInfo.CanBeRidden ? LanguageControl.Yes : LanguageControl.No) + "\n";
				this.m_propertyNames1Widget.Text = this.m_propertyNames1Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyValues1Widget.Text = this.m_propertyValues1Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyNames2Widget.Text = string.Empty;
				this.m_propertyValues2Widget.Text = string.Empty;
				LabelWidget propertyNames2Widget = this.m_propertyNames2Widget;
				propertyNames2Widget.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"speed"
				});
				LabelWidget propertyValues2Widget = this.m_propertyValues2Widget;
				propertyValues2Widget.Text = propertyValues2Widget.Text + ((double)bestiaryCreatureInfo.MovementSpeed * 3.6).ToString("0") + LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"speed unit"
				});
				LabelWidget propertyNames2Widget2 = this.m_propertyNames2Widget;
				propertyNames2Widget2.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"jump height"
				});
				LabelWidget propertyValues2Widget2 = this.m_propertyValues2Widget;
				propertyValues2Widget2.Text = propertyValues2Widget2.Text + bestiaryCreatureInfo.JumpHeight.ToString("0.0") + LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"length unit"
				});
				LabelWidget propertyNames2Widget3 = this.m_propertyNames2Widget;
				propertyNames2Widget3.Text += LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"weight"
				});
				LabelWidget propertyValues2Widget3 = this.m_propertyValues2Widget;
				propertyValues2Widget3.Text = propertyValues2Widget3.Text + bestiaryCreatureInfo.Mass.ToString() + LanguageControl.Get(new string[]
				{
					base.GetType().Name,
					"weight unit"
				});
				LabelWidget propertyNames2Widget4 = this.m_propertyNames2Widget;
				propertyNames2Widget4.Text = propertyNames2Widget4.Text + LanguageControl.Get(new string[]
				{
					"BlocksManager",
					"Spawner Eggs"
				}) + ":";
				LabelWidget propertyValues2Widget4 = this.m_propertyValues2Widget;
				propertyValues2Widget4.Text = propertyValues2Widget4.Text + (bestiaryCreatureInfo.HasSpawnerEgg ? LanguageControl.Exists : LanguageControl.None) + "\n";
				this.m_propertyNames2Widget.Text = this.m_propertyNames2Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyValues2Widget.Text = this.m_propertyValues2Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_dropsPanel.Children.Clear();
				if (bestiaryCreatureInfo.Loot.Count > 0)
				{
					using (List<ComponentLoot.Loot>.Enumerator enumerator = bestiaryCreatureInfo.Loot.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ComponentLoot.Loot loot = enumerator.Current;
							string text = (loot.MinCount >= loot.MaxCount) ? string.Format("{0}", loot.MinCount) : string.Format(LanguageControl.Get(new string[]
							{
								base.GetType().Name,
								"range"
							}), loot.MinCount, loot.MaxCount);
							if (loot.Probability < 1f)
							{
								text += string.Format(LanguageControl.Get(base.GetType().Name, 2), string.Format("{0:0}", loot.Probability * 100f));
							}
							this.m_dropsPanel.Children.Add(new StackPanelWidget
							{
								Margin = new Vector2(20f, 0f),
								Children = 
								{
									new BlockIconWidget
									{
										Size = new Vector2(32f),
										Scale = 1.2f,
										VerticalAlignment = WidgetAlignment.Center,
										Value = loot.Value
									},
									new CanvasWidget
									{
										Size = new Vector2(10f, 0f)
									},
									new LabelWidget
									{
										VerticalAlignment = WidgetAlignment.Center,
										Text = text
									}
								}
							});
						}
						return;
					}
				}
				this.m_dropsPanel.Children.Add(new LabelWidget
				{
					Margin = new Vector2(20f, 0f),
					Text = LanguageControl.Nothing
				});
			}
		}

		// Token: 0x0400033C RID: 828
		public ModelWidget m_modelWidget;

		// Token: 0x0400033D RID: 829
		public LabelWidget m_nameWidget;

		// Token: 0x0400033E RID: 830
		public ButtonWidget m_leftButtonWidget;

		// Token: 0x0400033F RID: 831
		public ButtonWidget m_rightButtonWidget;

		// Token: 0x04000340 RID: 832
		public LabelWidget m_descriptionWidget;

		// Token: 0x04000341 RID: 833
		public LabelWidget m_propertyNames1Widget;

		// Token: 0x04000342 RID: 834
		public LabelWidget m_propertyValues1Widget;

		// Token: 0x04000343 RID: 835
		public LabelWidget m_propertyNames2Widget;

		// Token: 0x04000344 RID: 836
		public LabelWidget m_propertyValues2Widget;

		// Token: 0x04000345 RID: 837
		public ContainerWidget m_dropsPanel;

		// Token: 0x04000346 RID: 838
		public int m_index;

		// Token: 0x04000347 RID: 839
		public IList<BestiaryCreatureInfo> m_infoList;
	}
}
