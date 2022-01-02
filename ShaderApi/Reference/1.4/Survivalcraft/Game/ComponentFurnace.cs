using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020B RID: 523
	public class ComponentFurnace : ComponentInventoryBase, IUpdateable
	{
		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x000721F8 File Offset: 0x000703F8
		public int RemainsSlotIndex
		{
			get
			{
				return this.SlotsCount - 1;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000F5E RID: 3934 RVA: 0x00072202 File Offset: 0x00070402
		public int ResultSlotIndex
		{
			get
			{
				return this.SlotsCount - 2;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0007220C File Offset: 0x0007040C
		public int FuelSlotIndex
		{
			get
			{
				return this.SlotsCount - 3;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000F60 RID: 3936 RVA: 0x00072216 File Offset: 0x00070416
		public float HeatLevel
		{
			get
			{
				return this.m_heatLevel;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0007221E File Offset: 0x0007041E
		public float SmeltingProgress
		{
			get
			{
				return this.m_smeltingProgress;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000F62 RID: 3938 RVA: 0x00072226 File Offset: 0x00070426
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x00072229 File Offset: 0x00070429
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != this.FuelSlotIndex)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			if (BlocksManager.Blocks[Terrain.ExtractContents(value)].GetFuelHeatLevel(value) > 0f)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x00072260 File Offset: 0x00070460
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			this.m_updateSmeltingRecipe = true;
			base.AddSlotItems(slotIndex, value, count);
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x00072272 File Offset: 0x00070472
		public override int RemoveSlotItems(int slotIndex, int count)
		{
			this.m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00072284 File Offset: 0x00070484
		public void Update(float dt)
		{
			Point3 coordinates = this.m_componentBlockEntity.Coordinates;
			if (this.m_heatLevel > 0f)
			{
				this.m_fireTimeRemaining = MathUtils.Max(0f, this.m_fireTimeRemaining - dt);
				if (this.m_fireTimeRemaining == 0f)
				{
					this.m_heatLevel = 0f;
				}
			}
			if (this.m_updateSmeltingRecipe)
			{
				this.m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (this.m_heatLevel > 0f)
				{
					heatLevel = this.m_heatLevel;
				}
				else
				{
					ComponentInventoryBase.Slot slot = this.m_slots[this.FuelSlotIndex];
					if (slot.Count > 0)
					{
						int num = Terrain.ExtractContents(slot.Value);
						heatLevel = BlocksManager.Blocks[num].GetFuelHeatLevel(slot.Value);
					}
				}
				CraftingRecipe craftingRecipe = this.FindSmeltingRecipe(heatLevel);
				if (craftingRecipe != this.m_smeltingRecipe)
				{
					this.m_smeltingRecipe = ((craftingRecipe != null && craftingRecipe.ResultValue != 0) ? craftingRecipe : null);
					this.m_smeltingProgress = 0f;
				}
			}
			if (this.m_smeltingRecipe == null)
			{
				this.m_heatLevel = 0f;
				this.m_fireTimeRemaining = 0f;
			}
			if (this.m_smeltingRecipe != null && this.m_fireTimeRemaining <= 0f)
			{
				ComponentInventoryBase.Slot slot2 = this.m_slots[this.FuelSlotIndex];
				if (slot2.Count > 0)
				{
					int num2 = Terrain.ExtractContents(slot2.Value);
					Block block = BlocksManager.Blocks[num2];
					if (block.GetExplosionPressure(slot2.Value) > 0f)
					{
						slot2.Count = 0;
						this.m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if (block.GetFuelHeatLevel(slot2.Value) > 0f)
					{
						slot2.Count--;
						this.m_fireTimeRemaining = block.GetFuelFireDuration(slot2.Value);
						this.m_heatLevel = block.GetFuelHeatLevel(slot2.Value);
					}
				}
			}
			if (this.m_fireTimeRemaining <= 0f)
			{
				this.m_smeltingRecipe = null;
				this.m_smeltingProgress = 0f;
			}
			if (this.m_smeltingRecipe != null)
			{
				this.m_smeltingProgress = MathUtils.Min(this.m_smeltingProgress + 0.15f * dt, 1f);
				if (this.m_smeltingProgress >= 1f)
				{
					for (int i = 0; i < this.m_furnaceSize; i++)
					{
						if (this.m_slots[i].Count > 0)
						{
							this.m_slots[i].Count--;
						}
					}
					this.m_slots[this.ResultSlotIndex].Value = this.m_smeltingRecipe.ResultValue;
					this.m_slots[this.ResultSlotIndex].Count += this.m_smeltingRecipe.ResultCount;
					if (this.m_smeltingRecipe.RemainsValue != 0 && this.m_smeltingRecipe.RemainsCount > 0)
					{
						this.m_slots[this.RemainsSlotIndex].Value = this.m_smeltingRecipe.RemainsValue;
						this.m_slots[this.RemainsSlotIndex].Count += this.m_smeltingRecipe.RemainsCount;
					}
					this.m_smeltingRecipe = null;
					this.m_smeltingProgress = 0f;
					this.m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				this.m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, (this.m_heatLevel > 0f) ? 65 : 64), true);
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x00072664 File Offset: 0x00070864
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
			this.m_furnaceSize = this.SlotsCount - 3;
			if (this.m_furnaceSize < 1 || this.m_furnaceSize > 3)
			{
				throw new InvalidOperationException("Invalid furnace size.");
			}
			this.m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			this.m_heatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			this.m_updateSmeltingRecipe = true;
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00072703 File Offset: 0x00070903
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<float>("FireTimeRemaining", this.m_fireTimeRemaining);
			valuesDictionary.SetValue<float>("HeatLevel", this.m_heatLevel);
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x00072730 File Offset: 0x00070930
		public virtual CraftingRecipe FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel > 0f)
			{
				for (int i = 0; i < this.m_furnaceSize; i++)
				{
					int slotValue = this.GetSlotValue(i);
					int num = Terrain.ExtractContents(slotValue);
					int num2 = Terrain.ExtractData(slotValue);
					if (this.GetSlotCount(i) > 0)
					{
						Block block = BlocksManager.Blocks[num];
						this.m_matchedIngredients[i] = block.GetCraftingId(slotValue) + ":" + num2.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this.m_matchedIngredients[i] = null;
					}
				}
				ComponentPlayer componentPlayer = base.FindInteractingPlayer();
				float playerLevel = (componentPlayer != null) ? componentPlayer.PlayerData.Level : 1f;
				CraftingRecipe craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(this.m_subsystemTerrain, this.m_matchedIngredients, heatLevel, playerLevel);
				if (craftingRecipe != null && craftingRecipe.ResultValue != 0)
				{
					if (craftingRecipe.RequiredHeatLevel <= 0f)
					{
						craftingRecipe = null;
					}
					if (craftingRecipe != null)
					{
						ComponentInventoryBase.Slot slot = this.m_slots[this.ResultSlotIndex];
						int num3 = Terrain.ExtractContents(craftingRecipe.ResultValue);
						if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value || craftingRecipe.ResultCount + slot.Count > BlocksManager.Blocks[num3].GetMaxStacking(craftingRecipe.ResultValue)))
						{
							craftingRecipe = null;
						}
					}
					if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
					{
						if (this.m_slots[this.RemainsSlotIndex].Count == 0 || this.m_slots[this.RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
						{
							if (BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].GetMaxStacking(craftingRecipe.RemainsValue) - this.m_slots[this.RemainsSlotIndex].Count < craftingRecipe.RemainsCount)
							{
								craftingRecipe = null;
							}
						}
						else
						{
							craftingRecipe = null;
						}
					}
				}
				if (craftingRecipe != null && !string.IsNullOrEmpty(craftingRecipe.Message) && componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(craftingRecipe.Message, Color.White, true, true);
				}
				return craftingRecipe;
			}
			return null;
		}

		// Token: 0x040008D8 RID: 2264
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008D9 RID: 2265
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x040008DA RID: 2266
		public ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x040008DB RID: 2267
		public int m_furnaceSize;

		// Token: 0x040008DC RID: 2268
		public string[] m_matchedIngredients = new string[9];

		// Token: 0x040008DD RID: 2269
		public float m_fireTimeRemaining;

		// Token: 0x040008DE RID: 2270
		public float m_heatLevel;

		// Token: 0x040008DF RID: 2271
		public bool m_updateSmeltingRecipe;

		// Token: 0x040008E0 RID: 2272
		public CraftingRecipe m_smeltingRecipe;

		// Token: 0x040008E1 RID: 2273
		public float m_smeltingProgress;
	}
}
