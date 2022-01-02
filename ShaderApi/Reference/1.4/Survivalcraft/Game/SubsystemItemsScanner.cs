using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B3 RID: 435
	public class SubsystemItemsScanner : Subsystem, IUpdateable
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0004AFCF File Offset: 0x000491CF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000B2B RID: 2859 RVA: 0x0004AFD2 File Offset: 0x000491D2
		// (set) Token: 0x06000B2C RID: 2860 RVA: 0x0004AFDA File Offset: 0x000491DA
		public virtual Action<ReadOnlyList<ScannedItemData>> ItemsScanned { get; set; }

		// Token: 0x06000B2D RID: 2861 RVA: 0x0004AFE4 File Offset: 0x000491E4
		public ReadOnlyList<ScannedItemData> ScanItems()
		{
			this.m_items.Clear();
			foreach (Subsystem subsystem in base.Project.Subsystems)
			{
				IInventory inventory = subsystem as IInventory;
				if (inventory != null)
				{
					this.ScanInventory(inventory, this.m_items);
				}
			}
			foreach (Entity entity in base.Project.Entities)
			{
				foreach (Component component in entity.Components)
				{
					IInventory inventory2 = component as IInventory;
					if (inventory2 != null)
					{
						this.ScanInventory(inventory2, this.m_items);
					}
				}
			}
			foreach (Pickable pickable in base.Project.FindSubsystem<SubsystemPickables>(true).Pickables)
			{
				if (pickable.Count > 0 && pickable.Value != 0)
				{
					List<ScannedItemData> items = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = pickable,
						Value = pickable.Value,
						Count = pickable.Count
					};
					items.Add(item);
				}
			}
			foreach (Projectile projectile in base.Project.FindSubsystem<SubsystemProjectiles>(true).Projectiles)
			{
				if (projectile.Value != 0)
				{
					List<ScannedItemData> items2 = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = projectile,
						Value = projectile.Value,
						Count = 1
					};
					items2.Add(item);
				}
			}
			foreach (IMovingBlockSet movingBlockSet in base.Project.FindSubsystem<SubsystemMovingBlocks>(true).MovingBlockSets)
			{
				for (int i = 0; i < movingBlockSet.Blocks.Count; i++)
				{
					List<ScannedItemData> items3 = this.m_items;
					ScannedItemData item = new ScannedItemData
					{
						Container = movingBlockSet,
						Value = movingBlockSet.Blocks[i].Value,
						Count = 1,
						IndexInContainer = i
					};
					items3.Add(item);
				}
			}
			return new ReadOnlyList<ScannedItemData>(this.m_items);
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x0004B2D4 File Offset: 0x000494D4
		public bool TryModifyItem(ScannedItemData itemData, int newValue)
		{
			if (itemData.Container is IInventory)
			{
				IInventory inventory = (IInventory)itemData.Container;
				inventory.RemoveSlotItems(itemData.IndexInContainer, itemData.Count);
				int slotCapacity = inventory.GetSlotCapacity(itemData.IndexInContainer, newValue);
				inventory.AddSlotItems(itemData.IndexInContainer, newValue, MathUtils.Min(itemData.Count, slotCapacity));
				return true;
			}
			if (itemData.Container is WorldItem)
			{
				((WorldItem)itemData.Container).Value = newValue;
				return true;
			}
			if (itemData.Container is IMovingBlockSet)
			{
				IMovingBlockSet movingBlockSet = (IMovingBlockSet)itemData.Container;
				MovingBlock movingBlock = movingBlockSet.Blocks.ElementAt(itemData.IndexInContainer);
				movingBlockSet.SetBlock(movingBlock.Offset, newValue);
				return true;
			}
			return false;
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0004B392 File Offset: 0x00049592
		public void Update(float dt)
		{
			if (Time.FrameStartTime >= this.m_nextAutomaticScanTime)
			{
				this.m_nextAutomaticScanTime = Time.FrameStartTime + 60.0;
				Action<ReadOnlyList<ScannedItemData>> itemsScanned = this.ItemsScanned;
				if (itemsScanned == null)
				{
					return;
				}
				itemsScanned(this.ScanItems());
			}
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0004B3CC File Offset: 0x000495CC
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_nextAutomaticScanTime = Time.FrameStartTime + 60.0;
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0004B3E4 File Offset: 0x000495E4
		public void ScanInventory(IInventory inventory, List<ScannedItemData> items)
		{
			for (int i = 0; i < inventory.SlotsCount; i++)
			{
				int slotCount = inventory.GetSlotCount(i);
				if (slotCount > 0)
				{
					int slotValue = inventory.GetSlotValue(i);
					if (slotValue != 0)
					{
						items.Add(new ScannedItemData
						{
							Container = inventory,
							IndexInContainer = i,
							Value = slotValue,
							Count = slotCount
						});
					}
				}
			}
		}

		// Token: 0x0400056C RID: 1388
		public const float m_automaticScanPeriod = 60f;

		// Token: 0x0400056D RID: 1389
		public double m_nextAutomaticScanTime;

		// Token: 0x0400056E RID: 1390
		public List<ScannedItemData> m_items = new List<ScannedItemData>();
	}
}
