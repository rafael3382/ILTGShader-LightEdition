using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020C RID: 524
	public class ComponentFurnitureInventory : Component, IInventory
	{
		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x00072941 File Offset: 0x00070B41
		// (set) Token: 0x06000F6C RID: 3948 RVA: 0x00072949 File Offset: 0x00070B49
		public int PageIndex { get; set; }

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000F6D RID: 3949 RVA: 0x00072952 File Offset: 0x00070B52
		// (set) Token: 0x06000F6E RID: 3950 RVA: 0x0007295A File Offset: 0x00070B5A
		public FurnitureSet FurnitureSet { get; set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000F6F RID: 3951 RVA: 0x00072963 File Offset: 0x00070B63
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0007296B File Offset: 0x00070B6B
		// (set) Token: 0x06000F71 RID: 3953 RVA: 0x0007296E File Offset: 0x00070B6E
		public int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000F72 RID: 3954 RVA: 0x00072970 File Offset: 0x00070B70
		public int SlotsCount
		{
			get
			{
				return this.m_slots.Count;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000F73 RID: 3955 RVA: 0x0007297D File Offset: 0x00070B7D
		// (set) Token: 0x06000F74 RID: 3956 RVA: 0x00072985 File Offset: 0x00070B85
		public int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x00072988 File Offset: 0x00070B88
		public virtual void FillSlots()
		{
			this.m_subsystemFurnitureBlockBehavior.GarbageCollectDesigns();
			this.m_slots.Clear();
			for (int i = 0; i < 8192; i++)
			{
				FurnitureDesign design = this.m_subsystemFurnitureBlockBehavior.GetDesign(i);
				if (design != null)
				{
					int num = (from f in design.ListChain()
					select f.Index).Min();
					if (design.Index == num)
					{
						int data = FurnitureBlock.SetDesignIndex(0, i, design.ShadowStrengthFactor, design.IsLightEmitter);
						int item = Terrain.MakeBlockValue(227, 0, data);
						this.m_slots.Add(item);
					}
				}
			}
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x00072A36 File Offset: 0x00070C36
		public virtual void ClearSlots()
		{
			this.m_slots.Clear();
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x00072A44 File Offset: 0x00070C44
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			string furnitureSetName = valuesDictionary.GetValue<string>("FurnitureSet");
			this.FurnitureSet = this.m_subsystemFurnitureBlockBehavior.FurnitureSets.FirstOrDefault((FurnitureSet f) => f.Name == furnitureSetName);
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x00072AA1 File Offset: 0x00070CA1
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<string>("FurnitureSet", (this.FurnitureSet != null) ? this.FurnitureSet.Name : string.Empty);
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00072AC8 File Offset: 0x00070CC8
		public virtual int GetSlotValue(int slotIndex)
		{
			if (slotIndex >= 0 && slotIndex < this.m_slots.Count)
			{
				return this.m_slots[slotIndex];
			}
			return 0;
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x00072AEA File Offset: 0x00070CEA
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

		// Token: 0x06000F7B RID: 3963 RVA: 0x00072B15 File Offset: 0x00070D15
		public virtual int GetSlotCapacity(int slotIndex, int value)
		{
			return 99980001;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x00072B1C File Offset: 0x00070D1C
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
			return 9999;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x00072B7B File Offset: 0x00070D7B
		public virtual void AddSlotItems(int slotIndex, int value, int count)
		{
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x00072B80 File Offset: 0x00070D80
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
			processedValue = 0;
			processedCount = 0;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x00072BFC File Offset: 0x00070DFC
		public virtual int RemoveSlotItems(int slotIndex, int count)
		{
			return 1;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x00072BFF File Offset: 0x00070DFF
		public virtual void DropAllItems(Vector3 position)
		{
		}

		// Token: 0x040008E2 RID: 2274
		public SubsystemFurnitureBlockBehavior m_subsystemFurnitureBlockBehavior;

		// Token: 0x040008E3 RID: 2275
		public List<int> m_slots = new List<int>();

		// Token: 0x040008E4 RID: 2276
		public const int m_largeNumber = 9999;
	}
}
