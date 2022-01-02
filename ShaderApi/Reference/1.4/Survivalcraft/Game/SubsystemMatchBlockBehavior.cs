using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B8 RID: 440
	public class SubsystemMatchBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0004BC9C File Offset: 0x00049E9C
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					108
				};
			}
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0004BCAC File Offset: 0x00049EAC
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			object obj = componentMiner.Raycast(ray, RaycastMode.Digging, true, true, true);
			if (obj is TerrainRaycastResult)
			{
				CellFace cellFace = ((TerrainRaycastResult)obj).CellFace;
				if (this.m_subsystemExplosivesBlockBehavior.IgniteFuse(cellFace.X, cellFace.Y, cellFace.Z))
				{
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
				if (this.m_subsystemFireBlockBehavior.SetCellOnFire(cellFace.X, cellFace.Y, cellFace.Z, 1f))
				{
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
			}
			else if (obj is BodyRaycastResult)
			{
				ComponentOnFire componentOnFire = ((BodyRaycastResult)obj).ComponentBody.Entity.FindComponent<ComponentOnFire>();
				if (componentOnFire != null)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode < GameMode.Challenging || this.m_random.Float(0f, 1f) < 0.33f)
					{
						componentOnFire.SetOnFire(componentMiner.ComponentCreature, this.m_random.Float(6f, 8f));
					}
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0004BE58 File Offset: 0x0004A058
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemExplosivesBlockBehavior = base.Project.FindSubsystem<SubsystemExplosivesBlockBehavior>(true);
		}

		// Token: 0x0400057A RID: 1402
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400057B RID: 1403
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400057C RID: 1404
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x0400057D RID: 1405
		public SubsystemExplosivesBlockBehavior m_subsystemExplosivesBlockBehavior;

		// Token: 0x0400057E RID: 1406
		public Game.Random m_random = new Game.Random();
	}
}
