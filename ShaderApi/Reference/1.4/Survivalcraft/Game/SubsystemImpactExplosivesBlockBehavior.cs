using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B1 RID: 433
	public class SubsystemImpactExplosivesBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000B23 RID: 2851 RVA: 0x0004AEFA File Offset: 0x000490FA
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x0004AF04 File Offset: 0x00049104
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			return this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(worldItem.Position.X), Terrain.ToCell(worldItem.Position.Y), Terrain.ToCell(worldItem.Position.Z), worldItem.Value);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0004AF52 File Offset: 0x00049152
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
		}

		// Token: 0x0400056B RID: 1387
		public SubsystemExplosions m_subsystemExplosions;
	}
}
