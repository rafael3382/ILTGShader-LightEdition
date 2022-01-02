using System;
using System.Collections.Generic;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EB RID: 491
	public class ComponentBehaviorSelector : Component, IUpdateable
	{
		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000D81 RID: 3457 RVA: 0x0006208E File Offset: 0x0006028E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x00062094 File Offset: 0x00060294
		public void Update(float dt)
		{
			ComponentBehavior componentBehavior = null;
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = 0f;
				foreach (ComponentBehavior componentBehavior2 in this.m_behaviors)
				{
					float importanceLevel = componentBehavior2.ImportanceLevel;
					if (importanceLevel > num)
					{
						num = importanceLevel;
						componentBehavior = componentBehavior2;
					}
				}
			}
			foreach (ComponentBehavior componentBehavior3 in this.m_behaviors)
			{
				if (componentBehavior3 == componentBehavior)
				{
					if (!componentBehavior3.IsActive)
					{
						componentBehavior3.IsActive = true;
					}
				}
				else if (componentBehavior3.IsActive)
				{
					componentBehavior3.IsActive = false;
				}
			}
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x00062178 File Offset: 0x00060378
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			foreach (ComponentBehavior item in base.Entity.FindComponents<ComponentBehavior>())
			{
				this.m_behaviors.Add(item);
			}
		}

		// Token: 0x0400070A RID: 1802
		public ComponentCreature m_componentCreature;

		// Token: 0x0400070B RID: 1803
		public List<ComponentBehavior> m_behaviors = new List<ComponentBehavior>();

		// Token: 0x0400070C RID: 1804
		public static bool ShowAIBehavior;
	}
}
