using System;
using Engine;

namespace Game
{
	// Token: 0x020002AB RID: 683
	public interface IAStarWorld<T>
	{
		// Token: 0x0600152B RID: 5419
		float Cost(T p1, T p2);

		// Token: 0x0600152C RID: 5420
		void Neighbors(T p, DynamicArray<T> neighbors);

		// Token: 0x0600152D RID: 5421
		float Heuristic(T p1, T p2);

		// Token: 0x0600152E RID: 5422
		bool IsGoal(T p);
	}
}
