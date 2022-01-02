using System;
using Engine;

namespace Game
{
	// Token: 0x0200018E RID: 398
	public class SubsystemBottomSuckerBlockBehavior : SubsystemInWaterBlockBehavior
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x00039A37 File Offset: 0x00037C37
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00039A40 File Offset: 0x00037C40
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			int face = BottomSuckerBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			Point3 point = CellFace.FaceToPoint3(CellFace.OppositeFace(face));
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
			if (!this.IsSupport(cellValue, face))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x00039ACC File Offset: 0x00037CCC
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)) == 226)
			{
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature == null)
				{
					return;
				}
				componentCreature.ComponentHealth.Injure(0.01f * MathUtils.Abs(velocity), null, false, "Spiked by a sea creature");
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00039B34 File Offset: 0x00037D34
		public bool IsSupport(int value, int face)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return block.IsCollidable_(value) && !block.IsFaceTransparent(base.SubsystemTerrain, CellFace.OppositeFace(face), value);
		}
	}
}
