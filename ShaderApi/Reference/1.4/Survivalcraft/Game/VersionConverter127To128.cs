using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034B RID: 843
	public class VersionConverter127To128 : VersionConverter
	{
		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x060018CA RID: 6346 RVA: 0x000C1D6E File Offset: 0x000BFF6E
		public override string SourceVersion
		{
			get
			{
				return "1.27";
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x060018CB RID: 6347 RVA: 0x000C1D75 File Offset: 0x000BFF75
		public override string TargetVersion
		{
			get
			{
				return "1.28";
			}
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x000C1D7C File Offset: 0x000BFF7C
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x000C1D90 File Offset: 0x000BFF90
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
