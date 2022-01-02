using System;

namespace Game
{
	// Token: 0x02000180 RID: 384
	public class SubsystemAdjustableDelayGateBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x00036C9C File Offset: 0x00034E9C
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					224
				};
			}
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00036CAC File Offset: 0x00034EAC
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int data = Terrain.ExtractData(value);
			int delay = AdjustableDelayGateBlock.GetDelay(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditAdjustableDelayGateDialog(delay, delegate(int newDelay)
			{
				int data = AdjustableDelayGateBlock.SetDelay(data, newDelay);
				int num = Terrain.ReplaceData(value, data);
				if (num != value)
				{
					inventory.RemoveSlotItems(slotIndex, count);
					inventory.AddSlotItems(slotIndex, num, 1);
				}
			}));
			return true;
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00036D38 File Offset: 0x00034F38
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			int data = Terrain.ExtractData(value);
			int delay = AdjustableDelayGateBlock.GetDelay(data);
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditAdjustableDelayGateDialog(delay, delegate(int newDelay)
			{
				int num = AdjustableDelayGateBlock.SetDelay(data, newDelay);
				if (num != data)
				{
					int value2 = Terrain.ReplaceData(value, num);
					this.SubsystemTerrain.ChangeCell(x, y, z, value2, true);
					int face = ((AdjustableDelayGateBlock)BlocksManager.Blocks[224]).GetFace(value);
					SubsystemElectricity subsystemElectricity = this.Project.FindSubsystem<SubsystemElectricity>(true);
					ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
					if (electricElement != null)
					{
						subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
					}
				}
			}));
			return true;
		}
	}
}
