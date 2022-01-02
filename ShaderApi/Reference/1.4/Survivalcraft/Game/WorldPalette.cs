using System;
using System.Linq;
using Engine;
using Engine.Serialization;
using SimpleJson;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200035E RID: 862
	public class WorldPalette
	{
		// Token: 0x06001921 RID: 6433 RVA: 0x000C591C File Offset: 0x000C3B1C
		public WorldPalette()
		{
			this.Colors = WorldPalette.DefaultColors.ToArray<Color>();
			JsonArray jsonArray = (LanguageControl.KeyWords[base.GetType().Name] as JsonObject)["Colors"] as JsonArray;
			this.Names = new string[jsonArray.Count];
			int num = 0;
			foreach (object obj in jsonArray)
			{
				this.Names[num++] = obj.ToString();
			}
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x000C59C8 File Offset: 0x000C3BC8
		public WorldPalette(ValuesDictionary valuesDictionary)
		{
			string[] array = valuesDictionary.GetValue<string>("Colors", new string(';', 15)).Split(new char[]
			{
				';'
			});
			if (array.Length != 16)
			{
				throw new InvalidOperationException(LanguageControl.Get(base.GetType().Name, 0));
			}
			this.Colors = array.Select(delegate(string s, int i)
			{
				if (string.IsNullOrEmpty(s))
				{
					return WorldPalette.DefaultColors[i];
				}
				return HumanReadableConverter.ConvertFromString<Color>(s);
			}).ToArray<Color>();
			string[] array2 = valuesDictionary.GetValue<string>("Names", new string(';', 15)).Split(new char[]
			{
				';'
			});
			if (array2.Length != 16)
			{
				throw new InvalidOperationException(LanguageControl.Get(base.GetType().Name, 1));
			}
			this.Names = array2.Select(delegate(string s, int i)
			{
				if (string.IsNullOrEmpty(s))
				{
					return LanguageControl.GetWorldPalette(i);
				}
				return s;
			}).ToArray<string>();
			string[] names = this.Names;
			for (int j = 0; j < names.Length; j++)
			{
				if (!WorldPalette.VerifyColorName(names[j]))
				{
					throw new InvalidOperationException(LanguageControl.Get(base.GetType().Name, 2));
				}
			}
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x000C5AF8 File Offset: 0x000C3CF8
		public ValuesDictionary Save()
		{
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			string value = string.Join(";", this.Colors.Select(delegate(Color c, int i)
			{
				if (c == WorldPalette.DefaultColors[i])
				{
					return string.Empty;
				}
				return HumanReadableConverter.ConvertToString(c);
			}));
			string value2 = string.Join(";", this.Names.Select(delegate(string n, int i)
			{
				if (n == LanguageControl.Get(base.GetType().Name, i))
				{
					return string.Empty;
				}
				return n;
			}));
			valuesDictionary.SetValue<string>("Colors", value);
			valuesDictionary.SetValue<string>("Names", value2);
			return valuesDictionary;
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x000C5B79 File Offset: 0x000C3D79
		public void CopyTo(WorldPalette palette)
		{
			palette.Colors = this.Colors.ToArray<Color>();
			palette.Names = this.Names.ToArray<string>();
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x000C5BA0 File Offset: 0x000C3DA0
		public static bool VerifyColorName(string name)
		{
			if (name.Length < 1 || name.Length > 16)
			{
				return false;
			}
			foreach (char c in name)
			{
				if (!char.IsLetterOrDigit(c) && c != '-' && c != ' ')
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04001141 RID: 4417
		public const int MaxColors = 16;

		// Token: 0x04001142 RID: 4418
		public const int MaxNameLength = 16;

		// Token: 0x04001143 RID: 4419
		public static readonly Color[] DefaultColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(181, 255, 255),
			new Color(255, 181, 255),
			new Color(160, 181, 255),
			new Color(255, 240, 160),
			new Color(181, 255, 181),
			new Color(255, 181, 160),
			new Color(181, 181, 181),
			new Color(112, 112, 112),
			new Color(32, 112, 112),
			new Color(112, 32, 112),
			new Color(26, 52, 128),
			new Color(87, 54, 31),
			new Color(24, 116, 24),
			new Color(136, 32, 32),
			new Color(24, 24, 24)
		};

		// Token: 0x04001144 RID: 4420
		public Color[] Colors;

		// Token: 0x04001145 RID: 4421
		public string[] Names;
	}
}
