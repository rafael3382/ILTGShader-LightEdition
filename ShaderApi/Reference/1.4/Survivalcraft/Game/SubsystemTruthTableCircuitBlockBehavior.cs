using System;
using Engine;

namespace Game
{
	// Token: 0x020001DF RID: 479
	public class SubsystemTruthTableCircuitBlockBehavior : SubsystemEditableItemBehavior<TruthTableData>
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0005ECA1 File Offset: 0x0005CEA1
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					188
				};
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0005ECB1 File Offset: 0x0005CEB1
		public SubsystemTruthTableCircuitBlockBehavior() : base(188)
		{
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x0005ECC0 File Offset: 0x0005CEC0
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			int value = inventory.GetSlotValue(slotIndex);
			int count = inventory.GetSlotCount(slotIndex);
			int id = Terrain.ExtractData(value);
			TruthTableData truthTableData = base.GetItemData(id);
			truthTableData = ((truthTableData != null) ? ((TruthTableData)truthTableData.Copy()) : new TruthTableData());
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditTruthTableDialog(truthTableData, delegate(bool <p0>)
			{
				int data = this.StoreItemDataAtUniqueId(truthTableData);
				int value = Terrain.ReplaceData(value, data);
				inventory.RemoveSlotItems(slotIndex, count);
				inventory.AddSlotItems(slotIndex, value, 1);
			}));
			return true;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0005ED78 File Offset: 0x0005CF78
		public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			TruthTableData truthTableData = base.GetBlockData(new Point3(x, y, z)) ?? new TruthTableData();
			DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditTruthTableDialog(truthTableData, delegate(bool <p0>)
			{
				this.SetBlockData(new Point3(x, y, z), truthTableData);
				int face = ((TruthTableCircuitBlock)BlocksManager.Blocks[188]).GetFace(value);
				SubsystemElectricity subsystemElectricity = this.SubsystemTerrain.Project.FindSubsystem<SubsystemElectricity>(true);
				ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
				if (electricElement != null)
				{
					subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
				}
			}));
			return true;
		}
	}
}
