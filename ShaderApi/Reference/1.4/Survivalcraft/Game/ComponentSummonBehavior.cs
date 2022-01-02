using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000234 RID: 564
	public class ComponentSummonBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x0008731F File Offset: 0x0008551F
		// (set) Token: 0x06001242 RID: 4674 RVA: 0x00087327 File Offset: 0x00085527
		public ComponentBody SummonTarget { get; set; }

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001243 RID: 4675 RVA: 0x00087330 File Offset: 0x00085530
		public bool IsEnabled
		{
			get
			{
				return this.m_isEnabled;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001244 RID: 4676 RVA: 0x00087338 File Offset: 0x00085538
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001245 RID: 4677 RVA: 0x0008733B File Offset: 0x0008553B
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x00087343 File Offset: 0x00085543
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00087350 File Offset: 0x00085550
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_isEnabled = valuesDictionary.GetValue<bool>("IsEnabled");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.SummonTarget = null;
				this.m_summonedTime = 0.0;
			}, delegate
			{
				if (this.m_isEnabled && this.SummonTarget != null && this.m_summonedTime == 0.0)
				{
					this.m_subsystemTime.QueueGameTimeDelayedExecution(this.m_subsystemTime.GameTime + 0.5, delegate
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
						this.m_importanceLevel = 270f;
						this.m_summonedTime = this.m_subsystemTime.GameTime;
					});
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("FollowTarget");
				}
			}, null);
			this.m_stateMachine.AddState("FollowTarget", delegate
			{
				this.FollowTarget(true);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.SummonTarget == null || this.m_componentPathfinding.IsStuck || this.m_subsystemTime.GameTime - this.m_summonedTime > 30.0)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_componentPathfinding.Destination == null)
				{
					if (this.m_stoppedTime < 0.0)
					{
						this.m_stoppedTime = this.m_subsystemTime.GameTime;
					}
					if (this.m_subsystemTime.GameTime - this.m_stoppedTime > 6.0)
					{
						this.m_importanceLevel = 0f;
					}
				}
				this.FollowTarget(false);
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00087408 File Offset: 0x00085608
		public virtual void FollowTarget(bool noDelay)
		{
			if (this.SummonTarget != null && (noDelay || this.m_random.Float(0f, 1f) < 5f * this.m_subsystemTime.GameTimeDelta))
			{
				float num = Vector3.Distance(this.m_componentCreature.ComponentBody.Position, this.SummonTarget.Position);
				if (num > 4f)
				{
					Vector3 vector = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, this.SummonTarget.Position - this.m_componentCreature.ComponentBody.Position));
					vector *= 0.75f * (float)((this.GetHashCode() % 2 != 0) ? 1 : -1) * (float)(1 + this.GetHashCode() % 3);
					float speed = MathUtils.Lerp(0.4f, 1f, MathUtils.Saturate(0.25f * (num - 5f)));
					this.m_componentPathfinding.SetDestination(new Vector3?(this.SummonTarget.Position + vector), speed, 3.75f, 2000, true, false, true, null);
					this.m_stoppedTime = -1.0;
				}
			}
		}

		// Token: 0x04000B31 RID: 2865
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B32 RID: 2866
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B33 RID: 2867
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B34 RID: 2868
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B35 RID: 2869
		public float m_importanceLevel;

		// Token: 0x04000B36 RID: 2870
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B37 RID: 2871
		public bool m_isEnabled;

		// Token: 0x04000B38 RID: 2872
		public double m_summonedTime;

		// Token: 0x04000B39 RID: 2873
		public double m_stoppedTime;
	}
}
