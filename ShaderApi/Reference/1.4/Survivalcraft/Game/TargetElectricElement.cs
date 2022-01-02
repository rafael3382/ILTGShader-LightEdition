using System;
using Engine;

namespace Game
{
	// Token: 0x02000318 RID: 792
	public class TargetElectricElement : MountedElectricElement
	{
		// Token: 0x06001728 RID: 5928 RVA: 0x000AE479 File Offset: 0x000AC679
		public TargetElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x000AE483 File Offset: 0x000AC683
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x000AE48C File Offset: 0x000AC68C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			if (this.m_score > 0)
			{
				this.m_voltage = (float)(this.m_score + 7) / 15f;
				this.m_score = 0;
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 50);
			}
			else
			{
				this.m_voltage = 0f;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x000AE4F8 File Offset: 0x000AC6F8
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (this.m_score == 0 && !ElectricElement.IsSignalHigh(this.m_voltage))
			{
				if (cellFace.Face == 0 || cellFace.Face == 2)
				{
					float num = worldItem.Position.X - (float)cellFace.X - 0.5f;
					float num2 = worldItem.Position.Y - (float)cellFace.Y - 0.5f;
					float num3 = MathUtils.Sqrt(num * num + num2 * num2);
					this.m_score = MathUtils.Clamp((int)MathUtils.Round(8f * (1f - num3 / 0.707f)), 1, 8);
				}
				else
				{
					float num4 = worldItem.Position.Z - (float)cellFace.Z - 0.5f;
					float num5 = worldItem.Position.Y - (float)cellFace.Y - 0.5f;
					float num6 = MathUtils.Sqrt(num4 * num4 + num5 * num5);
					this.m_score = MathUtils.Clamp((int)MathUtils.Round(8f * (1f - num6 / 0.5f)), 1, 8);
				}
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
			}
		}

		// Token: 0x04000FDB RID: 4059
		public float m_voltage;

		// Token: 0x04000FDC RID: 4060
		public int m_score;
	}
}
