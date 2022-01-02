using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000216 RID: 534
	public class ComponentInventory : ComponentInventoryBase, IInventory
	{
		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06001025 RID: 4133 RVA: 0x000796AE File Offset: 0x000778AE
		// (set) Token: 0x06001026 RID: 4134 RVA: 0x000796B6 File Offset: 0x000778B6
		public override int ActiveSlotIndex
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

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06001027 RID: 4135 RVA: 0x000796CD File Offset: 0x000778CD
		// (set) Token: 0x06001028 RID: 4136 RVA: 0x000796D8 File Offset: 0x000778D8
		public override int VisibleSlotsCount
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

		// Token: 0x06001029 RID: 4137 RVA: 0x00079775 File Offset: 0x00077975
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.ActiveSlotIndex = valuesDictionary.GetValue<int>("ActiveSlotIndex");
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x00079790 File Offset: 0x00077990
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<int>("ActiveSlotIndex", this.ActiveSlotIndex);
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x000797AB File Offset: 0x000779AB
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex >= this.VisibleSlotsCount && slotIndex < 10)
			{
				return 0;
			}
			return BlocksManager.Blocks[Terrain.ExtractContents(value)].GetMaxStacking(value);
		}

		// Token: 0x04000997 RID: 2455
		public int m_activeSlotIndex;

		// Token: 0x04000998 RID: 2456
		public int m_visibleSlotsCount = 10;

		// Token: 0x04000999 RID: 2457
		public const int ShortInventorySlotsCount = 10;
	}
}
