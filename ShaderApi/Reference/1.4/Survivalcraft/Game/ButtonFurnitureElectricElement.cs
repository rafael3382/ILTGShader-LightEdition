using System;
using Engine;

namespace Game
{
	// Token: 0x02000256 RID: 598
	public class ButtonFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x06001395 RID: 5013 RVA: 0x00093B7E File Offset: 0x00091D7E
		public ButtonFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x00093B88 File Offset: 0x00091D88
		public void Press()
		{
			if (!this.m_wasPressed && !ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_wasPressed = true;
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
			}
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x00093C19 File Offset: 0x00091E19
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x00093C24 File Offset: 0x00091E24
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			if (this.m_wasPressed)
			{
				this.m_wasPressed = false;
				this.m_voltage = 1f;
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10);
			}
			else
			{
				this.m_voltage = 0f;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00093C85 File Offset: 0x00091E85
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			this.Press();
			return true;
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x00093C8E File Offset: 0x00091E8E
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Press();
		}

		// Token: 0x04000C4A RID: 3146
		public float m_voltage;

		// Token: 0x04000C4B RID: 3147
		public bool m_wasPressed;
	}
}
