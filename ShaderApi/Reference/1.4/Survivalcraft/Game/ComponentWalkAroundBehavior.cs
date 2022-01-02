using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000239 RID: 569
	public class ComponentWalkAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06001288 RID: 4744 RVA: 0x0008A02A File Offset: 0x0008822A
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06001289 RID: 4745 RVA: 0x0008A02D File Offset: 0x0008822D
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x0008A035 File Offset: 0x00088235
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x0008A044 File Offset: 0x00088244
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = this.m_random.Float(0f, 1f);
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.05f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_importanceLevel = this.m_random.Float(1f, 2f);
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Walk");
				}
			}, null);
			this.m_stateMachine.AddState("Walk", delegate
			{
				float speed = (this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f) ? 1f : this.m_random.Float(0.25f, 0.35f);
				this.m_componentPathfinding.SetDestination(new Vector3?(this.FindDestination()), speed, 1f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.IsStuck || !this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_componentPathfinding.Destination == null)
				{
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_stateMachine.TransitionTo("Inactive");
					}
					else
					{
						this.m_stateMachine.TransitionTo(null);
						this.m_stateMachine.TransitionTo("Walk");
					}
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(false);
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x0008A0FC File Offset: 0x000882FC
		public virtual Vector3 FindDestination()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			float num = 0f;
			Vector3 result = position;
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector = Vector2.Normalize(this.m_random.Vector2(1f)) * this.m_random.Float(6f, 12f);
				Vector3 vector2 = new Vector3(position.X + vector.X, 0f, position.Z + vector.Y);
				vector2.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector2.X), Terrain.ToCell(vector2.Z)) + 1);
				float num2 = this.ScoreDestination(vector2);
				if (num2 > num)
				{
					num = num2;
					result = vector2;
				}
			}
			return result;
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0008A1D8 File Offset: 0x000883D8
		public virtual float ScoreDestination(Vector3 destination)
		{
			float num = 8f - MathUtils.Abs(this.m_componentCreature.ComponentBody.Position.Y - destination.Y);
			if (this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(destination.X), Terrain.ToCell(destination.Y) - 1, Terrain.ToCell(destination.Z)) == 18)
			{
				num -= 5f;
			}
			return num;
		}

		// Token: 0x04000B6C RID: 2924
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000B6D RID: 2925
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B6E RID: 2926
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B6F RID: 2927
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B70 RID: 2928
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B71 RID: 2929
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B72 RID: 2930
		public float m_importanceLevel;
	}
}
