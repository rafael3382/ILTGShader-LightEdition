using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021C RID: 540
	public class ComponentLoot : Component, IUpdateable
	{
		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060010B0 RID: 4272 RVA: 0x0007CF57 File Offset: 0x0007B157
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x0007CF5C File Offset: 0x0007B15C
		public static List<ComponentLoot.Loot> ParseLootList(ValuesDictionary lootVd)
		{
			List<ComponentLoot.Loot> list = new List<ComponentLoot.Loot>();
			foreach (object obj in lootVd.Values)
			{
				string lootString = (string)obj;
				list.Add(ComponentLoot.ParseLoot(lootString));
			}
			list.Sort((ComponentLoot.Loot l1, ComponentLoot.Loot l2) => l1.Value - l2.Value);
			return list;
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x0007CFE0 File Offset: 0x0007B1E0
		public void Update(float dt)
		{
			if (!this.m_lootDropped && this.m_componentCreature.ComponentHealth.DeathTime != null && this.m_subsystemGameInfo.TotalElapsedGameTime >= this.m_componentCreature.ComponentHealth.DeathTime.Value + (double)this.m_componentCreature.ComponentHealth.CorpseDuration)
			{
				ComponentOnFire componentOnFire = this.m_componentCreature.Entity.FindComponent<ComponentOnFire>();
				bool flag = componentOnFire != null && componentOnFire.IsOnFire;
				this.m_lootDropped = true;
				foreach (ComponentLoot.Loot loot in (flag ? this.m_lootOnFireList : this.m_lootList))
				{
					if (this.m_random.Float(0f, 1f) < loot.Probability)
					{
						int num = this.m_random.Int(loot.MinCount, loot.MaxCount);
						for (int i = 0; i < num; i++)
						{
							Vector3 position = (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max) / 2f;
							this.m_subsystemPickables.AddPickable(loot.Value, 1, position, null, null);
						}
					}
				}
			}
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x0007D170 File Offset: 0x0007B370
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_lootDropped = valuesDictionary.GetValue<bool>("LootDropped");
			this.m_lootList = ComponentLoot.ParseLootList(valuesDictionary.GetValue<ValuesDictionary>("Loot"));
			this.m_lootOnFireList = ComponentLoot.ParseLootList(valuesDictionary.GetValue<ValuesDictionary>("LootOnFire"));
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0007D1F0 File Offset: 0x0007B3F0
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("LootDropped", this.m_lootDropped);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x0007D204 File Offset: 0x0007B404
		public static ComponentLoot.Loot ParseLoot(string lootString)
		{
			string[] array = lootString.Split(new string[]
			{
				";"
			}, StringSplitOptions.None);
			if (array.Length >= 3)
			{
				try
				{
					int value = CraftingRecipesManager.DecodeResult(array[0]);
					return new ComponentLoot.Loot
					{
						Value = value,
						MinCount = int.Parse(array[1]),
						MaxCount = int.Parse(array[2]),
						Probability = ((array.Length >= 4) ? float.Parse(array[3]) : 1f)
					};
				}
				catch
				{
					return default(ComponentLoot.Loot);
				}
			}
			throw new InvalidOperationException("Invalid loot string.");
		}

		// Token: 0x040009F5 RID: 2549
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040009F6 RID: 2550
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040009F7 RID: 2551
		public ComponentCreature m_componentCreature;

		// Token: 0x040009F8 RID: 2552
		public List<ComponentLoot.Loot> m_lootList;

		// Token: 0x040009F9 RID: 2553
		public List<ComponentLoot.Loot> m_lootOnFireList;

		// Token: 0x040009FA RID: 2554
		public Game.Random m_random = new Game.Random();

		// Token: 0x040009FB RID: 2555
		public bool m_lootDropped;

		// Token: 0x020004D8 RID: 1240
		public struct Loot
		{
			// Token: 0x040017B8 RID: 6072
			public int Value;

			// Token: 0x040017B9 RID: 6073
			public int MinCount;

			// Token: 0x040017BA RID: 6074
			public int MaxCount;

			// Token: 0x040017BB RID: 6075
			public float Probability;
		}
	}
}
