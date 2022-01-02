using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000208 RID: 520
	public class ComponentFlyAwayBehavior : ComponentBehavior, IUpdateable, INoiseListener
	{
		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000F3A RID: 3898 RVA: 0x00070A83 File Offset: 0x0006EC83
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000F3B RID: 3899 RVA: 0x00070A86 File Offset: 0x0006EC86
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x170001CD RID: 461
		// (set) Token: 0x06000F3C RID: 3900 RVA: 0x00070A8E File Offset: 0x0006EC8E
		public override bool IsActive
		{
			set
			{
				base.IsActive = value;
				if (this.IsActive)
				{
					this.m_nextUpdateTime = 0.0;
				}
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00070AB0 File Offset: 0x0006ECB0
		public void Update(float dt)
		{
			if (this.m_componentCreature.ComponentHealth.HealthChange < 0f)
			{
				this.m_stateMachine.TransitionTo("DangerDetected");
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.5f, 1f);
				this.m_stateMachine.Update();
			}
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00070B2A File Offset: 0x0006ED2A
		public virtual void HearNoise(ComponentBody sourceBody, Vector3 sourcePosition, float loudness)
		{
			if (loudness >= 0.25f && this.m_stateMachine.CurrentState != "RunningAway")
			{
				this.m_stateMachine.TransitionTo("DangerDetected");
			}
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x00070B5C File Offset: 0x0006ED5C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			ComponentBody componentBody = this.m_componentCreature.ComponentBody;
			componentBody.CollidedWithBody = (Action<ComponentBody>)Delegate.Combine(componentBody.CollidedWithBody, new Action<ComponentBody>(delegate(ComponentBody <p0>)
			{
				if (this.m_stateMachine.CurrentState != "RunningAway")
				{
					this.m_stateMachine.TransitionTo("DangerDetected");
				}
			}));
			this.m_stateMachine.AddState("LookingForDanger", null, delegate
			{
				if (this.ScanForDanger())
				{
					this.m_stateMachine.TransitionTo("DangerDetected");
				}
			}, null);
			this.m_stateMachine.AddState("DangerDetected", delegate
			{
				this.m_importanceLevel = (float)((this.m_componentCreature.ComponentHealth.Health < 0.33f) ? 300 : 100);
				this.m_nextUpdateTime = 0.0;
			}, delegate
			{
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("RunningAway");
					this.m_nextUpdateTime = 0.0;
				}
			}, null);
			this.m_stateMachine.AddState("RunningAway", delegate
			{
				this.m_componentPathfinding.SetDestination(new Vector3?(this.FindSafePlace()), 1f, 1f, 0, false, true, false, null);
				this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/Wings", 0.8f, this.m_random.Float(-0.1f, 0.2f), this.m_componentCreature.ComponentBody.Position, 3f, true);
				this.m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 6f);
			}, delegate
			{
				if (!this.IsActive || this.m_componentPathfinding.Destination == null || this.m_componentPathfinding.IsStuck)
				{
					this.m_stateMachine.TransitionTo("LookingForDanger");
					return;
				}
				if (this.ScoreSafePlace(this.m_componentCreature.ComponentBody.Position, this.m_componentPathfinding.Destination.Value, null) < 4f)
				{
					this.m_componentPathfinding.SetDestination(new Vector3?(this.FindSafePlace()), 1f, 0.5f, 0, false, true, false, null);
				}
			}, delegate
			{
				this.m_importanceLevel = 0f;
			});
			this.m_stateMachine.TransitionTo("LookingForDanger");
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x00070CA0 File Offset: 0x0006EEA0
		public virtual bool ScanForDanger()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			Vector3 translation = matrix.Translation;
			Vector3 forward = matrix.Forward;
			return this.ScoreSafePlace(translation, translation, new Vector3?(forward)) < 7f;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00070CE8 File Offset: 0x0006EEE8
		public virtual Vector3 FindSafePlace()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			float num = float.NegativeInfinity;
			Vector3 result = position;
			for (int i = 0; i < 20; i++)
			{
				int num2 = Terrain.ToCell(position.X + this.m_random.Float(-20f, 20f));
				int num3 = Terrain.ToCell(position.Z + this.m_random.Float(-20f, 20f));
				int j = 255;
				while (j >= 0)
				{
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(num2, j, num3);
					if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsCollidable_(cellValue) || Terrain.ExtractContents(cellValue) == 18)
					{
						Vector3 vector = new Vector3((float)num2 + 0.5f, (float)j + 1.1f, (float)num3 + 0.5f);
						float num4 = this.ScoreSafePlace(position, vector, null);
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

		// Token: 0x06000F42 RID: 3906 RVA: 0x00070E00 File Offset: 0x0006F000
		public virtual float ScoreSafePlace(Vector3 currentPosition, Vector3 safePosition, Vector3? lookDirection)
		{
			float num = 16f;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), 16f, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (this.IsPredator(componentBody.Entity))
				{
					Vector3 position2 = componentBody.Position;
					Vector3 vector = safePosition - position2;
					if (lookDirection == null || 0f - Vector3.Dot(lookDirection.Value, vector) > 0f)
					{
						if (vector.Y >= 4f)
						{
							vector *= 2f;
						}
						num = MathUtils.Min(num, vector.Length());
					}
				}
			}
			float num2 = Vector3.Distance(currentPosition, safePosition);
			if (num2 < 8f)
			{
				return num * 0.5f;
			}
			return num * MathUtils.Lerp(1f, 0.75f, MathUtils.Saturate(num2 / 20f));
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x00070F28 File Offset: 0x0006F128
		public virtual bool IsPredator(Entity entity)
		{
			if (entity != base.Entity)
			{
				ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && (componentCreature.Category == CreatureCategory.LandPredator || componentCreature.Category == CreatureCategory.WaterPredator || componentCreature.Category == CreatureCategory.LandOther))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040008AA RID: 2218
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008AB RID: 2219
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040008AC RID: 2220
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040008AD RID: 2221
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008AE RID: 2222
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040008AF RID: 2223
		public ComponentCreature m_componentCreature;

		// Token: 0x040008B0 RID: 2224
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x040008B1 RID: 2225
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040008B2 RID: 2226
		public Game.Random m_random = new Game.Random();

		// Token: 0x040008B3 RID: 2227
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040008B4 RID: 2228
		public float m_importanceLevel;

		// Token: 0x040008B5 RID: 2229
		public double m_nextUpdateTime;
	}
}
