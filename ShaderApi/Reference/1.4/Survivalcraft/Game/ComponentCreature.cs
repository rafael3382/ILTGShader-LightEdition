using System;
using Engine;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F8 RID: 504
	public class ComponentCreature : Component
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0006A462 File Offset: 0x00068662
		// (set) Token: 0x06000E6A RID: 3690 RVA: 0x0006A46A File Offset: 0x0006866A
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000E6B RID: 3691 RVA: 0x0006A473 File Offset: 0x00068673
		// (set) Token: 0x06000E6C RID: 3692 RVA: 0x0006A47B File Offset: 0x0006867B
		public ComponentHealth ComponentHealth { get; set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0006A484 File Offset: 0x00068684
		// (set) Token: 0x06000E6E RID: 3694 RVA: 0x0006A48C File Offset: 0x0006868C
		public ComponentSpawn ComponentSpawn { get; set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0006A495 File Offset: 0x00068695
		// (set) Token: 0x06000E70 RID: 3696 RVA: 0x0006A49D File Offset: 0x0006869D
		public ComponentCreatureModel ComponentCreatureModel { get; set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0006A4A6 File Offset: 0x000686A6
		// (set) Token: 0x06000E72 RID: 3698 RVA: 0x0006A4AE File Offset: 0x000686AE
		public ComponentCreatureSounds ComponentCreatureSounds { get; set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0006A4B7 File Offset: 0x000686B7
		// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0006A4BF File Offset: 0x000686BF
		public ComponentLocomotion ComponentLocomotion { get; set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0006A4C8 File Offset: 0x000686C8
		public PlayerStats PlayerStats
		{
			get
			{
				ComponentPlayer componentPlayer = this as ComponentPlayer;
				if (componentPlayer != null)
				{
					return this.m_subsystemPlayerStats.GetPlayerStats(componentPlayer.PlayerData.PlayerIndex);
				}
				return null;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000E76 RID: 3702 RVA: 0x0006A4F7 File Offset: 0x000686F7
		// (set) Token: 0x06000E77 RID: 3703 RVA: 0x0006A4FF File Offset: 0x000686FF
		public bool ConstantSpawn { get; set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x0006A508 File Offset: 0x00068708
		// (set) Token: 0x06000E79 RID: 3705 RVA: 0x0006A510 File Offset: 0x00068710
		public CreatureCategory Category { get; set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x0006A519 File Offset: 0x00068719
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x0006A521 File Offset: 0x00068721
		public string DisplayName { get; set; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x0006A52A File Offset: 0x0006872A
		public ReadOnlyList<string> KillVerbs
		{
			get
			{
				return new ReadOnlyList<string>(this.m_killVerbs);
			}
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x0006A538 File Offset: 0x00068738
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.ComponentHealth = base.Entity.FindComponent<ComponentHealth>(true);
			this.ComponentSpawn = base.Entity.FindComponent<ComponentSpawn>(true);
			this.ComponentCreatureSounds = base.Entity.FindComponent<ComponentCreatureSounds>(true);
			this.ComponentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(true);
			this.ComponentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(true);
			this.m_subsystemPlayerStats = base.Project.FindSubsystem<SubsystemPlayerStats>(true);
			this.ConstantSpawn = valuesDictionary.GetValue<bool>("ConstantSpawn");
			this.Category = valuesDictionary.GetValue<CreatureCategory>("Category");
			this.DisplayName = valuesDictionary.GetValue<string>("DisplayName");
			if (this.DisplayName.StartsWith("[") && this.DisplayName.EndsWith("]"))
			{
				string[] array = this.DisplayName.Substring(1, this.DisplayName.Length - 2).Split(new string[]
				{
					":"
				}, StringSplitOptions.RemoveEmptyEntries);
				this.DisplayName = LanguageControl.GetDatabase("DisplayName", array[1]);
			}
			this.m_killVerbs = HumanReadableConverter.ValuesListFromString<string>(',', valuesDictionary.GetValue<string>("KillVerbs"));
			if (this.m_killVerbs.Length == 0)
			{
				throw new InvalidOperationException("Must have at least one KillVerb");
			}
			if (!MathUtils.IsPowerOf2((long)this.Category))
			{
				throw new InvalidOperationException("A single category must be assigned for creature.");
			}
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x0006A6A0 File Offset: 0x000688A0
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("ConstantSpawn", this.ConstantSpawn);
		}

		// Token: 0x040007BE RID: 1982
		public SubsystemPlayerStats m_subsystemPlayerStats;

		// Token: 0x040007BF RID: 1983
		public string[] m_killVerbs;
	}
}
