using System;
using Engine;

namespace Game
{
	// Token: 0x02000317 RID: 791
	public class SwitchFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x06001725 RID: 5925 RVA: 0x000AE39C File Offset: 0x000AC59C
		public SwitchFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point, int value) : base(subsystemElectricity, point)
		{
			FurnitureDesign design = FurnitureBlock.GetDesign(subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior, value);
			if (design != null && design.LinkedDesign != null)
			{
				this.m_voltage = (float)((design.Index >= design.LinkedDesign.Index) ? 1 : 0);
			}
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x000AE3EC File Offset: 0x000AC5EC
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x000AE3F4 File Offset: 0x000AC5F4
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = base.CellFaces[0];
			base.SubsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.SwitchToNextState(cellFace.X, cellFace.Y, cellFace.Z, false);
			base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			return true;
		}

		// Token: 0x04000FDA RID: 4058
		public float m_voltage;
	}
}
