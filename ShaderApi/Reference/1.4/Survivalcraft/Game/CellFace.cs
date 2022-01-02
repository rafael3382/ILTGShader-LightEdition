using System;
using Engine;

namespace Game
{
	// Token: 0x0200025A RID: 602
	public struct CellFace : IEquatable<CellFace>
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00093FB2 File Offset: 0x000921B2
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x00093FCB File Offset: 0x000921CB
		public Point3 Point
		{
			get
			{
				return new Point3(this.X, this.Y, this.Z);
			}
			set
			{
				this.X = value.X;
				this.Y = value.Y;
				this.Z = value.Z;
			}
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x00093FF1 File Offset: 0x000921F1
		public CellFace(int x, int y, int z, int face)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Face = face;
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x00094010 File Offset: 0x00092210
		public static int OppositeFace(int face)
		{
			return CellFace.m_oppositeFaces[face];
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00094019 File Offset: 0x00092219
		public static Point3 FaceToPoint3(int face)
		{
			return CellFace.m_faceToPoint3[face];
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00094026 File Offset: 0x00092226
		public static Vector3 FaceToVector3(int face)
		{
			return CellFace.m_faceToVector3[face];
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x00094034 File Offset: 0x00092234
		public static int Point3ToFace(Point3 p, int maxFace = 5)
		{
			for (int i = 0; i < maxFace; i++)
			{
				if (CellFace.m_faceToPoint3[i] == p)
				{
					return i;
				}
			}
			throw new InvalidOperationException("Invalid Point3.");
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0009406C File Offset: 0x0009226C
		public static int Vector3ToFace(Vector3 v, int maxFace = 5)
		{
			float num = float.NegativeInfinity;
			int result = 0;
			for (int i = 0; i <= maxFace; i++)
			{
				float num2 = Vector3.Dot(CellFace.m_faceToVector3[i], v);
				if (num2 > num)
				{
					result = i;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x000940A8 File Offset: 0x000922A8
		public static CellFace FromAxisAndDirection(int x, int y, int z, int axis, float direction)
		{
			CellFace result = default(CellFace);
			result.X = x;
			result.Y = y;
			result.Z = z;
			switch (axis)
			{
			case 0:
				result.Face = ((direction > 0f) ? 1 : 3);
				break;
			case 1:
				result.Face = ((direction > 0f) ? 4 : 5);
				break;
			case 2:
				result.Face = ((direction <= 0f) ? 2 : 0);
				break;
			}
			return result;
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x0009412C File Offset: 0x0009232C
		public Plane CalculatePlane()
		{
			switch (this.Face)
			{
			case 0:
				return new Plane(new Vector3(0f, 0f, 1f), (float)(-(float)(this.Z + 1)));
			case 1:
				return new Plane(new Vector3(-1f, 0f, 0f), (float)(this.X + 1));
			case 2:
				return new Plane(new Vector3(0f, 0f, -1f), (float)this.Z);
			case 3:
				return new Plane(new Vector3(1f, 0f, 0f), (float)(-(float)this.X));
			case 4:
				return new Plane(new Vector3(0f, 1f, 0f), (float)(-(float)(this.Y + 1)));
			default:
				return new Plane(new Vector3(0f, -1f, 0f), (float)this.Y);
			}
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x0009422D File Offset: 0x0009242D
		public override int GetHashCode()
		{
			return (this.X << 11) + (this.Y << 7) + (this.Z << 3) + this.Face;
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x00094251 File Offset: 0x00092451
		public override bool Equals(object obj)
		{
			return obj is CellFace && this.Equals((CellFace)obj);
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x00094269 File Offset: 0x00092469
		public bool Equals(CellFace other)
		{
			return other.X == this.X && other.Y == this.Y && other.Z == this.Z && other.Face == this.Face;
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x000942A8 File Offset: 0x000924A8
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.X.ToString(),
				", ",
				this.Y.ToString(),
				", ",
				this.Z.ToString(),
				", face ",
				this.Face.ToString()
			});
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x00094310 File Offset: 0x00092510
		public static bool operator ==(CellFace c1, CellFace c2)
		{
			return c1.Equals(c2);
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0009431A File Offset: 0x0009251A
		public static bool operator !=(CellFace c1, CellFace c2)
		{
			return !c1.Equals(c2);
		}

		// Token: 0x04000C55 RID: 3157
		public int X;

		// Token: 0x04000C56 RID: 3158
		public int Y;

		// Token: 0x04000C57 RID: 3159
		public int Z;

		// Token: 0x04000C58 RID: 3160
		public int Face;

		// Token: 0x04000C59 RID: 3161
		public static readonly int[] m_oppositeFaces = new int[]
		{
			2,
			3,
			0,
			1,
			5,
			4
		};

		// Token: 0x04000C5A RID: 3162
		public static readonly Point3[] m_faceToPoint3 = new Point3[]
		{
			new Point3(0, 0, 1),
			new Point3(1, 0, 0),
			new Point3(0, 0, -1),
			new Point3(-1, 0, 0),
			new Point3(0, 1, 0),
			new Point3(0, -1, 0)
		};

		// Token: 0x04000C5B RID: 3163
		public static readonly Vector3[] m_faceToVector3 = new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, -1f, 0f)
		};
	}
}
