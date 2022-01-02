using System;
using Engine;

namespace Game
{
	// Token: 0x020002B2 RID: 690
	public class IntroCamera : BasePerspectiveCamera
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x000A14C9 File Offset: 0x0009F6C9
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001552 RID: 5458 RVA: 0x000A14CC File Offset: 0x0009F6CC
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001553 RID: 5459 RVA: 0x000A14CF File Offset: 0x0009F6CF
		// (set) Token: 0x06001554 RID: 5460 RVA: 0x000A14D7 File Offset: 0x0009F6D7
		public Vector3 CameraPosition { get; set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x000A14E0 File Offset: 0x0009F6E0
		// (set) Token: 0x06001556 RID: 5462 RVA: 0x000A14E8 File Offset: 0x0009F6E8
		public Vector3 TargetPosition { get; set; }

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001557 RID: 5463 RVA: 0x000A14F1 File Offset: 0x0009F6F1
		// (set) Token: 0x06001558 RID: 5464 RVA: 0x000A14F9 File Offset: 0x0009F6F9
		public Vector3 TargetCameraPosition { get; set; }

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x000A1502 File Offset: 0x0009F702
		// (set) Token: 0x0600155A RID: 5466 RVA: 0x000A150A File Offset: 0x0009F70A
		public float Speed { get; set; }

		// Token: 0x0600155B RID: 5467 RVA: 0x000A1513 File Offset: 0x0009F713
		public IntroCamera(GameWidget gameWidget) : base(gameWidget)
		{
			this.Speed = 1f;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000A1527 File Offset: 0x0009F727
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000A1544 File Offset: 0x0009F744
		public override void Update(float dt)
		{
			float x = Vector3.Distance(this.TargetCameraPosition, this.CameraPosition);
			this.CameraPosition += MathUtils.Min(dt * this.Speed, x) * Vector3.Normalize(this.TargetCameraPosition - this.CameraPosition);
			base.SetupPerspectiveCamera(this.CameraPosition, this.TargetPosition - this.CameraPosition, Vector3.UnitY);
		}
	}
}
