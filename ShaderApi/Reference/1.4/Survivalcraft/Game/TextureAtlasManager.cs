using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000138 RID: 312
	public static class TextureAtlasManager
	{
		// Token: 0x060005DF RID: 1503 RVA: 0x00021897 File Offset: 0x0001FA97
		public static void Clear()
		{
			TextureAtlasManager.m_subtextures.Clear();
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x000218A4 File Offset: 0x0001FAA4
		public static void Initialize()
		{
			Texture2D atlasTexture_ = ContentManager.Get<Texture2D>("Atlases/AtlasTexture", null);
			string atlas = ContentManager.Get<string>("Atlases/Atlas", null);
			TextureAtlasManager.LoadAtlases(atlasTexture_, atlas);
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000218CE File Offset: 0x0001FACE
		public static void LoadAtlases(Texture2D AtlasTexture_, string Atlas)
		{
			TextureAtlasManager.Clear();
			TextureAtlasManager.AtlasTexture = AtlasTexture_;
			TextureAtlasManager.LoadTextureAtlas(TextureAtlasManager.AtlasTexture, Atlas, "Textures/Atlas/");
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x000218EC File Offset: 0x0001FAEC
		public static Subtexture GetSubtexture(string name)
		{
			Subtexture subtexture;
			if (!TextureAtlasManager.m_subtextures.TryGetValue(name, out subtexture))
			{
				try
				{
					subtexture = new Subtexture(ContentManager.Get(typeof(Texture2D), name, null) as Texture2D, Vector2.Zero, Vector2.One);
					TextureAtlasManager.m_subtextures.Add(name, subtexture);
					return subtexture;
				}
				catch (Exception innerException)
				{
					throw new InvalidOperationException("Required subtexture " + name + " not found in TextureAtlasManager.", innerException);
				}
				return subtexture;
			}
			return subtexture;
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0002196C File Offset: 0x0001FB6C
		public static void LoadTextureAtlas(Texture2D texture, string atlasDefinition, string prefix)
		{
			string[] array = atlasDefinition.Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length < 5)
				{
					throw new InvalidOperationException("Invalid texture atlas definition.");
				}
				string key = prefix + array2[0];
				int num = int.Parse(array2[1], CultureInfo.InvariantCulture);
				int num2 = int.Parse(array2[2], CultureInfo.InvariantCulture);
				int num3 = int.Parse(array2[3], CultureInfo.InvariantCulture);
				int num4 = int.Parse(array2[4], CultureInfo.InvariantCulture);
				Vector2 topLeft = new Vector2((float)num / (float)texture.Width, (float)num2 / (float)texture.Height);
				Vector2 bottomRight = new Vector2((float)(num + num3) / (float)texture.Width, (float)(num2 + num4) / (float)texture.Height);
				Subtexture value = new Subtexture(texture, topLeft, bottomRight);
				TextureAtlasManager.m_subtextures.Add(key, value);
			}
		}

		// Token: 0x040002A4 RID: 676
		public static Dictionary<string, Subtexture> m_subtextures = new Dictionary<string, Subtexture>();

		// Token: 0x040002A5 RID: 677
		public static Texture2D AtlasTexture;
	}
}
