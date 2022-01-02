using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FB RID: 507
	public class ComponentDamage : Component, IUpdateable
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0006B4DF File Offset: 0x000696DF
		// (set) Token: 0x06000EB6 RID: 3766 RVA: 0x0006B4E7 File Offset: 0x000696E7
		public float Hitpoints { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000EB7 RID: 3767 RVA: 0x0006B4F0 File Offset: 0x000696F0
		// (set) Token: 0x06000EB8 RID: 3768 RVA: 0x0006B4F8 File Offset: 0x000696F8
		public float HitpointsChange { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x0006B501 File Offset: 0x00069701
		// (set) Token: 0x06000EBA RID: 3770 RVA: 0x0006B509 File Offset: 0x00069709
		public float AttackResilience { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0006B512 File Offset: 0x00069712
		// (set) Token: 0x06000EBC RID: 3772 RVA: 0x0006B51A File Offset: 0x0006971A
		public string DamageSoundName { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000EBD RID: 3773 RVA: 0x0006B523 File Offset: 0x00069723
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0006B526 File Offset: 0x00069726
		public virtual void Damage(float amount)
		{
			if (amount > 0f && this.Hitpoints > 0f)
			{
				this.Hitpoints = MathUtils.Max(this.Hitpoints - amount, 0f);
			}
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0006B558 File Offset: 0x00069758
		public virtual void Update(float dt)
		{
			Vector3 position = this.m_componentBody.Position;
			if (this.Hitpoints <= 0f)
			{
				this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, position + this.m_componentBody.BoxSize.Y / 2f * Vector3.UnitY, this.m_debrisStrength, this.m_debrisScale, Color.White, this.m_debrisTextureSlot));
				this.m_subsystemAudio.PlayRandomSound(this.DamageSoundName, 1f, 0f, this.m_componentBody.Position, 4f, true);
				base.Project.RemoveEntity(base.Entity, true);
			}
			float num = MathUtils.Abs(this.m_componentBody.CollisionVelocityChange.Y);
			if (num > this.m_fallResilience)
			{
				float amount = MathUtils.Sqr(MathUtils.Max(num - this.m_fallResilience, 0f)) / 15f;
				this.Damage(amount);
			}
			if (position.Y < -10f || position.Y > 276f)
			{
				this.Damage(this.Hitpoints);
			}
			if (this.m_componentOnFire != null && (this.m_componentOnFire.IsOnFire || this.m_componentOnFire.TouchesFire))
			{
				this.Damage(dt / this.m_fireResilience);
			}
			this.HitpointsChange = this.Hitpoints - this.m_lastHitpoints;
			this.m_lastHitpoints = this.Hitpoints;
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x0006B6CC File Offset: 0x000698CC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentOnFire = base.Entity.FindComponent<ComponentOnFire>();
			this.Hitpoints = valuesDictionary.GetValue<float>("Hitpoints");
			this.AttackResilience = valuesDictionary.GetValue<float>("AttackResilience");
			this.m_fallResilience = valuesDictionary.GetValue<float>("FallResilience");
			this.m_fireResilience = valuesDictionary.GetValue<float>("FireResilience");
			this.m_debrisTextureSlot = valuesDictionary.GetValue<int>("DebrisTextureSlot");
			this.m_debrisStrength = valuesDictionary.GetValue<float>("DebrisStrength");
			this.m_debrisScale = valuesDictionary.GetValue<float>("DebrisScale");
			this.DamageSoundName = valuesDictionary.GetValue<string>("DestructionSoundName");
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0006B7BA File Offset: 0x000699BA
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Hitpoints", this.Hitpoints);
		}

		// Token: 0x040007F6 RID: 2038
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040007F7 RID: 2039
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040007F8 RID: 2040
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040007F9 RID: 2041
		public ComponentBody m_componentBody;

		// Token: 0x040007FA RID: 2042
		public ComponentOnFire m_componentOnFire;

		// Token: 0x040007FB RID: 2043
		public float m_lastHitpoints;

		// Token: 0x040007FC RID: 2044
		public float m_fallResilience;

		// Token: 0x040007FD RID: 2045
		public float m_fireResilience;

		// Token: 0x040007FE RID: 2046
		public int m_debrisTextureSlot;

		// Token: 0x040007FF RID: 2047
		public float m_debrisStrength;

		// Token: 0x04000800 RID: 2048
		public float m_debrisScale;
	}
}
