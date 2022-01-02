using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000233 RID: 563
	public class ComponentStubbornSteedBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x00087027 File Offset: 0x00085227
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0008702A File Offset: 0x0008522A
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x00087034 File Offset: 0x00085234
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)this.m_periodicEventOffset))
			{
				this.m_importanceLevel = ((this.m_subsystemGameInfo.TotalElapsedGameTime < this.m_stubbornEndTime && this.m_componentEatPickableBehavior.Satiation <= 0f && this.m_componentMount.Rider != null) ? 210f : 0f);
			}
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x000870C8 File Offset: 0x000852C8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_componentSteedBehavior = base.Entity.FindComponent<ComponentSteedBehavior>(true);
			this.m_componentEatPickableBehavior = base.Entity.FindComponent<ComponentEatPickableBehavior>(true);
			this.m_stubbornProbability = valuesDictionary.GetValue<float>("StubbornProbability");
			this.m_stubbornEndTime = valuesDictionary.GetValue<double>("StubbornEndTime");
			this.m_periodicEventOffset = this.m_random.Float(0f, 100f);
			this.m_isSaddled = base.Entity.ValuesDictionary.DatabaseObject.Name.EndsWith("_Saddled");
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)this.m_periodicEventOffset) && this.m_componentMount.Rider != null && this.m_random.Float(0f, 1f) < this.m_stubbornProbability && (!this.m_isSaddled || this.m_componentEatPickableBehavior.Satiation <= 0f))
				{
					this.m_stubbornEndTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)this.m_random.Float(60f, 120f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Stubborn");
				}
			}, null);
			this.m_stateMachine.AddState("Stubborn", null, delegate
			{
				if (this.m_componentSteedBehavior.WasOrderIssued)
				{
					this.m_componentCreature.ComponentCreatureModel.HeadShakeOrder = this.m_random.Float(0.6f, 1f);
					this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x000871EF File Offset: 0x000853EF
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("StubbornEndTime", this.m_stubbornEndTime);
		}

		// Token: 0x04000B24 RID: 2852
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B25 RID: 2853
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000B26 RID: 2854
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B27 RID: 2855
		public ComponentMount m_componentMount;

		// Token: 0x04000B28 RID: 2856
		public ComponentSteedBehavior m_componentSteedBehavior;

		// Token: 0x04000B29 RID: 2857
		public ComponentEatPickableBehavior m_componentEatPickableBehavior;

		// Token: 0x04000B2A RID: 2858
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B2B RID: 2859
		public float m_importanceLevel;

		// Token: 0x04000B2C RID: 2860
		public bool m_isSaddled;

		// Token: 0x04000B2D RID: 2861
		public Random m_random = new Random();

		// Token: 0x04000B2E RID: 2862
		public float m_periodicEventOffset;

		// Token: 0x04000B2F RID: 2863
		public float m_stubbornProbability;

		// Token: 0x04000B30 RID: 2864
		public double m_stubbornEndTime;
	}
}
