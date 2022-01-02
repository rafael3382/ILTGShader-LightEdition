using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F5 RID: 245
	public class SoilBlock : CubeBlock
	{
		// Token: 0x060004CD RID: 1229 RVA: 0x0001B1C8 File Offset: 0x000193C8
		public override int GetFaceTextureSlot(int face, int value)
		{
			int nitrogen = SoilBlock.GetNitrogen(Terrain.ExtractData(value));
			if (face != 4)
			{
				return 2;
			}
			if (nitrogen <= 0)
			{
				return 37;
			}
			return 53;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001B1F0 File Offset: 0x000193F0
		public static bool GetHydration(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001B1F8 File Offset: 0x000193F8
		public static int GetNitrogen(int data)
		{
			return data >> 1 & 3;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001B1FF File Offset: 0x000193FF
		public static int SetHydration(int data, bool hydration)
		{
			if (!hydration)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001B20C File Offset: 0x0001940C
		public static int SetNitrogen(int data, int nitrogen)
		{
			nitrogen = MathUtils.Clamp(nitrogen, 0, 3);
			return (data & -7) | (nitrogen & 3) << 1;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001B222 File Offset: 0x00019422
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(0, false));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(0, true));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(SoilBlock.SetNitrogen(0, 3), true));
			yield break;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001B234 File Offset: 0x00019434
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int nitrogen = SoilBlock.GetNitrogen(data);
			bool hydration = SoilBlock.GetHydration(data);
			if (nitrogen > 0 && hydration)
			{
				LanguageControl.Get(SoilBlock.fName, 2);
				return LanguageControl.Get(SoilBlock.fName, 1);
			}
			if (nitrogen > 0)
			{
				LanguageControl.Get(SoilBlock.fName, 2);
				return LanguageControl.Get(SoilBlock.fName, 2);
			}
			if (hydration)
			{
				return LanguageControl.Get(SoilBlock.fName, 3);
			}
			return LanguageControl.Get(SoilBlock.fName, 4);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001B2AC File Offset: 0x000194AC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = SoilBlock.GetHydration(Terrain.ExtractData(value)) ? new Color(180, 170, 150) : Color.White;
			generator.GenerateCubeVertices(this, value, x, y, z, 0.9375f, 0.9375f, 0.9375f, 0.9375f, color, color, color, color, color, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001B310 File Offset: 0x00019510
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			Color c = SoilBlock.GetHydration(Terrain.ExtractData(value)) ? new Color(180, 170, 150) : Color.White;
			base.DrawBlock(primitivesRenderer, value, color * c, size, ref matrix, environmentData);
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001B35B File Offset: 0x0001955B
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return SoilBlock.m_collisionBoxes;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001B362 File Offset: 0x00019562
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != 5;
		}

		// Token: 0x04000222 RID: 546
		public const int Index = 168;

		// Token: 0x04000223 RID: 547
		public new static string fName = "SoilBlock";

		// Token: 0x04000224 RID: 548
		public static BoundingBox[] m_collisionBoxes = new BoundingBox[]
		{
			new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.9375f, 1f))
		};
	}
}
