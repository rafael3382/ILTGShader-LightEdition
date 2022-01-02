using System;
using Engine;

namespace Game
{
	// Token: 0x02000296 RID: 662
	public class FppCamera : BasePerspectiveCamera
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060014A9 RID: 5289 RVA: 0x0009B2F8 File Offset: 0x000994F8
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x0009B2FB File Offset: 0x000994FB
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0009B2FE File Offset: 0x000994FE
		public FppCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x0009B307 File Offset: 0x00099507
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x0009B324 File Offset: 0x00099524
		public override void Update(float dt)
		{
			if (base.GameWidget.Target != null)
			{
				Matrix matrix = Matrix.CreateFromQuaternion(base.GameWidget.Target.ComponentCreatureModel.EyeRotation);
				matrix.Translation = base.GameWidget.Target.ComponentCreatureModel.EyePosition;
				base.SetupPerspectiveCamera(matrix.Translation, matrix.Forward, matrix.Up);
			}
		}
	}
}
