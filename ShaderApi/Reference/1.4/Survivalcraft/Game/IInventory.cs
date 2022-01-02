using System;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x020002AF RID: 687
	public interface IInventory
	{
		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06001540 RID: 5440
		Project Project { get; }

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001541 RID: 5441
		int SlotsCount { get; }

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001542 RID: 5442
		// (set) Token: 0x06001543 RID: 5443
		int VisibleSlotsCount { get; set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06001544 RID: 5444
		// (set) Token: 0x06001545 RID: 5445
		int ActiveSlotIndex { get; set; }

		// Token: 0x06001546 RID: 5446
		int GetSlotValue(int slotIndex);

		// Token: 0x06001547 RID: 5447
		int GetSlotCount(int slotIndex);

		// Token: 0x06001548 RID: 5448
		int GetSlotCapacity(int slotIndex, int value);

		// Token: 0x06001549 RID: 5449
		int GetSlotProcessCapacity(int slotIndex, int value);

		// Token: 0x0600154A RID: 5450
		void AddSlotItems(int slotIndex, int value, int count);

		// Token: 0x0600154B RID: 5451
		void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount);

		// Token: 0x0600154C RID: 5452
		int RemoveSlotItems(int slotIndex, int count);

		// Token: 0x0600154D RID: 5453
		void DropAllItems(Vector3 position);
	}
}
