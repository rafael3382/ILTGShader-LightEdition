using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x0200026B RID: 619
	[Conditional("DEBUG")]
	public class DebugMenuItemAttribute : DebugItemAttribute
	{
		// Token: 0x060013FD RID: 5117 RVA: 0x0009588C File Offset: 0x00093A8C
		public DebugMenuItemAttribute()
		{
			this.Step = 1.0;
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x000958A3 File Offset: 0x00093AA3
		public DebugMenuItemAttribute(double step)
		{
			this.Step = step;
		}

		// Token: 0x04000CAA RID: 3242
		public double Step;
	}
}
