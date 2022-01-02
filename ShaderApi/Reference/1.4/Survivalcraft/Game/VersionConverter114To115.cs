using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033E RID: 830
	public class VersionConverter114To115 : VersionConverter
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001885 RID: 6277 RVA: 0x000C0D6A File Offset: 0x000BEF6A
		public override string SourceVersion
		{
			get
			{
				return "1.14";
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x000C0D71 File Offset: 0x000BEF71
		public override string TargetVersion
		{
			get
			{
				return "1.15";
			}
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x000C0D78 File Offset: 0x000BEF78
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x000C0D8C File Offset: 0x000BEF8C
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
