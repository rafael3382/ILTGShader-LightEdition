using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A5 RID: 421
	public class SubsystemFertilizerBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x00045C09 File Offset: 0x00043E09
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					102
				};
			}
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00045C18 File Offset: 0x00043E18
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (terrainRaycastResult != null && terrainRaycastResult.Value.CellFace.Face == 4)
			{
				int y = terrainRaycastResult.Value.CellFace.Y;
				for (int i = terrainRaycastResult.Value.CellFace.X - 1; i <= terrainRaycastResult.Value.CellFace.X + 1; i++)
				{
					for (int j = terrainRaycastResult.Value.CellFace.Z - 1; j <= terrainRaycastResult.Value.CellFace.Z + 1; j++)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(i, y, j);
						if (Terrain.ExtractContents(cellValue) == 168)
						{
							int data = SoilBlock.SetNitrogen(Terrain.ExtractData(cellValue), 3);
							int value = Terrain.ReplaceData(cellValue, data);
							this.m_subsystemTerrain.ChangeCell(i, y, j, value, true);
						}
					}
				}
				this.m_subsystemAudio.PlayRandomSound("Audio/Impacts/Dirt", 0.5f, 0f, new Vector3((float)terrainRaycastResult.Value.CellFace.X, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z), 3f, true);
				Vector3 position = new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y + 1.5f, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(componentMiner.ActiveBlockValue)];
				this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this.m_subsystemTerrain, position, componentMiner.ActiveBlockValue, 1.25f));
				componentMiner.RemoveActiveTool(1);
				return true;
			}
			return false;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00045E0F File Offset: 0x0004400F
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x04000523 RID: 1315
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000524 RID: 1316
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000525 RID: 1317
		public SubsystemAudio m_subsystemAudio;
	}
}
