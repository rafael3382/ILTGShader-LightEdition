using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020003A8 RID: 936
	public class VitalStatsWidget : CanvasWidget
	{
		// Token: 0x06001C9A RID: 7322 RVA: 0x000DD51C File Offset: 0x000DB71C
		public VitalStatsWidget(ComponentPlayer componentPlayer)
		{
			this.m_componentPlayer = componentPlayer;
			XElement node = ContentManager.Get<XElement>("Widgets/VitalStatsWidget", null);
			base.LoadContents(this, node);
			this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
			this.m_healthLink = this.Children.Find<LinkWidget>("HealthLink", true);
			this.m_healthValueBar = this.Children.Find<ValueBarWidget>("HealthValueBar", true);
			this.m_staminaLink = this.Children.Find<LinkWidget>("StaminaLink", true);
			this.m_staminaValueBar = this.Children.Find<ValueBarWidget>("StaminaValueBar", true);
			this.m_foodLink = this.Children.Find<LinkWidget>("FoodLink", true);
			this.m_foodValueBar = this.Children.Find<ValueBarWidget>("FoodValueBar", true);
			this.m_sleepLink = this.Children.Find<LinkWidget>("SleepLink", true);
			this.m_sleepValueBar = this.Children.Find<ValueBarWidget>("SleepValueBar", true);
			this.m_temperatureLink = this.Children.Find<LinkWidget>("TemperatureLink", true);
			this.m_temperatureValueBar = this.Children.Find<ValueBarWidget>("TemperatureValueBar", true);
			this.m_wetnessLink = this.Children.Find<LinkWidget>("WetnessLink", true);
			this.m_wetnessValueBar = this.Children.Find<ValueBarWidget>("WetnessValueBar", true);
			this.m_chokeButton = this.Children.Find<ButtonWidget>("ChokeButton", true);
			this.m_strengthLink = this.Children.Find<LinkWidget>("StrengthLink", true);
			this.m_strengthLabel = this.Children.Find<LabelWidget>("StrengthLabel", true);
			this.m_resilienceLink = this.Children.Find<LinkWidget>("ResilienceLink", true);
			this.m_resilienceLabel = this.Children.Find<LabelWidget>("ResilienceLabel", true);
			this.m_speedLink = this.Children.Find<LinkWidget>("SpeedLink", true);
			this.m_speedLabel = this.Children.Find<LabelWidget>("SpeedLabel", true);
			this.m_hungerLink = this.Children.Find<LinkWidget>("HungerLink", true);
			this.m_hungerLabel = this.Children.Find<LabelWidget>("HungerLabel", true);
			this.m_experienceLink = this.Children.Find<LinkWidget>("ExperienceLink", true);
			this.m_experienceValueBar = this.Children.Find<ValueBarWidget>("ExperienceValueBar", true);
			this.m_insulationLink = this.Children.Find<LinkWidget>("InsulationLink", true);
			this.m_insulationLabel = this.Children.Find<LabelWidget>("InsulationLabel", true);
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x000DD7A0 File Offset: 0x000DB9A0
		public override void Update()
		{
			this.m_titleLabel.Text = string.Format("{0}, Level {1} {2}", this.m_componentPlayer.PlayerData.Name, MathUtils.Floor(this.m_componentPlayer.PlayerData.Level), this.m_componentPlayer.PlayerData.PlayerClass.ToString());
			this.m_healthValueBar.Value = this.m_componentPlayer.ComponentHealth.Health;
			this.m_staminaValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Stamina;
			this.m_foodValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Food;
			this.m_sleepValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Sleep;
			this.m_temperatureValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Temperature / 24f;
			this.m_wetnessValueBar.Value = this.m_componentPlayer.ComponentVitalStats.Wetness;
			this.m_experienceValueBar.Value = this.m_componentPlayer.PlayerData.Level - MathUtils.Floor(this.m_componentPlayer.PlayerData.Level);
			this.m_strengthLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.StrengthFactor);
			this.m_resilienceLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.ResilienceFactor);
			this.m_speedLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.SpeedFactor);
			this.m_hungerLabel.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", this.m_componentPlayer.ComponentLevel.HungerFactor);
			this.m_insulationLabel.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00} clo", this.m_componentPlayer.ComponentClothing.Insulation);
			if (this.m_healthLink.IsClicked)
			{
				HelpTopic topic = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Health");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic.Title, topic.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_staminaLink.IsClicked)
			{
				HelpTopic topic2 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Stamina");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic2.Title, topic2.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_foodLink.IsClicked)
			{
				HelpTopic topic3 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Hunger");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic3.Title, topic3.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_sleepLink.IsClicked)
			{
				HelpTopic topic4 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Sleep");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic4.Title, topic4.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_temperatureLink.IsClicked)
			{
				HelpTopic topic5 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Temperature");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic5.Title, topic5.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_wetnessLink.IsClicked)
			{
				HelpTopic topic6 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Wetness");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic6.Title, topic6.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_strengthLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors = new List<ComponentLevel.Factor>();
				float total = this.m_componentPlayer.ComponentLevel.CalculateStrengthFactor(factors);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(base.GetType().Name, "Strength"), LanguageControl.GetContentWidgets(base.GetType().Name, 16), factors, total));
			}
			if (this.m_resilienceLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors2 = new List<ComponentLevel.Factor>();
				float total2 = this.m_componentPlayer.ComponentLevel.CalculateResilienceFactor(factors2);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(base.GetType().Name, "Resilience"), LanguageControl.GetContentWidgets(base.GetType().Name, 17), factors2, total2));
			}
			if (this.m_speedLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors3 = new List<ComponentLevel.Factor>();
				float total3 = this.m_componentPlayer.ComponentLevel.CalculateSpeedFactor(factors3);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(base.GetType().Name, "Speed"), LanguageControl.GetContentWidgets(base.GetType().Name, 18), factors3, total3));
			}
			if (this.m_hungerLink.IsClicked)
			{
				List<ComponentLevel.Factor> factors4 = new List<ComponentLevel.Factor>();
				float total4 = this.m_componentPlayer.ComponentLevel.CalculateHungerFactor(factors4);
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new LevelFactorDialog(LanguageControl.GetContentWidgets(base.GetType().Name, "Hunger"), LanguageControl.GetContentWidgets(base.GetType().Name, 19), factors4, total4));
			}
			if (this.m_experienceLink.IsClicked)
			{
				HelpTopic topic7 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Levels");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic7.Title, topic7.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_insulationLink.IsClicked)
			{
				HelpTopic topic8 = ScreensManager.FindScreen<HelpScreen>("Help").GetTopic("Clothing");
				DialogsManager.ShowDialog(this.m_componentPlayer.GuiWidget, new MessageDialog(topic8.Title, topic8.Text, LanguageControl.Ok, null, new Vector2(700f, 360f), null));
			}
			if (this.m_chokeButton.IsClicked)
			{
				this.m_componentPlayer.ComponentHealth.Injure(0.1f, null, true, LanguageControl.GetContentWidgets(base.GetType().Name, "Choked"));
			}
		}

		// Token: 0x04001343 RID: 4931
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04001344 RID: 4932
		public ButtonWidget m_chokeButton;

		// Token: 0x04001345 RID: 4933
		public LabelWidget m_titleLabel;

		// Token: 0x04001346 RID: 4934
		public LinkWidget m_healthLink;

		// Token: 0x04001347 RID: 4935
		public ValueBarWidget m_healthValueBar;

		// Token: 0x04001348 RID: 4936
		public LinkWidget m_staminaLink;

		// Token: 0x04001349 RID: 4937
		public ValueBarWidget m_staminaValueBar;

		// Token: 0x0400134A RID: 4938
		public LinkWidget m_foodLink;

		// Token: 0x0400134B RID: 4939
		public ValueBarWidget m_foodValueBar;

		// Token: 0x0400134C RID: 4940
		public LinkWidget m_sleepLink;

		// Token: 0x0400134D RID: 4941
		public ValueBarWidget m_sleepValueBar;

		// Token: 0x0400134E RID: 4942
		public LinkWidget m_temperatureLink;

		// Token: 0x0400134F RID: 4943
		public ValueBarWidget m_temperatureValueBar;

		// Token: 0x04001350 RID: 4944
		public LinkWidget m_wetnessLink;

		// Token: 0x04001351 RID: 4945
		public ValueBarWidget m_wetnessValueBar;

		// Token: 0x04001352 RID: 4946
		public LinkWidget m_strengthLink;

		// Token: 0x04001353 RID: 4947
		public LabelWidget m_strengthLabel;

		// Token: 0x04001354 RID: 4948
		public LinkWidget m_resilienceLink;

		// Token: 0x04001355 RID: 4949
		public LabelWidget m_resilienceLabel;

		// Token: 0x04001356 RID: 4950
		public LinkWidget m_speedLink;

		// Token: 0x04001357 RID: 4951
		public LabelWidget m_speedLabel;

		// Token: 0x04001358 RID: 4952
		public LinkWidget m_hungerLink;

		// Token: 0x04001359 RID: 4953
		public LabelWidget m_hungerLabel;

		// Token: 0x0400135A RID: 4954
		public LinkWidget m_experienceLink;

		// Token: 0x0400135B RID: 4955
		public ValueBarWidget m_experienceValueBar;

		// Token: 0x0400135C RID: 4956
		public LinkWidget m_insulationLink;

		// Token: 0x0400135D RID: 4957
		public LabelWidget m_insulationLabel;
	}
}
