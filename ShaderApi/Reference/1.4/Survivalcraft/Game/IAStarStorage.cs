using System;

namespace Game
{
	// Token: 0x020002AA RID: 682
	public interface IAStarStorage<T>
	{
		// Token: 0x06001528 RID: 5416
		void Clear();

		// Token: 0x06001529 RID: 5417
		object Get(T p);

		// Token: 0x0600152A RID: 5418
		void Set(T p, object data);
	}
}
