using System;
using Engine;

namespace Game
{
	// Token: 0x02000081 RID: 129
	public interface IMovingBlockSet
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060002DC RID: 732
		Vector3 Position { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060002DD RID: 733
		string Id { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060002DE RID: 734
		object Tag { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060002DF RID: 735
		Vector3 CurrentVelocity { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060002E0 RID: 736
		ReadOnlyList<MovingBlock> Blocks { get; }

		// Token: 0x060002E1 RID: 737
		BoundingBox BoundingBox(bool extendToFillCells);

		// Token: 0x060002E2 RID: 738
		void SetBlock(Point3 offset, int value);

		// Token: 0x060002E3 RID: 739
		void Stop();
	}
}
