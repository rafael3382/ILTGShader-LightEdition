using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033C RID: 828
	public class VersionConverter112To113 : VersionConverter
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001877 RID: 6263 RVA: 0x000C0A7C File Offset: 0x000BEC7C
		public override string SourceVersion
		{
			get
			{
				return "1.12";
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001878 RID: 6264 RVA: 0x000C0A83 File Offset: 0x000BEC83
		public override string TargetVersion
		{
			get
			{
				return "1.13";
			}
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000C0A8A File Offset: 0x000BEC8A
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ProcessNode(projectNode);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000C0AA4 File Offset: 0x000BECA4
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

		// Token: 0x0600187B RID: 6267 RVA: 0x000C0B28 File Offset: 0x000BED28
		public void ProcessNode(XElement node)
		{
			foreach (XAttribute attribute in node.Attributes())
			{
				this.ProcessAttribute(attribute);
			}
			foreach (XElement node2 in node.Elements())
			{
				this.ProcessNode(node2);
			}
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x000C0BB4 File Offset: 0x000BEDB4
		public void ProcessAttribute(XAttribute attribute)
		{
			if (attribute.Name == "Value" && attribute.Value == "Dangerous")
			{
				attribute.Value = "Normal";
			}
		}
	}
}
