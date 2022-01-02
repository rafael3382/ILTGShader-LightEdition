using System;
using Engine;

namespace Game
{
	// Token: 0x020002B4 RID: 692
	public interface ITerrainContentsGenerator
	{
		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600155F RID: 5471
		int OceanLevel { get; }

		// Token: 0x06001560 RID: 5472
		Vector3 FindCoarseSpawnPosition();

		// Token: 0x06001561 RID: 5473
		float CalculateOceanShoreDistance(float x, float z);

		// Token: 0x06001562 RID: 5474
		float CalculateHeight(float x, float z);

		// Token: 0x06001563 RID: 5475
		int CalculateTemperature(float x, float z);

		// Token: 0x06001564 RID: 5476
		int CalculateHumidity(float x, float z);

		// Token: 0x06001565 RID: 5477
		float CalculateMountainRangeFactor(float x, float z);

		// Token: 0x06001566 RID: 5478
		void GenerateChunkContentsPass1(TerrainChunk chunk);

		// Token: 0x06001567 RID: 5479
		void GenerateChunkContentsPass2(TerrainChunk chunk);

		// Token: 0x06001568 RID: 5480
		void GenerateChunkContentsPass3(TerrainChunk chunk);

		// Token: 0x06001569 RID: 5481
		void GenerateChunkContentsPass4(TerrainChunk chunk);
	}
}
