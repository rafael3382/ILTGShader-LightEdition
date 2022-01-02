using System;
using Engine;

namespace Game
{
	// Token: 0x02000316 RID: 790
	public class SwitchElectricElement : MountedElectricElement
	{
		// Token: 0x06001722 RID: 5922 RVA: 0x000AE2BD File Offset: 0x000AC4BD
		public SwitchElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_voltage = (float)(SwitchBlock.GetLeverState(value) ? 1 : 0);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x000AE2DA File Offset: 0x000AC4DA
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000AE2E4 File Offset: 0x000AC4E4
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = base.CellFaces[0];
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int value = SwitchBlock.SetLeverState(cellValue, !SwitchBlock.GetLeverState(cellValue));
			base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			return true;
		}

		// Token: 0x04000FD9 RID: 4057
		public float m_voltage;
	}
}
