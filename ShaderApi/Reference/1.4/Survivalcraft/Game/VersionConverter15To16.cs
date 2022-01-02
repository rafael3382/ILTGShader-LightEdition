using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000350 RID: 848
	public class VersionConverter15To16 : VersionConverter
	{
		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060018E7 RID: 6375 RVA: 0x000C2E64 File Offset: 0x000C1064
		public override string SourceVersion
		{
			get
			{
				return "1.5";
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060018E8 RID: 6376 RVA: 0x000C2E6B File Offset: 0x000C106B
		public override string TargetVersion
		{
			get
			{
				return "1.6";
			}
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x000C2E72 File Offset: 0x000C1072
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x000C2E88 File Offset: 0x000C1088
		public override void ConvertWorld(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}
	}
}
