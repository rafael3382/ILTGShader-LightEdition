using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033F RID: 831
	public class VersionConverter115To116 : VersionConverter
	{
		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x0600188A RID: 6282 RVA: 0x000C0E18 File Offset: 0x000BF018
		public override string SourceVersion
		{
			get
			{
				return "1.15";
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600188B RID: 6283 RVA: 0x000C0E1F File Offset: 0x000BF01F
		public override string TargetVersion
		{
			get
			{
				return "1.16";
			}
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x000C0E26 File Offset: 0x000BF026
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x000C0E3C File Offset: 0x000BF03C
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
