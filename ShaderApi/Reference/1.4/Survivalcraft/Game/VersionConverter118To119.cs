using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000342 RID: 834
	public class VersionConverter118To119 : VersionConverter
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001899 RID: 6297 RVA: 0x000C1028 File Offset: 0x000BF228
		public override string SourceVersion
		{
			get
			{
				return "1.18";
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x000C102F File Offset: 0x000BF22F
		public override string TargetVersion
		{
			get
			{
				return "1.19";
			}
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x000C1036 File Offset: 0x000BF236
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x000C104C File Offset: 0x000BF24C
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
