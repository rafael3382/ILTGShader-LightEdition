using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C1 RID: 193
	public abstract class PaintedCubeBlock : CubeBlock, IPaintableBlock
	{
		// Token: 0x060003C8 RID: 968 RVA: 0x0001656A File Offset: 0x0001476A
		public PaintedCubeBlock(int coloredTextureSlot)
		{
			this.m_coloredTextureSlot = coloredTextureSlot;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00016579 File Offset: 0x00014779
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!PaintedCubeBlock.IsColored(Terrain.ExtractData(value)))
			{
				return this.DefaultTextureSlot;
			}
			return this.m_coloredTextureSlot;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00016598 File Offset: 0x00014798
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			Color color = SubsystemPalette.GetColor(generator, PaintedCubeBlock.GetColor(data));
			generator.GenerateCubeVertices(this, value, x, y, z, color, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x000165D0 File Offset: 0x000147D0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			color *= SubsystemPalette.GetColor(environmentData, PaintedCubeBlock.GetColor(data));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001660D File Offset: 0x0001480D
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PaintedCubeBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PaintedCubeBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00016620 File Offset: 0x00014820
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (PaintedCubeBlock.GetColor(data) != null)
			{
				showDebris = true;
				if (toolLevel >= this.RequiredToolLevel)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(this.DefaultDropContent, 0, data),
						Count = (int)this.DefaultDropCount
					});
					return;
				}
			}
			else
			{
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00016694 File Offset: 0x00014894
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int data = Terrain.ExtractData(value);
			Color color = SubsystemPalette.GetColor(subsystemTerrain, PaintedCubeBlock.GetColor(data));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(0, value));
		}

		// Token: 0x060003CF RID: 975 RVA: 0x000166D0 File Offset: 0x000148D0
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			return SubsystemPalette.GetName(subsystemTerrain, PaintedCubeBlock.GetColor(data), LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName"));
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00016718 File Offset: 0x00014918
		public override string GetCategory(int value)
		{
			if (PaintedCubeBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return "Painted";
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00016747 File Offset: 0x00014947
		public int? GetPaintColor(int value)
		{
			return PaintedCubeBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00016754 File Offset: 0x00014954
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, PaintedCubeBlock.SetColor(data, color));
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00016775 File Offset: 0x00014975
		public static bool IsColored(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00016780 File Offset: 0x00014980
		public static int? GetColor(int data)
		{
			if ((data & 1) != 0)
			{
				return new int?(data >> 1 & 15);
			}
			return null;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x000167A7 File Offset: 0x000149A7
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -32) | 1 | color.Value << 1;
			}
			return data & -32;
		}

		// Token: 0x040001B8 RID: 440
		public int m_coloredTextureSlot;
	}
}
