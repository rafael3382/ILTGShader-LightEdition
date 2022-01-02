using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000189 RID: 393
	public class SubsystemBlocksScanner : Subsystem, IUpdateable
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x00038B60 File Offset: 0x00036D60
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.BlocksScanner;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x00038B64 File Offset: 0x00036D64
		// (set) Token: 0x0600090F RID: 2319 RVA: 0x00038B6C File Offset: 0x00036D6C
		public virtual Action<TerrainChunk> ScanningChunkCompleted { get; set; }

		// Token: 0x06000910 RID: 2320 RVA: 0x00038B78 File Offset: 0x00036D78
		public void Update(float dt)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			this.m_pollCount += (float)(terrain.AllocatedChunks.Length * 16 * 16) * dt / 60f;
			this.m_pollCount = MathUtils.Clamp(this.m_pollCount, 0f, 200f);
			TerrainChunk nextChunk = terrain.GetNextChunk(this.m_pollChunkCoordinates.X, this.m_pollChunkCoordinates.Y);
			if (nextChunk == null)
			{
				return;
			}
			while (this.m_pollCount >= 1f)
			{
				if (nextChunk.State <= TerrainChunkState.InvalidContents4)
				{
					this.m_pollCount -= 65536f;
				}
				else
				{
					while (this.m_pollX < 16)
					{
						while (this.m_pollZ < 16)
						{
							if (this.m_pollCount < 1f)
							{
								return;
							}
							this.m_pollCount -= 1f;
							int topHeightFast = nextChunk.GetTopHeightFast(this.m_pollX, this.m_pollZ);
							int num = TerrainChunk.CalculateCellIndex(this.m_pollX, 0, this.m_pollZ);
							int i = 0;
							while (i <= topHeightFast)
							{
								int cellValueFast = nextChunk.GetCellValueFast(num);
								int num2 = Terrain.ExtractContents(cellValueFast);
								if (num2 != 0)
								{
									SubsystemPollableBlockBehavior[] array = this.m_pollableBehaviorsByContents[num2];
									for (int j = 0; j < array.Length; j++)
									{
										array[j].OnPoll(cellValueFast, nextChunk.Origin.X + this.m_pollX, i, nextChunk.Origin.Y + this.m_pollZ, this.m_pollPass);
									}
								}
								i++;
								num++;
							}
							this.m_pollZ++;
						}
						this.m_pollZ = 0;
						this.m_pollX++;
					}
					this.m_pollX = 0;
				}
				Action<TerrainChunk> scanningChunkCompleted = this.ScanningChunkCompleted;
				if (scanningChunkCompleted != null)
				{
					scanningChunkCompleted(nextChunk);
				}
				nextChunk = terrain.GetNextChunk(nextChunk.Coords.X + 1, nextChunk.Coords.Y);
				if (nextChunk == null)
				{
					break;
				}
				if (Terrain.ComparePoints(nextChunk.Coords, this.m_pollChunkCoordinates) < 0)
				{
					this.m_pollPass++;
				}
				this.m_pollChunkCoordinates = nextChunk.Coords;
			}
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x00038D90 File Offset: 0x00036F90
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_pollChunkCoordinates = valuesDictionary.GetValue<Point2>("PollChunkCoordinates");
			Point2 value = valuesDictionary.GetValue<Point2>("PollPoint");
			this.m_pollX = value.X;
			this.m_pollZ = value.Y;
			this.m_pollPass = valuesDictionary.GetValue<int>("PollPass");
			this.m_pollableBehaviorsByContents = new SubsystemPollableBlockBehavior[BlocksManager.Blocks.Length][];
			for (int i = 0; i < this.m_pollableBehaviorsByContents.Length; i++)
			{
				this.m_pollableBehaviorsByContents[i] = (from s in this.m_subsystemBlockBehaviors.GetBlockBehaviors(i)
				where s is SubsystemPollableBlockBehavior
				select (SubsystemPollableBlockBehavior)s).ToArray<SubsystemPollableBlockBehavior>();
			}
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00038E8D File Offset: 0x0003708D
		public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<Point2>("PollChunkCoordinates", this.m_pollChunkCoordinates);
			valuesDictionary.SetValue<Point2>("PollPoint", new Point2(this.m_pollX, this.m_pollZ));
			valuesDictionary.SetValue<int>("PollPass", this.m_pollPass);
		}

		// Token: 0x04000486 RID: 1158
		public const float ScanPeriod = 60f;

		// Token: 0x04000487 RID: 1159
		public SubsystemPollableBlockBehavior[][] m_pollableBehaviorsByContents;

		// Token: 0x04000488 RID: 1160
		public Point2 m_pollChunkCoordinates;

		// Token: 0x04000489 RID: 1161
		public int m_pollX;

		// Token: 0x0400048A RID: 1162
		public int m_pollZ;

		// Token: 0x0400048B RID: 1163
		public int m_pollPass;

		// Token: 0x0400048C RID: 1164
		public float m_pollCount;

		// Token: 0x0400048D RID: 1165
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400048E RID: 1166
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;
	}
}
