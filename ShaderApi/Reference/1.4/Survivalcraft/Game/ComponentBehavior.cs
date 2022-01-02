using System;
using GameEntitySystem;

namespace Game
{
	// Token: 0x020001EA RID: 490
	public abstract class ComponentBehavior : Component
	{
		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000D7C RID: 3452
		public abstract float ImportanceLevel { get; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0006206E File Offset: 0x0006026E
		// (set) Token: 0x06000D7E RID: 3454 RVA: 0x00062076 File Offset: 0x00060276
		public virtual bool IsActive { get; set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000D7F RID: 3455 RVA: 0x0006207F File Offset: 0x0006027F
		public virtual string DebugInfo
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
