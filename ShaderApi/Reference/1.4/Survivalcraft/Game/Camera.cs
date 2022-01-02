using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000257 RID: 599
	public abstract class Camera
	{
		// Token: 0x170002DE RID: 734
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x00093C96 File Offset: 0x00091E96
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x00093C9E File Offset: 0x00091E9E
		public GameWidget GameWidget { get; set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x0600139D RID: 5021
		public abstract Vector3 ViewPosition { get; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x0600139E RID: 5022
		public abstract Vector3 ViewDirection { get; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x0600139F RID: 5023
		public abstract Vector3 ViewUp { get; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060013A0 RID: 5024
		public abstract Vector3 ViewRight { get; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060013A1 RID: 5025
		public abstract Matrix ViewMatrix { get; }

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060013A2 RID: 5026
		public abstract Matrix InvertedViewMatrix { get; }

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060013A3 RID: 5027
		public abstract Matrix ProjectionMatrix { get; }

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060013A4 RID: 5028
		public abstract Matrix ScreenProjectionMatrix { get; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060013A5 RID: 5029
		public abstract Matrix InvertedProjectionMatrix { get; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x060013A6 RID: 5030
		public abstract Matrix ViewProjectionMatrix { get; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x060013A7 RID: 5031
		public abstract Vector2 ViewportSize { get; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x060013A8 RID: 5032
		public abstract Matrix ViewportMatrix { get; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x060013A9 RID: 5033
		public abstract BoundingFrustum ViewFrustum { get; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x060013AA RID: 5034
		public abstract bool UsesMovementControls { get; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060013AB RID: 5035
		public abstract bool IsEntityControlEnabled { get; }

		// Token: 0x060013AC RID: 5036 RVA: 0x00093CA7 File Offset: 0x00091EA7
		public Camera(GameWidget gameWidget)
		{
			this.GameWidget = gameWidget;
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x00093CB8 File Offset: 0x00091EB8
		public Vector3 WorldToScreen(Vector3 worldPoint, Matrix worldMatrix)
		{
			return new Viewport(0, 0, Window.Size.X, Window.Size.Y, 0f, 1f).Project(worldPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x00093D00 File Offset: 0x00091F00
		public Vector3 ScreenToWorld(Vector3 screenPoint, Matrix worldMatrix)
		{
			return new Viewport(0, 0, Window.Size.X, Window.Size.Y, 0f, 1f).Unproject(screenPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00093D48 File Offset: 0x00091F48
		public virtual void Activate(Camera previousCamera)
		{
		}

		// Token: 0x060013B0 RID: 5040
		public abstract void Update(float dt);

		// Token: 0x060013B1 RID: 5041 RVA: 0x00093D4A File Offset: 0x00091F4A
		public virtual void PrepareForDrawing()
		{
		}
	}
}
