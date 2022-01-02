using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034D RID: 845
	public class VersionConverter129To130 : VersionConverter
	{
		// Token: 0x170003BC RID: 956
		// (get) Token: 0x060018D8 RID: 6360 RVA: 0x000C2C54 File Offset: 0x000C0E54
		public override string SourceVersion
		{
			get
			{
				return "1.29";
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x060018D9 RID: 6361 RVA: 0x000C2C5B File Offset: 0x000C0E5B
		public override string TargetVersion
		{
			get
			{
				return "1.30";
			}
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x000C2C62 File Offset: 0x000C0E62
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x000C2C78 File Offset: 0x000C0E78
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
