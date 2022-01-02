using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000298 RID: 664
	public abstract class FurnitureElectricElement : ElectricElement
	{
		// Token: 0x060014E5 RID: 5349 RVA: 0x0009E175 File Offset: 0x0009C375
		public FurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, FurnitureElectricElement.GetMountingCellFaces(subsystemElectricity, point))
		{
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0009E185 File Offset: 0x0009C385
		public static IEnumerable<CellFace> GetMountingCellFaces(SubsystemElectricity subsystemElectricity, Point3 point)
		{
			int data = Terrain.ExtractData(subsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z));
			int rotation = FurnitureBlock.GetRotation(data);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				yield break;
			}
			int num2;
			for (int face = 0; face < 6; face = num2)
			{
				int num = (face < 4) ? ((face - rotation + 4) % 4) : face;
				if ((design.MountingFacesMask & 1 << num) != 0)
				{
					yield return new CellFace(point.X, point.Y, point.Z, CellFace.OppositeFace(face));
				}
				num2 = face + 1;
			}
			yield break;
		}
	}
}
