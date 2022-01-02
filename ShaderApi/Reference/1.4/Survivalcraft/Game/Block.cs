using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000245 RID: 581
	public abstract class Block
	{
		// Token: 0x060012DE RID: 4830 RVA: 0x0008BDB4 File Offset: 0x00089FB4
		public virtual Vector3 GetFirstPersonOffset(int value)
		{
			return this.FirstPersonOffset;
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0008BDBC File Offset: 0x00089FBC
		public virtual Vector3 GetFirstPersonRotation(int value)
		{
			return this.FirstPersonRotation;
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0008BDC4 File Offset: 0x00089FC4
		public virtual float GetInHandScale(int value)
		{
			return this.InHandScale;
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0008BDCC File Offset: 0x00089FCC
		public virtual Vector3 GetInHandOffset(int value)
		{
			return this.InHandOffset;
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0008BDD4 File Offset: 0x00089FD4
		public virtual Vector3 GetInHandRotation(int value)
		{
			return this.InHandRotation;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0008BDDC File Offset: 0x00089FDC
		public virtual float GetDensity(int value)
		{
			return this.Density;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0008BDE4 File Offset: 0x00089FE4
		public virtual float GetFirstPersonScale(int value)
		{
			return this.FirstPersonScale;
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0008BDEC File Offset: 0x00089FEC
		public virtual void Initialize()
		{
			if (this.Durability < -1 || this.Durability > 65535)
			{
				throw new InvalidOperationException(string.Format(LanguageControl.Get(Block.fName, 1), this.DefaultDisplayName));
			}
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0008BE20 File Offset: 0x0008A020
		public virtual bool GetIsDiggingTransparent(int value)
		{
			return this.IsDiggingTransparent;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0008BE28 File Offset: 0x0008A028
		public virtual float GetObjectShadowStrength(int value)
		{
			return this.ObjectShadowStrength;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0008BE30 File Offset: 0x0008A030
		public virtual float GetFuelHeatLevel(int value)
		{
			return this.FuelHeatLevel;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0008BE38 File Offset: 0x0008A038
		public virtual float GetExplosionResilience(int value)
		{
			return this.ExplosionResilience;
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0008BE40 File Offset: 0x0008A040
		public virtual float GetExplosionPressure(int value)
		{
			return this.DefaultExplosionPressure;
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0008BE48 File Offset: 0x0008A048
		public virtual int GetMaxStacking(int value)
		{
			return this.MaxStacking;
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x0008BE50 File Offset: 0x0008A050
		public virtual float GetFuelFireDuration(int value)
		{
			return this.FuelFireDuration;
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0008BE58 File Offset: 0x0008A058
		public virtual float GetProjectileResilience(int value)
		{
			return this.ProjectileResilience;
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0008BE60 File Offset: 0x0008A060
		public virtual float GetFireDuration(int value)
		{
			return this.FireDuration;
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0008BE68 File Offset: 0x0008A068
		public virtual float GetProjectileStickProbability(int value)
		{
			return this.ProjectileStickProbability;
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0008BE70 File Offset: 0x0008A070
		public virtual bool MatchCrafingId(string CraftId)
		{
			return CraftId == this.CraftingId;
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0008BE7E File Offset: 0x0008A07E
		public virtual int GetPlayerLevelRequired(int value)
		{
			return this.PlayerLevelRequired;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0008BE86 File Offset: 0x0008A086
		public virtual bool HasCollisionBehavior_(int value)
		{
			return this.HasCollisionBehavior;
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0008BE90 File Offset: 0x0008A090
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int num = Terrain.ExtractData(value);
			string result;
			if (LanguageControl.TryGetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "DisplayName", out result))
			{
				return result;
			}
			return this.DefaultDisplayName;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0008BED5 File Offset: 0x0008A0D5
		public virtual int GetTextureSlotCount(int value)
		{
			return 16;
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0008BED9 File Offset: 0x0008A0D9
		public virtual bool IsEditable_(int value)
		{
			return this.IsEditable;
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0008BEE1 File Offset: 0x0008A0E1
		public virtual bool IsAimable_(int value)
		{
			return this.IsAimable;
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0008BEE9 File Offset: 0x0008A0E9
		public virtual bool Eat(int value)
		{
			return false;
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0008BEEC File Offset: 0x0008A0EC
		public virtual bool CanWear(int value)
		{
			return false;
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0008BEEF File Offset: 0x0008A0EF
		public virtual ClothingData GetClothingData(int value)
		{
			return null;
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0008BEF2 File Offset: 0x0008A0F2
		public virtual int GetToolLevel(int value)
		{
			return this.ToolLevel;
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0008BEFA File Offset: 0x0008A0FA
		public virtual bool IsCollidable_(int value)
		{
			return this.IsCollidable;
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x0008BF02 File Offset: 0x0008A102
		public virtual bool IsTransparent_(int value)
		{
			return this.IsTransparent;
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x0008BF0A File Offset: 0x0008A10A
		public virtual bool IsFluidBlocker_(int value)
		{
			return this.IsFluidBlocker;
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x0008BF12 File Offset: 0x0008A112
		public virtual bool IsGatherable_(int value)
		{
			return this.IsGatherable;
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0008BF1A File Offset: 0x0008A11A
		public virtual bool IsNonDuplicable_(int value)
		{
			return this.IsNonDuplicable;
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0008BF22 File Offset: 0x0008A122
		public virtual bool IsPlaceable_(int value)
		{
			return this.IsPlaceable;
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0008BF2A File Offset: 0x0008A12A
		public virtual bool IsPlacementTransparent_(int value)
		{
			return this.IsPlacementTransparent;
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0008BF32 File Offset: 0x0008A132
		public virtual bool IsStickable_(int value)
		{
			return this.IsStickable;
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0008BF3A File Offset: 0x0008A13A
		public virtual float GetProjectileSpeed(int value)
		{
			return this.ProjectileSpeed;
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0008BF42 File Offset: 0x0008A142
		public virtual float GetProjectileDamping(int value)
		{
			return this.ProjectileDamping;
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0008BF4C File Offset: 0x0008A14C
		public virtual string GetDescription(int value)
		{
			int num = Terrain.ExtractData(value);
			string result;
			if (LanguageControl.TryGetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "Description", out result))
			{
				return result;
			}
			return this.DefaultDescription;
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0008BF91 File Offset: 0x0008A191
		public virtual FoodType GetFoodType(int value)
		{
			return this.FoodType;
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0008BF99 File Offset: 0x0008A199
		public virtual string GetCategory(int value)
		{
			return this.DefaultCategory;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0008BFA1 File Offset: 0x0008A1A1
		public virtual float GetDigResilience(int value)
		{
			return this.DigResilience;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x0008BFA9 File Offset: 0x0008A1A9
		public virtual BlockDigMethod GetBlockDigMethod(int value)
		{
			return this.DigMethod;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0008BFB1 File Offset: 0x0008A1B1
		public virtual float GetShovelPower(int value)
		{
			return this.ShovelPower;
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0008BFB9 File Offset: 0x0008A1B9
		public virtual float GetQuarryPower(int value)
		{
			return this.QuarryPower;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0008BFC1 File Offset: 0x0008A1C1
		public virtual float GetHackPower(int value)
		{
			return this.HackPower;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0008BFC9 File Offset: 0x0008A1C9
		public virtual IEnumerable<int> GetCreativeValues()
		{
			if (this.DefaultCreativeData >= 0)
			{
				yield return Terrain.ReplaceContents(Terrain.ReplaceData(0, this.DefaultCreativeData), this.BlockIndex);
			}
			yield break;
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0008BFD9 File Offset: 0x0008A1D9
		public virtual bool GetAlignToVelocity(int value)
		{
			return this.AlignToVelocity;
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0008BFE1 File Offset: 0x0008A1E1
		public virtual bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return this.DefaultIsInteractive;
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0008BFE9 File Offset: 0x0008A1E9
		public virtual IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			yield break;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0008BFF2 File Offset: 0x0008A1F2
		public virtual CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			return null;
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0008BFF5 File Offset: 0x0008A1F5
		public virtual bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return this.IsTransparent;
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0008C000 File Offset: 0x0008A200
		public virtual bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			int num = Terrain.ExtractContents(neighborValue);
			return BlocksManager.Blocks[num].IsFaceTransparent(subsystemTerrain, CellFace.OppositeFace(face), neighborValue);
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0008C02A File Offset: 0x0008A22A
		public virtual int GetShadowStrength(int value)
		{
			return this.DefaultShadowStrength;
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0008C032 File Offset: 0x0008A232
		public virtual int GetFaceTextureSlot(int face, int value)
		{
			return this.DefaultTextureSlot;
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0008C03A File Offset: 0x0008A23A
		public virtual string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return this.DefaultSoundMaterialName;
		}

		// Token: 0x06001317 RID: 4887
		public abstract void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z);

		// Token: 0x06001318 RID: 4888 RVA: 0x0008C042 File Offset: 0x0008A242
		public virtual void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubset geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06001319 RID: 4889
		public abstract void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData);

		// Token: 0x0600131A RID: 4890 RVA: 0x0008C044 File Offset: 0x0008A244
		public virtual BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0008C070 File Offset: 0x0008A270
		public virtual string GetCraftingId(int value)
		{
			return this.CraftingId;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x0008C078 File Offset: 0x0008A278
		public virtual int GetDisplayOrder(int value)
		{
			return this.DisplayOrder;
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0008C080 File Offset: 0x0008A280
		public virtual BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = 0,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0008C0AC File Offset: 0x0008A2AC
		public virtual float GetRequiredToolLevel(int value)
		{
			return (float)this.RequiredToolLevel;
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x0008C0B8 File Offset: 0x0008A2B8
		public virtual void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = (this.DestructionDebrisScale > 0f);
			if (toolLevel < this.RequiredToolLevel)
			{
				return;
			}
			if (this.DefaultDropContent != 0)
			{
				int num = (int)this.DefaultDropCount;
				if (this.Random.Bool(this.DefaultDropCount - (float)num))
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					BlockDropValue item = new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(this.DefaultDropContent),
						Count = 1
					};
					dropValues.Add(item);
				}
			}
			int num2 = (int)this.DefaultExperienceCount;
			if (this.Random.Bool(this.DefaultExperienceCount - (float)num2))
			{
				num2++;
			}
			for (int j = 0; j < num2; j++)
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(248),
					Count = 1
				};
				dropValues.Add(item);
			}
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x0008C1A1 File Offset: 0x0008A3A1
		public virtual int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 4 & 4095;
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x0008C1B4 File Offset: 0x0008A3B4
		public virtual int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= 15;
			num |= MathUtils.Clamp(damage, 0, 4095) << 4;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x0008C1E5 File Offset: 0x0008A3E5
		public virtual int GetDamageDestructionValue(int value)
		{
			return 0;
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0008C1E8 File Offset: 0x0008A3E8
		public virtual int GetRotPeriod(int value)
		{
			return this.DefaultRotPeriod;
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0008C1F0 File Offset: 0x0008A3F0
		public virtual float GetSicknessProbability(int value)
		{
			return this.DefaultSicknessProbability;
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0008C1F8 File Offset: 0x0008A3F8
		public virtual float GetMeleePower(int value)
		{
			return this.DefaultMeleePower;
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0008C200 File Offset: 0x0008A400
		public virtual float GetMeleeHitProbability(int value)
		{
			return this.DefaultMeleeHitProbability;
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0008C208 File Offset: 0x0008A408
		public virtual float GetProjectilePower(int value)
		{
			return this.DefaultProjectilePower;
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0008C210 File Offset: 0x0008A410
		public virtual float GetHeat(int value)
		{
			return this.DefaultHeat;
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0008C218 File Offset: 0x0008A418
		public virtual float GetBlockHealth(int value)
		{
			int durability = this.GetDurability(value);
			int damage = this.GetDamage(value);
			if (this.Durability > 0)
			{
				return (float)(durability - damage) / (float)durability;
			}
			return -1f;
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0008C24B File Offset: 0x0008A44B
		public virtual int GetDurability(int value)
		{
			return this.Durability;
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0008C253 File Offset: 0x0008A453
		public virtual bool GetExplosionIncendiary(int value)
		{
			return this.DefaultExplosionIncendiary;
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0008C25B File Offset: 0x0008A45B
		public virtual Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconBlockOffset;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0008C263 File Offset: 0x0008A463
		public virtual Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconViewOffset;
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x0008C26B File Offset: 0x0008A46B
		public virtual float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconViewScale;
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x0008C273 File Offset: 0x0008A473
		public virtual BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x0008C291 File Offset: 0x0008A491
		public virtual BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return Block.m_defaultCollisionBoxes;
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0008C298 File Offset: 0x0008A498
		public virtual BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0008C2A2 File Offset: 0x0008A4A2
		public virtual int GetEmittedLightAmount(int value)
		{
			return this.DefaultEmittedLightAmount;
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0008C2AA File Offset: 0x0008A4AA
		public virtual float GetNutritionalValue(int value)
		{
			return this.DefaultNutritionalValue;
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x0008C2B2 File Offset: 0x0008A4B2
		public virtual bool ShouldAvoid(int value)
		{
			return false;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0008C2B5 File Offset: 0x0008A4B5
		public virtual bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return true;
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0008C2B8 File Offset: 0x0008A4B8
		public virtual bool IsHeatBlocker(int value)
		{
			return this.IsCollidable_(value);
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x0008C2C4 File Offset: 0x0008A4C4
		public float? Raycast(Ray3 ray, SubsystemTerrain subsystemTerrain, int value, bool useInteractionBoxes, out int nearestBoxIndex, out BoundingBox nearestBox)
		{
			float? result = null;
			nearestBoxIndex = 0;
			nearestBox = default(BoundingBox);
			BoundingBox[] array = useInteractionBoxes ? this.GetCustomInteractionBoxes(subsystemTerrain, value) : this.GetCustomCollisionBoxes(subsystemTerrain, value);
			for (int i = 0; i < array.Length; i++)
			{
				float? num = ray.Intersection(array[i]);
				if (num != null && (result == null || num.Value < result.Value))
				{
					nearestBoxIndex = i;
					result = num;
				}
			}
			nearestBox = array[nearestBoxIndex];
			return result;
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0008C352 File Offset: 0x0008A552
		public virtual void DrawGeometry(int x, int y, int z)
		{
		}

		// Token: 0x04000BA9 RID: 2985
		public int BlockIndex;

		// Token: 0x04000BAA RID: 2986
		public string DefaultDisplayName = string.Empty;

		// Token: 0x04000BAB RID: 2987
		public string DefaultDescription = string.Empty;

		// Token: 0x04000BAC RID: 2988
		public string DefaultCategory = string.Empty;

		// Token: 0x04000BAD RID: 2989
		public int DisplayOrder;

		// Token: 0x04000BAE RID: 2990
		public Vector3 DefaultIconBlockOffset = Vector3.Zero;

		// Token: 0x04000BAF RID: 2991
		public Vector3 DefaultIconViewOffset = new Vector3(1f);

		// Token: 0x04000BB0 RID: 2992
		public float DefaultIconViewScale = 1f;

		// Token: 0x04000BB1 RID: 2993
		public float FirstPersonScale = 1f;

		// Token: 0x04000BB2 RID: 2994
		public Vector3 FirstPersonOffset = Vector3.Zero;

		// Token: 0x04000BB3 RID: 2995
		public Vector3 FirstPersonRotation = Vector3.Zero;

		// Token: 0x04000BB4 RID: 2996
		public float InHandScale = 1f;

		// Token: 0x04000BB5 RID: 2997
		public Vector3 InHandOffset = Vector3.Zero;

		// Token: 0x04000BB6 RID: 2998
		public Vector3 InHandRotation = Vector3.Zero;

		// Token: 0x04000BB7 RID: 2999
		public string Behaviors = string.Empty;

		// Token: 0x04000BB8 RID: 3000
		public string CraftingId = string.Empty;

		// Token: 0x04000BB9 RID: 3001
		public int DefaultCreativeData;

		// Token: 0x04000BBA RID: 3002
		public bool IsCollidable = true;

		// Token: 0x04000BBB RID: 3003
		public bool IsPlaceable = true;

		// Token: 0x04000BBC RID: 3004
		public bool IsDiggingTransparent;

		// Token: 0x04000BBD RID: 3005
		public bool IsPlacementTransparent;

		// Token: 0x04000BBE RID: 3006
		public bool DefaultIsInteractive;

		// Token: 0x04000BBF RID: 3007
		public bool IsEditable;

		// Token: 0x04000BC0 RID: 3008
		public bool IsNonDuplicable;

		// Token: 0x04000BC1 RID: 3009
		public bool IsGatherable;

		// Token: 0x04000BC2 RID: 3010
		public bool HasCollisionBehavior;

		// Token: 0x04000BC3 RID: 3011
		public bool KillsWhenStuck;

		// Token: 0x04000BC4 RID: 3012
		public bool IsFluidBlocker = true;

		// Token: 0x04000BC5 RID: 3013
		public bool IsTransparent;

		// Token: 0x04000BC6 RID: 3014
		public int DefaultShadowStrength;

		// Token: 0x04000BC7 RID: 3015
		public int LightAttenuation;

		// Token: 0x04000BC8 RID: 3016
		public int DefaultEmittedLightAmount;

		// Token: 0x04000BC9 RID: 3017
		public float ObjectShadowStrength;

		// Token: 0x04000BCA RID: 3018
		public int DefaultDropContent;

		// Token: 0x04000BCB RID: 3019
		public float DefaultDropCount = 1f;

		// Token: 0x04000BCC RID: 3020
		public float DefaultExperienceCount;

		// Token: 0x04000BCD RID: 3021
		public int RequiredToolLevel;

		// Token: 0x04000BCE RID: 3022
		public int MaxStacking = 40;

		// Token: 0x04000BCF RID: 3023
		public float SleepSuitability;

		// Token: 0x04000BD0 RID: 3024
		public float FrictionFactor = 1f;

		// Token: 0x04000BD1 RID: 3025
		public float Density = 4f;

		// Token: 0x04000BD2 RID: 3026
		public bool NoAutoJump;

		// Token: 0x04000BD3 RID: 3027
		public bool NoSmoothRise;

		// Token: 0x04000BD4 RID: 3028
		public int DefaultTextureSlot;

		// Token: 0x04000BD5 RID: 3029
		public float DestructionDebrisScale = 1f;

		// Token: 0x04000BD6 RID: 3030
		public float FuelHeatLevel;

		// Token: 0x04000BD7 RID: 3031
		public float FuelFireDuration;

		// Token: 0x04000BD8 RID: 3032
		public string DefaultSoundMaterialName;

		// Token: 0x04000BD9 RID: 3033
		public float ShovelPower = 1f;

		// Token: 0x04000BDA RID: 3034
		public float QuarryPower = 1f;

		// Token: 0x04000BDB RID: 3035
		public float HackPower = 1f;

		// Token: 0x04000BDC RID: 3036
		public float DefaultMeleePower = 1f;

		// Token: 0x04000BDD RID: 3037
		public float DefaultMeleeHitProbability = 0.66f;

		// Token: 0x04000BDE RID: 3038
		public float DefaultProjectilePower = 1f;

		// Token: 0x04000BDF RID: 3039
		public int ToolLevel;

		// Token: 0x04000BE0 RID: 3040
		public int PlayerLevelRequired = 1;

		// Token: 0x04000BE1 RID: 3041
		public int Durability = -1;

		// Token: 0x04000BE2 RID: 3042
		public BlockDigMethod DigMethod;

		// Token: 0x04000BE3 RID: 3043
		public float DigResilience = 1f;

		// Token: 0x04000BE4 RID: 3044
		public float ProjectileResilience = 1f;

		// Token: 0x04000BE5 RID: 3045
		public bool IsAimable;

		// Token: 0x04000BE6 RID: 3046
		public bool IsStickable;

		// Token: 0x04000BE7 RID: 3047
		public bool AlignToVelocity;

		// Token: 0x04000BE8 RID: 3048
		public float ProjectileSpeed = 15f;

		// Token: 0x04000BE9 RID: 3049
		public float ProjectileDamping = 0.8f;

		// Token: 0x04000BEA RID: 3050
		public float ProjectileTipOffset;

		// Token: 0x04000BEB RID: 3051
		public bool DisintegratesOnHit;

		// Token: 0x04000BEC RID: 3052
		public float ProjectileStickProbability;

		// Token: 0x04000BED RID: 3053
		public float DefaultHeat;

		// Token: 0x04000BEE RID: 3054
		public float FireDuration;

		// Token: 0x04000BEF RID: 3055
		public float ExplosionResilience;

		// Token: 0x04000BF0 RID: 3056
		public float DefaultExplosionPressure;

		// Token: 0x04000BF1 RID: 3057
		public bool DefaultExplosionIncendiary;

		// Token: 0x04000BF2 RID: 3058
		public bool IsExplosionTransparent;

		// Token: 0x04000BF3 RID: 3059
		public float DefaultNutritionalValue;

		// Token: 0x04000BF4 RID: 3060
		public FoodType FoodType;

		// Token: 0x04000BF5 RID: 3061
		public int DefaultRotPeriod;

		// Token: 0x04000BF6 RID: 3062
		public float DefaultSicknessProbability;

		// Token: 0x04000BF7 RID: 3063
		public static string fName = "Block";

		// Token: 0x04000BF8 RID: 3064
		public Game.Random Random = new Game.Random();

		// Token: 0x04000BF9 RID: 3065
		public static BoundingBox[] m_defaultCollisionBoxes = new BoundingBox[]
		{
			new BoundingBox(Vector3.Zero, Vector3.One)
		};
	}
}
