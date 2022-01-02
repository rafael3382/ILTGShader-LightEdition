using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034E RID: 846
	public class VersionConverter130To20 : VersionConverter
	{
		// Token: 0x170003BE RID: 958
		// (get) Token: 0x060018DD RID: 6365 RVA: 0x000C2D04 File Offset: 0x000C0F04
		public override string SourceVersion
		{
			get
			{
				return "1.30";
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x060018DE RID: 6366 RVA: 0x000C2D0B File Offset: 0x000C0F0B
		public override string TargetVersion
		{
			get
			{
				return "2.0";
			}
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x000C2D12 File Offset: 0x000C0F12
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x000C2D28 File Offset: 0x000C0F28
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
