using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C0 RID: 448
	public class SubsystemPalette : Subsystem
	{
		// Token: 0x06000BA4 RID: 2980 RVA: 0x0004FE4B File Offset: 0x0004E04B
		static SubsystemPalette()
		{
			SubsystemPalette.m_defaultFabricColors = SubsystemPalette.CreateFabricColors(WorldPalette.DefaultColors);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0004FE68 File Offset: 0x0004E068
		public Color GetColor(int index)
		{
			return this.m_colors[index];
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0004FE76 File Offset: 0x0004E076
		public string GetName(int index)
		{
			return this.m_names[index];
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0004FE80 File Offset: 0x0004E080
		public Color GetFabricColor(int index)
		{
			return this.m_fabricColors[index];
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0004FE8E File Offset: 0x0004E08E
		public static Color GetColor(BlockGeometryGenerator generator, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (generator.SubsystemPalette != null)
			{
				return generator.SubsystemPalette.GetColor(index.Value);
			}
			return WorldPalette.DefaultColors[index.Value];
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0004FECB File Offset: 0x0004E0CB
		public static Color GetColor(DrawBlockEnvironmentData environmentData, int? index)
		{
			return SubsystemPalette.GetColor(environmentData.SubsystemTerrain, index);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0004FED9 File Offset: 0x0004E0D9
		public static Color GetColor(SubsystemTerrain subsystemTerrain, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (subsystemTerrain != null && subsystemTerrain.SubsystemPalette != null)
			{
				return subsystemTerrain.SubsystemPalette.GetColor(index.Value);
			}
			return WorldPalette.DefaultColors[index.Value];
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0004FF19 File Offset: 0x0004E119
		public static Color GetFabricColor(BlockGeometryGenerator generator, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (generator.SubsystemPalette != null)
			{
				return generator.SubsystemPalette.GetFabricColor(index.Value);
			}
			return SubsystemPalette.m_defaultFabricColors[index.Value];
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0004FF56 File Offset: 0x0004E156
		public static Color GetFabricColor(DrawBlockEnvironmentData environmentData, int? index)
		{
			return SubsystemPalette.GetFabricColor(environmentData.SubsystemTerrain, index);
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0004FF64 File Offset: 0x0004E164
		public static Color GetFabricColor(SubsystemTerrain subsystemTerrain, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (subsystemTerrain != null && subsystemTerrain.SubsystemPalette != null)
			{
				return subsystemTerrain.SubsystemPalette.GetFabricColor(index.Value);
			}
			return SubsystemPalette.m_defaultFabricColors[index.Value];
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x0004FFA4 File Offset: 0x0004E1A4
		public static string GetName(SubsystemTerrain subsystemTerrain, int? index, string suffix)
		{
			if (index == null)
			{
				return suffix ?? string.Empty;
			}
			string worldPalette = LanguageControl.GetWorldPalette(index.Value);
			if (!string.IsNullOrEmpty(suffix))
			{
				return worldPalette + " " + suffix;
			}
			return worldPalette;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0004FFE8 File Offset: 0x0004E1E8
		public override void Load(ValuesDictionary valuesDictionary)
		{
			SubsystemGameInfo subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_colors = subsystemGameInfo.WorldSettings.Palette.Colors.ToArray<Color>();
			this.m_names = subsystemGameInfo.WorldSettings.Palette.Names.ToArray<string>();
			this.m_fabricColors = SubsystemPalette.CreateFabricColors(this.m_colors);
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0005004C File Offset: 0x0004E24C
		public static Color[] CreateFabricColors(Color[] colors)
		{
			Color[] array = new Color[16];
			for (int i = 0; i < 16; i++)
			{
				Vector3 rgb = new Vector3(colors[i]);
				Vector3 hsv = Color.RgbToHsv(rgb);
				hsv.Y *= 0.85f;
				rgb = Color.HsvToRgb(hsv);
				array[i] = new Color(rgb);
			}
			return array;
		}

		// Token: 0x040005B9 RID: 1465
		public static readonly Color[] m_defaultFabricColors = new Color[16];

		// Token: 0x040005BA RID: 1466
		public string[] m_names;

		// Token: 0x040005BB RID: 1467
		public Color[] m_colors;

		// Token: 0x040005BC RID: 1468
		public Color[] m_fabricColors;
	}
}
