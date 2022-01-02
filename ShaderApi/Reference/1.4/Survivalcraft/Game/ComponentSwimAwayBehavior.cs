using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000236 RID: 566
	public class ComponentSwimAwayBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x00087ACC File Offset: 0x00085CCC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x00087ACF File Offset: 0x00085CCF
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00087AD7 File Offset: 0x00085CD7
		public virtual void SwimAwayFrom(ComponentBody attacker)
		{
			this.m_attacker = attacker;
			this.m_timeToForgetAttacker = this.m_random.Float(10f, 20f);
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00087AFB File Offset: 0x00085CFB
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00087B08 File Offset: 0x00085D08
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>();
			ComponentHealth componentHealth = this.m_componentCreature.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature attacker)
			{
				this.SwimAwayFrom(attacker.ComponentBody);
			}));
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_attacker = null;
			}, delegate
			{
				if (this.m_attacker != null)
				{
					this.m_timeToForgetAttacker -= this.m_subsystemTime.GameTimeDelta;
					if (this.m_timeToForgetAttacker <= 0f)
					{
						this.m_attacker = null;
					}
				}
				if (this.m_componentCreature.ComponentHealth.HealthChange < 0f)
				{
					this.m_importanceLevel = (float)((this.m_componentCreature.ComponentHealth.Health < 0.33f) ? 300 : 100);
				}
				else if (this.m_attacker != null && Vector3.DistanceSquared(this.m_attacker.Position, this.m_componentCreature.ComponentBody.Position) < 25f)
				{
					this.m_importanceLevel = 100f;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("SwimmingAway");
				}
			}, null);
			this.m_stateMachine.AddState("SwimmingAway", delegate
			{
				this.m_componentPathfinding.SetDestination(new Vector3?(this.FindSafePlace()), 1f, 1f, 0, false, true, false, null);
			}, delegate
			{
				if (!this.IsActive || this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00087BFC File Offset: 0x00085DFC
		public virtual Vector3 FindSafePlace()
		{
			Vector3 vector = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
			Vector3? herdPosition = (this.m_componentHerdBehavior != null) ? this.m_componentHerdBehavior.FindHerdCenter() : null;
			float num = float.NegativeInfinity;
			Vector3 result = vector;
			for (int i = 0; i < 40; i++)
			{
				Vector2 vector2 = this.m_random.Vector2(1f, 1f);
				float y = 0.4f * this.m_random.Float(-1f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector2.X, y, vector2.Y));
				Vector3 vector3 = vector + this.m_random.Float(10f, 20f) * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, vector3, false, false, delegate(int value, float d)
				{
					int num3 = Terrain.ExtractContents(value);
					return !(BlocksManager.Blocks[num3] is WaterBlock);
				});
				Vector3 vector4 = (terrainRaycastResult != null) ? (vector + v * terrainRaycastResult.Value.Distance) : vector3;
				float num2 = this.ScoreSafePlace(vector, vector4, herdPosition);
				if (num2 > num)
				{
					num = num2;
					result = vector4;
				}
			}
			return result;
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00087D68 File Offset: 0x00085F68
		public virtual float ScoreSafePlace(Vector3 currentPosition, Vector3 safePosition, Vector3? herdPosition)
		{
			Vector2 vector = new Vector2(currentPosition.X, currentPosition.Z);
			Vector2 vector2 = new Vector2(safePosition.X, safePosition.Z);
			Vector2? vector3 = (herdPosition != null) ? new Vector2?(new Vector2(herdPosition.Value.X, herdPosition.Value.Z)) : null;
			Segment2 s = new Segment2(vector, vector2);
			float num = (vector3 != null) ? Segment2.Distance(s, vector3.Value) : 0f;
			if (this.m_attacker != null)
			{
				Vector3 position = this.m_attacker.Position;
				Vector2 vector4 = new Vector2(position.X, position.Z);
				float num2 = Vector2.Distance(vector4, vector2);
				float num3 = Segment2.Distance(s, vector4);
				return num2 + 1.5f * num3 - num;
			}
			return 1.5f * Vector2.Distance(vector, vector2) - num;
		}

		// Token: 0x04000B41 RID: 2881
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000B42 RID: 2882
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000B43 RID: 2883
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B44 RID: 2884
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B45 RID: 2885
		public ComponentHerdBehavior m_componentHerdBehavior;

		// Token: 0x04000B46 RID: 2886
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B47 RID: 2887
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000B48 RID: 2888
		public float m_importanceLevel;

		// Token: 0x04000B49 RID: 2889
		public ComponentFrame m_attacker;

		// Token: 0x04000B4A RID: 2890
		public float m_timeToForgetAttacker;
	}
}
