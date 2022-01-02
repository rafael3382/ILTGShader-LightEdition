using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000191 RID: 401
	public class SubsystemBulletBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x0003A7FB File Offset: 0x000389FB
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0003A804 File Offset: 0x00038A04
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			BulletBlock.BulletType bulletType = BulletBlock.GetBulletType(Terrain.ExtractData(worldItem.Value));
			bool result = true;
			if (cellFace != null)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z);
				int num = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num];
				if (worldItem.Velocity.Length() > 30f)
				{
					this.m_subsystemExplosions.TryExplodeBlock(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z, cellValue);
				}
				if (block.GetDensity(cellValue) >= 1.5f && worldItem.Velocity.Length() > 30f)
				{
					float num2 = 1f;
					float minDistance = 8f;
					if (bulletType == BulletBlock.BulletType.BuckshotBall)
					{
						num2 = 0.25f;
						minDistance = 4f;
					}
					if (this.m_random.Float(0f, 1f) < num2)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, this.m_random.Float(-0.2f, 0.2f), new Vector3((float)cellFace.Value.X, (float)cellFace.Value.Y, (float)cellFace.Value.Z), minDistance, true);
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0003A970 File Offset: 0x00038B70
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
		}

		// Token: 0x040004A9 RID: 1193
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004AA RID: 1194
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x040004AB RID: 1195
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040004AC RID: 1196
		public Game.Random m_random = new Game.Random();
	}
}
