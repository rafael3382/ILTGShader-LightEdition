using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000225 RID: 549
	public class ComponentPilot : Component, IUpdateable
	{
		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600114B RID: 4427 RVA: 0x000806C5 File Offset: 0x0007E8C5
		// (set) Token: 0x0600114C RID: 4428 RVA: 0x000806CD File Offset: 0x0007E8CD
		public Vector3? Destination { get; set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600114D RID: 4429 RVA: 0x000806D6 File Offset: 0x0007E8D6
		// (set) Token: 0x0600114E RID: 4430 RVA: 0x000806DE File Offset: 0x0007E8DE
		public float Speed { get; set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600114F RID: 4431 RVA: 0x000806E7 File Offset: 0x0007E8E7
		// (set) Token: 0x06001150 RID: 4432 RVA: 0x000806EF File Offset: 0x0007E8EF
		public float Range { get; set; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x000806F8 File Offset: 0x0007E8F8
		// (set) Token: 0x06001152 RID: 4434 RVA: 0x00080700 File Offset: 0x0007E900
		public bool IgnoreHeightDifference { get; set; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06001153 RID: 4435 RVA: 0x00080709 File Offset: 0x0007E909
		// (set) Token: 0x06001154 RID: 4436 RVA: 0x00080711 File Offset: 0x0007E911
		public bool RaycastDestination { get; set; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06001155 RID: 4437 RVA: 0x0008071A File Offset: 0x0007E91A
		// (set) Token: 0x06001156 RID: 4438 RVA: 0x00080722 File Offset: 0x0007E922
		public bool TakeRisks { get; set; }

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06001157 RID: 4439 RVA: 0x0008072B File Offset: 0x0007E92B
		// (set) Token: 0x06001158 RID: 4440 RVA: 0x00080733 File Offset: 0x0007E933
		public ComponentBody DoNotAvoidBody { get; set; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06001159 RID: 4441 RVA: 0x0008073C File Offset: 0x0007E93C
		// (set) Token: 0x0600115A RID: 4442 RVA: 0x00080744 File Offset: 0x0007E944
		public bool IsStuck { get; set; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600115B RID: 4443 RVA: 0x0008074D File Offset: 0x0007E94D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x00080750 File Offset: 0x0007E950
		public virtual void SetDestination(Vector3? destination, float speed, float range, bool ignoreHeightDifference, bool raycastDestination, bool takeRisks, ComponentBody doNotAvoidBody)
		{
			bool flag = true;
			if (this.Destination != null && destination != null)
			{
				Vector3 v = Vector3.Normalize(this.Destination.Value - this.m_componentCreature.ComponentBody.Position);
				if (Vector3.Dot(Vector3.Normalize(destination.Value - this.m_componentCreature.ComponentBody.Position), v) > 0.5f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.IsStuck = false;
				this.m_lastStuckCheckPosition = null;
				this.m_aboveBelowTime = null;
			}
			this.Destination = destination;
			this.Speed = speed;
			this.Range = range;
			this.IgnoreHeightDifference = ignoreHeightDifference;
			this.RaycastDestination = raycastDestination;
			this.TakeRisks = takeRisks;
			this.DoNotAvoidBody = doNotAvoidBody;
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00080828 File Offset: 0x0007EA28
		public virtual void Stop()
		{
			this.SetDestination(null, 0f, 0f, false, false, false, null);
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x00080854 File Offset: 0x0007EA54
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.09f, 0.11f);
				this.m_walkOrder = null;
				this.m_flyOrder = null;
				this.m_swimOrder = null;
				this.m_turnOrder = Vector2.Zero;
				this.m_jumpOrder = 0f;
				if (this.Destination != null)
				{
					Vector3 position = this.m_componentCreature.ComponentBody.Position;
					Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
					Vector3 vector = this.AvoidNearestBody(position, this.Destination.Value);
					Vector3 vector2 = vector - position;
					float num = vector2.LengthSquared();
					Vector2 vector3 = new Vector2(vector.X, vector.Z) - new Vector2(position.X, position.Z);
					float num2 = vector3.LengthSquared();
					float x = Vector2.Angle(forward.XZ, vector2.XZ);
					float num3 = ((this.m_componentCreature.ComponentBody.CollisionVelocityChange * new Vector3(1f, 0f, 1f)).LengthSquared() > 0f && this.m_componentCreature.ComponentBody.StandingOnValue != null) ? 0.15f : 0.4f;
					if (this.m_subsystemTime.GameTime >= this.m_lastStuckCheckTime + (double)num3 || this.m_lastStuckCheckPosition == null)
					{
						this.m_lastStuckCheckTime = this.m_subsystemTime.GameTime;
						if (MathUtils.Abs(x) > MathUtils.DegToRad(20f) || this.m_lastStuckCheckPosition == null || Vector3.Dot(position - this.m_lastStuckCheckPosition.Value, Vector3.Normalize(vector2)) > 0.2f)
						{
							this.m_lastStuckCheckPosition = new Vector3?(position);
							this.m_stuckCount = 0;
						}
						else
						{
							this.m_stuckCount++;
						}
						this.IsStuck = (this.m_stuckCount >= 4);
					}
					if (this.m_componentCreature.ComponentLocomotion.FlySpeed > 0f && (num > 9f || vector2.Y > 0.5f || vector2.Y < -1.5f || (this.m_componentCreature.ComponentBody.StandingOnValue == null && this.m_componentCreature.ComponentBody.ImmersionFactor == 0f)) && this.m_componentCreature.ComponentBody.ImmersionFactor < 1f)
					{
						float y = MathUtils.Min(0.08f * vector3.LengthSquared(), 12f);
						Vector3 v = vector + new Vector3(0f, y, 0f);
						Vector3 vector4 = this.Speed * Vector3.Normalize(v - position);
						vector4.Y = MathUtils.Max(vector4.Y, -0.5f);
						this.m_flyOrder = new Vector3?(vector4);
						this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
					}
					else if (this.m_componentCreature.ComponentLocomotion.SwimSpeed > 0f && this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f)
					{
						Vector3 vector5 = this.Speed * Vector3.Normalize(vector - position);
						vector5.Y = MathUtils.Clamp(vector5.Y, -0.5f, 0.5f);
						this.m_swimOrder = new Vector3?(vector5);
						this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
					}
					else if (this.m_componentCreature.ComponentLocomotion.WalkSpeed > 0f)
					{
						if (this.IsTerrainSafeToGo(position, vector2))
						{
							this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
							if (num2 > 1f)
							{
								this.m_walkOrder = new Vector2?(new Vector2(0f, MathUtils.Lerp(this.Speed, 0f, MathUtils.Saturate((MathUtils.Abs(x) - 0.33f) / 0.66f))));
								if (this.Speed >= 1f && this.m_componentCreature.ComponentLocomotion.InAirWalkFactor >= 1f && num > 1f && this.m_random.Float(0f, 1f) < 0.05f)
								{
									this.m_jumpOrder = 1f;
								}
							}
							else
							{
								float x2 = this.Speed * MathUtils.Min(1f * MathUtils.Sqrt(num2), 1f);
								this.m_walkOrder = new Vector2?(new Vector2(0f, MathUtils.Lerp(x2, 0f, MathUtils.Saturate(2f * MathUtils.Abs(x)))));
							}
						}
						else
						{
							this.IsStuck = true;
						}
						this.m_componentCreature.ComponentBody.IsSmoothRiseEnabled = (num2 >= 1f || vector2.Y >= -0.1f);
						if (num2 < 1f && (vector2.Y < -0.5f || vector2.Y > 1f))
						{
							if (vector2.Y > 0f && this.m_random.Float(0f, 1f) < 0.05f)
							{
								this.m_jumpOrder = 1f;
							}
							if (this.m_aboveBelowTime == null)
							{
								this.m_aboveBelowTime = new double?(this.m_subsystemTime.GameTime);
							}
							else if (this.m_subsystemTime.GameTime - this.m_aboveBelowTime.Value > 2.0 && this.m_componentCreature.ComponentBody.StandingOnValue != null)
							{
								this.IsStuck = true;
							}
						}
						else
						{
							this.m_aboveBelowTime = null;
						}
					}
					if ((!this.IgnoreHeightDifference) ? (num <= this.Range * this.Range) : (num2 <= this.Range * this.Range))
					{
						if (this.RaycastDestination)
						{
							if (this.m_subsystemTerrain.Raycast(position + new Vector3(0f, 0.5f, 0f), vector + new Vector3(0f, 0.5f, 0f), false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value)) == null)
							{
								this.Destination = null;
							}
						}
						else
						{
							this.Destination = null;
						}
					}
				}
				if (this.Destination == null && this.m_componentCreature.ComponentLocomotion.FlySpeed > 0f && this.m_componentCreature.ComponentBody.StandingOnValue == null && this.m_componentCreature.ComponentBody.ImmersionFactor == 0f)
				{
					this.m_turnOrder = Vector2.Zero;
					this.m_walkOrder = null;
					this.m_swimOrder = null;
					this.m_flyOrder = new Vector3?(new Vector3(0f, -0.5f, 0f));
				}
			}
			this.m_componentCreature.ComponentLocomotion.WalkOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.WalkOrder, this.m_walkOrder);
			this.m_componentCreature.ComponentLocomotion.SwimOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.SwimOrder, this.m_swimOrder);
			this.m_componentCreature.ComponentLocomotion.TurnOrder += this.m_turnOrder;
			this.m_componentCreature.ComponentLocomotion.FlyOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.FlyOrder, this.m_flyOrder);
			this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_jumpOrder, this.m_componentCreature.ComponentLocomotion.JumpOrder);
			this.m_jumpOrder = 0f;
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x000810EC File Offset: 0x0007F2EC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00081144 File Offset: 0x0007F344
		public virtual bool IsTerrainSafeToGo(Vector3 position, Vector3 direction)
		{
			Vector3 vector = position + new Vector3(0f, 0.1f, 0f) + ((direction.LengthSquared() < 1.2f) ? new Vector3(direction.X, 0f, direction.Z) : (1.2f * Vector3.Normalize(new Vector3(direction.X, 0f, direction.Z))));
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (Vector3.Dot(direction, new Vector3((float)i, 0f, (float)j)) > 0f)
					{
						for (int k = 0; k >= -2; k--)
						{
							int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector.X) + i, Terrain.ToCell(vector.Y) + k, Terrain.ToCell(vector.Z) + j);
							Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
							if (block.ShouldAvoid(cellValue))
							{
								return false;
							}
							if (block.IsCollidable_(cellValue))
							{
								break;
							}
						}
					}
				}
			}
			Vector3 vector2 = position + new Vector3(0f, 0.1f, 0f) + ((direction.LengthSquared() < 1f) ? new Vector3(direction.X, 0f, direction.Z) : (1f * Vector3.Normalize(new Vector3(direction.X, 0f, direction.Z))));
			bool flag = true;
			int num = this.TakeRisks ? 7 : 5;
			for (int l = 0; l >= -num; l--)
			{
				int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector2.X), Terrain.ToCell(vector2.Y) + l, Terrain.ToCell(vector2.Z));
				Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
				if ((block2.IsCollidable_(cellValue2) || block2.BlockIndex == 18) && !block2.ShouldAvoid(cellValue2))
				{
					flag = false;
					break;
				}
			}
			return !flag;
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00081374 File Offset: 0x0007F574
		public virtual ComponentBody FindNearestBodyInFront(Vector3 position, Vector2 direction)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextBodiesUpdateTime)
			{
				this.m_nextBodiesUpdateTime = this.m_subsystemTime.GameTime + 0.5;
				this.m_nearbyBodies.Clear();
				this.m_subsystemBodies.FindBodiesAroundPoint(this.m_componentCreature.ComponentBody.Position.XZ, 4f, this.m_nearbyBodies);
			}
			ComponentBody result = null;
			float num = float.MaxValue;
			foreach (ComponentBody componentBody in this.m_nearbyBodies)
			{
				if (componentBody != this.m_componentCreature.ComponentBody && MathUtils.Abs(componentBody.Position.Y - this.m_componentCreature.ComponentBody.Position.Y) <= 1.1f && Vector2.Dot(componentBody.Position.XZ - position.XZ, direction) > 0f)
				{
					float num2 = Vector2.DistanceSquared(componentBody.Position.XZ, position.XZ);
					if (num2 < num)
					{
						num = num2;
						result = componentBody;
					}
				}
			}
			return result;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x000814C4 File Offset: 0x0007F6C4
		public virtual Vector3 AvoidNearestBody(Vector3 position, Vector3 destination)
		{
			Vector2 v = destination.XZ - position.XZ;
			ComponentBody componentBody = this.FindNearestBodyInFront(position, Vector2.Normalize(v));
			if (componentBody != null && componentBody != this.DoNotAvoidBody)
			{
				float num = 0.72f * (componentBody.BoxSize.X + this.m_componentCreature.ComponentBody.BoxSize.X) + 0.5f;
				Vector2 xz = componentBody.Position.XZ;
				Vector2 v2 = Segment2.NearestPoint(new Segment2(position.XZ, destination.XZ), xz) - xz;
				if (v2.LengthSquared() < num * num)
				{
					float num2 = v.Length();
					Vector2 vector = Vector2.Normalize(xz + Vector2.Normalize(v2) * num - position.XZ);
					if (Vector2.Dot(v / num2, vector) > 0.5f)
					{
						return new Vector3(position.X + vector.X * num2, destination.Y, position.Z + vector.Y * num2);
					}
				}
			}
			return destination;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000815E4 File Offset: 0x0007F7E4
		public static Vector2? CombineNullables(Vector2? v1, Vector2? v2)
		{
			if (v1 == null)
			{
				return v2;
			}
			if (v2 == null)
			{
				return v1;
			}
			return new Vector2?(v1.Value + v2.Value);
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x00081614 File Offset: 0x0007F814
		public static Vector3? CombineNullables(Vector3? v1, Vector3? v2)
		{
			if (v1 == null)
			{
				return v2;
			}
			if (v2 == null)
			{
				return v1;
			}
			return new Vector3?(v1.Value + v2.Value);
		}

		// Token: 0x04000A5A RID: 2650
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000A5B RID: 2651
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000A5C RID: 2652
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000A5D RID: 2653
		public ComponentCreature m_componentCreature;

		// Token: 0x04000A5E RID: 2654
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000A5F RID: 2655
		public Vector2? m_walkOrder;

		// Token: 0x04000A60 RID: 2656
		public Vector3? m_flyOrder;

		// Token: 0x04000A61 RID: 2657
		public Vector3? m_swimOrder;

		// Token: 0x04000A62 RID: 2658
		public Vector2 m_turnOrder;

		// Token: 0x04000A63 RID: 2659
		public float m_jumpOrder;

		// Token: 0x04000A64 RID: 2660
		public double m_nextUpdateTime;

		// Token: 0x04000A65 RID: 2661
		public double m_lastStuckCheckTime;

		// Token: 0x04000A66 RID: 2662
		public int m_stuckCount;

		// Token: 0x04000A67 RID: 2663
		public double? m_aboveBelowTime;

		// Token: 0x04000A68 RID: 2664
		public Vector3? m_lastStuckCheckPosition;

		// Token: 0x04000A69 RID: 2665
		public DynamicArray<ComponentBody> m_nearbyBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000A6A RID: 2666
		public double m_nextBodiesUpdateTime;

		// Token: 0x04000A6B RID: 2667
		public static bool DrawPilotDestination;
	}
}
