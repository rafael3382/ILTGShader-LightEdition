using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200022A RID: 554
	public class ComponentRunAwayBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x0008389B File Offset: 0x00081A9B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060011B7 RID: 4535 RVA: 0x0008389E File Offset: 0x00081A9E
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x000838A6 File Offset: 0x00081AA6
		public virtual void RunAwayFrom(ComponentBody componentBody)
		{
			this.m_attacker = componentBody;
			this.m_timeToForgetAttacker = this.m_random.Float(10f, 20f);
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000838CA File Offset: 0x00081ACA
		public void Update(float dt)
		{
			this.m_stateMachine.Update();
			this.m_heardNoise = false;
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000838DE File Offset: 0x00081ADE
		public virtual void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 1f)
			{
				this.m_heardNoise = true;
				this.m_lastNoiseSourcePosition = new Vector3?(sourcePosition);
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000838FC File Offset: 0x00081AFC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_componentHerdBehavior = base.Entity.FindComponent<ComponentHerdBehavior>();
			ComponentHealth componentHealth = this.m_componentCreature.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature attacker)
			{
				this.RunAwayFrom(attacker.ComponentBody);
			}));
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_lastNoiseSourcePosition = null;
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
				if (this.m_componentCreature.ComponentHealth.HealthChange < 0f || (this.m_attacker != null && Vector3.DistanceSquared(this.m_attacker.Position, this.m_componentCreature.ComponentBody.Position) < 36f))
				{
					this.m_importanceLevel = MathUtils.Max(this.m_importanceLevel, (float)((this.m_componentCreature.ComponentHealth.Health < 0.33f) ? 300 : 100));
				}
				else if (this.m_heardNoise)
				{
					this.m_importanceLevel = MathUtils.Max(this.m_importanceLevel, 5f);
				}
				else if (!this.IsActive)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("RunningAway");
				}
			}, null);
			this.m_stateMachine.AddState("RunningAway", delegate
			{
				Vector3 value = this.FindSafePlace();
				this.m_componentPathfinding.SetDestination(new Vector3?(value), 1f, 1f, 0, false, true, false, null);
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 6f);
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
					return;
				}
				if (this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
					return;
				}
				if (this.m_attacker != null)
				{
					if (!this.m_attacker.IsAddedToProject)
					{
						this.m_importanceLevel = 0f;
						this.m_attacker = null;
						return;
					}
					ComponentHealth componentHealth2 = this.m_attacker.Entity.FindComponent<ComponentHealth>();
					if (componentHealth2 != null && componentHealth2.Health == 0f)
					{
						this.m_importanceLevel = 0f;
						this.m_attacker = null;
					}
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00083A04 File Offset: 0x00081C04
		public virtual Vector3 FindSafePlace()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3? herdPosition = (this.m_componentHerdBehavior != null) ? this.m_componentHerdBehavior.FindHerdCenter() : null;
			if (herdPosition != null && Vector3.DistanceSquared(position, herdPosition.Value) < 144f)
			{
				herdPosition = null;
			}
			float num = float.NegativeInfinity;
			Vector3 result = position;
			for (int i = 0; i < 30; i++)
			{
				int num2 = Terrain.ToCell(position.X + this.m_random.Float(-25f, 25f));
				int num3 = Terrain.ToCell(position.Z + this.m_random.Float(-25f, 25f));
				int j = 255;
				while (j >= 0)
				{
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(num2, j, num3);
					if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsCollidable_(cellValue) || Terrain.ExtractContents(cellValue) == 18)
					{
						Vector3 vector = new Vector3((float)num2 + 0.5f, (float)j + 1.1f, (float)num3 + 0.5f);
						float num4 = this.ScoreSafePlace(position, vector, herdPosition, this.m_lastNoiseSourcePosition, Terrain.ExtractContents(cellValue));
						if (num4 > num)
						{
							num = num4;
							result = vector;
							break;
						}
						break;
					}
					else
					{
						j--;
					}
				}
			}
			return result;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00083B68 File Offset: 0x00081D68
		public virtual float ScoreSafePlace(Vector3 currentPosition, Vector3 safePosition, Vector3? herdPosition, Vector3? noiseSourcePosition, int contents)
		{
			float num = 0f;
			Vector2 vector = new Vector2(currentPosition.X, currentPosition.Z);
			Vector2 vector2 = new Vector2(safePosition.X, safePosition.Z);
			Segment2 s = new Segment2(vector, vector2);
			if (this.m_attacker != null)
			{
				Vector3 position = this.m_attacker.Position;
				Vector2 vector3 = new Vector2(position.X, position.Z);
				float num2 = Vector2.Distance(vector3, vector2);
				float num3 = Segment2.Distance(s, vector3);
				num += num2 + 3f * num3;
			}
			else
			{
				num += 2f * Vector2.Distance(vector, vector2);
			}
			Vector2? vector4 = (herdPosition != null) ? new Vector2?(new Vector2(herdPosition.Value.X, herdPosition.Value.Z)) : null;
			float num4 = (vector4 != null) ? Segment2.Distance(s, vector4.Value) : 0f;
			num -= num4;
			Vector2? vector5 = (noiseSourcePosition != null) ? new Vector2?(new Vector2(noiseSourcePosition.Value.X, noiseSourcePosition.Value.Z)) : null;
			float num5 = (vector5 != null) ? Segment2.Distance(s, vector5.Value) : 0f;
			num += 1.5f * num5;
			if (contents == 18)
			{
				num -= 4f;
			}
			return num;
		}

		// Token: 0x04000AAF RID: 2735
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AB0 RID: 2736
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000AB1 RID: 2737
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x04000AB2 RID: 2738
		public ComponentCreature m_componentCreature;

		// Token: 0x04000AB3 RID: 2739
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000AB4 RID: 2740
		public ComponentHerdBehavior m_componentHerdBehavior;

		// Token: 0x04000AB5 RID: 2741
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000AB6 RID: 2742
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000AB7 RID: 2743
		public float m_importanceLevel;

		// Token: 0x04000AB8 RID: 2744
		public ComponentFrame m_attacker;

		// Token: 0x04000AB9 RID: 2745
		public float m_timeToForgetAttacker;

		// Token: 0x04000ABA RID: 2746
		public bool m_heardNoise;

		// Token: 0x04000ABB RID: 2747
		public Vector3? m_lastNoiseSourcePosition;
	}
}
