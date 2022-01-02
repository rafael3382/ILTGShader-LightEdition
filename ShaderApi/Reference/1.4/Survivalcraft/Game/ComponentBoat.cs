using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EF RID: 495
	public class ComponentBoat : Component, IUpdateable
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000DA0 RID: 3488 RVA: 0x0006374B File Offset: 0x0006194B
		// (set) Token: 0x06000DA1 RID: 3489 RVA: 0x00063753 File Offset: 0x00061953
		public float MoveOrder { get; set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000DA2 RID: 3490 RVA: 0x0006375C File Offset: 0x0006195C
		// (set) Token: 0x06000DA3 RID: 3491 RVA: 0x00063764 File Offset: 0x00061964
		public float TurnOrder { get; set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x0006376D File Offset: 0x0006196D
		// (set) Token: 0x06000DA5 RID: 3493 RVA: 0x00063775 File Offset: 0x00061975
		public float Health { get; set; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x0006377E File Offset: 0x0006197E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x00063781 File Offset: 0x00061981
		public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability)
		{
			if (amount > 0f)
			{
				this.Health = MathUtils.Max(this.Health - amount, 0f);
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x000637A4 File Offset: 0x000619A4
		public void Update(float dt)
		{
			if (this.m_componentDamage.Hitpoints < 0.33f)
			{
				this.m_componentBody.Density = 1.15f;
				if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.33f && this.m_componentBody.ImmersionFactor > 0f)
				{
					this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
				}
			}
			else if (this.m_componentDamage.Hitpoints < 0.66f)
			{
				this.m_componentBody.Density = 0.7f;
				if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.66f && this.m_componentBody.ImmersionFactor > 0f)
				{
					this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
				}
			}
			bool flag = this.m_componentBody.ImmersionFactor > 0.95f;
			object obj = !flag && this.m_componentBody.ImmersionFactor > 0.01f && this.m_componentBody.StandingOnValue == null && this.m_componentBody.StandingOnBody == null;
			this.m_turnSpeed += 2.5f * this.m_subsystemTime.GameTimeDelta * (1f * this.TurnOrder - this.m_turnSpeed);
			Quaternion rotation = this.m_componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			object obj2 = obj;
			if (obj2 != null)
			{
				num -= this.m_turnSpeed * dt;
			}
			this.m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			if (obj2 != null && this.MoveOrder != 0f)
			{
				this.m_componentBody.Velocity += dt * 3f * this.MoveOrder * this.m_componentBody.Matrix.Forward;
			}
			if (flag)
			{
				this.m_componentDamage.Damage(0.005f * dt);
				if (this.m_componentMount.Rider != null)
				{
					this.m_componentMount.Rider.StartDismounting();
				}
			}
			this.MoveOrder = 0f;
			this.TurnOrder = 0f;
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x00063A50 File Offset: 0x00061C50
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentDamage = base.Entity.FindComponent<ComponentDamage>(true);
		}

		// Token: 0x04000728 RID: 1832
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000729 RID: 1833
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400072A RID: 1834
		public ComponentMount m_componentMount;

		// Token: 0x0400072B RID: 1835
		public ComponentBody m_componentBody;

		// Token: 0x0400072C RID: 1836
		public ComponentDamage m_componentDamage;

		// Token: 0x0400072D RID: 1837
		public float m_turnSpeed;
	}
}
