using System;
using Engine;

namespace Game
{
	// Token: 0x020002FD RID: 765
	public struct Segment3
	{
		// Token: 0x060016AC RID: 5804 RVA: 0x000AA95C File Offset: 0x000A8B5C
		public Segment3(Vector3 start, Vector3 end)
		{
			this.Start = start;
			this.End = end;
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x000AA96C File Offset: 0x000A8B6C
		public override string ToString()
		{
			return string.Format("{0}, {1}, {2},  {3}, {4}, {5}", new object[]
			{
				this.Start.X,
				this.Start.Y,
				this.Start.Z,
				this.End.X,
				this.End.Y,
				this.End.Z
			});
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x000AA9FC File Offset: 0x000A8BFC
		public static float Distance(Segment3 s, Vector3 p)
		{
			Vector3 v = s.End - s.Start;
			Vector3 v2 = s.Start - p;
			Vector3 v3 = s.End - p;
			float num = Vector3.Dot(v2, v);
			if (num * Vector3.Dot(v3, v) <= 0f)
			{
				float num2 = v.LengthSquared();
				if (num2 == 0f)
				{
					return Vector3.Distance(p, s.Start);
				}
				return MathUtils.Sqrt(Vector3.Cross(p - s.Start, v).LengthSquared() / num2);
			}
			else
			{
				if (num <= 0f)
				{
					return v3.Length();
				}
				return v2.Length();
			}
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x000AAAA8 File Offset: 0x000A8CA8
		public static Vector3 NearestPoint(Segment3 s, Vector3 p)
		{
			Vector3 v = s.End - s.Start;
			Vector3 v2 = s.Start - p;
			Vector3 v3 = s.End - p;
			float num = Vector3.Dot(v2, v);
			if (num * Vector3.Dot(v3, v) <= 0f)
			{
				float num2 = v.LengthSquared();
				if (num2 == 0f)
				{
					return s.Start;
				}
				float num3 = MathUtils.Sqrt(v2.LengthSquared() - Vector3.Cross(p - s.Start, v).LengthSquared() / num2);
				return Vector3.Lerp(s.Start, s.End, num3 / MathUtils.Sqrt(num2));
			}
			else
			{
				if (num <= 0f)
				{
					return s.End;
				}
				return s.Start;
			}
		}

		// Token: 0x04000F6C RID: 3948
		public Vector3 Start;

		// Token: 0x04000F6D RID: 3949
		public Vector3 End;
	}
}
