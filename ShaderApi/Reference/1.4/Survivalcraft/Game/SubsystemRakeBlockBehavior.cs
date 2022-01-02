using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CB RID: 459
	public class SubsystemRakeBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00055B6F File Offset: 0x00053D6F
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					169,
					219,
					171,
					172
				};
			}
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00055B84 File Offset: 0x00053D84
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (terrainRaycastResult != null)
			{
				if (terrainRaycastResult.Value.CellFace.Face == 4)
				{
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z);
					int num = Terrain.ExtractContents(cellValue);
					Block block = BlocksManager.Blocks[num];
					if (num != 2)
					{
						if (num == 8)
						{
							int value = Terrain.ReplaceContents(cellValue, 2);
							this.m_subsystemTerrain.ChangeCell(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z, value, true);
							this.m_subsystemAudio.PlayRandomSound("Audio/Impacts/Plant", 0.5f, 0f, new Vector3((float)terrainRaycastResult.Value.CellFace.X, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z), 3f, true);
							Vector3 position = new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y + 1.2f, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f);
							this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this.m_subsystemTerrain, position, cellValue, 0.75f));
						}
					}
					else
					{
						int value2 = Terrain.ReplaceContents(cellValue, 168);
						this.m_subsystemTerrain.ChangeCell(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z, value2, true);
						this.m_subsystemAudio.PlayRandomSound("Audio/Impacts/Dirt", 0.5f, 0f, new Vector3((float)terrainRaycastResult.Value.CellFace.X, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z), 3f, true);
						Vector3 position2 = new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y + 1.25f, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f);
						this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this.m_subsystemTerrain, position2, cellValue, 0.5f));
					}
				}
				componentMiner.DamageActiveTool(1);
				return true;
			}
			return false;
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00055E5C File Offset: 0x0005405C
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x0400060B RID: 1547
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400060C RID: 1548
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400060D RID: 1549
		public SubsystemAudio m_subsystemAudio;
	}
}
