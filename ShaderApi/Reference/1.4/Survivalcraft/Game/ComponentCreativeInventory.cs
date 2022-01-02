using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F7 RID: 503
	public class ComponentCreativeInventory : Component, IInventory
	{
		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000E52 RID: 3666 RVA: 0x00069EAC File Offset: 0x000680AC
		// (set) Token: 0x06000E53 RID: 3667 RVA: 0x00069EB4 File Offset: 0x000680B4
		public int OpenSlotsCount { get; set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x00069EBD File Offset: 0x000680BD
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x00069EC5 File Offset: 0x000680C5
		public int CategoryIndex { get; set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000E56 RID: 3670 RVA: 0x00069ECE File Offset: 0x000680CE
		// (set) Token: 0x06000E57 RID: 3671 RVA: 0x00069ED6 File Offset: 0x000680D6
		public int PageIndex { get; set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000E58 RID: 3672 RVA: 0x00069EDF File Offset: 0x000680DF
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000E59 RID: 3673 RVA: 0x00069EE7 File Offset: 0x000680E7
		// (set) Token: 0x06000E5A RID: 3674 RVA: 0x00069EEF File Offset: 0x000680EF
		public int ActiveSlotIndex
		{
			get
			{
				return this.m_activeSlotIndex;
			}
			set
			{
				this.m_activeSlotIndex = MathUtils.Clamp(value, 0, this.VisibleSlotsCount - 1);
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000E5B RID: 3675 RVA: 0x00069F06 File Offset: 0x00068106
		public int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00069F13 File Offset: 0x00068113
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x00069F1C File Offset: 0x0006811C
		public int VisibleSlotsCount
		{
			get
			{
				return this.m_visibleSlotsCount;
			}
			set
			{
				value = MathUtils.Clamp(value, 0, 10);
				if (value == this.m_visibleSlotsCount)
				{
					return;
				}
				this.m_visibleSlotsCount = value;
				this.ActiveSlotIndex = this.ActiveSlotIndex;
				ComponentFrame componentFrame = base.Entity.FindComponent<ComponentFrame>();
				if (componentFrame != null)
				{
					Vector3 position = componentFrame.Position + new Vector3(0f, 0.5f, 0f);
					Vector3 velocity = 1f * componentFrame.Rotation.GetForwardVector();
					for (int i = this.m_visibleSlotsCount; i < 10; i++)
					{
						ComponentInventoryBase.DropSlotItems(this, i, position, velocity);
					}
				}
			}
		}

		// Token: 0x06000E5E RID: 3678 RVA: 0x00069FBC File Offset: 0x000681BC
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_activeSlotIndex = valuesDictionary.GetValue<int>("ActiveSlotIndex");
			this.OpenSlotsCount = valuesDictionary.GetValue<int>("OpenSlotsCount");
			this.CategoryIndex = valuesDictionary.GetValue<int>("CategoryIndex");
			this.PageIndex = valuesDictionary.GetValue<int>("PageIndex");
			for (int i = 0; i < this.OpenSlotsCount; i++)
			{
				this.m_slots.Add(0);
			}
			List<ComponentCreativeInventory.Order> list = new List<ComponentCreativeInventory.Order>();
			foreach (Block block in BlocksManager.Blocks)
			{
				foreach (int num in block.GetCreativeValues())
				{
					list.Add(new ComponentCreativeInventory.Order(block, block.GetDisplayOrder(num), num));
				}
			}
			foreach (ComponentCreativeInventory.Order order in from o in list
			orderby o.order
			select o)
			{
				this.m_slots.Add(order.value);
			}
			ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("Slots", null);
			if (value == null)
			{
				return;
			}
			for (int k = 0; k < this.OpenSlotsCount; k++)
			{
				ValuesDictionary value2 = value.GetValue<ValuesDictionary>("Slot" + k.ToString(CultureInfo.InvariantCulture), null);
				if (value2 != null)
				{
					this.m_slots[k] = value2.GetValue<int>("Contents");
				}
			}
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x0006A174 File Offset: 0x00068374
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<int>("ActiveSlotIndex", this.m_activeSlotIndex);
			valuesDictionary.SetValue<int>("CategoryIndex", this.CategoryIndex);
			valuesDictionary.SetValue<int>("PageIndex", this.PageIndex);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Slots", valuesDictionary2);
			for (int i = 0; i < this.OpenSlotsCount; i++)
			{
				if (this.m_slots[i] != 0)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
					valuesDictionary3.SetValue<int>("Contents", this.m_slots[i]);
				}
			}
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x0006A21F File Offset: 0x0006841F
		public virtual int GetSlotValue(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex];
			}
			return 0;
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x0006A241 File Offset: 0x00068441
		public virtual int GetSlotCount(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= this.m_slots.Count)
			{
				return 0;
			}
			if (this.m_slots[slotIndex] == 0)
			{
				return 0;
			}
			return 9999;
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x0006A26C File Offset: 0x0006846C
		public virtual int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= this.VisibleSlotsCount && slotIndex < 10)
			{
				return 0;
			}
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				return 99980001;
			}
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num].IsNonDuplicable_(value))
			{
				return 9999;
			}
			return 99980001;
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x0006A2C0 File Offset: 0x000684C0
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
			if (slotIndex < this.OpenSlotsCount)
			{
				return 0;
			}
			return 9999;
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x0006A32A File Offset: 0x0006852A
		public virtual void AddSlotItems(int slotIndex, int value, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				this.m_slots[slotIndex] = value;
			}
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x0006A348 File Offset: 0x00068548
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
			if (slotIndex >= this.OpenSlotsCount)
			{
				processedValue = 0;
				processedCount = 0;
				return;
			}
			processedValue = value;
			processedCount = count;
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x0006A3D8 File Offset: 0x000685D8
		public virtual int RemoveSlotItems(int slotIndex, int count)
		{
			if (slotIndex >= 0 && slotIndex < this.OpenSlotsCount)
			{
				int num = Terrain.ExtractContents(this.m_slots[slotIndex]);
				if (BlocksManager.Blocks[num].IsNonDuplicable_(this.m_slots[slotIndex]))
				{
					this.m_slots[slotIndex] = 0;
					return 1;
				}
				if (count >= 9999)
				{
					this.m_slots[slotIndex] = 0;
					return 1;
				}
			}
			return 1;
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x0006A445 File Offset: 0x00068645
		public virtual void DropAllItems(Vector3 position)
		{
		}

		// Token: 0x040007B7 RID: 1975
		public List<int> m_slots = new List<int>();

		// Token: 0x040007B8 RID: 1976
		public int m_activeSlotIndex;

		// Token: 0x040007B9 RID: 1977
		public int m_visibleSlotsCount = 10;

		// Token: 0x040007BA RID: 1978
		public const int m_largeNumber = 9999;

		// Token: 0x020004C2 RID: 1218
		internal class Order
		{
			// Token: 0x060020FC RID: 8444 RVA: 0x000E98CD File Offset: 0x000E7ACD
			public Order(Block b, int o, int v)
			{
				this.block = b;
				this.order = o;
				this.value = v;
			}

			// Token: 0x04001781 RID: 6017
			public Block block;

			// Token: 0x04001782 RID: 6018
			public int order;

			// Token: 0x04001783 RID: 6019
			public int value;
		}
	}
}
