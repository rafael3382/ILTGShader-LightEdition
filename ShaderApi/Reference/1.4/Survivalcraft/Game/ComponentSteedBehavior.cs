using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000232 RID: 562
	public class ComponentSteedBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x000868AB File Offset: 0x00084AAB
		// (set) Token: 0x06001226 RID: 4646 RVA: 0x000868B3 File Offset: 0x00084AB3
		public int SpeedOrder { get; set; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x000868BC File Offset: 0x00084ABC
		// (set) Token: 0x06001228 RID: 4648 RVA: 0x000868C4 File Offset: 0x00084AC4
		public float TurnOrder { get; set; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x000868CD File Offset: 0x00084ACD
		// (set) Token: 0x0600122A RID: 4650 RVA: 0x000868D5 File Offset: 0x00084AD5
		public float JumpOrder { get; set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x000868DE File Offset: 0x00084ADE
		// (set) Token: 0x0600122C RID: 4652 RVA: 0x000868E6 File Offset: 0x00084AE6
		public bool WasOrderIssued { get; set; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x000868EF File Offset: 0x00084AEF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x000868F2 File Offset: 0x00084AF2
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x000868FC File Offset: 0x00084AFC
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			if (this.SpeedOrder != 0 || this.TurnOrder != 0f || this.JumpOrder != 0f)
			{
				this.SpeedOrder = 0;
				this.TurnOrder = 0f;
				this.JumpOrder = 0f;
				this.WasOrderIssued = true;
			}
			else
			{
				this.WasOrderIssued = false;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)((float)(this.GetHashCode() % 100) * 0.01f)))
			{
				this.m_importanceLevel = 0f;
				if (this.m_isEnabled)
				{
					if (this.m_componentMount.Rider != null)
					{
						this.m_importanceLevel = 275f;
					}
					else if (this.FindNearbyRider(7f) != null)
					{
						this.m_importanceLevel = 7f;
					}
				}
			}
			if (!this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x000869E8 File Offset: 0x00084BE8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_isEnabled = base.Entity.ValuesDictionary.DatabaseObject.Name.EndsWith("_Saddled");
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Wait");
				}
			}, null);
			this.m_stateMachine.AddState("Wait", delegate
			{
				ComponentRider componentRider = this.FindNearbyRider(6f);
				if (componentRider != null)
				{
					this.m_componentPathfinding.SetDestination(new Vector3?(componentRider.ComponentCreature.ComponentBody.Position), this.m_random.Float(0.2f, 0.3f), 3.25f, 0, false, true, false, null);
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
					}
				}
			}, delegate
			{
				if (this.m_componentMount.Rider != null)
				{
					this.m_stateMachine.TransitionTo("Steed");
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.AddState("Steed", delegate
			{
				this.m_componentPathfinding.Stop();
				this.m_speed = 0f;
				this.m_speedLevel = 1;
			}, delegate
			{
				this.ProcessRidingOrders();
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x00086AF4 File Offset: 0x00084CF4
		public virtual ComponentRider FindNearbyRider(float range)
		{
			this.m_bodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(this.m_componentCreature.ComponentBody.Position.X, this.m_componentCreature.ComponentBody.Position.Z), range, this.m_bodies);
			foreach (ComponentBody componentBody in this.m_bodies)
			{
				if (Vector3.DistanceSquared(this.m_componentCreature.ComponentBody.Position, componentBody.Position) < range * range)
				{
					ComponentRider componentRider = componentBody.Entity.FindComponent<ComponentRider>();
					if (componentRider != null)
					{
						return componentRider;
					}
				}
			}
			return null;
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x00086BC4 File Offset: 0x00084DC4
		public virtual void ProcessRidingOrders()
		{
			this.m_speedLevel = MathUtils.Clamp(this.m_speedLevel + this.SpeedOrder, 0, this.m_speedLevels.Length - 1);
			if (this.m_speedLevel == this.m_speedLevels.Length - 1 && this.SpeedOrder > 0)
			{
				this.m_timeToSpeedReduction = this.m_random.Float(8f, 12f);
			}
			if (this.m_speedLevel == 0 && this.SpeedOrder < 0)
			{
				this.m_timeToSpeedReduction = 1.25f;
			}
			this.m_timeToSpeedReduction -= this.m_subsystemTime.GameTimeDelta;
			if (this.m_timeToSpeedReduction <= 0f && this.m_speedLevel == this.m_speedLevels.Length - 1)
			{
				this.m_speedLevel--;
				this.m_speedChangeFactor = 0.25f;
			}
			else if (this.m_timeToSpeedReduction <= 0f && this.m_speedLevel == 0)
			{
				this.m_speedLevel = 1;
				this.m_speedChangeFactor = 100f;
			}
			else
			{
				this.m_speedChangeFactor = 100f;
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(0.25, 0.0))
			{
				float num = new Vector2(this.m_componentCreature.ComponentBody.CollisionVelocityChange.X, this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z).Length();
				if (this.m_speedLevel == 0 || num < 0.1f || this.m_componentCreature.ComponentBody.Velocity.Length() > MathUtils.Abs(0.5f * this.m_speed * this.m_componentCreature.ComponentLocomotion.WalkSpeed))
				{
					this.m_lastNotBlockedTime = this.m_subsystemTime.GameTime;
				}
				else if (this.m_subsystemTime.GameTime - this.m_lastNotBlockedTime > 0.75)
				{
					this.m_speedLevel = 1;
				}
			}
			this.m_speed += MathUtils.Saturate(this.m_speedChangeFactor * this.m_subsystemTime.GameTimeDelta) * (this.m_speedLevels[this.m_speedLevel] - this.m_speed);
			this.m_turnSpeed += 2f * this.m_subsystemTime.GameTimeDelta * (MathUtils.Clamp(this.TurnOrder, -0.5f, 0.5f) - this.m_turnSpeed);
			this.m_componentCreature.ComponentLocomotion.TurnOrder = new Vector2(this.m_turnSpeed, 0f);
			this.m_componentCreature.ComponentLocomotion.WalkOrder = new Vector2?(new Vector2(0f, this.m_speed));
			if (MathUtils.Abs(this.m_speed) > 0.01f || MathUtils.Abs(this.m_turnSpeed) > 0.01f)
			{
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(2f * this.m_turnSpeed, 0f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_componentCreature.ComponentLocomotion.JumpOrder, this.JumpOrder);
		}

		// Token: 0x04000B0F RID: 2831
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B10 RID: 2832
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000B11 RID: 2833
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B12 RID: 2834
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B13 RID: 2835
		public ComponentMount m_componentMount;

		// Token: 0x04000B14 RID: 2836
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B15 RID: 2837
		public float m_importanceLevel;

		// Token: 0x04000B16 RID: 2838
		public bool m_isEnabled;

		// Token: 0x04000B17 RID: 2839
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B18 RID: 2840
		public DynamicArray<ComponentBody> m_bodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000B19 RID: 2841
		public float[] m_speedLevels = new float[]
		{
			-0.33f,
			0f,
			0.33f,
			0.66f,
			1f
		};

		// Token: 0x04000B1A RID: 2842
		public int m_speedLevel;

		// Token: 0x04000B1B RID: 2843
		public float m_speed;

		// Token: 0x04000B1C RID: 2844
		public float m_turnSpeed;

		// Token: 0x04000B1D RID: 2845
		public float m_speedChangeFactor;

		// Token: 0x04000B1E RID: 2846
		public float m_timeToSpeedReduction;

		// Token: 0x04000B1F RID: 2847
		public double m_lastNotBlockedTime;
	}
}
