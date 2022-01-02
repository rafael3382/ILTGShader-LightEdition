using System;
using Engine;

namespace Game
{
	// Token: 0x020002F2 RID: 754
	public class RandomGeneratorElectricElement : RotateableElectricElement
	{
		// Token: 0x0600168D RID: 5773 RVA: 0x000A9950 File Offset: 0x000A7B50
		public RandomGeneratorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			float? num = base.SubsystemElectricity.ReadPersistentVoltage(base.CellFaces[0].Point);
			this.m_voltage = ((num != null) ? num.Value : RandomGeneratorElectricElement.GetRandomVoltage());
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x000A99AC File Offset: 0x000A7BAC
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x000A99B4 File Offset: 0x000A7BB4
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			bool flag = false;
			bool flag2 = false;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					if (ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
					{
						if (this.m_clockAllowed)
						{
							flag = true;
							this.m_clockAllowed = false;
						}
					}
					else
					{
						this.m_clockAllowed = true;
					}
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (flag)
				{
					this.m_voltage = RandomGeneratorElectricElement.GetRandomVoltage();
				}
			}
			else
			{
				this.m_voltage = RandomGeneratorElectricElement.GetRandomVoltage();
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(RandomGeneratorElectricElement.s_random.Float(0.25f, 0.75f) / 0.01f), 1));
			}
			if (this.m_voltage != voltage)
			{
				base.SubsystemElectricity.WritePersistentVoltage(base.CellFaces[0].Point, this.m_voltage);
				return true;
			}
			return false;
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x000A9AEC File Offset: 0x000A7CEC
		public static float GetRandomVoltage()
		{
			return (float)RandomGeneratorElectricElement.s_random.Int(0, 15) / 15f;
		}

		// Token: 0x04000F41 RID: 3905
		public bool m_clockAllowed = true;

		// Token: 0x04000F42 RID: 3906
		public float m_voltage;

		// Token: 0x04000F43 RID: 3907
		public static Game.Random s_random = new Game.Random();
	}
}
