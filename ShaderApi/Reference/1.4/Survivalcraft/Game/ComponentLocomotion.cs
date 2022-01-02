using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021A RID: 538
	public class ComponentLocomotion : Component, IUpdateable
	{
		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06001068 RID: 4200 RVA: 0x0007B4CC File Offset: 0x000796CC
		// (set) Token: 0x06001069 RID: 4201 RVA: 0x0007B4D4 File Offset: 0x000796D4
		public float AccelerationFactor { get; set; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x0007B4DD File Offset: 0x000796DD
		// (set) Token: 0x0600106B RID: 4203 RVA: 0x0007B4E5 File Offset: 0x000796E5
		public float WalkSpeed { get; set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600106C RID: 4204 RVA: 0x0007B4EE File Offset: 0x000796EE
		// (set) Token: 0x0600106D RID: 4205 RVA: 0x0007B4F6 File Offset: 0x000796F6
		public float LadderSpeed { get; set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600106E RID: 4206 RVA: 0x0007B4FF File Offset: 0x000796FF
		// (set) Token: 0x0600106F RID: 4207 RVA: 0x0007B507 File Offset: 0x00079707
		public float JumpSpeed { get; set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06001070 RID: 4208 RVA: 0x0007B510 File Offset: 0x00079710
		// (set) Token: 0x06001071 RID: 4209 RVA: 0x0007B518 File Offset: 0x00079718
		public float FlySpeed { get; set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0007B521 File Offset: 0x00079721
		// (set) Token: 0x06001073 RID: 4211 RVA: 0x0007B529 File Offset: 0x00079729
		public float CreativeFlySpeed { get; set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06001074 RID: 4212 RVA: 0x0007B532 File Offset: 0x00079732
		// (set) Token: 0x06001075 RID: 4213 RVA: 0x0007B53A File Offset: 0x0007973A
		public float SwimSpeed { get; set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06001076 RID: 4214 RVA: 0x0007B543 File Offset: 0x00079743
		// (set) Token: 0x06001077 RID: 4215 RVA: 0x0007B54B File Offset: 0x0007974B
		public float TurnSpeed { get; set; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06001078 RID: 4216 RVA: 0x0007B554 File Offset: 0x00079754
		// (set) Token: 0x06001079 RID: 4217 RVA: 0x0007B55C File Offset: 0x0007975C
		public float LookSpeed { get; set; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x0007B565 File Offset: 0x00079765
		// (set) Token: 0x0600107B RID: 4219 RVA: 0x0007B56D File Offset: 0x0007976D
		public float InAirWalkFactor { get; set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x0007B576 File Offset: 0x00079776
		// (set) Token: 0x0600107D RID: 4221 RVA: 0x0007B57E File Offset: 0x0007977E
		public float? SlipSpeed { get; set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x0600107E RID: 4222 RVA: 0x0007B587 File Offset: 0x00079787
		// (set) Token: 0x0600107F RID: 4223 RVA: 0x0007B590 File Offset: 0x00079790
		public Vector2 LookAngles
		{
			get
			{
				return this.m_lookAngles;
			}
			set
			{
				value.X = MathUtils.Clamp(value.X, 0f - MathUtils.DegToRad(140f), MathUtils.DegToRad(140f));
				value.Y = MathUtils.Clamp(value.Y, 0f - MathUtils.DegToRad(82f), MathUtils.DegToRad(82f));
				this.m_lookAngles = value;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06001080 RID: 4224 RVA: 0x0007B5FC File Offset: 0x000797FC
		// (set) Token: 0x06001081 RID: 4225 RVA: 0x0007B604 File Offset: 0x00079804
		public int? LadderValue { get; set; }

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06001082 RID: 4226 RVA: 0x0007B60D File Offset: 0x0007980D
		// (set) Token: 0x06001083 RID: 4227 RVA: 0x0007B618 File Offset: 0x00079818
		public Vector2? WalkOrder
		{
			get
			{
				return this.m_walkOrder;
			}
			set
			{
				this.m_walkOrder = value;
				if (this.m_walkOrder != null)
				{
					float num = this.m_walkOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_walkOrder = new Vector2?(this.m_walkOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x0007B676 File Offset: 0x00079876
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x0007B680 File Offset: 0x00079880
		public Vector3? FlyOrder
		{
			get
			{
				return this.m_flyOrder;
			}
			set
			{
				this.m_flyOrder = value;
				if (this.m_flyOrder != null)
				{
					float num = this.m_flyOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_flyOrder = new Vector3?(this.m_flyOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x0007B6DE File Offset: 0x000798DE
		// (set) Token: 0x06001087 RID: 4231 RVA: 0x0007B6E8 File Offset: 0x000798E8
		public Vector3? SwimOrder
		{
			get
			{
				return this.m_swimOrder;
			}
			set
			{
				this.m_swimOrder = value;
				if (this.m_swimOrder != null)
				{
					float num = this.m_swimOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_swimOrder = new Vector3?(this.m_swimOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x0007B746 File Offset: 0x00079946
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x0007B74E File Offset: 0x0007994E
		public Vector2 TurnOrder
		{
			get
			{
				return this.m_turnOrder;
			}
			set
			{
				this.m_turnOrder = value;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x0007B757 File Offset: 0x00079957
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x0007B75F File Offset: 0x0007995F
		public Vector2 LookOrder
		{
			get
			{
				return this.m_lookOrder;
			}
			set
			{
				this.m_lookOrder = value;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x0007B768 File Offset: 0x00079968
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x0007B770 File Offset: 0x00079970
		public float JumpOrder
		{
			get
			{
				return this.m_jumpOrder;
			}
			set
			{
				this.m_jumpOrder = MathUtils.Saturate(value);
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x0007B77E File Offset: 0x0007997E
		// (set) Token: 0x0600108F RID: 4239 RVA: 0x0007B786 File Offset: 0x00079986
		public Vector3? VrMoveOrder { get; set; }

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0007B78F File Offset: 0x0007998F
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x0007B797 File Offset: 0x00079997
		public Vector2? VrLookOrder { get; set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x0007B7A0 File Offset: 0x000799A0
		// (set) Token: 0x06001093 RID: 4243 RVA: 0x0007B7A8 File Offset: 0x000799A8
		public float StunTime { get; set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x0007B7B1 File Offset: 0x000799B1
		// (set) Token: 0x06001095 RID: 4245 RVA: 0x0007B7B9 File Offset: 0x000799B9
		public Vector2? LastWalkOrder { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x0007B7C2 File Offset: 0x000799C2
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x0007B7CA File Offset: 0x000799CA
		public float LastJumpOrder { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x0007B7D3 File Offset: 0x000799D3
		// (set) Token: 0x06001099 RID: 4249 RVA: 0x0007B7DB File Offset: 0x000799DB
		public Vector3? LastFlyOrder { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x0600109A RID: 4250 RVA: 0x0007B7E4 File Offset: 0x000799E4
		// (set) Token: 0x0600109B RID: 4251 RVA: 0x0007B7EC File Offset: 0x000799EC
		public Vector3? LastSwimOrder { get; set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x0600109C RID: 4252 RVA: 0x0007B7F5 File Offset: 0x000799F5
		// (set) Token: 0x0600109D RID: 4253 RVA: 0x0007B7FD File Offset: 0x000799FD
		public Vector2 LastTurnOrder { get; set; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x0600109E RID: 4254 RVA: 0x0007B806 File Offset: 0x00079A06
		// (set) Token: 0x0600109F RID: 4255 RVA: 0x0007B80E File Offset: 0x00079A0E
		public bool IsCreativeFlyEnabled { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060010A0 RID: 4256 RVA: 0x0007B817 File Offset: 0x00079A17
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Locomotion;
			}
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0007B81C File Offset: 0x00079A1C
		public void Update(float dt)
		{
			this.SlipSpeed = null;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative)
			{
				this.IsCreativeFlyEnabled = false;
			}
			this.StunTime = MathUtils.Max(this.StunTime - dt, 0f);
			if (this.m_componentCreature.ComponentHealth.Health > 0f && this.StunTime <= 0f)
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				PlayerStats playerStats = this.m_componentCreature.PlayerStats;
				if (playerStats != null)
				{
					float num = (this.m_lastPosition != null) ? Vector3.Distance(position, this.m_lastPosition.Value) : 0f;
					num = MathUtils.Min(num, 25f * this.m_subsystemTime.PreviousGameTimeDelta);
					playerStats.DistanceTravelled += (double)num;
					if (this.m_componentRider != null && this.m_componentRider.Mount != null)
					{
						playerStats.DistanceRidden += (double)num;
					}
					else
					{
						if (this.m_walking)
						{
							playerStats.DistanceWalked += (double)num;
							this.m_walking = false;
						}
						if (this.m_falling)
						{
							playerStats.DistanceFallen += (double)num;
							this.m_falling = false;
						}
						if (this.m_climbing)
						{
							playerStats.DistanceClimbed += (double)num;
							this.m_climbing = false;
						}
						if (this.m_jumping)
						{
							playerStats.Jumps += 1L;
							this.m_jumping = false;
						}
						if (this.m_swimming)
						{
							playerStats.DistanceSwam += (double)num;
							this.m_swimming = false;
						}
						if (this.m_flying)
						{
							playerStats.DistanceFlown += (double)num;
							this.m_flying = false;
						}
					}
					playerStats.DeepestDive = MathUtils.Max(playerStats.DeepestDive, (double)this.m_componentCreature.ComponentBody.ImmersionDepth);
					playerStats.LowestAltitude = MathUtils.Min(playerStats.LowestAltitude, (double)position.Y);
					playerStats.HighestAltitude = MathUtils.Max(playerStats.HighestAltitude, (double)position.Y);
					playerStats.EasiestModeUsed = (GameMode)MathUtils.Min((int)this.m_subsystemGameInfo.WorldSettings.GameMode, (int)playerStats.EasiestModeUsed);
				}
				this.m_lastPosition = new Vector3?(position);
				this.m_swimBurstRemaining = MathUtils.Saturate(0.1f * this.m_swimBurstRemaining + dt);
				int x = Terrain.ToCell(position.X);
				int y = Terrain.ToCell(position.Y + 0.2f);
				int z = Terrain.ToCell(position.Z);
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
				int num2 = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num2];
				if (this.LadderSpeed > 0f && this.LadderValue == null && block is LadderBlock && this.m_subsystemTime.GameTime >= this.m_ladderActivationTime && !this.IsCreativeFlyEnabled && this.m_componentCreature.ComponentBody.ParentBody == null)
				{
					int face = LadderBlock.GetFace(Terrain.ExtractData(cellValue));
					if ((face == 0 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z > 0f) || (face == 1 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.X > 0f) || (face == 2 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z < 0f) || (face == 3 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.X < 0f) || this.m_componentCreature.ComponentBody.StandingOnValue == null)
					{
						this.LadderValue = new int?(cellValue);
						this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
						this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
					}
				}
				Quaternion rotation = this.m_componentCreature.ComponentBody.Rotation;
				float num3 = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				num3 += (0f - this.TurnSpeed) * this.TurnOrder.X * dt;
				if (this.VrLookOrder != null)
				{
					num3 += this.VrLookOrder.Value.X;
				}
				this.m_componentCreature.ComponentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num3);
				this.LookAngles += this.LookSpeed * this.LookOrder * dt;
				if (this.VrLookOrder != null)
				{
					this.LookAngles = new Vector2(this.LookAngles.X, this.VrLookOrder.Value.Y);
				}
				if (this.VrMoveOrder != null)
				{
					this.m_componentCreature.ComponentBody.ApplyDirectMove(this.VrMoveOrder.Value);
				}
				if (this.LadderValue != null)
				{
					this.LadderMovement(dt, cellValue);
				}
				else
				{
					this.NormalMovement(dt);
				}
			}
			else
			{
				this.m_componentCreature.ComponentBody.IsGravityEnabled = true;
				this.m_componentCreature.ComponentBody.IsGroundDragEnabled = true;
				this.m_componentCreature.ComponentBody.IsWaterDragEnabled = true;
			}
			this.LastWalkOrder = this.WalkOrder;
			this.LastFlyOrder = this.FlyOrder;
			this.LastSwimOrder = this.SwimOrder;
			this.LastTurnOrder = this.TurnOrder;
			this.LastJumpOrder = this.JumpOrder;
			this.WalkOrder = null;
			this.FlyOrder = null;
			this.SwimOrder = null;
			this.TurnOrder = Vector2.Zero;
			this.JumpOrder = 0f;
			this.VrMoveOrder = null;
			this.VrLookOrder = null;
			this.LookOrder = new Vector2(this.m_lookAutoLevelX ? (-10f * this.LookAngles.X / this.LookSpeed) : 0f, this.m_lookAutoLevelY ? (-10f * this.LookAngles.Y / this.LookSpeed) : 0f);
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x0007BEE8 File Offset: 0x0007A0E8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			this.m_componentLevel = base.Entity.FindComponent<ComponentLevel>();
			this.m_componentClothing = base.Entity.FindComponent<ComponentClothing>();
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>();
			this.m_componentRider = base.Entity.FindComponent<ComponentRider>();
			this.IsCreativeFlyEnabled = valuesDictionary.GetValue<bool>("IsCreativeFlyEnabled");
			this.AccelerationFactor = valuesDictionary.GetValue<float>("AccelerationFactor");
			this.WalkSpeed = valuesDictionary.GetValue<float>("WalkSpeed");
			this.LadderSpeed = valuesDictionary.GetValue<float>("LadderSpeed");
			this.JumpSpeed = valuesDictionary.GetValue<float>("JumpSpeed");
			this.CreativeFlySpeed = valuesDictionary.GetValue<float>("CreativeFlySpeed");
			this.FlySpeed = valuesDictionary.GetValue<float>("FlySpeed");
			this.SwimSpeed = valuesDictionary.GetValue<float>("SwimSpeed");
			this.TurnSpeed = valuesDictionary.GetValue<float>("TurnSpeed");
			this.LookSpeed = valuesDictionary.GetValue<float>("LookSpeed");
			this.InAirWalkFactor = valuesDictionary.GetValue<float>("InAirWalkFactor");
			this.m_walkSpeedWhenTurning = valuesDictionary.GetValue<float>("WalkSpeedWhenTurning");
			this.m_minFrictionFactor = valuesDictionary.GetValue<float>("MinFrictionFactor");
			this.m_lookAutoLevelX = valuesDictionary.GetValue<bool>("LookAutoLevelX");
			this.m_lookAutoLevelY = valuesDictionary.GetValue<bool>("LookAutoLevelY");
			if (base.Entity.FindComponent<ComponentPlayer>() == null)
			{
				this.WalkSpeed *= this.m_random.Float(0.85f, 1f);
				this.FlySpeed *= this.m_random.Float(0.85f, 1f);
				this.SwimSpeed *= this.m_random.Float(0.85f, 1f);
			}
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x0007C116 File Offset: 0x0007A316
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("IsCreativeFlyEnabled", this.IsCreativeFlyEnabled);
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x0007C12C File Offset: 0x0007A32C
		public virtual void NormalMovement(float dt)
		{
			this.m_componentCreature.ComponentBody.IsGravityEnabled = true;
			this.m_componentCreature.ComponentBody.IsGroundDragEnabled = true;
			this.m_componentCreature.ComponentBody.IsWaterDragEnabled = true;
			Vector3 vector = this.m_componentCreature.ComponentBody.Velocity;
			Vector3 right = this.m_componentCreature.ComponentBody.Matrix.Right;
			Vector3 vector2 = Vector3.Transform(this.m_componentCreature.ComponentBody.Matrix.Forward, Quaternion.CreateFromAxisAngle(right, this.LookAngles.Y));
			if (this.WalkSpeed > 0f && this.WalkOrder != null)
			{
				if (this.IsCreativeFlyEnabled || ComponentLocomotion.IsCreativeFlyEnabledSet)
				{
					Vector3 vector3 = new Vector3(this.WalkOrder.Value.X, 0f, this.WalkOrder.Value.Y);
					if (this.FlyOrder != null)
					{
						vector3 += this.FlyOrder.Value;
					}
					Vector3 v = (!SettingsManager.HorizontalCreativeFlight || this.m_componentPlayer == null || this.m_componentPlayer.ComponentInput.IsControlledByTouch) ? Vector3.Normalize(vector2 + 0.1f * Vector3.UnitY) : Vector3.Normalize(vector2 * new Vector3(1f, 0f, 1f));
					Vector3 v2 = this.CreativeFlySpeed * (right * vector3.X + Vector3.UnitY * vector3.Y + v * vector3.Z);
					float num = (vector3 == Vector3.Zero) ? 5f : 3f;
					vector += MathUtils.Saturate(num * dt) * (v2 - vector);
					this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
					this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
					this.m_flying = true;
				}
				else
				{
					Vector2 value = this.WalkOrder.Value;
					if (this.m_walkSpeedWhenTurning > 0f && MathUtils.Abs(this.TurnOrder.X) > 0.02f)
					{
						value.Y = MathUtils.Max(value.Y, MathUtils.Lerp(0f, this.m_walkSpeedWhenTurning, MathUtils.Saturate(2f * MathUtils.Abs(this.TurnOrder.X))));
					}
					float num2 = this.WalkSpeed;
					if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0.2f)
					{
						num2 *= 0.66f;
					}
					if (value.Y < 0f)
					{
						num2 *= 0.6f;
					}
					if (this.m_componentLevel != null)
					{
						num2 *= this.m_componentLevel.SpeedFactor;
					}
					if (this.m_componentMount != null)
					{
						ComponentRider rider = this.m_componentMount.Rider;
						if (rider != null)
						{
							ComponentClothing componentClothing = rider.Entity.FindComponent<ComponentClothing>();
							if (componentClothing != null)
							{
								num2 *= componentClothing.SteedMovementSpeedFactor;
							}
						}
					}
					Vector3 v3 = value.X * Vector3.Normalize(new Vector3(right.X, 0f, right.Z)) + value.Y * Vector3.Normalize(new Vector3(vector2.X, 0f, vector2.Z));
					Vector3 vector4 = num2 * v3 + this.m_componentCreature.ComponentBody.StandingOnVelocity;
					float num4;
					if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
					{
						float num3 = MathUtils.Max(BlocksManager.Blocks[Terrain.ExtractContents(this.m_componentCreature.ComponentBody.StandingOnValue.Value)].FrictionFactor, this.m_minFrictionFactor);
						num4 = MathUtils.Saturate(dt * 6f * this.AccelerationFactor * num3);
						if (num3 < 0.25f)
						{
							this.SlipSpeed = new float?(num2 * value.Length());
						}
						this.m_walking = true;
					}
					else
					{
						num4 = MathUtils.Saturate(dt * 6f * this.AccelerationFactor * this.InAirWalkFactor);
						if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0f)
						{
							this.m_swimming = true;
						}
						else
						{
							this.m_falling = true;
						}
					}
					vector.X += num4 * (vector4.X - vector.X);
					vector.Z += num4 * (vector4.Z - vector.Z);
					Vector3 vector5 = value.X * right + value.Y * vector2;
					if (this.m_componentLevel != null)
					{
						vector5 *= this.m_componentLevel.SpeedFactor;
					}
					vector.Y += 10f * this.AccelerationFactor * vector5.Y * this.m_componentCreature.ComponentBody.ImmersionFactor * dt;
					this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
					if (this.m_componentPlayer != null && Time.PeriodicEvent(10.0, 0.0) && (this.m_shoesWarningTime == 0.0 || Time.FrameStartTime - this.m_shoesWarningTime > 300.0) && this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_componentCreature.ComponentBody.ImmersionFactor < 0.1f)
					{
						bool flag = false;
						int value2 = this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Feet).LastOrDefault<int>();
						if (Terrain.ExtractContents(value2) == 203)
						{
							flag = (BlocksManager.Blocks[Terrain.ExtractContents(value2)].GetClothingData(value2).MovementSpeedFactor > 1f);
						}
						if (!flag && vector4.LengthSquared() / vector.LengthSquared() > 0.99f && this.WalkOrder.Value.LengthSquared() > 0.99f)
						{
							this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(base.GetType().Name, 1), Color.White, true, true);
							this.m_shoesWarningTime = Time.FrameStartTime;
						}
					}
				}
			}
			if (this.FlySpeed > 0f && this.FlyOrder != null)
			{
				Vector3 value3 = this.FlyOrder.Value;
				Vector3 v4 = this.FlySpeed * value3;
				vector += MathUtils.Saturate(2f * this.AccelerationFactor * dt) * (v4 - vector);
				this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
				this.m_flying = true;
			}
			if (this.SwimSpeed > 0f && this.SwimOrder != null && this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f)
			{
				Vector3 value4 = this.SwimOrder.Value;
				Vector3 vector6 = this.SwimSpeed * value4;
				float num5 = 2f;
				if (value4.LengthSquared() >= 0.99f)
				{
					vector6 *= MathUtils.Lerp(1f, 2f, this.m_swimBurstRemaining);
					num5 *= MathUtils.Lerp(1f, 4f, this.m_swimBurstRemaining);
					this.m_swimBurstRemaining -= dt;
				}
				vector += MathUtils.Saturate(num5 * this.AccelerationFactor * dt) * (vector6 - vector);
				this.m_componentCreature.ComponentBody.IsGravityEnabled = (MathUtils.Abs(value4.Y) <= 0.07f);
				this.m_componentCreature.ComponentBody.IsWaterDragEnabled = false;
				this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
				this.m_swimming = true;
			}
			if (this.JumpOrder > 0f && (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f) && !this.m_componentCreature.ComponentBody.IsSneaking)
			{
				float num6 = this.JumpSpeed;
				if (this.m_componentLevel != null)
				{
					num6 *= 0.25f * (this.m_componentLevel.SpeedFactor - 1f) + 1f;
				}
				vector.Y = MathUtils.Min(vector.Y + MathUtils.Saturate(this.JumpOrder) * num6, num6);
				this.m_jumping = true;
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 10f);
			}
			if (MathUtils.Abs(this.m_componentCreature.ComponentBody.CollisionVelocityChange.Y) > 3f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 10f);
			}
			this.m_componentCreature.ComponentBody.Velocity = vector;
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0007CAA8 File Offset: 0x0007ACA8
		public virtual void LadderMovement(float dt, int value)
		{
			this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = this.m_componentCreature.ComponentBody.Velocity;
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num] is LadderBlock)
			{
				this.LadderValue = new int?(value);
				if (this.WalkOrder != null)
				{
					Vector2 value2 = this.WalkOrder.Value;
					float num2 = this.LadderSpeed * value2.Y;
					vector.X = 5f * (MathUtils.Floor(position.X) + 0.5f - position.X);
					vector.Z = 5f * (MathUtils.Floor(position.Z) + 0.5f - position.Z);
					vector.Y += MathUtils.Saturate(20f * dt) * (num2 - vector.Y);
					this.m_climbing = true;
				}
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_subsystemTime.GameTime >= this.m_ladderActivationTime)
				{
					this.LadderValue = null;
					this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
				}
			}
			else
			{
				this.LadderValue = null;
				this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
			}
			if (this.JumpOrder > 0f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				vector += this.JumpSpeed * this.m_componentCreature.ComponentBody.Matrix.Forward;
				this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.33000001311302185;
				this.LadderValue = null;
				this.m_jumping = true;
			}
			if (this.IsCreativeFlyEnabled)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
				this.LadderValue = null;
			}
			if (this.m_componentCreature.ComponentBody.ParentBody != null)
			{
				this.LadderValue = null;
			}
			this.m_componentCreature.ComponentBody.Velocity = vector;
		}

		// Token: 0x040009B9 RID: 2489
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009BA RID: 2490
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040009BB RID: 2491
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009BC RID: 2492
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040009BD RID: 2493
		public ComponentCreature m_componentCreature;

		// Token: 0x040009BE RID: 2494
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040009BF RID: 2495
		public ComponentLevel m_componentLevel;

		// Token: 0x040009C0 RID: 2496
		public ComponentClothing m_componentClothing;

		// Token: 0x040009C1 RID: 2497
		public ComponentMount m_componentMount;

		// Token: 0x040009C2 RID: 2498
		public ComponentRider m_componentRider;

		// Token: 0x040009C3 RID: 2499
		public Game.Random m_random = new Game.Random();

		// Token: 0x040009C4 RID: 2500
		public Vector2? m_walkOrder;

		// Token: 0x040009C5 RID: 2501
		public Vector3? m_flyOrder;

		// Token: 0x040009C6 RID: 2502
		public Vector3? m_swimOrder;

		// Token: 0x040009C7 RID: 2503
		public Vector2 m_turnOrder;

		// Token: 0x040009C8 RID: 2504
		public Vector2 m_lookOrder;

		// Token: 0x040009C9 RID: 2505
		public float m_jumpOrder;

		// Token: 0x040009CA RID: 2506
		public bool m_lookAutoLevelX;

		// Token: 0x040009CB RID: 2507
		public bool m_lookAutoLevelY;

		// Token: 0x040009CC RID: 2508
		public double m_shoesWarningTime;

		// Token: 0x040009CD RID: 2509
		public float m_walkSpeedWhenTurning;

		// Token: 0x040009CE RID: 2510
		public float m_minFrictionFactor;

		// Token: 0x040009CF RID: 2511
		public double m_ladderActivationTime;

		// Token: 0x040009D0 RID: 2512
		public float m_swimBurstRemaining;

		// Token: 0x040009D1 RID: 2513
		public Vector2 m_lookAngles;

		// Token: 0x040009D2 RID: 2514
		public Vector3? m_lastPosition;

		// Token: 0x040009D3 RID: 2515
		public bool m_walking;

		// Token: 0x040009D4 RID: 2516
		public bool m_falling;

		// Token: 0x040009D5 RID: 2517
		public bool m_climbing;

		// Token: 0x040009D6 RID: 2518
		public bool m_jumping;

		// Token: 0x040009D7 RID: 2519
		public bool m_swimming;

		// Token: 0x040009D8 RID: 2520
		public bool m_flying;

		// Token: 0x040009ED RID: 2541
		public static bool IsCreativeFlyEnabledSet;
	}
}
