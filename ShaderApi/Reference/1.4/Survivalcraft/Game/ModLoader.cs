using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000156 RID: 342
	public abstract class ModLoader
	{
		// Token: 0x0600075F RID: 1887 RVA: 0x0002A788 File Offset: 0x00028988
		public virtual void __ModInitialize()
		{
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0002A78A File Offset: 0x0002898A
		public virtual void ModDispose()
		{
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x0002A78C File Offset: 0x0002898C
		public virtual void OnMinerHit(ComponentMiner miner, ComponentBody componentBody, Vector3 hitPoint, Vector3 hitDirection, ref float AttackPower, ref float Probability, out bool Hitted)
		{
			Hitted = false;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0002A792 File Offset: 0x00028992
		public virtual bool OnMinerDig(ComponentMiner miner, TerrainRaycastResult raycastResult, ref float DigProgress, out bool Digged)
		{
			Digged = false;
			return false;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0002A799 File Offset: 0x00028999
		public virtual bool ClothingProcessSlotItems(ComponentPlayer componentPlayer, Block block, int slotIndex, int value, int count)
		{
			return false;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0002A79C File Offset: 0x0002899C
		public virtual void OnEatPickable(ComponentEatPickableBehavior eatPickableBehavior, Pickable EatPickable, out bool Dealed)
		{
			Dealed = false;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0002A7A1 File Offset: 0x000289A1
		public virtual bool OnPlayerSpawned(PlayerData.SpawnMode spawnMode, ComponentPlayer componentPlayer, Vector3 position)
		{
			return false;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0002A7A4 File Offset: 0x000289A4
		public virtual void OnPlayerDead(PlayerData playerData)
		{
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x0002A7A6 File Offset: 0x000289A6
		public virtual bool AttackBody(ComponentBody target, ComponentCreature attacker, Vector3 hitPoint, Vector3 hitDirection, ref float attackPower, bool isMeleeAttack)
		{
			return false;
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0002A7A9 File Offset: 0x000289A9
		public virtual void OnSetModel(ComponentModel componentModel, Model model, out bool IsSet)
		{
			IsSet = false;
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0002A7AE File Offset: 0x000289AE
		public virtual void OnModelAnimate(ComponentCreatureModel componentCreatureModel, out bool Skip)
		{
			Skip = false;
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x0002A7B3 File Offset: 0x000289B3
		public virtual float ApplyArmorProtection(ComponentClothing componentClothing, float attackPower, out bool Applied)
		{
			Applied = false;
			return attackPower;
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0002A7B9 File Offset: 0x000289B9
		public virtual void OnLevelUpdate(ComponentLevel level)
		{
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0002A7BB File Offset: 0x000289BB
		public virtual void GuiUpdate(ComponentGui componentGui)
		{
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x0002A7BD File Offset: 0x000289BD
		public virtual void GuiDraw(ComponentGui componentGui, Camera camera, int drawOrder)
		{
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x0002A7BF File Offset: 0x000289BF
		public virtual void DrawToScreen(ViewWidget viewWidget, Widget.DrawContext dc)
		{
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0002A7C1 File Offset: 0x000289C1
		public virtual void ClothingWidgetOpen(ComponentGui componentGui, ClothingWidget clothingWidget)
		{
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x0002A7C3 File Offset: 0x000289C3
		public virtual void OnEntityAdd(Entity entity)
		{
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0002A7C5 File Offset: 0x000289C5
		public virtual void OnEntityRemove(Entity entity)
		{
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x0002A7C7 File Offset: 0x000289C7
		public virtual void InitializeCreatureTypes(SubsystemCreatureSpawn spawn, List<SubsystemCreatureSpawn.CreatureType> creatureTypes)
		{
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x0002A7C9 File Offset: 0x000289C9
		public virtual void SpawnEntity(SubsystemSpawn spawn, Entity entity, SpawnEntityData spawnEntityData, out bool Spawned)
		{
			Spawned = false;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x0002A7CF File Offset: 0x000289CF
		public virtual void OnDespawned(Entity entity, ComponentSpawn componentSpawn)
		{
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x0002A7D1 File Offset: 0x000289D1
		public virtual void JumpToPlace(out bool Pass)
		{
			Pass = false;
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x0002A7D6 File Offset: 0x000289D6
		public virtual void SetShaderParameter(Shader shader, Camera camera)
		{
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x0002A7D8 File Offset: 0x000289D8
		public virtual void ModelShaderParameter(Shader shader, Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData, float? alphaThreshold)
		{
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x0002A7DA File Offset: 0x000289DA
		public virtual void SkyDrawExtra(SubsystemSky subsystemSky, Camera camera)
		{
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0002A7DC File Offset: 0x000289DC
		public virtual int GetMaxInstancesCount()
		{
			return 7;
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0002A7DF File Offset: 0x000289DF
		public virtual void OnModelRendererDrawExtra(SubsystemModelsRenderer modelsRenderer, ComponentModel componentModel, Camera camera, float? alphaThreshold)
		{
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0002A7E1 File Offset: 0x000289E1
		public virtual void SetHitValueParticleSystem(HitValueParticleSystem hitValueParticleSystem, bool Hit)
		{
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0002A7E3 File Offset: 0x000289E3
		public virtual void LoadSpawnsData(SubsystemSpawn spawn, string data, List<SpawnEntityData> creaturesData, out bool Decoded)
		{
			Decoded = false;
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0002A7E9 File Offset: 0x000289E9
		public virtual string SaveSpawnsData(SubsystemSpawn spawn, List<SpawnEntityData> spawnsData, out bool Encoded)
		{
			Encoded = false;
			return "";
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0002A7F3 File Offset: 0x000289F3
		public virtual void OnTerrainContentsGenerated(TerrainChunk chunk)
		{
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0002A7F5 File Offset: 0x000289F5
		public virtual void SubsystemUpdate(float dt)
		{
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0002A7F7 File Offset: 0x000289F7
		public virtual void OnProjectLoaded(Project project)
		{
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0002A7F9 File Offset: 0x000289F9
		public virtual void OnProjectDisposed()
		{
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x0002A7FB File Offset: 0x000289FB
		public virtual void BlocksInitalized()
		{
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0002A7FD File Offset: 0x000289FD
		public virtual object BeforeGameLoading(PlayScreen playScreen, object item)
		{
			return item;
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0002A800 File Offset: 0x00028A00
		public virtual void OnLoadingStart(List<Action> actions)
		{
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0002A802 File Offset: 0x00028A02
		public virtual void OnLoadingFinished(List<Action> actions)
		{
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0002A804 File Offset: 0x00028A04
		public virtual void SaveSettings(XElement xElement)
		{
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0002A806 File Offset: 0x00028A06
		public virtual void LoadSettings(XElement xElement)
		{
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0002A808 File Offset: 0x00028A08
		public virtual void OnXdbLoad(XElement xElement)
		{
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0002A80A File Offset: 0x00028A0A
		public virtual void ProjectXmlLoad(XElement xElement)
		{
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0002A80C File Offset: 0x00028A0C
		public virtual void ProjectXmlSave(XElement xElement)
		{
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0002A80E File Offset: 0x00028A0E
		public virtual void OnCraftingRecipeDecode(List<CraftingRecipe> m_recipes, XElement element, out bool Decoded)
		{
			Decoded = false;
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0002A813 File Offset: 0x00028A13
		public virtual bool MatchRecipe(string[] requiredIngredients, string[] actualIngredient, out bool Matched)
		{
			Matched = false;
			return false;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0002A819 File Offset: 0x00028A19
		public virtual int DecodeResult(string result, out bool Decoded)
		{
			Decoded = false;
			return 0;
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x0002A81F File Offset: 0x00028A1F
		public virtual void DecodeIngredient(string ingredient, out string craftingId, out int? data, out bool Decoded)
		{
			Decoded = false;
			craftingId = string.Empty;
			data = null;
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x0002A833 File Offset: 0x00028A33
		public virtual void OnCameraChange(ComponentPlayer m_componentPlayer, ComponentGui componentGui)
		{
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0002A835 File Offset: 0x00028A35
		public virtual void OnCapture()
		{
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x0002A837 File Offset: 0x00028A37
		public virtual void CallNearbyCreaturesHelp(ComponentHerdBehavior herdBehavior, ComponentCreature target, float maxRange, float maxChaseTime, bool isPersistent)
		{
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x0002A839 File Offset: 0x00028A39
		public virtual void OnTreasureGenerate(SubsystemTerrain subsystemTerrain, int x, int y, int z, int neighborX, int neighborY, int neighborZ, ref int BlockValue, ref int Count, out bool IsGenerate)
		{
			IsGenerate = false;
		}

		// Token: 0x04000339 RID: 825
		public ModEntity Entity;
	}
}
