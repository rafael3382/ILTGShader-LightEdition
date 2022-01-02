using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F6 RID: 502
	public class ComponentCraftingTable : ComponentInventoryBase
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000E4A RID: 3658 RVA: 0x00069A5B File Offset: 0x00067C5B
		public int RemainsSlotIndex
		{
			get
			{
				return this.SlotsCount - 1;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00069A65 File Offset: 0x00067C65
		public int ResultSlotIndex
		{
			get
			{
				return this.SlotsCount - 2;
			}
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x00069A6F File Offset: 0x00067C6F
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex < this.SlotsCount - 2)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x00069A86 File Offset: 0x00067C86
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			base.AddSlotItems(slotIndex, value, count);
			this.UpdateCraftingResult();
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x00069A98 File Offset: 0x00067C98
		public override int RemoveSlotItems(int slotIndex, int count)
		{
			int num = 0;
			if (slotIndex == this.ResultSlotIndex)
			{
				if (this.m_matchedRecipe != null)
				{
					if (this.m_matchedRecipe.RemainsValue != 0 && this.m_matchedRecipe.RemainsCount > 0)
					{
						if (this.m_slots[this.RemainsSlotIndex].Count == 0 || this.m_slots[this.RemainsSlotIndex].Value == this.m_matchedRecipe.RemainsValue)
						{
							int num2 = BlocksManager.Blocks[Terrain.ExtractContents(this.m_matchedRecipe.RemainsValue)].GetMaxStacking(this.m_matchedRecipe.RemainsValue) - this.m_slots[this.RemainsSlotIndex].Count;
							count = MathUtils.Min(count, num2 / this.m_matchedRecipe.RemainsCount * this.m_matchedRecipe.ResultCount);
						}
						else
						{
							count = 0;
						}
					}
					count = count / this.m_matchedRecipe.ResultCount * this.m_matchedRecipe.ResultCount;
					num = base.RemoveSlotItems(slotIndex, count);
					if (num > 0)
					{
						for (int i = 0; i < 9; i++)
						{
							if (!string.IsNullOrEmpty(this.m_matchedIngredients[i]))
							{
								int index = i % 3 + this.m_craftingGridSize * (i / 3);
								this.m_slots[index].Count = MathUtils.Max(this.m_slots[index].Count - num / this.m_matchedRecipe.ResultCount, 0);
							}
						}
						if (this.m_matchedRecipe.RemainsValue != 0 && this.m_matchedRecipe.RemainsCount > 0)
						{
							this.m_slots[this.RemainsSlotIndex].Value = this.m_matchedRecipe.RemainsValue;
							this.m_slots[this.RemainsSlotIndex].Count += num / this.m_matchedRecipe.ResultCount * this.m_matchedRecipe.RemainsCount;
						}
						ComponentPlayer componentPlayer = base.FindInteractingPlayer();
						if (componentPlayer != null && componentPlayer.PlayerStats != null)
						{
							componentPlayer.PlayerStats.ItemsCrafted += (long)num;
						}
					}
				}
			}
			else
			{
				num = base.RemoveSlotItems(slotIndex, count);
			}
			this.UpdateCraftingResult();
			return num;
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00069CB7 File Offset: 0x00067EB7
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_craftingGridSize = (int)MathUtils.Sqrt((float)(this.SlotsCount - 2));
			this.UpdateCraftingResult();
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00069CDC File Offset: 0x00067EDC
		public virtual void UpdateCraftingResult()
		{
			int num = int.MaxValue;
			for (int i = 0; i < this.m_craftingGridSize; i++)
			{
				for (int j = 0; j < this.m_craftingGridSize; j++)
				{
					int num2 = i + j * 3;
					int slotIndex = i + j * this.m_craftingGridSize;
					int slotValue = this.GetSlotValue(slotIndex);
					int num3 = Terrain.ExtractContents(slotValue);
					int num4 = Terrain.ExtractData(slotValue);
					int slotCount = this.GetSlotCount(slotIndex);
					if (slotCount > 0)
					{
						Block block = BlocksManager.Blocks[num3];
						this.m_matchedIngredients[num2] = block.GetCraftingId(slotValue) + ":" + num4.ToString(CultureInfo.InvariantCulture);
						num = MathUtils.Min(num, slotCount);
					}
					else
					{
						this.m_matchedIngredients[num2] = null;
					}
				}
			}
			ComponentPlayer componentPlayer = base.FindInteractingPlayer();
			float playerLevel = (componentPlayer != null) ? componentPlayer.PlayerData.Level : 1f;
			CraftingRecipe craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(base.Project.FindSubsystem<SubsystemTerrain>(true), this.m_matchedIngredients, 0f, playerLevel);
			if (craftingRecipe != null && craftingRecipe.ResultValue != 0)
			{
				this.m_matchedRecipe = craftingRecipe;
				this.m_slots[this.ResultSlotIndex].Value = craftingRecipe.ResultValue;
				this.m_slots[this.ResultSlotIndex].Count = craftingRecipe.ResultCount * num;
			}
			else
			{
				this.m_matchedRecipe = null;
				this.m_slots[this.ResultSlotIndex].Value = 0;
				this.m_slots[this.ResultSlotIndex].Count = 0;
			}
			if (craftingRecipe != null && !string.IsNullOrEmpty(craftingRecipe.Message) && componentPlayer != null)
			{
				componentPlayer.ComponentGui.DisplaySmallMessage(craftingRecipe.Message, Color.White, true, true);
			}
		}

		// Token: 0x040007B4 RID: 1972
		public int m_craftingGridSize;

		// Token: 0x040007B5 RID: 1973
		public string[] m_matchedIngredients = new string[9];

		// Token: 0x040007B6 RID: 1974
		public CraftingRecipe m_matchedRecipe;
	}
}
