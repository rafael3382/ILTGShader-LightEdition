using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000207 RID: 519
	public class ComponentFlyAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x00070692 File Offset: 0x0006E892
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000F31 RID: 3889 RVA: 0x00070695 File Offset: 0x0006E895
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x000706A0 File Offset: 0x0006E8A0
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_random.Float(0f, 1f) < 0.05f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2f);
			}
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0007072C File Offset: 0x0006E92C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Fly");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.m_stateMachine.TransitionTo("Fly");
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
					this.m_importanceLevel = 1f;
				}
			}, null, null);
			this.m_stateMachine.AddState("Fly", delegate
			{
				this.m_angle = this.m_random.Float(0f, 6.28318548f);
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination == null)
				{
					float num = (this.m_random.Float(0f, 1f) < 0.2f) ? this.m_random.Float(0.4f, 0.6f) : (0f - this.m_random.Float(0.4f, 0.6f));
					this.m_angle = MathUtils.NormalizeAngle(this.m_angle + num);
					Vector2 vector = Vector2.CreateFromAngle(this.m_angle);
					Vector3 vector2 = position + new Vector3(vector.X, 0f, vector.Y) * 10f;
					vector2.Y = this.EstimateHeight(new Vector2(vector2.X, vector2.Z), 8) + this.m_random.Float(3f, 5f);
					this.m_componentPathfinding.SetDestination(new Vector3?(vector2), this.m_random.Float(0.6f, 1.05f), 6f, 0, false, true, false, null);
					if (this.m_random.Float(0f, 1f) < 0.15f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
						return;
					}
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x000707D4 File Offset: 0x0006E9D4
		public virtual float EstimateHeight(Vector2 position, int radius)
		{
			int num = 0;
			for (int i = 0; i < 15; i++)
			{
				int x = Terrain.ToCell(position.X) + this.m_random.Int(-radius, radius);
				int z = Terrain.ToCell(position.Y) + this.m_random.Int(-radius, radius);
				num = MathUtils.Max(num, this.m_subsystemTerrain.Terrain.GetTopHeight(x, z));
			}
			return (float)num;
		}

		// Token: 0x040008A3 RID: 2211
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008A4 RID: 2212
		public ComponentCreature m_componentCreature;

		// Token: 0x040008A5 RID: 2213
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040008A6 RID: 2214
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040008A7 RID: 2215
		public float m_angle;

		// Token: 0x040008A8 RID: 2216
		public float m_importanceLevel = 1f;

		// Token: 0x040008A9 RID: 2217
		public Game.Random m_random = new Game.Random();
	}
}
