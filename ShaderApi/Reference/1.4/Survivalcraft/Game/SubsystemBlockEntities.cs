using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000188 RID: 392
	public class SubsystemBlockEntities : Subsystem
	{
		// Token: 0x06000909 RID: 2313 RVA: 0x00038AD4 File Offset: 0x00036CD4
		public ComponentBlockEntity GetBlockEntity(int x, int y, int z)
		{
			ComponentBlockEntity result;
			this.m_blockEntities.TryGetValue(new Point3(x, y, z), out result);
			return result;
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00038AF8 File Offset: 0x00036CF8
		public override void OnEntityAdded(Entity entity)
		{
			ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
			if (componentBlockEntity != null)
			{
				this.m_blockEntities.Add(componentBlockEntity.Coordinates, componentBlockEntity);
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00038B24 File Offset: 0x00036D24
		public override void OnEntityRemoved(Entity entity)
		{
			ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
			if (componentBlockEntity != null)
			{
				this.m_blockEntities.Remove(componentBlockEntity.Coordinates);
			}
		}

		// Token: 0x04000485 RID: 1157
		public Dictionary<Point3, ComponentBlockEntity> m_blockEntities = new Dictionary<Point3, ComponentBlockEntity>();
	}
}
