using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034F RID: 847
	public class VersionConverter14To15 : VersionConverter
	{
		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x060018E2 RID: 6370 RVA: 0x000C2DB4 File Offset: 0x000C0FB4
		public override string SourceVersion
		{
			get
			{
				return "1.4";
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x060018E3 RID: 6371 RVA: 0x000C2DBB File Offset: 0x000C0FBB
		public override string TargetVersion
		{
			get
			{
				return "1.5";
			}
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x000C2DC2 File Offset: 0x000C0FC2
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x000C2DD8 File Offset: 0x000C0FD8
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
