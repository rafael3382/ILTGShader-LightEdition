using System;
using Engine;

namespace Game
{
	// Token: 0x020002BE RID: 702
	public class LoadingCamera : BasePerspectiveCamera
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06001583 RID: 5507 RVA: 0x000A2292 File Offset: 0x000A0492
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x000A2295 File Offset: 0x000A0495
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x000A2298 File Offset: 0x000A0498
		public LoadingCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x000A22A1 File Offset: 0x000A04A1
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x000A22BB File Offset: 0x000A04BB
		public override void Update(float dt)
		{
			base.SetupPerspectiveCamera(base.GameWidget.PlayerData.SpawnPosition, Vector3.UnitX, Vector3.UnitY);
		}
	}
}
