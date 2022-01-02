using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000193 RID: 403
	public class SubsystemCampfireBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000953 RID: 2387 RVA: 0x0003AC32 File Offset: 0x00038E32
		public Dictionary<Point3, FireParticleSystem>.KeyCollection Campfires
		{
			get
			{
				return this.m_particleSystemsByCell.Keys;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000954 RID: 2388 RVA: 0x0003AC3F File Offset: 0x00038E3F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x0003AC42 File Offset: 0x00038E42
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0003AC4C File Offset: 0x00038E4C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
			{
				this.m_updateIndex++;
				foreach (Point3 point in this.m_particleSystemsByCell.Keys)
				{
					PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(point.X, point.Z);
					if ((precipitationShaftInfo.Intensity > 0f && point.Y >= precipitationShaftInfo.YLimit - 1) || this.m_updateIndex % 5 == 0)
					{
						this.m_toReduce.Add(point);
					}
				}
				foreach (Point3 point2 in this.m_toReduce)
				{
					this.ResizeCampfire(point2.X, point2.Y, point2.Z, -1, true);
				}
				this.m_toReduce.Clear();
			}
			if (Time.PeriodicEvent(0.5, 0.0))
			{
				float num = float.MaxValue;
				foreach (Point3 point3 in this.m_particleSystemsByCell.Keys)
				{
					float x = this.m_subsystemAmbientSounds.SubsystemAudio.CalculateListenerDistanceSquared(new Vector3((float)point3.X, (float)point3.Y, (float)point3.Z));
					num = MathUtils.Min(num, x);
				}
				this.m_fireSoundVolume = this.m_subsystemAmbientSounds.SubsystemAudio.CalculateVolume(MathUtils.Sqrt(num), 2f, 2f);
			}
			this.m_subsystemAmbientSounds.FireSoundVolume = MathUtils.Max(this.m_subsystemAmbientSounds.FireSoundVolume, this.m_fireSoundVolume);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0003AE64 File Offset: 0x00039064
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0003AEAD File Offset: 0x000390AD
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0003AEBB File Offset: 0x000390BB
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveCampfireParticleSystem(x, y, z);
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0003AEC8 File Offset: 0x000390C8
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveCampfireParticleSystem(x, y, z);
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0003AEE1 File Offset: 0x000390E1
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0003AEF0 File Offset: 0x000390F0
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.ResizeCampfire(point2.X, point2.Y, point2.Z, -15, false);
				this.RemoveCampfireParticleSystem(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0003B014 File Offset: 0x00039214
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				int x = cellFace.X;
				int y = cellFace.Y;
				int z = cellFace.Z;
				int value = worldItem.Value;
				Pickable pickable = worldItem as Pickable;
				if (this.AddFuel(x, y, z, value, (pickable != null) ? pickable.Count : 1))
				{
					worldItem.ToRemove = true;
				}
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0003B062 File Offset: 0x00039262
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (this.AddFuel(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z, componentMiner.ActiveBlockValue, 1))
			{
				componentMiner.RemoveActiveTool(1);
			}
			return true;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0003B09C File Offset: 0x0003929C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0003B0F8 File Offset: 0x000392F8
		public void AddCampfireParticleSystem(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num > 0)
			{
				Vector3 v = new Vector3(0.5f, 0.15f, 0.5f);
				float size = MathUtils.Lerp(0.2f, 0.5f, (float)num / 15f);
				FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 256f);
				this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
				this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0003B180 File Offset: 0x00039380
		public void RemoveCampfireParticleSystem(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem fireParticleSystem;
			if (this.m_particleSystemsByCell.TryGetValue(key, out fireParticleSystem))
			{
				fireParticleSystem.IsStopped = true;
				this.m_particleSystemsByCell.Remove(key);
			}
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0003B1BC File Offset: 0x000393BC
		public bool AddFuel(int x, int y, int z, int value, int count)
		{
			if (Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)) > 0)
			{
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				if (base.Project.FindSubsystem<SubsystemExplosions>(true).TryExplodeBlock(x, y, z, value))
				{
					return true;
				}
				if (block is SnowBlock || block is SnowballBlock || block is IceBlock)
				{
					return this.ResizeCampfire(x, y, z, -1, true);
				}
				if (block.GetFuelHeatLevel(value) > 0f)
				{
					float num2 = (float)count * MathUtils.Min(block.GetFuelFireDuration(value), 20f) / 5f;
					int num3 = (int)num2;
					float num4 = num2 - (float)num3;
					if (this.m_random.Float(0f, 1f) < num4)
					{
						num3++;
					}
					return num3 <= 0 || this.ResizeCampfire(x, y, z, num3, true);
				}
			}
			return false;
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0003B298 File Offset: 0x00039498
		public bool ResizeCampfire(int x, int y, int z, int steps, bool playSound)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractData(cellValue);
			if (num > 0)
			{
				int num2 = MathUtils.Clamp(num + steps, 0, 15);
				if (num2 != num)
				{
					int value = Terrain.ReplaceData(cellValue, num2);
					base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
					if (playSound)
					{
						if (steps >= 0)
						{
							this.m_subsystemAmbientSounds.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, new Vector3((float)x, (float)y, (float)z), 3f, false);
						}
						else
						{
							this.m_subsystemAmbientSounds.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, 0f, new Vector3((float)x, (float)y, (float)z), 3f, true);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x040004B1 RID: 1201
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004B2 RID: 1202
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040004B3 RID: 1203
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x040004B4 RID: 1204
		public SubsystemAmbientSounds m_subsystemAmbientSounds;

		// Token: 0x040004B5 RID: 1205
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		// Token: 0x040004B6 RID: 1206
		public float m_fireSoundVolume;

		// Token: 0x040004B7 RID: 1207
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004B8 RID: 1208
		public int m_updateIndex;

		// Token: 0x040004B9 RID: 1209
		public List<Point3> m_toReduce = new List<Point3>();
	}
}
