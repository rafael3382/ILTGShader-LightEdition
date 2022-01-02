using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033B RID: 827
	public class VersionConverter111To112 : VersionConverter
	{
		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001872 RID: 6258 RVA: 0x000C09CC File Offset: 0x000BEBCC
		public override string SourceVersion
		{
			get
			{
				return "1.11";
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001873 RID: 6259 RVA: 0x000C09D3 File Offset: 0x000BEBD3
		public override string TargetVersion
		{
			get
			{
				return "1.12";
			}
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x000C09DA File Offset: 0x000BEBDA
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x000C09F0 File Offset: 0x000BEBF0
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
