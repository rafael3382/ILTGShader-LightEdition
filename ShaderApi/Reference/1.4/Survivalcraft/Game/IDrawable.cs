using System;

namespace Game
{
	// Token: 0x020002AC RID: 684
	public interface IDrawable
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x0600152F RID: 5423
		int[] DrawOrders { get; }

		// Token: 0x06001530 RID: 5424
		void Draw(Camera camera, int drawOrder);
	}
}
