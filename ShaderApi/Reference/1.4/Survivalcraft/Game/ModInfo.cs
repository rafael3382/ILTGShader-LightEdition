using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200036A RID: 874
	public class ModInfo
	{
		// Token: 0x060019AA RID: 6570 RVA: 0x000CAEB1 File Offset: 0x000C90B1
		public override int GetHashCode()
		{
			return (this.PackageName + this.ApiVersion + this.Version).GetHashCode();
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x000CAECF File Offset: 0x000C90CF
		public override bool Equals(object obj)
		{
			return obj is ModInfo && obj.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x04001194 RID: 4500
		public string Name;

		// Token: 0x04001195 RID: 4501
		public string Version;

		// Token: 0x04001196 RID: 4502
		public string ApiVersion;

		// Token: 0x04001197 RID: 4503
		public string Description;

		// Token: 0x04001198 RID: 4504
		public string ScVersion;

		// Token: 0x04001199 RID: 4505
		public string Link;

		// Token: 0x0400119A RID: 4506
		public string Author;

		// Token: 0x0400119B RID: 4507
		public string PackageName;

		// Token: 0x0400119C RID: 4508
		public List<string> Dependencies = new List<string>();
	}
}
