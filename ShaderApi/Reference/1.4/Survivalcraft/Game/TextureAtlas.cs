using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000137 RID: 311
	public class TextureAtlas
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x000216EF File Offset: 0x0001F8EF
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000216F8 File Offset: 0x0001F8F8
		public TextureAtlas(Texture2D texture, string atlasDefinition, string prefix)
		{
			this.m_texture = texture;
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
				Rectangle value = new Rectangle
				{
					Left = int.Parse(array2[1], CultureInfo.InvariantCulture),
					Top = int.Parse(array2[2], CultureInfo.InvariantCulture),
					Width = int.Parse(array2[3], CultureInfo.InvariantCulture),
					Height = int.Parse(array2[4], CultureInfo.InvariantCulture)
				};
				this.m_rectangles.Add(key, value);
			}
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x000217E3 File Offset: 0x0001F9E3
		public bool ContainsTexture(string textureName)
		{
			return this.m_rectangles.ContainsKey(textureName);
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x000217F4 File Offset: 0x0001F9F4
		public Vector4? GetTextureCoordinates(string textureName)
		{
			Rectangle rectangle;
			if (this.m_rectangles.TryGetValue(textureName, out rectangle))
			{
				return new Vector4?(new Vector4
				{
					X = (float)rectangle.Left / (float)this.m_texture.Width,
					Y = (float)rectangle.Top / (float)this.m_texture.Height,
					Z = (float)rectangle.Right / (float)this.m_texture.Width,
					W = (float)rectangle.Bottom / (float)this.m_texture.Height
				});
			}
			return null;
		}

		// Token: 0x040002A2 RID: 674
		public Texture2D m_texture;

		// Token: 0x040002A3 RID: 675
		public Dictionary<string, Rectangle> m_rectangles = new Dictionary<string, Rectangle>();
	}
}
