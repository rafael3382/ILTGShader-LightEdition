using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AB RID: 427
	public class SubsystemGameInfo : Subsystem, IUpdateable
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0004A122 File Offset: 0x00048322
		// (set) Token: 0x06000AF8 RID: 2808 RVA: 0x0004A12A File Offset: 0x0004832A
		public WorldSettings WorldSettings { get; set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x0004A133 File Offset: 0x00048333
		// (set) Token: 0x06000AFA RID: 2810 RVA: 0x0004A13B File Offset: 0x0004833B
		public string DirectoryName { get; set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000AFB RID: 2811 RVA: 0x0004A144 File Offset: 0x00048344
		// (set) Token: 0x06000AFC RID: 2812 RVA: 0x0004A14C File Offset: 0x0004834C
		public double TotalElapsedGameTime { get; set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000AFD RID: 2813 RVA: 0x0004A155 File Offset: 0x00048355
		// (set) Token: 0x06000AFE RID: 2814 RVA: 0x0004A15D File Offset: 0x0004835D
		public float TotalElapsedGameTimeDelta { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x0004A166 File Offset: 0x00048366
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x0004A16E File Offset: 0x0004836E
		public int WorldSeed { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000B01 RID: 2817 RVA: 0x0004A177 File Offset: 0x00048377
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x0004A17A File Offset: 0x0004837A
		public IEnumerable<ActiveExternalContentInfo> GetActiveExternalContent()
		{
			string downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.World, this.DirectoryName);
			if (!string.IsNullOrEmpty(downloadedContentAddress))
			{
				yield return new ActiveExternalContentInfo
				{
					Address = downloadedContentAddress,
					DisplayName = this.WorldSettings.Name,
					Type = ExternalContentType.World
				};
			}
			if (!BlocksTexturesManager.IsBuiltIn(this.WorldSettings.BlocksTextureName))
			{
				downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.BlocksTexture, this.WorldSettings.BlocksTextureName);
				if (!string.IsNullOrEmpty(downloadedContentAddress))
				{
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = BlocksTexturesManager.GetDisplayName(this.WorldSettings.BlocksTextureName),
						Type = ExternalContentType.BlocksTexture
					};
				}
			}
			SubsystemPlayers subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			foreach (PlayerData playerData in subsystemPlayers.PlayersData)
			{
				if (!CharacterSkinsManager.IsBuiltIn(playerData.CharacterSkinName))
				{
					downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.CharacterSkin, playerData.CharacterSkinName);
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = CharacterSkinsManager.GetDisplayName(playerData.CharacterSkinName),
						Type = ExternalContentType.CharacterSkin
					};
				}
			}
			ReadOnlyList<PlayerData>.Enumerator enumerator = default(ReadOnlyList<PlayerData>.Enumerator);
			SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			foreach (FurnitureSet furnitureSet in subsystemFurnitureBlockBehavior.FurnitureSets)
			{
				if (furnitureSet.ImportedFrom != null)
				{
					downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.FurniturePack, furnitureSet.ImportedFrom);
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = FurniturePacksManager.GetDisplayName(furnitureSet.ImportedFrom),
						Type = ExternalContentType.FurniturePack
					};
				}
			}
			ReadOnlyList<FurnitureSet>.Enumerator enumerator2 = default(ReadOnlyList<FurnitureSet>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x0004A18C File Offset: 0x0004838C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.WorldSettings = new WorldSettings();
			this.WorldSettings.Load(valuesDictionary);
			this.DirectoryName = valuesDictionary.GetValue<string>("WorldDirectoryName");
			this.TotalElapsedGameTime = valuesDictionary.GetValue<double>("TotalElapsedGameTime");
			this.WorldSeed = valuesDictionary.GetValue<int>("WorldSeed");
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0004A1F5 File Offset: 0x000483F5
		public override void Save(ValuesDictionary valuesDictionary)
		{
			this.WorldSettings.Save(valuesDictionary, false);
			valuesDictionary.SetValue<int>("WorldSeed", this.WorldSeed);
			valuesDictionary.SetValue<double>("TotalElapsedGameTime", this.TotalElapsedGameTime);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x0004A228 File Offset: 0x00048428
		public void Update(float dt)
		{
			this.TotalElapsedGameTime += (double)dt;
			this.TotalElapsedGameTimeDelta = ((this.m_lastTotalElapsedGameTime != null) ? ((float)(this.TotalElapsedGameTime - this.m_lastTotalElapsedGameTime.Value)) : 0f);
			this.m_lastTotalElapsedGameTime = new double?(this.TotalElapsedGameTime);
			if (this.m_subsystemTime.GameTime >= 600.0 && this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta < 600.0 && UserManager.ActiveUser != null)
			{
				foreach (ActiveExternalContentInfo activeExternalContentInfo in this.GetActiveExternalContent())
				{
					CommunityContentManager.SendPlayTime(activeExternalContentInfo.Address, UserManager.ActiveUser.UniqueId, this.m_subsystemTime.GameTime, null, delegate
					{
					}, delegate
					{
					});
				}
			}
		}

		// Token: 0x04000558 RID: 1368
		public double? m_lastTotalElapsedGameTime;

		// Token: 0x04000559 RID: 1369
		public SubsystemTime m_subsystemTime;
	}
}
