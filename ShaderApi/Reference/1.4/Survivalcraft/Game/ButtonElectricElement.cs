using System;
using Engine;

namespace Game
{
	// Token: 0x02000255 RID: 597
	public class ButtonElectricElement : MountedElectricElement
	{
		// Token: 0x0600138F RID: 5007 RVA: 0x00093A65 File Offset: 0x00091C65
		public ButtonElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x00093A70 File Offset: 0x00091C70
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

		// Token: 0x06001391 RID: 5009 RVA: 0x00093B01 File Offset: 0x00091D01
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x00093B0C File Offset: 0x00091D0C
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

		// Token: 0x06001393 RID: 5011 RVA: 0x00093B6D File Offset: 0x00091D6D
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			this.Press();
			return true;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x00093B76 File Offset: 0x00091D76
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Press();
		}

		// Token: 0x04000C48 RID: 3144
		public float m_voltage;

		// Token: 0x04000C49 RID: 3145
		public bool m_wasPressed;
	}
}
