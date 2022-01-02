using System;
using Engine;

namespace Game
{
	// Token: 0x020002BA RID: 698
	public class LedElectricElement : MountedElectricElement
	{
		// Token: 0x06001574 RID: 5492 RVA: 0x000A19C7 File Offset: 0x0009FBC7
		public LedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x000A19E4 File Offset: 0x0009FBE4
		public override void OnAdded()
		{
			this.m_glowPoint = this.m_subsystemGlow.AddGlowPoint();
			CellFace cellFace = base.CellFaces[0];
			int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = LedBlock.GetMountingFace(data);
			this.m_color = LedBlock.LedColors[LedBlock.GetColor(data)];
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

		// Token: 0x06001576 RID: 5494 RVA: 0x000A1B4A File Offset: 0x0009FD4A
		public override void OnRemoved()
		{
			this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoint);
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x000A1B60 File Offset: 0x0009FD60
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateVoltage();
			if (ElectricElement.IsSignalHigh(this.m_voltage) != ElectricElement.IsSignalHigh(voltage))
			{
				this.m_glowPoint.Color = (ElectricElement.IsSignalHigh(this.m_voltage) ? this.m_color : Color.Transparent);
			}
			return false;
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x000A1BB9 File Offset: 0x0009FDB9
		public float CalculateVoltage()
		{
			return (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
		}

		// Token: 0x04000DF5 RID: 3573
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000DF6 RID: 3574
		public float m_voltage;

		// Token: 0x04000DF7 RID: 3575
		public GlowPoint m_glowPoint;

		// Token: 0x04000DF8 RID: 3576
		public Color m_color;
	}
}
