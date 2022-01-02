using System;
using Engine;

namespace Game
{
	// Token: 0x020002CC RID: 716
	public class MulticoloredLedElectricElement : MountedElectricElement
	{
		// Token: 0x060015BA RID: 5562 RVA: 0x000A3A84 File Offset: 0x000A1C84
		public MulticoloredLedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x000A3AA0 File Offset: 0x000A1CA0
		public override void OnAdded()
		{
			this.m_glowPoint = this.m_subsystemGlow.AddGlowPoint();
			CellFace cellFace = base.CellFaces[0];
			int mountingFace = MulticoloredLedBlock.GetMountingFace(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
			Vector3 v = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f);
			this.m_glowPoint.Position = v - 0.4375f * CellFace.FaceToVector3(mountingFace);
			this.m_glowPoint.Forward = CellFace.FaceToVector3(mountingFace);
			this.m_glowPoint.Up = ((mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX);
			this.m_glowPoint.Right = Vector3.Cross(this.m_glowPoint.Forward, this.m_glowPoint.Up);
			this.m_glowPoint.Color = Color.Transparent;
			this.m_glowPoint.Size = 0.0324f;
			this.m_glowPoint.FarSize = 0.0324f;
			this.m_glowPoint.FarDistance = 0f;
			this.m_glowPoint.Type = GlowPointType.Square;
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x000A3BED File Offset: 0x000A1DED
		public override void OnRemoved()
		{
			this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoint);
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x000A3C00 File Offset: 0x000A1E00
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
				this.m_glowPoint.Color = ((num >= 8) ? LedBlock.LedColors[MathUtils.Clamp(num - 8, 0, 7)] : Color.Transparent);
			}
			return false;
		}

		// Token: 0x04000E4A RID: 3658
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000E4B RID: 3659
		public float m_voltage;

		// Token: 0x04000E4C RID: 3660
		public GlowPoint m_glowPoint;
	}
}
