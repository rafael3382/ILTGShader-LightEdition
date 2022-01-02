using System;
using Engine;

namespace Game
{
	// Token: 0x0200032E RID: 814
	public struct TouchInput
	{
		// Token: 0x040010F6 RID: 4342
		public TouchInputType InputType;

		// Token: 0x040010F7 RID: 4343
		public Vector2 Position;

		// Token: 0x040010F8 RID: 4344
		public Vector2 Move;

		// Token: 0x040010F9 RID: 4345
		public Vector2 TotalMove;

		// Token: 0x040010FA RID: 4346
		public Vector2 TotalMoveLimited;

		// Token: 0x040010FB RID: 4347
		public float Duration;

		// Token: 0x040010FC RID: 4348
		public int DurationFrames;
	}
}
