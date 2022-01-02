using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FF RID: 511
	public class ComponentDumpRiderBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x0006C497 File Offset: 0x0006A697
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x0006C49A File Offset: 0x0006A69A
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0006C4A2 File Offset: 0x0006A6A2
		public virtual void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0006C4B0 File Offset: 0x0006A6B0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_isEnabled = !base.Entity.ValuesDictionary.DatabaseObject.Name.EndsWith("_Saddled");
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_rider = null;
			}, delegate
			{
				if (this.m_isEnabled && this.m_random.Float(0f, 1f) < 1f * this.m_subsystemTime.GameTimeDelta && this.m_componentMount.Rider != null)
				{
					this.m_importanceLevel = 220f;
					this.m_dumpStartTime = this.m_subsystemTime.GameTime;
					this.m_rider = this.m_componentMount.Rider;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("WildJumping");
				}
			}, null);
			this.m_stateMachine.AddState("WildJumping", delegate
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentMount.Rider == null)
				{
					this.m_importanceLevel = 0f;
					this.RunAway();
				}
				if (this.m_random.Float(0f, 1f) < 1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				}
				if (this.m_random.Float(0f, 1f) < 3f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_walkOrder = new Vector2(this.m_random.Float(-0.5f, 0.5f), this.m_random.Float(-0.5f, 1.5f));
				}
				if (this.m_random.Float(0f, 1f) < 2.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_turnOrder.X = this.m_random.Float(-1f, 1f);
				}
				if (this.m_random.Float(0f, 1f) < 2f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentLocomotion.JumpOrder = this.m_random.Float(0.9f, 1f);
					if (this.m_componentMount.Rider != null && this.m_subsystemTime.GameTime - this.m_dumpStartTime > 3.0)
					{
						if (this.m_random.Float(0f, 1f) < 0.05f)
						{
							this.m_componentMount.Rider.StartDismounting();
							this.m_componentMount.Rider.ComponentCreature.ComponentHealth.Injure(this.m_random.Float(0.05f, 0.2f), this.m_componentCreature, false, "Thrown from a mount");
						}
						if (this.m_random.Float(0f, 1f) < 0.25f)
						{
							this.m_componentMount.Rider.ComponentCreature.ComponentHealth.Injure(0.05f, this.m_componentCreature, false, "Thrown from a mount");
						}
					}
				}
				if (this.m_random.Float(0f, 1f) < 4f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_lookOrder = new Vector2(this.m_random.Float(-3f, 3f), this.m_lookOrder.Y);
				}
				if (this.m_random.Float(0f, 1f) < 0.25f * this.m_subsystemTime.GameTimeDelta)
				{
					this.TransitionToRandomDumpingBehavior();
				}
				this.m_componentCreature.ComponentLocomotion.WalkOrder = new Vector2?(this.m_walkOrder);
				this.m_componentCreature.ComponentLocomotion.TurnOrder = this.m_turnOrder;
				this.m_componentCreature.ComponentLocomotion.LookOrder = this.m_lookOrder;
			}, null);
			this.m_stateMachine.AddState("BlindRacing", delegate
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + new Vector3(this.m_random.Float(-15f, 15f), 0f, this.m_random.Float(-15f, 15f))), 1f, 2f, 0, false, true, false, null);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentMount.Rider == null)
				{
					this.m_importanceLevel = 0f;
					this.RunAway();
				}
				else if (this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.TransitionToRandomDumpingBehavior();
				}
				if (this.m_random.Float(0f, 1f) < 0.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentLocomotion.JumpOrder = 1f;
					this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				}
			}, null);
			this.m_stateMachine.AddState("Stupor", delegate
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentMount.Rider == null)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
				{
					this.TransitionToRandomDumpingBehavior();
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0006C5E4 File Offset: 0x0006A7E4
		public virtual void TransitionToRandomDumpingBehavior()
		{
			float num = this.m_random.Float(0f, 1f);
			if (num < 0.5f)
			{
				this.m_stateMachine.TransitionTo("WildJumping");
				return;
			}
			if (num < 0.8f)
			{
				this.m_stateMachine.TransitionTo("BlindRacing");
				return;
			}
			this.m_stateMachine.TransitionTo("Stupor");
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0006C649 File Offset: 0x0006A849
		public virtual void RunAway()
		{
			if (this.m_rider != null)
			{
				ComponentRunAwayBehavior componentRunAwayBehavior = base.Entity.FindComponent<ComponentRunAwayBehavior>();
				if (componentRunAwayBehavior == null)
				{
					return;
				}
				componentRunAwayBehavior.RunAwayFrom(this.m_rider.ComponentCreature.ComponentBody);
			}
		}

		// Token: 0x04000827 RID: 2087
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000828 RID: 2088
		public ComponentCreature m_componentCreature;

		// Token: 0x04000829 RID: 2089
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x0400082A RID: 2090
		public ComponentMount m_componentMount;

		// Token: 0x0400082B RID: 2091
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400082C RID: 2092
		public float m_importanceLevel;

		// Token: 0x0400082D RID: 2093
		public bool m_isEnabled;

		// Token: 0x0400082E RID: 2094
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400082F RID: 2095
		public ComponentRider m_rider;

		// Token: 0x04000830 RID: 2096
		public double m_dumpStartTime;

		// Token: 0x04000831 RID: 2097
		public Vector2 m_walkOrder;

		// Token: 0x04000832 RID: 2098
		public Vector2 m_turnOrder;

		// Token: 0x04000833 RID: 2099
		public Vector2 m_lookOrder;
	}
}
