using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000224 RID: 548
	public class ComponentPathfinding : Component, IUpdateable
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0007FD1F File Offset: 0x0007DF1F
		// (set) Token: 0x06001128 RID: 4392 RVA: 0x0007FD27 File Offset: 0x0007DF27
		public Vector3? Destination { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06001129 RID: 4393 RVA: 0x0007FD30 File Offset: 0x0007DF30
		// (set) Token: 0x0600112A RID: 4394 RVA: 0x0007FD38 File Offset: 0x0007DF38
		public float Range { get; set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600112B RID: 4395 RVA: 0x0007FD41 File Offset: 0x0007DF41
		// (set) Token: 0x0600112C RID: 4396 RVA: 0x0007FD49 File Offset: 0x0007DF49
		public float Speed { get; set; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600112D RID: 4397 RVA: 0x0007FD52 File Offset: 0x0007DF52
		// (set) Token: 0x0600112E RID: 4398 RVA: 0x0007FD5A File Offset: 0x0007DF5A
		public int MaxPathfindingPositions { get; set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600112F RID: 4399 RVA: 0x0007FD63 File Offset: 0x0007DF63
		// (set) Token: 0x06001130 RID: 4400 RVA: 0x0007FD6B File Offset: 0x0007DF6B
		public bool UseRandomMovements { get; set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06001131 RID: 4401 RVA: 0x0007FD74 File Offset: 0x0007DF74
		// (set) Token: 0x06001132 RID: 4402 RVA: 0x0007FD7C File Offset: 0x0007DF7C
		public bool IgnoreHeightDifference { get; set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x0007FD85 File Offset: 0x0007DF85
		// (set) Token: 0x06001134 RID: 4404 RVA: 0x0007FD8D File Offset: 0x0007DF8D
		public bool RaycastDestination { get; set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001135 RID: 4405 RVA: 0x0007FD96 File Offset: 0x0007DF96
		// (set) Token: 0x06001136 RID: 4406 RVA: 0x0007FD9E File Offset: 0x0007DF9E
		public ComponentBody DoNotAvoidBody { get; set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06001137 RID: 4407 RVA: 0x0007FDA7 File Offset: 0x0007DFA7
		// (set) Token: 0x06001138 RID: 4408 RVA: 0x0007FDAF File Offset: 0x0007DFAF
		public bool IsStuck { get; set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x0007FDB8 File Offset: 0x0007DFB8
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0007FDBC File Offset: 0x0007DFBC
		public virtual void SetDestination(Vector3? destination, float speed, float range, int maxPathfindingPositions, bool useRandomMovements, bool ignoreHeightDifference, bool raycastDestination, ComponentBody doNotAvoidBody)
		{
			this.Destination = destination;
			this.Speed = speed;
			this.Range = range;
			this.MaxPathfindingPositions = maxPathfindingPositions;
			this.UseRandomMovements = useRandomMovements;
			this.IgnoreHeightDifference = ignoreHeightDifference;
			this.RaycastDestination = raycastDestination;
			this.DoNotAvoidBody = doNotAvoidBody;
			this.m_destinationChanged = true;
			this.m_nextUpdateTime = 0.0;
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0007FE1C File Offset: 0x0007E01C
		public virtual void Stop()
		{
			this.SetDestination(null, 0f, 0f, 0, false, false, false, null);
			this.m_componentPilot.Stop();
			this.IsStuck = false;
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x0007FE5C File Offset: 0x0007E05C
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				float num = this.m_random.Float(0.08f, 0.12f);
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)num;
				this.m_pathfindingCongestion = MathUtils.Max(this.m_pathfindingCongestion - 20f * num, 0f);
				this.m_stateMachine.Update();
			}
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x0007FED0 File Offset: 0x0007E0D0
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemPathfinding = base.Project.FindSubsystem<SubsystemPathfinding>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPilot = base.Entity.FindComponent<ComponentPilot>(true);
			this.m_stateMachine.AddState("Stopped", delegate
			{
				this.Stop();
				this.m_randomMoveCount = 0;
			}, delegate
			{
				if (this.Destination != null)
				{
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.AddState("MovingDirect", delegate
			{
				this.IsStuck = false;
				this.m_destinationChanged = true;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_destinationChanged)
				{
					this.m_componentPilot.SetDestination(this.Destination, this.Speed, this.Range, this.IgnoreHeightDifference, this.RaycastDestination, this.Speed >= 1f, this.DoNotAvoidBody);
					this.m_destinationChanged = false;
					return;
				}
				if (this.m_componentPilot.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_componentPilot.IsStuck)
				{
					if (this.MaxPathfindingPositions > 0 && this.m_componentCreature.ComponentLocomotion.WalkSpeed > 0f)
					{
						this.m_stateMachine.TransitionTo("SearchingForPath");
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
			this.m_stateMachine.AddState("SearchingForPath", delegate
			{
				this.m_pathfindingResult.IsCompleted = false;
				this.m_pathfindingResult.IsInProgress = false;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
				}
				else
				{
					if (!this.m_pathfindingResult.IsInProgress)
					{
						if (this.m_lastPathfindingTime != null)
						{
							double? num = this.m_subsystemTime.GameTime - this.m_lastPathfindingTime;
							double num2 = 8.0;
							if (!(num.GetValueOrDefault() > num2 & num != null))
							{
								goto IL_158;
							}
						}
						if (this.m_pathfindingCongestion < 500f)
						{
							this.m_lastPathfindingDestination = new Vector3?(this.Destination.Value);
							this.m_lastPathfindingTime = new double?(this.m_subsystemTime.GameTime);
							this.m_subsystemPathfinding.QueuePathSearch(this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0.01f, 0f), this.Destination.Value + new Vector3(0f, 0.01f, 0f), 1f, this.m_componentCreature.ComponentBody.BoxSize, this.MaxPathfindingPositions, this.m_pathfindingResult);
							goto IL_170;
						}
					}
					IL_158:
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
					}
				}
				IL_170:
				if (this.m_pathfindingResult.IsCompleted)
				{
					this.m_pathfindingCongestion = MathUtils.Min(this.m_pathfindingCongestion + (float)this.m_pathfindingResult.PositionsChecked, 1000f);
					if (this.m_pathfindingResult.Path.Count > 0)
					{
						this.m_stateMachine.TransitionTo("MovingWithPath");
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
				}
			}, null);
			this.m_stateMachine.AddState("MovingWithPath", delegate
			{
				this.m_componentPilot.Stop();
				this.m_randomMoveCount = 0;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_componentPilot.Destination == null)
				{
					if (this.m_pathfindingResult.Path.Count > 0)
					{
						Vector3 value = this.m_pathfindingResult.Path.Array[this.m_pathfindingResult.Path.Count - 1];
						this.m_componentPilot.SetDestination(new Vector3?(value), MathUtils.Min(this.Speed, 0.75f), 0.75f, false, false, this.Speed >= 1f, this.DoNotAvoidBody);
						this.m_pathfindingResult.Path.RemoveAt(this.m_pathfindingResult.Path.Count - 1);
						return;
					}
					this.m_stateMachine.TransitionTo("MovingDirect");
					return;
				}
				else
				{
					if (!this.m_componentPilot.IsStuck)
					{
						float num = Vector3.DistanceSquared(this.m_componentCreature.ComponentBody.Position, this.Destination.Value);
						if (Vector3.DistanceSquared(this.m_lastPathfindingDestination.Value, this.Destination.Value) > num)
						{
							this.m_stateMachine.TransitionTo("MovingDirect");
						}
						return;
					}
					if (this.UseRandomMovements)
					{
						this.m_stateMachine.TransitionTo("MovingRandomly");
						return;
					}
					this.m_stateMachine.TransitionTo("Stuck");
					return;
				}
			}, null);
			this.m_stateMachine.AddState("MovingRandomly", delegate
			{
				this.m_componentPilot.SetDestination(new Vector3?(this.m_componentCreature.ComponentBody.Position + new Vector3(5f * this.m_random.Float(-1f, 1f), 0f, 5f * this.m_random.Float(-1f, 1f))), 1f, 1f, true, false, false, this.DoNotAvoidBody);
				this.m_randomMoveCount++;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_randomMoveCount > 3)
				{
					this.m_stateMachine.TransitionTo("Stuck");
					return;
				}
				if (this.m_componentPilot.IsStuck || this.m_componentPilot.Destination == null)
				{
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.AddState("Stuck", delegate
			{
				this.IsStuck = true;
			}, delegate
			{
				if (this.Destination == null)
				{
					this.m_stateMachine.TransitionTo("Stopped");
					return;
				}
				if (this.m_destinationChanged)
				{
					this.m_destinationChanged = false;
					this.m_stateMachine.TransitionTo("MovingDirect");
				}
			}, null);
			this.m_stateMachine.TransitionTo("Stopped");
		}

		// Token: 0x04000A3F RID: 2623
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A40 RID: 2624
		public SubsystemPathfinding m_subsystemPathfinding;

		// Token: 0x04000A41 RID: 2625
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A42 RID: 2626
		public ComponentPilot m_componentPilot;

		// Token: 0x04000A43 RID: 2627
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000A44 RID: 2628
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A45 RID: 2629
		public Vector3? m_lastPathfindingDestination;

		// Token: 0x04000A46 RID: 2630
		public double? m_lastPathfindingTime;

		// Token: 0x04000A47 RID: 2631
		public float m_pathfindingCongestion;

		// Token: 0x04000A48 RID: 2632
		public PathfindingResult m_pathfindingResult = new PathfindingResult();

		// Token: 0x04000A49 RID: 2633
		public double m_nextUpdateTime;

		// Token: 0x04000A4A RID: 2634
		public int m_randomMoveCount;

		// Token: 0x04000A4B RID: 2635
		public bool m_destinationChanged;

		// Token: 0x04000A4C RID: 2636
		public const float m_minPathfindingPeriod = 8f;

		// Token: 0x04000A4D RID: 2637
		public const float m_pathfindingCongestionCapacity = 500f;

		// Token: 0x04000A4E RID: 2638
		public const float m_pathfindingCongestionCapacityLimit = 1000f;

		// Token: 0x04000A4F RID: 2639
		public const float m_pathfindingCongestionDecayRate = 20f;

		// Token: 0x04000A50 RID: 2640
		public static bool DrawPathfinding;
	}
}
