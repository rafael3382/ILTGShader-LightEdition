using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000228 RID: 552
	public class ComponentRandomPeckBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x0600119D RID: 4509 RVA: 0x00082C62 File Offset: 0x00080E62
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x0600119E RID: 4510 RVA: 0x00082C65 File Offset: 0x00080E65
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00082C70 File Offset: 0x00080E70
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Move");
			}
			if (this.m_random.Float(0f, 1f) < 0.033f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2.5f);
			}
			this.m_dt = dt;
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00082D04 File Offset: 0x00080F04
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentBirdModel = base.Entity.FindComponent<ComponentBirdModel>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Move");
			}, null, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				float num = (this.m_random.Float(0f, 1f) < 0.2f) ? 8f : 3f;
				Vector3 vector = position + new Vector3(num * this.m_random.Float(-1f, 1f), 0f, num * this.m_random.Float(-1f, 1f));
				vector.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Z)) + 1);
				this.m_componentPathfinding.SetDestination(new Vector3?(vector), this.m_random.Float(0.5f, 0.7f), 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.Destination != null)
				{
					if (this.m_componentPathfinding.IsStuck)
					{
						this.m_stateMachine.TransitionTo("Stuck");
					}
					return;
				}
				if (this.m_random.Float(0f, 1f) < 0.33f)
				{
					this.m_stateMachine.TransitionTo("Wait");
					return;
				}
				this.m_stateMachine.TransitionTo("Peck");
			}, null);
			this.m_stateMachine.AddState("Wait", delegate
			{
				this.m_waitTime = this.m_random.Float(0.75f, 1f);
			}, delegate
			{
				this.m_waitTime -= this.m_dt;
				if (this.m_waitTime <= 0f)
				{
					if (this.m_random.Float(0f, 1f) < 0.25f)
					{
						this.m_stateMachine.TransitionTo("Move");
						if (this.m_random.Float(0f, 1f) < 0.33f)
						{
							this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
							return;
						}
					}
					else
					{
						this.m_stateMachine.TransitionTo("Peck");
					}
				}
			}, null);
			this.m_stateMachine.AddState("Peck", delegate
			{
				this.m_peckTime = this.m_random.Float(2f, 6f);
			}, delegate
			{
				this.m_peckTime -= this.m_dt;
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
				{
					this.m_componentBirdModel.FeedOrder = true;
				}
				if (this.m_peckTime <= 0f)
				{
					if (this.m_random.Float(0f, 1f) < 0.25f)
					{
						this.m_stateMachine.TransitionTo("Move");
						return;
					}
					this.m_stateMachine.TransitionTo("Wait");
				}
			}, null);
		}

		// Token: 0x04000A9A RID: 2714
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A9B RID: 2715
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A9C RID: 2716
		public ComponentBirdModel m_componentBirdModel;

		// Token: 0x04000A9D RID: 2717
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000A9E RID: 2718
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A9F RID: 2719
		public float m_importanceLevel = 1f;

		// Token: 0x04000AA0 RID: 2720
		public float m_dt;

		// Token: 0x04000AA1 RID: 2721
		public float m_peckTime;

		// Token: 0x04000AA2 RID: 2722
		public float m_waitTime;

		// Token: 0x04000AA3 RID: 2723
		public Game.Random m_random = new Game.Random();
	}
}
