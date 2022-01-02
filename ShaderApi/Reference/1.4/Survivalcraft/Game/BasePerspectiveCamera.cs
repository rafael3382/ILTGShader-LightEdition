using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000242 RID: 578
	public abstract class BasePerspectiveCamera : Camera
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x0008B694 File Offset: 0x00089894
		public override Vector3 ViewPosition
		{
			get
			{
				return this.m_viewPosition;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x0008B69C File Offset: 0x0008989C
		public override Vector3 ViewDirection
		{
			get
			{
				return this.m_viewDirection;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x060012CB RID: 4811 RVA: 0x0008B6A4 File Offset: 0x000898A4
		public override Vector3 ViewUp
		{
			get
			{
				return this.m_viewUp;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x0008B6AC File Offset: 0x000898AC
		public override Vector3 ViewRight
		{
			get
			{
				return this.m_viewRight;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x060012CD RID: 4813 RVA: 0x0008B6B4 File Offset: 0x000898B4
		public override Matrix ViewMatrix
		{
			get
			{
				if (this.m_viewMatrix == null)
				{
					this.m_viewMatrix = new Matrix?(Matrix.CreateLookAt(this.m_viewPosition, this.m_viewPosition + this.m_viewDirection, this.m_viewUp));
				}
				return this.m_viewMatrix.Value;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x060012CE RID: 4814 RVA: 0x0008B706 File Offset: 0x00089906
		public override Matrix InvertedViewMatrix
		{
			get
			{
				if (this.m_invertedViewMatrix == null)
				{
					this.m_invertedViewMatrix = new Matrix?(Matrix.Invert(this.ViewMatrix));
				}
				return this.m_invertedViewMatrix.Value;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x060012CF RID: 4815 RVA: 0x0008B738 File Offset: 0x00089938
		public override Matrix ProjectionMatrix
		{
			get
			{
				if (this.m_projectionMatrix == null)
				{
					this.m_projectionMatrix = new Matrix?(BasePerspectiveCamera.CalculateBaseProjectionMatrix(base.GameWidget.ViewWidget.ActualSize));
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					if (viewWidget.ScalingRenderTargetSize == null)
					{
						this.m_projectionMatrix *= MatrixUtils.CreateScaleTranslation(0.5f * viewWidget.ActualSize.X, -0.5f * viewWidget.ActualSize.Y, viewWidget.ActualSize.X / 2f, viewWidget.ActualSize.Y / 2f) * viewWidget.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)Display.Viewport.Width, -2f / (float)Display.Viewport.Height, -1f, 1f);
					}
				}
				return this.m_projectionMatrix.Value;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x060012D0 RID: 4816 RVA: 0x0008B860 File Offset: 0x00089A60
		public override Matrix ScreenProjectionMatrix
		{
			get
			{
				if (this.m_screenProjectionMatrix == null)
				{
					Point2 size = Window.Size;
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					this.m_screenProjectionMatrix = new Matrix?(BasePerspectiveCamera.CalculateBaseProjectionMatrix(base.GameWidget.ViewWidget.ActualSize) * MatrixUtils.CreateScaleTranslation(0.5f * viewWidget.ActualSize.X, -0.5f * viewWidget.ActualSize.Y, viewWidget.ActualSize.X / 2f, viewWidget.ActualSize.Y / 2f) * viewWidget.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)size.X, -2f / (float)size.Y, -1f, 1f));
				}
				return this.m_screenProjectionMatrix.Value;
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x060012D1 RID: 4817 RVA: 0x0008B941 File Offset: 0x00089B41
		public override Matrix InvertedProjectionMatrix
		{
			get
			{
				if (this.m_invertedProjectionMatrix == null)
				{
					this.m_invertedProjectionMatrix = new Matrix?(Matrix.Invert(this.ProjectionMatrix));
				}
				return this.m_invertedProjectionMatrix.Value;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x060012D2 RID: 4818 RVA: 0x0008B971 File Offset: 0x00089B71
		public override Matrix ViewProjectionMatrix
		{
			get
			{
				if (this.m_viewProjectionMatrix == null)
				{
					this.m_viewProjectionMatrix = new Matrix?(this.ViewMatrix * this.ProjectionMatrix);
				}
				return this.m_viewProjectionMatrix.Value;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x060012D3 RID: 4819 RVA: 0x0008B9A8 File Offset: 0x00089BA8
		public override Vector2 ViewportSize
		{
			get
			{
				if (this.m_viewportSize == null)
				{
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					this.m_viewportSize = new Vector2?((viewWidget.ScalingRenderTargetSize != null) ? new Vector2(viewWidget.ScalingRenderTargetSize.Value) : new Vector2(viewWidget.ActualSize.X * viewWidget.GlobalTransform.Right.Length(), viewWidget.ActualSize.Y * viewWidget.GlobalTransform.Up.Length()));
				}
				return this.m_viewportSize.Value;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x0008BA58 File Offset: 0x00089C58
		public override Matrix ViewportMatrix
		{
			get
			{
				if (this.m_viewportMatrix == null)
				{
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					if (viewWidget.ScalingRenderTargetSize != null)
					{
						this.m_viewportMatrix = new Matrix?(Matrix.Identity);
					}
					else
					{
						Matrix identity = Matrix.Identity;
						identity.Right = Vector3.Normalize(viewWidget.GlobalTransform.Right);
						identity.Up = Vector3.Normalize(viewWidget.GlobalTransform.Up);
						identity.Forward = viewWidget.GlobalTransform.Forward;
						identity.Translation = viewWidget.GlobalTransform.Translation;
						this.m_viewportMatrix = new Matrix?(identity);
					}
				}
				return this.m_viewportMatrix.Value;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x060012D5 RID: 4821 RVA: 0x0008BB20 File Offset: 0x00089D20
		public override BoundingFrustum ViewFrustum
		{
			get
			{
				if (!this.m_viewFrustumValid)
				{
					if (this.m_viewFrustum == null)
					{
						this.m_viewFrustum = new BoundingFrustum(this.ViewProjectionMatrix);
					}
					else
					{
						this.m_viewFrustum.Matrix = this.ViewProjectionMatrix;
					}
					this.m_viewFrustumValid = true;
				}
				return this.m_viewFrustum;
			}
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0008BB74 File Offset: 0x00089D74
		public override void PrepareForDrawing()
		{
			this.m_viewMatrix = null;
			this.m_invertedViewMatrix = null;
			this.m_projectionMatrix = null;
			this.m_invertedProjectionMatrix = null;
			this.m_screenProjectionMatrix = null;
			this.m_viewProjectionMatrix = null;
			this.m_viewportSize = null;
			this.m_viewportMatrix = null;
			this.m_viewFrustumValid = false;
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0008BBE8 File Offset: 0x00089DE8
		public BasePerspectiveCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0008BBF1 File Offset: 0x00089DF1
		public void SetupPerspectiveCamera(Vector3 position, Vector3 direction, Vector3 up)
		{
			this.m_viewPosition = position;
			this.m_viewDirection = Vector3.Normalize(direction);
			this.m_viewUp = Vector3.Normalize(up);
			this.m_viewRight = Vector3.Normalize(Vector3.Cross(this.m_viewDirection, this.m_viewUp));
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0008BC30 File Offset: 0x00089E30
		public static Matrix CalculateBaseProjectionMatrix(Vector2 wh)
		{
			float num = 90f;
			float num2 = 1f;
			if (SettingsManager.ViewAngleMode == ViewAngleMode.Narrow)
			{
				num2 = 0.8f;
			}
			else if (SettingsManager.ViewAngleMode == ViewAngleMode.Normal)
			{
				num2 = 0.9f;
			}
			float num3 = wh.X / wh.Y;
			float num4 = MathUtils.Min(num * num3, num);
			float num5 = num4 * num3;
			if (num5 < 90f)
			{
				num4 *= 90f / num5;
			}
			else if (num5 > 175f)
			{
				num4 *= 175f / num5;
			}
			return Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(num4 * num2), num3, 0.1f, 2048f);
		}

		// Token: 0x04000B8D RID: 2957
		public Vector3 m_viewPosition;

		// Token: 0x04000B8E RID: 2958
		public Vector3 m_viewDirection;

		// Token: 0x04000B8F RID: 2959
		public Vector3 m_viewUp;

		// Token: 0x04000B90 RID: 2960
		public Vector3 m_viewRight;

		// Token: 0x04000B91 RID: 2961
		public Matrix? m_viewMatrix;

		// Token: 0x04000B92 RID: 2962
		public Matrix? m_invertedViewMatrix;

		// Token: 0x04000B93 RID: 2963
		public Matrix? m_projectionMatrix;

		// Token: 0x04000B94 RID: 2964
		public Matrix? m_invertedProjectionMatrix;

		// Token: 0x04000B95 RID: 2965
		public Matrix? m_screenProjectionMatrix;

		// Token: 0x04000B96 RID: 2966
		public Matrix? m_viewProjectionMatrix;

		// Token: 0x04000B97 RID: 2967
		public Vector2? m_viewportSize;

		// Token: 0x04000B98 RID: 2968
		public Matrix? m_viewportMatrix;

		// Token: 0x04000B99 RID: 2969
		public BoundingFrustum m_viewFrustum;

		// Token: 0x04000B9A RID: 2970
		public bool m_viewFrustumValid;
	}
}
