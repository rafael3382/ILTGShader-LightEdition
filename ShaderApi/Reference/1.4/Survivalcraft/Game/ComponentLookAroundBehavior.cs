using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021B RID: 539
	public class ComponentLookAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060010A7 RID: 4263 RVA: 0x0007CD2B File Offset: 0x0007AF2B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060010A8 RID: 4264 RVA: 0x0007CD2E File Offset: 0x0007AF2E
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x0007CD36 File Offset: 0x0007AF36
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0007CD44 File Offset: 0x0007AF44
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = this.m_random.Float(0f, 1f);
			}, delegate
			{
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_random.Float(0f, 1f) < 0.05f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_importanceLevel = this.m_random.Float(1f, 5f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("LookAround");
				}
			}, null);
			this.m_stateMachine.AddState("LookAround", delegate
			{
				this.m_lookAroundTime = this.m_random.Float(8f, 15f);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_lookAroundTime <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
				this.m_lookAroundTime -= this.m_subsystemTime.GameTimeDelta;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x040009EF RID: 2543
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009F0 RID: 2544
		public ComponentCreature m_componentCreature;

		// Token: 0x040009F1 RID: 2545
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040009F2 RID: 2546
		public Random m_random = new Random();

		// Token: 0x040009F3 RID: 2547
		public float m_lookAroundTime;

		// Token: 0x040009F4 RID: 2548
		public float m_importanceLevel;
	}
}
