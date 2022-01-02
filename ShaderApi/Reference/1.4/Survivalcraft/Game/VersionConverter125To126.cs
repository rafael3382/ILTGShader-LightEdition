using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000349 RID: 841
	public class VersionConverter125To126 : VersionConverter
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060018BC RID: 6332 RVA: 0x000C18E0 File Offset: 0x000BFAE0
		public override string SourceVersion
		{
			get
			{
				return "1.25";
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060018BD RID: 6333 RVA: 0x000C18E7 File Offset: 0x000BFAE7
		public override string TargetVersion
		{
			get
			{
				return "1.26";
			}
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x000C18EE File Offset: 0x000BFAEE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x000C1904 File Offset: 0x000BFB04
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
