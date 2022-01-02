using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000210 RID: 528
	public class ComponentHerdBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x0007584D File Offset: 0x00073A4D
		// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x00075855 File Offset: 0x00073A55
		public string HerdName { get; set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0007585E File Offset: 0x00073A5E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x00075861 File Offset: 0x00073A61
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x0007586C File Offset: 0x00073A6C
		public void CallNearbyCreaturesHelp(ComponentCreature target, float maxRange, float maxChaseTime, bool isPersistent)
		{
			Vector3 position = target.ComponentBody.Position;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (Vector3.DistanceSquared(position, componentCreature.ComponentBody.Position) < 256f)
				{
					ComponentHerdBehavior componentHerdBehavior = componentCreature.Entity.FindComponent<ComponentHerdBehavior>();
					if (componentHerdBehavior != null && componentHerdBehavior.HerdName == this.HerdName && componentHerdBehavior.m_autoNearbyCreaturesHelp)
					{
						ComponentChaseBehavior componentChaseBehavior = componentCreature.Entity.FindComponent<ComponentChaseBehavior>();
						if (componentChaseBehavior != null && componentChaseBehavior.Target == null)
						{
							componentChaseBehavior.Attack(target, maxRange, maxChaseTime, isPersistent);
						}
					}
				}
			}
			ModsManager.HookAction("CallNearbyCreaturesHelp", delegate(ModLoader modLoader)
			{
				modLoader.CallNearbyCreaturesHelp(this, target, maxRange, maxChaseTime, isPersistent);
				return false;
			});
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00075994 File Offset: 0x00073B94
		public Vector3? FindHerdCenter()
		{
			if (string.IsNullOrEmpty(this.HerdName))
			{
				return null;
			}
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			int num = 0;
			Vector3 vector = Vector3.Zero;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (componentCreature.ComponentHealth.Health > 0f)
				{
					ComponentHerdBehavior componentHerdBehavior = componentCreature.Entity.FindComponent<ComponentHerdBehavior>();
					if (componentHerdBehavior != null && componentHerdBehavior.HerdName == this.HerdName)
					{
						Vector3 position2 = componentCreature.ComponentBody.Position;
						if (Vector3.DistanceSquared(position, position2) < this.m_herdingRange * this.m_herdingRange)
						{
							vector += position2;
							num++;
						}
					}
				}
			}
			if (num > 0)
			{
				return new Vector3?(vector / (float)num);
			}
			return null;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00075AA0 File Offset: 0x00073CA0
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState) || !this.IsActive)
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			this.m_dt = dt;
			this.m_stateMachine.Update();
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00075AE0 File Offset: 0x00073CE0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.HerdName = valuesDictionary.GetValue<string>("HerdName");
			this.m_herdingRange = valuesDictionary.GetValue<float>("HerdingRange");
			this.m_autoNearbyCreaturesHelp = valuesDictionary.GetValue<bool>("AutoNearbyCreaturesHelp");
			ComponentHealth componentHealth = this.m_componentCreature.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature attacker)
			{
				this.CallNearbyCreaturesHelp(attacker, 20f, 30f, false);
			}));
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)(1f * ((float)(this.GetHashCode() % 256) / 256f))))
				{
					Vector3? vector = this.FindHerdCenter();
					if (vector != null)
					{
						float num = Vector3.Distance(vector.Value, this.m_componentCreature.ComponentBody.Position);
						if (num > 10f)
						{
							this.m_importanceLevel = 1f;
						}
						if (num > 12f)
						{
							this.m_importanceLevel = 3f;
						}
						if (num > 16f)
						{
							this.m_importanceLevel = 50f;
						}
						if (num > 20f)
						{
							this.m_importanceLevel = 250f;
						}
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Herd");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Herd");
				if (this.m_random.Bool(0.5f))
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					this.m_importanceLevel = 0f;
				}
			}, null, null);
			this.m_stateMachine.AddState("Herd", delegate
			{
				Vector3? vector = this.FindHerdCenter();
				if (vector != null && Vector3.Distance(this.m_componentCreature.ComponentBody.Position, vector.Value) > 6f)
				{
					float speed = (this.m_importanceLevel > 10f) ? this.m_random.Float(0.9f, 1f) : this.m_random.Float(0.25f, 0.35f);
					int maxPathfindingPositions = (this.m_importanceLevel > 200f) ? 100 : 0;
					this.m_componentPathfinding.SetDestination(new Vector3?(vector.Value), speed, 7f, maxPathfindingPositions, false, true, false, null);
					return;
				}
				this.m_importanceLevel = 0f;
			}, delegate
			{
				this.m_componentCreature.ComponentLocomotion.LookOrder = this.m_look - this.m_componentCreature.ComponentLocomotion.LookAngles;
				if (this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("Stuck");
				}
				if (this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.05f * this.m_dt)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				if (this.m_random.Float(0f, 1f) < 1.5f * this.m_dt)
				{
					this.m_look = new Vector2(MathUtils.DegToRad(45f) * this.m_random.Float(-1f, 1f), MathUtils.DegToRad(10f) * this.m_random.Float(-1f, 1f));
				}
			}, null);
		}

		// Token: 0x04000940 RID: 2368
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x04000941 RID: 2369
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000942 RID: 2370
		public ComponentCreature m_componentCreature;

		// Token: 0x04000943 RID: 2371
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000944 RID: 2372
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000945 RID: 2373
		public float m_dt;

		// Token: 0x04000946 RID: 2374
		public float m_importanceLevel;

		// Token: 0x04000947 RID: 2375
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000948 RID: 2376
		public Vector2 m_look;

		// Token: 0x04000949 RID: 2377
		public float m_herdingRange;

		// Token: 0x0400094A RID: 2378
		public bool m_autoNearbyCreaturesHelp;
	}
}
