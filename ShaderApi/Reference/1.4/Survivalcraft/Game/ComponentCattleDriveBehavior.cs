using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F1 RID: 497
	public class ComponentCattleDriveBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x0006660C File Offset: 0x0006480C
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000DF9 RID: 3577 RVA: 0x0006660F File Offset: 0x0006480F
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00066618 File Offset: 0x00064818
		public virtual void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 0.5f)
			{
				Vector3 v = this.m_componentCreature.ComponentBody.Position - sourcePosition;
				this.m_driveVector += Vector3.Normalize(v) * MathUtils.Max(8f - 0.25f * v.Length(), 1f);
				float num = 12f + this.m_random.Float(0f, 3f);
				if (this.m_driveVector.Length() > num)
				{
					this.m_driveVector = num * Vector3.Normalize(this.m_driveVector);
				}
			}
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x000666C1 File Offset: 0x000648C1
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x000666D0 File Offset: 0x000648D0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_driveVector = Vector3.Zero;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Drive");
				}
				if (this.m_driveVector.Length() > 3f)
				{
					this.m_importanceLevel = 7f;
				}
				this.FadeDriveVector();
			}, null);
			this.m_stateMachine.AddState("Drive", delegate
			{
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_driveVector.LengthSquared() < 1f || this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_random.Float(0f, 1f) < 3f * this.m_subsystemTime.GameTimeDelta)
				{
					Vector3 v = this.CalculateDriveDirectionAndSpeed();
					float speed = MathUtils.Saturate(0.2f * v.Length());
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + 15f * Vector3.Normalize(v)), speed, 5f, 0, false, true, false, null);
				}
				this.FadeDriveVector();
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x000667AC File Offset: 0x000649AC
		public virtual void FadeDriveVector()
		{
			float num = this.m_driveVector.Length();
			if (num > 0.1f)
			{
				this.m_driveVector -= this.m_subsystemTime.GameTimeDelta * this.m_driveVector / num;
			}
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x000667FC File Offset: 0x000649FC
		public virtual Vector3 CalculateDriveDirectionAndSpeed()
		{
			int num = 1;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = position;
			Vector3 vector2 = this.m_driveVector;
			foreach (ComponentCreature componentCreature in this.m_subsystemCreatureSpawn.Creatures)
			{
				if (componentCreature != this.m_componentCreature && componentCreature.ComponentHealth.Health > 0f)
				{
					ComponentCattleDriveBehavior componentCattleDriveBehavior = componentCreature.Entity.FindComponent<ComponentCattleDriveBehavior>();
					if (componentCattleDriveBehavior != null && componentCattleDriveBehavior.m_componentHerdBehavior.HerdName == this.m_componentHerdBehavior.HerdName)
					{
						Vector3 position2 = componentCreature.ComponentBody.Position;
						if (Vector3.DistanceSquared(position, position2) < 625f)
						{
							vector += position2;
							vector2 += componentCattleDriveBehavior.m_driveVector;
							num++;
						}
					}
				}
			}
			vector /= (float)num;
			vector2 /= (float)num;
			Vector3 v = vector - position;
			float s = MathUtils.Max(1.5f * v.Length() - 3f, 0f);
			return 0.33f * this.m_driveVector + 0.66f * vector2 + s * Vector3.Normalize(v);
		}

		// Token: 0x04000761 RID: 1889
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000762 RID: 1890
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x04000763 RID: 1891
		public ComponentCreature m_componentCreature;

		// Token: 0x04000764 RID: 1892
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000765 RID: 1893
		public ComponentHerdBehavior m_componentHerdBehavior;

		// Token: 0x04000766 RID: 1894
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000767 RID: 1895
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000768 RID: 1896
		public float m_importanceLevel;

		// Token: 0x04000769 RID: 1897
		public Vector3 m_driveVector;
	}
}
