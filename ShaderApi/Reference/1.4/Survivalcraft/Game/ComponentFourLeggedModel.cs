using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000209 RID: 521
	public class ComponentFourLeggedModel : ComponentCreatureModel
	{
		// Token: 0x06000F4C RID: 3916 RVA: 0x0007118C File Offset: 0x0006F38C
		public override void Update(float dt)
		{
			float footstepsPhase = this.m_footstepsPhase;
			float num = this.m_componentCreature.ComponentLocomotion.SlipSpeed ?? Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward);
			if (this.m_canCanter && num > 0.7f * this.m_componentCreature.ComponentLocomotion.WalkSpeed)
			{
				this.m_gait = ComponentFourLeggedModel.Gait.Canter;
				base.MovementAnimationPhase += num * dt * 0.7f * this.m_walkAnimationSpeed;
				this.m_footstepsPhase += 0.7f * this.m_walkAnimationSpeed * num * dt;
			}
			else if (this.m_canTrot && num > 0.5f * this.m_componentCreature.ComponentLocomotion.WalkSpeed)
			{
				this.m_gait = ComponentFourLeggedModel.Gait.Trot;
				base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
				this.m_footstepsPhase += 1.25f * this.m_walkAnimationSpeed * num * dt;
			}
			else if (MathUtils.Abs(num) > 0.2f)
			{
				this.m_gait = ComponentFourLeggedModel.Gait.Walk;
				base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
				this.m_footstepsPhase += 1.25f * this.m_walkAnimationSpeed * num * dt;
			}
			else
			{
				this.m_gait = ComponentFourLeggedModel.Gait.Walk;
				base.MovementAnimationPhase = 0f;
				this.m_footstepsPhase = 0f;
			}
			float num2 = 0f;
			if (this.m_gait == ComponentFourLeggedModel.Gait.Canter)
			{
				num2 = (0f - this.m_walkBobHeight) * 1.5f * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase);
			}
			else if (this.m_gait == ComponentFourLeggedModel.Gait.Trot)
			{
				num2 = this.m_walkBobHeight * 1.5f * MathUtils.Sqr(MathUtils.Sin(6.28318548f * base.MovementAnimationPhase));
			}
			else if (this.m_gait == ComponentFourLeggedModel.Gait.Walk)
			{
				num2 = (0f - this.m_walkBobHeight) * MathUtils.Sqr(MathUtils.Sin(6.28318548f * base.MovementAnimationPhase));
			}
			float num3 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
			base.Bob += num3 * (num2 - base.Bob);
			if (this.m_gait == ComponentFourLeggedModel.Gait.Canter && this.m_useCanterSound)
			{
				float num4 = MathUtils.Floor(this.m_footstepsPhase);
				if (this.m_footstepsPhase > num4 && footstepsPhase <= num4)
				{
					string footstepSoundMaterialName = this.m_subsystemSoundMaterials.GetFootstepSoundMaterialName(this.m_componentCreature);
					if (!string.IsNullOrEmpty(footstepSoundMaterialName) && footstepSoundMaterialName != "Water")
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Footsteps/CanterDirt", 0.75f, this.m_random.Float(-0.25f, 0f), this.m_componentCreature.ComponentBody.Position, 3f, true);
					}
				}
			}
			else
			{
				float num5 = MathUtils.Floor(this.m_footstepsPhase);
				if (this.m_footstepsPhase > num5 && footstepsPhase <= num5)
				{
					this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
				}
			}
			this.m_feedFactor = (base.FeedOrder ? MathUtils.Min(this.m_feedFactor + 2f * dt, 1f) : MathUtils.Max(this.m_feedFactor - 2f * dt, 0f));
			base.IsAttackHitMoment = false;
			if (base.AttackOrder)
			{
				this.m_buttFactor = MathUtils.Min(this.m_buttFactor + 4f * dt, 1f);
				float buttPhase = this.m_buttPhase;
				this.m_buttPhase = MathUtils.Remainder(this.m_buttPhase + dt * 2f, 1f);
				if (buttPhase < 0.5f && this.m_buttPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
				}
			}
			else
			{
				this.m_buttFactor = MathUtils.Max(this.m_buttFactor - 4f * dt, 0f);
				if (this.m_buttPhase != 0f)
				{
					if (this.m_buttPhase > 0.5f)
					{
						this.m_buttPhase = MathUtils.Remainder(MathUtils.Min(this.m_buttPhase + dt * 2f, 1f), 1f);
					}
					else if (this.m_buttPhase > 0f)
					{
						this.m_buttPhase = MathUtils.Max(this.m_buttPhase - dt * 2f, 0f);
					}
				}
			}
			base.FeedOrder = false;
			base.AttackOrder = false;
			base.Update(dt);
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x00071618 File Offset: 0x0006F818
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
				float num4 = 0f;
				float num5 = 0f;
				if (base.MovementAnimationPhase != 0f && (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0f))
				{
					if (this.m_gait == ComponentFourLeggedModel.Gait.Canter)
					{
						float num6 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0f));
						float num7 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.25f));
						float num8 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.15f));
						float num9 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.4f));
						num = this.m_walkFrontLegsAngle * this.m_canterLegsAngleFactor * num6;
						num2 = this.m_walkFrontLegsAngle * this.m_canterLegsAngleFactor * num7;
						num3 = this.m_walkHindLegsAngle * this.m_canterLegsAngleFactor * num8;
						num4 = this.m_walkHindLegsAngle * this.m_canterLegsAngleFactor * num9;
						num5 = MathUtils.DegToRad(8f) * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase);
					}
					else if (this.m_gait == ComponentFourLeggedModel.Gait.Trot)
					{
						float num10 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0f));
						float num11 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.5f));
						float num12 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.5f));
						float num13 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0f));
						num = this.m_walkFrontLegsAngle * num10;
						num2 = this.m_walkFrontLegsAngle * num11;
						num3 = this.m_walkHindLegsAngle * num12;
						num4 = this.m_walkHindLegsAngle * num13;
						num5 = MathUtils.DegToRad(3f) * MathUtils.Sin(12.566371f * base.MovementAnimationPhase);
					}
					else
					{
						float num14 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0f));
						float num15 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.5f));
						float num16 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.25f));
						float num17 = MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + 0.75f));
						num = this.m_walkFrontLegsAngle * num14;
						num2 = this.m_walkFrontLegsAngle * num15;
						num3 = this.m_walkHindLegsAngle * num16;
						num4 = this.m_walkHindLegsAngle * num17;
						num5 = MathUtils.DegToRad(3f) * MathUtils.Sin(12.566371f * base.MovementAnimationPhase);
					}
				}
				float num18 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
				this.m_legAngle1 += num18 * (num - this.m_legAngle1);
				this.m_legAngle2 += num18 * (num2 - this.m_legAngle2);
				this.m_legAngle3 += num18 * (num3 - this.m_legAngle3);
				this.m_legAngle4 += num18 * (num4 - this.m_legAngle4);
				this.m_headAngleY += num18 * (num5 - this.m_headAngleY);
				Vector2 vector2 = this.m_componentCreature.ComponentLocomotion.LookAngles;
				vector2.Y += this.m_headAngleY;
				vector2.X = MathUtils.Clamp(vector2.X, 0f - MathUtils.DegToRad(65f), MathUtils.DegToRad(65f));
				vector2.Y = MathUtils.Clamp(vector2.Y, 0f - MathUtils.DegToRad(55f), MathUtils.DegToRad(55f));
				Vector2 vector3 = Vector2.Zero;
				if (this.m_neckBone != null)
				{
					vector3 = 0.6f * vector2;
					vector2 = 0.4f * vector2;
				}
				if (this.m_feedFactor > 0f)
				{
					float y = 0f - MathUtils.DegToRad(25f + 45f * SimplexNoise.OctavedNoise((float)this.m_subsystemTime.GameTime, 3f, 2, 2f, 0.75f, false));
					Vector2 v = new Vector2(0f, y);
					vector2 = Vector2.Lerp(vector2, v, this.m_feedFactor);
					if (this.m_moveLegWhenFeeding)
					{
						float x = MathUtils.DegToRad(20f) + MathUtils.PowSign(SimplexNoise.OctavedNoise((float)this.m_subsystemTime.GameTime, 1f, 1, 1f, 1f, false) - 0.5f, 0.33f) / 0.5f * MathUtils.DegToRad(25f) * (float)MathUtils.Sin(17.0 * this.m_subsystemTime.GameTime);
						num2 = MathUtils.Lerp(num2, x, this.m_feedFactor);
					}
				}
				if (this.m_buttFactor != 0f)
				{
					float y2 = (0f - MathUtils.DegToRad(40f)) * MathUtils.Sin(6.28318548f * MathUtils.Sigmoid(this.m_buttPhase, 4f));
					Vector2 v = new Vector2(0f, y2);
					vector2 = Vector2.Lerp(vector2, v, this.m_buttFactor);
				}
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateRotationY(vector.X) * Matrix.CreateTranslation(position.X, position.Y + base.Bob, position.Z)));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateRotationX(vector2.Y) * Matrix.CreateRotationZ(0f - vector2.X)));
				if (this.m_neckBone != null)
				{
					this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.CreateRotationX(vector3.Y) * Matrix.CreateRotationZ(0f - vector3.X)));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2)));
				this.SetBoneTransform(this.m_leg3Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle3)));
				this.SetBoneTransform(this.m_leg4Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle4)));
			}
			else
			{
				float num19 = 1f - base.DeathPhase;
				float num20 = (float)((Vector3.Dot(this.m_componentFrame.Matrix.Right, base.DeathCauseOffset) > 0f) ? 1 : -1);
				float num21 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateTranslation(-0.5f * num21 * Vector3.UnitY * base.DeathPhase) * Matrix.CreateFromYawPitchRoll(vector.X, 0f, 1.57079637f * base.DeathPhase * num20) * Matrix.CreateTranslation(0.2f * num21 * Vector3.UnitY * base.DeathPhase) * Matrix.CreateTranslation(position)));
				this.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateRotationX(MathUtils.DegToRad(50f) * base.DeathPhase)));
				if (this.m_neckBone != null)
				{
					this.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.Identity));
				}
				this.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1 * num19)));
				this.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2 * num19)));
				this.SetBoneTransform(this.m_leg3Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle3 * num19)));
				this.SetBoneTransform(this.m_leg4Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle4 * num19)));
			}
			base.Animate();
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x00071F10 File Offset: 0x00070110
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_walkAnimationSpeed = valuesDictionary.GetValue<float>("WalkAnimationSpeed");
			this.m_walkFrontLegsAngle = valuesDictionary.GetValue<float>("WalkFrontLegsAngle");
			this.m_walkHindLegsAngle = valuesDictionary.GetValue<float>("WalkHindLegsAngle");
			this.m_canterLegsAngleFactor = valuesDictionary.GetValue<float>("CanterLegsAngleFactor");
			this.m_walkBobHeight = valuesDictionary.GetValue<float>("WalkBobHeight");
			this.m_moveLegWhenFeeding = valuesDictionary.GetValue<bool>("MoveLegWhenFeeding");
			this.m_canCanter = valuesDictionary.GetValue<bool>("CanCanter");
			this.m_canTrot = valuesDictionary.GetValue<bool>("CanTrot");
			this.m_useCanterSound = valuesDictionary.GetValue<bool>("UseCanterSound");
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x00071FE4 File Offset: 0x000701E4
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
				this.m_leg3Bone = base.Model.FindBone("Leg3", true);
				this.m_leg4Bone = base.Model.FindBone("Leg4", true);
				return;
			}
			this.m_bodyBone = null;
			this.m_neckBone = null;
			this.m_headBone = null;
			this.m_leg1Bone = null;
			this.m_leg2Bone = null;
			this.m_leg3Bone = null;
			this.m_leg4Bone = null;
		}

		// Token: 0x040008B6 RID: 2230
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040008B7 RID: 2231
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x040008B8 RID: 2232
		public ModelBone m_bodyBone;

		// Token: 0x040008B9 RID: 2233
		public ModelBone m_neckBone;

		// Token: 0x040008BA RID: 2234
		public ModelBone m_headBone;

		// Token: 0x040008BB RID: 2235
		public ModelBone m_leg1Bone;

		// Token: 0x040008BC RID: 2236
		public ModelBone m_leg2Bone;

		// Token: 0x040008BD RID: 2237
		public ModelBone m_leg3Bone;

		// Token: 0x040008BE RID: 2238
		public ModelBone m_leg4Bone;

		// Token: 0x040008BF RID: 2239
		public float m_walkAnimationSpeed;

		// Token: 0x040008C0 RID: 2240
		public float m_canterLegsAngleFactor;

		// Token: 0x040008C1 RID: 2241
		public float m_walkFrontLegsAngle;

		// Token: 0x040008C2 RID: 2242
		public float m_walkHindLegsAngle;

		// Token: 0x040008C3 RID: 2243
		public float m_walkBobHeight;

		// Token: 0x040008C4 RID: 2244
		public bool m_moveLegWhenFeeding;

		// Token: 0x040008C5 RID: 2245
		public bool m_canCanter;

		// Token: 0x040008C6 RID: 2246
		public bool m_canTrot;

		// Token: 0x040008C7 RID: 2247
		public bool m_useCanterSound;

		// Token: 0x040008C8 RID: 2248
		public ComponentFourLeggedModel.Gait m_gait;

		// Token: 0x040008C9 RID: 2249
		public float m_feedFactor;

		// Token: 0x040008CA RID: 2250
		public float m_buttFactor;

		// Token: 0x040008CB RID: 2251
		public float m_buttPhase;

		// Token: 0x040008CC RID: 2252
		public float m_footstepsPhase;

		// Token: 0x040008CD RID: 2253
		public float m_legAngle1;

		// Token: 0x040008CE RID: 2254
		public float m_legAngle2;

		// Token: 0x040008CF RID: 2255
		public float m_legAngle3;

		// Token: 0x040008D0 RID: 2256
		public float m_legAngle4;

		// Token: 0x040008D1 RID: 2257
		public float m_headAngleY;

		// Token: 0x020004CB RID: 1227
		public enum Gait
		{
			// Token: 0x04001795 RID: 6037
			Walk,
			// Token: 0x04001796 RID: 6038
			Trot,
			// Token: 0x04001797 RID: 6039
			Canter
		}
	}
}
