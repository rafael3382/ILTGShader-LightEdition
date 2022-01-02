using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000229 RID: 553
	public class ComponentRider : Component, IUpdateable
	{
		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060011AA RID: 4522 RVA: 0x0008312C File Offset: 0x0008132C
		// (set) Token: 0x060011AB RID: 4523 RVA: 0x00083134 File Offset: 0x00081334
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x060011AC RID: 4524 RVA: 0x0008313D File Offset: 0x0008133D
		public ComponentMount Mount
		{
			get
			{
				if (this.ComponentCreature.ComponentBody.ParentBody != null)
				{
					return this.ComponentCreature.ComponentBody.ParentBody.Entity.FindComponent<ComponentMount>();
				}
				return null;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060011AD RID: 4525 RVA: 0x0008316D File Offset: 0x0008136D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00083170 File Offset: 0x00081370
		public virtual ComponentMount FindNearestMount()
		{
			Vector2 point = new Vector2(this.ComponentCreature.ComponentBody.Position.X, this.ComponentCreature.ComponentBody.Position.Z);
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(point, 2.5f, this.m_componentBodies);
			float num = 0f;
			ComponentMount result = null;
			foreach (ComponentMount componentMount in from b in this.m_componentBodies
			select b.Entity.FindComponent<ComponentMount>() into m
			where m != null && m.Entity != base.Entity
			select m)
			{
				float num2 = this.ScoreMount(componentMount, 2.5f);
				if (num2 > num)
				{
					num = num2;
					result = componentMount;
				}
			}
			return result;
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00083264 File Offset: 0x00081464
		public virtual void StartMounting(ComponentMount componentMount)
		{
			if (!this.m_isAnimating && this.Mount == null)
			{
				this.m_isAnimating = true;
				this.m_animationTime = 0f;
				this.m_isDismounting = false;
				this.ComponentCreature.ComponentBody.ParentBody = componentMount.ComponentBody;
				this.ComponentCreature.ComponentBody.ParentBodyPositionOffset = Vector3.Transform(this.ComponentCreature.ComponentBody.Position - componentMount.ComponentBody.Position, Quaternion.Conjugate(componentMount.ComponentBody.Rotation));
				this.ComponentCreature.ComponentBody.ParentBodyRotationOffset = Quaternion.Conjugate(componentMount.ComponentBody.Rotation) * this.ComponentCreature.ComponentBody.Rotation;
				this.m_targetPositionOffset = componentMount.MountOffset + this.m_riderOffset;
				this.m_targetRotationOffset = Quaternion.Identity;
				this.ComponentCreature.ComponentLocomotion.IsCreativeFlyEnabled = false;
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00083364 File Offset: 0x00081564
		public virtual void StartDismounting()
		{
			if (!this.m_isAnimating && this.Mount != null)
			{
				float x = 0f;
				if (this.Mount.DismountOffset.X > 0f)
				{
					float s = this.Mount.DismountOffset.X + 0.5f;
					Vector3 vector = 0.5f * (this.ComponentCreature.ComponentBody.BoundingBox.Min + this.ComponentCreature.ComponentBody.BoundingBox.Max);
					TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, vector - s * this.ComponentCreature.ComponentBody.Matrix.Right, false, true, null);
					TerrainRaycastResult? terrainRaycastResult2 = this.m_subsystemTerrain.Raycast(vector, vector + s * this.ComponentCreature.ComponentBody.Matrix.Right, false, true, null);
					x = ((terrainRaycastResult == null) ? (0f - this.Mount.DismountOffset.X) : ((terrainRaycastResult2 == null) ? this.Mount.DismountOffset.X : ((terrainRaycastResult.Value.Distance <= terrainRaycastResult2.Value.Distance) ? MathUtils.Min(terrainRaycastResult2.Value.Distance, this.Mount.DismountOffset.X) : (0f - MathUtils.Min(terrainRaycastResult.Value.Distance, this.Mount.DismountOffset.X)))));
				}
				this.m_isAnimating = true;
				this.m_animationTime = 0f;
				this.m_isDismounting = true;
				this.m_targetPositionOffset = this.Mount.MountOffset + this.m_riderOffset + new Vector3(x, this.Mount.DismountOffset.Y, this.Mount.DismountOffset.Z);
				this.m_targetRotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.Sign(x) * MathUtils.DegToRad(60f));
			}
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00083588 File Offset: 0x00081788
		public void Update(float dt)
		{
			if (this.m_isAnimating)
			{
				float f = 8f * dt;
				ComponentBody componentBody = this.ComponentCreature.ComponentBody;
				componentBody.ParentBodyPositionOffset = Vector3.Lerp(componentBody.ParentBodyPositionOffset, this.m_targetPositionOffset, f);
				componentBody.ParentBodyRotationOffset = Quaternion.Slerp(componentBody.ParentBodyRotationOffset, this.m_targetRotationOffset, f);
				this.m_animationTime += dt;
				if (Vector3.DistanceSquared(componentBody.ParentBodyPositionOffset, this.m_targetPositionOffset) < 0.0100000007f || this.m_animationTime > 0.75f)
				{
					this.m_isAnimating = false;
					if (this.m_isDismounting)
					{
						if (componentBody.ParentBody != null)
						{
							componentBody.Velocity = componentBody.ParentBody.Velocity;
							componentBody.ParentBody = null;
						}
					}
					else
					{
						componentBody.ParentBodyPositionOffset = this.m_targetPositionOffset;
						componentBody.ParentBodyRotationOffset = this.m_targetRotationOffset;
						this.m_outOfMountTime = 0f;
					}
				}
			}
			ComponentMount mount = this.Mount;
			if (mount != null && !this.m_isAnimating)
			{
				ComponentBody componentBody2 = this.ComponentCreature.ComponentBody;
				ComponentBody parentBody = this.ComponentCreature.ComponentBody.ParentBody;
				if (Vector3.DistanceSquared(parentBody.Position + Vector3.Transform(componentBody2.ParentBodyPositionOffset, parentBody.Rotation), componentBody2.Position) > 0.160000011f)
				{
					this.m_outOfMountTime += dt;
				}
				else
				{
					this.m_outOfMountTime = 0f;
				}
				ComponentHealth componentHealth = mount.Entity.FindComponent<ComponentHealth>();
				if (this.m_outOfMountTime > 0.1f || (componentHealth != null && componentHealth.Health <= 0f) || this.ComponentCreature.ComponentHealth.Health <= 0f)
				{
					this.StartDismounting();
				}
				this.ComponentCreature.ComponentBody.ParentBodyPositionOffset = mount.MountOffset + this.m_riderOffset;
				this.ComponentCreature.ComponentBody.ParentBodyRotationOffset = Quaternion.Identity;
			}
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0008376C File Offset: 0x0008196C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_riderOffset = valuesDictionary.GetValue<Vector3>("RiderOffset");
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x000837C0 File Offset: 0x000819C0
		public virtual float ScoreMount(ComponentMount componentMount, float maxDistance)
		{
			if (componentMount.ComponentBody.Velocity.LengthSquared() < 1f)
			{
				Vector3 v = componentMount.ComponentBody.Position + Vector3.Transform(componentMount.MountOffset, componentMount.ComponentBody.Rotation) - this.ComponentCreature.ComponentCreatureModel.EyePosition;
				if (v.Length() < maxDistance)
				{
					Vector3 forward = Matrix.CreateFromQuaternion(this.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
					if (Vector3.Dot(Vector3.Normalize(v), forward) > 0.33f)
					{
						return maxDistance - v.Length();
					}
				}
			}
			return 0f;
		}

		// Token: 0x04000AA4 RID: 2724
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000AA5 RID: 2725
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AA6 RID: 2726
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000AA7 RID: 2727
		public Vector3 m_riderOffset;

		// Token: 0x04000AA8 RID: 2728
		public float m_animationTime;

		// Token: 0x04000AA9 RID: 2729
		public bool m_isAnimating;

		// Token: 0x04000AAA RID: 2730
		public bool m_isDismounting;

		// Token: 0x04000AAB RID: 2731
		public Vector3 m_targetPositionOffset;

		// Token: 0x04000AAC RID: 2732
		public Quaternion m_targetRotationOffset;

		// Token: 0x04000AAD RID: 2733
		public float m_outOfMountTime;
	}
}
