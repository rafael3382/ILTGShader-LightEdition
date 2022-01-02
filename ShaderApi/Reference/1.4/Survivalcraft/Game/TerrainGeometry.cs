using System;
using System.Collections.Generic;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000322 RID: 802
	public class TerrainGeometry
	{
		// Token: 0x060017ED RID: 6125 RVA: 0x000BB544 File Offset: 0x000B9744
		public TerrainGeometry GetGeometry(Texture2D texture)
		{
			if (texture == null)
			{
				throw new Exception("Texture can not be null");
			}
			TerrainGeometry terrainGeometry;
			if (!this.Draws.TryGetValue(texture, out terrainGeometry))
			{
				terrainGeometry = new TerrainGeometry();
				terrainGeometry.Subsets = new TerrainGeometrySubset[7];
				for (int i = 0; i < 7; i++)
				{
					terrainGeometry.Subsets[i] = new TerrainGeometrySubset();
				}
				terrainGeometry.SubsetOpaque = terrainGeometry.Subsets[4];
				terrainGeometry.SubsetAlphaTest = terrainGeometry.Subsets[5];
				terrainGeometry.SubsetTransparent = terrainGeometry.Subsets[6];
				terrainGeometry.OpaqueSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometry.Subsets[0],
					terrainGeometry.Subsets[1],
					terrainGeometry.Subsets[2],
					terrainGeometry.Subsets[3],
					terrainGeometry.Subsets[4],
					terrainGeometry.Subsets[4]
				};
				terrainGeometry.AlphaTestSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometry.Subsets[5],
					terrainGeometry.Subsets[5],
					terrainGeometry.Subsets[5],
					terrainGeometry.Subsets[5],
					terrainGeometry.Subsets[5],
					terrainGeometry.Subsets[5]
				};
				terrainGeometry.TransparentSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometry.Subsets[6],
					terrainGeometry.Subsets[6],
					terrainGeometry.Subsets[6],
					terrainGeometry.Subsets[6],
					terrainGeometry.Subsets[6],
					terrainGeometry.Subsets[6]
				};
				this.Draws.Add(texture, terrainGeometry);
			}
			return terrainGeometry;
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x000BB6C0 File Offset: 0x000B98C0
		public void ClearSubsets(SubsystemAnimatedTextures animatedTextures)
		{
			this.Draws.Clear();
			TerrainGeometry geometry = this.GetGeometry(animatedTextures.AnimatedBlocksTexture);
			this.SubsetOpaque = geometry.SubsetOpaque;
			this.SubsetAlphaTest = geometry.SubsetAlphaTest;
			this.SubsetTransparent = geometry.SubsetOpaque;
			this.OpaqueSubsetsByFace = geometry.OpaqueSubsetsByFace;
			this.AlphaTestSubsetsByFace = geometry.AlphaTestSubsetsByFace;
			this.TransparentSubsetsByFace = geometry.TransparentSubsetsByFace;
			this.Subsets = geometry.Subsets;
		}

		// Token: 0x04001095 RID: 4245
		public TerrainGeometrySubset SubsetOpaque;

		// Token: 0x04001096 RID: 4246
		public TerrainGeometrySubset SubsetAlphaTest;

		// Token: 0x04001097 RID: 4247
		public TerrainGeometrySubset SubsetTransparent;

		// Token: 0x04001098 RID: 4248
		public TerrainGeometrySubset[] OpaqueSubsetsByFace;

		// Token: 0x04001099 RID: 4249
		public TerrainGeometrySubset[] AlphaTestSubsetsByFace;

		// Token: 0x0400109A RID: 4250
		public TerrainGeometrySubset[] TransparentSubsetsByFace;

		// Token: 0x0400109B RID: 4251
		public TerrainGeometrySubset[] Subsets;

		// Token: 0x0400109C RID: 4252
		public Dictionary<Texture2D, TerrainGeometry> Draws = new Dictionary<Texture2D, TerrainGeometry>();
	}
}
