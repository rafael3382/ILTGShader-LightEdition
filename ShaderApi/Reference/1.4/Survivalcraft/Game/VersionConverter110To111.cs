using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033A RID: 826
	public class VersionConverter110To111 : VersionConverter
	{
		// Token: 0x17000396 RID: 918
		// (get) Token: 0x0600186D RID: 6253 RVA: 0x000C091C File Offset: 0x000BEB1C
		public override string SourceVersion
		{
			get
			{
				return "1.10";
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x000C0923 File Offset: 0x000BEB23
		public override string TargetVersion
		{
			get
			{
				return "1.11";
			}
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000C092A File Offset: 0x000BEB2A
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000C0940 File Offset: 0x000BEB40
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
