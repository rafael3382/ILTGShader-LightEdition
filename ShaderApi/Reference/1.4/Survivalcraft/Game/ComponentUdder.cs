using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000237 RID: 567
	public class ComponentUdder : Component
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001266 RID: 4710 RVA: 0x00087FE3 File Offset: 0x000861E3
		public bool HasMilk
		{
			get
			{
				return this.m_lastMilkingTime < 0.0 || this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_lastMilkingTime >= (double)this.m_milkRegenerationTime;
			}
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x00088018 File Offset: 0x00086218
		public virtual bool Milk(ComponentMiner milker)
		{
			if (milker != null)
			{
				ComponentHerdBehavior componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>();
				if (componentHerdBehavior != null)
				{
					componentHerdBehavior.CallNearbyCreaturesHelp(milker.ComponentCreature, 20f, 20f, true);
				}
			}
			if (this.HasMilk)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				this.m_lastMilkingTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
				return true;
			}
			this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
			return false;
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x0008808C File Offset: 0x0008628C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_milkRegenerationTime = valuesDictionary.GetValue<float>("MilkRegenerationTime");
			this.m_lastMilkingTime = valuesDictionary.GetValue<double>("LastMilkingTime");
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x000880DF File Offset: 0x000862DF
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("LastMilkingTime", this.m_lastMilkingTime);
		}

		// Token: 0x04000B4B RID: 2891
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000B4C RID: 2892
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B4D RID: 2893
		public float m_milkRegenerationTime;

		// Token: 0x04000B4E RID: 2894
		public double m_lastMilkingTime;
	}
}
