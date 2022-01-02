using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000217 RID: 535
	public abstract class ComponentInventoryBase : Component, IInventory
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x000797DF File Offset: 0x000779DF
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600102E RID: 4142 RVA: 0x000797E7 File Offset: 0x000779E7
		public virtual int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x000797F4 File Offset: 0x000779F4
		// (set) Token: 0x06001030 RID: 4144 RVA: 0x000797FC File Offset: 0x000779FC
		public virtual int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x000797FE File Offset: 0x000779FE
		// (set) Token: 0x06001032 RID: 4146 RVA: 0x00079801 File Offset: 0x00077A01
		public virtual int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00079804 File Offset: 0x00077A04
		public static int FindAcquireSlotForItem(IInventory inventory, int value)
		{
			for (int i = 0; i < inventory.SlotsCount; i++)
			{
				if (inventory.GetSlotCount(i) > 0 && inventory.GetSlotValue(i) == value && inventory.GetSlotCount(i) < inventory.GetSlotCapacity(i, value))
				{
					return i;
				}
			}
			for (int j = 0; j < inventory.SlotsCount; j++)
			{
				if (inventory.GetSlotCount(j) == 0 && inventory.GetSlotCapacity(j, value) > 0)
				{
					return j;
				}
			}
			return -1;
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00079874 File Offset: 0x00077A74
		public static int AcquireItems(IInventory inventory, int value, int count)
		{
			while (count > 0)
			{
				int num = ComponentInventoryBase.FindAcquireSlotForItem(inventory, value);
				if (num < 0)
				{
					break;
				}
				inventory.AddSlotItems(num, value, 1);
				count--;
			}
			return count;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x000798A4 File Offset: 0x00077AA4
		public ComponentPlayer FindInteractingPlayer()
		{
			ComponentPlayer componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			if (componentPlayer == null)
			{
				ComponentBlockEntity componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>();
				if (componentBlockEntity != null)
				{
					Vector3 position = new Vector3(componentBlockEntity.Coordinates);
					componentPlayer = base.Project.FindSubsystem<SubsystemPlayers>(true).FindNearestPlayer(position);
				}
			}
			return componentPlayer;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x000798F0 File Offset: 0x00077AF0
		public static void DropSlotItems(IInventory inventory, int slotIndex, Vector3 position, Vector3 velocity)
		{
			int slotCount = inventory.GetSlotCount(slotIndex);
			if (slotCount > 0)
			{
				int slotValue = inventory.GetSlotValue(slotIndex);
				int num = inventory.RemoveSlotItems(slotIndex, slotCount);
				if (num > 0)
				{
					inventory.Project.FindSubsystem<SubsystemPickables>(true).AddPickable(slotValue, num, position, new Vector3?(velocity), null);
				}
			}
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00079944 File Offset: 0x00077B44
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			int value = valuesDictionary.GetValue<int>("SlotsCount");
			for (int i = 0; i < value; i++)
			{
				this.m_slots.Add(new ComponentInventoryBase.Slot());
			}
			ValuesDictionary value2 = valuesDictionary.GetValue<ValuesDictionary>("Slots");
			for (int j = 0; j < this.m_slots.Count; j++)
			{
				ValuesDictionary value3 = value2.GetValue<ValuesDictionary>("Slot" + j.ToString(CultureInfo.InvariantCulture), null);
				if (value3 != null)
				{
					ComponentInventoryBase.Slot slot = this.m_slots[j];
					slot.Value = value3.GetValue<int>("Contents");
					slot.Count = value3.GetValue<int>("Count");
				}
			}
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x000799F0 File Offset: 0x00077BF0
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Slots", valuesDictionary2);
			for (int i = 0; i < this.m_slots.Count; i++)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[i];
				if (slot.Count > 0)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
					valuesDictionary3.SetValue<int>("Contents", slot.Value);
					valuesDictionary3.SetValue<int>("Count", slot.Count);
				}
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00079A80 File Offset: 0x00077C80
		public virtual int GetSlotValue(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= this.m_slots.Count)
			{
				return 0;
			}
			if (this.m_slots[slotIndex].Count <= 0)
			{
				return 0;
			}
			return this.m_slots[slotIndex].Value;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00079ABD File Offset: 0x00077CBD
		public virtual int GetSlotCount(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex].Count;
			}
			return 0;
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x00079AE4 File Offset: 0x00077CE4
		public virtual int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return BlocksManager.Blocks[Terrain.ExtractContents(value)].GetMaxStacking(value);
			}
			return 0;
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00079B0C File Offset: 0x00077D0C
		public virtual int GetSlotProcessCapacity(int slotIndex, int value)
		{
			int slotCount = this.GetSlotCount(slotIndex);
			int slotValue = this.GetSlotValue(slotIndex);
			if (slotCount > 0 && slotValue != 0)
			{
				SubsystemBlockBehavior[] blockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true).GetBlockBehaviors(Terrain.ExtractContents(slotValue));
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					int processInventoryItemCapacity = blockBehaviors[i].GetProcessInventoryItemCapacity(this, slotIndex, value);
					if (processInventoryItemCapacity > 0)
					{
						return processInventoryItemCapacity;
					}
				}
			}
			return 0;
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x00079B68 File Offset: 0x00077D68
		public virtual void AddSlotItems(int slotIndex, int value, int count)
		{
			if (count > 0 && slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[slotIndex];
				if ((this.GetSlotCount(slotIndex) != 0 && this.GetSlotValue(slotIndex) != value) || this.GetSlotCount(slotIndex) + count > this.GetSlotCapacity(slotIndex, value))
				{
					throw new InvalidOperationException("Cannot add slot items.");
				}
				slot.Value = value;
				slot.Count += count;
			}
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00079BDC File Offset: 0x00077DDC
		public virtual void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			int slotCount = this.GetSlotCount(slotIndex);
			int slotValue = this.GetSlotValue(slotIndex);
			if (slotCount > 0 && slotValue != 0)
			{
				foreach (SubsystemBlockBehavior subsystemBlockBehavior in base.Project.FindSubsystem<SubsystemBlockBehaviors>(true).GetBlockBehaviors(Terrain.ExtractContents(slotValue)))
				{
					int processInventoryItemCapacity = subsystemBlockBehavior.GetProcessInventoryItemCapacity(this, slotIndex, value);
					if (processInventoryItemCapacity > 0)
					{
						subsystemBlockBehavior.ProcessInventoryItem(this, slotIndex, value, count, MathUtils.Min(processInventoryItemCapacity, processCount), out processedValue, out processedCount);
						return;
					}
				}
			}
			processedValue = value;
			processedCount = count;
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x00079C58 File Offset: 0x00077E58
		public virtual int RemoveSlotItems(int slotIndex, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				ComponentInventoryBase.Slot slot = this.m_slots[slotIndex];
				count = MathUtils.Min(count, this.GetSlotCount(slotIndex));
				slot.Count -= count;
				return count;
			}
			return 0;
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00079C98 File Offset: 0x00077E98
		public void DropAllItems(Vector3 position)
		{
			for (int i = 0; i < this.SlotsCount; i++)
			{
				ComponentInventoryBase.DropSlotItems(this, i, position, this.m_random.Float(5f, 10f) * Vector3.Normalize(new Vector3(this.m_random.Float(-1f, 1f), this.m_random.Float(1f, 2f), this.m_random.Float(-1f, 1f))));
			}
		}

		// Token: 0x0400099A RID: 2458
		public List<ComponentInventoryBase.Slot> m_slots = new List<ComponentInventoryBase.Slot>();

		// Token: 0x0400099B RID: 2459
		public Game.Random m_random = new Game.Random();

		// Token: 0x020004D6 RID: 1238
		public class Slot
		{
			// Token: 0x040017B4 RID: 6068
			public int Value;

			// Token: 0x040017B5 RID: 6069
			public int Count;
		}
	}
}
