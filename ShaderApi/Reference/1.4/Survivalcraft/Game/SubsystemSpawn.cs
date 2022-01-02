using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D5 RID: 469
	public class SubsystemSpawn : Subsystem, IUpdateable
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000C9C RID: 3228 RVA: 0x0005ABEA File Offset: 0x00058DEA
		public Dictionary<ComponentSpawn, bool>.KeyCollection Spawns
		{
			get
			{
				return this.m_spawns.Keys;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000C9D RID: 3229 RVA: 0x0005ABF7 File Offset: 0x00058DF7
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000C9E RID: 3230 RVA: 0x0005ABFA File Offset: 0x00058DFA
		// (set) Token: 0x06000C9F RID: 3231 RVA: 0x0005AC02 File Offset: 0x00058E02
		public virtual Action<SpawnChunk> SpawningChunk { get; set; }

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0005AC0C File Offset: 0x00058E0C
		public virtual SpawnChunk GetSpawnChunk(Point2 point)
		{
			SpawnChunk result;
			this.m_chunks.TryGetValue(point, out result);
			return result;
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0005AC2C File Offset: 0x00058E2C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextDiscardOldChunksTime)
			{
				this.m_nextDiscardOldChunksTime = this.m_subsystemTime.GameTime + 60.0;
				this.DiscardOldChunks();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextVisitedTime)
			{
				this.m_nextVisitedTime = this.m_subsystemTime.GameTime + 5.0;
				this.UpdateLastVisitedTime();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextChunkSpawnTime)
			{
				this.m_nextChunkSpawnTime = this.m_subsystemTime.GameTime + 4.0;
				this.SpawnChunks();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextDespawnTime)
			{
				this.m_nextDespawnTime = this.m_subsystemTime.GameTime + 2.0;
				this.DespawnChunks();
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0005AD0C File Offset: 0x00058F0C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Chunks"))
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)keyValuePair.Value;
				SpawnChunk spawnChunk = new SpawnChunk();
				spawnChunk.Point = HumanReadableConverter.ConvertFromString<Point2>(keyValuePair.Key);
				spawnChunk.IsSpawned = valuesDictionary2.GetValue<bool>("IsSpawned");
				spawnChunk.LastVisitedTime = new double?(valuesDictionary2.GetValue<double>("LastVisitedTime"));
				string value = valuesDictionary2.GetValue<string>("SpawnsData", string.Empty);
				if (!string.IsNullOrEmpty(value))
				{
					this.LoadSpawnsData(value, spawnChunk.SpawnsData);
				}
				this.m_chunks[spawnChunk.Point] = spawnChunk;
			}
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x0005AE40 File Offset: 0x00059040
		public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Chunks", valuesDictionary2);
			foreach (SpawnChunk spawnChunk in this.m_chunks.Values)
			{
				if (spawnChunk.LastVisitedTime != null)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>(HumanReadableConverter.ConvertToString(spawnChunk.Point), valuesDictionary3);
					valuesDictionary3.SetValue<bool>("IsSpawned", spawnChunk.IsSpawned);
					valuesDictionary3.SetValue<double>("LastVisitedTime", spawnChunk.LastVisitedTime.Value);
					string value = this.SaveSpawnsData(spawnChunk.SpawnsData);
					if (!string.IsNullOrEmpty(value))
					{
						valuesDictionary3.SetValue<string>("SpawnsData", value);
					}
				}
			}
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x0005AF1C File Offset: 0x0005911C
		public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentSpawn key in entity.FindComponents<ComponentSpawn>())
			{
				this.m_spawns.Add(key, true);
			}
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x0005AF78 File Offset: 0x00059178
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentSpawn key in entity.FindComponents<ComponentSpawn>())
			{
				this.m_spawns.Remove(key);
			}
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0005AFD4 File Offset: 0x000591D4
		public virtual SpawnChunk GetOrCreateSpawnChunk(Point2 point)
		{
			SpawnChunk spawnChunk = this.GetSpawnChunk(point);
			if (spawnChunk == null)
			{
				spawnChunk = new SpawnChunk
				{
					Point = point
				};
				this.m_chunks.Add(point, spawnChunk);
			}
			return spawnChunk;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0005B008 File Offset: 0x00059208
		public virtual void DiscardOldChunks()
		{
			List<Point2> list = new List<Point2>();
			foreach (SpawnChunk spawnChunk in this.m_chunks.Values)
			{
				if (spawnChunk.LastVisitedTime == null || this.m_subsystemGameInfo.TotalElapsedGameTime - spawnChunk.LastVisitedTime.Value > 76800.0)
				{
					list.Add(spawnChunk.Point);
				}
			}
			foreach (Point2 key in list)
			{
				this.m_chunks.Remove(key);
			}
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x0005B0E0 File Offset: 0x000592E0
		public virtual void UpdateLastVisitedTime()
		{
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				Vector2 v = new Vector2(componentPlayer.ComponentBody.Position.X, componentPlayer.ComponentBody.Position.Z);
				Vector2 p = v - new Vector2(8f);
				Vector2 p2 = v + new Vector2(8f);
				Point2 point = Terrain.ToChunk(p);
				Point2 point2 = Terrain.ToChunk(p2);
				for (int i = point.X; i <= point2.X; i++)
				{
					for (int j = point.Y; j <= point2.Y; j++)
					{
						SpawnChunk spawnChunk = this.GetSpawnChunk(new Point2(i, j));
						if (spawnChunk != null)
						{
							spawnChunk.LastVisitedTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
						}
					}
				}
			}
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0005B1F8 File Offset: 0x000593F8
		public virtual void SpawnChunks()
		{
			List<SpawnChunk> list = new List<SpawnChunk>();
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				Vector2 v = new Vector2(gameWidget.ActiveCamera.ViewPosition.X, gameWidget.ActiveCamera.ViewPosition.Z);
				Vector2 p = v - new Vector2(40f);
				Vector2 p2 = v + new Vector2(40f);
				Point2 point = Terrain.ToChunk(p);
				Point2 point2 = Terrain.ToChunk(p2);
				for (int i = point.X; i <= point2.X; i++)
				{
					for (int j = point.Y; j <= point2.Y; j++)
					{
						Vector2 vector = new Vector2(((float)i + 0.5f) * 16f, ((float)j + 0.5f) * 16f);
						if (Vector2.DistanceSquared(v, vector) < 1600f)
						{
							TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Y));
							if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidPropagatedLight)
							{
								Point2 point3 = new Point2(i, j);
								SpawnChunk orCreateSpawnChunk = this.GetOrCreateSpawnChunk(point3);
								foreach (SpawnEntityData data in orCreateSpawnChunk.SpawnsData)
								{
									this.SpawnEntity(data);
								}
								orCreateSpawnChunk.SpawnsData.Clear();
								Action<SpawnChunk> spawningChunk = this.SpawningChunk;
								if (spawningChunk != null)
								{
									spawningChunk(orCreateSpawnChunk);
								}
								orCreateSpawnChunk.IsSpawned = true;
							}
						}
					}
				}
			}
			foreach (SpawnChunk spawnChunk in list)
			{
				foreach (SpawnEntityData data2 in spawnChunk.SpawnsData)
				{
					this.SpawnEntity(data2);
				}
				spawnChunk.SpawnsData.Clear();
			}
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x0005B4AC File Offset: 0x000596AC
		public virtual void DespawnChunks()
		{
			List<ComponentSpawn> list = new List<ComponentSpawn>(0);
			foreach (ComponentSpawn componentSpawn in this.m_spawns.Keys)
			{
				if (componentSpawn.AutoDespawn && !componentSpawn.IsDespawning)
				{
					bool flag = true;
					Vector3 position = componentSpawn.ComponentFrame.Position;
					Vector2 v = new Vector2(position.X, position.Z);
					foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
					{
						Vector3 viewPosition = gameWidget.ActiveCamera.ViewPosition;
						Vector2 v2 = new Vector2(viewPosition.X, viewPosition.Z);
						if (Vector2.DistanceSquared(v, v2) <= 2704f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list.Add(componentSpawn);
					}
				}
			}
			foreach (ComponentSpawn componentSpawn2 in list)
			{
				Point2 point = Terrain.ToChunk(componentSpawn2.ComponentFrame.Position.XZ);
				List<SpawnEntityData> spawnsData = this.GetOrCreateSpawnChunk(point).SpawnsData;
				SpawnEntityData spawnEntityData = new SpawnEntityData();
				spawnEntityData.TemplateName = componentSpawn2.Entity.ValuesDictionary.DatabaseObject.Name;
				spawnEntityData.Position = componentSpawn2.ComponentFrame.Position;
				ComponentCreature componentCreature = componentSpawn2.ComponentCreature;
				spawnEntityData.ConstantSpawn = (componentCreature != null && componentCreature.ConstantSpawn);
				spawnsData.Add(spawnEntityData);
				componentSpawn2.Despawn();
			}
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x0005B688 File Offset: 0x00059888
		public virtual Entity SpawnEntity(SpawnEntityData data)
		{
			Entity result;
			try
			{
				Entity entity = DatabaseManager.CreateEntity(base.Project, data.TemplateName, true);
				bool flag = false;
				ModsManager.HookAction("SpawnEntity", delegate(ModLoader modLoader)
				{
					bool result2;
					modLoader.SpawnEntity(this, entity, data, out result2);
					return result2;
				});
				if (!flag)
				{
					entity.FindComponent<ComponentBody>(true).Position = data.Position;
					entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.28318548f));
					ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
					if (componentCreature != null)
					{
						componentCreature.ConstantSpawn = data.ConstantSpawn;
					}
				}
				base.Project.AddEntity(entity);
				result = entity;
			}
			catch (Exception ex)
			{
				Log.Error("Unable to spawn entity with template \"" + data.TemplateName + "\". Reason: " + ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x0005B7A0 File Offset: 0x000599A0
		public virtual void LoadSpawnsData(string data, List<SpawnEntityData> creaturesData)
		{
			bool flag = false;
			ModsManager.HookAction("LoadSpawnsData", delegate(ModLoader modLoader)
			{
				modLoader.LoadSpawnsData(this, data, creaturesData, out flag);
				return flag;
			});
			if (!flag)
			{
				string[] array = data.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length < 4)
					{
						throw new InvalidOperationException("Invalid spawn data string.");
					}
					SpawnEntityData spawnEntityData = new SpawnEntityData
					{
						TemplateName = array2[0],
						Position = new Vector3
						{
							X = float.Parse(array2[1], CultureInfo.InvariantCulture),
							Y = float.Parse(array2[2], CultureInfo.InvariantCulture),
							Z = float.Parse(array2[3], CultureInfo.InvariantCulture)
						}
					};
					if (array2.Length == 5)
					{
						spawnEntityData.ConstantSpawn = bool.Parse(array2[4]);
					}
					creaturesData.Add(spawnEntityData);
				}
				return;
			}
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x0005B8C4 File Offset: 0x00059AC4
		public virtual string SaveSpawnsData(List<SpawnEntityData> spawnsData)
		{
			bool flag = false;
			string data = string.Empty;
			ModsManager.HookAction("SaveSpawnsData", delegate(ModLoader modLoader)
			{
				data = modLoader.SaveSpawnsData(this, spawnsData, out flag);
				return flag;
			});
			if (!flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SpawnEntityData spawnEntityData in spawnsData)
				{
					stringBuilder.Append(spawnEntityData.TemplateName);
					stringBuilder.Append(',');
					stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.X * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(',');
					stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.Y * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(',');
					stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.Z * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(',');
					stringBuilder.Append(spawnEntityData.ConstantSpawn.ToString());
					if (!string.IsNullOrEmpty(spawnEntityData.ExtraData))
					{
						stringBuilder.Append(',');
						stringBuilder.Append(spawnEntityData.ExtraData);
					}
					stringBuilder.Append(';');
				}
				data = stringBuilder.ToString();
			}
			return data;
		}

		// Token: 0x04000672 RID: 1650
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000673 RID: 1651
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x04000674 RID: 1652
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x04000675 RID: 1653
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000676 RID: 1654
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000677 RID: 1655
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000678 RID: 1656
		public double m_nextDiscardOldChunksTime = 1.0;

		// Token: 0x04000679 RID: 1657
		public double m_nextVisitedTime = 1.0;

		// Token: 0x0400067A RID: 1658
		public double m_nextChunkSpawnTime = 1.0;

		// Token: 0x0400067B RID: 1659
		public double m_nextDespawnTime = 1.0;

		// Token: 0x0400067C RID: 1660
		public Dictionary<Point2, SpawnChunk> m_chunks = new Dictionary<Point2, SpawnChunk>();

		// Token: 0x0400067D RID: 1661
		public Dictionary<ComponentSpawn, bool> m_spawns = new Dictionary<ComponentSpawn, bool>();

		// Token: 0x0400067E RID: 1662
		public const float MaxChunkAge = 76800f;

		// Token: 0x0400067F RID: 1663
		public const float VisitedRadius = 8f;

		// Token: 0x04000680 RID: 1664
		public const float SpawnRadius = 40f;

		// Token: 0x04000681 RID: 1665
		public const float DespawnRadius = 52f;
	}
}
