﻿using System;
using Engine;

namespace Game
{
	// Token: 0x020002FC RID: 764
	public struct Segment2
	{
		// Token: 0x060016A7 RID: 5799 RVA: 0x000AA634 File Offset: 0x000A8834
		public Segment2(Vector2 start, Vector2 end)
		{
			this.Start = start;
			this.End = end;
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x000AA644 File Offset: 0x000A8844
		public override string ToString()
		{
			return string.Format("{0}, {1},  {2}, {3}", new object[]
			{
				this.Start.X,
				this.Start.Y,
				this.End.X,
				this.End.Y
			});
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x000AA6B0 File Offset: 0x000A88B0
		public static float Distance(Segment2 s, Vector2 p)
		{
			Vector2 v = s.End - s.Start;
			Vector2 v2 = s.Start - p;
			Vector2 v3 = s.End - p;
			float num = Vector2.Dot(v2, v);
			if (num * Vector2.Dot(v3, v) <= 0f)
			{
				float num2 = v.LengthSquared();
				if (num2 == 0f)
				{
					return Vector2.Distance(p, s.Start);
				}
				return MathUtils.Abs(Vector2.Cross(p - s.Start, v)) / MathUtils.Sqrt(num2);
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

		// Token: 0x060016AA RID: 5802 RVA: 0x000AA758 File Offset: 0x000A8958
		public static bool Intersection(Segment2 s1, Segment2 s2, out Vector2 result)
		{
			Vector2 vector = s1.End - s1.Start;
			Vector2 vector2 = s2.End - s2.Start;
			float num = Vector2.Cross(vector, vector2);
			if (num == 0f)
			{
				result = Vector2.Zero;
				return false;
			}
			float num2 = 1f / num;
			float num3 = (vector2.X * (s1.Start.Y - s2.Start.Y) - vector2.Y * (s1.Start.X - s2.Start.X)) * num2;
			float num4 = (vector.X * (s1.Start.Y - s2.Start.Y) - vector.Y * (s1.Start.X - s2.Start.X)) * num2;
			if (num3 < 0f || num3 > 1f || num4 < 0f || num4 > 1f)
			{
				result = Vector2.Zero;
				return false;
			}
			result = new Vector2(s1.Start.X + num3 * vector.X, s1.Start.Y + num3 * vector.Y);
			return true;
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x000AA898 File Offset: 0x000A8A98
		public static Vector2 NearestPoint(Segment2 s, Vector2 p)
		{
			Vector2 v = s.End - s.Start;
			Vector2 v2 = s.Start - p;
			Vector2 v3 = s.End - p;
			float num = Vector2.Dot(v2, v);
			if (num * Vector2.Dot(v3, v) <= 0f)
			{
				float num2 = v.Length();
				if (num2 == 0f)
				{
					return s.Start;
				}
				float num3 = MathUtils.Sqrt(v2.LengthSquared() - MathUtils.Sqr(MathUtils.Abs(Vector2.Cross(p - s.Start, v)) / num2));
				return Vector2.Lerp(s.Start, s.End, num3 / num2);
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

		// Token: 0x04000F6A RID: 3946
		public Vector2 Start;

		// Token: 0x04000F6B RID: 3947
		public Vector2 End;
	}
}
