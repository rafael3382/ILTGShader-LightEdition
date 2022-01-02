using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019B RID: 411
	public class SubsystemDoorBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x00041089 File Offset: 0x0003F289
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					56,
					57,
					58
				};
			}
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0004109C File Offset: 0x0003F29C
		public bool OpenCloseDoor(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is DoorBlock)
			{
				int data = DoorBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemDoorBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00041150 File Offset: 0x0003F350
		public bool IsDoorElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is DoorBlock)
			{
				int num2 = DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z) ? y : (y - 1);
				for (int i = num2; i <= num2 + 1; i++)
				{
					ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, i, z, DoorBlock.GetHingeFace(data));
					if (electricElement != null && electricElement.Connections.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x000411E4 File Offset: 0x0003F3E4
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 56 || !this.IsDoorElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = DoorBlock.GetOpen(data);
				return this.OpenCloseDoor(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0004126C File Offset: 0x0003F46C
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
			int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			if (!BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsTransparent_(cellValue) && Terrain.ExtractContents(cellValue2) == 0)
			{
				base.SubsystemTerrain.ChangeCell(x, y + 1, z, value, true);
			}
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x000412DC File Offset: 0x0003F4DC
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.ChangeCell(x, y - 1, z, 0, true);
			}
			if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.ChangeCell(x, y + 1, z, 0, true);
			}
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00041340 File Offset: 0x0003F540
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int data = Terrain.ExtractData(cellValue);
			if (!(block is DoorBlock))
			{
				return;
			}
			if (neighborX == x && neighborY == y && neighborZ == z)
			{
				if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
				{
					int value = Terrain.ReplaceData(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z), data);
					base.SubsystemTerrain.ChangeCell(x, y + 1, z, value, true);
				}
				if (DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
				{
					int value2 = Terrain.ReplaceData(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z), data);
					base.SubsystemTerrain.ChangeCell(x, y - 1, z, value2, true);
				}
			}
			if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
				if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsTransparent_(cellValue2))
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
			if (!DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z) && !DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x000414AE File Offset: 0x0003F6AE
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x040004E2 RID: 1250
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x040004E3 RID: 1251
		public static Game.Random m_random = new Game.Random();
	}
}
