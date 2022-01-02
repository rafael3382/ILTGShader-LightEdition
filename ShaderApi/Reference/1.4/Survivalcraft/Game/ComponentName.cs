using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000221 RID: 545
	public class ComponentName : Component
	{
		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x0007F425 File Offset: 0x0007D625
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0007F42D File Offset: 0x0007D62D
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_name = valuesDictionary.GetValue<string>("Name");
		}

		// Token: 0x04000A2E RID: 2606
		public string m_name;
	}
}
