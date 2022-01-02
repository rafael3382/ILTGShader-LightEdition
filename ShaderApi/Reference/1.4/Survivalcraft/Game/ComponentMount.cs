using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200021F RID: 543
	public class ComponentMount : Component
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060010FD RID: 4349 RVA: 0x0007EFD4 File Offset: 0x0007D1D4
		// (set) Token: 0x060010FE RID: 4350 RVA: 0x0007EFDC File Offset: 0x0007D1DC
		public ComponentBody ComponentBody { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060010FF RID: 4351 RVA: 0x0007EFE5 File Offset: 0x0007D1E5
		// (set) Token: 0x06001100 RID: 4352 RVA: 0x0007EFED File Offset: 0x0007D1ED
		public Vector3 MountOffset { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06001101 RID: 4353 RVA: 0x0007EFF6 File Offset: 0x0007D1F6
		// (set) Token: 0x06001102 RID: 4354 RVA: 0x0007EFFE File Offset: 0x0007D1FE
		public Vector3 DismountOffset { get; set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06001103 RID: 4355 RVA: 0x0007F008 File Offset: 0x0007D208
		public ComponentRider Rider
		{
			get
			{
				foreach (ComponentBody componentBody in this.ComponentBody.ChildBodies)
				{
					ComponentRider componentRider = componentBody.Entity.FindComponent<ComponentRider>();
					if (componentRider != null)
					{
						return componentRider;
					}
				}
				return null;
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0007F070 File Offset: 0x0007D270
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.ComponentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.MountOffset = valuesDictionary.GetValue<Vector3>("MountOffset");
			this.DismountOffset = valuesDictionary.GetValue<Vector3>("DismountOffset");
		}
	}
}
