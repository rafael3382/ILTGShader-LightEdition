using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CD RID: 461
	public class SubsystemSaddleBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000C2E RID: 3118 RVA: 0x0005613C File Offset: 0x0005433C
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					158
				};
			}
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x0005614C File Offset: 0x0005434C
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			BodyRaycastResult? bodyRaycastResult = componentMiner.Raycast<BodyRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (bodyRaycastResult != null)
			{
				ComponentHealth componentHealth = bodyRaycastResult.Value.ComponentBody.Entity.FindComponent<ComponentHealth>();
				if (componentHealth == null || componentHealth.Health > 0f)
				{
					string entityTemplateName = bodyRaycastResult.Value.ComponentBody.Entity.ValuesDictionary.DatabaseObject.Name + "_Saddled";
					Entity entity = DatabaseManager.CreateEntity(base.Project, entityTemplateName, false);
					if (entity != null)
					{
						ComponentBody componentBody = entity.FindComponent<ComponentBody>(true);
						componentBody.Position = bodyRaycastResult.Value.ComponentBody.Position;
						componentBody.Rotation = bodyRaycastResult.Value.ComponentBody.Rotation;
						componentBody.Velocity = bodyRaycastResult.Value.ComponentBody.Velocity;
						entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
						base.Project.RemoveEntity(bodyRaycastResult.Value.ComponentBody.Entity, true);
						base.Project.AddEntity(entity);
						this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
						componentMiner.RemoveActiveTool(1);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0005629F File Offset: 0x0005449F
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x04000615 RID: 1557
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000616 RID: 1558
		public Game.Random m_random = new Game.Random();
	}
}
