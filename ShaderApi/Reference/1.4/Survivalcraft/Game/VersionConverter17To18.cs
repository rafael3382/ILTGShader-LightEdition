using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000352 RID: 850
	public class VersionConverter17To18 : VersionConverter
	{
		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060018F1 RID: 6385 RVA: 0x000C2FC4 File Offset: 0x000C11C4
		public override string SourceVersion
		{
			get
			{
				return "1.7";
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x060018F2 RID: 6386 RVA: 0x000C2FCB File Offset: 0x000C11CB
		public override string TargetVersion
		{
			get
			{
				return "1.8";
			}
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x000C2FD2 File Offset: 0x000C11D2
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x000C2FE8 File Offset: 0x000C11E8
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
