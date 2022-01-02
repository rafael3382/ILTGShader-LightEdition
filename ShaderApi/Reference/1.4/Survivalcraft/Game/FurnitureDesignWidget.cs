using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000388 RID: 904
	public class FurnitureDesignWidget : Widget
	{
		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x000D13F0 File Offset: 0x000CF5F0
		// (set) Token: 0x06001ADF RID: 6879 RVA: 0x000D13F8 File Offset: 0x000CF5F8
		public Vector2 Size { get; set; }

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x000D1401 File Offset: 0x000CF601
		// (set) Token: 0x06001AE1 RID: 6881 RVA: 0x000D1409 File Offset: 0x000CF609
		public FurnitureDesignWidget.ViewMode Mode { get; set; }

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001AE2 RID: 6882 RVA: 0x000D1412 File Offset: 0x000CF612
		// (set) Token: 0x06001AE3 RID: 6883 RVA: 0x000D141A File Offset: 0x000CF61A
		public FurnitureDesign Design { get; set; }

		// Token: 0x06001AE4 RID: 6884 RVA: 0x000D1424 File Offset: 0x000CF624
		public FurnitureDesignWidget()
		{
			base.ClampToBounds = true;
			this.Size = new Vector2(float.PositiveInfinity);
			this.Mode = FurnitureDesignWidget.ViewMode.Perspective;
			this.m_direction = Vector3.Normalize(new Vector3(1f, -0.5f, -1f));
			this.m_rotationSpeed = new Vector2(2f, 0.5f);
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x000D14A0 File Offset: 0x000CF6A0
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Design == null)
			{
				return;
			}
			Matrix matrix;
			if (this.Mode == FurnitureDesignWidget.ViewMode.Perspective)
			{
				Viewport viewport = Display.Viewport;
				Vector3 vector = new Vector3(0.5f, 0.5f, 0.5f);
				Matrix m = Matrix.CreateLookAt(2.65f * this.m_direction + vector, vector, Vector3.UnitY);
				Matrix m2 = Matrix.CreatePerspectiveFieldOfView(1.2f, base.ActualSize.X / base.ActualSize.Y, 0.4f, 4f);
				Matrix m3 = MatrixUtils.CreateScaleTranslation(base.ActualSize.X, 0f - base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
				matrix = m * m2 * m3;
				FlatBatch3D flatBatch3D = this.m_primitivesRenderer3d.FlatBatch(1, DepthStencilState.DepthRead, null, null);
				for (int i = 0; i <= this.Design.Resolution; i++)
				{
					float num = (float)i / (float)this.Design.Resolution;
					Color color = (i % 2 == 0) ? new Color(56, 56, 56, 56) : new Color(28, 28, 28, 28);
					color *= base.GlobalColorTransform;
					flatBatch3D.QueueLine(new Vector3(num, 0f, 0f), new Vector3(num, 0f, 1f), color);
					flatBatch3D.QueueLine(new Vector3(0f, 0f, num), new Vector3(1f, 0f, num), color);
					flatBatch3D.QueueLine(new Vector3(0f, num, 0f), new Vector3(0f, num, 1f), color);
					flatBatch3D.QueueLine(new Vector3(0f, 0f, num), new Vector3(0f, 1f, num), color);
					flatBatch3D.QueueLine(new Vector3(0f, num, 1f), new Vector3(1f, num, 1f), color);
					flatBatch3D.QueueLine(new Vector3(num, 0f, 1f), new Vector3(num, 1f, 1f), color);
				}
				Color color2 = new Color(64, 64, 64, 255) * base.GlobalColorTransform;
				FontBatch3D fontBatch3D = this.m_primitivesRenderer3d.FontBatch(LabelWidget.BitmapFont, 1, null, null, null, null);
				fontBatch3D.QueueText(LanguageControl.Get(base.GetType().Name, 0), new Vector3(0.5f, 0f, 0f), 0.004f * new Vector3(-1f, 0f, 0f), 0.004f * new Vector3(0f, 0f, -1f), color2, TextAnchor.HorizontalCenter);
				fontBatch3D.QueueText(LanguageControl.Get(base.GetType().Name, 1), new Vector3(1f, 0f, 0.5f), 0.004f * new Vector3(0f, 0f, -1f), 0.004f * new Vector3(1f, 0f, 0f), color2, TextAnchor.HorizontalCenter);
				if (FurnitureDesignWidget.DrawDebugFurniture)
				{
					this.DebugDraw();
				}
			}
			else
			{
				Vector3 position;
				Vector3 up;
				if (this.Mode == FurnitureDesignWidget.ViewMode.Side)
				{
					position = new Vector3(1f, 0f, 0f);
					up = new Vector3(0f, 1f, 0f);
				}
				else if (this.Mode != FurnitureDesignWidget.ViewMode.Top)
				{
					position = new Vector3(0f, 0f, -10f);
					up = new Vector3(0f, 1f, 0f);
				}
				else
				{
					position = new Vector3(0f, 1f, 0f);
					up = new Vector3(0f, 0f, 1f);
				}
				Viewport viewport2 = Display.Viewport;
				float num2 = MathUtils.Min(base.ActualSize.X, base.ActualSize.Y);
				Matrix m4 = Matrix.CreateLookAt(position, new Vector3(0f, 0f, 0f), up);
				Matrix m5 = Matrix.CreateOrthographic(2f, 2f, -10f, 10f);
				Matrix m6 = MatrixUtils.CreateScaleTranslation(num2, 0f - num2, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport2.Width, -2f / (float)viewport2.Height, -1f, 1f);
				matrix = Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) * m4 * m5 * m6;
				FlatBatch2D flatBatch2D = this.m_primitivesRenderer2d.FlatBatch(0, null, null, null);
				Matrix globalTransform = base.GlobalTransform;
				for (int j = 1; j < this.Design.Resolution; j++)
				{
					float num3 = (float)j / (float)this.Design.Resolution;
					Vector2 vector2 = new Vector2(base.ActualSize.X * num3, 0f);
					Vector2 vector3 = new Vector2(base.ActualSize.X * num3, base.ActualSize.Y);
					Vector2 vector4 = new Vector2(0f, base.ActualSize.Y * num3);
					Vector2 vector5 = new Vector2(base.ActualSize.X, base.ActualSize.Y * num3);
					Vector2.Transform(ref vector2, ref globalTransform, out vector2);
					Vector2.Transform(ref vector3, ref globalTransform, out vector3);
					Vector2.Transform(ref vector4, ref globalTransform, out vector4);
					Vector2.Transform(ref vector5, ref globalTransform, out vector5);
					Color color3 = (j % 2 == 0) ? new Color(0, 0, 0, 56) : new Color(0, 0, 0, 28);
					Color color4 = (j % 2 == 0) ? new Color(56, 56, 56, 56) : new Color(28, 28, 28, 28);
					color3 *= base.GlobalColorTransform;
					color4 *= base.GlobalColorTransform;
					flatBatch2D.QueueLine(vector2, vector3, 0f, (j % 2 == 0) ? color3 : (color3 * 0.75f));
					flatBatch2D.QueueLine(vector2 + new Vector2(1f, 0f), vector3 + new Vector2(1f, 0f), 0f, color4);
					flatBatch2D.QueueLine(vector4, vector5, 0f, color3);
					flatBatch2D.QueueLine(vector4 + new Vector2(0f, 1f), vector5 + new Vector2(0f, 1f), 0f, color4);
				}
			}
			Matrix identity = Matrix.Identity;
			FurnitureGeometry geometry = this.Design.Geometry;
			for (int k = 0; k < 6; k++)
			{
				Color color5 = base.GlobalColorTransform;
				if (this.Mode == FurnitureDesignWidget.ViewMode.Perspective)
				{
					float num4 = LightingManager.LightIntensityByLightValueAndFace[15 + 16 * CellFace.OppositeFace(k)];
					color5 *= new Color(num4, num4, num4);
				}
				if (geometry.SubsetOpaqueByFace[k] != null)
				{
					BlocksManager.DrawMeshBlock(this.m_primitivesRenderer3d, geometry.SubsetOpaqueByFace[k], color5, 1f, ref identity, null);
				}
				if (geometry.SubsetAlphaTestByFace[k] != null)
				{
					BlocksManager.DrawMeshBlock(this.m_primitivesRenderer3d, geometry.SubsetAlphaTestByFace[k], color5, 1f, ref identity, null);
				}
			}
			this.m_primitivesRenderer3d.Flush(matrix, true, int.MaxValue);
			this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x000D1CB0 File Offset: 0x000CFEB0
		public override void Update()
		{
			if (this.Mode != FurnitureDesignWidget.ViewMode.Perspective)
			{
				return;
			}
			if (base.Input.Tap != null && base.HitTestGlobal(base.Input.Tap.Value, null) == this)
			{
				this.m_dragStartPoint = base.Input.Tap;
			}
			if (base.Input.Press != null)
			{
				if (this.m_dragStartPoint != null)
				{
					Vector2 vector = base.ScreenToWidget(base.Input.Press.Value) - base.ScreenToWidget(this.m_dragStartPoint.Value);
					Vector2 vector2 = default(Vector2);
					vector2.Y = -0.01f * vector.X;
					vector2.X = 0.01f * vector.Y;
					if (Time.FrameDuration > 0f)
					{
						this.m_rotationSpeed = vector2 / Time.FrameDuration;
					}
					this.Rotate(vector2);
					this.m_dragStartPoint = base.Input.Press;
					return;
				}
			}
			else
			{
				this.m_dragStartPoint = null;
				this.Rotate(this.m_rotationSpeed * Time.FrameDuration);
				this.m_rotationSpeed *= MathUtils.Pow(0.1f, Time.FrameDuration);
			}
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x000D1E08 File Offset: 0x000D0008
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.Design != null);
			base.DesiredSize = this.Size;
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x000D1E28 File Offset: 0x000D0028
		public void Rotate(Vector2 angles)
		{
			float num = MathUtils.DegToRad(1f);
			Vector3 axis = Vector3.Normalize(Vector3.Cross(this.m_direction, Vector3.UnitY));
			this.m_direction = Vector3.TransformNormal(this.m_direction, Matrix.CreateRotationY(angles.Y));
			float num2 = MathUtils.Acos(Vector3.Dot(this.m_direction, Vector3.UnitY));
			float num3 = MathUtils.Acos(Vector3.Dot(this.m_direction, -Vector3.UnitY));
			angles.X = MathUtils.Min(angles.X, num2 - num);
			angles.X = MathUtils.Max(angles.X, 0f - (num3 - num));
			this.m_direction = Vector3.TransformNormal(this.m_direction, Matrix.CreateFromAxisAngle(axis, angles.X));
			this.m_direction = Vector3.Normalize(this.m_direction);
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x000D1F01 File Offset: 0x000D0101
		public void DebugDraw()
		{
		}

		// Token: 0x04001251 RID: 4689
		public static string fName = "FurnitureDesignWidget";

		// Token: 0x04001252 RID: 4690
		public PrimitivesRenderer2D m_primitivesRenderer2d = new PrimitivesRenderer2D();

		// Token: 0x04001253 RID: 4691
		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		// Token: 0x04001254 RID: 4692
		public Vector2? m_dragStartPoint;

		// Token: 0x04001255 RID: 4693
		public Vector3 m_direction;

		// Token: 0x04001256 RID: 4694
		public Vector2 m_rotationSpeed;

		// Token: 0x04001257 RID: 4695
		public static bool DrawDebugFurniture;

		// Token: 0x02000580 RID: 1408
		public enum ViewMode
		{
			// Token: 0x040019DF RID: 6623
			Side,
			// Token: 0x040019E0 RID: 6624
			Top,
			// Token: 0x040019E1 RID: 6625
			Front,
			// Token: 0x040019E2 RID: 6626
			Perspective
		}
	}
}
