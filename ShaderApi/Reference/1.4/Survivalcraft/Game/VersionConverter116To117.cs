using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000340 RID: 832
	public class VersionConverter116To117 : VersionConverter
	{
		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x0600188F RID: 6287 RVA: 0x000C0EC8 File Offset: 0x000BF0C8
		public override string SourceVersion
		{
			get
			{
				return "1.16";
			}
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06001890 RID: 6288 RVA: 0x000C0ECF File Offset: 0x000BF0CF
		public override string TargetVersion
		{
			get
			{
				return "1.17";
			}
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x000C0ED6 File Offset: 0x000BF0D6
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000C0EEC File Offset: 0x000BF0EC
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
