using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000204 RID: 516
	public class ComponentFishOutOfWaterBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0006F0DA File Offset: 0x0006D2DA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0006F0DD File Offset: 0x0006D2DD
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0006F0E5 File Offset: 0x0006D2E5
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0006F0F4 File Offset: 0x0006D2F4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentFishModel = base.Entity.FindComponent<ComponentFishModel>(true);
			this.m_stateMachine.AddState("Inactive", null, delegate
			{
				if (this.IsOutOfWater())
				{
					this.m_outOfWaterTime += this.m_subsystemTime.GameTimeDelta;
				}
				else
				{
					this.m_outOfWaterTime = 0f;
				}
				if (this.m_outOfWaterTime > 3f)
				{
					this.m_importanceLevel = 1000f;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Jump");
				}
			}, null);
			this.m_stateMachine.AddState("Jump", null, delegate
			{
				this.m_componentFishModel.BendOrder = new float?(2f * (2f * MathUtils.Saturate(SimplexNoise.OctavedNoise((float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0), 1.2f * this.m_componentCreature.ComponentLocomotion.TurnSpeed, 1, 1f, 1f, false)) - 1f));
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (!this.IsOutOfWater())
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_random.Float(0f, 1f) < 2.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentLocomotion.JumpOrder = this.m_random.Float(0.33f, 1f);
					this.m_direction = new Vector2(MathUtils.Sign(this.m_componentFishModel.BendOrder.Value), 0f);
				}
				if (this.m_componentCreature.ComponentBody.StandingOnValue == null)
				{
					this.m_componentCreature.ComponentLocomotion.TurnOrder = new Vector2(0f - this.m_componentFishModel.BendOrder.Value, 0f);
					this.m_componentCreature.ComponentLocomotion.WalkOrder = new Vector2?(this.m_direction);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0006F195 File Offset: 0x0006D395
		public virtual bool IsOutOfWater()
		{
			return this.m_componentCreature.ComponentBody.ImmersionFactor < 0.33f;
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0006F1B0 File Offset: 0x0006D3B0
		public virtual Vector3? FindDestination()
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector = this.m_random.Vector2(1f, 1f);
				float y = 0.2f * this.m_random.Float(-0.8f, 1f);
				Vector3 v = Vector3.Normalize(new Vector3(vector.X, y, vector.Y));
				Vector3 vector2 = this.m_componentCreature.ComponentBody.Position + this.m_random.Float(8f, 16f) * v;
				TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(this.m_componentCreature.ComponentBody.Position, vector2, false, false, delegate(int value, float d)
				{
					int num = Terrain.ExtractContents(value);
					return !(BlocksManager.Blocks[num] is WaterBlock);
				});
				if (terrainRaycastResult == null)
				{
					return new Vector3?(vector2);
				}
				if (terrainRaycastResult.Value.Distance > 4f)
				{
					return new Vector3?(this.m_componentCreature.ComponentBody.Position + v * terrainRaycastResult.Value.Distance);
				}
			}
			return null;
		}

		// Token: 0x0400087A RID: 2170
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400087B RID: 2171
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400087C RID: 2172
		public ComponentCreature m_componentCreature;

		// Token: 0x0400087D RID: 2173
		public ComponentFishModel m_componentFishModel;

		// Token: 0x0400087E RID: 2174
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400087F RID: 2175
		public float m_importanceLevel;

		// Token: 0x04000880 RID: 2176
		public float m_outOfWaterTime;

		// Token: 0x04000881 RID: 2177
		public Vector2 m_direction;

		// Token: 0x04000882 RID: 2178
		public Game.Random m_random = new Game.Random();
	}
}
