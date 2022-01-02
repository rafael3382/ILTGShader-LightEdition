using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000198 RID: 408
	public class SubsystemCreatureSpawn : Subsystem, IUpdateable
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x0003BD8D File Offset: 0x00039F8D
		public Dictionary<ComponentCreature, bool>.KeyCollection Creatures
		{
			get
			{
				return this.m_creatures.Keys;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000982 RID: 2434 RVA: 0x0003BD9A File Offset: 0x00039F9A
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0003BDA0 File Offset: 0x00039FA0
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living)
			{
				if (this.m_newSpawnChunks.Count > 0)
				{
					this.m_newSpawnChunks.RandomShuffle((int max) => this.m_random.Int(0, max - 1));
					foreach (SpawnChunk chunk in this.m_newSpawnChunks)
					{
						this.SpawnChunkCreatures(chunk, 10, false);
					}
					this.m_newSpawnChunks.Clear();
				}
				if (this.m_spawnChunks.Count > 0)
				{
					this.m_spawnChunks.RandomShuffle((int max) => this.m_random.Int(0, max - 1));
					foreach (SpawnChunk chunk2 in this.m_spawnChunks)
					{
						this.SpawnChunkCreatures(chunk2, 2, true);
					}
					this.m_spawnChunks.Clear();
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 2.0))
				{
					this.SpawnRandomCreature();
				}
			}
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0003BED4 File Offset: 0x0003A0D4
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSpawn = base.Project.FindSubsystem<SubsystemSpawn>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.InitializeCreatureTypes();
			SubsystemSpawn subsystemSpawn = this.m_subsystemSpawn;
			subsystemSpawn.SpawningChunk = (Action<SpawnChunk>)Delegate.Combine(subsystemSpawn.SpawningChunk, new Action<SpawnChunk>(delegate(SpawnChunk chunk)
			{
				this.m_spawnChunks.Add(chunk);
				if (!chunk.IsSpawned)
				{
					this.m_newSpawnChunks.Add(chunk);
				}
			}));
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0003BF8C File Offset: 0x0003A18C
		public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentCreature key in entity.FindComponents<ComponentCreature>())
			{
				this.m_creatures.Add(key, true);
			}
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0003BFE8 File Offset: 0x0003A1E8
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentCreature key in entity.FindComponents<ComponentCreature>())
			{
				this.m_creatures.Remove(key);
			}
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0003C044 File Offset: 0x0003A244
		public virtual void InitializeCreatureTypes()
		{
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Duck", SpawnLocationType.Surface, true, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					int topHeight = this.m_subsystemTerrain.Terrain.GetTopHeight(point.X, point.Z);
					if (humidity <= 8 || temperature <= 4 || num <= 30f || point.Y < topHeight || (!(BlocksManager.Blocks[num2] is LeavesBlock) && num2 != 18 && num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 2.5f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Duck", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Raven", SpawnLocationType.Surface, true, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					int topHeight = this.m_subsystemTerrain.Terrain.GetTopHeight(point.X, point.Z);
					if ((humidity > 8 && temperature > 4) || num <= 30f || point.Y < topHeight || (!(BlocksManager.Blocks[num2] is LeavesBlock) && num2 != 62 && num2 != 8 && num2 != 2 && num2 != 7))
					{
						return 0f;
					}
					return 2.5f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Raven", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Seagull", SpawnLocationType.Surface, true, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					int topHeight = this.m_subsystemTerrain.Terrain.GetTopHeight(point.X, point.Z);
					if (num <= -100f || num >= 40f || point.Y < topHeight || (num2 != 18 && num2 != 7 && num2 != 6 && num2 != 62))
					{
						return 0f;
					}
					return 2.5f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Seagull", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Wildboar", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || humidity <= 8 || point.Y >= 80 || (num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 0.25f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Wildboar", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Brown Cattle", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || humidity <= 4 || temperature < 8 || point.Y >= 70 || (num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = this.m_random.Int(3, 5);
					int num2 = MathUtils.Min(this.m_random.Int(1, 3), num);
					int count = num - num2;
					return this.SpawnCreatures(creatureType, "Bull_Brown", point, num2).Count + this.SpawnCreatures(creatureType, "Cow_Brown", point, count).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Black Cattle", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || humidity <= 4 || temperature >= 8 || point.Y >= 70 || (num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = this.m_random.Int(3, 5);
					int num2 = MathUtils.Min(this.m_random.Int(1, 3), num);
					int count = num - num2;
					return this.SpawnCreatures(creatureType, "Bull_Black", point, num2).Count + this.SpawnCreatures(creatureType, "Cow_Black", point, count).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("White Bull", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || humidity <= 8 || temperature >= 4 || point.Y >= 70 || (num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 0.01f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bull_White", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Gray Wolves", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || humidity < 8 || point.Y >= 100 || (num2 != 8 && num2 != 2))
					{
						return 0f;
					}
					return 0.075f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Wolf_Gray", point, this.m_random.Int(1, 3)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Coyotes", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature <= 8 || humidity >= 8 || humidity < 2 || point.Y >= 100 || num2 != 7)
					{
						return 0f;
					}
					return 0.075f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Wolf_Coyote", point, this.m_random.Int(1, 3)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Brown Bears", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || humidity < 4 || temperature < 8 || point.Y >= 110 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Brown", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Black Bears", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || humidity < 4 || temperature >= 8 || point.Y >= 120 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Black", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Polar Bears", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= -40f || temperature >= 8 || point.Y >= 80 || num2 != 62)
					{
						return 0f;
					}
					return 0.1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Polar", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Horses", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 3 || humidity <= 6 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num = 0;
					if (this.m_random.Float(0f, 1f) < 0.35f)
					{
						num += this.SpawnCreatures(creatureType, "Horse_Black", point, 1).Count;
					}
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						num += this.SpawnCreatures(creatureType, "Horse_Bay", point, 1).Count;
					}
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						num += this.SpawnCreatures(creatureType, "Horse_Chestnut", point, 1).Count;
					}
					if (temperature > 8 && this.m_random.Float(0f, 1f) < 0.3f)
					{
						num += this.SpawnCreatures(creatureType, "Horse_Palomino", point, 1).Count;
					}
					if (temperature < 8 && this.m_random.Float(0f, 1f) < 0.3f)
					{
						num += this.SpawnCreatures(creatureType, "Horse_White", point, 1).Count;
					}
					return num;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Camels", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || humidity >= 8 || point.Y >= 80 || num2 != 7)
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Camel", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Donkeys", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 6 || point.Y >= 120 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Donkey", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Giraffes", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || humidity <= 7 || point.Y >= 75 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.03f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Giraffe", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Rhinos", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature <= 8 || humidity <= 7 || point.Y >= 75 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.04f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Rhino", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Tigers", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || humidity <= 8 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7))
					{
						return 0f;
					}
					return 0.025f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Tiger", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("White Tigers", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature >= 2 || point.Y >= 90 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7 && num2 != 62))
					{
						return 0f;
					}
					return 0.025f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Tiger_White", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Lions", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature <= 8 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Lion", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Jaguars", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || humidity <= 8 || temperature <= 8 || point.Y >= 100 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7 && num2 != 12))
					{
						return 0f;
					}
					return 0.04f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Jaguar", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Leopards", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature <= 8 || point.Y >= 120 || (num2 != 8 && num2 != 2 && num2 != 3 && num2 != 7 && num2 != 12))
					{
						return 0f;
					}
					return 0.04f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Leopard", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Zebras", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || humidity <= 7 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Zebra", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Gnus", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 3))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Gnu", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Reindeers", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (temperature >= 3 || point.Y >= 90 || (num != 8 && num != 2 && num != 3 && num != 62))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Reindeer", point, this.m_random.Int(1, 3)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Mooses", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (temperature >= 7 || point.Y >= 90 || (num != 8 && num != 2 && num != 3 && num != 62))
					{
						return 0f;
					}
					return 0.1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Moose", point, this.m_random.Int(1, 1)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Bisons", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (temperature >= 10 || humidity >= 12 || point.Y >= 80 || (num != 8 && num != 2 && num != 3 && num != 62))
					{
						return 0f;
					}
					return 0.1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bison", point, this.m_random.Int(1, 4)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Ostriches", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || humidity >= 8 || point.Y >= 75 || (num2 != 8 && num2 != 2 && num2 != 7))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Ostrich", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cassowaries", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 20f || temperature <= 8 || humidity >= 12 || point.Y >= 75 || (num2 != 8 && num2 != 2 && num2 != 7))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Cassowary", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Hyenas", SpawnLocationType.Surface, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num <= 40f || temperature <= 8 || point.Y >= 80 || (num2 != 8 && num2 != 2 && num2 != 7))
					{
						return 0f;
					}
					return 0.05f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Hyena", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Bears", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7)
					{
						return 0f;
					}
					return 1f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = (this.m_random.Int(0, 1) == 0) ? "Bear_Black" : "Bear_Brown";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Tigers", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7)
					{
						return 0f;
					}
					return 0.25f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Tiger", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Lions", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if ((num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7) || temperature <= 8 || humidity >= 8)
					{
						return 0f;
					}
					return 0.25f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Lion", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Jaguars", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7)
					{
						return 0f;
					}
					return 0.5f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Jaguar", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Leopards", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if (num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7)
					{
						return 0f;
					}
					return 0.25f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Leopard", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Cave Hyenas", SpawnLocationType.Cave, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					int num = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
					if ((num != 3 && num != 67 && num != 4 && num != 66 && num != 2 && num != 7) || temperature <= 8)
					{
						return 0f;
					}
					return 1f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Hyena", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Bull Sharks", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < -2f)
					{
						return 0.4f;
					}
					return 0f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Shark_Bull";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Tiger Sharks", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < -5f)
					{
						return 0.3f;
					}
					return 0f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Shark_Tiger";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Great White Sharks", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < -20f)
					{
						return 0.2f;
					}
					return 0f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Shark_GreatWhite";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Barracudas", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < -2f)
					{
						return 0.5f;
					}
					return 0f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Barracuda";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Bass_Sea", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < -2f)
					{
						return 1f;
					}
					return 0f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Bass_Sea";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Bass_Freshwater", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					if (num <= 10f || temperature < 4)
					{
						return 0f;
					}
					return 1f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Bass_Freshwater";
					return this.SpawnCreatures(creatureType, templateName, point, this.m_random.Int(1, 2)).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Rays", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z) < 10f)
					{
						return 0.5f;
					}
					return 1f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					int num = 0;
					int num2 = 0;
					for (int i = point.X - 2; i <= point.X + 2; i++)
					{
						for (int j = point.Z - 2; j <= point.Z + 2; j++)
						{
							if (this.m_subsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 18)
							{
								for (int k = point.Y - 1; k > 0; k--)
								{
									int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(point.X, k, point.Z);
									if (cellContents == 2)
									{
										num++;
										break;
									}
									if (cellContents == 7)
									{
										num2++;
										break;
									}
								}
							}
						}
					}
					string templateName = (num >= num2) ? "Ray_Brown" : "Ray_Yellow";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Piranhas", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
					int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
					if (num <= 10f || humidity < 4 || temperature < 7)
					{
						return 0f;
					}
					return 1f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Piranha";
					return this.SpawnCreatures(creatureType, templateName, point, this.m_random.Int(2, 4)).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Orcas", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					if (num < -100f)
					{
						return 0.05f;
					}
					if (num >= -20f)
					{
						return 0f;
					}
					return 0.01f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Orca";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Belugas", SpawnLocationType.Water, false, false)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
					if (num < -100f)
					{
						return 0.05f;
					}
					if (num >= -20f)
					{
						return 0f;
					}
					return 0.01f;
				},
				SpawnFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					string templateName = "Beluga";
					return this.SpawnCreatures(creatureType, templateName, point, 1).Count;
				}
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Gray Wolves", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						float num2 = (float)this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num3 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (((num > 20f && humidity >= 8) || (num2 <= 8f && point.Y < 90 && cellLightFast <= 7)) && (num3 == 8 || num3 == 2))
						{
							return 2f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Wolf_Gray", point, this.m_random.Int(1, 3)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Coyotes", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						float num2 = (float)this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						float num3 = (float)this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num4 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && num3 > 8f && num2 < 8f && point.Y < 90 && cellLightFast <= 7 && num4 == 7)
						{
							return 2f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Wolf_Coyote", point, this.m_random.Int(1, 3)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Brown Bears", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && humidity >= 4 && temperature >= 8 && point.Y < 100 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3))
						{
							return 0.5f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Brown", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Black Bears", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && temperature < 8 && point.Y < 110 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3))
						{
							return 0.5f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Black", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Polar Bears", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > -40f && temperature < 8 && point.Y < 90 && cellLightFast <= 7 && num2 == 62)
						{
							return 0.25f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Bear_Black", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Tigers", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && humidity > 8 && point.Y < 90 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3))
						{
							return 0.05f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Tiger", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Lions", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && temperature > 8 && point.Y < 90 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3 || num2 == 7))
						{
							return 0.25f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Lion", point, this.m_random.Int(1, 2)).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Jaguars", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int humidity = this.m_subsystemTerrain.Terrain.GetHumidity(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && temperature > 8 && humidity > 8 && point.Y < 100 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3 || num2 == 12))
						{
							return 0.25f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Jaguar", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Leopards", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && temperature > 8 && point.Y < 110 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3 || num2 == 12))
						{
							return 0.25f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Leopard", point, 1).Count)
			});
			this.m_creatureTypes.Add(new SubsystemCreatureSpawn.CreatureType("Constant Hyenas", SpawnLocationType.Surface, false, true)
			{
				SpawnSuitabilityFunction = delegate(SubsystemCreatureSpawn.CreatureType creatureType, Point3 point)
				{
					if (this.m_subsystemSky.SkyLightIntensity < 0.1f)
					{
						float num = this.m_subsystemTerrain.TerrainContentsGenerator.CalculateOceanShoreDistance((float)point.X, (float)point.Z);
						int temperature = this.m_subsystemTerrain.Terrain.GetTemperature(point.X, point.Z);
						int num2 = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValueFast(point.X, point.Y - 1, point.Z));
						int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(point.X, point.Y + 1, point.Z);
						if (num > 20f && temperature > 8 && point.Y < 100 && cellLightFast <= 7 && (num2 == 8 || num2 == 2 || num2 == 3 || num2 == 7))
						{
							return 1f;
						}
					}
					return 0f;
				},
				SpawnFunction = ((SubsystemCreatureSpawn.CreatureType creatureType, Point3 point) => this.SpawnCreatures(creatureType, "Hyena", point, this.m_random.Int(1, 2)).Count)
			});
			ModsManager.HookAction("InitializeCreatureTypes", delegate(ModLoader modLoader)
			{
				modLoader.InitializeCreatureTypes(this, this.m_creatureTypes);
				return false;
			});
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0003CD88 File Offset: 0x0003AF88
		public virtual void SpawnRandomCreature()
		{
			if (this.CountCreatures(false) < 24)
			{
				foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
				{
					int num = 48;
					Vector2 v = new Vector2(gameWidget.ActiveCamera.ViewPosition.X, gameWidget.ActiveCamera.ViewPosition.Z);
					if (this.CountCreaturesInArea(v - new Vector2(60f), v + new Vector2(60f), false) >= num)
					{
						break;
					}
					SpawnLocationType spawnLocationType = this.GetRandomSpawnLocationType();
					Point3? spawnPoint = this.GetRandomSpawnPoint(gameWidget.ActiveCamera, spawnLocationType);
					if (spawnPoint != null)
					{
						Vector2 c3 = new Vector2((float)spawnPoint.Value.X, (float)spawnPoint.Value.Z) - new Vector2(16f);
						Vector2 c2 = new Vector2((float)spawnPoint.Value.X, (float)spawnPoint.Value.Z) + new Vector2(16f);
						if (this.CountCreaturesInArea(c3, c2, false) >= 3)
						{
							break;
						}
						IEnumerable<SubsystemCreatureSpawn.CreatureType> source = from c in this.m_creatureTypes
						where c.SpawnLocationType == spawnLocationType && c.RandomSpawn
						select c;
						IEnumerable<float> items = from c in source
						select c.SpawnSuitabilityFunction(c, spawnPoint.Value);
						int randomWeightedItem = this.GetRandomWeightedItem(items);
						if (randomWeightedItem >= 0)
						{
							SubsystemCreatureSpawn.CreatureType creatureType = source.ElementAt(randomWeightedItem);
							creatureType.SpawnFunction(creatureType, spawnPoint.Value);
						}
					}
				}
			}
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0003CF78 File Offset: 0x0003B178
		public virtual void SpawnChunkCreatures(SpawnChunk chunk, int maxAttempts, bool constantSpawn)
		{
			int num = constantSpawn ? 18 : 24;
			int num2 = constantSpawn ? 4 : 3;
			float v = (float)(constantSpawn ? 42 : 16);
			int num3 = this.CountCreatures(constantSpawn);
			Vector2 c3 = new Vector2((float)(chunk.Point.X * 16), (float)(chunk.Point.Y * 16)) - new Vector2(v);
			Vector2 c2 = new Vector2((float)((chunk.Point.X + 1) * 16), (float)((chunk.Point.Y + 1) * 16)) + new Vector2(v);
			int num4 = this.CountCreaturesInArea(c3, c2, constantSpawn);
			for (int i = 0; i < maxAttempts; i++)
			{
				if (num3 >= num || num4 >= num2)
				{
					break;
				}
				SpawnLocationType spawnLocationType = this.GetRandomSpawnLocationType();
				Point3? spawnPoint = this.GetRandomChunkSpawnPoint(chunk, spawnLocationType);
				if (spawnPoint != null)
				{
					IEnumerable<SubsystemCreatureSpawn.CreatureType> source = from c in this.m_creatureTypes
					where c.SpawnLocationType == spawnLocationType && c.ConstantSpawn == constantSpawn
					select c;
					IEnumerable<float> items = from c in source
					select c.SpawnSuitabilityFunction(c, spawnPoint.Value);
					int randomWeightedItem = this.GetRandomWeightedItem(items);
					if (randomWeightedItem >= 0)
					{
						SubsystemCreatureSpawn.CreatureType creatureType = source.ElementAt(randomWeightedItem);
						int num5 = creatureType.SpawnFunction(creatureType, spawnPoint.Value);
						num3 += num5;
						num4 += num5;
					}
				}
			}
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0003D11C File Offset: 0x0003B31C
		public virtual List<Entity> SpawnCreatures(SubsystemCreatureSpawn.CreatureType creatureType, string templateName, Point3 point, int count)
		{
			List<Entity> list = new List<Entity>();
			int num = 0;
			while (count > 0 && num < 50)
			{
				Point3 spawnPoint = point;
				if (num > 0)
				{
					spawnPoint.X += this.m_random.Int(-8, 8);
					spawnPoint.Y += this.m_random.Int(-4, 8);
					spawnPoint.Z += this.m_random.Int(-8, 8);
				}
				Point3? point2 = this.ProcessSpawnPoint(spawnPoint, creatureType.SpawnLocationType);
				if (point2 != null && creatureType.SpawnSuitabilityFunction(creatureType, point2.Value) > 0f)
				{
					Vector3 position = new Vector3((float)point2.Value.X + this.m_random.Float(0.4f, 0.6f), (float)point2.Value.Y + 1.1f, (float)point2.Value.Z + this.m_random.Float(0.4f, 0.6f));
					Entity entity = this.SpawnCreature(templateName, position, creatureType.ConstantSpawn);
					if (entity != null)
					{
						list.Add(entity);
						count--;
					}
				}
				num++;
			}
			return list;
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0003D254 File Offset: 0x0003B454
		public virtual Entity SpawnCreature(string templateName, Vector3 position, bool constantSpawn)
		{
			Entity result;
			try
			{
				Entity entity = DatabaseManager.CreateEntity(base.Project, templateName, true);
				entity.FindComponent<ComponentBody>(true).Position = position;
				entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.28318548f));
				entity.FindComponent<ComponentCreature>(true).ConstantSpawn = constantSpawn;
				base.Project.AddEntity(entity);
				result = entity;
			}
			catch (Exception ex)
			{
				Log.Error("Unable to spawn creature with template \"" + templateName + "\". Reason: " + ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0003D2F8 File Offset: 0x0003B4F8
		public virtual Point3? GetRandomChunkSpawnPoint(SpawnChunk chunk, SpawnLocationType spawnLocationType)
		{
			for (int i = 0; i < 5; i++)
			{
				int x = 16 * chunk.Point.X + this.m_random.Int(0, 15);
				int y = this.m_random.Int(10, 246);
				int z = 16 * chunk.Point.Y + this.m_random.Int(0, 15);
				Point3? result = this.ProcessSpawnPoint(new Point3(x, y, z), spawnLocationType);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0003D388 File Offset: 0x0003B588
		public virtual Point3? GetRandomSpawnPoint(Camera camera, SpawnLocationType spawnLocationType)
		{
			for (int i = 0; i < 10; i++)
			{
				int x = Terrain.ToCell(camera.ViewPosition.X) + this.m_random.Sign() * this.m_random.Int(20, 40);
				int y = MathUtils.Clamp(Terrain.ToCell(camera.ViewPosition.Y) + this.m_random.Int(-30, 30), 2, 254);
				int z = Terrain.ToCell(camera.ViewPosition.Z) + this.m_random.Sign() * this.m_random.Int(20, 40);
				Point3? result = this.ProcessSpawnPoint(new Point3(x, y, z), spawnLocationType);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0003D458 File Offset: 0x0003B658
		public virtual Point3? ProcessSpawnPoint(Point3 spawnPoint, SpawnLocationType spawnLocationType)
		{
			int x = spawnPoint.X;
			int num = MathUtils.Clamp(spawnPoint.Y, 1, 254);
			int z = spawnPoint.Z;
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
			if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidPropagatedLight)
			{
				for (int i = 0; i < 30; i++)
				{
					Point3 point = new Point3(x, num + i, z);
					if (this.TestSpawnPoint(point, spawnLocationType))
					{
						return new Point3?(point);
					}
					Point3 point2 = new Point3(x, num - i, z);
					if (this.TestSpawnPoint(point2, spawnLocationType))
					{
						return new Point3?(point2);
					}
				}
			}
			return null;
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0003D500 File Offset: 0x0003B700
		public virtual bool TestSpawnPoint(Point3 spawnPoint, SpawnLocationType spawnLocationType)
		{
			int x = spawnPoint.X;
			int y = spawnPoint.Y;
			int z = spawnPoint.Z;
			if (y <= 3 || y >= 253)
			{
				return false;
			}
			switch (spawnLocationType)
			{
			case SpawnLocationType.Surface:
			{
				int cellLightFast = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, y, z);
				if (this.m_subsystemSky.SkyLightValue - cellLightFast > 3)
				{
					return false;
				}
				int cellValueFast = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y - 1, z);
				int cellValueFast2 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				int cellValueFast3 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast)];
				Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast2)];
				Block block3 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast3)];
				return (block.IsCollidable_(cellValueFast) || block is WaterBlock) && !block2.IsCollidable_(cellValueFast2) && !(block2 is WaterBlock) && !block3.IsCollidable_(cellValueFast3) && !(block3 is WaterBlock);
			}
			case SpawnLocationType.Cave:
			{
				int cellLightFast2 = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, y, z);
				if (this.m_subsystemSky.SkyLightValue - cellLightFast2 < 5)
				{
					return false;
				}
				int cellValueFast4 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y - 1, z);
				int cellValueFast5 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y, z);
				int cellValueFast6 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z);
				Block block4 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast4)];
				Block block5 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast5)];
				Block block6 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast6)];
				return (block4.IsCollidable_(cellValueFast4) || block4 is WaterBlock) && !block5.IsCollidable_(cellValueFast5) && !(block5 is WaterBlock) && !block6.IsCollidable_(cellValueFast6) && !(block6 is WaterBlock);
			}
			case SpawnLocationType.Water:
			{
				int cellContentsFast = this.m_subsystemTerrain.Terrain.GetCellContentsFast(x, y, z);
				int cellValueFast7 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y + 1, z);
				int cellValueFast8 = this.m_subsystemTerrain.Terrain.GetCellValueFast(x, y + 2, z);
				Block block7 = BlocksManager.Blocks[Terrain.ExtractContents(cellContentsFast)];
				Block block8 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast7)];
				Block block9 = BlocksManager.Blocks[Terrain.ExtractContents(cellValueFast8)];
				return block7 is WaterBlock && !block8.IsCollidable_(cellValueFast7) && !block9.IsCollidable_(cellValueFast8);
			}
			default:
				throw new InvalidOperationException("Unknown spawn location type.");
			}
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0003D794 File Offset: 0x0003B994
		public virtual int CountCreatures(bool constantSpawn)
		{
			int num = 0;
			foreach (ComponentBody componentBody in this.m_subsystemBodies.Bodies)
			{
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && componentCreature.ConstantSpawn == constantSpawn)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0003D804 File Offset: 0x0003BA04
		public virtual int CountCreaturesInArea(Vector2 c1, Vector2 c2, bool constantSpawn)
		{
			int num = 0;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesInArea(c1, c2, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && componentCreature.ConstantSpawn == constantSpawn)
				{
					Vector3 position = componentBody.Position;
					if (position.X >= c1.X && position.X <= c2.X && position.Z >= c1.Y && position.Z <= c2.Y)
					{
						num++;
					}
				}
			}
			Point2 point = Terrain.ToChunk(c1);
			Point2 point2 = Terrain.ToChunk(c2);
			for (int j = point.X; j <= point2.X; j++)
			{
				for (int k = point.Y; k <= point2.Y; k++)
				{
					SpawnChunk spawnChunk = this.m_subsystemSpawn.GetSpawnChunk(new Point2(j, k));
					if (spawnChunk != null)
					{
						foreach (SpawnEntityData spawnEntityData in spawnChunk.SpawnsData)
						{
							if (spawnEntityData.ConstantSpawn == constantSpawn)
							{
								Vector3 position2 = spawnEntityData.Position;
								if (position2.X >= c1.X && position2.X <= c2.X && position2.Z >= c1.Y && position2.Z <= c2.Y)
								{
									num++;
								}
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0003D9C0 File Offset: 0x0003BBC0
		public virtual int GetRandomWeightedItem(IEnumerable<float> items)
		{
			float max = MathUtils.Max(items.Sum(), 1f);
			float num = this.m_random.Float(0f, max);
			int num2 = 0;
			foreach (float num3 in items)
			{
				if (num < num3)
				{
					return num2;
				}
				num -= num3;
				num2++;
			}
			return -1;
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0003DA40 File Offset: 0x0003BC40
		public virtual SpawnLocationType GetRandomSpawnLocationType()
		{
			return SubsystemCreatureSpawn.m_spawnLocations[this.m_random.Int(0, SubsystemCreatureSpawn.m_spawnLocations.Length - 1)];
		}

		// Token: 0x040004C3 RID: 1219
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004C4 RID: 1220
		public SubsystemSpawn m_subsystemSpawn;

		// Token: 0x040004C5 RID: 1221
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004C6 RID: 1222
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004C7 RID: 1223
		public SubsystemSky m_subsystemSky;

		// Token: 0x040004C8 RID: 1224
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040004C9 RID: 1225
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x040004CA RID: 1226
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004CB RID: 1227
		public List<SubsystemCreatureSpawn.CreatureType> m_creatureTypes = new List<SubsystemCreatureSpawn.CreatureType>();

		// Token: 0x040004CC RID: 1228
		public Dictionary<ComponentCreature, bool> m_creatures = new Dictionary<ComponentCreature, bool>();

		// Token: 0x040004CD RID: 1229
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040004CE RID: 1230
		public List<SpawnChunk> m_newSpawnChunks = new List<SpawnChunk>();

		// Token: 0x040004CF RID: 1231
		public List<SpawnChunk> m_spawnChunks = new List<SpawnChunk>();

		// Token: 0x040004D0 RID: 1232
		public static SpawnLocationType[] m_spawnLocations = EnumUtils.GetEnumValues(typeof(SpawnLocationType)).Cast<SpawnLocationType>().ToArray<SpawnLocationType>();

		// Token: 0x040004D1 RID: 1233
		public const int m_totalLimit = 24;

		// Token: 0x040004D2 RID: 1234
		public const int m_areaLimit = 3;

		// Token: 0x040004D3 RID: 1235
		public const int m_areaRadius = 16;

		// Token: 0x040004D4 RID: 1236
		public const int m_totalLimitConstant = 18;

		// Token: 0x040004D5 RID: 1237
		public const int m_areaLimitConstant = 4;

		// Token: 0x040004D6 RID: 1238
		public const int m_areaRadiusConstant = 42;

		// Token: 0x02000480 RID: 1152
		public class CreatureType
		{
			// Token: 0x06002057 RID: 8279 RVA: 0x000E7A0E File Offset: 0x000E5C0E
			public CreatureType(string name, SpawnLocationType spawnLocationType, bool randomSpawn, bool constantSpawn)
			{
				this.Name = name;
				this.SpawnLocationType = spawnLocationType;
				this.RandomSpawn = randomSpawn;
				this.ConstantSpawn = constantSpawn;
			}

			// Token: 0x06002058 RID: 8280 RVA: 0x000E7A33 File Offset: 0x000E5C33
			public override string ToString()
			{
				return this.Name;
			}

			// Token: 0x0400167E RID: 5758
			public string Name;

			// Token: 0x0400167F RID: 5759
			public SpawnLocationType SpawnLocationType;

			// Token: 0x04001680 RID: 5760
			public bool RandomSpawn;

			// Token: 0x04001681 RID: 5761
			public bool ConstantSpawn;

			// Token: 0x04001682 RID: 5762
			public Func<SubsystemCreatureSpawn.CreatureType, Point3, float> SpawnSuitabilityFunction;

			// Token: 0x04001683 RID: 5763
			public Func<SubsystemCreatureSpawn.CreatureType, Point3, int> SpawnFunction;
		}
	}
}
