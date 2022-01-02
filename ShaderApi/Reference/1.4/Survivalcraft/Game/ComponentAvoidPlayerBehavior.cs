using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E9 RID: 489
	public class ComponentAvoidPlayerBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000D71 RID: 3441 RVA: 0x00061B9F File Offset: 0x0005FD9F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x00061BA2 File Offset: 0x0005FDA2
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x00061BAC File Offset: 0x0005FDAC
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_dt = this.m_random.Float(0.4f, 0.6f) + MathUtils.Min((float)(this.m_subsystemTime.GameTime - this.m_nextUpdateTime), 0.1f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_dt;
				this.m_stateMachine.Update();
			}
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x00061C2C File Offset: 0x0005FE2C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_dayRange = valuesDictionary.GetValue<float>("DayRange");
			this.m_nightRange = valuesDictionary.GetValue<float>("NightRange");
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
				float importanceLevel;
				this.m_target = this.FindTarget(out importanceLevel);
				if (this.m_target != null)
				{
					Vector3.Distance(this.m_target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
					this.SetImportanceLevel(importanceLevel);
					return;
				}
				this.m_importanceLevel = 0f;
			}, null);
			this.m_stateMachine.AddState("Move", null, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (this.m_target == null || this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				float num = this.ScoreTarget(this.m_target);
				this.SetImportanceLevel(num);
				Vector3 vector = Vector3.Normalize(this.m_componentCreature.ComponentBody.Position - this.m_target.ComponentBody.Position);
				Vector3 value = this.m_componentCreature.ComponentBody.Position + 10f * Vector3.Normalize(new Vector3(vector.X, 0f, vector.Z));
				this.m_componentPathfinding.SetDestination(new Vector3?(value), MathUtils.Lerp(0.6f, 1f, num), 1f, 0, false, true, false, null);
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x00061D0C File Offset: 0x0005FF0C
		public virtual void SetImportanceLevel(float score)
		{
			this.m_importanceLevel = MathUtils.Lerp(4f, 8f, MathUtils.Sqrt(score));
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00061D2C File Offset: 0x0005FF2C
		public virtual ComponentCreature FindTarget(out float targetScore)
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
			targetScore = num;
			return result;
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x00061DDC File Offset: 0x0005FFDC
		public virtual float ScoreTarget(ComponentCreature target)
		{
			float num = (this.m_subsystemSky.SkyLightIntensity > 0.2f) ? this.m_dayRange : this.m_nightRange;
			if (num <= 0f)
			{
				return 0f;
			}
			if (!target.IsAddedToProject || target.ComponentHealth.Health <= 0f || target.Entity.FindComponent<ComponentPlayer>() == null)
			{
				return 0f;
			}
			float num2 = Vector3.Distance(target.ComponentBody.Position, this.m_componentCreature.ComponentBody.Position);
			return MathUtils.Saturate(1f - num2 / num);
		}

		// Token: 0x040006FB RID: 1787
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006FC RID: 1788
		public SubsystemSky m_subsystemSky;

		// Token: 0x040006FD RID: 1789
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040006FE RID: 1790
		public ComponentCreature m_componentCreature;

		// Token: 0x040006FF RID: 1791
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000700 RID: 1792
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000701 RID: 1793
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000702 RID: 1794
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000703 RID: 1795
		public float m_importanceLevel;

		// Token: 0x04000704 RID: 1796
		public float m_dayRange;

		// Token: 0x04000705 RID: 1797
		public float m_nightRange;

		// Token: 0x04000706 RID: 1798
		public float m_dt;

		// Token: 0x04000707 RID: 1799
		public ComponentCreature m_target;

		// Token: 0x04000708 RID: 1800
		public double m_nextUpdateTime;
	}
}
