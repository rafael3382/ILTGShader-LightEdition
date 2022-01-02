using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EC RID: 492
	public class ComponentBirdModel : ComponentCreatureModel
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000D85 RID: 3461 RVA: 0x000621FF File Offset: 0x000603FF
		// (set) Token: 0x06000D86 RID: 3462 RVA: 0x00062207 File Offset: 0x00060407
		public float FlyPhase { get; set; }

		// Token: 0x06000D87 RID: 3463 RVA: 0x00062210 File Offset: 0x00060410
		public override void Update(float dt)
		{
			float num = Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward);
			if (MathUtils.Abs(num) > 0.1f)
			{
				base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
			}
			else
			{
				float num2 = MathUtils.Floor(base.MovementAnimationPhase);
				if (base.MovementAnimationPhase != num2)
				{
					base.MovementAnimationPhase = ((base.MovementAnimationPhase - num2 > 0.5f) ? MathUtils.Min(base.MovementAnimationPhase + 2f * dt, num2 + 1f) : MathUtils.Max(base.MovementAnimationPhase - 2f * dt, num2));
				}
			}
			float num3 = (0f - this.m_walkBobHeight) * MathUtils.Sqr(MathUtils.Sin(6.28318548f * base.MovementAnimationPhase));
			float num4 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
			base.Bob += num4 * (num3 - base.Bob);
			if (this.m_hasWings)
			{
				if (this.m_componentCreature.ComponentLocomotion.LastFlyOrder != null)
				{
					float num5 = (this.m_componentCreature.ComponentLocomotion.LastFlyOrder.Value.LengthSquared() > 0.99f) ? 1.5f : 1f;
					this.FlyPhase = MathUtils.Remainder(this.FlyPhase + this.m_flyAnimationSpeed * num5 * dt, 1f);
					if (this.m_componentCreature.ComponentLocomotion.LastFlyOrder.Value.Y < -0.1f && this.m_componentCreature.ComponentBody.Velocity.Length() > 4f)
					{
						this.FlyPhase = 0.72f;
					}
				}
				else if (this.FlyPhase != 1f)
				{
					this.FlyPhase = MathUtils.Min(this.FlyPhase + this.m_flyAnimationSpeed * dt, 1f);
				}
			}
			if (base.FeedOrder)
			{
				this.m_peckPhase += this.m_peckAnimationSpeed * dt;
				if (this.m_peckPhase > 0.75f)
				{
					this.m_peckPhase -= 0.5f;
				}
			}
			else if (this.m_peckPhase != 0f)
			{
				this.m_peckPhase = MathUtils.Remainder(MathUtils.Min(this.m_peckPhase + this.m_peckAnimationSpeed * dt, 1f), 1f);
			}
			base.FeedOrder = false;
			base.Update(dt);
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x000624B0 File Offset: 0x000606B0
		public override void Animate()
		{
			bool flag = false;
			bool skip = false;
			ModsManager.HookAction("OnModelAnimate", delegate(ModLoader loader)
			{
				loader.OnModelAnimate(this, out skip);
				flag |= skip;
				return false;
			});
			if (flag)
			{
				base.Animate();
				return;
			}
			float num = 0f;
			if (this.m_hasWings)
			{
				num += 1.2f * MathUtils.Sin(6.28318548f * (this.FlyPhase + 0.75f));
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
				{
					num += 0.3f * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase);
				}
			}
			float num2;
			float num3;
			if (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0f || this.m_componentCreature.ComponentLocomotion.FlySpeed == 0f)
			{
				num2 = 0.6f * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase);
				num3 = 0f - num2;
			}
			else
			{
				num3 = (num2 = 0f - MathUtils.DegToRad(60f));
			}
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float yaw = this.m_componentCreature.ComponentLocomotion.LookAngles.X / 2f;
				float yaw2 = this.m_componentCreature.ComponentLocomotion.LookAngles.X / 2f;
				float num4 = 0f;
				float num5 = 0f;
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0f)
				{
					num4 = 0.5f * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase / 2f);
					num5 = 0f - num4;
				}
				float num6 = MathUtils.Cos(6.28318548f * this.m_peckPhase);
				num4 -= 1.25f * (1f - ((num6 >= 0f) ? num6 : (-0.5f * num6)));
				num4 += this.m_componentCreature.ComponentLocomotion.LookAngles.Y;
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(vector.X, 0f, 0f) * Matrix.CreateTranslation(this.m_componentCreature.ComponentBody.Position + new Vector3(0f, base.Bob, 0f))));
				this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(yaw2, num4, 0f)));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(yaw, num5 + MathUtils.Clamp(vector.Y, -0.7853982f, 0.7853982f), vector.Z)));
				if (this.m_hasWings)
				{
					this.SetBoneTransform(this.m_wing1Bone.Index, new Matrix?(Matrix.CreateRotationY(num)));
					this.SetBoneTransform(this.m_wing2Bone.Index, new Matrix?(Matrix.CreateRotationY(0f - num)));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(num2)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(num3)));
			}
			else
			{
				float num7 = 1f - base.DeathPhase;
				float num8 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				Vector3 position = this.m_componentCreature.ComponentBody.Position + 0.5f * num8 * Vector3.Normalize(this.m_componentCreature.ComponentBody.Matrix.Forward * new Vector3(1f, 0f, 1f));
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(vector.X, 1.57079637f * base.DeathPhase, 0f) * Matrix.CreateTranslation(position)));
				this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.Identity));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.Identity));
				if (this.m_hasWings)
				{
					this.SetBoneTransform(this.m_wing1Bone.Index, new Matrix?(Matrix.CreateRotationY(num * num7)));
					this.SetBoneTransform(this.m_wing2Bone.Index, new Matrix?(Matrix.CreateRotationY((0f - num) * num7)));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(num2 * num7)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(num3 * num7)));
			}
			base.Animate();
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x000629F4 File Offset: 0x00060BF4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_flyAnimationSpeed = valuesDictionary.GetValue<float>("FlyAnimationSpeed");
			this.m_walkAnimationSpeed = valuesDictionary.GetValue<float>("WalkAnimationSpeed");
			this.m_peckAnimationSpeed = valuesDictionary.GetValue<float>("PeckAnimationSpeed");
			this.m_walkBobHeight = valuesDictionary.GetValue<float>("WalkBobHeight");
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x00062A50 File Offset: 0x00060C50
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_neckBone = base.Model.FindBone("Neck", true);
				this.m_headBone = base.Model.FindBone("Head", true);
				this.m_leg1Bone = base.Model.FindBone("Leg1", true);
				this.m_leg2Bone = base.Model.FindBone("Leg2", true);
				this.m_wing1Bone = base.Model.FindBone("Wing1", false);
				this.m_wing2Bone = base.Model.FindBone("Wing2", false);
			}
			else
			{
				this.m_bodyBone = null;
				this.m_neckBone = null;
				this.m_headBone = null;
				this.m_leg1Bone = null;
				this.m_leg2Bone = null;
				this.m_wing1Bone = null;
				this.m_wing2Bone = null;
			}
			this.m_hasWings = (this.m_wing1Bone != null && this.m_wing2Bone != null);
		}

		// Token: 0x0400070D RID: 1805
		public bool m_hasWings;

		// Token: 0x0400070E RID: 1806
		public ModelBone m_bodyBone;

		// Token: 0x0400070F RID: 1807
		public ModelBone m_neckBone;

		// Token: 0x04000710 RID: 1808
		public ModelBone m_headBone;

		// Token: 0x04000711 RID: 1809
		public ModelBone m_leg1Bone;

		// Token: 0x04000712 RID: 1810
		public ModelBone m_leg2Bone;

		// Token: 0x04000713 RID: 1811
		public ModelBone m_wing1Bone;

		// Token: 0x04000714 RID: 1812
		public ModelBone m_wing2Bone;

		// Token: 0x04000715 RID: 1813
		public float m_flyAnimationSpeed;

		// Token: 0x04000716 RID: 1814
		public float m_walkAnimationSpeed;

		// Token: 0x04000717 RID: 1815
		public float m_peckAnimationSpeed;

		// Token: 0x04000718 RID: 1816
		public float m_walkBobHeight;

		// Token: 0x04000719 RID: 1817
		public float m_peckPhase;
	}
}
