using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x02000293 RID: 659
	public class FlyCamera : BasePerspectiveCamera
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x0009AD3E File Offset: 0x00098F3E
		public override bool UsesMovementControls
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x0009AD41 File Offset: 0x00098F41
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x0009AD44 File Offset: 0x00098F44
		public FlyCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x0009AD4D File Offset: 0x00098F4D
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			this.m_direction = previousCamera.ViewDirection;
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x0009AD80 File Offset: 0x00098F80
		public override void Update(float dt)
		{
			Vector3 vector = Vector3.Zero;
			Vector2 vector2 = Vector2.Zero;
			ComponentPlayer componentPlayer = base.GameWidget.PlayerData.ComponentPlayer;
			ComponentInput componentInput = (componentPlayer != null) ? componentPlayer.ComponentInput : null;
			if (componentInput != null)
			{
				vector = componentInput.PlayerInput.CameraMove * new Vector3(1f, 0f, 1f);
				vector2 = componentInput.PlayerInput.CameraLook;
			}
			bool flag = Keyboard.IsKeyDown(Key.Shift);
			bool flag2 = Keyboard.IsKeyDown(Key.Control);
			Vector3 direction = this.m_direction;
			Vector3 unitY = Vector3.UnitY;
			Vector3 vector3 = Vector3.Normalize(Vector3.Cross(direction, unitY));
			float num = 10f;
			if (flag)
			{
				num *= 5f;
			}
			if (flag2)
			{
				num /= 5f;
			}
			Vector3 v = Vector3.Zero;
			v += num * vector.X * vector3;
			v += num * vector.Y * unitY;
			v += num * vector.Z * direction;
			this.m_rollSpeed = MathUtils.Lerp(this.m_rollSpeed, -1.5f * vector2.X, 3f * dt);
			this.m_rollAngle += this.m_rollSpeed * dt;
			this.m_rollAngle *= MathUtils.Pow(0.33f, dt);
			this.m_pitchSpeed = MathUtils.Lerp(this.m_pitchSpeed, -0.2f * vector2.Y, 3f * dt);
			this.m_pitchSpeed *= MathUtils.Pow(0.33f, dt);
			this.m_velocity += 1.5f * (v - this.m_velocity) * dt;
			this.m_position += this.m_velocity * dt;
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(unitY, 0.05f * this.m_rollAngle));
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(vector3, 0.2f * this.m_pitchSpeed));
			Vector3 up = Vector3.TransformNormal(Vector3.UnitY, Matrix.CreateFromAxisAngle(this.m_direction, 0f - this.m_rollAngle));
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, up);
		}

		// Token: 0x04000D70 RID: 3440
		public Vector3 m_position;

		// Token: 0x04000D71 RID: 3441
		public Vector3 m_direction;

		// Token: 0x04000D72 RID: 3442
		public Vector3 m_velocity;

		// Token: 0x04000D73 RID: 3443
		public float m_rollSpeed;

		// Token: 0x04000D74 RID: 3444
		public float m_pitchSpeed;

		// Token: 0x04000D75 RID: 3445
		public float m_rollAngle;
	}
}
