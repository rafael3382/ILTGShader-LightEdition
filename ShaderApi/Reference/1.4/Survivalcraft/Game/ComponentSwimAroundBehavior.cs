using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000235 RID: 565
	public class ComponentSwimAroundBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x00087700 File Offset: 0x00085900
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001250 RID: 4688 RVA: 0x00087703 File Offset: 0x00085903
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0008770C File Offset: 0x0008590C
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Inactive");
			}
			if (this.m_random.Float(0f, 1f) < 0.05f * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 3f);
			}
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00087798 File Offset: 0x00085998
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Swim");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f)
				{
					this.m_importanceLevel = 1f;
				}
				this.m_stateMachine.TransitionTo("Swim");
			}, null, null);
			this.m_stateMachine.AddState("Swim", delegate
			{
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				if (this.m_componentPathfinding.Destination != null)
				{
					if (this.m_componentPathfinding.IsStuck)
					{
						this.m_stateMachine.TransitionTo("Stuck");
					}
					return;
				}
				Vector3? destination = this.FindDestination();
				if (destination != null)
				{
					this.m_componentPathfinding.SetDestination(destination, this.m_random.Float(0.3f, 0.4f), 1f, 0, false, true, false, null);
					return;
				}
				this.m_importanceLevel = 1f;
			}, null);
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00087840 File Offset: 0x00085A40
		public virtual Vector3? FindDestination()
		{
			Vector3 vector = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
			float num = 2f;
			Vector3? result = null;
			float num2 = this.m_random.Float(10f, 16f);
			for (int i = 0; i < 16; i++)
			{
				Vector2 vector2 = this.m_random.Vector2(1f, 1f);
				float y = 0.3f * this.m_random.Float(-0.9f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector2.X, y, vector2.Y));
				Vector3 vector3 = vector + num2 * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, vector3, false, false, delegate(int value, float d)
				{
					int num3 = Terrain.ExtractContents(value);
					return !(BlocksManager.Blocks[num3] is WaterBlock);
				});
				if (terrainRaycastResult == null)
				{
					if (num2 > num)
					{
						result = new Vector3?(vector3);
						num = num2;
					}
				}
				else if (terrainRaycastResult.Value.Distance > num)
				{
					result = new Vector3?(vector + v * terrainRaycastResult.Value.Distance);
					num = terrainRaycastResult.Value.Distance;
				}
			}
			return result;
		}

		// Token: 0x04000B3B RID: 2875
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000B3C RID: 2876
		public ComponentCreature m_componentCreature;

		// Token: 0x04000B3D RID: 2877
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000B3E RID: 2878
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000B3F RID: 2879
		public float m_importanceLevel = 1f;

		// Token: 0x04000B40 RID: 2880
		public Game.Random m_random = new Game.Random();
	}
}
