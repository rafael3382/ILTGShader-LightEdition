using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x02000291 RID: 657
	[Conditional("DEBUG")]
	public class FloatListDebugMenuItemAttribute : DebugMenuItemAttribute
	{
		// Token: 0x0600149F RID: 5279 RVA: 0x0009AD18 File Offset: 0x00098F18
		public FloatListDebugMenuItemAttribute(float[] items, int precision, string unit) : base(0.0)
		{
			this.Items = items;
			this.Precision = precision;
			this.Unit = unit;
		}

		// Token: 0x04000D6C RID: 3436
		public float[] Items;
	}
}
