using System;

namespace Game
{
	// Token: 0x02000185 RID: 389
	public class SubsystemBatteryBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x000387E1 File Offset: 0x000369E1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					138
				};
			}
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x000387F4 File Offset: 0x000369F4
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int data = Terrain.ExtractData(value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditBatteryDialog(voltageLevel, delegate(int newVoltageLevel)
			{
				int data = BatteryBlock.SetVoltageLevel(data, newVoltageLevel);
				int num = Terrain.ReplaceData(value, data);
				if (num != value)
				{
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, num, 1);
				}
			}));
			return true;
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00038880 File Offset: 0x00036A80
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			int data = Terrain.ExtractData(value);
			int voltageLevel = BatteryBlock.GetVoltageLevel(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditBatteryDialog(voltageLevel, delegate(int newVoltageLevel)
			{
				int num = BatteryBlock.SetVoltageLevel(data, newVoltageLevel);
				if (num != data)
				{
					int value2 = Terrain.ReplaceData(value, num);
					this.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
					SubsystemElectricity subsystemElectricity = this.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, 4);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementConnectionsForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}
			}));
			return true;
		}
	}
}
