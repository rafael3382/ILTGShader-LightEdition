using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000205 RID: 517
	public class ComponentFlightlessBirdModel : ComponentCreatureModel
	{
		// Token: 0x06000F1E RID: 3870 RVA: 0x0006F4F0 File Offset: 0x0006D6F0
		public override void Update(float dt)
		{
			float footstepsPhase = this.m_footstepsPhase;
			float num = this.m_componentCreature.ComponentLocomotion.SlipSpeed ?? Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward);
			if (MathUtils.Abs(num) > 0.2f)
			{
				base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
				this.m_footstepsPhase += 1.25f * this.m_walkAnimationSpeed * num * dt;
			}
			else
			{
				base.MovementAnimationPhase = 0f;
				this.m_footstepsPhase = 0f;
			}
			float num2 = (0f - this.m_walkBobHeight) * MathUtils.Sqr(MathUtils.Sin(6.28318548f * base.MovementAnimationPhase));
			float num3 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
			base.Bob += num3 * (num2 - base.Bob);
			float num4 = MathUtils.Floor(this.m_footstepsPhase);
			if (this.m_footstepsPhase > num4 && footstepsPhase <= num4)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
			}
			this.m_feedFactor = (base.FeedOrder ? MathUtils.Min(this.m_feedFactor + 2f * dt, 1f) : MathUtils.Max(this.m_feedFactor - 2f * dt, 0f));
			base.IsAttackHitMoment = false;
			if (base.AttackOrder)
			{
				this.m_kickFactor = MathUtils.Min(this.m_kickFactor + 6f * dt, 1f);
				float kickPhase = this.m_kickPhase;
				this.m_kickPhase = MathUtils.Remainder(this.m_kickPhase + dt * 2f, 1f);
				if (kickPhase < 0.5f && this.m_kickPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
				}
			}
			else
			{
				this.m_kickFactor = MathUtils.Max(this.m_kickFactor - 6f * dt, 0f);
				if (this.m_kickPhase != 0f)
				{
					if (this.m_kickPhase > 0.5f)
					{
						this.m_kickPhase = MathUtils.Remainder(MathUtils.Min(this.m_kickPhase + dt * 2f, 1f), 1f);
					}
					else if (this.m_kickPhase > 0f)
					{
						this.m_kickPhase = MathUtils.Max(this.m_kickPhase - dt * 2f, 0f);
					}
				}
			}
			base.FeedOrder = false;
			base.AttackOrder = false;
			base.Update(dt);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0006F798 File Offset: 0x0006D998
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
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				if (base.MovementAnimationPhase != 0f && (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0f))
				{
					object obj = (Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward) > 0.75f * this.m_componentCreature.ComponentLocomotion.WalkSpeed) ? (1.5f * this.m_walkLegsAngle) : this.m_walkLegsAngle;
					float num4 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0f));
					float num5 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.5f));
					object obj2 = obj;
					num = obj2 * num4 + this.m_kickPhase;
					num2 = obj2 * num5;
					num3 = MathUtils.DegToRad(5f) * MathUtils.Sin(12.566371f * base.MovementAnimationPhase);
				}
				if (this.m_kickFactor != 0f)
				{
					float x = MathUtils.DegToRad(60f) * MathUtils.Sin(3.14159274f * MathUtils.Sigmoid(this.m_kickPhase, 5f));
					num = MathUtils.Lerp(num, x, this.m_kickFactor);
				}
				float num6 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
				this.m_legAngle1 += num6 * (num - this.m_legAngle1);
				this.m_legAngle2 += num6 * (num2 - this.m_legAngle2);
				this.m_headAngleY += num6 * (num3 - this.m_headAngleY);
				Vector2 vector2 = this.m_componentCreature.ComponentLocomotion.LookAngles;
				vector2.Y += this.m_headAngleY;
				if (this.m_feedFactor > 0f)
				{
					float y = 0f - MathUtils.DegToRad(35f + 55f * SimplexNoise.OctavedNoise((float)this.m_subsystemTime.GameTime, 3f, 2, 2f, 0.75f, false));
					Vector2 v = new Vector2(0f, y);
					vector2 = Vector2.Lerp(vector2, v, this.m_feedFactor);
				}
				vector2.X = MathUtils.Clamp(vector2.X, 0f - MathUtils.DegToRad(90f), MathUtils.DegToRad(90f));
				vector2.Y = MathUtils.Clamp(vector2.Y, 0f - MathUtils.DegToRad(90f), MathUtils.DegToRad(50f));
				Vector2 vector3 = Vector2.Zero;
				if (this.m_neckBone != null)
				{
					vector3 = 0.4f * vector2;
					vector2 = 0.6f * vector2;
				}
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateRotationY(vector.X) * Matrix.CreateTranslation(position.X, position.Y + base.Bob, position.Z)));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateRotationX(vector2.Y) * Matrix.CreateRotationZ(0f - vector2.X)));
				if (this.m_neckBone != null)
				{
					this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.CreateRotationX(vector3.Y) * Matrix.CreateRotationZ(0f - vector3.X)));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2)));
			}
			else
			{
				float num7 = 1f - base.DeathPhase;
				float num8 = (float)((Vector3.Dot(this.m_componentFrame.Matrix.Right, base.DeathCauseOffset) > 0f) ? 1 : -1);
				float num9 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateTranslation(-0.5f * num9 * base.DeathPhase * Vector3.UnitY) * Matrix.CreateFromYawPitchRoll(vector.X, 0f, 1.57079637f * base.DeathPhase * num8) * Matrix.CreateTranslation(0.2f * num9 * base.DeathPhase * Vector3.UnitY) * Matrix.CreateTranslation(position)));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.Identity));
				if (this.m_neckBone != null)
				{
					this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.Identity));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1 * num7)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2 * num7)));
			}
			base.Animate();
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0006FD85 File Offset: 0x0006DF85
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_walkAnimationSpeed = valuesDictionary.GetValue<float>("WalkAnimationSpeed");
			this.m_walkLegsAngle = valuesDictionary.GetValue<float>("WalkLegsAngle");
			this.m_walkBobHeight = valuesDictionary.GetValue<float>("WalkBobHeight");
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0006FDC4 File Offset: 0x0006DFC4
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_neckBone = base.Model.FindBone("Neck", false);
				this.m_headBone = base.Model.FindBone("Head", true);
				this.m_leg1Bone = base.Model.FindBone("Leg1", true);
				this.m_leg2Bone = base.Model.FindBone("Leg2", true);
				return;
			}
			this.m_bodyBone = null;
			this.m_neckBone = null;
			this.m_headBone = null;
			this.m_leg1Bone = null;
			this.m_leg2Bone = null;
		}

		// Token: 0x04000883 RID: 2179
		public ModelBone m_bodyBone;

		// Token: 0x04000884 RID: 2180
		public ModelBone m_neckBone;

		// Token: 0x04000885 RID: 2181
		public ModelBone m_headBone;

		// Token: 0x04000886 RID: 2182
		public ModelBone m_leg1Bone;

		// Token: 0x04000887 RID: 2183
		public ModelBone m_leg2Bone;

		// Token: 0x04000888 RID: 2184
		public float m_walkAnimationSpeed;

		// Token: 0x04000889 RID: 2185
		public float m_walkLegsAngle;

		// Token: 0x0400088A RID: 2186
		public float m_walkBobHeight;

		// Token: 0x0400088B RID: 2187
		public float m_feedFactor;

		// Token: 0x0400088C RID: 2188
		public float m_footstepsPhase;

		// Token: 0x0400088D RID: 2189
		public float m_kickFactor;

		// Token: 0x0400088E RID: 2190
		public float m_kickPhase;

		// Token: 0x0400088F RID: 2191
		public float m_legAngle1;

		// Token: 0x04000890 RID: 2192
		public float m_legAngle2;

		// Token: 0x04000891 RID: 2193
		public float m_headAngleY;
	}
}
