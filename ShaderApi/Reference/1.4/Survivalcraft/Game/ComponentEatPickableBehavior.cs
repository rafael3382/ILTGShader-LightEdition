using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000200 RID: 512
	public class ComponentEatPickableBehavior : ComponentBehavior, IUpdateable
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0006CC38 File Offset: 0x0006AE38
		public float Satiation
		{
			get
			{
				return this.m_satiation;
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0006CC40 File Offset: 0x0006AE40
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x0006CC43 File Offset: 0x0006AE43
		public override float ImportanceLevel
		{
			get
			{
				return this.m_importanceLevel;
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0006CC4C File Offset: 0x0006AE4C
		public void Update(float dt)
		{
			if (this.m_satiation > 0f)
			{
				this.m_satiation = MathUtils.Max(this.m_satiation - 0.01f * this.m_subsystemTime.GameTimeDelta, 0f);
			}
			this.m_stateMachine.Update();
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0006CC9C File Offset: 0x0006AE9C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPathfinding = base.Entity.FindComponent<ComponentPathfinding>(true);
			this.m_foodFactors = new float[EnumUtils.GetEnumValues(typeof(FoodType)).Max() + 1];
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("FoodFactors"))
			{
				FoodType foodType = (FoodType)Enum.Parse(typeof(FoodType), keyValuePair.Key, false);
				this.m_foodFactors[(int)foodType] = (float)keyValuePair.Value;
			}
			SubsystemPickables subsystemPickables = this.m_subsystemPickables;
			subsystemPickables.PickableAdded = (Action<Pickable>)Delegate.Combine(subsystemPickables.PickableAdded, new Action<Pickable>(delegate(Pickable pickable)
			{
				if (this.TryAddPickable(pickable) && this.m_pickable == null)
				{
					this.m_pickable = pickable;
				}
			}));
			SubsystemPickables subsystemPickables2 = this.m_subsystemPickables;
			subsystemPickables2.PickableRemoved = (Action<Pickable>)Delegate.Combine(subsystemPickables2.PickableRemoved, new Action<Pickable>(delegate(Pickable pickable)
			{
				this.m_pickables.Remove(pickable);
				if (this.m_pickable == pickable)
				{
					this.m_pickable = null;
				}
			}));
			this.m_stateMachine.AddState("Inactive", delegate
			{
				this.m_importanceLevel = 0f;
				this.m_pickable = null;
			}, delegate
			{
				if (this.m_satiation < 1f)
				{
					if (this.m_pickable == null)
					{
						if (this.m_subsystemTime.GameTime > this.m_nextFindPickableTime)
						{
							this.m_nextFindPickableTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(2f, 4f);
							this.m_pickable = this.FindPickable(this.m_componentCreature.ComponentBody.Position);
						}
					}
					else
					{
						this.m_importanceLevel = this.m_random.Float(5f, 10f);
					}
				}
				if (this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Move");
					this.m_blockedCount = 0;
				}
			}, null);
			this.m_stateMachine.AddState("Move", delegate
			{
				if (this.m_pickable != null)
				{
					float speed = (this.m_satiation == 0f) ? this.m_random.Float(0.5f, 0.7f) : 0.5f;
					int maxPathfindingPositions = (this.m_satiation == 0f) ? 1000 : 500;
					float num = Vector3.Distance(this.m_componentCreature.ComponentCreatureModel.EyePosition, this.m_componentCreature.ComponentBody.Position);
					this.m_componentPathfinding.SetDestination(new Vector3?(this.m_pickable.Position), speed, 1f + num, maxPathfindingPositions, true, false, true, null);
					if (this.m_random.Float(0f, 1f) < 0.66f)
					{
						this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
					}
				}
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				else if (this.m_pickable == null)
				{
					this.m_importanceLevel = 0f;
				}
				else if (this.m_componentPathfinding.IsStuck)
				{
					this.m_importanceLevel = 0f;
					this.m_satiation += 0.75f;
				}
				else if (this.m_componentPathfinding.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Eat");
				}
				else if (Vector3.DistanceSquared(this.m_componentPathfinding.Destination.Value, this.m_pickable.Position) > 0.0625f)
				{
					this.m_stateMachine.TransitionTo("PickableMoved");
				}
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_pickable != null)
				{
					this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_pickable.Position);
					return;
				}
				this.m_componentCreature.ComponentCreatureModel.LookRandomOrder = true;
			}, null);
			this.m_stateMachine.AddState("PickableMoved", null, delegate
			{
				if (this.m_pickable != null)
				{
					this.m_componentCreature.ComponentCreatureModel.LookAtOrder = new Vector3?(this.m_pickable.Position);
				}
				if (this.m_subsystemTime.PeriodicGameTimeEvent(0.25, (double)(this.GetHashCode() % 100) * 0.01))
				{
					this.m_stateMachine.TransitionTo("Move");
				}
			}, null);
			this.m_stateMachine.AddState("Eat", delegate
			{
				this.m_eatTime = (double)this.m_random.Float(4f, 5f);
				this.m_blockedTime = 0f;
			}, delegate
			{
				if (!this.IsActive)
				{
					this.m_stateMachine.TransitionTo("Inactive");
				}
				if (this.m_pickable == null)
				{
					this.m_importanceLevel = 0f;
				}
				if (this.m_pickable != null)
				{
					if (Vector3.DistanceSquared(new Vector3(this.m_componentCreature.ComponentCreatureModel.EyePosition.X, this.m_componentCreature.ComponentBody.Position.Y, this.m_componentCreature.ComponentCreatureModel.EyePosition.Z), this.m_pickable.Position) < 0.640000045f)
					{
						this.m_eatTime -= (double)this.m_subsystemTime.GameTimeDelta;
						this.m_blockedTime = 0f;
						if (this.m_eatTime <= 0.0)
						{
							this.m_satiation += 1f;
							this.m_pickable.Count = MathUtils.Max(this.m_pickable.Count - 1, 0);
							if (this.m_pickable.Count == 0)
							{
								this.m_pickable.ToRemove = true;
								this.m_importanceLevel = 0f;
							}
							else if (this.m_random.Float(0f, 1f) < 0.5f)
							{
								this.m_importanceLevel = 0f;
							}
							ModsManager.HookAction("OnEatPickable", delegate(ModLoader modLoader)
							{
								bool result;
								modLoader.OnEatPickable(this, this.m_pickable, out result);
								return result;
							});
						}
					}
					else
					{
						float num = Vector3.Distance(this.m_componentCreature.ComponentCreatureModel.EyePosition, this.m_componentCreature.ComponentBody.Position);
						this.m_componentPathfinding.SetDestination(new Vector3?(this.m_pickable.Position), 0.3f, 0.5f + num, 0, false, true, false, null);
						this.m_blockedTime += this.m_subsystemTime.GameTimeDelta;
					}
					if (this.m_blockedTime > 3f)
					{
						this.m_blockedCount++;
						if (this.m_blockedCount >= 3)
						{
							this.m_importanceLevel = 0f;
							this.m_satiation += 0.75f;
						}
						else
						{
							this.m_stateMachine.TransitionTo("Move");
						}
					}
				}
				this.m_componentCreature.ComponentCreatureModel.FeedOrder = true;
				if (this.m_random.Float(0f, 1f) < 0.1f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayIdleSound(true);
				}
				if (this.m_random.Float(0f, 1f) < 1.5f * this.m_subsystemTime.GameTimeDelta)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				}
			}, null);
			this.m_stateMachine.TransitionTo("Inactive");
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0006CE78 File Offset: 0x0006B078
		public virtual float GetFoodFactor(FoodType foodType)
		{
			return this.m_foodFactors[(int)foodType];
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0006CE84 File Offset: 0x0006B084
		public virtual Pickable FindPickable(Vector3 position)
		{
			if (this.m_subsystemTime.GameTime > this.m_nextPickablesUpdateTime)
			{
				this.m_nextPickablesUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(2f, 4f);
				this.m_pickables.Clear();
				foreach (Pickable pickable in this.m_subsystemPickables.Pickables)
				{
					this.TryAddPickable(pickable);
				}
				if (this.m_pickable != null && !this.m_pickables.ContainsKey(this.m_pickable))
				{
					this.m_pickable = null;
				}
			}
			foreach (Pickable pickable2 in this.m_pickables.Keys)
			{
				float num = Vector3.DistanceSquared(position, pickable2.Position);
				if (this.m_random.Float(0f, 1f) > num / 256f)
				{
					return pickable2;
				}
			}
			return null;
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0006CFC4 File Offset: 0x0006B1C4
		public virtual bool TryAddPickable(Pickable pickable)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(pickable.Value)];
			if (this.m_foodFactors[(int)block.GetFoodType(pickable.Value)] > 0f && Vector3.DistanceSquared(pickable.Position, this.m_componentCreature.ComponentBody.Position) < 256f)
			{
				this.m_pickables.Add(pickable, true);
				return true;
			}
			return false;
		}

		// Token: 0x04000834 RID: 2100
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000835 RID: 2101
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000836 RID: 2102
		public ComponentCreature m_componentCreature;

		// Token: 0x04000837 RID: 2103
		public ComponentPathfinding m_componentPathfinding;

		// Token: 0x04000838 RID: 2104
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000839 RID: 2105
		public Dictionary<Pickable, bool> m_pickables = new Dictionary<Pickable, bool>();

		// Token: 0x0400083A RID: 2106
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400083B RID: 2107
		public float[] m_foodFactors;

		// Token: 0x0400083C RID: 2108
		public float m_importanceLevel;

		// Token: 0x0400083D RID: 2109
		public double m_nextFindPickableTime;

		// Token: 0x0400083E RID: 2110
		public double m_nextPickablesUpdateTime;

		// Token: 0x0400083F RID: 2111
		public Pickable m_pickable;

		// Token: 0x04000840 RID: 2112
		public double m_eatTime;

		// Token: 0x04000841 RID: 2113
		public float m_satiation;

		// Token: 0x04000842 RID: 2114
		public float m_blockedTime;

		// Token: 0x04000843 RID: 2115
		public int m_blockedCount;

		// Token: 0x04000844 RID: 2116
		public const float m_range = 16f;
	}
}
