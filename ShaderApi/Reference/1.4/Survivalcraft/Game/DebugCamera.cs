using System;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x02000268 RID: 616
	public class DebugCamera : BasePerspectiveCamera
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x00095597 File Offset: 0x00093797
		public override bool UsesMovementControls
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0009559A File Offset: 0x0009379A
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x0009559D File Offset: 0x0009379D
		public DebugCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x000955B1 File Offset: 0x000937B1
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			this.m_direction = previousCamera.ViewDirection;
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x000955E4 File Offset: 0x000937E4
		public override void Update(float dt)
		{
			dt = MathUtils.Min(dt, 0.1f);
			Vector3 zero = Vector3.Zero;
			if (Keyboard.IsKeyDown(Key.A))
			{
				zero.X = -1f;
			}
			if (Keyboard.IsKeyDown(Key.D))
			{
				zero.X = 1f;
			}
			if (Keyboard.IsKeyDown(Key.W))
			{
				zero.Z = 1f;
			}
			if (Keyboard.IsKeyDown(Key.S))
			{
				zero.Z = -1f;
			}
			Vector2 vector = 0.03f * new Vector2((float)Mouse.MouseMovement.X, (float)(-(float)Mouse.MouseMovement.Y));
			bool flag = Keyboard.IsKeyDown(Key.Shift);
			bool flag2 = Keyboard.IsKeyDown(Key.Control);
			Vector3 direction = this.m_direction;
			Vector3 unitY = Vector3.UnitY;
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(direction, unitY));
			float num = 8f;
			if (flag)
			{
				num *= 10f;
			}
			if (flag2)
			{
				num /= 10f;
			}
			Vector3 vector3 = Vector3.Zero;
			vector3 += num * zero.X * vector2;
			vector3 += num * zero.Y * unitY;
			vector3 += num * zero.Z * direction;
			this.m_position += vector3 * dt;
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(unitY, -4f * vector.X * dt));
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(vector2, 4f * vector.Y * dt));
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
			Vector2 v = this.ViewportSize / 2f;
			FlatBatch2D flatBatch2D = this.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
			int count = flatBatch2D.LineVertices.Count;
			flatBatch2D.QueueLine(v - new Vector2(5f, 0f), v + new Vector2(5f, 0f), 0f, Color.White);
			flatBatch2D.QueueLine(v - new Vector2(0f, 5f), v + new Vector2(0f, 5f), 0f, Color.White);
			flatBatch2D.TransformLines(this.ViewportMatrix, count, -1);
			this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
		}

		// Token: 0x04000CA3 RID: 3235
		public static string AmbientParameters = string.Empty;

		// Token: 0x04000CA4 RID: 3236
		public static string PlantParameters = string.Empty;

		// Token: 0x04000CA5 RID: 3237
		public Vector3 m_position;

		// Token: 0x04000CA6 RID: 3238
		public Vector3 m_direction;

		// Token: 0x04000CA7 RID: 3239
		public PrimitivesRenderer2D PrimitivesRenderer2D = new PrimitivesRenderer2D();
	}
}
