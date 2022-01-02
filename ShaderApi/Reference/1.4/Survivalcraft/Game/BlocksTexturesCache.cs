using System;
using System.Collections.Generic;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200024E RID: 590
	public class BlocksTexturesCache
	{
		// Token: 0x06001371 RID: 4977 RVA: 0x00092940 File Offset: 0x00090B40
		public Texture2D GetTexture(string name)
		{
			Texture2D texture2D;
			if (!this.m_textures.TryGetValue(name, out texture2D))
			{
				texture2D = BlocksTexturesManager.LoadTexture(name);
				this.m_textures.Add(name, texture2D);
			}
			return texture2D;
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x00092974 File Offset: 0x00090B74
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

		// Token: 0x04000C23 RID: 3107
		public Dictionary<string, Texture2D> m_textures = new Dictionary<string, Texture2D>();
	}
}
