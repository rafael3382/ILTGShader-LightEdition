using System;
using System.Collections.Generic;
using GameEntitySystem;

namespace Game
{
	// Token: 0x020001BE RID: 446
	public class SubsystemNames : Subsystem
	{
		// Token: 0x06000B98 RID: 2968 RVA: 0x0004FBD2 File Offset: 0x0004DDD2
		public Component FindComponentByName(string name, Type componentType, string componentName)
		{
			Entity entity = this.FindEntityByName(name);
			if (entity == null)
			{
				return null;
			}
			return entity.FindComponent(componentType, componentName, false);
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0004FBEC File Offset: 0x0004DDEC
		public T FindComponentByName<T>(string name, string componentName) where T : Component
		{
			Entity entity = this.FindEntityByName(name);
			if (entity == null)
			{
				return default(T);
			}
			return entity.FindComponent<T>(componentName, false);
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0004FC18 File Offset: 0x0004DE18
		public Entity FindEntityByName(string name)
		{
			ComponentName componentName;
			this.m_componentsByName.TryGetValue(name, out componentName);
			if (componentName == null)
			{
				return null;
			}
			return componentName.Entity;
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0004FC40 File Offset: 0x0004DE40
		public static string GetEntityName(Entity entity)
		{
			ComponentName componentName = entity.FindComponent<ComponentName>();
			if (componentName != null)
			{
				return componentName.Name;
			}
			return string.Empty;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0004FC64 File Offset: 0x0004DE64
		public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentName componentName in entity.FindComponents<ComponentName>())
			{
				this.m_componentsByName.Add(componentName.Name, componentName);
			}
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0004FCC8 File Offset: 0x0004DEC8
		public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentName componentName in entity.FindComponents<ComponentName>())
			{
				this.m_componentsByName.Remove(componentName.Name);
			}
		}

		// Token: 0x040005B6 RID: 1462
		public Dictionary<string, ComponentName> m_componentsByName = new Dictionary<string, ComponentName>();
	}
}
