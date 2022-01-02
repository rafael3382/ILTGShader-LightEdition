using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033D RID: 829
	public class VersionConverter113To114 : VersionConverter
	{
		// Token: 0x1700039C RID: 924
		// (get) Token: 0x0600187E RID: 6270 RVA: 0x000C0BF2 File Offset: 0x000BEDF2
		public override string SourceVersion
		{
			get
			{
				return "1.13";
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x0600187F RID: 6271 RVA: 0x000C0BF9 File Offset: 0x000BEDF9
		public override string TargetVersion
		{
			get
			{
				return "1.14";
			}
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000C0C00 File Offset: 0x000BEE00
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ProcessNode(projectNode);
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x000C0C1C File Offset: 0x000BEE1C
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

		// Token: 0x06001882 RID: 6274 RVA: 0x000C0CA0 File Offset: 0x000BEEA0
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

		// Token: 0x06001883 RID: 6275 RVA: 0x000C0D2C File Offset: 0x000BEF2C
		public void ProcessAttribute(XAttribute attribute)
		{
			if (attribute.Name == "Value" && attribute.Value == "Dangerous")
			{
				attribute.Value = "Normal";
			}
		}
	}
}
