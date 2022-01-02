using System;

namespace Game
{
	// Token: 0x02000290 RID: 656
	public class FixedCamera : BasePerspectiveCamera
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600149A RID: 5274 RVA: 0x0009ACED File Offset: 0x00098EED
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x0009ACF0 File Offset: 0x00098EF0
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x0009ACF3 File Offset: 0x00098EF3
		public FixedCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x0009ACFC File Offset: 0x00098EFC
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0009AD16 File Offset: 0x00098F16
		public override void Update(float dt)
		{
		}
	}
}
