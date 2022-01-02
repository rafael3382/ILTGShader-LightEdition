using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000347 RID: 839
	public class VersionConverter123To124 : VersionConverter
	{
		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x060018B2 RID: 6322 RVA: 0x000C1780 File Offset: 0x000BF980
		public override string SourceVersion
		{
			get
			{
				return "1.23";
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060018B3 RID: 6323 RVA: 0x000C1787 File Offset: 0x000BF987
		public override string TargetVersion
		{
			get
			{
				return "1.24";
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x000C178E File Offset: 0x000BF98E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x000C17A4 File Offset: 0x000BF9A4
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
