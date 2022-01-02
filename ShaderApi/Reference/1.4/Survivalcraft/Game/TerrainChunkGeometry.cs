using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000135 RID: 309
	public class TerrainChunkGeometry : IDisposable
	{
		// Token: 0x060005CB RID: 1483 RVA: 0x00020F70 File Offset: 0x0001F170
		public TerrainChunkGeometry()
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i] = new TerrainChunkSliceGeometry();
			}
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00020FBC File Offset: 0x0001F1BC
		public void Dispose()
		{
			foreach (TerrainChunkGeometry.Buffer buffer in this.Buffers)
			{
				buffer.Dispose();
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0002100C File Offset: 0x0001F20C
		public void InvalidateSliceContentsHashes()
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i].ContentsHash = 0;
			}
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0002103C File Offset: 0x0001F23C
		public void CopySliceContentsHashes(TerrainChunk chunk)
		{
			for (int i = 0; i < this.Slices.Length; i++)
			{
				this.Slices[i].ContentsHash = chunk.SliceContentsHashes[i];
			}
		}

		// Token: 0x04000291 RID: 657
		public const int SubsetsCount = 7;

		// Token: 0x04000292 RID: 658
		public TerrainChunkSliceGeometry[] Slices = new TerrainChunkSliceGeometry[16];

		// Token: 0x04000293 RID: 659
		public DynamicArray<TerrainChunkGeometry.Buffer> Buffers = new DynamicArray<TerrainChunkGeometry.Buffer>();

		// Token: 0x02000413 RID: 1043
		public class Buffer : IDisposable
		{
			// Token: 0x06001F28 RID: 7976 RVA: 0x000E46D5 File Offset: 0x000E28D5
			public void Dispose()
			{
				Utilities.Dispose<VertexBuffer>(ref this.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.IndexBuffer);
			}

			// Token: 0x0400151C RID: 5404
			public VertexBuffer VertexBuffer;

			// Token: 0x0400151D RID: 5405
			public IndexBuffer IndexBuffer;

			// Token: 0x0400151E RID: 5406
			public Texture2D Texture;

			// Token: 0x0400151F RID: 5407
			public int[] SubsetIndexBufferStarts = new int[7];

			// Token: 0x04001520 RID: 5408
			public int[] SubsetIndexBufferEnds = new int[7];

			// Token: 0x04001521 RID: 5409
			public int[] SubsetVertexBufferStarts = new int[7];
		}
	}
}
