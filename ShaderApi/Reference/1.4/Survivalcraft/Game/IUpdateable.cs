using System;

namespace Game
{
	// Token: 0x020002B6 RID: 694
	public interface IUpdateable
	{
		// Token: 0x17000337 RID: 823
		// (get) Token: 0x0600156E RID: 5486
		UpdateOrder UpdateOrder { get; }

		// Token: 0x0600156F RID: 5487
		void Update(float dt);
	}
}
