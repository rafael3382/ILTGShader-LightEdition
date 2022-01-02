using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FD RID: 509
	public class ComponentDigInMudBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0006BEE8 File Offset: 0x0006A0E8
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000ECA RID: 3786 RVA: 0x0006BEEB File Offset: 0x0006A0EB
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0006BEF3 File Offset: 0x0006A0F3
		public virtual void Update(float dt)
		{
			this.m_stateMachine.Update();
			this.m_collidedWithBody = null;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0006BF08 File Offset: 0x0006A108
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_componentFishModel = base.Entity.FindComponent<ComponentFishModel>(true);
			this.m_componentSwimAwayBehavior = base.Entity.FindComponent<ComponentSwimAwayBehavior>(true);
			string digInBlockName = valuesDictionary.GetValue<string>("DigInBlockName");
			this.m_digInBlockIndex = ((!string.IsNullOrEmpty(digInBlockName)) ? BlocksManager.Blocks.First((Block b) => b.GetType().Name == digInBlockName).BlockIndex : 0);
			this.m_maxDigInDepth = valuesDictionary.GetValue<float>("MaxDigInDepth");
			ComponentBody componentBody = this.m_componentCreature.ComponentBody;
			componentBody.CollidedWithBody = (Action<ComponentBody>)Delegate.Combine(componentBody.CollidedWithBody, new Action<ComponentBody>(delegate(ComponentBody b)
			{
				this.m_collidedWithBody = b;
			}));
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 0.5f * this.m_subsystemTime.GameTimeDelta && this.m_subsystemTime.GameTime > this.m_digOutTime + 15.0 && this.m_digInBlockIndex != 0)
				{
					int x = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.X);
					int y = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.Y - 0.9f);
					int z = Terrain.ToCell(this.m_componentCreature.ComponentBody.Position.Z);
					if (this.m_subsystemTerrain.Terrain.GetCellContents(x, y, z) == this.m_digInBlockIndex)
					{
						this.m_importanceLevel = this.m_random.Float(1f, 3f);
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Sink");
				}
			}, null);
			this.m_stateMachine.AddState("Sink", delegate
			{
				this.m_importanceLevel = 10f;
				this.m_sinkTime = this.m_subsystemTime.GameTime;
				this.m_componentPathfinding.Stop();
			}, delegate
			{
				if (this.m_random.Float(0f, 1f) < 2f * this.m_subsystemTime.GameTimeDelta)
				{
					int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
					int digInBlockIndex = this.m_digInBlockIndex;
					if ((standingOnValue.GetValueOrDefault() == digInBlockIndex & standingOnValue != null) && this.m_componentCreature.ComponentBody.Velocity.LengthSquared() < 1f)
					{
						this.m_stateMachine.TransitionTo("DigIn");
					}
				}
				if (!this.IsActive || this.m_subsystemTime.GameTime > this.m_sinkTime + 6.0)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
			}, null);
			this.m_stateMachine.AddState("DigIn", delegate
			{
				this.m_digInTime = this.m_subsystemTime.GameTime;
				this.m_digOutTime = this.m_digInTime + (double)this.m_random.Float(30f, 60f);
			}, delegate
			{
				this.m_componentFishModel.DigInOrder = this.m_maxDigInDepth;
				if (this.m_collidedWithBody != null)
				{
					if (this.m_subsystemTime.GameTime - this.m_digInTime > 2.0 && this.m_collidedWithBody.Density < 0.95f)
					{
						this.m_componentMiner.Hit(this.m_collidedWithBody, this.m_collidedWithBody.Position, Vector3.Normalize(this.m_collidedWithBody.Position - this.m_componentCreature.ComponentBody.Position));
					}
					this.m_componentSwimAwayBehavior.SwimAwayFrom(this.m_collidedWithBody);
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.IsActive && this.m_subsystemTime.GameTime < this.m_digOutTime)
				{
					int? standingOnValue = this.m_componentCreature.ComponentBody.StandingOnValue;
					int digInBlockIndex = this.m_digInBlockIndex;
					if ((standingOnValue.GetValueOrDefault() == digInBlockIndex & standingOnValue != null) && this.m_componentCreature.ComponentBody.Velocity.LengthSquared() <= 1f)
					{
						return;
					}
				}
				this.m_stateMachine.TransitionTo("Inactive");
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0006C0AC File Offset: 0x0006A2AC
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

		// Token: 0x04000812 RID: 2066
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000813 RID: 2067
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000814 RID: 2068
		public ComponentCreature m_componentCreature;

		// Token: 0x04000815 RID: 2069
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000816 RID: 2070
		public ComponentMiner m_componentMiner;

		// Token: 0x04000817 RID: 2071
		public ComponentFishModel m_componentFishModel;

		// Token: 0x04000818 RID: 2072
		public ComponentSwimAwayBehavior m_componentSwimAwayBehavior;

		// Token: 0x04000819 RID: 2073
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x0400081A RID: 2074
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400081B RID: 2075
		public float m_importanceLevel;

		// Token: 0x0400081C RID: 2076
		public double m_sinkTime;

		// Token: 0x0400081D RID: 2077
		public double m_digInTime;

		// Token: 0x0400081E RID: 2078
		public double m_digOutTime = double.NegativeInfinity;

		// Token: 0x0400081F RID: 2079
		public float m_maxDigInDepth;

		// Token: 0x04000820 RID: 2080
		public int m_digInBlockIndex;

		// Token: 0x04000821 RID: 2081
		public ComponentBody m_collidedWithBody;
	}
}
