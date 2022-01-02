using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000230 RID: 560
	public class ComponentSpawn : Component, IUpdateable
	{
		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x000861AF File Offset: 0x000843AF
		// (set) Token: 0x06001204 RID: 4612 RVA: 0x000861B7 File Offset: 0x000843B7
		public ComponentFrame ComponentFrame { get; set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x000861C0 File Offset: 0x000843C0
		// (set) Token: 0x06001206 RID: 4614 RVA: 0x000861C8 File Offset: 0x000843C8
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x000861D1 File Offset: 0x000843D1
		// (set) Token: 0x06001208 RID: 4616 RVA: 0x000861D9 File Offset: 0x000843D9
		public bool AutoDespawn { get; set; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001209 RID: 4617 RVA: 0x000861E4 File Offset: 0x000843E4
		public bool IsDespawning
		{
			get
			{
				return this.DespawnTime != null;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x000861FF File Offset: 0x000843FF
		// (set) Token: 0x0600120B RID: 4619 RVA: 0x00086207 File Offset: 0x00084407
		public double SpawnTime { get; set; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600120C RID: 4620 RVA: 0x00086210 File Offset: 0x00084410
		// (set) Token: 0x0600120D RID: 4621 RVA: 0x00086218 File Offset: 0x00084418
		public double? DespawnTime { get; set; }

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x00086221 File Offset: 0x00084421
		// (set) Token: 0x0600120F RID: 4623 RVA: 0x00086229 File Offset: 0x00084429
		public float SpawnDuration { get; set; }

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x00086232 File Offset: 0x00084432
		// (set) Token: 0x06001211 RID: 4625 RVA: 0x0008623A File Offset: 0x0008443A
		public float DespawnDuration { get; set; }

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x00086243 File Offset: 0x00084443
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001213 RID: 4627 RVA: 0x00086246 File Offset: 0x00084446
		// (set) Token: 0x06001214 RID: 4628 RVA: 0x0008624E File Offset: 0x0008444E
		public virtual Action<ComponentSpawn> Despawned { get; set; }

		// Token: 0x06001215 RID: 4629 RVA: 0x00086258 File Offset: 0x00084458
		public virtual void Despawn()
		{
			if (this.DespawnTime == null)
			{
				this.DespawnTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0008628C File Offset: 0x0008448C
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.ComponentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>();
			this.AutoDespawn = valuesDictionary.GetValue<bool>("AutoDespawn");
			double value = valuesDictionary.GetValue<double>("SpawnTime");
			double value2 = valuesDictionary.GetValue<double>("DespawnTime");
			this.SpawnDuration = 2f;
			this.DespawnDuration = 2f;
			this.SpawnTime = ((value < 0.0) ? this.m_subsystemGameInfo.TotalElapsedGameTime : value);
			this.DespawnTime = ((value2 >= 0.0) ? new double?(value2) : null);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00086350 File Offset: 0x00084550
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("SpawnTime", this.SpawnTime);
			if (this.DespawnTime != null)
			{
				valuesDictionary.SetValue<double>("DespawnTime", this.DespawnTime.Value);
			}
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00086398 File Offset: 0x00084598
		public void Update(float dt)
		{
			if (this.DespawnTime != null && this.m_subsystemGameInfo.TotalElapsedGameTime >= this.DespawnTime.Value + (double)this.DespawnDuration)
			{
				base.Project.RemoveEntity(base.Entity, true);
				Action<ComponentSpawn> despawned = this.Despawned;
				if (despawned == null)
				{
					return;
				}
				despawned(this);
			}
		}

		// Token: 0x04000AFB RID: 2811
		public SubsystemGameInfo m_subsystemGameInfo;
	}
}
