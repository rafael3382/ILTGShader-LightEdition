using System;
using Engine;

namespace Game
{
	// Token: 0x020002E8 RID: 744
	public class PressurePlateElectricElement : MountedElectricElement
	{
		// Token: 0x06001642 RID: 5698 RVA: 0x000A7E36 File Offset: 0x000A6036
		public PressurePlateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x000A7E40 File Offset: 0x000A6040
		public void Press(float pressure)
		{
			this.m_lastPressFrameIndex = Time.FrameIndex;
			if (pressure > this.m_pressure)
			{
				this.m_pressure = pressure;
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0.3f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2.5f, true);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
			}
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x000A7ED0 File Offset: 0x000A60D0
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000A7ED8 File Offset: 0x000A60D8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			if (this.m_pressure > 0f && Time.FrameIndex - this.m_lastPressFrameIndex < 2)
			{
				this.m_voltage = PressurePlateElectricElement.PressureToVoltage(this.m_pressure);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10);
			}
			else
			{
				if (ElectricElement.IsSignalHigh(this.m_voltage))
				{
					CellFace cellFace = base.CellFaces[0];
					base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/BlockPlaced", 0.6f, -0.1f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2.5f, true);
				}
				this.m_voltage = 0f;
				this.m_pressure = 0f;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x000A7FB3 File Offset: 0x000A61B3
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			this.Press(componentBody.Mass);
			componentBody.ApplyImpulse(new Vector3(0f, -2E-05f, 0f));
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x000A7FDC File Offset: 0x000A61DC
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			int num = Terrain.ExtractContents(worldItem.Value);
			Block block = BlocksManager.Blocks[num];
			this.Press(1f * block.GetDensity(worldItem.Value));
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x000A8018 File Offset: 0x000A6218
		public static float PressureToVoltage(float pressure)
		{
			if (pressure <= 0f)
			{
				return 0f;
			}
			if (pressure < 1f)
			{
				return 0.533333361f;
			}
			if (pressure < 2f)
			{
				return 0.6f;
			}
			if (pressure < 5f)
			{
				return 0.6666667f;
			}
			if (pressure < 25f)
			{
				return 0.733333349f;
			}
			if (pressure < 100f)
			{
				return 0.8f;
			}
			if (pressure < 250f)
			{
				return 0.8666667f;
			}
			if (pressure < 500f)
			{
				return 0.933333337f;
			}
			return 1f;
		}

		// Token: 0x04000F0C RID: 3852
		public float m_voltage;

		// Token: 0x04000F0D RID: 3853
		public int m_lastPressFrameIndex;

		// Token: 0x04000F0E RID: 3854
		public float m_pressure;
	}
}
