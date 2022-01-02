using System;
using Engine;

namespace Game
{
	// Token: 0x02000313 RID: 787
	public class StraightFlightCamera : BasePerspectiveCamera
	{
		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06001703 RID: 5891 RVA: 0x000AD61D File Offset: 0x000AB81D
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x000AD620 File Offset: 0x000AB820
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x000AD623 File Offset: 0x000AB823
		public StraightFlightCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x000AD62C File Offset: 0x000AB82C
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			base.SetupPerspectiveCamera(this.m_position, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x000AD654 File Offset: 0x000AB854
		public override void Update(float dt)
		{
			Vector3 vector = 10f * (Vector3.UnitX + (float)MathUtils.Sin(0.20000000298023224 * Time.FrameStartTime) * Vector3.UnitZ);
			this.m_position.Y = 120f;
			this.m_position += vector * dt;
			base.SetupPerspectiveCamera(this.m_position, vector, Vector3.UnitY);
		}

		// Token: 0x04000FC1 RID: 4033
		public Vector3 m_position;
	}
}
