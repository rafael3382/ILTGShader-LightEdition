using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F5 RID: 501
	public class ComponentClothing : Component, IUpdateable, IInventory
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000E27 RID: 3623 RVA: 0x000681E3 File Offset: 0x000663E3
		public Texture2D InnerClothedTexture
		{
			get
			{
				return this.m_innerClothedTexture;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000E28 RID: 3624 RVA: 0x000681EB File Offset: 0x000663EB
		public Texture2D OuterClothedTexture
		{
			get
			{
				return this.m_outerClothedTexture;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x000681F3 File Offset: 0x000663F3
		// (set) Token: 0x06000E2A RID: 3626 RVA: 0x000681FB File Offset: 0x000663FB
		public float Insulation { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x00068204 File Offset: 0x00066404
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x0006820C File Offset: 0x0006640C
		public ClothingSlot LeastInsulatedSlot { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x00068215 File Offset: 0x00066415
		// (set) Token: 0x06000E2E RID: 3630 RVA: 0x0006821D File Offset: 0x0006641D
		public float SteedMovementSpeedFactor { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000E2F RID: 3631 RVA: 0x00068226 File Offset: 0x00066426
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000E30 RID: 3632 RVA: 0x00068229 File Offset: 0x00066429
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000E31 RID: 3633 RVA: 0x00068231 File Offset: 0x00066431
		public int SlotsCount
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x00068234 File Offset: 0x00066434
		// (set) Token: 0x06000E33 RID: 3635 RVA: 0x0006823C File Offset: 0x0006643C
		public int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x0006823E File Offset: 0x0006643E
		// (set) Token: 0x06000E35 RID: 3637 RVA: 0x00068241 File Offset: 0x00066441
		public int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x00068243 File Offset: 0x00066443
		public virtual ReadOnlyList<int> GetClothes(ClothingSlot slot)
		{
			return new ReadOnlyList<int>(this.m_clothes[slot]);
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00068258 File Offset: 0x00066458
		public virtual void SetClothes(ClothingSlot slot, IEnumerable<int> clothes)
		{
			if (!this.m_clothes[slot].SequenceEqual(clothes))
			{
				this.m_clothes[slot].Clear();
				this.m_clothes[slot].AddRange(clothes);
				this.m_clothedTexturesValid = false;
				float num = 0f;
				foreach (KeyValuePair<ClothingSlot, List<int>> keyValuePair in this.m_clothes)
				{
					foreach (int value in keyValuePair.Value)
					{
						ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value);
						num += clothingData.DensityModifier;
					}
				}
				float num2 = num - this.m_densityModifierApplied;
				this.m_densityModifierApplied += num2;
				this.m_componentBody.Density += num2;
				this.SteedMovementSpeedFactor = 1f;
				float num3 = 2f;
				float num4 = 0.2f;
				float num5 = 0.4f;
				float num6 = 2f;
				foreach (int value2 in this.GetClothes(ClothingSlot.Head))
				{
					ClothingData clothingData2 = BlocksManager.Blocks[Terrain.ExtractContents(value2)].GetClothingData(value2);
					num3 += clothingData2.Insulation;
					this.SteedMovementSpeedFactor *= clothingData2.SteedMovementSpeedFactor;
				}
				foreach (int value3 in this.GetClothes(ClothingSlot.Torso))
				{
					ClothingData clothingData3 = BlocksManager.Blocks[Terrain.ExtractContents(value3)].GetClothingData(value3);
					num4 += clothingData3.Insulation;
					this.SteedMovementSpeedFactor *= clothingData3.SteedMovementSpeedFactor;
				}
				foreach (int value4 in this.GetClothes(ClothingSlot.Legs))
				{
					ClothingData clothingData4 = BlocksManager.Blocks[Terrain.ExtractContents(value4)].GetClothingData(value4);
					num5 += clothingData4.Insulation;
					this.SteedMovementSpeedFactor *= clothingData4.SteedMovementSpeedFactor;
				}
				foreach (int value5 in this.GetClothes(ClothingSlot.Feet))
				{
					ClothingData clothingData5 = BlocksManager.Blocks[Terrain.ExtractContents(value5)].GetClothingData(value5);
					num6 += clothingData5.Insulation;
					this.SteedMovementSpeedFactor *= clothingData5.SteedMovementSpeedFactor;
				}
				this.Insulation = 1f / (1f / num3 + 1f / num4 + 1f / num5 + 1f / num6);
				float num7 = MathUtils.Min(num3, num4, num5, num6);
				if (num3 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Head;
					return;
				}
				if (num4 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Torso;
					return;
				}
				if (num5 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Legs;
					return;
				}
				if (num6 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Feet;
				}
			}
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x000685E8 File Offset: 0x000667E8
		public float ApplyArmorProtection(float attackPower)
		{
			bool Applied = false;
			ModsManager.HookAction("ApplyArmorProtection", delegate(ModLoader modLoader)
			{
				bool flag;
				attackPower = modLoader.ApplyArmorProtection(this, attackPower, out flag);
				Applied = (Applied || flag);
				return false;
			});
			if (!Applied)
			{
				float num = this.m_random.Float(0f, 1f);
				ClothingSlot slot = (num < 0.1f) ? ClothingSlot.Feet : ((num < 0.3f) ? ClothingSlot.Legs : ((num < 0.9f) ? ClothingSlot.Torso : ClothingSlot.Head));
				float num2 = (float)(((ClothingBlock)BlocksManager.Blocks[203]).Durability + 1);
				List<int> list = new List<int>(this.GetClothes(slot));
				for (int i = 0; i < list.Count; i++)
				{
					int value = list[i];
					ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value);
					float x = (num2 - (float)BlocksManager.Blocks[203].GetDamage(value)) / num2 * clothingData.Sturdiness;
					float num3 = MathUtils.Min(attackPower * MathUtils.Saturate(clothingData.ArmorProtection), x);
					if (num3 > 0f)
					{
						attackPower -= num3;
						if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative)
						{
							float x2 = num3 / clothingData.Sturdiness * num2 + 0.001f;
							int damageCount = (int)(MathUtils.Floor(x2) + (float)(this.m_random.Bool(MathUtils.Remainder(x2, 1f)) ? 1 : 0));
							list[i] = BlocksManager.DamageItem(value, damageCount);
						}
						if (!string.IsNullOrEmpty(clothingData.ImpactSoundsFolder))
						{
							this.m_subsystemAudio.PlayRandomSound(clothingData.ImpactSoundsFolder, 1f, this.m_random.Float(-0.3f, 0.3f), this.m_componentBody.Position, 4f, 0.15f);
						}
					}
				}
				int j = 0;
				while (j < list.Count)
				{
					if (Terrain.ExtractContents(list[j]) != 203)
					{
						list.RemoveAt(j);
						this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, this.m_componentBody.Position + this.m_componentBody.BoxSize / 2f, 1f, 1f, Color.White, 0));
					}
					else
					{
						j++;
					}
				}
				this.SetClothes(slot, list);
			}
			return MathUtils.Max(attackPower, 0f);
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00068878 File Offset: 0x00066A78
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentHumanModel = base.Entity.FindComponent<ComponentHumanModel>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentOuterClothingModel = base.Entity.FindComponent<ComponentOuterClothingModel>(true);
			this.m_componentVitalStats = base.Entity.FindComponent<ComponentVitalStats>(true);
			this.m_componentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.SteedMovementSpeedFactor = 1f;
			this.Insulation = 0f;
			this.LeastInsulatedSlot = ClothingSlot.Feet;
			this.m_clothes[ClothingSlot.Head] = new List<int>();
			this.m_clothes[ClothingSlot.Torso] = new List<int>();
			this.m_clothes[ClothingSlot.Legs] = new List<int>();
			this.m_clothes[ClothingSlot.Feet] = new List<int>();
			ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("Clothes");
			this.SetClothes(ClothingSlot.Head, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Head")));
			this.SetClothes(ClothingSlot.Torso, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Torso")));
			this.SetClothes(ClothingSlot.Legs, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Legs")));
			this.SetClothes(ClothingSlot.Feet, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Feet")));
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00068A54 File Offset: 0x00066C54
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Clothes", valuesDictionary2);
			valuesDictionary2.SetValue<string>("Head", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Head].ToArray()));
			valuesDictionary2.SetValue<string>("Torso", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Torso].ToArray()));
			valuesDictionary2.SetValue<string>("Legs", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Legs].ToArray()));
			valuesDictionary2.SetValue<string>("Feet", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Feet].ToArray()));
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x00068B00 File Offset: 0x00066D00
		public override void Dispose()
		{
			base.Dispose();
			if (this.m_skinTexture != null && !ContentManager.IsContent(this.m_skinTexture))
			{
				this.m_skinTexture.Dispose();
				this.m_skinTexture = null;
			}
			if (this.m_innerClothedTexture != null)
			{
				this.m_innerClothedTexture.Dispose();
				this.m_innerClothedTexture = null;
			}
			if (this.m_outerClothedTexture != null)
			{
				this.m_outerClothedTexture.Dispose();
				this.m_outerClothedTexture = null;
			}
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00068B80 File Offset: 0x00066D80
		public void Update(float dt)
		{
			foreach (ClothingSlot slot in ComponentClothing.m_innerSlotsOrder)
			{
				foreach (int value in this.GetClothes(slot))
				{
					BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value).OnUpdate();
				}
			}
			foreach (ClothingSlot slot2 in ComponentClothing.m_outerSlotsOrder)
			{
				foreach (int value2 in this.GetClothes(slot2))
				{
					BlocksManager.Blocks[Terrain.ExtractContents(value2)].GetClothingData(value2).OnUpdate();
				}
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled && this.m_subsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				foreach (int slot3 in EnumUtils.GetEnumValues(typeof(ClothingSlot)))
				{
					bool flag = false;
					this.m_clothesList.Clear();
					this.m_clothesList.AddRange(this.GetClothes((ClothingSlot)slot3));
					int j = 0;
					while (j < this.m_clothesList.Count)
					{
						int value3 = this.m_clothesList[j];
						ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(value3)].GetClothingData(value3);
						if ((float)clothingData.PlayerLevelRequired > this.m_componentPlayer.PlayerData.Level)
						{
							this.m_componentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentClothing.fName, 1), clothingData.PlayerLevelRequired, clothingData.DisplayName), Color.White, true, true);
							this.m_subsystemPickables.AddPickable(value3, 1, this.m_componentBody.Position, null, null);
							this.m_clothesList.RemoveAt(j);
							flag = true;
						}
						else
						{
							j++;
						}
					}
					if (flag)
					{
						this.SetClothes((ClothingSlot)slot3, this.m_clothesList);
					}
				}
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled && this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0) && ((this.m_componentLocomotion.LastWalkOrder != null && this.m_componentLocomotion.LastWalkOrder.Value != Vector2.Zero) || (this.m_componentLocomotion.LastSwimOrder != null && this.m_componentLocomotion.LastSwimOrder.Value != Vector3.Zero) || this.m_componentLocomotion.LastJumpOrder != 0f))
			{
				if (this.m_lastTotalElapsedGameTime != null)
				{
					foreach (int slot4 in EnumUtils.GetEnumValues(typeof(ClothingSlot)))
					{
						bool flag2 = false;
						this.m_clothesList.Clear();
						this.m_clothesList.AddRange(this.GetClothes((ClothingSlot)slot4));
						for (int k = 0; k < this.m_clothesList.Count; k++)
						{
							int value4 = this.m_clothesList[k];
							ClothingData clothingData2 = BlocksManager.Blocks[Terrain.ExtractContents(value4)].GetClothingData(value4);
							float num = (this.m_componentVitalStats.Wetness > 0f) ? (10f * clothingData2.Sturdiness) : (20f * clothingData2.Sturdiness);
							double num2 = MathUtils.Floor(this.m_lastTotalElapsedGameTime.Value / (double)num);
							if (MathUtils.Floor(this.m_subsystemGameInfo.TotalElapsedGameTime / (double)num) > num2 && this.m_random.Float(0f, 1f) < 0.75f)
							{
								this.m_clothesList[k] = BlocksManager.DamageItem(value4, 1);
								flag2 = true;
							}
						}
						int l = 0;
						while (l < this.m_clothesList.Count)
						{
							if (Terrain.ExtractContents(this.m_clothesList[l]) != 203)
							{
								this.m_clothesList.RemoveAt(l);
								this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, this.m_componentBody.Position + this.m_componentBody.BoxSize / 2f, 1f, 1f, Color.White, 0));
								this.m_componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentClothing.fName, 2), Color.White, true, true);
							}
							else
							{
								l++;
							}
						}
						if (flag2)
						{
							this.SetClothes((ClothingSlot)slot4, this.m_clothesList);
						}
					}
				}
				this.m_lastTotalElapsedGameTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
			this.UpdateRenderTargets();
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x0006914C File Offset: 0x0006734C
		public virtual int GetSlotValue(int slotIndex)
		{
			return this.GetClothes((ClothingSlot)slotIndex).LastOrDefault<int>();
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x00069160 File Offset: 0x00067360
		public virtual int GetSlotCount(int slotIndex)
		{
			if (this.GetClothes((ClothingSlot)slotIndex).Count <= 0)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x00069182 File Offset: 0x00067382
		public virtual int GetSlotCapacity(int slotIndex, int value)
		{
			return 0;
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x00069188 File Offset: 0x00067388
		public virtual int GetSlotProcessCapacity(int slotIndex, int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			if (block.GetNutritionalValue(value) > 0f)
			{
				return 1;
			}
			if (block is ClothingBlock && this.CanWearClothing(value))
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000691C6 File Offset: 0x000673C6
		public virtual void AddSlotItems(int slotIndex, int value, int count)
		{
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000691C8 File Offset: 0x000673C8
		public virtual void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			processedCount = 0;
			processedValue = 0;
			if (processCount != 1)
			{
				return;
			}
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			ModsManager.HookAction("ClothingProcessSlotItems", (ModLoader modLoader) => modLoader.ClothingProcessSlotItems(this.m_componentPlayer, block, slotIndex, value, count));
			if (block.GetNutritionalValue(value) > 0f)
			{
				if (block is BucketBlock)
				{
					processedValue = Terrain.MakeBlockValue(90, 0, Terrain.ExtractData(value));
					processedCount = 1;
				}
				if (count > 1 && processedCount > 0 && processedValue != value)
				{
					processedValue = value;
					processedCount = processCount;
				}
				else if (block.Eat(value) || !this.m_componentVitalStats.Eat(value))
				{
					processedValue = value;
					processedCount = processCount;
				}
			}
			if (block.CanWear(value))
			{
				ClothingData clothingData = block.GetClothingData(value);
				clothingData.OnMount();
				List<int> list = new List<int>(this.GetClothes(clothingData.Slot));
				list.Add(value);
				this.SetClothes(clothingData.Slot, list);
			}
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00069330 File Offset: 0x00067530
		public virtual int RemoveSlotItems(int slotIndex, int count)
		{
			if (count == 1)
			{
				List<int> list = new List<int>(this.GetClothes((ClothingSlot)slotIndex));
				if (list.Count > 0)
				{
					int value = list[list.Count - 1];
					Terrain.ExtractData(value);
					BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value).OnDismount();
					list.RemoveAt(list.Count - 1);
					this.SetClothes((ClothingSlot)slotIndex, list);
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x000693A4 File Offset: 0x000675A4
		public virtual void DropAllItems(Vector3 position)
		{
			Game.Random random = new Game.Random();
			SubsystemPickables subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			for (int i = 0; i < this.SlotsCount; i++)
			{
				int slotCount = this.GetSlotCount(i);
				if (slotCount > 0)
				{
					int slotValue = this.GetSlotValue(i);
					int count = this.RemoveSlotItems(i, slotCount);
					Vector3 value = random.Float(5f, 10f) * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(1f, 2f), random.Float(-1f, 1f)));
					subsystemPickables.AddPickable(slotValue, count, position, new Vector3?(value), null);
				}
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x0006946B File Offset: 0x0006766B
		public virtual void Display_DeviceReset()
		{
			this.m_clothedTexturesValid = false;
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00069474 File Offset: 0x00067674
		public virtual bool CanWearClothing(int value)
		{
			ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value);
			IList<int> list = this.GetClothes(clothingData.Slot);
			if (list.Count == 0)
			{
				return true;
			}
			int value2 = list[list.Count - 1];
			ClothingData clothingData2 = BlocksManager.Blocks[Terrain.ExtractContents(value2)].GetClothingData(value2);
			return clothingData.Layer > clothingData2.Layer;
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x000694E0 File Offset: 0x000676E0
		public virtual void UpdateRenderTargets()
		{
			if (this.m_skinTexture == null || this.m_componentPlayer.PlayerData.CharacterSkinName != this.m_skinTextureName)
			{
				this.m_skinTexture = CharacterSkinsManager.LoadTexture(this.m_componentPlayer.PlayerData.CharacterSkinName);
				this.m_skinTextureName = this.m_componentPlayer.PlayerData.CharacterSkinName;
				Utilities.Dispose<RenderTarget2D>(ref this.m_innerClothedTexture);
				Utilities.Dispose<RenderTarget2D>(ref this.m_outerClothedTexture);
			}
			if (this.m_innerClothedTexture == null || this.m_innerClothedTexture.Width != this.m_skinTexture.Width || this.m_innerClothedTexture.Height != this.m_skinTexture.Height)
			{
				this.m_innerClothedTexture = new RenderTarget2D(this.m_skinTexture.Width, this.m_skinTexture.Height, 1, ColorFormat.Rgba8888, DepthFormat.None);
				this.m_componentHumanModel.TextureOverride = this.m_innerClothedTexture;
				this.m_clothedTexturesValid = false;
			}
			if (this.m_outerClothedTexture == null || this.m_outerClothedTexture.Width != this.m_skinTexture.Width || this.m_outerClothedTexture.Height != this.m_skinTexture.Height)
			{
				this.m_outerClothedTexture = new RenderTarget2D(this.m_skinTexture.Width, this.m_skinTexture.Height, 1, ColorFormat.Rgba8888, DepthFormat.None);
				this.m_componentOuterClothingModel.TextureOverride = this.m_outerClothedTexture;
				this.m_clothedTexturesValid = false;
			}
			if (ComponentClothing.DrawClothedTexture && !this.m_clothedTexturesValid)
			{
				this.m_clothedTexturesValid = true;
				Rectangle scissorRectangle = Display.ScissorRectangle;
				RenderTarget2D renderTarget = Display.RenderTarget;
				try
				{
					Display.RenderTarget = this.m_innerClothedTexture;
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					int num = 0;
					TexturedBatch2D texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(this.m_skinTexture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
					texturedBatch2D.QueueQuad(Vector2.Zero, new Vector2((float)this.m_innerClothedTexture.Width, (float)this.m_innerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, Color.White);
					foreach (ClothingSlot slot in ComponentClothing.m_innerSlotsOrder)
					{
						foreach (int value in this.GetClothes(slot))
						{
							int data = Terrain.ExtractData(value);
							ClothingData clothingData = BlocksManager.Blocks[Terrain.ExtractContents(value)].GetClothingData(value);
							Color fabricColor = SubsystemPalette.GetFabricColor(this.m_subsystemTerrain, new int?(ClothingBlock.GetClothingColor(data)));
							texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(clothingData.Texture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
							if (!clothingData.IsOuter)
							{
								texturedBatch2D.QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_innerClothedTexture.Width, (float)this.m_innerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, fabricColor);
							}
						}
					}
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
					Display.RenderTarget = this.m_outerClothedTexture;
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					num = 0;
					foreach (ClothingSlot slot2 in ComponentClothing.m_outerSlotsOrder)
					{
						foreach (int value2 in this.GetClothes(slot2))
						{
							int data2 = Terrain.ExtractData(value2);
							ClothingData clothingData2 = BlocksManager.Blocks[Terrain.ExtractContents(value2)].GetClothingData(value2);
							Color fabricColor2 = SubsystemPalette.GetFabricColor(this.m_subsystemTerrain, new int?(ClothingBlock.GetClothingColor(data2)));
							texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(clothingData2.Texture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
							if (clothingData2.IsOuter)
							{
								texturedBatch2D.QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_outerClothedTexture.Width, (float)this.m_outerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, fabricColor2);
							}
						}
					}
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
				}
				finally
				{
					Display.RenderTarget = renderTarget;
					Display.ScissorRectangle = scissorRectangle;
				}
			}
		}

		// Token: 0x04000794 RID: 1940
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000795 RID: 1941
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000796 RID: 1942
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000797 RID: 1943
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000798 RID: 1944
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000799 RID: 1945
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400079A RID: 1946
		public ComponentGui m_componentGui;

		// Token: 0x0400079B RID: 1947
		public ComponentHumanModel m_componentHumanModel;

		// Token: 0x0400079C RID: 1948
		public ComponentBody m_componentBody;

		// Token: 0x0400079D RID: 1949
		public ComponentOuterClothingModel m_componentOuterClothingModel;

		// Token: 0x0400079E RID: 1950
		public ComponentVitalStats m_componentVitalStats;

		// Token: 0x0400079F RID: 1951
		public ComponentLocomotion m_componentLocomotion;

		// Token: 0x040007A0 RID: 1952
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040007A1 RID: 1953
		public Texture2D m_skinTexture;

		// Token: 0x040007A2 RID: 1954
		public string m_skinTextureName;

		// Token: 0x040007A3 RID: 1955
		public RenderTarget2D m_innerClothedTexture;

		// Token: 0x040007A4 RID: 1956
		public RenderTarget2D m_outerClothedTexture;

		// Token: 0x040007A5 RID: 1957
		public PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x040007A6 RID: 1958
		public Game.Random m_random = new Game.Random();

		// Token: 0x040007A7 RID: 1959
		public float m_densityModifierApplied;

		// Token: 0x040007A8 RID: 1960
		public double? m_lastTotalElapsedGameTime;

		// Token: 0x040007A9 RID: 1961
		public bool m_clothedTexturesValid;

		// Token: 0x040007AA RID: 1962
		public static string fName = "ComponentClothing";

		// Token: 0x040007AB RID: 1963
		public List<int> m_clothesList = new List<int>();

		// Token: 0x040007AC RID: 1964
		public Dictionary<ClothingSlot, List<int>> m_clothes = new Dictionary<ClothingSlot, List<int>>();

		// Token: 0x040007AD RID: 1965
		public static ClothingSlot[] m_innerSlotsOrder = new ClothingSlot[]
		{
			ClothingSlot.Head,
			ClothingSlot.Torso,
			ClothingSlot.Feet,
			ClothingSlot.Legs
		};

		// Token: 0x040007AE RID: 1966
		public static ClothingSlot[] m_outerSlotsOrder = new ClothingSlot[]
		{
			ClothingSlot.Head,
			ClothingSlot.Torso,
			ClothingSlot.Legs,
			ClothingSlot.Feet
		};

		// Token: 0x040007AF RID: 1967
		public static bool ShowClothedTexture = false;

		// Token: 0x040007B0 RID: 1968
		public static bool DrawClothedTexture = true;
	}
}
