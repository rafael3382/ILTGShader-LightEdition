using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000196 RID: 406
	public class SubsystemCollapsingBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x0003B700 File Offset: 0x00039900
		public override int[] HandledBlocks
		{
			get
			{
				return SubsystemCollapsingBlockBehavior.m_handledBlocks;
			}
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x0003B707 File Offset: 0x00039907
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living)
			{
				this.TryCollapseColumn(new Point3(x, y, z));
			}
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0003B72C File Offset: 0x0003992C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemMovingBlocks.Stopped += this.MovingBlocksStopped;
			this.m_subsystemMovingBlocks.CollidedWithTerrain += this.MovingBlocksCollidedWithTerrain;
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0003B7A4 File Offset: 0x000399A4
		public void MovingBlocksCollidedWithTerrain(IMovingBlockSet movingBlockSet, Point3 p)
		{
			if (movingBlockSet.Id == "CollapsingBlock")
			{
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(p.X, p.Y, p.Z);
				if (this.IsCollapseSupportBlock(cellValue))
				{
					movingBlockSet.Stop();
					return;
				}
				if (SubsystemCollapsingBlockBehavior.IsCollapseDestructibleBlock(cellValue))
				{
					base.SubsystemTerrain.DestroyCell(0, p.X, p.Y, p.Z, 0, false, false);
				}
			}
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0003B820 File Offset: 0x00039A20
		public void MovingBlocksStopped(IMovingBlockSet movingBlockSet)
		{
			if (movingBlockSet.Id == "CollapsingBlock")
			{
				Point3 p = Terrain.ToCell(MathUtils.Round(movingBlockSet.Position.X), MathUtils.Round(movingBlockSet.Position.Y), MathUtils.Round(movingBlockSet.Position.Z));
				foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
				{
					Point3 point = p + movingBlock.Offset;
					base.SubsystemTerrain.DestroyCell(0, point.X, point.Y, point.Z, movingBlock.Value, false, false);
				}
				this.m_subsystemMovingBlocks.RemoveMovingBlockSet(movingBlockSet);
				if (movingBlockSet.Blocks.Count > 0)
				{
					this.m_subsystemSoundMaterials.PlayImpactSound(movingBlockSet.Blocks[0].Value, movingBlockSet.Position, 1f);
				}
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0003B938 File Offset: 0x00039B38
		public void TryCollapseColumn(Point3 p)
		{
			if (p.Y <= 0)
			{
				return;
			}
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(p.X, p.Y - 1, p.Z);
			if (this.IsCollapseSupportBlock(cellValue))
			{
				return;
			}
			List<MovingBlock> list = new List<MovingBlock>();
			for (int i = p.Y; i < 256; i++)
			{
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(p.X, i, p.Z);
				if (!SubsystemCollapsingBlockBehavior.IsCollapsibleBlock(cellValue2))
				{
					break;
				}
				list.Add(new MovingBlock
				{
					Value = cellValue2,
					Offset = new Point3(0, i - p.Y, 0)
				});
			}
			if (list.Count != 0 && this.m_subsystemMovingBlocks.AddMovingBlockSet(new Vector3(p), new Vector3((float)p.X, (float)(-(float)list.Count - 1), (float)p.Z), 0f, 10f, 0.7f, new Vector2(0f), list, "CollapsingBlock", null, true) != null)
			{
				foreach (MovingBlock movingBlock in list)
				{
					Point3 point = p + movingBlock.Offset;
					base.SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, 0, true);
				}
			}
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0003BAB8 File Offset: 0x00039CB8
		public static bool IsCollapsibleBlock(int value)
		{
			return SubsystemCollapsingBlockBehavior.m_handledBlocks.Contains(Terrain.ExtractContents(value));
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0003BACC File Offset: 0x00039CCC
		public bool IsCollapseSupportBlock(int value)
		{
			int num = Terrain.ExtractContents(value);
			if (num == 0)
			{
				return false;
			}
			int data = Terrain.ExtractData(value);
			Block block = BlocksManager.Blocks[num];
			if (block is TrapdoorBlock)
			{
				return TrapdoorBlock.GetUpsideDown(data) && !TrapdoorBlock.GetOpen(data);
			}
			return block.BlockIndex == 238 || !block.IsFaceTransparent(base.SubsystemTerrain, 4, value) || block is SoilBlock;
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0003BB3C File Offset: 0x00039D3C
		public static bool IsCollapseDestructibleBlock(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block is TrapdoorBlock)
			{
				int data = Terrain.ExtractData(value);
				if (TrapdoorBlock.GetUpsideDown(data) && TrapdoorBlock.GetOpen(data))
				{
					return false;
				}
			}
			else if (block is FluidBlock)
			{
				return false;
			}
			return true;
		}

		// Token: 0x040004BE RID: 1214
		public const string IdString = "CollapsingBlock";

		// Token: 0x040004BF RID: 1215
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004C0 RID: 1216
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x040004C1 RID: 1217
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x040004C2 RID: 1218
		public static int[] m_handledBlocks = new int[]
		{
			7,
			6
		};
	}
}
