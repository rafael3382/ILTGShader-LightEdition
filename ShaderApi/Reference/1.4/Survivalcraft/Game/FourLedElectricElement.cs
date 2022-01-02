using System;
using Engine;

namespace Game
{
	// Token: 0x02000295 RID: 661
	public class FourLedElectricElement : MountedElectricElement
	{
		// Token: 0x060014A5 RID: 5285 RVA: 0x0009AFDF File Offset: 0x000991DF
		public FourLedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x0009B008 File Offset: 0x00099208
		public override void OnAdded()
		{
			CellFace cellFace = base.CellFaces[0];
			int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = FourLedBlock.GetMountingFace(data);
			this.m_color = LedBlock.LedColors[FourLedBlock.GetColor(data)];
			for (int i = 0; i < 4; i++)
			{
				int num = (i % 2 == 0) ? 1 : -1;
				int num2 = (i / 2 == 0) ? 1 : -1;
				Vector3 v = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f);
				Vector3 vector = CellFace.FaceToVector3(mountingFace);
				Vector3 vector2 = (mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX;
				Vector3 vector3 = Vector3.Cross(vector, vector2);
				this.m_glowPoints[i] = this.m_subsystemGlow.AddGlowPoint();
				this.m_glowPoints[i].Position = v - 0.4375f * CellFace.FaceToVector3(mountingFace) + 0.25f * vector3 * (float)num + 0.25f * vector2 * (float)num2;
				this.m_glowPoints[i].Forward = vector;
				this.m_glowPoints[i].Up = vector2;
				this.m_glowPoints[i].Right = vector3;
				this.m_glowPoints[i].Color = Color.Transparent;
				this.m_glowPoints[i].Size = 0.26f;
				this.m_glowPoints[i].FarSize = 0.26f;
				this.m_glowPoints[i].FarDistance = 1f;
				this.m_glowPoints[i].Type = GlowPointType.Square;
			}
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x0009B1E8 File Offset: 0x000993E8
		public override void OnRemoved()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoints[i]);
			}
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x0009B214 File Offset: 0x00099414
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					this.m_voltage = MathUtils.Max(this.m_voltage, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			if (this.m_voltage != voltage)
			{
				int num = (int)MathUtils.Round(this.m_voltage * 15f);
				for (int i = 0; i < 4; i++)
				{
					this.m_glowPoints[i].Color = (((num & 1 << i) != 0) ? this.m_color : Color.Transparent);
				}
			}
			return false;
		}

		// Token: 0x04000D7D RID: 3453
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000D7E RID: 3454
		public float m_voltage;

		// Token: 0x04000D7F RID: 3455
		public Color m_color;

		// Token: 0x04000D80 RID: 3456
		public GlowPoint[] m_glowPoints = new GlowPoint[4];
	}
}
