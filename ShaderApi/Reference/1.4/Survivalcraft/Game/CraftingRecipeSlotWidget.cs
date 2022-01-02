using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200037B RID: 891
	public class CraftingRecipeSlotWidget : CanvasWidget
	{
		// Token: 0x06001A7A RID: 6778 RVA: 0x000CDEBC File Offset: 0x000CC0BC
		public CraftingRecipeSlotWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipeSlot", null);
			base.LoadContents(this, node);
			this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("CraftingRecipeSlotWidget.Icon", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("CraftingRecipeSlotWidget.Count", true);
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x000CDF11 File Offset: 0x000CC111
		public void SetIngredient(string ingredient)
		{
			this.m_ingredient = ingredient;
			this.m_resultValue = 0;
			this.m_resultCount = 0;
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x000CDF28 File Offset: 0x000CC128
		public void SetResult(int value, int count)
		{
			this.m_resultValue = value;
			this.m_resultCount = count;
			this.m_ingredient = null;
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x000CDF40 File Offset: 0x000CC140
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_blockIconWidget.IsVisible = false;
			this.m_labelWidget.IsVisible = false;
			if (!string.IsNullOrEmpty(this.m_ingredient))
			{
				string craftingId;
				int? num;
				CraftingRecipesManager.DecodeIngredient(this.m_ingredient, out craftingId, out num);
				Block[] array = BlocksManager.FindBlocksByCraftingId(craftingId);
				if (array.Length != 0)
				{
					Block block = array[(int)(1.0 * Time.RealTime) % array.Length];
					if (block != null)
					{
						this.m_blockIconWidget.Value = Terrain.MakeBlockValue(block.BlockIndex, 0, (num != null) ? num.Value : 4);
						this.m_blockIconWidget.Light = 15;
						this.m_blockIconWidget.IsVisible = true;
					}
				}
			}
			else if (this.m_resultValue != 0)
			{
				this.m_blockIconWidget.Value = this.m_resultValue;
				this.m_blockIconWidget.Light = 15;
				this.m_labelWidget.Text = this.m_resultCount.ToString();
				this.m_blockIconWidget.IsVisible = true;
				this.m_labelWidget.IsVisible = true;
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040011F7 RID: 4599
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x040011F8 RID: 4600
		public LabelWidget m_labelWidget;

		// Token: 0x040011F9 RID: 4601
		public string m_ingredient;

		// Token: 0x040011FA RID: 4602
		public int m_resultValue;

		// Token: 0x040011FB RID: 4603
		public int m_resultCount;
	}
}
