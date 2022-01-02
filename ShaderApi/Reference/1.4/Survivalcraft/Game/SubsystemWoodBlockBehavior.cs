using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E5 RID: 485
	public class SubsystemWoodBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000D4B RID: 3403 RVA: 0x0006061E File Offset: 0x0005E81E
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x00060626 File Offset: 0x0005E826
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0006062C File Offset: 0x0005E82C
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living || this.m_leavesToCheck.Count >= 5000 || !(BlocksManager.Blocks[Terrain.ExtractContents(value)] is WoodBlock))
			{
				return;
			}
			int num = x - 3;
			int num2 = MathUtils.Max(y - 3, 0);
			int num3 = z - 3;
			int num4 = x + 3;
			int num5 = MathUtils.Min(y + 3, 255);
			int num6 = z + 3;
			for (int i = num; i <= num4; i++)
			{
				for (int j = num3; j <= num6; j++)
				{
					TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(i, j);
					if (chunkAtCell != null)
					{
						int num7 = TerrainChunk.CalculateCellIndex(i & 15, 0, j & 15);
						for (int k = num2; k <= num5; k++)
						{
							int num8 = Terrain.ExtractContents(chunkAtCell.GetCellValueFast(num7 + k));
							if (num8 != 0 && BlocksManager.Blocks[num8] is LeavesBlock)
							{
								this.m_leavesToCheck.Add(new Point3(i, k, j));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00060740 File Offset: 0x0005E940
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			int num = chunk.Origin.X - 16;
			int num2 = chunk.Origin.Y - 16;
			int num3 = chunk.Origin.X + 32;
			int num4 = chunk.Origin.Y + 32;
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_leavesToCheck)
			{
				if (point.X >= num && point.X < num3 && point.Z >= num2 && point.Z < num4)
				{
					list.Add(point);
				}
			}
			foreach (Point3 p in list)
			{
				this.DecayLeavesIfNeeded(p);
			}
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00060844 File Offset: 0x0005EA44
		public void Update(float dt)
		{
			if (this.m_leavesToCheck.Count <= 0 || !this.m_subsystemTime.PeriodicGameTimeEvent(20.0, 0.0))
			{
				return;
			}
			int num = MathUtils.Min(MathUtils.Max((int)((float)this.m_leavesToCheck.Count * 0.1f), 10), 200);
			int num2 = 0;
			while (num2 < num && this.m_leavesToCheck.Count > 0)
			{
				this.DecayLeavesIfNeeded(this.m_leavesToCheck.First<Point3>());
				num2++;
			}
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x000608D0 File Offset: 0x0005EAD0
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			string value = valuesDictionary.GetValue<string>("LeavesToCheck");
			foreach (Point3 item in HumanReadableConverter.ValuesListFromString<Point3>(';', value))
			{
				this.m_leavesToCheck.Add(item);
			}
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00060940 File Offset: 0x0005EB40
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			string value = HumanReadableConverter.ValuesListToString<Point3>(';', this.m_leavesToCheck.ToArray<Point3>());
			valuesDictionary.SetValue<string>("LeavesToCheck", value);
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x00060974 File Offset: 0x0005EB74
		public void DecayLeavesIfNeeded(Point3 p)
		{
			this.m_leavesToCheck.Remove(p);
			if (!(BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(p.X, p.Y, p.Z)] is LeavesBlock))
			{
				return;
			}
			bool flag = false;
			int num = p.X - 3;
			int num2 = MathUtils.Max(p.Y - 3, 0);
			int num3 = p.Z - 3;
			int num4 = p.X + 3;
			int num5 = MathUtils.Min(p.Y + 3, 255);
			int num6 = p.Z + 3;
			for (int i = num; i <= num4; i++)
			{
				for (int j = num3; j <= num6; j++)
				{
					TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(i, j);
					if (chunkAtCell != null)
					{
						int num7 = TerrainChunk.CalculateCellIndex(i & 15, 0, j & 15);
						for (int k = num2; k <= num5; k++)
						{
							int num8 = Terrain.ExtractContents(chunkAtCell.GetCellValueFast(num7 + k));
							if (num8 != 0 && BlocksManager.Blocks[num8] is WoodBlock)
							{
								flag = true;
								goto IL_115;
							}
						}
					}
				}
			}
			IL_115:
			if (!flag)
			{
				base.SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, 0, true);
			}
		}

		// Token: 0x040006DA RID: 1754
		public const int m_radius = 3;

		// Token: 0x040006DB RID: 1755
		public const int m_maxLeavesToCheck = 5000;

		// Token: 0x040006DC RID: 1756
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040006DD RID: 1757
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006DE RID: 1758
		public HashSet<Point3> m_leavesToCheck = new HashSet<Point3>();
	}
}
