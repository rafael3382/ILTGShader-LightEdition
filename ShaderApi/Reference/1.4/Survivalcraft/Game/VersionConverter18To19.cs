using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000353 RID: 851
	public class VersionConverter18To19 : VersionConverter
	{
		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x060018F6 RID: 6390 RVA: 0x000C3074 File Offset: 0x000C1274
		public override string SourceVersion
		{
			get
			{
				return "1.8";
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x060018F7 RID: 6391 RVA: 0x000C307B File Offset: 0x000C127B
		public override string TargetVersion
		{
			get
			{
				return "1.9";
			}
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x000C3082 File Offset: 0x000C1282
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x000C3098 File Offset: 0x000C1298
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
