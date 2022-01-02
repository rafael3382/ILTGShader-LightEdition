using System;
using System.Diagnostics;

namespace Game
{
	// Token: 0x02000269 RID: 617
	[Conditional("DEBUG")]
	public class DebugItemAttribute : Attribute
	{
		// Token: 0x04000CA8 RID: 3240
		public int Precision = 3;

		// Token: 0x04000CA9 RID: 3241
		public string Unit;
	}
}
