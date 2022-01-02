using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B0 RID: 432
	public class SubsystemHammerBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000B1F RID: 2847 RVA: 0x0004AE89 File Offset: 0x00049089
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0004AE94 File Offset: 0x00049094
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging, true, true, true);
			if (terrainRaycastResult != null)
			{
				this.m_subsystemFurnitureBlockBehavior.ScanDesign(terrainRaycastResult.Value.CellFace, ray.Direction, componentMiner);
				return true;
			}
			return false;
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0004AED7 File Offset: 0x000490D7
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
		}

		// Token: 0x0400056A RID: 1386
		public SubsystemFurnitureBlockBehavior m_subsystemFurnitureBlockBehavior;
	}
}
