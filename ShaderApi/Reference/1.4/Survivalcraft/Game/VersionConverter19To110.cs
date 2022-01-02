using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000354 RID: 852
	public class VersionConverter19To110 : VersionConverter
	{
		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060018FB RID: 6395 RVA: 0x000C3124 File Offset: 0x000C1324
		public override string SourceVersion
		{
			get
			{
				return "1.9";
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060018FC RID: 6396 RVA: 0x000C312B File Offset: 0x000C132B
		public override string TargetVersion
		{
			get
			{
				return "1.10";
			}
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x000C3132 File Offset: 0x000C1332
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018FE RID: 6398 RVA: 0x000C3148 File Offset: 0x000C1348
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
