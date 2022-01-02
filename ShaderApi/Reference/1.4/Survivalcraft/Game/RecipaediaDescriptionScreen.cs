using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200016D RID: 365
	public class RecipaediaDescriptionScreen : Screen
	{
		// Token: 0x06000845 RID: 2117 RVA: 0x000316F8 File Offset: 0x0002F8F8
		public RecipaediaDescriptionScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/RecipaediaDescriptionScreen", null);
			base.LoadContents(this, node);
			this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("Icon", true);
			this.m_nameWidget = this.Children.Find<LabelWidget>("Name", true);
			this.m_leftButtonWidget = this.Children.Find<ButtonWidget>("Left", true);
			this.m_rightButtonWidget = this.Children.Find<ButtonWidget>("Right", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("Description", true);
			this.m_propertyNames1Widget = this.Children.Find<LabelWidget>("PropertyNames1", true);
			this.m_propertyValues1Widget = this.Children.Find<LabelWidget>("PropertyValues1", true);
			this.m_propertyNames2Widget = this.Children.Find<LabelWidget>("PropertyNames2", true);
			this.m_propertyValues2Widget = this.Children.Find<LabelWidget>("PropertyValues2", true);
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x000317F0 File Offset: 0x0002F9F0
		public override void Enter(object[] parameters)
		{
			int item = (int)parameters[0];
			this.m_valuesList = (IList<int>)parameters[1];
			this.m_index = this.m_valuesList.IndexOf(item);
			this.UpdateBlockProperties();
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0003182C File Offset: 0x0002FA2C
		public override void Update()
		{
			this.m_leftButtonWidget.IsEnabled = (this.m_index > 0);
			this.m_rightButtonWidget.IsEnabled = (this.m_index < this.m_valuesList.Count - 1);
			if (this.m_leftButtonWidget.IsClicked || base.Input.Left)
			{
				this.m_index = MathUtils.Max(this.m_index - 1, 0);
				this.UpdateBlockProperties();
			}
			if (this.m_rightButtonWidget.IsClicked || base.Input.Right)
			{
				this.m_index = MathUtils.Min(this.m_index + 1, this.m_valuesList.Count - 1);
				this.UpdateBlockProperties();
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x00031924 File Offset: 0x0002FB24
		public Dictionary<string, string> GetBlockProperties(int value)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.GetEmittedLightAmount(value) > 0)
			{
				dictionary.Add("Luminosity", block.GetEmittedLightAmount(value).ToString());
			}
			if (block.GetFuelFireDuration(value) > 0f)
			{
				dictionary.Add("Fuel Value", block.GetFuelFireDuration(value).ToString());
			}
			dictionary.Add("Is Stackable", (block.GetMaxStacking(value) > 1) ? string.Format(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 1), block.GetMaxStacking(value).ToString()) : LanguageControl.No);
			dictionary.Add("Is Flammable", (block.GetFireDuration(value) > 0f) ? LanguageControl.Yes : LanguageControl.No);
			if (block.GetNutritionalValue(value) > 0f)
			{
				dictionary.Add("Nutrition", block.GetNutritionalValue(value).ToString());
			}
			if (block.GetRotPeriod(value) > 0)
			{
				dictionary.Add("Max Storage Time", string.Format(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 2), string.Format("{0:0.0}", (float)(2 * block.GetRotPeriod(value)) * 60f / 1200f)));
			}
			if (block.DigMethod != BlockDigMethod.None)
			{
				dictionary.Add("Digging Method", LanguageControl.Get(new string[]
				{
					"DigMethod",
					block.DigMethod.ToString()
				}));
				dictionary.Add("Digging Resilience", block.DigResilience.ToString());
			}
			if (block.ExplosionResilience > 0f)
			{
				dictionary.Add("Explosion Resilience", block.ExplosionResilience.ToString());
			}
			if (block.GetExplosionPressure(value) > 0f)
			{
				dictionary.Add("Explosive Power", block.GetExplosionPressure(value).ToString());
			}
			bool flag = false;
			if (block.GetMeleePower(value) > 1f)
			{
				dictionary.Add("Melee Power", block.GetMeleePower(value).ToString());
				flag = true;
			}
			if (block.GetMeleePower(value) > 1f)
			{
				dictionary.Add("Melee Hit Ratio", string.Format("{0:0}%", 100f * block.GetMeleeHitProbability(value)));
				flag = true;
			}
			if (block.GetProjectilePower(value) > 1f)
			{
				dictionary.Add("Projectile Power", block.GetProjectilePower(value).ToString());
				flag = true;
			}
			if (block.ShovelPower > 1f)
			{
				dictionary.Add("Shoveling", block.ShovelPower.ToString());
				flag = true;
			}
			if (block.HackPower > 1f)
			{
				dictionary.Add("Hacking", block.HackPower.ToString());
				flag = true;
			}
			if (block.QuarryPower > 1f)
			{
				dictionary.Add("Quarrying", block.QuarryPower.ToString());
				flag = true;
			}
			if (flag && block.Durability > 0)
			{
				dictionary.Add("Durability", block.Durability.ToString());
			}
			if (block.DefaultExperienceCount > 0f)
			{
				dictionary.Add("Experience Orbs", block.DefaultExperienceCount.ToString());
			}
			if (block.CanWear(value))
			{
				ClothingData clothingData = block.GetClothingData(value);
				dictionary.Add("Can Be Dyed", clothingData.CanBeDyed ? LanguageControl.Yes : LanguageControl.No);
				dictionary.Add("Armor Protection", string.Format("{0}%", (int)(clothingData.ArmorProtection * 100f)));
				dictionary.Add("Armor Durability", clothingData.Sturdiness.ToString());
				dictionary.Add("Insulation", string.Format("{0:0.0} clo", clothingData.Insulation));
				dictionary.Add("Movement Speed", string.Format("{0:0}%", clothingData.MovementSpeedFactor * 100f));
			}
			return dictionary;
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x00031D04 File Offset: 0x0002FF04
		public void UpdateBlockProperties()
		{
			if (this.m_index >= 0 && this.m_index < this.m_valuesList.Count)
			{
				int value = this.m_valuesList[this.m_index];
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				this.m_blockIconWidget.Value = value;
				this.m_nameWidget.Text = block.GetDisplayName(null, value);
				this.m_descriptionWidget.Text = block.GetDescription(value);
				this.m_propertyNames1Widget.Text = string.Empty;
				this.m_propertyValues1Widget.Text = string.Empty;
				this.m_propertyNames2Widget.Text = string.Empty;
				this.m_propertyValues2Widget.Text = string.Empty;
				Dictionary<string, string> blockProperties = this.GetBlockProperties(value);
				int num2 = 0;
				foreach (KeyValuePair<string, string> keyValuePair in blockProperties)
				{
					if (num2 < blockProperties.Count - blockProperties.Count / 2)
					{
						LabelWidget propertyNames1Widget = this.m_propertyNames1Widget;
						propertyNames1Widget.Text = propertyNames1Widget.Text + LanguageControl.Get(new string[]
						{
							RecipaediaDescriptionScreen.fName,
							keyValuePair.Key
						}) + ":\n";
						LabelWidget propertyValues1Widget = this.m_propertyValues1Widget;
						propertyValues1Widget.Text = propertyValues1Widget.Text + keyValuePair.Value + "\n";
					}
					else
					{
						LabelWidget propertyNames2Widget = this.m_propertyNames2Widget;
						propertyNames2Widget.Text = propertyNames2Widget.Text + LanguageControl.Get(new string[]
						{
							RecipaediaDescriptionScreen.fName,
							keyValuePair.Key
						}) + ":\n";
						LabelWidget propertyValues2Widget = this.m_propertyValues2Widget;
						propertyValues2Widget.Text = propertyValues2Widget.Text + keyValuePair.Value + "\n";
					}
					num2++;
				}
			}
		}

		// Token: 0x040003B0 RID: 944
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x040003B1 RID: 945
		public LabelWidget m_nameWidget;

		// Token: 0x040003B2 RID: 946
		public ButtonWidget m_leftButtonWidget;

		// Token: 0x040003B3 RID: 947
		public ButtonWidget m_rightButtonWidget;

		// Token: 0x040003B4 RID: 948
		public LabelWidget m_descriptionWidget;

		// Token: 0x040003B5 RID: 949
		public LabelWidget m_propertyNames1Widget;

		// Token: 0x040003B6 RID: 950
		public LabelWidget m_propertyValues1Widget;

		// Token: 0x040003B7 RID: 951
		public LabelWidget m_propertyNames2Widget;

		// Token: 0x040003B8 RID: 952
		public LabelWidget m_propertyValues2Widget;

		// Token: 0x040003B9 RID: 953
		public int m_index;

		// Token: 0x040003BA RID: 954
		public IList<int> m_valuesList;

		// Token: 0x040003BB RID: 955
		public static string fName = "RecipaediaDescriptionScreen";
	}
}
