using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000194 RID: 404
	public class SubsystemCarpetBlockBehavior : SubsystemPollableBlockBehavior
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000965 RID: 2405 RVA: 0x0003B38A File Offset: 0x0003958A
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0003B392 File Offset: 0x00039592
		public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0003B3B0 File Offset: 0x000395B0
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0003B3FC File Offset: 0x000395FC
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_random.Float(0f, 1f) < 0.25f)
			{
				PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
				if (precipitationShaftInfo.Intensity > 0f && y >= precipitationShaftInfo.YLimit - 1)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, true, false);
				}
			}
		}

		// Token: 0x040004BA RID: 1210
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x040004BB RID: 1211
		public Random m_random = new Random();
	}
}
