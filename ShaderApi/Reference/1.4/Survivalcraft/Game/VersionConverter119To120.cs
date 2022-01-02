using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000343 RID: 835
	public class VersionConverter119To120 : VersionConverter
	{
		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x0600189E RID: 6302 RVA: 0x000C10D8 File Offset: 0x000BF2D8
		public override string SourceVersion
		{
			get
			{
				return "1.19";
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x0600189F RID: 6303 RVA: 0x000C10DF File Offset: 0x000BF2DF
		public override string TargetVersion
		{
			get
			{
				return "1.20";
			}
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x000C10E6 File Offset: 0x000BF2E6
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x000C10FC File Offset: 0x000BF2FC
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
