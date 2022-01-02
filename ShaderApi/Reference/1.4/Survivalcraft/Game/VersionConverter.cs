using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000338 RID: 824
	public abstract class VersionConverter
	{
		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001863 RID: 6243
		public abstract string SourceVersion { get; }

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001864 RID: 6244
		public abstract string TargetVersion { get; }

		// Token: 0x06001865 RID: 6245
		public abstract void ConvertProjectXml(XElement projectNode);

		// Token: 0x06001866 RID: 6246
		public abstract void ConvertWorld(string directoryName);
	}
}
