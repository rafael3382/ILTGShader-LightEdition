using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000220 RID: 544
	public class ComponentMoveAwayBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06001106 RID: 4358 RVA: 0x0007F0AE File Offset: 0x0007D2AE
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x0007F0B1 File Offset: 0x0007D2B1
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0007F0B9 File Offset: 0x0007D2B9
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0007F0C8 File Offset: 0x0007D2C8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			ComponentBody componentBody = this.m_componentCreature.ComponentBody;
			componentBody.CollidedWithBody = (Action<ComponentBody>)Delegate.Combine(componentBody.CollidedWithBody, new Action<ComponentBody>(delegate(ComponentBody body)
			{
				this.m_target = body;
				this.m_isFast = (MathUtils.Max(body.Velocity.Length(), this.m_componentCreature.ComponentBody.Velocity.Length()) > 3f);
			}));
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
				if (this.m_target != null)
				{
					this.m_importanceLevel = 6f;
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_target != null)
				{
					Vector3 vector = this.m_target.Position + 0.5f * this.m_target.Velocity;
					Vector2 v = Vector2.Normalize(this.m_componentCreature.ComponentBody.Position.XZ - vector.XZ);
					Vector2 vector2 = Vector2.Zero;
					float num = float.MinValue;
					for (float num2 = 0f; num2 < 6.28318548f; num2 += 0.1f)
					{
						Vector2 vector3 = Vector2.CreateFromAngle(num2);
						if (Vector2.Dot(vector3, v) > 0.2f)
						{
							float num3 = Vector2.Dot(this.m_componentCreature.ComponentBody.Matrix.Forward.XZ, vector3);
							if (num3 > num)
							{
								vector2 = vector3;
								num = num3;
							}
						}
					}
					float s = this.m_random.Float(1.5f, 2f);
					float speed = this.m_isFast ? 0.7f : 0.35f;
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + s * new Vector3(vector2.X, 0f, vector2.Y)), speed, 1f, 0, false, true, false, null);
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_componentPathfinding.IsStuck || this.m_componentPathfinding.Destination == null)
				{
					this.m_importanceLevel = 0f;
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x04000A27 RID: 2599
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A28 RID: 2600
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000A29 RID: 2601
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A2A RID: 2602
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A2B RID: 2603
		public float m_importanceLevel;

		// Token: 0x04000A2C RID: 2604
		public ComponentBody m_target;

		// Token: 0x04000A2D RID: 2605
		public bool m_isFast;
	}
}
