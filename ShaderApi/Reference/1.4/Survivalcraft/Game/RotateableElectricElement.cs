using System;
using Engine;

namespace Game
{
	// Token: 0x020002F9 RID: 761
	public abstract class RotateableElectricElement : MountedElectricElement
	{
		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060016A0 RID: 5792 RVA: 0x000AA470 File Offset: 0x000A8670
		// (set) Token: 0x060016A1 RID: 5793 RVA: 0x000AA4C0 File Offset: 0x000A86C0
		public int Rotation
		{
			get
			{
				CellFace cellFace = base.CellFaces[0];
				return RotateableMountedElectricElementBlock.GetRotation(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
			}
			set
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int value2 = Terrain.ReplaceData(cellValue, RotateableMountedElectricElementBlock.SetRotation(Terrain.ExtractData(cellValue), value % 4));
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value2, true);
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			}
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x000AA57B File Offset: 0x000A877B
		public RotateableElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x000AA588 File Offset: 0x000A8788
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int rotation = this.Rotation + 1;
			this.Rotation = rotation;
			return true;
		}
	}
}
