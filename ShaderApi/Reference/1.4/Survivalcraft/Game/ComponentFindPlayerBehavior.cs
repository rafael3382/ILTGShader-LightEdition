using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000201 RID: 513
	public class ComponentFindPlayerBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0006D6E5 File Offset: 0x0006B8E5
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0006D6E8 File Offset: 0x0006B8E8
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0006D6F0 File Offset: 0x0006B8F0
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_dt = this.m_random.Float(1.25f, 1.75f) + MathUtils.Min((float)(this.m_subsystemTime.GameTime - this.m_nextUpdateTime), 0.1f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_dt;
				this.m_stateMachine.Update();
			}
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0006D770 File Offset: 0x0006B970
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_dayRange = valuesDictionary.GetValue<float>("DayRange");
			this.m_nightRange = valuesDictionary.GetValue<float>("NightRange");
			this.m_minRange = valuesDictionary.GetValue<float>("MinRange");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_target = null;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
				if (this.m_subsystemGameInfo.WorldSettings.GameMode > GameMode.Harmless)
				{
					this.m_target = this.FindTarget();
					if (this.m_target != null)
					{
						ComponentPlayer componentPlayer = this.m_target.Entity.FindComponent<ComponentPlayer>();
						if (componentPlayer != null && componentPlayer.ComponentSleep.IsSleeping)
						{
							this.m_importanceLevel = 5f;
							return;
						}
						if (this.m_random.Float(0f, 1f) < 0.05f * this.m_dt)
						{
							this.m_importanceLevel = this.m_random.Float(1f, 4f);
							return;
						}
					}
					else
					{
						this.m_importanceLevel = 0f;
					}
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_target != null)
				{
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_target.ComponentBody.Position), this.m_random.Float(0.5f, 0.7f), this.m_minRange, 500, true, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null || this.ScoreTarget(this.m_target) <= 0f)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0006D880 File Offset: 0x0006BA80
		public virtual ComponentCreature FindTarget()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			ComponentCreature result = null;
			float num = 0f;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), MathUtils.Max(this.m_nightRange, this.m_dayRange), this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentCreature componentCreature = this.m_componentBodies.Array[i].Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					float num2 = this.ScoreTarget(componentCreature);
					if (num2 > num)
					{
						num = num2;
						result = componentCreature;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0006D930 File Offset: 0x0006BB30
		public virtual float ScoreTarget(ComponentCreature target)
		{
			float num = (this.m_subsystemSky.SkyLightIntensity > 0.2f) ? this.m_dayRange : this.m_nightRange;
			if (!target.IsAddedToProject || target.ComponentHealth.Health <= 0f || target.Entity.FindComponent<ComponentPlayer>() == null)
			{
				return 0f;
			}
			float num2 = Vector3.DistanceSquared(target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
			if (num2 < this.m_minRange * this.m_minRange)
			{
				return 0f;
			}
			return num * num - num2;
		}

		// Token: 0x04000845 RID: 2117
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000846 RID: 2118
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000847 RID: 2119
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000848 RID: 2120
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000849 RID: 2121
		public ComponentCreature m_componentCreature;

		// Token: 0x0400084A RID: 2122
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400084B RID: 2123
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400084C RID: 2124
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x0400084D RID: 2125
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400084E RID: 2126
		public float m_importanceLevel;

		// Token: 0x0400084F RID: 2127
		public float m_dayRange;

		// Token: 0x04000850 RID: 2128
		public float m_nightRange;

		// Token: 0x04000851 RID: 2129
		public float m_minRange;

		// Token: 0x04000852 RID: 2130
		public float m_dt;

		// Token: 0x04000853 RID: 2131
		public ComponentCreature m_target;

		// Token: 0x04000854 RID: 2132
		public double m_nextUpdateTime;
	}
}
