using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F0 RID: 496
	public class ComponentBody : ComponentFrame, IUpdateable
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x00063ABF File Offset: 0x00061CBF
		// (set) Token: 0x06000DAC RID: 3500 RVA: 0x00063AC7 File Offset: 0x00061CC7
		public Vector3 BoxSize { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000DAD RID: 3501 RVA: 0x00063AD0 File Offset: 0x00061CD0
		// (set) Token: 0x06000DAE RID: 3502 RVA: 0x00063AD8 File Offset: 0x00061CD8
		public float Mass { get; set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000DAF RID: 3503 RVA: 0x00063AE1 File Offset: 0x00061CE1
		// (set) Token: 0x06000DB0 RID: 3504 RVA: 0x00063AE9 File Offset: 0x00061CE9
		public float Density { get; set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x00063AF2 File Offset: 0x00061CF2
		// (set) Token: 0x06000DB2 RID: 3506 RVA: 0x00063AFA File Offset: 0x00061CFA
		public Vector2 AirDrag { get; set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x00063B03 File Offset: 0x00061D03
		// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x00063B0B File Offset: 0x00061D0B
		public Vector2 WaterDrag { get; set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x00063B14 File Offset: 0x00061D14
		// (set) Token: 0x06000DB6 RID: 3510 RVA: 0x00063B1C File Offset: 0x00061D1C
		public float WaterSwayAngle { get; set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000DB7 RID: 3511 RVA: 0x00063B25 File Offset: 0x00061D25
		// (set) Token: 0x06000DB8 RID: 3512 RVA: 0x00063B2D File Offset: 0x00061D2D
		public float WaterTurnSpeed { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000DB9 RID: 3513 RVA: 0x00063B36 File Offset: 0x00061D36
		// (set) Token: 0x06000DBA RID: 3514 RVA: 0x00063B3E File Offset: 0x00061D3E
		public float ImmersionDepth { get; set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000DBB RID: 3515 RVA: 0x00063B47 File Offset: 0x00061D47
		// (set) Token: 0x06000DBC RID: 3516 RVA: 0x00063B4F File Offset: 0x00061D4F
		public float ImmersionFactor { get; set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000DBD RID: 3517 RVA: 0x00063B58 File Offset: 0x00061D58
		// (set) Token: 0x06000DBE RID: 3518 RVA: 0x00063B60 File Offset: 0x00061D60
		public FluidBlock ImmersionFluidBlock { get; set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000DBF RID: 3519 RVA: 0x00063B69 File Offset: 0x00061D69
		// (set) Token: 0x06000DC0 RID: 3520 RVA: 0x00063B71 File Offset: 0x00061D71
		public int? StandingOnValue { get; set; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000DC1 RID: 3521 RVA: 0x00063B7A File Offset: 0x00061D7A
		// (set) Token: 0x06000DC2 RID: 3522 RVA: 0x00063B82 File Offset: 0x00061D82
		public ComponentBody StandingOnBody { get; set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x00063B8B File Offset: 0x00061D8B
		// (set) Token: 0x06000DC4 RID: 3524 RVA: 0x00063B93 File Offset: 0x00061D93
		public Vector3 StandingOnVelocity { get; set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x00063B9C File Offset: 0x00061D9C
		// (set) Token: 0x06000DC6 RID: 3526 RVA: 0x00063BA4 File Offset: 0x00061DA4
		public Vector3 Velocity
		{
			get
			{
				return this.m_velocity;
			}
			set
			{
				this.m_velocity = ((value.LengthSquared() > 625f) ? (25f * Vector3.Normalize(value)) : value);
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x00063BCD File Offset: 0x00061DCD
		// (set) Token: 0x06000DC8 RID: 3528 RVA: 0x00063BD8 File Offset: 0x00061DD8
		public bool IsSneaking
		{
			get
			{
				return this.m_isSneaking;
			}
			set
			{
				if (this.StandingOnValue == null)
				{
					value = false;
				}
				this.m_isSneaking = value;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x00063BFF File Offset: 0x00061DFF
		// (set) Token: 0x06000DCA RID: 3530 RVA: 0x00063C07 File Offset: 0x00061E07
		public bool IsGravityEnabled { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000DCB RID: 3531 RVA: 0x00063C10 File Offset: 0x00061E10
		// (set) Token: 0x06000DCC RID: 3532 RVA: 0x00063C18 File Offset: 0x00061E18
		public bool IsGroundDragEnabled { get; set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00063C21 File Offset: 0x00061E21
		// (set) Token: 0x06000DCE RID: 3534 RVA: 0x00063C29 File Offset: 0x00061E29
		public bool IsWaterDragEnabled { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00063C32 File Offset: 0x00061E32
		// (set) Token: 0x06000DD0 RID: 3536 RVA: 0x00063C3A File Offset: 0x00061E3A
		public bool IsSmoothRiseEnabled { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x00063C43 File Offset: 0x00061E43
		// (set) Token: 0x06000DD2 RID: 3538 RVA: 0x00063C4B File Offset: 0x00061E4B
		public float MaxSmoothRiseHeight { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x00063C54 File Offset: 0x00061E54
		// (set) Token: 0x06000DD4 RID: 3540 RVA: 0x00063C5C File Offset: 0x00061E5C
		public Vector3 CollisionVelocityChange { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x00063C68 File Offset: 0x00061E68
		public BoundingBox BoundingBox
		{
			get
			{
				Vector3 boxSize = this.BoxSize;
				Vector3 position = base.Position;
				return new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x00063CD9 File Offset: 0x00061ED9
		public ReadOnlyList<ComponentBody> ChildBodies
		{
			get
			{
				return new ReadOnlyList<ComponentBody>(this.m_childBodies);
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000DD7 RID: 3543 RVA: 0x00063CE6 File Offset: 0x00061EE6
		// (set) Token: 0x06000DD8 RID: 3544 RVA: 0x00063CF0 File Offset: 0x00061EF0
		public ComponentBody ParentBody
		{
			get
			{
				return this.m_parentBody;
			}
			set
			{
				if (value != this.m_parentBody)
				{
					if (this.m_parentBody != null)
					{
						this.m_parentBody.m_childBodies.Remove(this);
					}
					this.m_parentBody = value;
					if (this.m_parentBody != null)
					{
						this.m_parentBody.m_childBodies.Add(this);
					}
				}
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x00063D40 File Offset: 0x00061F40
		// (set) Token: 0x06000DDA RID: 3546 RVA: 0x00063D48 File Offset: 0x00061F48
		public Vector3 ParentBodyPositionOffset { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000DDB RID: 3547 RVA: 0x00063D51 File Offset: 0x00061F51
		// (set) Token: 0x06000DDC RID: 3548 RVA: 0x00063D59 File Offset: 0x00061F59
		public Quaternion ParentBodyRotationOffset { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00063D62 File Offset: 0x00061F62
		public UpdateOrder UpdateOrder
		{
			get
			{
				if (this.m_parentBody == null)
				{
					return UpdateOrder.Body;
				}
				return this.m_parentBody.UpdateOrder + 1;
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000DDE RID: 3550 RVA: 0x00063D7B File Offset: 0x00061F7B
		// (set) Token: 0x06000DDF RID: 3551 RVA: 0x00063D83 File Offset: 0x00061F83
		public virtual Action<ComponentBody> CollidedWithBody { get; set; }

		// Token: 0x06000DE0 RID: 3552 RVA: 0x00063D8C File Offset: 0x00061F8C
		static ComponentBody()
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = -2; i <= 2; i++)
			{
				for (int j = -2; j <= 2; j++)
				{
					for (int k = -2; k <= 2; k++)
					{
						Vector3 item = new Vector3(0.25f * (float)i, 0.25f * (float)j, 0.25f * (float)k);
						list.Add(item);
					}
				}
			}
			list.Sort((Vector3 o1, Vector3 o2) => Comparer<float>.Default.Compare(o1.LengthSquared(), o2.LengthSquared()));
			ComponentBody.m_freeSpaceOffsets = list.ToArray();
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x00063E0E File Offset: 0x0006200E
		public void ApplyImpulse(Vector3 impulse)
		{
			this.m_totalImpulse += impulse;
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x00063E22 File Offset: 0x00062022
		public void ApplyDirectMove(Vector3 directMove)
		{
			this.m_directMove += directMove;
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x00063E36 File Offset: 0x00062036
		public bool IsChildOfBody(ComponentBody componentBody)
		{
			return this.ParentBody == componentBody || (this.ParentBody != null && this.ParentBody.IsChildOfBody(componentBody));
		}

		// Token: 0x06000DE4 RID: 3556 RVA: 0x00063E5C File Offset: 0x0006205C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			this.BoxSize = valuesDictionary.GetValue<Vector3>("BoxSize");
			this.Mass = valuesDictionary.GetValue<float>("Mass");
			this.Density = valuesDictionary.GetValue<float>("Density");
			this.AirDrag = valuesDictionary.GetValue<Vector2>("AirDrag");
			this.WaterDrag = valuesDictionary.GetValue<Vector2>("WaterDrag");
			this.WaterSwayAngle = valuesDictionary.GetValue<float>("WaterSwayAngle");
			this.WaterTurnSpeed = valuesDictionary.GetValue<float>("WaterTurnSpeed");
			this.Velocity = valuesDictionary.GetValue<Vector3>("Velocity");
			this.MaxSmoothRiseHeight = valuesDictionary.GetValue<float>("MaxSmoothRiseHeight");
			this.ParentBody = valuesDictionary.GetValue<EntityReference>("ParentBody").GetComponent<ComponentBody>(base.Entity, idToEntityMap, false);
			this.ParentBodyPositionOffset = valuesDictionary.GetValue<Vector3>("ParentBodyPositionOffset");
			this.ParentBodyRotationOffset = valuesDictionary.GetValue<Quaternion>("ParentBodyRotationOffset");
			this.IsSmoothRiseEnabled = true;
			this.IsGravityEnabled = true;
			this.IsGroundDragEnabled = true;
			this.IsWaterDragEnabled = true;
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x00063FFC File Offset: 0x000621FC
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			if (this.Velocity != Vector3.Zero)
			{
				valuesDictionary.SetValue<Vector3>("Velocity", this.Velocity);
			}
			EntityReference value = EntityReference.FromId(this.ParentBody, entityToIdMap);
			if (!value.IsNullOrEmpty())
			{
				valuesDictionary.SetValue<EntityReference>("ParentBody", value);
				valuesDictionary.SetValue<Vector3>("ParentBodyPositionOffset", this.ParentBodyPositionOffset);
				valuesDictionary.SetValue<Quaternion>("ParentBodyRotationOffset", this.ParentBodyRotationOffset);
			}
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x00064078 File Offset: 0x00062278
		public override void OnEntityRemoved()
		{
			this.ParentBody = null;
			ComponentBody[] array = this.ChildBodies.ToArray<ComponentBody>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ParentBody = null;
			}
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x000640B4 File Offset: 0x000622B4
		public void Update(float dt)
		{
			this.CollisionVelocityChange = Vector3.Zero;
			this.Velocity += this.m_totalImpulse;
			this.m_totalImpulse = Vector3.Zero;
			if (this.m_parentBody != null || this.m_velocity.LengthSquared() > 9.99999944E-11f || this.m_directMove != Vector3.Zero)
			{
				this.m_stoppedTime = 0f;
			}
			else
			{
				this.m_stoppedTime += dt;
				if (this.m_stoppedTime > 0.5f && !Time.PeriodicEvent(0.25, 0.0))
				{
					return;
				}
			}
			Vector3 position = base.Position;
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(position.X), Terrain.ToCell(position.Z));
			if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
			{
				this.Velocity = Vector3.Zero;
				return;
			}
			this.m_bodiesCollisionBoxes.Clear();
			this.FindBodiesCollisionBoxes(position, this.m_bodiesCollisionBoxes);
			this.m_movingBlocksCollisionBoxes.Clear();
			this.FindMovingBlocksCollisionBoxes(position, this.m_movingBlocksCollisionBoxes);
			if (!this.MoveToFreeSpace())
			{
				ComponentHealth componentHealth = base.Entity.FindComponent<ComponentHealth>();
				if (componentHealth != null)
				{
					componentHealth.Injure(1f, null, true, "Crushed");
					return;
				}
				base.Project.RemoveEntity(base.Entity, true);
				return;
			}
			else
			{
				if (this.IsGravityEnabled)
				{
					this.m_velocity.Y = this.m_velocity.Y - 10f * dt;
					if (this.ImmersionFactor > 0f)
					{
						float num = this.ImmersionFactor * (1f + 0.03f * MathUtils.Sin((float)MathUtils.Remainder(2.0 * this.m_subsystemTime.GameTime, 6.2831854820251465)));
						this.m_velocity.Y = this.m_velocity.Y + 10f * (1f / this.Density * num) * dt;
					}
				}
				float num2 = MathUtils.Saturate(this.AirDrag.X * dt);
				float num3 = MathUtils.Saturate(this.AirDrag.Y * dt);
				this.m_velocity.X = this.m_velocity.X * (1f - num2);
				this.m_velocity.Y = this.m_velocity.Y * (1f - num3);
				this.m_velocity.Z = this.m_velocity.Z * (1f - num2);
				if (this.IsWaterDragEnabled && this.ImmersionFactor > 0f && this.ImmersionFluidBlock != null)
				{
					Vector2? vector = this.m_subsystemFluidBlockBehavior.CalculateFlowSpeed(Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
					Vector3 vector2 = (vector != null) ? new Vector3(vector.Value.X, 0f, vector.Value.Y) : Vector3.Zero;
					float num4 = 1f;
					if (this.ImmersionFluidBlock.FrictionFactor != 1f)
					{
						num4 = ((SimplexNoise.Noise((float)MathUtils.Remainder(6.0 * Time.FrameStartTime + (double)(this.GetHashCode() % 1000), 1000.0)) > 0.5f) ? this.ImmersionFluidBlock.FrictionFactor : 1f);
					}
					float f = MathUtils.Saturate(this.WaterDrag.X * num4 * this.ImmersionFactor * dt);
					float f2 = MathUtils.Saturate(this.WaterDrag.Y * num4 * dt);
					this.m_velocity.X = MathUtils.Lerp(this.m_velocity.X, vector2.X, f);
					this.m_velocity.Y = MathUtils.Lerp(this.m_velocity.Y, vector2.Y, f2);
					this.m_velocity.Z = MathUtils.Lerp(this.m_velocity.Z, vector2.Z, f);
					if (this.m_parentBody == null && vector != null && this.StandingOnValue == null)
					{
						if (this.WaterTurnSpeed > 0f)
						{
							float s = MathUtils.Saturate(MathUtils.Lerp(1f, 0f, this.m_velocity.Length()));
							Vector2 vector3 = Vector2.Normalize(vector.Value) * s;
							base.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.WaterTurnSpeed * (-1f * vector3.X + 0.71f * vector3.Y) * dt);
						}
						if (this.WaterSwayAngle > 0f)
						{
							base.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, this.WaterSwayAngle * (float)MathUtils.Sin((double)(200f / this.Mass) * this.m_subsystemTime.GameTime));
						}
					}
				}
				if (this.m_parentBody != null)
				{
					Vector3 v = Vector3.Transform(this.ParentBodyPositionOffset, this.m_parentBody.Rotation) + this.m_parentBody.Position - position;
					this.m_velocity = ((dt > 0f) ? (v / dt) : Vector3.Zero);
					base.Rotation = this.ParentBodyRotationOffset * this.m_parentBody.Rotation;
				}
				this.StandingOnValue = null;
				this.StandingOnBody = null;
				this.StandingOnVelocity = Vector3.Zero;
				Vector3 velocity = this.m_velocity;
				float num5 = this.m_velocity.Length();
				if (num5 > 0f)
				{
					float x = 0.45f * MathUtils.Min(this.BoxSize.X, this.BoxSize.Y, this.BoxSize.Z) / num5;
					float num7;
					for (float num6 = dt; num6 > 0f; num6 -= num7)
					{
						num7 = MathUtils.Min(num6, x);
						this.MoveWithCollision(num7, this.m_velocity * num7 + this.m_directMove);
						this.m_directMove = Vector3.Zero;
					}
				}
				this.CollisionVelocityChange = this.m_velocity - velocity;
				if (this.IsGroundDragEnabled && this.StandingOnValue != null)
				{
					this.m_velocity = Vector3.Lerp(this.m_velocity, this.StandingOnVelocity, 6f * dt);
				}
				if (this.StandingOnValue == null)
				{
					this.IsSneaking = false;
				}
				this.UpdateImmersionData();
				if (this.ImmersionFluidBlock is WaterBlock && this.ImmersionDepth > 0.3f && !this.m_fluidEffectsPlayed)
				{
					this.m_fluidEffectsPlayed = true;
					this.m_subsystemAudio.PlayRandomSound("Audio/WaterFallIn", this.m_random.Float(0.75f, 1f), this.m_random.Float(-0.3f, 0f), position, 4f, true);
					this.m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(this.m_subsystemTerrain, position, (this.BoundingBox.Max - this.BoundingBox.Min).Length() > 0.8f));
					return;
				}
				if (this.ImmersionFluidBlock is MagmaBlock && this.ImmersionDepth > 0f && !this.m_fluidEffectsPlayed)
				{
					this.m_fluidEffectsPlayed = true;
					this.m_subsystemAudio.PlaySound("Audio/SizzleLong", 1f, 0f, position, 4f, true);
					this.m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(this.m_subsystemTerrain, position, (this.BoundingBox.Max - this.BoundingBox.Min).Length() > 0.8f));
					return;
				}
				if (this.ImmersionFluidBlock == null)
				{
					this.m_fluidEffectsPlayed = false;
				}
				return;
			}
		}

		// Token: 0x06000DE8 RID: 3560 RVA: 0x00064880 File Offset: 0x00062A80
		public void UpdateImmersionData()
		{
			Vector3 position = base.Position;
			int x = Terrain.ToCell(position.X);
			int y = Terrain.ToCell(position.Y + 0.01f);
			int z = Terrain.ToCell(position.Z);
			FluidBlock fluidBlock;
			float? surfaceHeight = this.m_subsystemFluidBlockBehavior.GetSurfaceHeight(x, y, z, out fluidBlock);
			if (surfaceHeight != null)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
				this.ImmersionDepth = MathUtils.Max(surfaceHeight.Value - position.Y, 0f);
				this.ImmersionFactor = MathUtils.Saturate(MathUtils.Pow(this.ImmersionDepth / this.BoxSize.Y, 0.7f));
				this.ImmersionFluidBlock = BlocksManager.FluidBlocks[Terrain.ExtractContents(cellValue)];
				return;
			}
			this.ImmersionDepth = 0f;
			this.ImmersionFactor = 0f;
			this.ImmersionFluidBlock = null;
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x00064964 File Offset: 0x00062B64
		public bool MoveToFreeSpace()
		{
			Vector3 boxSize = this.BoxSize;
			Vector3 position = base.Position;
			for (int i = 0; i < ComponentBody.m_freeSpaceOffsets.Length; i++)
			{
				Vector3? vector = null;
				Vector3 vector2 = position + ComponentBody.m_freeSpaceOffsets[i];
				if (!(Terrain.ToCell(vector2) != Terrain.ToCell(position)))
				{
					BoundingBox box = new BoundingBox(vector2 - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), vector2 + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
					box.Min += new Vector3(0.01f, this.MaxSmoothRiseHeight + 0.01f, 0.01f);
					box.Max -= new Vector3(0.01f);
					this.m_collisionBoxes.Clear();
					this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
					this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
					this.m_collisionBoxes.AddRange(this.m_bodiesCollisionBoxes);
					if (!this.IsColliding(box, this.m_collisionBoxes))
					{
						vector = new Vector3?(vector2);
					}
					else
					{
						this.m_stoppedTime = 0f;
						ComponentBody.CollisionBox collisionBox;
						float num = this.CalculatePushBack(box, 0, this.m_collisionBoxes, out collisionBox);
						ComponentBody.CollisionBox collisionBox2;
						float num2 = this.CalculatePushBack(box, 1, this.m_collisionBoxes, out collisionBox2);
						ComponentBody.CollisionBox collisionBox3;
						float num3 = this.CalculatePushBack(box, 2, this.m_collisionBoxes, out collisionBox3);
						float num4 = num * num;
						float num5 = num2 * num2;
						float num6 = num3 * num3;
						List<Vector3> list = new List<Vector3>();
						if (num4 <= num5 && num4 <= num6)
						{
							list.Add(vector2 + new Vector3(num, 0f, 0f));
							if (num5 <= num6)
							{
								list.Add(vector2 + new Vector3(0f, num2, 0f));
								list.Add(vector2 + new Vector3(0f, 0f, num3));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, 0f, num3));
								list.Add(vector2 + new Vector3(0f, num2, 0f));
							}
						}
						else if (num5 <= num4 && num5 <= num6)
						{
							list.Add(vector2 + new Vector3(0f, num2, 0f));
							if (num4 <= num6)
							{
								list.Add(vector2 + new Vector3(num, 0f, 0f));
								list.Add(vector2 + new Vector3(0f, 0f, num3));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, 0f, num3));
								list.Add(vector2 + new Vector3(num, 0f, 0f));
							}
						}
						else
						{
							list.Add(vector2 + new Vector3(0f, 0f, num3));
							if (num4 <= num5)
							{
								list.Add(vector2 + new Vector3(num, 0f, 0f));
								list.Add(vector2 + new Vector3(0f, num2, 0f));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, num2, 0f));
								list.Add(vector2 + new Vector3(num, 0f, 0f));
							}
						}
						foreach (Vector3 vector3 in list)
						{
							box = new BoundingBox(vector3 - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), vector3 + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
							box.Min += new Vector3(0.02f, this.MaxSmoothRiseHeight + 0.02f, 0.02f);
							box.Max -= new Vector3(0.02f);
							this.m_collisionBoxes.Clear();
							this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
							this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
							this.m_collisionBoxes.AddRange(this.m_bodiesCollisionBoxes);
							if (!this.IsColliding(box, this.m_collisionBoxes))
							{
								vector = new Vector3?(vector3);
								break;
							}
						}
					}
					if (vector != null)
					{
						base.Position = vector.Value;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x00064EB8 File Offset: 0x000630B8
		public void MoveWithCollision(float dt, Vector3 move)
		{
			Vector3 position = base.Position;
			bool isSmoothRising = this.IsSmoothRiseEnabled && this.MaxSmoothRiseHeight > 0f && this.HandleSmoothRise(ref move, position, dt);
			this.HandleAxisCollision(1, move.Y, ref position, isSmoothRising);
			this.HandleAxisCollision(0, move.X, ref position, isSmoothRising);
			this.HandleAxisCollision(2, move.Z, ref position, isSmoothRising);
			base.Position = position;
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00064F28 File Offset: 0x00063128
		public bool HandleSmoothRise(ref Vector3 move, Vector3 position, float dt)
		{
			Vector3 boxSize = this.BoxSize;
			BoundingBox box = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			box.Min += new Vector3(0.04f, 0f, 0.04f);
			box.Max -= new Vector3(0.04f, 0f, 0.04f);
			this.m_collisionBoxes.Clear();
			this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
			this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
			ComponentBody.CollisionBox collisionBox;
			float num = MathUtils.Max(this.CalculatePushBack(box, 1, this.m_collisionBoxes, out collisionBox), 0f);
			if (!BlocksManager.Blocks[Terrain.ExtractContents(collisionBox.BlockValue)].NoSmoothRise && num > 0.04f)
			{
				float x = MathUtils.Min(4.5f * dt, num);
				move.Y = MathUtils.Max(move.Y, x);
				this.m_velocity.Y = MathUtils.Max(this.m_velocity.Y, 0f);
				this.StandingOnValue = new int?(collisionBox.BlockValue);
				this.StandingOnBody = collisionBox.ComponentBody;
				this.m_stoppedTime = 0f;
				return true;
			}
			return false;
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x000650BC File Offset: 0x000632BC
		public void HandleAxisCollision(int axis, float move, ref Vector3 position, bool isSmoothRising)
		{
			Vector3 boxSize = this.BoxSize;
			this.m_collisionBoxes.Clear();
			if (this.IsSneaking && axis != 1)
			{
				this.FindSneakCollisionBoxes(position, new Vector2(boxSize.X - 0.08f, boxSize.Z - 0.08f), this.m_collisionBoxes);
			}
			Vector3 v;
			if (axis != 0)
			{
				if (axis != 1)
				{
					position.Z += move;
					v = new Vector3(0.04f, 0.04f, 0f);
				}
				else
				{
					position.Y += move;
					v = new Vector3(0.04f, 0f, 0.04f);
				}
			}
			else
			{
				position.X += move;
				v = new Vector3(0f, 0.04f, 0.04f);
			}
			BoundingBox boundingBox = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f) + v, position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f) - v);
			this.FindTerrainCollisionBoxes(boundingBox, this.m_collisionBoxes);
			this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
			ComponentBody.CollisionBox collisionBox;
			float num;
			if (axis != 1 || isSmoothRising)
			{
				BoundingBox smoothRiseBox = boundingBox;
				smoothRiseBox.Min.Y = smoothRiseBox.Min.Y + this.MaxSmoothRiseHeight;
				num = this.CalculateSmoothRisePushBack(boundingBox, smoothRiseBox, axis, this.m_collisionBoxes, out collisionBox);
			}
			else
			{
				num = this.CalculatePushBack(boundingBox, axis, this.m_collisionBoxes, out collisionBox);
			}
			BoundingBox box = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f) + v, position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f) - v);
			ComponentBody.CollisionBox collisionBox2;
			float num2 = this.CalculatePushBack(box, axis, this.m_bodiesCollisionBoxes, out collisionBox2);
			if (MathUtils.Abs(num) > MathUtils.Abs(num2))
			{
				if (num == 0f)
				{
					return;
				}
				int num3 = Terrain.ExtractContents(collisionBox.BlockValue);
				if (BlocksManager.Blocks[num3].HasCollisionBehavior_(collisionBox.BlockValue))
				{
					SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num3);
					for (int i = 0; i < blockBehaviors.Length; i++)
					{
						Vector3 vector = (collisionBox.Box.Min + collisionBox.Box.Max) / 2f;
						CellFace cellFace = CellFace.FromAxisAndDirection(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Y), Terrain.ToCell(vector.Z), axis, 0f - ComponentBody.GetVectorComponent(this.m_velocity, axis));
						blockBehaviors[i].OnCollide(cellFace, ComponentBody.GetVectorComponent(this.m_velocity, axis), this);
					}
				}
				if (axis == 0)
				{
					position.X += num;
					this.m_velocity.X = collisionBox.BlockVelocity.X;
					return;
				}
				if (axis != 1)
				{
					position.Z += num;
					this.m_velocity.Z = collisionBox.BlockVelocity.Z;
					return;
				}
				position.Y += num;
				this.m_velocity.Y = collisionBox.BlockVelocity.Y;
				if (move < 0f)
				{
					this.StandingOnValue = new int?(collisionBox.BlockValue);
					this.StandingOnBody = collisionBox.ComponentBody;
					this.StandingOnVelocity = collisionBox.BlockVelocity;
					return;
				}
			}
			else
			{
				if (num2 == 0f)
				{
					return;
				}
				ComponentBody componentBody = collisionBox2.ComponentBody;
				if (axis != 0)
				{
					if (axis != 1)
					{
						ComponentBody.InelasticCollision(this.m_velocity.Z, componentBody.m_velocity.Z, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.Z, out componentBody.m_velocity.Z);
						position.Z += num2;
					}
					else
					{
						ComponentBody.InelasticCollision(this.m_velocity.Y, componentBody.m_velocity.Y, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.Y, out componentBody.m_velocity.Y);
						position.Y += num2;
						if (move < 0f)
						{
							this.StandingOnValue = new int?(collisionBox2.BlockValue);
							this.StandingOnBody = collisionBox2.ComponentBody;
							this.StandingOnVelocity = new Vector3(componentBody.m_velocity.X, 0f, componentBody.m_velocity.Z);
						}
					}
				}
				else
				{
					ComponentBody.InelasticCollision(this.m_velocity.X, componentBody.m_velocity.X, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.X, out componentBody.m_velocity.X);
					position.X += num2;
				}
				Action<ComponentBody> collidedWithBody = this.CollidedWithBody;
				if (collidedWithBody != null)
				{
					collidedWithBody(componentBody);
				}
				Action<ComponentBody> collidedWithBody2 = componentBody.CollidedWithBody;
				if (collidedWithBody2 == null)
				{
					return;
				}
				collidedWithBody2(this);
			}
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00065604 File Offset: 0x00063804
		public void FindBodiesCollisionBoxes(Vector3 position, DynamicArray<ComponentBody.CollisionBox> result)
		{
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), 4f, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (componentBody != this && componentBody != this.m_parentBody && componentBody.m_parentBody != this)
				{
					result.Add(new ComponentBody.CollisionBox
					{
						Box = componentBody.BoundingBox,
						ComponentBody = componentBody
					});
				}
			}
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x000656A4 File Offset: 0x000638A4
		public void FindMovingBlocksCollisionBoxes(Vector3 position, DynamicArray<ComponentBody.CollisionBox> result)
		{
			Vector3 boxSize = this.BoxSize;
			BoundingBox boundingBox = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			boundingBox.Min -= new Vector3(1f);
			boundingBox.Max += new Vector3(1f);
			this.m_movingBlockSets.Clear();
			this.m_subsystemMovingBlocks.FindMovingBlocks(boundingBox, false, this.m_movingBlockSets);
			for (int i = 0; i < this.m_movingBlockSets.Count; i++)
			{
				IMovingBlockSet movingBlockSet = this.m_movingBlockSets.Array[i];
				for (int j = 0; j < movingBlockSet.Blocks.Count; j++)
				{
					MovingBlock movingBlock = movingBlockSet.Blocks[j];
					int num = Terrain.ExtractContents(movingBlock.Value);
					Block block = BlocksManager.Blocks[num];
					if (block.IsCollidable_(movingBlock.Value))
					{
						BoundingBox[] customCollisionBoxes = block.GetCustomCollisionBoxes(this.m_subsystemTerrain, movingBlock.Value);
						Vector3 v = new Vector3(movingBlock.Offset) + movingBlockSet.Position;
						for (int k = 0; k < customCollisionBoxes.Length; k++)
						{
							result.Add(new ComponentBody.CollisionBox
							{
								Box = new BoundingBox(v + customCollisionBoxes[k].Min, v + customCollisionBoxes[k].Max),
								BlockValue = movingBlock.Value,
								BlockVelocity = movingBlockSet.CurrentVelocity
							});
						}
					}
				}
			}
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x000658A0 File Offset: 0x00063AA0
		public void FindTerrainCollisionBoxes(BoundingBox box, DynamicArray<ComponentBody.CollisionBox> result)
		{
			Point3 point = Terrain.ToCell(box.Min);
			Point3 point2 = Terrain.ToCell(box.Max);
			point.Y = MathUtils.Max(point.Y, 0);
			point2.Y = MathUtils.Min(point2.Y, 255);
			if (point.Y > point2.Y)
			{
				return;
			}
			for (int i = point.X; i <= point2.X; i++)
			{
				for (int j = point.Z; j <= point2.Z; j++)
				{
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(i, j);
					if (chunkAtCell != null)
					{
						int num = TerrainChunk.CalculateCellIndex(i & 15, point.Y, j & 15);
						int k = point.Y;
						while (k <= point2.Y)
						{
							int cellValueFast = chunkAtCell.GetCellValueFast(num);
							int num2 = Terrain.ExtractContents(cellValueFast);
							if (num2 != 0)
							{
								Block block = BlocksManager.Blocks[num2];
								if (block.IsCollidable_(cellValueFast))
								{
									BoundingBox[] customCollisionBoxes = block.GetCustomCollisionBoxes(this.m_subsystemTerrain, cellValueFast);
									Vector3 v = new Vector3((float)i, (float)k, (float)j);
									for (int l = 0; l < customCollisionBoxes.Length; l++)
									{
										result.Add(new ComponentBody.CollisionBox
										{
											Box = new BoundingBox(v + customCollisionBoxes[l].Min, v + customCollisionBoxes[l].Max),
											BlockValue = cellValueFast
										});
									}
								}
							}
							k++;
							num++;
						}
					}
				}
			}
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x00065A40 File Offset: 0x00063C40
		public void FindSneakCollisionBoxes(Vector3 position, Vector2 overhang, DynamicArray<ComponentBody.CollisionBox> result)
		{
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(num, num2 - 1, num3);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsCollidable_(cellValue))
			{
				return;
			}
			bool flag = position.X < (float)num + 0.5f;
			bool flag2 = position.Z < (float)num3 + 0.5f;
			if (flag)
			{
				if (flag2)
				{
					int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(num, num2 - 1, num3 - 1);
					int cellValue3 = this.m_subsystemTerrain.Terrain.GetCellValue(num - 1, num2 - 1, num3);
					int cellValue4 = this.m_subsystemTerrain.Terrain.GetCellValue(num - 1, num2 - 1, num3 - 1);
					bool flag3 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsCollidable_(cellValue2);
					bool flag4 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue3)].IsCollidable_(cellValue3);
					bool flag5 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue4)].IsCollidable_(cellValue4);
					if ((flag3 && !flag4) || (!flag3 && !flag4 && flag5))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if ((!flag3 && flag4) || (!flag3 && !flag4 && flag5))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if (flag3 && flag4)
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
						return;
					}
				}
				else
				{
					int cellValue5 = this.m_subsystemTerrain.Terrain.GetCellValue(num, num2 - 1, num3 + 1);
					int cellValue6 = this.m_subsystemTerrain.Terrain.GetCellValue(num - 1, num2 - 1, num3);
					int cellValue7 = this.m_subsystemTerrain.Terrain.GetCellValue(num - 1, num2 - 1, num3 + 1);
					bool flag6 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue5)].IsCollidable_(cellValue5);
					bool flag7 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue6)].IsCollidable_(cellValue6);
					bool flag8 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue7)].IsCollidable_(cellValue7);
					if ((flag6 && !flag7) || (!flag6 && !flag7 && flag8))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
							BlockValue = 0
						};
						result.Add(item);
					}
					if ((!flag6 && flag7) || (!flag6 && !flag7 && flag8))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if (flag6 && flag7)
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
							BlockValue = 0
						};
						result.Add(item);
						return;
					}
				}
			}
			else if (flag2)
			{
				int cellValue8 = this.m_subsystemTerrain.Terrain.GetCellValue(num, num2 - 1, num3 - 1);
				int cellValue9 = this.m_subsystemTerrain.Terrain.GetCellValue(num + 1, num2 - 1, num3);
				int cellValue10 = this.m_subsystemTerrain.Terrain.GetCellValue(num + 1, num2 - 1, num3 - 1);
				bool flag9 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue8)].IsCollidable_(cellValue8);
				bool flag10 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue9)].IsCollidable_(cellValue9);
				bool flag11 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue10)].IsCollidable_(cellValue10);
				if ((flag9 && !flag10) || (!flag9 && !flag10 && flag11))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if ((!flag9 && flag10) || (!flag9 && !flag10 && flag11))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if (flag9 && flag10)
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
					return;
				}
			}
			else
			{
				int cellValue11 = this.m_subsystemTerrain.Terrain.GetCellValue(num, num2 - 1, num3 + 1);
				int cellValue12 = this.m_subsystemTerrain.Terrain.GetCellValue(num + 1, num2 - 1, num3);
				int cellValue13 = this.m_subsystemTerrain.Terrain.GetCellValue(num + 1, num2 - 1, num3 + 1);
				bool flag12 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue11)].IsCollidable_(cellValue11);
				bool flag13 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue12)].IsCollidable_(cellValue12);
				bool flag14 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue13)].IsCollidable_(cellValue13);
				if ((flag12 && !flag13) || (!flag12 && !flag13 && flag14))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
						BlockValue = 0
					};
					result.Add(item);
				}
				if ((!flag12 && flag13) || (!flag12 && !flag13 && flag14))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if (flag12 && flag13)
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
						BlockValue = 0
					};
					result.Add(item);
				}
			}
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x000661A8 File Offset: 0x000643A8
		public bool IsColliding(BoundingBox box, DynamicArray<ComponentBody.CollisionBox> collisionBoxes)
		{
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				if (box.Intersection(collisionBoxes.Array[i].Box))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x000661E4 File Offset: 0x000643E4
		public float CalculatePushBack(BoundingBox box, int axis, DynamicArray<ComponentBody.CollisionBox> collisionBoxes, out ComponentBody.CollisionBox pushingCollisionBox)
		{
			pushingCollisionBox = default(ComponentBody.CollisionBox);
			float num = 0f;
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				float num2 = ComponentBody.CalculateBoxBoxOverlap(ref box, ref collisionBoxes.Array[i].Box, axis);
				if (MathUtils.Abs(num2) > MathUtils.Abs(num))
				{
					num = num2;
					pushingCollisionBox = collisionBoxes.Array[i];
				}
			}
			return num;
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x00066250 File Offset: 0x00064450
		public float CalculateSmoothRisePushBack(BoundingBox normalBox, BoundingBox smoothRiseBox, int axis, DynamicArray<ComponentBody.CollisionBox> collisionBoxes, out ComponentBody.CollisionBox pushingCollisionBox)
		{
			pushingCollisionBox = default(ComponentBody.CollisionBox);
			float num = 0f;
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				float num2 = (!BlocksManager.Blocks[Terrain.ExtractContents(collisionBoxes.Array[i].BlockValue)].NoSmoothRise) ? ComponentBody.CalculateBoxBoxOverlap(ref smoothRiseBox, ref collisionBoxes.Array[i].Box, axis) : ComponentBody.CalculateBoxBoxOverlap(ref normalBox, ref collisionBoxes.Array[i].Box, axis);
				if (MathUtils.Abs(num2) > MathUtils.Abs(num))
				{
					num = num2;
					pushingCollisionBox = collisionBoxes.Array[i];
				}
			}
			return num;
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x00066304 File Offset: 0x00064504
		public static float CalculateBoxBoxOverlap(ref BoundingBox b1, ref BoundingBox b2, int axis)
		{
			if (b1.Max.X <= b2.Min.X || b1.Min.X >= b2.Max.X || b1.Max.Y <= b2.Min.Y || b1.Min.Y >= b2.Max.Y || b1.Max.Z <= b2.Min.Z || b1.Min.Z >= b2.Max.Z)
			{
				return 0f;
			}
			if (axis == 0)
			{
				float num = b1.Min.X + b1.Max.X;
				float num2 = b2.Min.X + b2.Max.X;
				float num3 = b1.Max.X - b1.Min.X;
				float num4 = b2.Max.X - b2.Min.X;
				float num5 = num2 - num;
				float num6 = num3 + num4;
				return 0.5f * ((num5 > 0f) ? (num5 - num6) : (num5 + num6));
			}
			if (axis != 1)
			{
				float num7 = b1.Min.Z + b1.Max.Z;
				float num8 = b2.Min.Z + b2.Max.Z;
				float num9 = b1.Max.Z - b1.Min.Z;
				float num10 = b2.Max.Z - b2.Min.Z;
				float num11 = num8 - num7;
				float num12 = num9 + num10;
				return 0.5f * ((num11 > 0f) ? (num11 - num12) : (num11 + num12));
			}
			float num13 = b1.Min.Y + b1.Max.Y;
			float num14 = b2.Min.Y + b2.Max.Y;
			float num15 = b1.Max.Y - b1.Min.Y;
			float num16 = b2.Max.Y - b2.Min.Y;
			float num17 = num14 - num13;
			float num18 = num15 + num16;
			return 0.5f * ((num17 > 0f) ? (num17 - num18) : (num17 + num18));
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x00066549 File Offset: 0x00064749
		public static float GetVectorComponent(Vector3 v, int axis)
		{
			if (axis == 0)
			{
				return v.X;
			}
			if (axis != 1)
			{
				return v.Z;
			}
			return v.Y;
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00066568 File Offset: 0x00064768
		public static void InelasticCollision(float v1, float v2, float m1, float m2, float cr, out float result1, out float result2)
		{
			float num = 1f / (m1 + m2);
			result1 = (cr * m2 * (v2 - v1) + m1 * v1 + m2 * v2) * num;
			result2 = (cr * m1 * (v1 - v2) + m1 * v1 + m2 * v2) * num;
		}

		// Token: 0x04000731 RID: 1841
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000732 RID: 1842
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000733 RID: 1843
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000734 RID: 1844
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x04000735 RID: 1845
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000736 RID: 1846
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000737 RID: 1847
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000738 RID: 1848
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x04000739 RID: 1849
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400073A RID: 1850
		public DynamicArray<ComponentBody.CollisionBox> m_collisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x0400073B RID: 1851
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x0400073C RID: 1852
		public DynamicArray<IMovingBlockSet> m_movingBlockSets = new DynamicArray<IMovingBlockSet>();

		// Token: 0x0400073D RID: 1853
		public DynamicArray<ComponentBody.CollisionBox> m_bodiesCollisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x0400073E RID: 1854
		public DynamicArray<ComponentBody.CollisionBox> m_movingBlocksCollisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x0400073F RID: 1855
		public ComponentBody m_parentBody;

		// Token: 0x04000740 RID: 1856
		public List<ComponentBody> m_childBodies = new List<ComponentBody>();

		// Token: 0x04000741 RID: 1857
		public Vector3 m_velocity;

		// Token: 0x04000742 RID: 1858
		public bool m_isSneaking;

		// Token: 0x04000743 RID: 1859
		public Vector3 m_totalImpulse;

		// Token: 0x04000744 RID: 1860
		public Vector3 m_directMove;

		// Token: 0x04000745 RID: 1861
		public bool m_fluidEffectsPlayed;

		// Token: 0x04000746 RID: 1862
		public float m_stoppedTime;

		// Token: 0x04000747 RID: 1863
		public static Vector3[] m_freeSpaceOffsets;

		// Token: 0x04000748 RID: 1864
		public static bool DrawBodiesBounds;

		// Token: 0x04000749 RID: 1865
		public const float SleepThresholdSpeed = 1E-05f;

		// Token: 0x0400074A RID: 1866
		public const float MaxSpeed = 25f;

		// Token: 0x020004BC RID: 1212
		public struct CollisionBox
		{
			// Token: 0x04001770 RID: 6000
			public int BlockValue;

			// Token: 0x04001771 RID: 6001
			public Vector3 BlockVelocity;

			// Token: 0x04001772 RID: 6002
			public ComponentBody ComponentBody;

			// Token: 0x04001773 RID: 6003
			public BoundingBox Box;
		}
	}
}
