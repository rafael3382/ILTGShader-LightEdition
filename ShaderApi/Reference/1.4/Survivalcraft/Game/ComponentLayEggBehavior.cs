using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000218 RID: 536
	public class ComponentLayEggBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06001042 RID: 4162 RVA: 0x00079D3F File Offset: 0x00077F3F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x00079D42 File Offset: 0x00077F42
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00079D4C File Offset: 0x00077F4C
		public void Update(float dt)
		{
			if (string.IsNullOrEmpty(this.m_stateMachine.CurrentState))
			{
				this.m_stateMachine.TransitionTo("Move");
			}
			if (this.m_eggType != null && this.m_random.Float(0f, 1f) < this.m_layFrequency * dt)
			{
				this.m_importanceLevel = this.m_random.Float(1f, 2f);
			}
			this.m_dt = dt;
			if (this.IsActive)
			{
				this.m_stateMachine.Update();
				return;
			}
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00079DE8 File Offset: 0x00077FE8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			EggBlock eggBlock = (EggBlock)BlocksManager.Blocks[118];
			this.m_layFrequency = valuesDictionary.GetValue<float>("LayFrequency");
			this.m_eggType = eggBlock.GetEggTypeByCreatureTemplateName(base.Entity.ValuesDictionary.DatabaseObject.Name);
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
				float num = 5f;
				Vector3 vector = position + new Vector3(num * this.m_random.Float(-1f, 1f), 0f, num * this.m_random.Float(-1f, 1f));
				vector.Y = (float)(this.m_subsystemTerrain.Terrain.GetTopHeight(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Z)) + 1);
				this.m_componentPathfinding.SetDestination(new Vector3?(vector), this.m_random.Float(0.4f, 0.6f), 0.5f, 0, false, true, false, null);
			}, delegate
			{
				if (this.m_componentPathfinding.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Lay");
					return;
				}
				if (this.m_componentPathfinding.IsStuck)
				{
					if (this.m_random.Float(0f, 1f) < 0.5f)
					{
						this.m_stateMachine.TransitionTo("Stuck");
						return;
					}
					this.m_importanceLevel = 0f;
				}
			}, null);
			this.m_stateMachine.AddState("Lay", delegate
			{
				this.m_layTime = 0f;
			}, delegate
			{
				if (this.m_eggType != null)
				{
					this.m_layTime += this.m_dt;
					if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
					{
						this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(0f, 0.25f * (float)MathUtils.Sin(20.0 * this.m_subsystemTime.GameTime) + this.m_layTime / 3f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
						if (this.m_layTime >= 3f)
						{
							this.m_importanceLevel = 0f;
							int value = Terrain.MakeBlockValue(118, 0, EggBlock.SetIsLaid(EggBlock.SetEggType(0, this.m_eggType.EggTypeIndex), true));
							Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
							Vector3 position = 0.5f * (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max);
							Vector3 value2 = 3f * Vector3.Normalize(-matrix.Forward + 0.1f * matrix.Up + 0.2f * this.m_random.Float(-1f, 1f) * matrix.Right);
							this.m_subsystemPickables.AddPickable(value, 1, position, new Vector3?(value2), null);
							this.m_subsystemAudio.PlaySound("Audio/EggLaid", 1f, this.m_random.Float(-0.1f, 0.1f), position, 2f, true);
							return;
						}
					}
					else if (this.m_layTime >= 3f)
					{
						this.m_importanceLevel = 0f;
						return;
					}
				}
				else
				{
					this.m_importanceLevel = 0f;
				}
			}, null);
		}

		// Token: 0x0400099C RID: 2460
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400099D RID: 2461
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400099E RID: 2462
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400099F RID: 2463
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040009A0 RID: 2464
		public ComponentCreature m_componentCreature;

		// Token: 0x040009A1 RID: 2465
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040009A2 RID: 2466
		public EggBlock.EggType m_eggType;

		// Token: 0x040009A3 RID: 2467
		public float m_layFrequency;

		// Token: 0x040009A4 RID: 2468
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040009A5 RID: 2469
		public float m_importanceLevel;

		// Token: 0x040009A6 RID: 2470
		public float m_dt;

		// Token: 0x040009A7 RID: 2471
		public float m_layTime;

		// Token: 0x040009A8 RID: 2472
		public Game.Random m_random = new Game.Random();
	}
}
