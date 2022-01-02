using System;
using Engine;

namespace Game
{
	// Token: 0x02000267 RID: 615
	public class DeathCamera : BasePerspectiveCamera
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060013EF RID: 5103 RVA: 0x000952B4 File Offset: 0x000934B4
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060013F0 RID: 5104 RVA: 0x000952B7 File Offset: 0x000934B7
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x000952BA File Offset: 0x000934BA
		public DeathCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x000952C4 File Offset: 0x000934C4
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			ComponentCreature target = base.GameWidget.Target;
			Vector3 vector = (target != null) ? target.ComponentBody.BoundingBox.Center() : this.m_position;
			this.m_bestPosition = new Vector3?(this.FindBestCameraPosition(vector, 6f));
			base.SetupPerspectiveCamera(this.m_position, vector - this.m_position, Vector3.UnitY);
			if (base.GameWidget.Target is ComponentPlayer && this.m_bestPosition != null)
			{
				Vector3 vector2 = Matrix.CreateWorld(Vector3.Zero, vector - this.m_bestPosition.Value, Vector3.UnitY).ToYawPitchRoll();
				this.m_vrDeltaYaw = vector2.X;
			}
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x00095390 File Offset: 0x00093590
		public override void Update(float dt)
		{
			ComponentCreature target = base.GameWidget.Target;
			Vector3 v = (target != null) ? target.ComponentBody.BoundingBox.Center() : this.m_position;
			if (this.m_bestPosition != null)
			{
				if (Vector3.Distance(this.m_bestPosition.Value, this.m_position) > 20f)
				{
					this.m_position = this.m_bestPosition.Value;
				}
				this.m_position += 1.5f * dt * (this.m_bestPosition.Value - this.m_position);
			}
			base.SetupPerspectiveCamera(this.m_position, v - this.m_position, Vector3.UnitY);
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x00095454 File Offset: 0x00093654
		public Vector3 FindBestCameraPosition(Vector3 targetPosition, float distance)
		{
			Vector3? vector = null;
			for (int i = 0; i < 36; i++)
			{
				float x = 1f + 6.28318548f * (float)i / 36f;
				Vector3 v2 = Vector3.Normalize(new Vector3(MathUtils.Sin(x), 0.5f, MathUtils.Cos(x)));
				Vector3 vector2 = targetPosition + v2 * distance;
				TerrainRaycastResult? terrainRaycastResult = base.GameWidget.SubsystemGameWidgets.SubsystemTerrain.Raycast(targetPosition, vector2, false, true, (int v, float d) => !BlocksManager.Blocks[Terrain.ExtractContents(v)].IsTransparent_(v));
				Vector3 vector3 = Vector3.Zero;
				if (terrainRaycastResult != null)
				{
					CellFace cellFace = terrainRaycastResult.Value.CellFace;
					vector3 = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) - 1f * v2;
				}
				else
				{
					vector3 = vector2;
				}
				if (vector == null || Vector3.Distance(vector3, targetPosition) > Vector3.Distance(vector.Value, targetPosition))
				{
					vector = new Vector3?(vector3);
				}
			}
			if (vector != null)
			{
				return vector.Value;
			}
			return targetPosition;
		}

		// Token: 0x04000CA0 RID: 3232
		public Vector3 m_position;

		// Token: 0x04000CA1 RID: 3233
		public Vector3? m_bestPosition;

		// Token: 0x04000CA2 RID: 3234
		public float m_vrDeltaYaw;
	}
}
