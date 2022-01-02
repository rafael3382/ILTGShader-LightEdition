using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000348 RID: 840
	public class VersionConverter124To125 : VersionConverter
	{
		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060018B7 RID: 6327 RVA: 0x000C1830 File Offset: 0x000BFA30
		public override string SourceVersion
		{
			get
			{
				return "1.24";
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060018B8 RID: 6328 RVA: 0x000C1837 File Offset: 0x000BFA37
		public override string TargetVersion
		{
			get
			{
				return "1.25";
			}
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x000C183E File Offset: 0x000BFA3E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x000C1854 File Offset: 0x000BFA54
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
