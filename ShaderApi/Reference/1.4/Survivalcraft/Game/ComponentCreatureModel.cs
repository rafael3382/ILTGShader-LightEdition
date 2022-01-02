using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F9 RID: 505
	public class ComponentCreatureModel : ComponentModel, IUpdateable
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0006A6BB File Offset: 0x000688BB
		// (set) Token: 0x06000E81 RID: 3713 RVA: 0x0006A6C3 File Offset: 0x000688C3
		public float Bob { get; set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x0006A6CC File Offset: 0x000688CC
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x0006A6D4 File Offset: 0x000688D4
		public float MovementAnimationPhase { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x0006A6DD File Offset: 0x000688DD
		// (set) Token: 0x06000E85 RID: 3717 RVA: 0x0006A6E5 File Offset: 0x000688E5
		public float DeathPhase { get; set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0006A6EE File Offset: 0x000688EE
		// (set) Token: 0x06000E87 RID: 3719 RVA: 0x0006A6F6 File Offset: 0x000688F6
		public Vector3 DeathCauseOffset { get; set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000E88 RID: 3720 RVA: 0x0006A6FF File Offset: 0x000688FF
		// (set) Token: 0x06000E89 RID: 3721 RVA: 0x0006A707 File Offset: 0x00068907
		public Vector3? LookAtOrder { get; set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000E8A RID: 3722 RVA: 0x0006A710 File Offset: 0x00068910
		// (set) Token: 0x06000E8B RID: 3723 RVA: 0x0006A718 File Offset: 0x00068918
		public bool LookRandomOrder { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000E8C RID: 3724 RVA: 0x0006A721 File Offset: 0x00068921
		// (set) Token: 0x06000E8D RID: 3725 RVA: 0x0006A729 File Offset: 0x00068929
		public float HeadShakeOrder { get; set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000E8E RID: 3726 RVA: 0x0006A732 File Offset: 0x00068932
		// (set) Token: 0x06000E8F RID: 3727 RVA: 0x0006A73A File Offset: 0x0006893A
		public bool AttackOrder { get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000E90 RID: 3728 RVA: 0x0006A743 File Offset: 0x00068943
		// (set) Token: 0x06000E91 RID: 3729 RVA: 0x0006A74B File Offset: 0x0006894B
		public bool FeedOrder { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x0006A754 File Offset: 0x00068954
		// (set) Token: 0x06000E93 RID: 3731 RVA: 0x0006A75C File Offset: 0x0006895C
		public bool RowLeftOrder { get; set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000E94 RID: 3732 RVA: 0x0006A765 File Offset: 0x00068965
		// (set) Token: 0x06000E95 RID: 3733 RVA: 0x0006A76D File Offset: 0x0006896D
		public bool RowRightOrder { get; set; }

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0006A776 File Offset: 0x00068976
		// (set) Token: 0x06000E97 RID: 3735 RVA: 0x0006A77E File Offset: 0x0006897E
		public float AimHandAngleOrder { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000E98 RID: 3736 RVA: 0x0006A787 File Offset: 0x00068987
		// (set) Token: 0x06000E99 RID: 3737 RVA: 0x0006A78F File Offset: 0x0006898F
		public Vector3 InHandItemOffsetOrder { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000E9A RID: 3738 RVA: 0x0006A798 File Offset: 0x00068998
		// (set) Token: 0x06000E9B RID: 3739 RVA: 0x0006A7A0 File Offset: 0x000689A0
		public Vector3 InHandItemRotationOrder { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0006A7A9 File Offset: 0x000689A9
		// (set) Token: 0x06000E9D RID: 3741 RVA: 0x0006A7B1 File Offset: 0x000689B1
		public bool IsAttackHitMoment { get; set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000E9E RID: 3742 RVA: 0x0006A7BA File Offset: 0x000689BA
		public Vector3 EyePosition
		{
			get
			{
				if (this.m_eyePosition == null)
				{
					this.m_eyePosition = new Vector3?(this.CalculateEyePosition());
				}
				return this.m_eyePosition.Value;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000E9F RID: 3743 RVA: 0x0006A7E5 File Offset: 0x000689E5
		public Quaternion EyeRotation
		{
			get
			{
				if (this.m_eyeRotation == null)
				{
					this.m_eyeRotation = new Quaternion?(this.CalculateEyeRotation());
				}
				return this.m_eyeRotation.Value;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x0006A810 File Offset: 0x00068A10
		public UpdateOrder UpdateOrder
		{
			get
			{
				ComponentBody parentBody = this.m_componentCreature.ComponentBody.ParentBody;
				if (parentBody != null)
				{
					ComponentCreatureModel componentCreatureModel = parentBody.Entity.FindComponent<ComponentCreatureModel>();
					if (componentCreatureModel != null)
					{
						return componentCreatureModel.UpdateOrder + 1;
					}
				}
				return UpdateOrder.CreatureModels;
			}
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0006A84C File Offset: 0x00068A4C
		public override void Animate()
		{
			base.Opacity = new float?((this.m_componentCreature.ComponentSpawn.SpawnDuration > 0f) ? ((float)MathUtils.Saturate((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentCreature.ComponentSpawn.SpawnTime) / (double)this.m_componentCreature.ComponentSpawn.SpawnDuration)) : 1f);
			if (this.m_componentCreature.ComponentSpawn.DespawnTime != null)
			{
				base.Opacity = new float?(MathUtils.Min(base.Opacity.Value, (float)MathUtils.Saturate(1.0 - (this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentCreature.ComponentSpawn.DespawnTime.Value) / (double)this.m_componentCreature.ComponentSpawn.DespawnDuration)));
			}
			base.DiffuseColor = new Vector3?(Vector3.Lerp(Vector3.One, new Vector3(1f, 0f, 0f), this.m_injuryColorFactor));
			if (base.Opacity != null && base.Opacity.Value < 1f)
			{
				bool flag = this.m_componentCreature.ComponentBody.ImmersionFactor >= 1f;
				bool flag2 = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
				this.RenderingMode = ((flag == flag2) ? ModelRenderingMode.TransparentAfterWater : ModelRenderingMode.TransparentBeforeWater);
				return;
			}
			this.RenderingMode = ModelRenderingMode.Solid;
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x0006A9CC File Offset: 0x00068BCC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			ComponentHealth componentHealth = this.m_componentCreature.ComponentHealth;
			componentHealth.Attacked = (Action<ComponentCreature>)Delegate.Combine(componentHealth.Attacked, new Action<ComponentCreature>(delegate(ComponentCreature attacker)
			{
				if (this.DeathPhase == 0f && this.m_componentCreature.ComponentHealth.Health == 0f)
				{
					this.DeathCauseOffset = attacker.ComponentBody.BoundingBox.Center() - this.m_componentCreature.ComponentBody.BoundingBox.Center();
				}
			}));
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x0006AA58 File Offset: 0x00068C58
		public override void OnEntityAdded()
		{
			ComponentBody componentBody = this.m_componentCreature.ComponentBody;
			componentBody.PositionChanged = (Action<ComponentFrame>)Delegate.Combine(componentBody.PositionChanged, new Action<ComponentFrame>(delegate(ComponentFrame <p0>)
			{
				this.m_eyePosition = null;
			}));
			ComponentBody componentBody2 = this.m_componentCreature.ComponentBody;
			componentBody2.RotationChanged = (Action<ComponentFrame>)Delegate.Combine(componentBody2.RotationChanged, new Action<ComponentFrame>(delegate(ComponentFrame <p0>)
			{
				this.m_eyeRotation = null;
			}));
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0006AAC0 File Offset: 0x00068CC0
		public virtual void Update(float dt)
		{
			if (this.LookRandomOrder)
			{
				Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
				Vector3 v = Vector3.Normalize(this.m_randomLookPoint - this.m_componentCreature.ComponentCreatureModel.EyePosition);
				if (this.m_random.Float(0f, 1f) < 0.25f * dt || Vector3.Dot(matrix.Forward, v) < 0.2f)
				{
					float s = this.m_random.Float(-5f, 5f);
					float s2 = this.m_random.Float(-1f, 1f);
					float s3 = this.m_random.Float(3f, 8f);
					this.m_randomLookPoint = this.m_componentCreature.ComponentCreatureModel.EyePosition + s3 * matrix.Forward + s2 * matrix.Up + s * matrix.Right;
				}
				this.LookAtOrder = new Vector3?(this.m_randomLookPoint);
			}
			if (this.LookAtOrder != null)
			{
				Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
				Vector3 vector = this.LookAtOrder.Value - this.m_componentCreature.ComponentCreatureModel.EyePosition;
				float x = Vector2.Angle(new Vector2(forward.X, forward.Z), new Vector2(vector.X, vector.Z));
				float y = MathUtils.Asin(0.99f * Vector3.Normalize(vector).Y);
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(x, y) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			if (this.HeadShakeOrder > 0f)
			{
				this.HeadShakeOrder = MathUtils.Max(this.HeadShakeOrder - dt, 0f);
				float num = 1f * MathUtils.Saturate(4f * this.HeadShakeOrder);
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(num * (float)MathUtils.Sin(16.0 * this.m_subsystemTime.GameTime + (double)(0.01f * (float)this.GetHashCode())), 0f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			if (this.m_componentCreature.ComponentHealth.Health == 0f)
			{
				this.DeathPhase = MathUtils.Min(this.DeathPhase + 3f * dt, 1f);
			}
			if (this.m_componentCreature.ComponentHealth.HealthChange < 0f)
			{
				this.m_injuryColorFactor = 1f;
			}
			this.m_injuryColorFactor = MathUtils.Saturate(this.m_injuryColorFactor - 3f * dt);
			this.m_eyePosition = null;
			this.m_eyeRotation = null;
			this.LookRandomOrder = false;
			this.LookAtOrder = null;
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0006ADE8 File Offset: 0x00068FE8
		public virtual Vector3 CalculateEyePosition()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			return this.m_componentCreature.ComponentBody.Position + matrix.Up * 0.95f * this.m_componentCreature.ComponentBody.BoxSize.Y + matrix.Forward * 0.45f * this.m_componentCreature.ComponentBody.BoxSize.Z;
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0006AE78 File Offset: 0x00069078
		public virtual Quaternion CalculateEyeRotation()
		{
			return this.m_componentCreature.ComponentBody.Rotation * Quaternion.CreateFromYawPitchRoll(0f - this.m_componentCreature.ComponentLocomotion.LookAngles.X, this.m_componentCreature.ComponentLocomotion.LookAngles.Y, 0f);
		}

		// Token: 0x040007C9 RID: 1993
		public SubsystemTime m_subsystemTime;

		// Token: 0x040007CA RID: 1994
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040007CB RID: 1995
		public ComponentCreature m_componentCreature;

		// Token: 0x040007CC RID: 1996
		public Vector3? m_eyePosition;

		// Token: 0x040007CD RID: 1997
		public Quaternion? m_eyeRotation;

		// Token: 0x040007CE RID: 1998
		public float m_injuryColorFactor;

		// Token: 0x040007CF RID: 1999
		public Vector3 m_randomLookPoint;

		// Token: 0x040007D0 RID: 2000
		public Game.Random m_random = new Game.Random();
	}
}
