using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000203 RID: 515
	public class ComponentFishModel : ComponentCreatureModel
	{
		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000F0B RID: 3851 RVA: 0x0006E6DB File Offset: 0x0006C8DB
		// (set) Token: 0x06000F0C RID: 3852 RVA: 0x0006E6E3 File Offset: 0x0006C8E3
		public float? BendOrder { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000F0D RID: 3853 RVA: 0x0006E6EC File Offset: 0x0006C8EC
		// (set) Token: 0x06000F0E RID: 3854 RVA: 0x0006E6F4 File Offset: 0x0006C8F4
		public float DigInOrder { get; set; }

		// Token: 0x06000F0F RID: 3855 RVA: 0x0006E700 File Offset: 0x0006C900
		public override void Update(float dt)
		{
			if (this.m_componentCreature.ComponentLocomotion.LastSwimOrder != null && this.m_componentCreature.ComponentLocomotion.LastSwimOrder.Value != Vector3.Zero)
			{
				float num = (this.m_componentCreature.ComponentLocomotion.LastSwimOrder.Value.LengthSquared() > 0.99f) ? 1.75f : 1f;
				base.MovementAnimationPhase = MathUtils.Remainder(base.MovementAnimationPhase + this.m_swimAnimationSpeed * num * dt, 1000f);
			}
			else
			{
				base.MovementAnimationPhase = MathUtils.Remainder(base.MovementAnimationPhase + 0.15f * this.m_swimAnimationSpeed * dt, 1000f);
			}
			if (this.BendOrder != null)
			{
				if (this.m_hasVerticalTail)
				{
					this.m_tailTurn.X = 0f;
					this.m_tailTurn.Y = this.BendOrder.Value;
				}
				else
				{
					this.m_tailTurn.X = this.BendOrder.Value;
					this.m_tailTurn.Y = 0f;
				}
			}
			else
			{
				this.m_tailTurn.X = this.m_tailTurn.X + MathUtils.Saturate(2f * this.m_componentCreature.ComponentLocomotion.TurnSpeed * dt) * (0f - this.m_componentCreature.ComponentLocomotion.LastTurnOrder.X - this.m_tailTurn.X);
			}
			if (this.DigInOrder > this.m_digInDepth)
			{
				float num2 = (this.DigInOrder - this.m_digInDepth) * MathUtils.Min(1.5f * dt, 1f);
				this.m_digInDepth += num2;
				this.m_digInTailPhase += 20f * num2;
			}
			else if (this.DigInOrder < this.m_digInDepth)
			{
				this.m_digInDepth += (this.DigInOrder - this.m_digInDepth) * MathUtils.Min(5f * dt, 1f);
			}
			float num3 = 0.33f * this.m_componentCreature.ComponentLocomotion.TurnSpeed;
			float num4 = 1f * this.m_componentCreature.ComponentLocomotion.TurnSpeed;
			base.IsAttackHitMoment = false;
			if (base.AttackOrder || base.FeedOrder)
			{
				if (base.AttackOrder)
				{
					this.m_tailWagPhase = MathUtils.Remainder(this.m_tailWagPhase + num3 * dt, 1f);
				}
				float bitingPhase = this.m_bitingPhase;
				this.m_bitingPhase = MathUtils.Remainder(this.m_bitingPhase + num4 * dt, 1f);
				if (base.AttackOrder && bitingPhase < 0.5f && this.m_bitingPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
				}
			}
			else
			{
				if (this.m_tailWagPhase != 0f)
				{
					this.m_tailWagPhase = MathUtils.Remainder(MathUtils.Min(this.m_tailWagPhase + num3 * dt, 1f), 1f);
				}
				if (this.m_bitingPhase != 0f)
				{
					this.m_bitingPhase = MathUtils.Remainder(MathUtils.Min(this.m_bitingPhase + num4 * dt, 1f), 1f);
				}
			}
			base.AttackOrder = false;
			base.FeedOrder = false;
			this.BendOrder = null;
			this.DigInOrder = 0f;
			base.Update(dt);
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0006EA64 File Offset: 0x0006CC64
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
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = this.m_digInTailPhase + this.m_tailWagPhase;
				float num2;
				float num3;
				float num4;
				float num5;
				if (this.m_hasVerticalTail)
				{
					num2 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.28318548f * num) - this.m_tailTurn.X, -1f, 1f);
					num3 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(2f * (3.14159274f * MathUtils.Max(num - 0.25f, 0f))) - this.m_tailTurn.X, -1f, 1f);
					num4 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.28318548f * base.MovementAnimationPhase) - this.m_tailTurn.Y, -1f, 1f);
					num5 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.28318548f * MathUtils.Max(base.MovementAnimationPhase - 0.25f, 0f)) - this.m_tailTurn.Y, -1f, 1f);
				}
				else
				{
					num2 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.28318548f * (base.MovementAnimationPhase + num)) - this.m_tailTurn.X, -1f, 1f);
					num3 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(2f * (3.14159274f * MathUtils.Max(base.MovementAnimationPhase + num - 0.25f, 0f))) - this.m_tailTurn.X, -1f, 1f);
					num4 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0f - this.m_tailTurn.Y, -1f, 1f);
					num5 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0f - this.m_tailTurn.Y, -1f, 1f);
				}
				float radians = 0f;
				if (this.m_bitingPhase > 0f)
				{
					radians = (0f - MathUtils.DegToRad(30f)) * MathUtils.Sin(3.14159274f * this.m_bitingPhase);
				}
				Matrix value = Matrix.CreateFromYawPitchRoll(vector.X, 0f, 0f) * Matrix.CreateTranslation(this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0f - this.m_digInDepth, 0f));
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(value));
				Matrix matrix = Matrix.Identity;
				if (num2 != 0f)
				{
					matrix *= Matrix.CreateRotationZ(num2);
				}
				if (num4 != 0f)
				{
					matrix *= Matrix.CreateRotationX(num4);
				}
				Matrix matrix2 = Matrix.Identity;
				if (num3 != 0f)
				{
					matrix2 *= Matrix.CreateRotationZ(num3);
				}
				if (num5 != 0f)
				{
					matrix2 *= Matrix.CreateRotationX(num5);
				}
				this.SetBoneTransform(this.m_tail1Bone.Index, new Matrix?(matrix));
				this.SetBoneTransform(this.m_tail2Bone.Index, new Matrix?(matrix2));
				if (this.m_jawBone != null)
				{
					this.SetBoneTransform(this.m_jawBone.Index, new Matrix?(Matrix.CreateRotationX(radians)));
				}
			}
			else
			{
				float num6 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				Vector3 position = this.m_componentCreature.ComponentBody.Position + 1f * num6 * base.DeathPhase * Vector3.UnitY;
				this.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(vector.X, 0f, 3.14159274f * base.DeathPhase) * Matrix.CreateTranslation(position)));
				this.SetBoneTransform(this.m_tail1Bone.Index, new Matrix?(Matrix.Identity));
				this.SetBoneTransform(this.m_tail2Bone.Index, new Matrix?(Matrix.Identity));
				if (this.m_jawBone != null)
				{
					this.SetBoneTransform(this.m_jawBone.Index, new Matrix?(Matrix.Identity));
				}
			}
			base.Animate();
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0006EF80 File Offset: 0x0006D180
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_hasVerticalTail = valuesDictionary.GetValue<bool>("HasVerticalTail");
			this.m_swimAnimationSpeed = valuesDictionary.GetValue<float>("SwimAnimationSpeed");
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0006EFAC File Offset: 0x0006D1AC
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_tail1Bone = base.Model.FindBone("Tail1", true);
				this.m_tail2Bone = base.Model.FindBone("Tail2", true);
				this.m_jawBone = base.Model.FindBone("Jaw", false);
				return;
			}
			this.m_bodyBone = null;
			this.m_tail1Bone = null;
			this.m_tail2Bone = null;
			this.m_jawBone = null;
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0006F044 File Offset: 0x0006D244
		public override Vector3 CalculateEyePosition()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			return this.m_componentCreature.ComponentBody.Position + matrix.Up * 1f * this.m_componentCreature.ComponentBody.BoxSize.Y + matrix.Forward * 0.45f * this.m_componentCreature.ComponentBody.BoxSize.Z;
		}

		// Token: 0x0400086D RID: 2157
		public ModelBone m_bodyBone;

		// Token: 0x0400086E RID: 2158
		public ModelBone m_tail1Bone;

		// Token: 0x0400086F RID: 2159
		public ModelBone m_tail2Bone;

		// Token: 0x04000870 RID: 2160
		public ModelBone m_jawBone;

		// Token: 0x04000871 RID: 2161
		public float m_swimAnimationSpeed;

		// Token: 0x04000872 RID: 2162
		public bool m_hasVerticalTail;

		// Token: 0x04000873 RID: 2163
		public float m_bitingPhase;

		// Token: 0x04000874 RID: 2164
		public float m_tailWagPhase;

		// Token: 0x04000875 RID: 2165
		public Vector2 m_tailTurn;

		// Token: 0x04000876 RID: 2166
		public float m_digInDepth;

		// Token: 0x04000877 RID: 2167
		public float m_digInTailPhase;
	}
}
