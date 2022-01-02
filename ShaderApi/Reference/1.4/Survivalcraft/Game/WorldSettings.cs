using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200035F RID: 863
	public class WorldSettings
	{
		// Token: 0x06001928 RID: 6440 RVA: 0x000C5DA4 File Offset: 0x000C3FA4
		public void ResetOptionsForNonCreativeMode()
		{
			if (this.TerrainGenerationMode == TerrainGenerationMode.FlatContinent)
			{
				this.TerrainGenerationMode = TerrainGenerationMode.Continent;
			}
			if (this.TerrainGenerationMode == TerrainGenerationMode.FlatIsland)
			{
				this.TerrainGenerationMode = TerrainGenerationMode.Island;
			}
			this.EnvironmentBehaviorMode = EnvironmentBehaviorMode.Living;
			this.TimeOfDayMode = TimeOfDayMode.Changing;
			this.AreWeatherEffectsEnabled = true;
			this.IsAdventureRespawnAllowed = true;
			this.AreAdventureSurvivalMechanicsEnabled = true;
			this.TerrainLevel = 64;
			this.ShoreRoughness = 0.5f;
			this.TerrainBlockIndex = 8;
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x000C5E10 File Offset: 0x000C4010
		public void Load(ValuesDictionary valuesDictionary)
		{
			this.Name = valuesDictionary.GetValue<string>("WorldName");
			this.OriginalSerializationVersion = valuesDictionary.GetValue<string>("OriginalSerializationVersion", string.Empty);
			this.Seed = valuesDictionary.GetValue<string>("WorldSeedString", string.Empty);
			this.GameMode = valuesDictionary.GetValue<GameMode>("GameMode", GameMode.Challenging);
			this.EnvironmentBehaviorMode = valuesDictionary.GetValue<EnvironmentBehaviorMode>("EnvironmentBehaviorMode", EnvironmentBehaviorMode.Living);
			this.TimeOfDayMode = valuesDictionary.GetValue<TimeOfDayMode>("TimeOfDayMode", TimeOfDayMode.Changing);
			this.StartingPositionMode = valuesDictionary.GetValue<StartingPositionMode>("StartingPositionMode", StartingPositionMode.Easy);
			this.AreWeatherEffectsEnabled = valuesDictionary.GetValue<bool>("AreWeatherEffectsEnabled", true);
			this.IsAdventureRespawnAllowed = valuesDictionary.GetValue<bool>("IsAdventureRespawnAllowed", true);
			this.AreAdventureSurvivalMechanicsEnabled = valuesDictionary.GetValue<bool>("AreAdventureSurvivalMechanicsEnabled", true);
			this.AreSupernaturalCreaturesEnabled = valuesDictionary.GetValue<bool>("AreSupernaturalCreaturesEnabled", true);
			this.IsFriendlyFireEnabled = valuesDictionary.GetValue<bool>("IsFriendlyFireEnabled", true);
			this.TerrainGenerationMode = valuesDictionary.GetValue<TerrainGenerationMode>("TerrainGenerationMode", TerrainGenerationMode.Continent);
			this.IslandSize = valuesDictionary.GetValue<Vector2>("IslandSize", new Vector2(200f, 200f));
			this.TerrainLevel = valuesDictionary.GetValue<int>("TerrainLevel", 64);
			this.ShoreRoughness = valuesDictionary.GetValue<float>("ShoreRoughness", 0f);
			this.TerrainBlockIndex = valuesDictionary.GetValue<int>("TerrainBlockIndex", 8);
			this.TerrainOceanBlockIndex = valuesDictionary.GetValue<int>("TerrainOceanBlockIndex", 18);
			this.TemperatureOffset = valuesDictionary.GetValue<float>("TemperatureOffset", 0f);
			this.HumidityOffset = valuesDictionary.GetValue<float>("HumidityOffset", 0f);
			this.SeaLevelOffset = valuesDictionary.GetValue<int>("SeaLevelOffset", 0);
			this.BiomeSize = valuesDictionary.GetValue<float>("BiomeSize", 1f);
			this.BlocksTextureName = valuesDictionary.GetValue<string>("BlockTextureName", string.Empty);
			this.Palette = new WorldPalette(valuesDictionary.GetValue<ValuesDictionary>("Palette", new ValuesDictionary()));
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x000C6004 File Offset: 0x000C4204
		public void Save(ValuesDictionary valuesDictionary, bool liveModifiableParametersOnly)
		{
			valuesDictionary.SetValue<string>("WorldName", this.Name);
			valuesDictionary.SetValue<string>("OriginalSerializationVersion", this.OriginalSerializationVersion);
			valuesDictionary.SetValue<GameMode>("GameMode", this.GameMode);
			valuesDictionary.SetValue<EnvironmentBehaviorMode>("EnvironmentBehaviorMode", this.EnvironmentBehaviorMode);
			valuesDictionary.SetValue<TimeOfDayMode>("TimeOfDayMode", this.TimeOfDayMode);
			valuesDictionary.SetValue<bool>("AreWeatherEffectsEnabled", this.AreWeatherEffectsEnabled);
			valuesDictionary.SetValue<bool>("IsAdventureRespawnAllowed", this.IsAdventureRespawnAllowed);
			valuesDictionary.SetValue<bool>("AreAdventureSurvivalMechanicsEnabled", this.AreAdventureSurvivalMechanicsEnabled);
			valuesDictionary.SetValue<bool>("AreSupernaturalCreaturesEnabled", this.AreSupernaturalCreaturesEnabled);
			valuesDictionary.SetValue<bool>("IsFriendlyFireEnabled", this.IsFriendlyFireEnabled);
			if (!liveModifiableParametersOnly)
			{
				valuesDictionary.SetValue<string>("WorldSeedString", this.Seed);
				valuesDictionary.SetValue<TerrainGenerationMode>("TerrainGenerationMode", this.TerrainGenerationMode);
				valuesDictionary.SetValue<Vector2>("IslandSize", this.IslandSize);
				valuesDictionary.SetValue<int>("TerrainLevel", this.TerrainLevel);
				valuesDictionary.SetValue<float>("ShoreRoughness", this.ShoreRoughness);
				valuesDictionary.SetValue<int>("TerrainBlockIndex", this.TerrainBlockIndex);
				valuesDictionary.SetValue<int>("TerrainOceanBlockIndex", this.TerrainOceanBlockIndex);
				valuesDictionary.SetValue<float>("TemperatureOffset", this.TemperatureOffset);
				valuesDictionary.SetValue<float>("HumidityOffset", this.HumidityOffset);
				valuesDictionary.SetValue<int>("SeaLevelOffset", this.SeaLevelOffset);
				valuesDictionary.SetValue<float>("BiomeSize", this.BiomeSize);
				valuesDictionary.SetValue<StartingPositionMode>("StartingPositionMode", this.StartingPositionMode);
			}
			valuesDictionary.SetValue<string>("BlockTextureName", this.BlocksTextureName);
			valuesDictionary.SetValue<ValuesDictionary>("Palette", this.Palette.Save());
		}

		// Token: 0x04001146 RID: 4422
		public string Name = string.Empty;

		// Token: 0x04001147 RID: 4423
		public string OriginalSerializationVersion = string.Empty;

		// Token: 0x04001148 RID: 4424
		public string Seed = string.Empty;

		// Token: 0x04001149 RID: 4425
		public GameMode GameMode = GameMode.Challenging;

		// Token: 0x0400114A RID: 4426
		public EnvironmentBehaviorMode EnvironmentBehaviorMode;

		// Token: 0x0400114B RID: 4427
		public TimeOfDayMode TimeOfDayMode;

		// Token: 0x0400114C RID: 4428
		public StartingPositionMode StartingPositionMode;

		// Token: 0x0400114D RID: 4429
		public bool AreWeatherEffectsEnabled = true;

		// Token: 0x0400114E RID: 4430
		public bool IsAdventureRespawnAllowed = true;

		// Token: 0x0400114F RID: 4431
		public bool AreAdventureSurvivalMechanicsEnabled = true;

		// Token: 0x04001150 RID: 4432
		public bool AreSupernaturalCreaturesEnabled = true;

		// Token: 0x04001151 RID: 4433
		public bool IsFriendlyFireEnabled = true;

		// Token: 0x04001152 RID: 4434
		public TerrainGenerationMode TerrainGenerationMode;

		// Token: 0x04001153 RID: 4435
		public Vector2 IslandSize = new Vector2(400f, 400f);

		// Token: 0x04001154 RID: 4436
		public float BiomeSize = 1f;

		// Token: 0x04001155 RID: 4437
		public int TerrainLevel = 64;

		// Token: 0x04001156 RID: 4438
		public float ShoreRoughness = 0.5f;

		// Token: 0x04001157 RID: 4439
		public int TerrainBlockIndex = 8;

		// Token: 0x04001158 RID: 4440
		public int TerrainOceanBlockIndex = 18;

		// Token: 0x04001159 RID: 4441
		public float TemperatureOffset;

		// Token: 0x0400115A RID: 4442
		public float HumidityOffset;

		// Token: 0x0400115B RID: 4443
		public int SeaLevelOffset;

		// Token: 0x0400115C RID: 4444
		public string BlocksTextureName = string.Empty;

		// Token: 0x0400115D RID: 4445
		public WorldPalette Palette = new WorldPalette();
	}
}
