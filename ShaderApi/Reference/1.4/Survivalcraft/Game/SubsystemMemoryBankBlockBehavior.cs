using System;
using Engine;

namespace Game
{
	// Token: 0x020001B9 RID: 441
	public class SubsystemMemoryBankBlockBehavior : SubsystemEditableItemBehavior<MemoryBankData>
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000B53 RID: 2899 RVA: 0x0004BEC7 File Offset: 0x0004A0C7
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					186
				};
			}
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0004BED7 File Offset: 0x0004A0D7
		public SubsystemMemoryBankBlockBehavior() : base(186)
		{
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0004BEE4 File Offset: 0x0004A0E4
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int id = Terrain.ExtractData(value);
			MemoryBankData memoryBankData = base.GetItemData(id);
			memoryBankData = ((memoryBankData != null) ? ((MemoryBankData)memoryBankData.Copy()) : new MemoryBankData());
			if (SettingsManager.UsePrimaryMemoryBank)
			{
				DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemoryBankDialog(memoryBankData, delegate()
				{
					int data = this.StoreItemDataAtUniqueId(memoryBankData);
					int value = Terrain.ReplaceData(value, data);
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, value, 1);
				}));
			}
			else
			{
				DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemoryBankDialogAPI(memoryBankData, delegate()
				{
					int data = this.StoreItemDataAtUniqueId(memoryBankData);
					int value = Terrain.ReplaceData(value, data);
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, value, 1);
				}));
			}
			return true;
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0004BFC8 File Offset: 0x0004A1C8
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			MemoryBankData memoryBankData = base.GetBlockData(new Point3(x, y, z)) ?? new MemoryBankData();
			if (SettingsManager.UsePrimaryMemoryBank)
			{
				DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemoryBankDialog(memoryBankData, delegate()
				{
					this.SetBlockData(new Point3(x, y, z), memoryBankData);
					int face = ((MemoryBankBlock)BlocksManager.Blocks[186]).GetFace(value);
					SubsystemElectricity subsystemElectricity = this.SubsystemTerrain.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}));
			}
			else
			{
				DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditMemoryBankDialogAPI(memoryBankData, delegate()
				{
					this.SetBlockData(new Point3(x, y, z), memoryBankData);
					int face = ((MemoryBankBlock)BlocksManager.Blocks[186]).GetFace(value);
					SubsystemElectricity subsystemElectricity = this.SubsystemTerrain.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}));
			}
			return true;
		}

		// Token: 0x0400057F RID: 1407
		public static string fName = "MemoryBankBlockBehavior";
	}
}
