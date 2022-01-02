using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000192 RID: 402
	public class SubsystemCactusBlockBehavior : SubsystemPollableBlockBehavior, IUpdateable
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x0003A9C2 File Offset: 0x00038BC2
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					127
				};
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x0003A9CF File Offset: 0x00038BCF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0003A9D4 File Offset: 0x00038BD4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (cellContents != 7 && cellContents != 127)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0003AA14 File Offset: 0x00038C14
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (Terrain.ExtractContents(cellValue) == 0 && Terrain.ExtractLight(cellValue) >= 12)
			{
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 2, z);
				if ((cellContents != 127 || cellContents2 != 127) && this.m_random.Float(0f, 1f) < 0.25f)
				{
					this.m_toUpdate[new Point3(x, y + 1, z)] = Terrain.MakeBlockValue(127, 0, 0);
				}
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0003AACE File Offset: 0x00038CCE
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
			if (componentCreature == null)
			{
				return;
			}
			componentCreature.ComponentHealth.Injure(0.01f * MathUtils.Abs(velocity), null, false, "Spiked by cactus");
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0003AAFD File Offset: 0x00038CFD
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0003AB2C File Offset: 0x00038D2C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(60.0, 0.0))
			{
				foreach (KeyValuePair<Point3, int> keyValuePair in this.m_toUpdate)
				{
					if (base.SubsystemTerrain.Terrain.GetCellContents(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z) == 0)
					{
						base.SubsystemTerrain.ChangeCell(keyValuePair.Key.X, keyValuePair.Key.Y, keyValuePair.Key.Z, keyValuePair.Value, true);
					}
				}
				this.m_toUpdate.Clear();
			}
		}

		// Token: 0x040004AD RID: 1197
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004AE RID: 1198
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004AF RID: 1199
		public Dictionary<Point3, int> m_toUpdate = new Dictionary<Point3, int>();

		// Token: 0x040004B0 RID: 1200
		public Game.Random m_random = new Game.Random();
	}
}
