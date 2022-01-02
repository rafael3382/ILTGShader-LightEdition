using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DD RID: 477
	public class SubsystemTrapdoorBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x0005D9C4 File Offset: 0x0005BBC4
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					83,
					84
				};
			}
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0005D9D8 File Offset: 0x0005BBD8
		public bool IsTrapdoorElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is TrapdoorBlock)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, TrapdoorBlock.GetMountingFace(data));
				if (electricElement != null && electricElement.Connections.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0005DA3C File Offset: 0x0005BC3C
		public bool OpenCloseTrapdoor(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is TrapdoorBlock)
			{
				int data = TrapdoorBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemTrapdoorBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0005DAF0 File Offset: 0x0005BCF0
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 83 || !this.IsTrapdoorElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = TrapdoorBlock.GetOpen(data);
				return this.OpenCloseTrapdoor(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0005DB78 File Offset: 0x0005BD78
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int data = Terrain.ExtractData(cellValue);
			if (block is TrapdoorBlock)
			{
				int rotation = TrapdoorBlock.GetRotation(data);
				bool upsideDown = TrapdoorBlock.GetUpsideDown(data);
				bool flag = false;
				Point3 point = CellFace.FaceToPoint3(rotation);
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
				flag |= !BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsTransparent_(cellValue2);
				if (upsideDown)
				{
					int cellValue3 = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
					flag |= !BlocksManager.Blocks[Terrain.ExtractContents(cellValue3)].IsTransparent_(cellValue3);
					int cellValue4 = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y + 1, z - point.Z);
					flag |= !BlocksManager.Blocks[Terrain.ExtractContents(cellValue4)].IsTransparent_(cellValue4);
				}
				else
				{
					int cellValue5 = base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
					flag |= !BlocksManager.Blocks[Terrain.ExtractContents(cellValue5)].IsTransparent_(cellValue5);
					int cellValue6 = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y - 1, z - point.Z);
					flag |= !BlocksManager.Blocks[Terrain.ExtractContents(cellValue6)].IsTransparent_(cellValue6);
				}
				if (!flag)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0005DD32 File Offset: 0x0005BF32
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x040006B2 RID: 1714
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x040006B3 RID: 1715
		public static Game.Random m_random = new Game.Random();
	}
}
