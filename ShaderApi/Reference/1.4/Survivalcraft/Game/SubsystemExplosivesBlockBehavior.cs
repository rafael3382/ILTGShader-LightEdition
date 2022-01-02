using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Audio;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A2 RID: 418
	public class SubsystemExplosivesBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0004529B File Offset: 0x0004349B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000A7B RID: 2683 RVA: 0x0004529E File Offset: 0x0004349E
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x000452A8 File Offset: 0x000434A8
		public bool IgniteFuse(int x, int y, int z)
		{
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(x, y, z);
			if (BlocksManager.Blocks[cellContents] is GunpowderKegBlock)
			{
				this.AddExplosive(new Point3(x, y, z), this.m_random.Float(4f, 5f));
				return true;
			}
			if (BlocksManager.Blocks[cellContents] is DetonatorBlock)
			{
				this.AddExplosive(new Point3(x, y, z), this.m_random.Float(0.8f, 1.2f));
				return true;
			}
			return false;
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x00045330 File Offset: 0x00043530
		public void Update(float dt)
		{
			float num = float.MaxValue;
			if (this.m_explosiveDataByPoint.Count > 0)
			{
				foreach (SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData in this.m_explosiveDataByPoint.Values.ToArray<SubsystemExplosivesBlockBehavior.ExplosiveData>())
				{
					Point3 point = explosiveData.Point;
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					int num2 = Terrain.ExtractContents(cellValue);
					Block block = BlocksManager.Blocks[num2];
					if (explosiveData.FuseParticleSystem == null)
					{
						GunpowderKegBlock gunpowderKegBlock = block as GunpowderKegBlock;
						if (gunpowderKegBlock != null)
						{
							explosiveData.FuseParticleSystem = new FuseParticleSystem(new Vector3((float)point.X, (float)point.Y, (float)point.Z) + gunpowderKegBlock.FuseOffset);
							this.m_subsystemParticles.AddParticleSystem(explosiveData.FuseParticleSystem);
						}
					}
					explosiveData.TimeToExplosion -= dt;
					if (explosiveData.TimeToExplosion <= 0f)
					{
						this.m_subsystemExplosions.TryExplodeBlock(explosiveData.Point.X, explosiveData.Point.Y, explosiveData.Point.Z, cellValue);
					}
					float x = this.m_subsystemAudio.CalculateListenerDistance(new Vector3((float)point.X, (float)point.Y, (float)point.Z) + new Vector3(0.5f));
					num = MathUtils.Min(num, x);
				}
			}
			if (this.m_fuseSound != null)
			{
				this.m_fuseSound.Volume = SettingsManager.SoundsVolume * this.m_subsystemAudio.CalculateVolume(num, 2f, 2f);
				if (this.m_fuseSound.Volume > AudioManager.MinAudibleVolume)
				{
					this.m_fuseSound.Play();
					return;
				}
				this.m_fuseSound.Pause();
			}
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x000454FD File Offset: 0x000436FD
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (this.m_subsystemFireBlockBehavior.IsCellOnFire(x, y, z))
			{
				this.IgniteFuse(x, y, z);
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0004551C File Offset: 0x0004371C
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			Point3 point = new Point3(x, y, z);
			this.RemoveExplosive(point);
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x0004553C File Offset: 0x0004373C
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_explosiveDataByPoint.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveExplosive(point2);
			}
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x0004562C File Offset: 0x0004382C
		public override void OnExplosion(int value, int x, int y, int z, float damage)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.GetExplosionPressure(value) > 0f && MathUtils.Saturate(damage / block.ExplosionResilience) > 0.01f && this.m_random.Float(0f, 1f) < 0.5f)
			{
				this.IgniteFuse(x, y, z);
			}
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00045694 File Offset: 0x00043894
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_fuseSound = this.m_subsystemAudio.CreateSound("Audio/Fuse");
			this.m_fuseSound.IsLooped = true;
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Explosives").Values)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Point3 value = valuesDictionary2.GetValue<Point3>("Point");
				float value2 = valuesDictionary2.GetValue<float>("TimeToExplosion");
				this.AddExplosive(value, value2);
			}
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0004578C File Offset: 0x0004398C
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			int num = 0;
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Explosives", valuesDictionary2);
			foreach (SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData in this.m_explosiveDataByPoint.Values)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num++.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				valuesDictionary3.SetValue<Point3>("Point", explosiveData.Point);
				valuesDictionary3.SetValue<float>("TimeToExplosion", explosiveData.TimeToExplosion);
			}
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x00045840 File Offset: 0x00043A40
		public override void Dispose()
		{
			Utilities.Dispose<Sound>(ref this.m_fuseSound);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x00045850 File Offset: 0x00043A50
		public void AddExplosive(Point3 point, float timeToExplosion)
		{
			if (!this.m_explosiveDataByPoint.ContainsKey(point))
			{
				SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData = new SubsystemExplosivesBlockBehavior.ExplosiveData();
				explosiveData.Point = point;
				explosiveData.TimeToExplosion = timeToExplosion;
				this.m_explosiveDataByPoint.Add(point, explosiveData);
			}
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0004588C File Offset: 0x00043A8C
		public void RemoveExplosive(Point3 point)
		{
			SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData;
			if (this.m_explosiveDataByPoint.TryGetValue(point, out explosiveData))
			{
				this.m_explosiveDataByPoint.Remove(point);
				if (explosiveData.FuseParticleSystem != null)
				{
					this.m_subsystemParticles.RemoveParticleSystem(explosiveData.FuseParticleSystem);
				}
			}
		}

		// Token: 0x04000519 RID: 1305
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400051A RID: 1306
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400051B RID: 1307
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x0400051C RID: 1308
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x0400051D RID: 1309
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400051E RID: 1310
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400051F RID: 1311
		public Dictionary<Point3, SubsystemExplosivesBlockBehavior.ExplosiveData> m_explosiveDataByPoint = new Dictionary<Point3, SubsystemExplosivesBlockBehavior.ExplosiveData>();

		// Token: 0x04000520 RID: 1312
		public Sound m_fuseSound;

		// Token: 0x02000489 RID: 1161
		public class ExplosiveData
		{
			// Token: 0x040016A2 RID: 5794
			public Point3 Point;

			// Token: 0x040016A3 RID: 5795
			public float TimeToExplosion;

			// Token: 0x040016A4 RID: 5796
			public FuseParticleSystem FuseParticleSystem;
		}
	}
}
