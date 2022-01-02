using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022E RID: 558
	public class ComponentSimpleModel : ComponentModel
	{
		// Token: 0x060011F2 RID: 4594 RVA: 0x000857E8 File Offset: 0x000839E8
		public override void Animate()
		{
			if (this.m_componentSpawn != null)
			{
				base.Opacity = new float?((this.m_componentSpawn.SpawnDuration > 0f) ? ((float)MathUtils.Saturate((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.SpawnTime) / (double)this.m_componentSpawn.SpawnDuration)) : 1f);
				if (this.m_componentSpawn.DespawnTime != null)
				{
					base.Opacity = new float?(MathUtils.Min(base.Opacity.Value, (float)MathUtils.Saturate(1.0 - (this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.DespawnTime.Value) / (double)this.m_componentSpawn.DespawnDuration)));
				}
			}
			this.SetBoneTransform(base.Model.RootBone.Index, new Matrix?(this.m_componentFrame.Matrix));
			base.Animate();
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x000858E9 File Offset: 0x00083AE9
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentSpawn = base.Entity.FindComponent<ComponentSpawn>();
			base.Load(valuesDictionary, idToEntityMap);
		}

		// Token: 0x04000AEC RID: 2796
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000AED RID: 2797
		public ComponentSpawn m_componentSpawn;
	}
}
