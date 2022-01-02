using System;
using System.Collections.Generic;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200025B RID: 603
	public class CharacterSkinsCache
	{
		// Token: 0x060013D1 RID: 5073 RVA: 0x0009445D File Offset: 0x0009265D
		public bool ContainsTexture(Texture2D texture)
		{
			return this.m_textures.ContainsValue(texture);
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x0009446C File Offset: 0x0009266C
		public Texture2D GetTexture(string name)
		{
			Texture2D texture2D;
			if (!this.m_textures.TryGetValue(name, out texture2D))
			{
				texture2D = CharacterSkinsManager.LoadTexture(name);
				this.m_textures.Add(name, texture2D);
			}
			return texture2D;
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x000944A0 File Offset: 0x000926A0
		public void Clear()
		{
			foreach (Texture2D texture2D in this.m_textures.Values)
			{
				if (!ContentManager.IsContent(texture2D))
				{
					texture2D.Dispose();
				}
			}
			this.m_textures.Clear();
		}

		// Token: 0x04000C5C RID: 3164
		public Dictionary<string, Texture2D> m_textures = new Dictionary<string, Texture2D>();
	}
}
