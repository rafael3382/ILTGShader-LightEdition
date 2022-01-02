using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000346 RID: 838
	public class VersionConverter122To123 : VersionConverter
	{
		// Token: 0x170003AE RID: 942
		// (get) Token: 0x060018AD RID: 6317 RVA: 0x000C16D0 File Offset: 0x000BF8D0
		public override string SourceVersion
		{
			get
			{
				return "1.22";
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x060018AE RID: 6318 RVA: 0x000C16D7 File Offset: 0x000BF8D7
		public override string TargetVersion
		{
			get
			{
				return "1.23";
			}
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x000C16DE File Offset: 0x000BF8DE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x000C16F4 File Offset: 0x000BF8F4
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
