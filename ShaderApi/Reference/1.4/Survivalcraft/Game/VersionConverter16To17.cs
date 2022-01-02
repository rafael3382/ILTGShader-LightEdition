using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000351 RID: 849
	public class VersionConverter16To17 : VersionConverter
	{
		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060018EC RID: 6380 RVA: 0x000C2F14 File Offset: 0x000C1114
		public override string SourceVersion
		{
			get
			{
				return "1.6";
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060018ED RID: 6381 RVA: 0x000C2F1B File Offset: 0x000C111B
		public override string TargetVersion
		{
			get
			{
				return "1.7";
			}
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x000C2F22 File Offset: 0x000C1122
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x000C2F38 File Offset: 0x000C1138
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
