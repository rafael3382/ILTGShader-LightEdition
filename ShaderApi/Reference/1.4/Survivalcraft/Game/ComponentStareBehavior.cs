using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000231 RID: 561
	public class ComponentStareBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x00086403 File Offset: 0x00084603
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600121B RID: 4635 RVA: 0x00086406 File Offset: 0x00084606
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0008640E File Offset: 0x0008460E
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0008641C File Offset: 0x0008461C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stareRange = valuesDictionary.GetValue<float>("StareRange");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
			}, delegate
			{
				if (this.m_subsystemTime.GameTime > this.m_stareEndTime + 8.0 && this.m_random.Float(0f, 1f) < 1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_target = this.FindTarget();
					if (this.m_target != null)
					{
						float probability = (this.m_target.Entity.FindComponent<ComponentPlayer>() != null) ? 1f : 0.25f;
						if (this.m_random.Bool(probability))
						{
							this.m_importanceLevel = this.m_random.Float(3f, 5f);
						}
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Stare");
				}
			}, null);
			this.m_stateMachine.AddState("Stare", delegate
			{
				this.m_stareEndTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(6f, 12f);
				if (this.m_target != null)
				{
					Vector3 position = this.m_componentCreature.ComponentBody.Position;
					Vector3 v = Vector3.Normalize(this.m_target.ComponentBody.Position - position);
					this.m_componentPathfinding.SetDestination(new Vector3?(position + 1.1f * v), this.m_random.Float(0.3f, 0.4f), 1f, 0, false, true, false, null);
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					}
				}
			}, delegate
			{
				if (!this.IsActive || this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_subsystemTime.GameTime > this.m_stareEndTime)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (this.m_random.Float(0f, 1f) < 1f * this.m_subsystemTime.GameTimeDelta && this.ScoreTarget(this.m_target) <= 0f)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_target.ComponentCreatureModel.EyePosition);
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x000864E4 File Offset: 0x000846E4
		public virtual ComponentCreature FindTarget()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), this.m_stareRange, this.m_componentBodies);
			ComponentCreature result = null;
			float num = 0f;
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentCreature componentCreature = this.m_componentBodies.Array[i].Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					float num2 = this.ScoreTarget(componentCreature);
					if (num2 > num)
					{
						result = componentCreature;
						num = num2;
					}
				}
			}
			return result;
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00086588 File Offset: 0x00084788
		public virtual float ScoreTarget(ComponentCreature componentCreature)
		{
			if (componentCreature != this.m_componentCreature && componentCreature.Entity.IsAddedToProject)
			{
				float num = Vector3.Distance(this.m_componentCreature.ComponentBody.Position, componentCreature.ComponentBody.Position);
				float num2 = this.m_stareRange - num;
				if (this.m_random.Float(0f, 1f) < 0.66f && componentCreature.Entity.FindComponent<ComponentPlayer>() != null)
				{
					num2 *= 100f;
				}
				return num2;
			}
			return 0f;
		}

		// Token: 0x04000B04 RID: 2820
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B05 RID: 2821
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000B06 RID: 2822
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B07 RID: 2823
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B08 RID: 2824
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000B09 RID: 2825
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B0A RID: 2826
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B0B RID: 2827
		public float m_importanceLevel;

		// Token: 0x04000B0C RID: 2828
		public float m_stareRange;

		// Token: 0x04000B0D RID: 2829
		public double m_stareEndTime;

		// Token: 0x04000B0E RID: 2830
		public ComponentCreature m_target;
	}
}
