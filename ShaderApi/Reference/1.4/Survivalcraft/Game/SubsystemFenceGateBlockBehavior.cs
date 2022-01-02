using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A4 RID: 420
	public class SubsystemFenceGateBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000A8D RID: 2701 RVA: 0x00045A2D File Offset: 0x00043C2D
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00045A38 File Offset: 0x00043C38
		public bool OpenCloseGate(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is FenceGateBlock)
			{
				int data = FenceGateBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemFenceGateBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00045AEC File Offset: 0x00043CEC
		public bool IsGateElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is FenceGateBlock)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, FenceGateBlock.GetHingeFace(data));
				if (electricElement != null && electricElement.Connections.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x00045B50 File Offset: 0x00043D50
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 166 || !this.IsGateElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = FenceGateBlock.GetOpen(data);
				return this.OpenCloseGate(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00045BDA File Offset: 0x00043DDA
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x04000521 RID: 1313
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x04000522 RID: 1314
		public static Game.Random m_random = new Game.Random();
	}
}
