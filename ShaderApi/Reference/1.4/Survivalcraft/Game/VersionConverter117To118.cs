using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000341 RID: 833
	public class VersionConverter117To118 : VersionConverter
	{
		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06001894 RID: 6292 RVA: 0x000C0F78 File Offset: 0x000BF178
		public override string SourceVersion
		{
			get
			{
				return "1.17";
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001895 RID: 6293 RVA: 0x000C0F7F File Offset: 0x000BF17F
		public override string TargetVersion
		{
			get
			{
				return "1.18";
			}
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x000C0F86 File Offset: 0x000BF186
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x000C0F9C File Offset: 0x000BF19C
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
